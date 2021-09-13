#region File Description
//-----------------------------------------------------------------------------
// AvatarTypingGame.cs
// Copyright (C) Matt McGrath. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;
using PixelEngine;
using PixelEngine.AchievementSystem;
using PixelEngine.Audio;
using PixelEngine.Avatars;
using PixelEngine.CameraSystem;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
using PixelEngine.Storage;
using PixelEngine.Text;
using System.IO;
#endregion

using System.Xml;

namespace AvatarTyping
{
     /// <remarks>
     /// Avatar Typing game. Action typing game for the 360.
     /// Most of the game logic is found within the ScreenManager component.
     /// </remarks>
     public class AvatarTypingGame : EngineCore
     {
          #region Fields

          // A global reference to the Player.
          public static Player CurrentPlayer = null;

          // A global reference to Save-Game data.
          public static SaveGameData SaveGameData = new SaveGameData(); // Converted to Class, so added = new SaveGameData()

          // A global reference to Achievement data.
          public static AchievementData AwardData = new AchievementData();  // Converted to Class, so added = new AchievementData()

          // Global Achievement-Related Variables.
          public static bool GamePurchased = false;
          public static bool CreditsWatched = false;

          /// <summary>
          /// A GraphicsInfo object must be created and passed into the
          /// EngineCore Constructor, in order to determine how the
          /// PixelEngine will be Initialized.
          /// </summary>
          private static GraphicsInfo gi = new GraphicsInfo
          {
               ScreenWidth = 1280,
               ScreenHeight = 720,
               PreferMultiSampling = true,
               IsFullScreen = true
          };

          #endregion

          public static Stream GlobalScoreStream;
          public static Leaderboards.TopScoreListContainer HighScoreSaveData;

          public static void SaveHighScores(StorageDevice device)
          {
               // Show "Now Saving" animation?
               ScreenManager.AddPopupScreen(new PixelEngine.SavePopUpScreen(), EngineCore.ControllingPlayer.Value);

               if (device != null && device.IsConnected)
               {
                    // Open a storage container.
                    StorageContainer container = StorageManager.OpenContainer(device, "Scores");

                    // Get the path of the save game.
                    string filename = "highScores.sav";


                    // Open the file, creating it if necessary.
                    using (Stream stream = container.OpenFile(filename, FileMode.Create))
                    {
                         BinaryWriter writer = new BinaryWriter(stream);

                         AvatarTypingGame.HighScoreSaveData.save(writer);

                         GlobalScoreStream = stream;
                         writer.Close();
                    }
                    
                    // Dispose the container, to commit changes.
                    container.Dispose();
               }
          }

          public static void LoadHighScores(StorageDevice device)
          {
               // Should be in a different spot?
               //AvatarTypingGame.HighScoreSaveData = new Leaderboards.TopScoreListContainer(1, 10);

               // NEW
               // Open a storage container.
               StorageContainer container = StorageManager.OpenContainer(device, "Scores");

               // Get the path of the save game.
               string filename = "highScores.sav";
               
               // Check to see whether the save exists.
               if (!container.FileExists(filename))
               {
                    // Notify the user there is no save.
                    container.CreateFile(filename);

                    AvatarTypingGame.HighScoreSaveData = new Leaderboards.TopScoreListContainer(1, 10);
               }
               
               // Open the file.
               else
               {
                    // Open the file, creating it if necessary.
                    using (Stream stream = container.OpenFile(filename, FileMode.Open, FileAccess.Read))//filename, FileMode.Create))
                    {
                         BinaryReader reader = new BinaryReader(stream);

                         if (stream.Length <= 0)
                         {
                           //   AvatarTypingGame.HighScoreSaveData = new Leaderboards.TopScoreListContainer(1, 10);
                         }

                         else
                         {
                              AvatarTypingGame.HighScoreSaveData = new Leaderboards.TopScoreListContainer(reader);
                         }

                         reader.Close();
                    }
               }

               // Dispose the container, to commit changes.
               container.Dispose();
          }

          #region SaveGame and LoadGame - Temporary Locations

