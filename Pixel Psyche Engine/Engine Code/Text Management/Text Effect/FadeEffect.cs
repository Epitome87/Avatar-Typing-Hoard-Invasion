#region File Description
//-----------------------------------------------------------------------------
// FadeEffect.cs
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
     /// A type of TextEffect which draws TextObjects with a fade effect.
     /// That is, the text alternates fading in and out (using a Sine function) with a given Intensity (scale)
     /// and Period (how many times per second a complete cycle is made).
     /// </summary>
     public class FadeEffect : TextEffect
     {
          #region Fields

          private float elapsedTime;
          private float remaining;
          private float minIntensity;
          private float maxIntensity;
          private float pulsatePeriod;

          private bool runInfinitely = false;

          private float colorWithFade;

          #endregion

          #region Properties

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          /// <param name="gameTime">.</param>
          public FadeEffect(float runTime, String message)
               : base(runTime)
          {
               elapsedTime = 0.0f;
               remaining = runTime;

               minIntensity = 0.0f;
               maxIntensity = 1.0f;

               pulsatePeriod = 1.0f;

               if (runTime <= 0)
                    runInfinitely = true;
          }

          /// <summary>
          /// Constructor.
          /// </summary>
          /// <param name="gameTime">.</param>
          public FadeEffect(float runTime, float minimumAlpha, float maximumAlpha, float period, String message)
               : base(runTime)
          {
               elapsedTime = 0.0f;
               remaining = runTime;

               minIntensity = minimumAlpha;
               maxIntensity = maximumAlpha;

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

               float pulsate = (float)Math.Sin((time * 6) * pulsatePeriod) + 1;

               // divide by 2? Then range is 0-1
               pulsate /= 2;

               // ...But we want a range of minimum-maximum...
               float intensity = pulsate * (maxIntensity - minIntensity) + minIntensity; // 0-1? (but with (max - min) / pulsate, the period is slower due to the new limited range.)
               
               //scale = MathHelper.Clamp(scale, minIntensity, maxIntensity); 
               // THIS WORKS BUT scale will sit at minIntensity and maxIntensity until sin cicles back between a # higher and lower than those, respectively.
               // If Min is 0.5, for example, it will remain at 0.5 during the period where sin = 0 thru 0.5. This means half the time will be spent at 0.5 intensity, 
               // leaving less time to spend on intermediate values (giving the effect a smoother transition).

               //textObj.Color = Color.Blue * intensity;
               colorWithFade = intensity;


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

               MySpriteBatch.DrawString(textObj.Font, textObj.Text, textObj.Position, textObj.Color * colorWithFade,
                              textObj.Rotation, textObj.Origin, textObj.Scale, SpriteEffects.None, 0.0f);
          }

          #endregion
     }
}