#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using PixelEngine.Audio;
using PixelEngine.Menu;
using PixelEngine.Screen;
using PixelEngine.Storage;
#endregion

namespace AvatarTyping
{
     /// <remarks>
     /// The Options screen is brought up over the top of the Main Menu
     /// screen, and gives the user a chance to configure the game.
     /// Menu Entries include: Difficulty, Language, Vibration, Music.
     /// </remarks>
     public class SettingsMenuScreen : MenuScreen
     {
          #region Fields

          // The Menu Entries.
          private MenuEntry difficultyMenuEntry;
          //private MenuEntry vibrationMenuEntry;
          //private MenuEntry musicMenuEntry;
          private BarMenuEntry musicVolumeEntry;
          private BarMenuEntry soundVolumeEntry;
          private MenuEntry fontSizeMenuEntry;
          private MenuEntry storageMenuEntry;

          // Helper variables for displaying the Menu Entry's contents.
          private static Difficulty currentDifficulty = AvatarTypingGameSettings.Difficulty;
          //private static bool vibration = true;
          //private static bool music = true;
          private static FontSize currentFontSize = AvatarTypingGameSettings.FontSize;

          //private bool isVibrating;
          //private float elapsedTime;

          private bool storageRequested = false;

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public SettingsMenuScreen()
               : base("S E T T I N G S")
          {
               this.TransitionOnTime = TimeSpan.FromSeconds(1.5f);
               this.TransitionOffTime = TimeSpan.FromSeconds(1.0f);

               // Create our menu entries.
               difficultyMenuEntry = new MenuEntry(String.Empty, "Recommended for players with Chat Pads.\nSlow enemies & no punctuation marks.\nScore Multiplier: x 0.5");
               //vibrationMenuEntry = new MenuEntry(String.Empty, "Enable or disable controller vibration.");
               //musicMenuEntry = new MenuEntry(String.Empty, "Enable or disable the music\nplaying in the background.");
               musicVolumeEntry = new BarMenuEntry(String.Empty, "Change the volume level of the music.");
               soundVolumeEntry = new BarMenuEntry(String.Empty, "Change the volume level of sound effects.");
               fontSizeMenuEntry = new MenuEntry(String.Empty, "The size for the gameplay screen font.");
               storageMenuEntry = new MenuEntry(String.Empty, "Change the Storage Device used for saving,\nif multiple devices are present.\nAlso saves current game.");

               // Check to see if the Pause Menu is up, meaning we are visiting Settings from in-game.
               for (int i = 0; i < ScreenManager.GetScreens().Length; i++)
               {
                    // And if we are in the Pause Menu...
                    if (ScreenManager.GetScreens()[i].GetType().Equals(typeof(PauseMenuScreen)))
                    {
                         // Do not let the user mess with Difficulty!
                         difficultyMenuEntry.SelectedColor = Color.Gray;
                         difficultyMenuEntry.UnselectedColor = Color.Gray;
                         difficultyMenuEntry.Description = "Cannot change Difficulty while in-game.\nReturn to Main Menu to change Difficulty.";
                    }
               }


               this.TransitionOnTime = TimeSpan.FromSeconds(1.0);
               this.TransitionOffTime = TimeSpan.FromSeconds(1.0);

               SetDifficulty();
               SetFontSize();
               SetMenuEntryText();

               MenuEntry backMenuEntry = new MenuEntry("Back", "Return to the Main Menu.");
               backMenuEntry.Position = new Vector2(backMenuEntry.Position.X, backMenuEntry.Position.Y + 30f);

               // Set the Bars for the Music and Sound entries to full.
               musicVolumeEntry.CurrentBar = AvatarTypingGameSettings.MusicVolume;// startingMusicVolume;// musicVolumeEntry.NumberOfBars;
               soundVolumeEntry.CurrentBar = AvatarTypingGameSettings.SoundVolume;// startingSoundVolume;//soundVolumeEntry.NumberOfBars;

               // Hook up menu event handlers.
               difficultyMenuEntry.Selected += DifficultyMenuEntrySelected;
               //vibrationMenuEntry.Selected += VibrationMenuEntrySelected;
               //musicMenuEntry.Selected += MusicMenuEntrySelected;
               musicVolumeEntry.Selected += MusicVolumeEntrySelected;
               soundVolumeEntry.Selected += SoundVolumeEntrySelected;
               fontSizeMenuEntry.Selected += FontSizeMenuEntrySelected;
               storageMenuEntry.Selected += StorageMenuEntrySelected;
               backMenuEntry.Selected += OnCancel;



               if (Guide.IsTrialMode)
               {
                    storageMenuEntry.SelectedColor = Color.Gray;
                    storageMenuEntry.UnselectedColor = Color.Gray;
                    storageMenuEntry.Description = "\nNot Available In Trial Mode.";
               }




               // Add entries to the menu.
               MenuEntries.Add(difficultyMenuEntry);
               //MenuEntries.Add(vibrationMenuEntry);
               //MenuEntries.Add(musicMenuEntry);
               MenuEntries.Add(musicVolumeEntry);
               MenuEntries.Add(soundVolumeEntry);
               MenuEntries.Add(fontSizeMenuEntry);
               MenuEntries.Add(storageMenuEntry);
               //MenuEntries.Add(backMenuEntry);


               foreach (MenuEntry entry in MenuEntries)
               {
                    entry.AdditionalVerticalSpacing = 20;
                    entry.menuEntryBorderSize = new Vector2(550f, 100f);
                    entry.Position = new Vector2(entry.Position.X, entry.Position.Y + 15);
                    entry.IsPulsating = false;
                    entry.ShowBorder = false;
                    entry.SelectedColor = entry.UnselectedColor;
                    entry.ShowGradientBorder = true;
               }

               //elapsedTime = 0.0f;     
          }

