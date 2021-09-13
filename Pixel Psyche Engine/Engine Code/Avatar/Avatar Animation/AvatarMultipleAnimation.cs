#region File Description
//-----------------------------------------------------------------------------
// AvatarMultipleAnimation.cs
//
// Copyright (C) Matt McGrath. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace PixelEngine.Avatars
{
     /// <summary>
     /// Defines what animation the avatar should be using
     /// </summary>
     public enum AnimationPlaybackMode { All, Animation1, Animation2 };

     /// <remarks>
     /// Defines a class which inherits from AvatarBaseAnimation.
     /// 
     /// This class defines an easy way to combine multiple AvatarBaseAnimations together.
     /// </remarks>
     public class AvatarMultipleAnimation : AvatarBaseAnimation
     {
          #region Fields

          // A 3D List to store the necessary list of bone ints, used for updating transforms.
          private List<List<List<int>>> allBonesList = new List<List<List<int>>>();

          // A List of all the animations that are used to form this MultipleAnimation.
          private List<AvatarBaseAnimation> animationsToCombine = new List<AvatarBaseAnimation>();

          private List<List<AvatarBone>> tempListOfBoneList = new List<List<AvatarBone>>();

          /* List of the final list of bone transforms.
           * The list will contain the necessary transforms from all the other sub-animations. */
          private List<Matrix> finalBoneTransforms = new List<Matrix>(AvatarRenderer.BoneCount);

          // Playback mode defines what animations should be playing
          private AnimationPlaybackMode animationPlaybackMode = AnimationPlaybackMode.All;

          // The AvatarRenderer.
          //private AvatarRenderer avatarRenderer;

          #endregion

          private List<bool> loopThisAnimation = new List<bool>();
          AvatarExpression expression;

          // THIS IS SLOPPY: FIX ITTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTT

          /// <summary>
          /// The current expression of the avatar in the animation
          /// We do not blend the expression and only use the current animations
          /// expression
          /// </summary>
          public new AvatarExpression Expression
          {
               get
               {
                    return expression;
               }

               set { expression = value; }
          }

          #region MultipleAnimation-Specific Properties

          /// <summary>
          /// Gets the number of animations being used to form this animation.
          /// </summary>
          public int NumberOfAnimations
          {
               get 
               {
                    if (animationsToCombine != null)
                         return animationsToCombine.Count;

                    else
                         return 0;
               }
          }

          /// <summary>
          /// Returns the final bone transforms; that is the bone transforms
          /// after all other bone transforms of the sub-animations have been
          /// appropriately combined.
          /// </summary>
          public IList<Matrix> FinalBoneTransforms
          {
               get { return finalBoneTransforms; }
          }

          /// <summary>
          /// Gets the List of all AvatarBaseAnimations being used to form
          /// the final, multiple animation.
          /// </summary>
          public List<AvatarBaseAnimation> Animations
          {
               get { return animationsToCombine; }
          }

          /// <summary>
          /// Gets the List of List of AvatarBones being used to specify
          /// what AvatarBones each sub-animation is using.
          /// 
          /// This is used privately for updating.
          /// </summary>
          public List<List<AvatarBone>> TempListOfBoneList
          {
               get { return tempListOfBoneList; }
               set { tempListOfBoneList = value; }
          }

          /// <summary>
          /// Gets whether or not this BaseAnimation has finished playing.
          /// 
          /// (This means if the BaseAnimation's CurrentPosition has reached its end.)
          /// </summary>
          public bool IsThisAnimationFinished(int animationIndex)
          {
               if (animationsToCombine != null)
               {
                    if (animationsToCombine[animationIndex] != null)
                    {
                         return animationsToCombine[animationIndex].IsFinished;
                    }

                    return false;
               }

               else
                    return false;
          }

          #endregion

          #region Overridden Properties

          /// <summary>
          /// The current position of the bones.
          /// 
          /// We override this because we ant to return the final bone transforms list,
          /// rather than one belonging to an individual sub-animation.
          /// </summary>
          public override IList<Matrix> CustomBoneTransforms
          {
               get
               {
                    return finalBoneTransforms;
               }
          }

          /// <summary>
          /// The current position in the animation.
          /// 
          /// When getting this value, only the primary sub-animation's CurrentPosition
          /// is returned.
          /// 
          /// When setting this to a value, all sub-animations have their CurrentPosition
          /// property set to the same value.
          /// </summary>
          public override TimeSpan CurrentPosition
          {
               get
               {
                    if (animationsToCombine != null && animationsToCombine[0] != null)
                    {
                         return this.animationsToCombine[0].CurrentPosition;
                    }

                    else
                    {
                         return new TimeSpan();
                    }
               }

               set
               {
                    foreach (AvatarBaseAnimation anim in animationsToCombine)
                    {
                         if (anim != null)
                         {
                              anim.CurrentPosition = value;
                         }

                         else
                         {
                              // Do nothing.
                         }
                    }
               }
          }

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor which combines a List of AvatarBaseAnimations, as specified by the bones parameter.
          /// </summary>
          /// <param name="ar">The AvatarRenderer to do the rendering.</param>
          /// <param name="animations">A List of animations, with the first entry being the "primary" animation.</param>
          /// <param name="bones">
          /// A List of AvatarBone Lists. The 1st List corresponds to the 1st animation, the 2nd List to the 2nd animation, and so on.
          /// Example: 1st List contains 2 AvatarBone Lists for AvatarBone.LeftArm and AvatarBone.RightArm; this means these two bones
          /// are to get their animation from the 1st AvatarBaseAnimation specified in the "animations" parameter, and so on.
          /// </param>
          public AvatarMultipleAnimation(AvatarRenderer ar, List<AvatarBaseAnimation> animations, List<List<AvatarBone>> bones)
          {
               //avatarRenderer = ar;
               
               // This is new and temp.
               tempListOfBoneList = bones;

               foreach (AvatarBaseAnimation animation in animations)
               {
                    animationsToCombine.Add(animation);
               }

               for (int i = 0; i < AvatarRenderer.BoneCount; ++i)
               {
                    finalBoneTransforms.Add(Matrix.Identity);
               }

               for (int i = 0; i < bones.Count; i++)
               {
                    allBonesList.Add(new List<List<int>>());

                    foreach (AvatarBone avatarBone in bones[i])
                    {
                        allBonesList[i].Add(FindInfluencedBones(avatarBone, ar.ParentBones));
                    }
               }
          }

          /// <summary>
          /// Constructor which combines a List of AvatarBaseAnimations, as specified by the bones parameter.
          /// </summary>
          /// <param name="ar">The AvatarRenderer to do the rendering.</param>
          /// <param name="animations">A List of animations, with the first entry being the "primary" animation.</param>
          /// <param name="bones">
          /// A List of AvatarBone Lists. The 1st List corresponds to the 1st animation, the 2nd List to the 2nd animation, and so on.
          /// Example: 1st List contains 2 AvatarBone Lists for AvatarBone.LeftArm and AvatarBone.RightArm; this means these two bones
          /// are to get their animation from the 1st AvatarBaseAnimation specified in the "animations" parameter, and so on.
          /// </param>
          public AvatarMultipleAnimation(List<bool> loopAnims, AvatarRenderer ar, List<AvatarBaseAnimation> animations, List<List<AvatarBone>> bones)
          {
               //avatarRenderer = ar;
               
               // This is new and temp.
               tempListOfBoneList = bones;

               foreach (AvatarBaseAnimation animation in animations)
               {
                    animationsToCombine.Add(animation);
               }

               for (int i = 0; i < AvatarRenderer.BoneCount; ++i)
               {
                    finalBoneTransforms.Add(Matrix.Identity);
               }

               for (int i = 0; i < bones.Count; i++)
               {
                    allBonesList.Add(new List<List<int>>());

                    foreach (AvatarBone avatarBone in bones[i])
                    {
                        allBonesList[i].Add(FindInfluencedBones(avatarBone, ar.ParentBones));
                    }
               }

       
               for (int i = 0; i < animations.Count; i++)
               {
                    loopThisAnimation.Add(loopAnims[i]);
               }
          }

          #endregion

          #region Update

          /// <summary>
          /// Updates the AvatarMultipleAnimation by calling the Update for each of the 
          /// AvatarBaseAnimation that compose it. Finally, it calls a private helper
          /// to update the final Bone Transforms, giving us our final animation.
          /// </summary>
          /// <param name="elapsedAnimationTime">Time since the last update.</param>
          /// <param name="loop">Should the animation loop?</param>
          public override void Update(TimeSpan elapsedAnimationTime, bool loop)
          {
               // Update the current animation and world space bones.
               //if (avatarRenderer.State == AvatarRendererState.Ready)
               {
                    /*
                    foreach (AvatarBaseAnimation animation in animationsToCombine)
                    {
                         animation.Update(elapsedAnimationTime, loop);
                    }

                    UpdateTransforms();
                    */
                    for (int i = 0; i < animationsToCombine.Count; i++)
                    {
                         if (i < this.loopThisAnimation.Count)
                         {
                              animationsToCombine[i].Update(elapsedAnimationTime, this.loopThisAnimation[i]);
                         }

                         else
                              animationsToCombine[i].Update(elapsedAnimationTime, loop);
                    }

                    UpdateTransforms();
               }
          }

          /// <summary>
          /// Updates the AvatarMultipleAnimation by calling the Update for each of the 
          /// AvatarBaseAnimation that compose it. Finally, it calls a private helper
          /// to update the final Bone Transforms, giving us our final animation.
          /// </summary>
          /// <param name="gameTime">Privately uses the ElapsedGameTime property from this.</param>
          public override void Update(GameTime gameTime)
          {
               // Update the current animation and world space bones
               //if (avatarRenderer.State == AvatarRendererState.Ready)
               {
                    foreach (AvatarBaseAnimation animation in animationsToCombine)
                    {
                         animation.Update(gameTime.ElapsedGameTime, true);
                    }

                    UpdateTransforms();
               }
          }

          /// <summary>
          /// Updates the AvatarMultipleAnimation by calling the Update for each of the 
          /// AvatarBaseAnimation that compose it. Finally, it calls a private helper
          /// to update the final Bone Transforms, giving us our final animation.
          /// </summary>
          /// <param name="elapsedAnimationTime">Time since the last update.</param>
          public override void Update(TimeSpan elapsedAnimationTime)
          {
               // Update the current animation and world space bones
               //if (avatarRenderer.State == AvatarRendererState.Ready)
               {
                    foreach (AvatarBaseAnimation animation in animationsToCombine)
                    {
                         animation.Update(elapsedAnimationTime, true);
                    }

                    UpdateTransforms();
               }
          }

          #endregion

          #region Draw

          AvatarExpression penis = new AvatarExpression();
          public override void Draw(AvatarRenderer ar)
          {
               if (ar == null)
                    return;

               ar.Draw(finalBoneTransforms, penis);//animationsToCombine[0].Expression);
          }

          public void Draw(AvatarRenderer ar, AvatarExpression anExpression)
          {
               if (ar == null)
                    return;

               ar.Draw(finalBoneTransforms, anExpression);
          }

          #endregion

          #region Private Helpers - Updating Transforms and Bones

          /// <summary>
          /// Combines the transforms of the clap and wave animations
          /// </summary>
          private void UpdateTransforms()
          {
               List<IList<Matrix>> listOfAnimationTransforms = new List<IList<Matrix>>();

               foreach (AvatarBaseAnimation animation in animationsToCombine)
               {
                    listOfAnimationTransforms.Add(animation.CustomBoneTransforms);
               }

               // Check to see if we are playing both of the animations 
               if (animationPlaybackMode == AnimationPlaybackMode.All)
               {
                    // Copy the main (first) animation's transforms to the final list of transforms.
                    for (int i = 0; i < finalBoneTransforms.Count; i++)
                    {
                         finalBoneTransforms[i] = listOfAnimationTransforms[0][i];
                    }

                    foreach (IList<Matrix> animationTransforms in listOfAnimationTransforms)
                    {
                         foreach (List<List<int>> listOfListOfBones in allBonesList)
                         {
                              foreach (List<int> listOfBones in listOfListOfBones)
                              {
                                   foreach (int boneInts in listOfBones)
                                   {
                                        finalBoneTransforms[boneInts] = animationTransforms[boneInts];
                                   }
                              }
                         }
                    }
               }
          }

          /// <summary>
          /// Creates a list of bone index values for the given avatar bone 
          /// and its children.
          /// </summary>
          /// <param name="avatarBone">The root bone to start search</param>
          /// <param name="parentBones">List of parent bones from the avatar 
          /// renderer</param>
          /// <returns></returns>
          List<int> FindInfluencedBones(AvatarBone avatarBone, ReadOnlyCollection<int> parentBones)
          {
               // New list of bones that will be influenced
               List<int> influencedList = new List<int>();

               // Add the first bone
               influencedList.Add((int)avatarBone);

               // Start searching after the first bone
               int currentBoneID = influencedList[0] + 1;

               // Loop until we are done with all of the bones
               while (currentBoneID < parentBones.Count)
               {
                    // Check to see if the current bone is a child of any of the 
                    // previous bones we have found
                    if (influencedList.Contains(parentBones[currentBoneID]))
                    {
                         // Add the bone to the influenced list
                         influencedList.Add(currentBoneID);
                    }

                    // Move to the next bone
                    currentBoneID++;
               }

               return influencedList;
          }

          #endregion
     }
}