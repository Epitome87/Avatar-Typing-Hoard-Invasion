#region File Description
//-----------------------------------------------------------------------------
// MirroredEffect.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.Graphics;
using PixelEngine.Screen;
#endregion

namespace PixelEngine.Text
{
     /// <summary>
     /// A type of TextEffect which draws TextObjects with a pulsating effect.
     /// That is, the text pulsates (using a Sine function) with a given Intensity (scale)
     /// and Period (how many times per second a complete cycle is made).
     /// </summary>
     public class MirroredEffect : TextEffect
     {
          #region Fields

          private float elapsedTime;
          private float remaining;
          private float rotation = 45f;

          private bool runInfinitely = false;

          #endregion

          #region Properties

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          /// <param name="gameTime">.</param>
          public MirroredEffect(float runTime, String message)
               : base(runTime)
          {
               elapsedTime = 0.0f;
               remaining = runTime;

               if (runTime <= 0)
                    runInfinitely = true;
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
                      MathHelper.Clamp((float)((RunTime - remaining) / RunTime), 0, 1));

               if (!runInfinitely)
               {
                    if (Step >= 1)
                    {
                         return;
                    }
               }

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

               // Draw the original text.
               MySpriteBatch.DrawString(textObj.Font, textObj.Text, textObj.Position, textObj.Color,
               textObj.Rotation, textObj.Origin, textObj.Scale, SpriteEffects.None, 0.0f);

               // Draw the mirrored text.
               MySpriteBatch.DrawString(textObj.Font, textObj.Text, new Vector2(textObj.Position.X, textObj.Position.Y + (textObj.Scale * (textObj.Font.LineSpacing))), textObj.Color,
                              textObj.Rotation, textObj.Origin, textObj.Scale, SpriteEffects.FlipVertically, 0.0f);
          }

          #endregion
     }
}