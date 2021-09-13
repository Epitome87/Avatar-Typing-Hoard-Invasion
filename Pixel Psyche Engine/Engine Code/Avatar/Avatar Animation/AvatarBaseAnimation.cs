#region File Description
//-----------------------------------------------------------------------------
// AvatarBaseAnimation.cs
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
     /// <remarks>
     /// Defines a base Avatar Animation class which inherits from the IAvatarAnimation interface.
     /// This is much more than a Wrapper Class for AvatarAnimation, although it does use an AvatarAnimation
     /// object to Update and Draw a AvatarBaseAnimation. Basically, we will instanciate this type of object
     /// whenever we would normally want an AvatarAnimation object.
     /// 
     /// To render an Avatar Animation, we really only need a list of BoneTransforms and an AvatarExpression.
     /// By creating this AvatarBaseAnimation class, we can extend the Engine to allow for a wide variety of 
     /// animations, such as using the AvatarAnimationPreset animations, custom animations 
     /// (through our own .fbx files, thanks to CustomAvatarAnimationPlayer framework, multiple animations 
     /// (by combining 2 or more other animations), and blended animations (where we smoothen the transition from
     /// one animation to another.
     /// </remarks>
     public class AvatarBaseAnimation : IAvatarAnimation 
     {
          #region Fields

          // An array of bones is useful for initializing the Bone Transforms.
          protected Matrix[] avatarBones = new Matrix[AvatarRenderer.BoneCount];

          // Bone Transforms are REQUIRED for use with an AvatarRenderer.
          protected ReadOnlyCollection<Matrix> boneTransforms;

          // The current animation to play. Only used in the BaseAnimation class.
          private AvatarAnimation currentAnimation;


          // Fields to add more flexibility to animation-playing:
          private bool isAnimationFinished = false;

          #endregion
          
          #region Properties
          
          /// <summary>
          /// The current position of the bones.
          /// 
          /// We create our own separate from the Property provided by IAvatarAnimation
          /// so we can have more flexibility, as it returns the property as an IList
          /// rather than a restricted ReadOnlyCollection.
          /// 
          /// We also create this as a Virtual Property because each Animation type will
          /// require different logic (usually access to a specific animation) to return
          /// the list of bone transforms.
          /// </summary>
          public virtual IList<Matrix> CustomBoneTransforms
          {
               get
               {
                    return boneTransforms;
               }
          }

          /// <summary>
          /// The current position of the bones
          /// </summary>
          public ReadOnlyCollection<Matrix> BoneTransforms
          {
               get
               {
                    return boneTransforms;
               }

               //set { boneTransforms = value; } // Shouldn't allow this.
          }

          /// <summary>
          /// The current expression of the avatar in the animation
          /// We do not blend the expression and only use the current animations
          /// expression
          /// </summary>
          public AvatarExpression Expression
          {
               get
               {
                    return currentAnimation.Expression;
               }
          }

          /// <summary>
          /// The current position in the animation.
          /// 
          /// This is a Virtual Property because the Current Position logic varies.
          /// For a BaseAnimation, we just return AvatarAnimation.CurrentPosition.
          /// For a CustomAnimation, we just return CustomAnimationPlayer.CurrentPosition.
          /// For a MultipleAnimation, we just return the first sub-animations CurrentPosition.
          /// </summary>
          public virtual TimeSpan CurrentPosition
          {
               get
               {
                    return currentAnimation.CurrentPosition;
               }

               set
               {
                    currentAnimation.CurrentPosition = value;
               }
          }

          /// <summary>
          /// The length of the animation
          /// Uses the target animation while blending to it
          /// </summary>
          public virtual TimeSpan Length
          {
               get
               {
                    return currentAnimation.Length;
               }
          }

          /// <summary>
          /// Gets whether or not this BaseAnimation has finished playing.
          /// 
          /// (This means if the BaseAnimation's CurrentPosition has reached its end.)
          /// </summary>
          public virtual bool IsFinished
          {
               get { return isAnimationFinished; }
          }

          /// <summary>
          /// Return the underlying AvatarAnimation that this BaseAnimation uses.
          /// 
          /// Why? Could be useful in the future!
          /// </summary>
          public AvatarAnimation CurrentAnimation
          {
               get { return currentAnimation; }
               set { currentAnimation = value; }
          }

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor which creates a new Avatar Animation given the specified Preset.
          /// </summary>
          /// <param name="presetAnimation">The AvatarAnimationPreset that describes our Animation.</param>
          public AvatarBaseAnimation(AvatarAnimationPreset presetAnimation)
          {
               currentAnimation = new AvatarAnimation(presetAnimation);

               boneTransforms = currentAnimation.BoneTransforms;
          }

          /// <summary>
          /// Protected Constructor which initializes a blank list of Bone Transforms.
          /// 
          /// Should never be used directly to create an AvatarBaseAnimation object;
          /// it is merely called by deriving classes.
          /// </summary>
          public AvatarBaseAnimation() // Use to be protected
          {
               //boneTransforms = new ReadOnlyCollection<Matrix>(new List<Matrix>());

               // Or this, but probably not necessary? Seems more robust:
               boneTransforms = new ReadOnlyCollection<Matrix>(avatarBones);
          }

          #endregion

          #region Update

          public virtual void Update(TimeSpan elapsedAnimationTime)
          {
               currentAnimation.Update(elapsedAnimationTime, true);
          }

          public virtual void Update(GameTime elapsedAnimationTime)
          {
               currentAnimation.Update(elapsedAnimationTime.ElapsedGameTime, true);
          }

          /// <summary>
          /// Updates the animation and blends with the next animation if there is one.
          /// </summary>
          /// <param name="elapsedAnimationTime">Time since the last update</param>
          /// <param name="loop">Should the animation loop</param>
          public virtual void Update(TimeSpan elapsedAnimationTime, bool loop)
          {
               currentAnimation.Update(elapsedAnimationTime, loop);

               // Put code for setting isAnimationFinished here?

          }

          #endregion

          #region Play Animation - Not used yet.

          public virtual void Play()
          {

          }

          #endregion

          #region Draw

          public virtual void Draw(AvatarRenderer ar)
          {
               // Don't bother rendreing if the renderer isn't valid.
               if (ar == null)
                    return;

               // Don't bother rendering if the animation isn't valid.
               if (this.currentAnimation == null)
                    return;

               ar.Draw(this.CustomBoneTransforms, this.currentAnimation.Expression);//this.currentAnimation.BoneTransforms, this.currentAnimation.Expression);
          }

          #endregion
     }
}