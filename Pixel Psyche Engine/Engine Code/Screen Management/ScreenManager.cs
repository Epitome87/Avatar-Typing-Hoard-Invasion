#region File Description
//-----------------------------------------------------------------------------
// ScreenManager.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.ResourceManagement;
using Microsoft.Xna.Framework.Content;
#endregion

namespace PixelEngine.Screen
{
     /// <remarks>
     /// The Screen Manager is a component which manages one or more GameScreen
     /// instances. It maintains a stack of screens, calls their Update and Draw
     /// methods at the appropriate times, and automatically routes input to the
     /// topmost active screen.
     /// </remarks>
     public class ScreenManager : DrawableGameComponent
     {
          #region Singleton

          /// <summary>
          /// The singleton for this type.
          /// </summary>
          private static ScreenManager screenManager = null;

          #endregion

          private static List<GameScreen> popupScreens = new List<GameScreen>();
          private static List<GameScreen> popupScreensToUpdate = new List<GameScreen>();

          #region Fields

          private static List<GameScreen> screens = new List<GameScreen>();
          private static List<GameScreen> screensToUpdate = new List<GameScreen>();

          private static InputState input = new InputState();

          private static SpriteBatch spriteBatch;
          private static GameResourceFont font;
          private static GameResourceTexture2D blankTexture;
          private static GameResourceTexture2D gradientTexture;
          private static GameResourceTexture2D acceptIcon;
          private static GameResourceTexture2D cancelIcon;

          private static bool isInitialized;
          private static bool traceEnabled;

          private static Game game;

          #endregion

          #region Properties

          public static ScreenManager GetInstance
          {
               get { return screenManager; }
          }

          /// <summary>
          /// A default SpriteBatch shared by all the screens. This saves
          /// each screen having to bother creating their own local instance.
          /// </summary>
          public static SpriteBatch SpriteBatch
          {
               get { return spriteBatch; }
          }

          /// <summary>
          /// A default font shared by all the screens. This saves
          /// each screen having to bother loading their own local copy.
          /// </summary>
          public static SpriteFont Font
          {
               get { return font.SpriteFont; }
          }

          /// <summary>
          /// If true, the manager prints out a list of all the screens
          /// each time it is updated. This can be useful for making sure
          /// everything is being added and removed at the right times.
          /// </summary>
          public static bool TraceEnabled
          {
               get { return traceEnabled; }
               set { traceEnabled = value; }
          }

          /// <summary>
          /// Returns the Game.
          /// </summary>
          new public static Game Game
          {
               get { return game; }
               set { game = value; }
          }

          /// <summary>
          /// Returns Game.GraphicsDevice.
          /// </summary>
          new public static GraphicsDevice GraphicsDevice
          {
               get { return game.GraphicsDevice; }
          }

          #endregion

          #region Initialization

          /// <summary>
          /// Constructs a new screen manager component.
          /// </summary>
          private ScreenManager(Game game)
               : base(game)
          {
               Game = game;
          }

          public static void Initialize(Game game)
          {
               screenManager = new ScreenManager(game);

               if (game != null)
               {
                    game.Components.Add(screenManager);
               }
          }

          /// <summary>
          /// Initializes the screen manager component.
          /// </summary>
          public override void Initialize()
          {
               base.Initialize();

               isInitialized = true;
          }

          /// <summary>
          /// Load your graphics content.
          /// </summary>
          protected override void LoadContent()
          {
               // Load content belonging to the screen manager.
               ContentManager content = Game.Content;

               spriteBatch = new SpriteBatch(GraphicsDevice);

               font = ResourceManager.LoadFont(@"Fonts\MenuFont_Border");
               blankTexture = ResourceManager.LoadTexture(@"Textures\Blank Textures\blank");

               // Load the necessary resources--not just for the ScreenManager,
               // but for any potentially common menu that should have its 
               // resources kept in memory the entire game-life.
               blankTexture = ResourceManager.LoadTexture(@"Textures\Blank Textures\blank");
               gradientTexture = ResourceManager.LoadTexture(@"Menus\Gradient");
               acceptIcon = ResourceManager.LoadTexture(@"Buttons\xboxControllerButtonA");
               cancelIcon = ResourceManager.LoadTexture(@"Buttons\xboxControllerButtonB");

               // Tell each of the screens to load their content.
               foreach (GameScreen screen in screens)
               {
                    screen.LoadContent();
               }
          }