          public static void SaveGame(StorageDevice device)
          {
               SaveGameData data = new SaveGameData();

               data = AvatarTypingGame.SaveGameData;
               data.IsUnlockedAchievement = AchievementManager.IsUnlockedList;
               data.AwardData = AvatarTypingGame.AwardData;
               data.Difficulty = (int)AvatarTypingGameSettings.Difficulty;
               data.FontSize = (int)AvatarTypingGameSettings.FontSize;

               data.SoundVolume = AvatarTypingGameSettings.SoundVolume;
               data.MusicVolume = AvatarTypingGameSettings.MusicVolume;

               AudioManager.SoundAudioCategory.SetVolume(MathHelper.Clamp(AvatarTypingGameSettings.SoundVolume / 10f, 0.0f, 1.0f));
               AudioManager.MusicAudioCategory.SetVolume(MathHelper.Clamp(AvatarTypingGameSettings.MusicVolume / 10f, 0.0f, 1.0f));

               if (device != null && device.IsConnected)
               {
                    StorageManager.DoSaveGame(device, data);
               }
          }

          public static bool LoadGame(StorageDevice device)
          {
               bool saveExisted = false;
               
               saveExisted = StorageManager.DoLoadGame(device);

               AvatarTypingGame.SaveGameData = StorageManager.SaveData;
               AvatarTypingGame.AwardData = AvatarTypingGame.SaveGameData.AwardData;

               // Test for achievement fix
               AvatarTypingGame.AwardData.CurrentWave = 0;
               // End Test for achievement fix

               // Grab the User's Sentences.
               SentenceDatabase.UserSentences = new List<string>();

               if (AvatarTypingGame.SaveGameData.WordList != null)
               {
                    foreach (string sentence in AvatarTypingGame.SaveGameData.WordList)
                    {
                         SentenceDatabase.UserSentences.Add(sentence);
                    }
               }

               // Grab the User's Achievements.
               if (AvatarTypingGame.SaveGameData.IsUnlockedAchievement != null)
               {
                    int i = 0;

                    foreach (bool isUnlocked in AvatarTypingGame.SaveGameData.IsUnlockedAchievement)
                    {
                         AchievementManager.Achievements[i++].IsUnlocked = isUnlocked;
                    }
               }

               // If a previous save does not exist...
               if (!saveExisted)
               {
                    // We need to default various Settings.
                    AvatarTypingGame.SaveGameData.FontSize = (int)FontSize.Medium;
                    AvatarTypingGame.SaveGameData.SoundVolume = 10;
                    AvatarTypingGame.SaveGameData.MusicVolume = 10;
               }

               // Grab the User's Settings.
               AvatarTypingGameSettings.Difficulty = (Difficulty)AvatarTypingGame.SaveGameData.Difficulty;
               AvatarTypingGameSettings.FontSize = (FontSize)AvatarTypingGame.SaveGameData.FontSize;
               AvatarTypingGameSettings.SoundVolume = AvatarTypingGame.SaveGameData.SoundVolume;
               AvatarTypingGameSettings.MusicVolume = AvatarTypingGame.SaveGameData.MusicVolume;
               
               return saveExisted;
          }

          #endregion

          #region Constructor

          /// <summary>
          /// The main Game constructor.
          /// </summary>
          public AvatarTypingGame(GraphicsInfo graphics)
               : base(graphics)
          {
               Content.RootDirectory = "Content";

               #region Debug: Safe Area
               // Draw Safe Area if in Debug && Xbox
#if XBOX && DEBUG
               PixelEngine.DebugUtilities.SafeArea.SafeAreaOverlay safeAreaOverlay;
               safeAreaOverlay = new PixelEngine.DebugUtilities.SafeArea.SafeAreaOverlay(this);
               Components.Add(safeAreaOverlay);
               safeAreaOverlay.Visible = true;
#endif
               #endregion

               // Set presence mode to "At Menu".
               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode = GamerPresenceMode.AtMenu;
               }

               // Simulate Trial Mode.
               //Guide.SimulateTrialMode = true;

               SignedInGamer.SignedIn += new EventHandler<SignedInEventArgs>(SignedInGamer_SignedIn);
               SignedInGamer.SignedOut += new System.EventHandler<SignedOutEventArgs>(SignedInGamer_SignedOut);
          }

          #endregion

          public static Leaderboards.OnlineDataSyncManager OnlineSyncManager;

