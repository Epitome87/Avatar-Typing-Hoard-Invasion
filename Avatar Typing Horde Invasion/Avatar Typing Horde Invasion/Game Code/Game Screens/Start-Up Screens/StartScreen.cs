#region File Description
//-----------------------------------------------------------------------------
// StartScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using PixelEngine;
using PixelEngine.AchievementSystem;
using PixelEngine.CameraSystem;
using PixelEngine.Menu;
using PixelEngine.Screen;
using PixelEngine.Storage;
using PixelEngine.Text;
using System.IO;
#endregion

namespace AvatarTyping
{
     /// <remarks>
     /// The Start Screen is brought up after the Splash Screen. 
     /// This screen merely displays a request for the player to press Start.
     /// Upon pressing Start, the Player's Index is obtained, setting him as
     /// the controlling player.
     /// </remarks>
     public class StartScreen : MenuScreen
     {
          #region Fields

          SpriteBatch spriteBatch;
          ContentManager content;
          Vector2 textPosition = new Vector2(1240 / 2, 0);

          TextObject startMessage;
          TextObject startMessage2;

          float elapsedTime = 0.0f;

          IAsyncResult result;

          Color bgColor = Color.Blue;
          static string pName;

          CinematicEvent triangularZoomEvent;

          bool updateCinematicEvent = true;

          string message = "";
          bool saveGameBeingRequested = false;
          bool gameSavedAlready = false;
          bool saveRequested = false;
          bool gamerSignedIn = false;
          bool profilePromptRead = false;

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public StartScreen()
               : base("")
          {

               PixelEngine.ResourceManagement.ResourceManager.LoadFont(@"Fonts\ChatFont");

               PixelEngine.Audio.AudioManager.PlayMusic("One-eyed Maestro");

               TransitionOnTime = TimeSpan.FromSeconds(5f);
               TransitionOffTime = TimeSpan.FromSeconds(0f);

               // Create our menu entries.
               MenuEntry startMenuEntry = new MenuEntry("Press Start Or\n[Enter] to Continue");

               startMenuEntry.SelectedColor = Color.White;
               startMenuEntry.Position = new Vector2(PixelEngine.EngineCore.ScreenCenter.X, 410f);
               startMenuEntry.ShowIcon = false;
               startMenuEntry.ShowGradientBorder = false;

               startMenuEntry.ShowBorder = true;
               startMenuEntry.BorderColor = Color.CornflowerBlue * .5f;
               startMenuEntry.SelectedBorderColor = Color.CornflowerBlue;

               // Hook up menu event handlers.
               startMenuEntry.Selected += StartMenuEntrySelected;

               // Add entries to the menu.
               MenuEntries.Add(startMenuEntry);

               startMessage = new TextObject("Avatar Typing:", new Vector2(EngineCore.ScreenCenter.X, EngineCore.ScreenCenter.Y - 100));
               startMessage.FontType = TextManager.FontType.TitleFont;
               startMessage.Scale = 0.7f;
               startMessage.Color = Color.Gold;
               startMessage.Origin = startMessage.Font.MeasureString(startMessage.Text) / 2;
               startMessage.AddTextEffect(new MoveInEffect(2500f, startMessage.Text, new Vector2(startMessage.Position.X, startMessage.Position.Y - 400f), startMessage.Position));

               startMessage2 = new TextObject("Horde Invasion", new Vector2(EngineCore.ScreenCenter.X, EngineCore.ScreenCenter.Y + 25));
               startMessage2.FontType = TextManager.FontType.TitleFont;
               startMessage2.Scale = 0.5f;
               startMessage2.Color = Color.Gold;
               startMessage2.Origin = startMessage.Font.MeasureString(startMessage2.Text) / 2;
               startMessage2.AddTextEffect(new TypingEffect(2000.0f, startMessage2.Text));
          }

