#region File Description
//-----------------------------------------------------------------------------
// Counter.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.Audio;
using PixelEngine.Text;
#endregion

namespace PixelEngine
{
     /// <remarks>
     /// This class represents a Countdown object.
     /// It provides logic for updating and rendering.
     /// 
     /// Useful for counting down to the start of a round or other event.
     /// 
     /// Features: 
     /// Customize how often the countdown is displayed.
     /// Customize how often an audio cue accompanies the countdown.
     /// Customize how long the countdown lasts.
     /// Customize the text that accompanies the countdown (+ time remaining text).
     /// </remarks>
     public class Counter
     {
          #region Fields

          private float elapsedTime;
          private int previousElapsedTime;
          private int duration;

          private int countdownStart;
          private int countdownEnd;
          private int currentCount;
          private bool isShowingCountdown;
          private string countdownText;
          private string countdownTextEnd;
          private string countdownSoundName;

          private int displayInterval;
          private int soundInterval;
          private int previousSoundTime;
          private int previousDisplayTime;

          #endregion

          #region Properties

          /// <summary>
          /// The time at which the Countdown begins.
          /// </summary>
          public int Start
          {
               get { return countdownStart; }
          }

          /// <summary>
          /// The time at which the Countdown ends.
          /// </summary>
          public int End
          {
               get { return countdownEnd; }
          }

          /// <summary>
          /// The time at which the Countdown is at currently.
          /// </summary>
          public int CurrentCount
          {
               get { return currentCount; }
          }

          /// <summary>
          /// Whether or not the Countdown is Active (in progress).
          /// </summary>
          public bool Active
          {
               get { return isShowingCountdown; }

               // shouldn't have htis
               set { isShowingCountdown = value; }
          }

          /// <summary>
          /// The Text displayed along with the Countdown.
          /// </summary>
          public string Text
          {
               get { return countdownText; }
          }

          /// <summary>
          /// The Text displayed when the Countdown is finished.
          /// 
          /// Sometimes we will want to display something unique when the Countdown is done,
          /// like "Begin!", so this is necessary.
          /// </summary>
          public string TextEnd
          {
               get { return countdownTextEnd; }
          }

          /// <summary>
          /// The name of the audio file which will be played at each interval of the Countdown.
          /// </summary>
          public string Sound
          {
               get { return countdownSoundName; }
          }

          /// <summary>
          /// The interval at which the Countdown should update its display.
          /// 
          /// For example, every 5 seconds would show "20", then 5 seconds later "15", etc.
          /// </summary>
          public int DisplayInterval
          {
               get { return displayInterval; }
          }

          /// <summary>
          /// The interval at which the audio for the Countdown should play.
          /// 
          /// For example, a value of 5 means a sound plays every 5 seconds.
          /// </summary>
          public int SoundInterval
          {
               get { return soundInterval; }
          }

          #endregion

          #region Initialization

          public Counter()
          {
               elapsedTime = 0.0f;
               isShowingCountdown = true;

               displayInterval = 0;
               soundInterval = 1;
          }

          public Counter(int start, int end, string text, string textEnd, string sound) 
               : this()
          {
               countdownStart = start;
               countdownEnd = end;
               duration = countdownStart - countdownEnd;

               countdownText = text;
               countdownTextEnd = textEnd;

               countdownSoundName = sound;

               previousDisplayTime = start;
               previousSoundTime = start;
          }

          public Counter(int start, int end, string text, string textEnd, string sound, int soundInt)
               : this(start, end, text, textEnd, sound)
          {
               soundInterval = soundInt;
          }

          #endregion

          #region Display Countdown

          /// <remarks>
          /// Handles the updating and rendering of the countdown.
          /// </remarks>
          /// <param name="gameTime"></param>
          public void DisplayCountdown(GameTime gameTime)
          {
               elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

               currentCount = countdownStart - (int)elapsedTime;

               // If the end of the Countdown has not yet been reached...
               if (currentCount > countdownEnd)
               {
                    if (Math.Abs(currentCount - previousDisplayTime) >= displayInterval)
                    {
                         TextManager.DrawAutoCentered(PixelEngine.Text.TextManager.TitleFont,
                              countdownText + currentCount.ToString(), EngineCore.ScreenCenter, Color.Gold, 0.75f);

                         previousDisplayTime = currentCount;
                    }
               }

               // Otherwise, we have reached the end of the Countdown...
               else
               {
                    TextManager.DrawAutoCentered(PixelEngine.Text.TextManager.TitleFont,
                         countdownTextEnd, EngineCore.ScreenCenter, Color.CornflowerBlue, 1.0f);
               }

               // Only play the audio if the proper interval has passed...
               if ( (Math.Abs(currentCount - previousSoundTime) >= soundInterval) &&
                    (currentCount != countdownStart && currentCount != countdownEnd - 1))
               {
                    AudioManager.PlayCue(countdownSoundName);

                    previousSoundTime = currentCount;
               }

               previousElapsedTime = currentCount;

               if (currentCount <= countdownEnd - 1)
               {
                    isShowingCountdown = false;
               }
          }

          #endregion
     }
}