          #region Initialize

          protected override void Initialize()
          {
               // Create and add the Screen Manager component.
               ScreenManager.Initialize(this);

               // Add a GamerServiceComponent to list.
               Components.Add(new GamerServicesComponent(this));

               // Create and add the Audio Manager.
               AudioManager.Initialize(this, @"Content\MyGameAudio.xgs", @"Content\Wave Bank.xwb", @"Content\Sound Bank.xsb");

               // Create and add the Award Manager.
               AchievementManager.Initialize(this);

               // Create and add the Text Handler.
               TextManager.Initialize(this);
               
               // Create and add the Camera Manager.
               CameraManager.Initialize(this);

               // Create and add the Content Manager.
               ResourceManager.Initialize(this);

               AvatarManager.Initialize(this);

               //this.IsFixedTimeStep = false;
               //Components.Add(new PixelEngine.DebugUtilities.SafeArea.SafeAreaOverlay(this));
               //PixelEngine.DebugUtilities.FpsCounter.Initialize(this);

               OnlineSyncManager = new Leaderboards.OnlineDataSyncManager(0, this);
               Components.Add(OnlineSyncManager);

               AchievementList.InitializeAchievements();
               
               base.Initialize();
               
               // Activate the first screen.
               PixelEngine.Screen.ScreenManager.AddScreen(new SplashScreen(), null);
          }

          #endregion

          #region Draw

          /// <summary>
          /// This is called when the game should draw itself.
          /// </summary>
          protected override void Draw(GameTime gameTime)
          {
               // The real drawing happens inside the EngineCore,
               // which calls upon game components such as ScreenManager.
               base.Draw(gameTime);

               if (ShowSignedOutMessageBox)
               {
                    if (!Guide.IsVisible)
                    {
                         Guide.BeginShowMessageBox(ControllingPlayer.Value,
                                   "Sign-In Status Changed",
                                   "You have returned to the main menu because the number of signed-in players changed.",
                                   new string[] { "Continue" }, 0, MessageBoxIcon.None, SignInCompleteCallback, null);
                    }
               }
          }

          #endregion

          #region Entry Point

          /// <summary>
          /// The main entry point for the application.
          /// </summary>
          static void Main(string[] args)
          {
               using (AvatarTypingGame game = new AvatarTypingGame(gi))
               {
                    game.Run();
               }
          }

          #endregion

          #region SignedInGamer Events

          /// <summary>
          /// Event hooked to when a gamer Signs In.
          /// Note: This also is triggered automatically upon game-start.
          /// </summary>
          /// <param name="sender"></param>
          /// <param name="e"></param>
          void SignedInGamer_SignedIn(object sender, SignedInEventArgs e)
          {
               AvatarTypingGame.CurrentPlayer = new Player(e.Gamer.PlayerIndex);
               
               AvatarTypingGame.CurrentPlayer.GamerInfo.Gamer = e.Gamer;

               AvatarTypingGame.SaveGameData.PlayerName = e.Gamer.Gamertag;
          }

          public bool ShowSignedOutMessageBox = false;

          /// <summary>
          /// Event hooked to when a gamer Signs Out.
          /// </summary>
          /// <param name="sender"></param>
          /// <param name="e"></param>
          void SignedInGamer_SignedOut(object sender, SignedOutEventArgs e)
          {
               // Check to see if the Gamer that Signed-Out is our Current Player.
               if (AvatarTypingGame.CurrentPlayer.GamerInfo.Gamer == e.Gamer)
               {
                    GameplayBackgroundScreen.isUpdate = true;

                    // New Testing
                    AnimatedLoadingScreen.RemoveAllButGameplayScreen();
                    AvatarTypingGame.CurrentPlayer = null;



                    // NEW FOR LEADERBOARDS

                    AvatarTypingGame.OnlineSyncManager.stop(null, false);

                    // END NEW FOR LEADERBOARDS


                    ScreenManager.AddScreen(new StartScreen(), e.Gamer.PlayerIndex);

                    ShowSignedOutMessageBox = true;
               }
          }

          void SignInCompleteCallback(IAsyncResult r)
          {
               int? button = Guide.EndShowMessageBox(r);

               ShowSignedOutMessageBox = false;
          }

          #endregion
     }
}