          public override void LoadContent()
          {
               base.LoadContent();

               if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

               spriteBatch = ScreenManager.SpriteBatch;

               if (Gamer.SignedInGamers.Count < 1)
                    Guide.ShowSignIn(1, false);//true);

               CameraManager.ActiveCamera.Reset(EngineCore.GraphicsDevice.Viewport);
               CameraManager.ActiveCamera.Position = new Vector3(0, 1, 0);
               CameraManager.ActiveCamera.LookAt = new Vector3(0f, 1.25f, 10f);
               CameraManager.ActiveCamera.ViewMatrix = Matrix.CreateLookAt(CameraManager.ActiveCamera.Position, CameraManager.ActiveCamera.LookAt, Vector3.Up);
               CameraManager.ActiveCamera.FieldOfView = 45f;

               List<CameraEffect> effectList = new List<CameraEffect>();

               effectList.Add(new MoveEffect(5000.0f, new Vector3(1f, 1f, 0f)));
               effectList.Add(new MoveEffect(5000.0f, new Vector3(0f, 5f, 0f)));
               effectList.Add(new PointEffect(5000.0f, new Vector3(0, 1.25f, 40f)));
               effectList.Add(new ZoomEffect(5000.0f, 0.0f, 15.0f));
               effectList.Add(new PointEffect(5000.0f, new Vector3(0f, 1.25f, 10f)));
               effectList.Add(new ZoomEffect(5000.0f, 0.0f, -15.0f));
               effectList.Add(new MoveEffect(5000.0f, new Vector3(-1f, 1f, 0f)));

               for (int i = 0; i < 0; i++)
               {
                    effectList.Add(new MoveEffect(3000.0f, new Vector3(1f, 1f, 0f)));
                    effectList.Add(new MoveEffect(3000.0f, new Vector3(-1f, 5f, 0f)));
                    effectList.Add(new MoveEffect(3000.0f, new Vector3(1f, 5f, 0f)));
                    effectList.Add(new MoveEffect(3000.0f, new Vector3(-1f, 1f, 0f)));
               }

               triangularZoomEvent =
                    new CinematicEvent(effectList, true);
          }

          #endregion

          #region Handle Input

          public override void HandleInput(InputState input)
          {
               // This prevents the user from providing input for the first 2.5 seconds.
               // This prevents him from Pressing Start right away.
               if (elapsedTime < 5f)
                    return;

               base.HandleInput(input);

               if (input == null)
                    throw new ArgumentNullException("input");

               // Look up inputs for the active player profile.
               KeyboardState keyboardState = input.CurrentKeyboardStates[(int)PlayerIndex.One];
               GamePadState gamePadState = input.CurrentGamePadStates[(int)PlayerIndex.One];

               // The game pauses either if the user presses the pause button, or if
               // they unplug the active gamepad. This requires us to keep track of
               // whether a gamepad was ever plugged in, because we don't want to pause
               // on PC if they are playing with a keyboard and have no gamepad at all!
               if (ControllingPlayer.HasValue)
               {
                    bool gamePadDisconnected = !gamePadState.IsConnected &&
                                               input.GamePadWasConnected[(int)ControllingPlayer.Value];
               }
          }

          /// <summary>
          /// Event handler for when the Start Game menu entry is selected.
          /// </summary>
          void StartMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               updateCinematicEvent = false;

               ControllingPlayer = e.PlayerIndex;
               PixelEngine.EngineCore.ControllingPlayer = ControllingPlayer;

               AvatarTypingGame.CurrentPlayer = new Player(e.PlayerIndex);

               SignedInGamer gamer = Gamer.SignedInGamers[ControllingPlayer.Value];

               if (gamer != null)
               {
                    pName = gamer.Gamertag;

                    gamerSignedIn = true;
               }

               else
               {
                    Guide.ShowSignIn(1, false);

                    gamerSignedIn = false;
               }

               if (gamer != null)
               {
                    AvatarTypingGame.SaveGameData.PlayerName = pName;

#if XBOX
                    // Set the request flag
                    saveGameBeingRequested = true;
#else
                    ExitScreen();
                    ScreenManager.AddScreen(new MainMenuScreen(), ControllingPlayer);
#endif
               }
          }

          protected override void OnCancel(PlayerIndex playerIndex)
          {
               // Do nothing.
          }