          #endregion

          #region Update

          /// <summary>
          /// Allows each screen to run logic.
          /// </summary>
          public override void Update(GameTime gameTime)
          {
               // Read the keyboard and gamepad.
               input.Update();

               // Make a copy of the master screen list, to avoid confusion if
               // the process of updating one screen adds or removes others.
               screensToUpdate.Clear();

               try
               {
                    foreach (GameScreen screen in screens)
                         screensToUpdate.Add(screen);
               }

               catch (System.InvalidOperationException)
               { }

               bool otherScreenHasFocus = !Game.IsActive;
               bool coveredByOtherScreen = false;

               // Loop as long as there are screens waiting to be updated.
               while (screensToUpdate.Count > 0)
               {
                    // Pop the topmost screen off the waiting list.
                    GameScreen screen = screensToUpdate[screensToUpdate.Count - 1];

                    screensToUpdate.RemoveAt(screensToUpdate.Count - 1);

                    // BECAME NULL ?!?!?! TESTING
                    if (screen == null)
                         return;
                    // END TESTING

                    // Update the screen.
                    screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                    if (screen.ScreenState == ScreenState.TransitionOn ||
                        screen.ScreenState == ScreenState.Active)
                    {
                         // If this is the first active screen we came across,
                         // give it a chance to handle input.
                         if (!otherScreenHasFocus)
                         {
                              screen.HandleInput(input);


                              otherScreenHasFocus = true;
                         }

                         // If this is an active non-popup, inform any subsequent
                         // screens that they are covered by it.
                         if (!screen.IsPopup)
                              coveredByOtherScreen = true;


                         // TESTING
                         if (screen.IsOverlayPopup)
                         {
                              otherScreenHasFocus = false;
                              coveredByOtherScreen = false;
                         }
                         // END TESTING
                    }
               }

               // Print debug trace?
               if (traceEnabled)
                    TraceScreens();








               popupScreensToUpdate.Clear();

               try
               {
                    foreach (GameScreen screen in popupScreens)
                         popupScreensToUpdate.Add(screen);
               }

               catch (System.InvalidOperationException)
               { }

               // Loop as long as there are screens waiting to be updated.
               while (popupScreensToUpdate.Count > 0)
               {
                    // Pop the topmost screen off the waiting list.
                    GameScreen screen = popupScreensToUpdate[popupScreensToUpdate.Count - 1];

                    popupScreensToUpdate.RemoveAt(popupScreensToUpdate.Count - 1);

                    // BECAME NULL ?!?!?! TESTING
                    if (screen == null)
                         return;
                    // END TESTING

                    // Update the screen.
                    screen.Update(gameTime, false, false);

                    if (screen.ScreenState == ScreenState.TransitionOn ||
                        screen.ScreenState == ScreenState.Active)
                    {
                         // If this is the first active screen we came across,
                         // give it a chance to handle input.
                         if (!otherScreenHasFocus)
                         {
                              screen.HandleInput(input);

                              otherScreenHasFocus = true;
                         }

                         // If this is an active non-popup, inform any subsequent
                         // screens that they are covered by it.
                         if (!screen.IsPopup)
                              coveredByOtherScreen = true;

                         // TESTING
                         if (screen.IsOverlayPopup)
                         {
                              otherScreenHasFocus = false;
                              coveredByOtherScreen = false;
                         }
                         // END TESTING
                    }
               }
          }

          #endregion

          #region Draw

          /// <summary>
          /// Tells each screen to draw itself.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               try
               {
                    foreach (GameScreen screen in screens)
                    {
                         if (screen.ScreenState == ScreenState.Hidden)
                              continue;

                         screen.Draw(gameTime);
                    }
               }
               catch (System.InvalidOperationException)
               { }






               try
               {
                    foreach (GameScreen screen in popupScreens)
                    {
                         if (screen.ScreenState == ScreenState.Hidden)
                              continue;

                         screen.Draw(gameTime);
                    }
               }
               catch (System.InvalidOperationException)
               { }
          }

