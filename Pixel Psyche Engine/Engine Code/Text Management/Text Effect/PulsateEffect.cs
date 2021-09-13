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
     public class PulsateEffect : TextEffect
     {
          #region Fields

          private float elapsedTime;
          private float remaining;
          private float intensity;
          private float pulsatePeriod;

          private bool runInfinitely = false;

          #endregion

          #region Properties

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          /// <param name="gameTime">.</param>
          public PulsateEffect(float runTime, String message)
               : base(runTime)
          {
               elapsedTime = 0.0f;
               remaining = runTime;
               intensity = 1.0f;
               pulsatePeriod = 1.0f;

               if (runTime <= 0)
                    runInfinitely = true;
          }

          /// <summary>
          /// Constructor.
          /// </summary>
          /// <param name="gameTime">.</param>
          public PulsateEffect(float runTime, float intense, float period, String message)
               : base(runTime)
          {
               elapsedTime = 0.0f;
               remaining = runTime;
               intensity = intense;
               pulsatePeriod = period;

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
                         return;
               }

               // Pulsate the size of the selected menu entry.
               double time = gameTime.TotalGameTime.TotalSeconds;

               float pulsate = (float)Math.Sin( (time * 6) * pulsatePeriod ) + 1;

               float scale = 1 + pulsate * (intensity * 0.05f);
               textObj.Scale = scale;

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