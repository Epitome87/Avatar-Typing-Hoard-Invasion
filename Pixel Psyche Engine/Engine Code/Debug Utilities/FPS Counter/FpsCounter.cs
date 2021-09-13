#region File Description
//-----------------------------------------------------------------------------
// FpsCounter.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using PixelEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace PixelEngine.DebugUtilities
{
     /// <summary>
     /// Calculate and display the Framerate.
     /// </summary>
     public class FpsCounter : DrawableGameComponent
     {
          #region Singleton

          /// <summary>
          /// The singleton for the FpsCounter.
          /// </summary>
          private static FpsCounter fpsCounter = null;

          #endregion

          #region Fields

          private int fps = 0;
          private int frameCount = 0;
          private float totalElapsedTime = 0.0f;

          #endregion

          #region Properties

          public int Framerate
          {
               get { return fps; }
          }

          #endregion

          #region Initialization

          private FpsCounter(Game game) 
               : base(game)
          {
               fps = 0;
               frameCount = 0;
               totalElapsedTime = 0.0f;

               DrawOrder = 2000;
          }

          public static void Initialize(Game game)
          {
               fpsCounter = new FpsCounter(game);

               if (game != null)
               {
                    game.Components.Add(fpsCounter);
               }
          }

          #endregion

          #region Update

          /// <summary>
          /// Updates the frame count.
          /// </summary>
          public override void Update(GameTime gameTime)
          {
               // Doesn't work in XNA 4.0:
               //totalElapsedTime += (float)gameTime.ElapsedRealTime.TotalSeconds;

               // Change To: Some article says this may be valid
               totalElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

               frameCount++;

               //  Calculate how many frames occured per second.
               if (totalElapsedTime >= 1.0f)
               {
                    fps = frameCount;
                    frameCount = 0;
                    totalElapsedTime = 0.0f;
               }
          }

          #endregion

          #region Draw

          public override void Draw(GameTime gameTime)
          {
               SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

               spriteBatch.Begin();

               spriteBatch.DrawString(ScreenManager.Font, String.Format("Fps: {0}", fps.ToString()),
                    new Vector2(50, 50), Color.White);

               spriteBatch.End();
          }

          #endregion
     }
}