          #endregion

          /// <summary>
          /// Adds a new screen to the screen manager.
          /// </summary>
          public static void AddPopupScreen(GameScreen screen, PlayerIndex? controllingPlayer)
          {
               screen.ControllingPlayer = controllingPlayer;
               screen.IsExiting = false;

               // If we have a graphics device, tell the screen to load content.
               if (isInitialized)
               {
                    screen.LoadContent();
               }

               popupScreens.Add(screen);
          }

          /// <summary>
          /// Removes a screen from the screen manager. You should normally
          /// use GameScreen.ExitScreen instead of calling this directly, so
          /// the screen can gradually transition off rather than just being
          /// instantly removed.
          /// </summary>
          public static void RemovePopupScreen(GameScreen screen)
          {
               //lock (screen)
               {
                    // If we have a graphics device, tell the screen to unload content.
                    if (isInitialized)
                    {
                         screen.UnloadContent();
                    }

                    popupScreens.Remove(screen);
                    popupScreensToUpdate.Remove(screen);
               }
          }

          /// <summary>
          /// Removes a screen from the screen manager. You should normally
          /// use GameScreen.ExitScreen instead of calling this directly, so
          /// the screen can gradually transition off rather than just being
          /// instantly removed.
          /// </summary>
          public static void RemoveAllScreens()
          {
               foreach (GameScreen screen in screens)
               {
                    // If we have a graphics device, tell the screen to unload content.
                    if (isInitialized)
                         screen.UnloadContent();
               }

               screens.Clear();
               screensToUpdate.Clear();
          }

          #region Public Methods

          /// <summary>
          /// Adds a new screen to the screen manager.
          /// </summary>
          public static void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer)
          {
               screen.ControllingPlayer = controllingPlayer;
               screen.IsExiting = false;

               // If we have a graphics device, tell the screen to load content.
               if (isInitialized)
               {
                    screen.LoadContent();
               }

               screens.Add(screen);
          }


          /// <summary>
          /// Removes a screen from the screen manager. You should normally
          /// use GameScreen.ExitScreen instead of calling this directly, so
          /// the screen can gradually transition off rather than just being
          /// instantly removed.
          /// </summary>
          public static void RemoveScreen(GameScreen screen)
          {
               //lock (screen)
               {
                    // If we have a graphics device, tell the screen to unload content.
                    if (isInitialized)
                    {
                         screen.UnloadContent();
                    }

                    screens.Remove(screen);
                    screensToUpdate.Remove(screen);
               }
          }


          /// <summary>
          /// Expose an array holding all the screens. We return a copy rather
          /// than the real master list, because screens should only ever be added
          /// or removed using the AddScreen and RemoveScreen methods.
          /// </summary>
          public static GameScreen[] GetScreens()
          {
               return screens.ToArray();
          }


          /// <summary>
          /// Helper draws a translucent black fullscreen sprite, used for fading
          /// screens in and out, and for darkening the background behind popups.
          /// </summary>
          public static void FadeBackBufferToBlack(int alpha)
          {
               Viewport viewport = GraphicsDevice.Viewport;

               spriteBatch.Begin();

               spriteBatch.Draw(blankTexture.Texture2D,
                                new Rectangle(0, 0, viewport.Width, viewport.Height),
                                new Color(0, 0, 0, (byte)alpha));

               spriteBatch.End();
          }


          #endregion

          #region Disposal

          /// <summary>
          /// Unload your graphics content.
          /// </summary>
          protected override void UnloadContent()
          {
               // Tell each of the screens to unload their content.
               foreach (GameScreen screen in screens)
               {
                    screen.UnloadContent();
               }
          }

          #endregion

          #region Trace Screens

          /// <summary>
          /// Prints a list of all the screens, for debugging.
          /// </summary>
          void TraceScreens()
          {
               List<string> screenNames = new List<string>();

               foreach (GameScreen screen in screens)
                    screenNames.Add(screen.GetType().Name);

               // Doesn't work in XNA 4.0:
               //Trace.WriteLine(string.Join(", ", screenNames.ToArray()));

               // Change To: Don't really use, so not important (And I don't know!)
          }

          #endregion
     }
}