          #endregion

          #region Update

          public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               if (IsActive)
               {
                    if (updateCinematicEvent)
                    {
                         triangularZoomEvent.Update(gameTime);
                    }

                    elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // If we have Pressed Start, but we are not Signed-In...
                    if (ControllingPlayer.HasValue)
                    {
                         if (Gamer.SignedInGamers[ControllingPlayer.Value] == null)//(!gamerSignedIn && ControllingPlayer.HasValue)
                         {
                              if (profilePromptRead)
                              {
                                   // Force the Player to Sign-In.
                                   PromptForSignIn();
                              }

                              profilePromptRead = false;

                              if (Gamer.SignedInGamers[ControllingPlayer.Value] == null)//(!gamerSignedIn)
                              {
                                   // If we reached this point, they must have not signed-in despite being prompted.
                                   int? profileRequiredPromptRead = SimpleGuideMessageBox.ShowMessageBox(ControllingPlayer, "Profile Required", "This game requires you to be signed into a Player Profile. Please select a Player Profile before you can continue.",
                                        new string[] { "OK" }, 0, MessageBoxIcon.Warning);

                                   // If we reached this point, they must have read the Profile Required prompt.
                                   if (profileRequiredPromptRead.HasValue)
                                   {
                                        profilePromptRead = true;
                                   }
                              }

                              return;
                         }
                    }

                    // We don't save in Trial Mode, so just go to Main Menu.
                    if (saveGameBeingRequested && Guide.IsTrialMode)
                    {
                         SimpleGuideMessageBox.ShowMessageBox(ControllingPlayer, "Saving Disabled", "Since you are playing Trial Mode, your progress will not be saved during gameplay.",
                              new string[] { "OK" }, 0, MessageBoxIcon.Warning);

                         ScreenManager.AddScreen(new MainMenuScreen(), ControllingPlayer);

                         AchievementManager.IsUpdating = true;

                         ScreenManager.RemoveScreen(this);
                    }


                    // Takes care of Guest-specific storage problem: Just don't let them save! 
                    else if (saveGameBeingRequested && AvatarTypingGame.CurrentPlayer.GamerInfo.Gamer.IsGuest)
                    {
                         SimpleGuideMessageBox.ShowMessageBox(ControllingPlayer, "Saving Disabled", "Since you are playing as a Guest, your progress will not be saved during gameplay.",
                                                   new string[] { "OK" }, 0, MessageBoxIcon.Warning);

                         ExitScreen();

                         ScreenManager.AddScreen(new MainMenuScreen(), ControllingPlayer);
                         AchievementManager.IsUpdating = true;
                    }
               }

               #region Save Process

               // If not Trial Mode, do the Save process.
               if (saveGameBeingRequested)
               {
                    /*
                    // show the message box - we end up calling this each frame as long as we're in the saveGame state - it will
                    // return null until the user presses a button or closes the guide - it returns -1 if the guide
                    // is closed, otherwise it returns the button number
                    int? button = SimpleGuideMessageBox.ShowMessageBox(ControllingPlayer, "Enable Saving", "Do you want to be able to load and save your progress for this play-session?",
                                                                  new string[] { "Yes, Save", "No, Don't Save" }, 0, MessageBoxIcon.None);

                    switch (button)
                    {
                         case -1:
                              message = "No Button";
                              saveGameBeingRequested = false;
                              break;

                         case 0:
                              message = "Saved";
                              saveGameBeingRequested = false;
                              break;

                         case 1:
                              message = "Cancelled";
                              saveGameBeingRequested = false;
                              break;
                    }

                    if (message == "Cancelled" || message == "No Button")
                    {
                         SimpleGuideMessageBox.ShowMessageBox(ControllingPlayer, "Saving Disabled", "Your progress for this play-session will not be saved.",
                                                                       new string[] { "OK" }, 0, MessageBoxIcon.Warning);

                         ExitScreen();

                         ScreenManager.AddScreen(new MainMenuScreen(), ControllingPlayer);
                         AchievementManager.IsUpdating = true;
                    }
                    */


                    // New
                    message = "Saved";
                    saveGameBeingRequested = false;
               }

