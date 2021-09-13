#region File Description
//-----------------------------------------------------------------------------
// PulsateEffect.cs
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
     public class ZoomInEffect : TextEffect
     {
          #region Fields

          private float elapsedTime;
          private float remaining;
          private float intensity;

          private bool runInfinitely = false;

          #endregion

          #region Properties

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          /// <param name="gameTime">.</param>
          public ZoomInEffect(float runTime, String message)
               : base(runTime)
          {
               elapsedTime = 0.0f;
               remaining = runTime;
               intensity = 1.0f;

               if (runTime <= 0)
                    runInfinitely = true;
          }

          /// <summary>
          /// 
          /// </summary>
          /// <param name="runTime"></param>
          /// <param name="intense">The factor by which we should "Zoom" (scale the text). Use -Intensity for "Zoom Out" effect.</param>
          /// <param name="period"></param>
          /// <param name="message"></param>
          public ZoomInEffect(float runTime, float intense, float period, String message)
               : base(runTime)
          {
               elapsedTime = 0.0f;
               remaining = runTime;
               intensity = intense;

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

               float scale;

               if (!runInfinitely)
               {
                    if (Step >= 1)
                    {
                         textObj.Scale = 0;
                         return;
                    }
               }

               if (intensity > 0)
               {
                    scale = Step * intensity;
                    textObj.Scale = scale;
               }

               else
               {
                    scale = Step * intensity;
                    textObj.Scale = 1f / Math.Abs(scale);
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

               MySpriteBatch.DrawString(textObj.Font, textObj.Text, textObj.Position, textObj.Color,
                              textObj.Rotation, textObj.Origin, textObj.Scale, SpriteEffects.None, 0.0f);
          }

          #endregion
     }
}