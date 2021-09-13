#region File Description
//-----------------------------------------------------------------------------
// LoadingScreen.cs
// Matt McGrath, with help provided by Creaters.Xna.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.Graphics;
using PixelEngine.Screen;
using PixelEngine.Text;
#endregion

namespace AvatarTyping
{
     /// <summary>
     /// The loading screen coordinates transitions between the menu system and the
     /// game itself. Normally one screen will transition off at the same time as
     /// the next screen is transitioning on, but for larger transitions that can
     /// take a longer time to load their data, we want the menu system to be entirely
     /// gone before we start loading the game. This is done as follows:
     /// 
     /// - Tell all the existing screens to transition off.
     /// - Activate a loading screen, which will transition on at the same time.
     /// - The loading screen watches the state of the previous screens.
     /// - When it sees they have finished transitioning off, it activates the real
     ///   next screen, which may take a long time to load its data. The loading
     ///   screen will be the only thing displayed while this load is taking place.
     /// </summary>
     public class LoadingScreen : GameScreen
     {
          #region Fields

          private bool loadingIsSlow;
          private bool otherScreensAreGone;

          private GameScreen[] screensToLoad;

          private string message;
          private static string customMessage;
          private static bool IsCustomMessage;
          private static bool IsRemoveScreens;

          #endregion

          #region Initialization

          /// <summary>
          /// The constructor is private: loading screens should
          /// be activated via the static Load method instead.
          /// </summary>
          protected LoadingScreen(bool loadingIsSlow,
                                GameScreen[] screensToLoad)
          {
               TextManager.Reset();

               this.loadingIsSlow = loadingIsSlow;
               this.screensToLoad = screensToLoad;

               TransitionOnTime = TimeSpan.FromSeconds(0.5);

               if (IsCustomMessage)
               {
                    message = customMessage;
                    IsRemoveScreens = false;
               }

               else
               {
                    message = "L o a d i n g. . .";
                    IsRemoveScreens = true;
               }
          }

          /// <summary>
          /// Activates the loading screen.
          /// </summary>
          public static void Load(bool loadingIsSlow,
                                  PlayerIndex? controllingPlayer,
                                  params GameScreen[] screensToLoad)
          {
               // Tell all the current screens to transition off.
               foreach (GameScreen screen in ScreenManager.GetScreens())
                    screen.ExitScreen();

               LoadingScreen.IsCustomMessage = false;

               // Create and activate the loading screen.
               LoadingScreen loadingScreen = new LoadingScreen(loadingIsSlow,
                                                               screensToLoad);

               ScreenManager.AddScreen(loadingScreen, controllingPlayer);
          }

          /// <summary>
          /// Activates the loading screen.
          /// </summary>
          public static void Load(bool loadingIsSlow,
                                  PlayerIndex? controllingPlayer, string customMessage,
                                  params GameScreen[] screensToLoad)
          {
               // Tell all the current screens to transition off.
               //foreach (GameScreen screen in ScreenManager.GetScreens())
               //screen.ExitScreen();

               LoadingScreen.IsCustomMessage = true;
               LoadingScreen.customMessage = customMessage;

               // Create and activate the loading screen.
               LoadingScreen loadingScreen = new LoadingScreen(loadingIsSlow,
                                                               screensToLoad);

               ScreenManager.AddScreen(loadingScreen, controllingPlayer);
          }


          #endregion

          #region Update

          /// <summary>
          /// Updates the loading screen.
          /// </summary>
          public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                         bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               // If all the previous screens have finished transitioning
               // off, it is time to actually perform the load.
               if (otherScreensAreGone)
               {
                    ScreenManager.RemoveScreen(this);

                    foreach (GameScreen screen in screensToLoad)
                    {
                         if (screen != null)
                         {
                              ScreenManager.AddScreen(screen, ControllingPlayer);
                         }
                    }

                    // Once the load has finished, we use ResetElapsedTime to tell
                    // the  game timing mechanism that we have just finished a very
                    // long frame, and that it should not try to catch up.
                    ScreenManager.Game.ResetElapsedTime();
               }
          }

          #endregion

          #region Draw

          /// <summary>
          /// Draws the loading screen.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               // If we are the only active screen, that means all the previous screens
               // must have finished transitioning off. We check for this in the Draw
               // method, rather than in Update, because it isn't enough just for the
               // screens to be gone: in order for the transition to look good we must
               // have actually drawn a frame without them before we perform the load.
               if (IsRemoveScreens)
               {
                    if ((ScreenState == ScreenState.Active) &&
                        (ScreenManager.GetScreens().Length == 1))
                    {
                         otherScreensAreGone = true;
                    }
               }

               else
               {
                    if ((ScreenState == ScreenState.Active))
                    {
                         otherScreensAreGone = true;
                    }
               }

               // The gameplay screen takes a while to load, so we display a loading
               // message while that is going on, but the menus load very quickly, and
               // it would look silly if we flashed this up for just a fraction of a
               // second while returning from the game to the menus. This parameter
               // tells us how long the loading is going to take, so we know whether
               // to bother drawing the message.
               if (loadingIsSlow)
               {
                    SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
                    SpriteFont font = ScreenManager.Font;

                    // Center the text in the viewport.
                    Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
                    Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
                    Vector2 textSize = font.MeasureString(message);
                    Vector2 textPosition = (viewportSize - textSize) / 2;
                    
                    Color color = Color.White * (TransitionAlpha / 255f);

                    //Draw the text.
                    MySpriteBatch.Begin();
                    TextManager.DrawString(font, message, textPosition, color);
                    MySpriteBatch.End();
               }
          }

          #endregion
     }
}
