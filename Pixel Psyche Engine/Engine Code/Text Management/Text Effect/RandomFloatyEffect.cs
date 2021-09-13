#region File Description
//-----------------------------------------------------------------------------
// RandomFloatyEffect.cs
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
     /// <remarks>
     /// A type of TextEffect which draws TextObjects one String character at a time,
     /// causing the String to appear as if it is being typed in via keyboard.
     /// </remarks>
     public class RandomFloatyEffect : TextEffect
     {
          #region Fields

          private string effectText;
          private float elapsedTime;
          private float remaining;

          List<char> characterList = new List<char>();
          List<Vector2> characterPositionList = new List<Vector2>();
          List<float> characterRotationList = new List<float>();

          Random random = new Random();

          #endregion

          #region Properties

          #endregion

          #region Initialization

          /// <summary>Constructor.</summary>
          /// <param name="runTime">Time (in milliseconds) of each Step.</param>
          /// <param name="message">The text to be rendered.</param>
          public RandomFloatyEffect(float runTime, String text)
               : base(runTime)
          {
               effectText = text;
               elapsedTime = 0.0f;
               remaining = runTime;

               foreach (char character in text)
               {
                    characterList.Add(character);
                    characterPositionList.Add(new Vector2(random.Next(1280), random.Next(720)));
                    characterRotationList.Add(0);
               }
          }

          #endregion

          #region Update

          /// <summary>Overriden Update method.</summary>
          /// <param name="gameTime">GameTime from the XNA Game instance.</param>
          /// <param name="textObj">The TextObject to Update.</param>
          public override void Update(GameTime gameTime, TextObject textObj)
          {
               elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

               Step = (RunTime == 0 ? 1 :
                      MathHelper.Clamp((float)((RunTime -
                          remaining) / RunTime), 0, 1));

               // Update position
               for (int i = 0; i < characterPositionList.Count; i++)
               {
                    characterPositionList[i] =
                         new Vector2(characterPositionList[i].X + random.Next(-5, 5),
                              characterPositionList[i].Y + random.Next(-5, 5));

                    characterRotationList[i] += MathHelper.ToRadians(random.Next(5));
               }

               remaining -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
          }

          #endregion

          #region Draw

          /// <summary>Overriden Draw method.</summary>
          /// <param name="gameTime">GameTime from the XNA Game instance.</param>
          /// <param name="textObj">The TextObject to Draw.</param>
          public override void Draw(GameTime gameTime, TextObject textObj)
          {
               SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

               if (textObj.IsCenter)
               {
                    for (int i = 0; i < characterList.Count; i++)
                    {
                         TextManager.DrawCentered(false, textObj.Font, characterList[i].ToString(),
                              characterPositionList[i], textObj.Color, characterRotationList[i], 
                              textObj.Origin, textObj.Scale, SpriteEffects.None, 0.0f);
                    }
               }

               else
               {
                    for (int i = 0; i < characterList.Count; i++)
                    {
                         MySpriteBatch.DrawString(textObj.Font, characterList[i].ToString(),
                              characterPositionList[i], textObj.Color, characterRotationList[i],
                              textObj.Origin, textObj.Scale, SpriteEffects.None, 0.0f);
                    }
               }
          }

          #endregion
     }
}