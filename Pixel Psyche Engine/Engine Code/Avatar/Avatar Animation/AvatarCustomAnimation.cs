#region File Description
//-----------------------------------------------------------------------------
// AvatarCustomAnimation.cs
//
// Copyright (C) Matt McGrath. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CustomAvatarAnimation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using PixelEngine.Screen;
#endregion

namespace PixelEngine.Avatars
{
     public class AvatarCustomAnimation : AvatarBaseAnimation
     {
          #region Fields

          CustomAvatarAnimationPlayer animation;
          AvatarExpression expression;

          #endregion

          #region Overriden Properties

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

          /// <summary>
          /// The current position in the animation
          /// Uses the target animation while blending to it
          /// </summary>
          public override TimeSpan CurrentPosition
          {
               get
               {
                    return animation.CurrentPosition;
               }

               set
               {
                    animation.CurrentPosition = value;
               }
          }

          /// <summary>
          /// The current position of the bones.
          /// </summary>
          public override IList<Matrix> CustomBoneTransforms
          {
               get
               {
                    return animation.BoneTransforms;
               }
          }

          /// <summary>
          /// The length of the animation.
          /// </summary>
          public override TimeSpan Length
          {
               get
               {
                    return animation.Length;
               }
          }

          public override bool IsFinished
          {
               get
               {
                    return animation.IsFinished;
               }
          }

          #endregion

          #region Initialization

          public AvatarCustomAnimation(string animationFileName)
          {
               CustomAvatarAnimationData animationData = 
                    ScreenManager.Game.Content.Load<CustomAvatarAnimationData>(animationFileName);

               animation = new CustomAvatarAnimationPlayer(animationData);
          }

          public AvatarCustomAnimation(CustomAvatarAnimationData customAvatarAnimationData)
          {
               animation = new CustomAvatarAnimationPlayer(customAvatarAnimationData);
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
               // Update the current animation and world space bones
               //if (avatarRenderer.State == AvatarRendererState.Ready)
               {
                    animation.Update(elapsedAnimationTime, loop);
               }
          }

          #endregion

          #region Draw

          public override void Draw(AvatarRenderer ar)
          {
               if (ar == null)
                    return;

               ar.Draw(this.CustomBoneTransforms, this.Expression);
          }

          #endregion
     }
}