               if (message == "Saved" && !gameSavedAlready)
               {
                    gameSavedAlready = true;

                    if ((!Guide.IsVisible) && !saveRequested)//(StorageManager.SaveRequested == false))
                    {
                         // Added this Try Statement on 8-6-2010:
                         try
                         {
                              saveRequested = true;

                              result = StorageDevice.BeginShowSelector(ControllingPlayer.Value, null, null);
                         }
                         catch
                         {
                         }
                    }
               }

               // If a save is pending, save as soon as the storage device is chosen
               if (saveRequested && result != null)//(StorageManager.SaveRequested) && (result != null))
               {
                    if (result.IsCompleted)
                    {
                         StorageDevice device = StorageDevice.EndShowSelector(result);

                         StorageManager.Device = device;

                         StorageDevice.DeviceChanged += new EventHandler<EventArgs>(StorageManager.StorageDevice_DeviceChanged);

                         if (device != null && device.IsConnected)
                         {
                              AvatarTypingGame.LoadGame(device);

                              AvatarTypingGame.SaveGame(device);
                         }

                         // Reset the request flag
                         saveRequested = false;

                         ExitScreen();





                         // LEADERBOARD CODE HERE

                         if (!Guide.IsTrialMode &&
                              AvatarTypingGame.CurrentPlayer != null &&
                              AvatarTypingGame.CurrentPlayer.GamerInfo.Gamer.IsSignedInToLive &&
                              AvatarTypingGame.CurrentPlayer.GamerInfo.Gamer.Privileges.AllowOnlineSessions)
                         {

                              // Should be in a different spot?
                              AvatarTypingGame.HighScoreSaveData = new Leaderboards.TopScoreListContainer(2, 100);

                              //AvatarTypingGame.LoadHighScores(device);
                              //AvatarTypingGame.SaveHighScores(device);

                              AvatarTypingGame.OnlineSyncManager.start(AvatarTypingGame.CurrentPlayer.GamerInfo.Gamer, AvatarTypingGame.HighScoreSaveData);
                         }



                         // END LEADERBOARD CODE




                         ScreenManager.AddScreen(new MainMenuScreen(), ControllingPlayer);
                         AchievementManager.IsUpdating = true;

                         if (!Guide.IsTrialMode)
                         {
                              AvatarTypingGame.AwardData.GamePurchased = true;
                         }
                    }
               }

               #endregion
          }

          #endregion

          #region Draw

          public override void Draw(GameTime gameTime)
          {
               // This prevents the Title Screen from rendering for the first
               // 75% of its TransitionOn period.
               //if (TransitionAlpha < 255 / (4.0f / 3.0f))
               //     return;

               Color color = bgColor * (this.TransitionAlpha / 255f);

               // Doesn't work in XNA 4.0:
               //ScreenManager.SpriteBatch.Begin(BlendState.AlphaBlend);

               // Change To:
               ScreenManager.SpriteBatch.Begin();

               startMessage.Update(gameTime);
               startMessage.Draw(gameTime);

               if (elapsedTime > 2.5f)
               {
                    startMessage2.Update(gameTime);
                    startMessage2.Draw(gameTime);
               }

               ScreenManager.SpriteBatch.End();

               // This prevents MenuScreen.Draw from being called for the first 5 seconds. 
               // This means no menu entries ("Press Start") will render until then.
               if (elapsedTime > 5f)
                    base.Draw(gameTime);
          }

          #endregion

          #region Prompt For Sign-In

          /// <summary>
          /// This forces a Profile to be Signed-In.
          /// </summary>
          private void PromptForSignIn()
          {
               if (ControllingPlayer.HasValue)
               {
                    SignedInGamer gamer = Gamer.SignedInGamers[ControllingPlayer.Value];

                    if (gamer == null)
                    {
                         gamerSignedIn = false;
                         Guide.ShowSignIn(1, false);
                    }

                    else
                    {
                         gamerSignedIn = true;
                    }
               }
          }

          #endregion
     }
}
