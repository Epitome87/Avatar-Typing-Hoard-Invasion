#region File Description
//-----------------------------------------------------------------------------
// ReadingEffect.cs
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
     public class ReadingEffect : TextEffect
     {
          #region Fields

          private string effectText;
          private float elapsedTime;
          private float remaining;

          #endregion

          #region Properties

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          /// <param name="gameTime">.</param>
          public ReadingEffect(float runTime, String message)
               : base(runTime)
          {
               this.effectText = message;
               elapsedTime = 0.0f;
               remaining = runTime;
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

               this.effectText = textObj.Text.Substring(0, (int)(Step * textObj.Text.Length));

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