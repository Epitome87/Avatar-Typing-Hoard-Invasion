#region File Description
//-----------------------------------------------------------------------------
// MoveInEffect.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using PixelEngine.Audio;
using PixelEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.Graphics;
#endregion

namespace PixelEngine.Text
{
     /// <summary>
     /// A type of TextEffect which draws TextObjects one String character at a time,
     /// causing the String to appear as if it is being typed in via keyboard.
     /// </summary>
     public class MoveInEffect : TextEffect
     {
          #region Fields

          private string effectText;
          private float elapsedTime;
          private float remaining;
          private Vector2 startingPosition;
          private Vector2 destinationPosition;

          #endregion

          #region Properties

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          /// <param name="gameTime">.</param>
          public MoveInEffect(float runTime, String message)
               : base(runTime)
          {
               this.effectText = message;
               elapsedTime = 0.0f;
               remaining = runTime;

               startingPosition = new Vector2(0, EngineCore.ScreenCenter.Y);
               destinationPosition = EngineCore.ScreenCenter;
          }

          /// <summary>
          /// Constructor.
          /// </summary>
          /// <param name="gameTime">.</param>
          public MoveInEffect(float runTime, String message, Vector2 startPosition, Vector2 endPosition)
               : base(runTime)
          {
               this.effectText = message;
               elapsedTime = 0.0f;
               remaining = runTime;

               startingPosition = startPosition;
               destinationPosition = endPosition;
               
          }

          #endregion

          #region Update

          /// <summary>
          /// Overriden Update method.
          /// </summary>
          /// <param name="gameTime">GameTime from the XNA Game instance.</param>
          public override void Update(GameTime gameTime, TextObject textObj)
          {
               elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

               Step = (RunTime == 0 ? 1 :
                      MathHelper.Clamp((float)((RunTime -
                          remaining) / RunTime), 0, 1));

               textObj.Position = startingPosition + Step * (destinationPosition - startingPosition);

               remaining -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
          }

          #endregion

          #region Draw

          /// <summary>
          /// Overriden Draw method.
          /// </summary>
          /// <param name="gameTime">GameTime from the XNA Game instance.</param>
          public override void Draw(GameTime gameTime, TextObject textObj)
          {
               SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

               MySpriteBatch.DrawString(textObj.Font, this.effectText, textObj.Position, textObj.Color,
                              textObj.Rotation, textObj.Origin, textObj.Scale, SpriteEffects.None, 0.0f);
          }

          #endregion
     }
}