          /// <summary>
          /// Fills in the latest values for the options screen menu text.
          /// </summary>
          void SetMenuEntryText()
          {
               difficultyMenuEntry.Text = "Difficulty: " + currentDifficulty;
               //vibrationMenuEntry.Text = "Vibration: " + (vibration ? "On" : "Off");
               //musicMenuEntry.Text = "Music: " + (music ? "On" : "Off");
               musicVolumeEntry.Text = "Music Volume:         ";
               soundVolumeEntry.Text = "Sound Volume:         ";
               fontSizeMenuEntry.Text = "Font Size: " + currentFontSize;

               storageMenuEntry.Text = "Select Storage Device";
               storageMenuEntry.Description = "Change the Storage Device used for saving,\nif multiple devices are present.\nAlso saves current game.";
          }

          #endregion

          #region Handle Input

          /// <summary>
          /// Event handler for when the Difficulty menu entry is selected.
          /// </summary>
          void DifficultyMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               for (int i = 0; i < ScreenManager.GetScreens().Length; i++)
               {
                    if (ScreenManager.GetScreens()[i].GetType().Equals(typeof(PauseMenuScreen)))
                         return;
               }

               // Current design solution for Award glitch.
               AvatarTypingGame.AwardData.CurrentWave = 0;
               // End current design solution for Award glitch.

               currentDifficulty++;

               if (currentDifficulty > Difficulty.Insane)
                    currentDifficulty = 0;

               AvatarTypingGameSettings.Difficulty = currentDifficulty;

               SetDifficulty();
               SetMenuEntryText();
          }

          /// <summary>
          /// Event handler for when the Music Volume menu entry is selected.
          /// </summary>
          void MusicVolumeEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               AudioManager.MusicAudioCategory.SetVolume(
                    MathHelper.Clamp(musicVolumeEntry.Value, 0.0f, 1.0f));

