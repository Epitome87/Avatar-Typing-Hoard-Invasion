#region File Description
//-----------------------------------------------------------------------------
// ExplosionEffect.cs
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
     public class ExplosionEffect : TextEffect
     {
          #region Fields

          private string effectText;
          private float elapsedTime;
          private float remaining;

          List<char> characterList = new List<char>();
          List<Vector2> characterPositionList = new List<Vector2>();
          List<float> characterRotationList = new List<float>();
          List<Vector2> slopeList = new List<Vector2>();

          private bool normalizeExplosion = true;

          Random random = new Random();

          #endregion

          #region Properties

          #endregion

          #region Initialization

          /// <summary>Constructor.</summary>
          /// <param name="runTime">Time (in milliseconds) of each Step.</param>
          /// <param name="message">The text to be rendered.</param>
          public ExplosionEffect(TextObject textObj, float runTime, String text)
               : base(runTime)
          {
               effectText = text;
               elapsedTime = 0.0f;
               remaining = runTime;

               Vector2 explosionPosition = new Vector2(1280 / 2, 720 / 2);

               // Only works if IsCenter = false
               Vector2 leftMostPosition = textObj.Position;

               Vector2 spacing = Vector2.Zero;

               foreach (char character in text)
               {
                    characterList.Add(character);

                    leftMostPosition = 
                         new Vector2(leftMostPosition.X + spacing.X, leftMostPosition.Y);
                    
                    spacing = textObj.Font.MeasureString(character.ToString());

                    Vector2 slope = 
                         new Vector2(explosionPosition.X - leftMostPosition.X, explosionPosition.Y - leftMostPosition.Y);

                    // We don't want closer characters moving slower or quicker
                    // than those further away.
                    if (normalizeExplosion)
                    {
                         //float magnitude = (float) Math.Sqrt((slope.X * slope.X) + (slope.Y * slope.Y));
                         //slope.X = slope.X / magnitude;
                         //slope.Y = slope.Y / magnitude;
                         slope = Vector2.Normalize(slope);
                    }
                        
                    // If not normalized, characters closer to explosion
                    // get a faster velocity.
                    if (!normalizeExplosion)
                    {
                         slope.X /= 30f;
                         slope.Y /= 30f;
                    }

                    characterPositionList.Add(leftMostPosition);

                    slopeList.Add(slope);
                    
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
                    //characterPositionList[i] = new Vector2(characterPositionList[i].X - slopeList[i].X / 1f, characterPositionList[i].Y - slopeList[i].Y / 1f);

                    characterPositionList[i] = new Vector2(characterPositionList[i].X - slopeList[i].X, characterPositionList[i].Y - slopeList[i].Y);
                    //characterRotationList[i] += MathHelper.ToRadians(0.2f);
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