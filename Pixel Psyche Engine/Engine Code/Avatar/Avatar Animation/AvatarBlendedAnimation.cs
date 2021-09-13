#region File Description
//-----------------------------------------------------------------------------
// AvatarBlendedAnimation.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using CustomAvatarAnimation;
#endregion

namespace PixelEngine.Avatars
{
     public class AvatarBlendedAnimation : AvatarBaseAnimation//IAvatarAnimation
     {
          #region Fields

         

          // The current animation to play and blend from
          private AvatarAnimation currentAnimation;
          
          // The next animation to play and blend target
          private AvatarAnimation targetAnimation;

          // The amount of time to take to blend between animations
          private TimeSpan blendTotalTime = new TimeSpan(0, 0, 0, 0, 250);
          
          // The current time in the blend cycle
          private TimeSpan blendCurrentTime;

          // The current animation to play and blend from.
          private CustomAvatarAnimationPlayer currentCustomAnimation;

          // The next custom animation to play and blend target
          private CustomAvatarAnimationPlayer targetCustomAnimation;

          #endregion

          #region Properties

          /// <summary>
          /// The current position of the bones
          /// </summary>
          public new ReadOnlyCollection<Matrix> BoneTransforms
          {
               get
               {
                    return boneTransforms;
               }
          }

          /// <summary>
          /// The current expression of the avatar in the animation
          /// We do not blend the expression and only use the current animations
          /// expression
          /// </summary>
          public new AvatarExpression Expression
          {
               get
               {
                    return currentAnimation.Expression;
               }
          }

          /// <summary>
          /// The current position in the animation
          /// Uses the target animation while blending to it
          /// </summary>
          public new TimeSpan CurrentPosition
          {
               get
               {
                    if (targetAnimation != null)
                         return targetAnimation.CurrentPosition;
                    else
                         return currentAnimation.CurrentPosition;
               }
               set
               {
                    if (targetAnimation != null)
                         targetAnimation.CurrentPosition = value;
                    else
                         currentAnimation.CurrentPosition = value;
               }
          }

          /// <summary>
          /// The length of the animation
          /// Uses the target animation while blending to it
          /// </summary>
          public new TimeSpan Length
          {
               get
               {
                    if (targetAnimation != null)
                         return targetAnimation.Length;
                    else
                         return currentAnimation.Length;
               }
          }

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor
          /// </summary>
          /// <param name="startingAnimation">The first animation to play</param>
          public AvatarBlendedAnimation(AvatarAnimation startingAnimation)
          {
               // Set the first animation
               currentAnimation = startingAnimation;

               // Create collection we will use for blended transfroms
               boneTransforms = new ReadOnlyCollection<Matrix>(avatarBones);
          }

          /// <summary>
          /// Constructor
          /// </summary>
          /// <param name="startingAnimation">The first animation to play</param>
          public AvatarBlendedAnimation(CustomAvatarAnimationPlayer startingCustomAnimation)
          {
               // Set the first animation
               currentCustomAnimation = startingCustomAnimation;

               // Create collection we will use for blended transfroms
               boneTransforms = new ReadOnlyCollection<Matrix>(avatarBones);
          }

          #endregion

          #region Update

          /// <summary>
          /// Updates the animation and blends with the next animation if there is one.
          /// </summary>
          /// <param name="elapsedAnimationTime">Time since the last update</param>
          /// <param name="loop">Should the animation loop</param>
          public override void Update(TimeSpan elapsedAnimationTime, bool loop)
          {
               // Update the current animation
               currentAnimation.Update(elapsedAnimationTime, loop);

               // Check to see if we need to blend animations or not
               if (targetAnimation == null)
               {
                    // We are not blending so copy the current animations bone transforms
                    currentAnimation.BoneTransforms.CopyTo(avatarBones, 0);
               }
               else
               {
                    // Update the target animation
                    targetAnimation.Update(elapsedAnimationTime, loop);

                    ReadOnlyCollection<Matrix> currentBoneTransforms =
                                                             currentAnimation.BoneTransforms;
                    ReadOnlyCollection<Matrix> targetBoneTransforms =
                                                              targetAnimation.BoneTransforms;

                    // Update the current blended time
                    blendCurrentTime += elapsedAnimationTime;

                    // Calculate blend factor
                    float blendFactor = (float)(blendCurrentTime.TotalSeconds /
                                                blendTotalTime.TotalSeconds);


                    // Check to see if we are done blending
                    if (blendFactor >= 1.0f)
                    {
                         // Set the current animtion and remove the target animation
                         currentAnimation = targetAnimation;
                         targetAnimation = null;
                         blendFactor = 1.0f;
                    }

                    // Variables to hold the rotations and translations for the 
                    // current, target, and blended transforms
                    Quaternion currentRotation, targetRotation, finalRotation;
                    Vector3 currentTranslation, targetTranslation, finalTranslation;

                    // Loop all of the bones in the avatar
                    for (int i = 0; i < avatarBones.Length; ++i)
                    {
                         // Find the rotation of the current and target bone transforms
                         currentRotation =
                                Quaternion.CreateFromRotationMatrix(currentBoneTransforms[i]);
                         targetRotation =
                                 Quaternion.CreateFromRotationMatrix(targetBoneTransforms[i]);

                         // Calculate the blended rotation from the current to the target
                         Quaternion.Slerp(ref currentRotation, ref targetRotation,
                                                              blendFactor, out finalRotation);

                         // Find the translation of the current and target bone transforms
                         currentTranslation = currentBoneTransforms[i].Translation;
                         targetTranslation = targetBoneTransforms[i].Translation;

                         // Calculate the blended translation from the current to the target
                         Vector3.Lerp(ref currentTranslation, ref targetTranslation,
                                                           blendFactor, out finalTranslation);

                         // Build the final bone transform 
                         avatarBones[i] = Matrix.CreateFromQuaternion(finalRotation) *
                                          Matrix.CreateTranslation(finalTranslation);
                    }
               }
          }