               AvatarTypingGameSettings.MusicVolume = musicVolumeEntry.CurrentBar;
          }

          /// <summary>
          /// Event handler for when the Sound Volume menu entry is selected.
          /// </summary>
          void SoundVolumeEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               AudioManager.SoundAudioCategory.SetVolume(
                    MathHelper.Clamp(soundVolumeEntry.Value, 0.0f, 1.0f));

               AvatarTypingGameSettings.SoundVolume = soundVolumeEntry.CurrentBar;
          }

          void FontSizeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               currentFontSize++;
               
               if (currentFontSize > FontSize.ExtraLarge)
                    currentFontSize = 0;

               SetFontSize();
               SetMenuEntryText();
          }

          IAsyncResult result;

          private void StorageMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               if (Guide.IsTrialMode)
                    return;

               bool doingStorageShit = true;

               if (doingStorageShit)
               {
                    doingStorageShit = false;

                    if ((!Guide.IsVisible) && !storageRequested)
                    {
                         try
                         {
                              result = StorageDevice.BeginShowSelector(e.PlayerIndex, null, null);

                              storageRequested = true;
                         }
                         catch
                         {
                         }
                    }
               }
          }

          private void SetDifficulty()
          {
               switch (currentDifficulty)
               {
                    case Difficulty.Easy:
                         difficultyMenuEntry.Description = "Recommended for players with Chat Pads.\nSlow enemies & no punctuation marks.\nScore Multiplier: x 0.5";
                         break;

                    case Difficulty.Normal:
                         difficultyMenuEntry.Description = "Recommended for players who \"peck\" type.\nEnemies are slow.\nScore Multiplier: x 1.0";
                         break;

                    case Difficulty.Hard:
                         difficultyMenuEntry.Description = "Recommended for players with USB keyboards.\nOn Hard, enemies are fast.\nScore Multiplier: x 1.5";
                         break;

                    case Difficulty.Insane:
                         difficultyMenuEntry.Description = "Recommended for fast typers.\nOn Insane, enemies are even faster.\nScore Multiplier: x 2.0";
                         break;
               }
          }

          private void SetFontSize()
          {
               string description = "The size for the gameplay screen font.\n";

               switch (currentFontSize)
               {
                    case FontSize.ExtraSmall:
                         fontSizeMenuEntry.Description = description + "Extra Small: Recommended for larger HD televisions.";
                         break;

                    case FontSize.Small:
                         fontSizeMenuEntry.Description = description + "Small: Recommended for HD televisions.";
                         break;

                    case FontSize.Medium:
                         fontSizeMenuEntry.Description = description + "Medium: Recommended for HD or SD televisions.";
                         break;

                    case FontSize.Large:
                         fontSizeMenuEntry.Description = description + "Large: Recommended for SD televisions.";
                         break;

                    case FontSize.ExtraLarge:
                         fontSizeMenuEntry.Description = description + "Extra Large: For older players on SD televisions.";
                         break;
               }

               AvatarTypingGameSettings.FontSize = currentFontSize;
          }

          /// <summary>
          /// Overridden Event handler for when the user cancels the menu.
          /// </summary>
          protected override void OnCancel(PlayerIndex playerIndex)
          {
               base.OnCancel(playerIndex);
               GamePad.SetVibration(playerIndex, 0.0f, 0.0f);
          }

          #endregion

          #region Update

          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
               
               /*
               // Unless we are on the Vibration entry, we do not want the controller to vibrate.
               if (SelectedMenuEntry != 1)
               {
                    GamePad.SetVibration(ControllingPlayer.Value, 0.0f, 0.0f);

                    isVibrating = false;
                    elapsedTime = 0.0f;
               }

               // We are on Vibration entry, so let's check for that hidden Award!
               else
               {
                    if (isVibrating)
                    {
                         elapsedTime += gameTime.ElapsedGameTime.Milliseconds;

                         if (elapsedTime >= 30000.0f)
                         {
                              AvatarTypingGame.AwardData.VibrationSet = true;
                         }
                    }
               }
               */

               // If a save is pending, save as soon as the storage device is chosen
               if (storageRequested && result != null)
               {
                    if (result.IsCompleted)
                    {
                         StorageDevice device = StorageDevice.EndShowSelector(result);

                         StorageManager.Device = device;

                         StorageDevice.DeviceChanged += new EventHandler<EventArgs>(StorageManager.StorageDevice_DeviceChanged);

                         if (device != null && device.IsConnected)
                         {
                              AvatarTypingGame.SaveGame(device);
                         }

                         // Reset the request flag.
                         storageRequested = false;

                         storageMenuEntry.Description = "Storage selection & save complete!\nIf you weren't prompted to choose a device,\nyou don't have multiple present.";
                    }
               }
          }

          #endregion

          #region Draw

          public override void Draw(GameTime gameTime)
          {
               ScreenManager.FadeBackBufferToBlack(255 * 3 / 5);

               base.Draw(gameTime);
          }

          #endregion
     }
}
