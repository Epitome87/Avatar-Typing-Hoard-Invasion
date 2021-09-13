#region File Description
//-----------------------------------------------------------------------------
// AnimatedLoadingScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine;
using PixelEngine.Graphics;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
using PixelEngine.Text;
using System.Collections;
#endregion

namespace AvatarTyping
{
     /// <summary>
     /// Derives from LoadingScreen, adding the use of  
     /// threads for animated loading screen support.
     /// </summary>
     public class AnimatedLoadingScreen : GameScreen
     {
          #region Fields

          private bool loadingIsSlow;
          private bool otherScreensAreGone;
          private GameScreen[] screensToLoad;

          private string message;
          private static string customMessage;
          private static bool IsCustomMessage;
          private static bool IsRemoveScreens;

          private bool isLoading;

          private static bool enableTip;

          private Thread updateThread;
          private bool threadFinished;          
          
          ContentManager content;
          GameResourceTexture2D backgroundTexture;
          GameResourceTexture2D yellowGradient;

          Random random = new Random();

          string tip = String.Empty;

          string[] tips = 
          {
               /*"Use your hands to type,\nnot your feet.",
               "Try not sucking\nas much.",
               "Target the wrong enemy?\nNeed to target a different one?\nPress Tab or LB / RB.",
               "You can change the text size\nin the Settings Menu.",
               "Confused about the controls?\nSee the Controls Menu\nby Pausing the game.",
               "Not sure about a certain Enemy?\nView the Enemy Information Menu\nby Pausing the game.",
               "Filling up your Combo Meter\nwill earn you extra lives.",
               "Don't underestimate the\nimportance of filling\nyour Combo Meter!",
               "Sometimes it's okay\nto just give up.",
               "Getting owned?\nChange Difficulty in the Settings Menu.",
               "Type faster!",
               "Eliminate Fast\n(green) enemies first.",*/"Use your hands to type,\nnot your feet.",
               "Eliminating Kamikaze (yellow)\nenemies with a Speedy or Perfect Bonus\nwill destroy all other enemies\non-screen."
          };

          #endregion

          #region Initialization

          /// <summary>
          /// The constructor is private: loading screens should
          /// be activated via the static Load method instead.
          /// </summary>
          private AnimatedLoadingScreen(bool loadingIsSlow, GameScreen[] screensToLoad, bool? showTip)
          {
               tip = "Tip: " + tips[random.Next(tips.Length)];

               TextManager.Reset();

               this.loadingIsSlow = loadingIsSlow;
               this.screensToLoad = screensToLoad;

               TransitionOnTime = TimeSpan.FromSeconds(0.0);
               TransitionOffTime = TimeSpan.FromSeconds(0.0);

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

               isLoading = false;
               threadFinished = false;
               enableTip = true;

               updateThread = new Thread(new ThreadStart(ThreadUpdateMethod));
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
               {
                    if (screen.GetType() != typeof(BackgroundScreen))
                         screen.ExitScreen();
               }

               AnimatedLoadingScreen.IsCustomMessage = false;

               // Create and activate the loading screen.
               AnimatedLoadingScreen loadingScreen = new AnimatedLoadingScreen(loadingIsSlow,
                                                               screensToLoad, enableTip);

               ScreenManager.AddScreen(loadingScreen, controllingPlayer);
          }

          public static void RemoveAllButGameplayScreen()
          {
               foreach (GameScreen screen in ScreenManager.GetScreens())
               {
                    if (screen.GetType() != typeof(GameplayBackgroundScreen))
                              screen.ExitScreen();
               }
          }

          /// <summary>
          /// Activates the loading screen.
          /// </summary>
          public static void Load(bool loadingIsSlow,
                                  PlayerIndex? controllingPlayer, string customMessage,
               bool removeScreens, params GameScreen[] screensToLoad)
          {
               // Tell all the current screens to transition off.
               if (removeScreens)
               {
                    foreach (GameScreen screen in ScreenManager.GetScreens())
                    {
                         if (screen.GetType() != typeof(GameplayBackgroundScreen))
                              screen.ExitScreen();
                    }
               }

               AnimatedLoadingScreen.IsCustomMessage = true;
               AnimatedLoadingScreen.customMessage = customMessage;

               // Create and activate the loading screen.
               AnimatedLoadingScreen loadingScreen = new AnimatedLoadingScreen(loadingIsSlow,
                                                               screensToLoad, enableTip);

               ScreenManager.AddScreen(loadingScreen, controllingPlayer);
          }

          public override void LoadContent()
          {
               if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

               backgroundTexture = ResourceManager.LoadTexture(@"Textures\Sky_Night");
               yellowGradient = ResourceManager.LoadTexture(@"Menus\WhiteGradient");
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

               if (otherScreensAreGone && !isLoading)
               {
                    isLoading = true;
                    updateThread.Start();
               }

               if (threadFinished)
               {
                    updateThread = null;
                    
                    ScreenManager.RemoveScreen(this);

                    Thread.Sleep(2000);
               }
          }

          #endregion

          #region Draw

          uint periodFrame = 0;
          string periodText = "";

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

                    // This is new
                    ScreenManager.GraphicsDevice.Clear(Color.Black);
                    // End this is new

                    MySpriteBatch.Begin();

                    MySpriteBatch.Draw(backgroundTexture.Texture2D, 
                         new Rectangle(0, 0, EngineCore.GraphicsInformation.ScreenWidth, EngineCore.GraphicsInformation.ScreenHeight), Color.White);

                    MySpriteBatch.DrawCentered(yellowGradient.Texture2D, new Vector2(EngineCore.ScreenCenter.X, 
                         EngineCore.ScreenCenter.Y - 150.0f), Color.Gray, 0.6f);




                    MySpriteBatch.DrawCentered(yellowGradient.Texture2D, new Vector2(EngineCore.ScreenCenter.X, 
                         EngineCore.ScreenCenter.Y + 105.0f), Color.White, new Vector2(1.0f, 0.7f));

                    TextManager.DrawCentered(true, font, message, new Vector2(EngineCore.ScreenCenter.X, 
                         EngineCore.ScreenCenter.Y - 125f), Color.White, 1.5f);

                    if (enableTip)
                    {
                         TextManager.DrawCentered(true, font, tip, 
                              new Vector2(EngineCore.ScreenCenter.X, EngineCore.ScreenCenter.Y * 1.35f), Color.Gray);
                    }

                    string loadingText = "Loading";

                    periodFrame++;
                    periodFrame = periodFrame % (4 * 10);

                    periodText = "";

                    for (int i = 0; i < (periodFrame / 10); i++)
                    {
                         periodText += ".";
                    }

                    // Render the "Loading" text.
                    TextManager.DrawString(font, loadingText + periodText,
                              new Vector2(EngineCore.ScreenCenter.X * 1.25f, EngineCore.ScreenCenter.Y * 1.65f), Color.White, 1.25f);

                    MySpriteBatch.End();
               }
          }

          #endregion

          #region Thread Updating Method

          protected void ThreadUpdateMethod()
          {
#if XBOX
               updateThread.SetProcessorAffinity(new[] { 5 });
#endif
               AvatarTypingGame.Game.Exiting += delegate
               {
                    if (updateThread != null)
                    {
                         updateThread.Abort();
                    }
               };

               foreach (GameScreen screen in screensToLoad)
               {
                    if (screen != null)
                    {
                         ScreenManager.AddScreen(screen, ControllingPlayer);
                    }
               }

               threadFinished = true;
          }

          #endregion
     }
}
