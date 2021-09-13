#region File Description
//-----------------------------------------------------------------------------
// ReadingEffect.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.Graphics;
using PixelEngine.Screen;
#endregion

namespace PixelEngine.Text
{
     /// <summary>
     /// A type of TextEffect which draws TextObjects one String character at a time,
     /// causing the String to appear as if it is being typed in via keyboard.
     /// </summary>
     public class BounceEffect : TextEffect
     {
          #region Fields

          private string effectText;
          private float elapsedTime;
          private float remaining;

          private float timeToRise;
          private float timeToFall;

          private float bouncePeriod;
          private float bounceHeight;

          private List<TextObject> characterList = new List<TextObject>();

          #endregion

          #region Properties

          public float TimeToRise
          {
               get { return timeToRise; }
               set { timeToFall = value; }
          }

          public float TimeToFall
          {
               get { return timeToFall; }
               set { timeToFall = value; }
          }

          public float BounceHeight
          {
               get { return bounceHeight; }
               set { bounceHeight = value; }
          }

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          /// <param name="gameTime">.</param>
          public BounceEffect(float runTime, String message)
               : base(runTime)
          {
               this.effectText = message;
               elapsedTime = 0.0f;
               remaining = runTime;

               timeToRise = 3000.0f;
               timeToFall = 2000.0f;

               bouncePeriod = 0.20f;
               bounceHeight = 100.0f;

               //for (int i = 0; i < message.Length; i++)
               {
                 //   characterList.Add(new TextObject(message.Substring(i, 1), new Vector2()));
               }
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

               //this.effectText = textObj.Text.Substring(0, (int)(Step * textObj.Text.Length));

               // Pulsate the size of the selected menu entry.
               double time = gameTime.TotalGameTime.TotalSeconds;

               float bounce = (float)Math.Sin((time * 6) * bouncePeriod) + 1;

               float textPosition = 1 + bounce * (bounceHeight);
               textObj.Position = new Vector2(textObj.Position.X, textPosition);

               remaining -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;

               /*
                * foreach (TextObject textObj in textObjList)
                * Bounce(textObj);
                * */

               /*
               int counter = 0;

               foreach (TextObject charObj in characterList)
               {
                    charObj.Position = new Vector2(charObj.Position.X, charObj.Position.Y + (counter * 10));
                    counter++;
               }
                * */
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

               /*
               foreach (TextObject charObj in characterList)
               {
                    MySpriteBatch.DrawString(charObj.Font, this.effectText, charObj.Position, charObj.Color, charObj.Rotation, charObj.Origin, charObj.Scale, SpriteEffects.None, 0.0f);
               }
                * */
               
               MySpriteBatch.DrawString(textObj.Font, this.effectText, textObj.Position, textObj.Color,
                              textObj.Rotation, textObj.Origin, textObj.Scale, SpriteEffects.None, 0.0f);
          }

          #endregion

          #region Helper Methods

          public void Bounce(TextObject textObj)
          {
               textObj.Position = new Vector2(textObj.Position.X, textObj.Position.Y + 0.01f);
          }

          #endregion
     }
}