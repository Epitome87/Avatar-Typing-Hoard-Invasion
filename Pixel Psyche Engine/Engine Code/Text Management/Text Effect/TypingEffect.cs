#region File Description
//-----------------------------------------------------------------------------
// TypingEffect.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.Audio;
using PixelEngine.Graphics;
using PixelEngine.Screen;
#endregion

namespace PixelEngine.Text
{
     /// <remarks>
     /// A type of TextEffect which draws TextObjects one String character at a time,
     /// causing the String to appear as if it is being typed in via keyboard.
     /// </remarks>
     public class TypingEffect : TextEffect
     {
          #region Fields

          private string effectText;
          private float elapsedTime;
          private float remaining;
          private float soundTime;
          private string keyTypedSound = "KeyPress";
          private bool playSound = true;

          private float typesPerSecond;
          private float totalTypes;
          private float typingRunTime;

          private bool usingConstantSpeed = false;
          private bool checkRunTime = true;

          #endregion

          #region Properties

          /// <summary>
          /// The name of the Cue to Play when a key is struck.
          /// </summary>
          public string KeyTypedSound
          {
               get { return keyTypedSound; }
               set { keyTypedSound = value; }
          }

          /// <summary>
          /// Whether or not a Cue will Play when a key is struck.
          /// </summary>
          public bool IsSoundEnabled
          {
               get { return playSound; }
               set { playSound = value; }
          }

          #endregion

          #region Initialization

          /// <summary>Constructor.</summary>
          /// <param name="runTime">Time (in milliseconds) of each Step.</param>
          /// <param name="message">The text to be rendered.</param>
          public TypingEffect(float runTime, String text)
               : base(runTime)
          {
               effectText = text;
               elapsedTime = 0.0f;
               remaining = runTime;
               soundTime = 0.0f;

               usingConstantSpeed = false;
          }

          /// <summary>Constructor.</summary>
          /// <param name="runTime">Time (in milliseconds) of each Step.</param>
          /// <param name="message">The text to be rendered.</param>
          public TypingEffect(float runTime, String text, bool soundEnabled)
               : base(runTime)
          {
               effectText = text;
               elapsedTime = 0.0f;
               remaining = runTime;
               soundTime = 0.0f;

               usingConstantSpeed = false;

               playSound = soundEnabled;
          }

          /// <summary>Constructor.</summary>
          /// <param name="runTime">Time (in milliseconds) of each Step.</param>
          /// <param name="message">The text to be rendered.</param>
          public TypingEffect(float runTime, String text, bool soundEnabled, string typingSoundName)
               : base(runTime)
          {
               effectText = text;
               elapsedTime = 0.0f;
               remaining = runTime;
               soundTime = 0.0f;

               usingConstantSpeed = false;

               playSound = soundEnabled;
               keyTypedSound = typingSoundName;
          }
          /// <summary>Constructor.</summary>
          /// <param name="runTime">Time (in milliseconds) of each Step.</param>
          /// <param name="message">The text to be rendered.</param>
          public TypingEffect(String text, float strokesPerSecond)
               : base(0)
          {
               effectText = text;
               elapsedTime = 0.0f;

               typesPerSecond = strokesPerSecond;

               usingConstantSpeed = true;
          }

          #endregion

          #region Update

          /// <summary>Overriden Update method.</summary>
          /// <param name="gameTime">GameTime from the XNA Game instance.</param>
          /// <param name="textObj">The TextObject to Update.</param>
          public override void Update(GameTime gameTime, TextObject textObj)
          {
               if (usingConstantSpeed)
               {
                    if (checkRunTime)
                    {
                         totalTypes = textObj.Text.Length;

                         typingRunTime = totalTypes / typesPerSecond;
                         runTime = typingRunTime * 1000.0f;

                         RunTime = runTime;
                         remaining = RunTime;

                         checkRunTime = false;
                    }
               }

               elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

               Step = (RunTime == 0 ? 1 :
                      MathHelper.Clamp((float)((RunTime -
                          remaining) / RunTime), 0, 1));

               this.effectText = textObj.Text.Substring(0, (int)(Step * textObj.Text.Length));

               if ((int)(Step * textObj.Text.Length) > (int)(soundTime))
               {
                    if (playSound)
                    {
                         if (usingConstantSpeed)
                         {
                              keyTypedSound = "KeyPress_ExtraSoft";
                         }

                         AudioManager.PlayCue(keyTypedSound);
                    }
               }

               remaining -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
               soundTime = (int)(Step * textObj.Text.Length);
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
                    TextManager.DrawCentered(false, textObj.Font, this.effectText, textObj.Position, textObj.Color,
                         textObj.Rotation, textObj.Origin, textObj.Scale, SpriteEffects.None, 0.0f);
               }
               
               else
               {
                    MySpriteBatch.DrawString(textObj.Font, this.effectText, textObj.Position, textObj.Color,
                                   textObj.Rotation, textObj.Origin, textObj.Scale, SpriteEffects.None, 0.0f);
               }
          }

          #endregion
     }
}