          /// <summary>
          /// Updates the animation and blends with the next animation if there is one.
          /// </summary>
          /// <param name="elapsedAnimationTime">Time since the last update</param>
          /// <param name="loop">Should the animation loop</param>
          public void UpdateCustomAnimation(TimeSpan elapsedAnimationTime, bool loop)
          {
               // Update the current animation
               currentCustomAnimation.Update(elapsedAnimationTime, loop);

               // Check to see if we need to blend animations or not
               if (targetCustomAnimation == null)
               {
                    // We are not blending so copy the current animations bone transforms
                    currentCustomAnimation.BoneTransforms.CopyTo(avatarBones, 0);
               }
               else
               {
                    // Update the target animation
                    targetCustomAnimation.Update(elapsedAnimationTime, loop);

                    ReadOnlyCollection<Matrix> currentBoneTransforms =
                                                             (ReadOnlyCollection<Matrix>)currentCustomAnimation.BoneTransforms;
                    ReadOnlyCollection<Matrix> targetBoneTransforms =
                                                               (ReadOnlyCollection<Matrix>)targetCustomAnimation.BoneTransforms;

                    // Update the current blended time
                    blendCurrentTime += elapsedAnimationTime;

                    // Calculate blend factor
                    float blendFactor = (float)(blendCurrentTime.TotalSeconds /
                                                blendTotalTime.TotalSeconds);


                    // Check to see if we are done blending
                    if (blendFactor >= 1.0f)
                    {
                         // Set the current animtion and remove the target animation
                         currentCustomAnimation = targetCustomAnimation;
                         targetCustomAnimation = null;
                         blendFactor = 1.0f;
                    }

                    // Variables to hold the rotations and translations for the 
                    // current, target, and blended transforms
                    Quaternion currentRotation, targetRotation, finalRotation;
                    Vector3 currentTranslation, targetTranslation, finalTranslation;

                    // Loop all of the bones in the avatar
                    for (int i = 0; i < avatarBones.Length; ++i)
                    {
                         // Find the rotation of the current and target bone transforms
                         currentRotation =
                                Quaternion.CreateFromRotationMatrix(currentBoneTransforms[i]);
                         targetRotation =
                                 Quaternion.CreateFromRotationMatrix(targetBoneTransforms[i]);

                         // Calculate the blended rotation from the current to the target
                         Quaternion.Slerp(ref currentRotation, ref targetRotation,
                                                              blendFactor, out finalRotation);

                         // Find the translation of the current and target bone transforms
                         currentTranslation = currentBoneTransforms[i].Translation;
                         targetTranslation = targetBoneTransforms[i].Translation;

                         // Calculate the blended translation from the current to the target
                         Vector3.Lerp(ref currentTranslation, ref targetTranslation,
                                                           blendFactor, out finalTranslation);

                         // Build the final bone transform 
                         avatarBones[i] = Matrix.CreateFromQuaternion(finalRotation) *
                                          Matrix.CreateTranslation(finalTranslation);
                    }
               }
          }

          #endregion

          #region Play

          /// <summary>
          /// Plays a new animation and blends from the current animation.
          /// </summary>
          /// <param name="nextAnimation">Next animation to play</param>
          public void Play(AvatarAnimation nextAnimation)
          {
               // Check to make sure we are not already playing the animation passed in
               if (currentAnimation == nextAnimation)
                    return;

               // Set the next animation
               targetAnimation = nextAnimation;

               // Reset the animation to the start
               targetAnimation.CurrentPosition = TimeSpan.Zero;

               // Reset the blend current time to zero
               blendCurrentTime = TimeSpan.Zero;
          }

          public bool UsingCustomAnimation;

          /// <summary>
          /// Plays a new animation and blends from the current animation.
          /// </summary>
          /// <param name="nextAnimation">Next animation to play</param>
          public void PlayCustomAnimation(CustomAvatarAnimationPlayer nextAnimation)
          {
               // Check to make sure we are not already playing the animation passed in
               //if (currentAnimation == nextAnimation)
               //     return;

               // Set the next animation
               targetCustomAnimation = nextAnimation;

               // Reset the animation to the start
               targetCustomAnimation.CurrentPosition = TimeSpan.Zero;

               // Reset the blend current time to zero
               blendCurrentTime = TimeSpan.Zero;

               UsingCustomAnimation = true;
          }

          #endregion
     }
}