#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using PixelEngine;
using PixelEngine.AchievementSystem;
using PixelEngine.CameraSystem;
using PixelEngine.Menu;
using PixelEngine.Screen;
using PixelEngine.Text;
using PixelEngine.Storage;
#endregion

namespace AvatarTyping
{
     /// <remarks>
     /// The Main Menu screen is the first thing displayed when the game starts up.
     /// It displays the title's picture, as well as a number of menu entries:
     /// Play Game, Options, View Credits, View Awards, View Unlockables, Exit.
     /// </remarks>
     public class MainMenuScreen : MenuScreen
     {
          #region Fields

          ContentManager content;
          SpriteBatch spriteBatch;
          SpriteFont ChatFont;

          // Create our menu entries.
          MenuEntry playGameMenuEntry = new MenuEntry("Single Player", "Play A Solo Game of Arcade Mode\nor Survival Mode."); 
          MenuEntry optionsMenuEntry = new MenuEntry("Help & Options", "Get Help &\nCustomize Your Experience.");
          MenuEntry extrasMenuEntry = new MenuEntry("Extras", "View Your Awards, Watch The Credits,\nOr Tell A Friend About Avatar Typing!");
          MenuEntry editorMenuEntry = new MenuEntry("Sentence Editor", "Want New Sentences to Type?\nCreate Your Own Here!");
          MenuEntry exitMenuEntry = new MenuEntry("Exit", "Exit Back to the Dashboard.");


          MenuEntry scoreEntry = new MenuEntry("Scores", "SCores");

          MenuEntry purchaseGameMenuEntry;

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor fills in the menu contents.
          /// </summary>
          public MainMenuScreen()
               : base("M a i n  M e n u")
          {
               CameraManager.SetActiveCamera(CameraManager.CameraNumber.ThirdPerson);
               CameraManager.ActiveCamera.Reset(EngineCore.GraphicsDevice.Viewport);
               CameraManager.ActiveCamera.Position = new Vector3(0, 5f, -5f);
               CameraManager.ActiveCamera.LookAt = new Vector3(0f, 0f, 20f);

               AchievementManager.IsUnlockNow = true;

               TextManager.Reset();

               TransitionOnTime = TimeSpan.FromSeconds(1.5f);
               TransitionOffTime = TimeSpan.FromSeconds(1.0f);

               // Hook up menu event handlers.
               playGameMenuEntry.Selected += SinglePlayerMenuEntrySelected;
               optionsMenuEntry.Selected += HelpAndOptionsMenuEntrySelected;
               extrasMenuEntry.Selected += ExtrasMenuEntrySelected;
               editorMenuEntry.Selected += SentenceEditorMenuEntrySelected;

               exitMenuEntry.Selected += OnCancel;



               scoreEntry.Selected += ScoreMenuEntrySelected;

               // Add entries to the menu.
               MenuEntries.Add(playGameMenuEntry);
               MenuEntries.Add(optionsMenuEntry);
               MenuEntries.Add(extrasMenuEntry);
               MenuEntries.Add(editorMenuEntry);
               MenuEntries.Add(exitMenuEntry);



               MenuEntries.Add(scoreEntry);

               float menuScale = 1f;
               float additionalSpacing = 20f;

               if (Guide.IsTrialMode)
               {
                    purchaseGameMenuEntry = new MenuEntry("Purchase Game!", "Buy the FULL Version of Avatar Typing");
                    purchaseGameMenuEntry.Selected += PurchaseGameEntrySelected;
                    MenuEntries.Add(purchaseGameMenuEntry);

                    menuScale = 0.8f;
                    additionalSpacing = 10f;
               }

               foreach (MenuEntry entry in MenuEntries)
               {
                    entry.Position = new Vector2(entry.Position.X, entry.Position.Y + 10);
                    entry.AdditionalVerticalSpacing = additionalSpacing;
                    entry.menuEntryBorderScale = new Vector2(1.0f, menuScale);//0.80f);
                    entry.IsPulsating = false;
                    entry.SelectedColor = entry.UnselectedColor;
               }

               exitMenuEntry.IsPulsating = true;
          }

          public override void LoadContent()
          {
               base.LoadContent();

               if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

               spriteBatch = ScreenManager.SpriteBatch;

               ChatFont = ScreenManager.Font;
          }

          public override void UnloadContent()
          {
               base.UnloadContent();
          }

          #endregion

          #region Menu Entry Events

          /// <summary>
          /// Event handler for when the Single Player menu entry is selected.
          /// </summary>
          void SinglePlayerMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.AddScreen(new ModeSelectScreen(), e.PlayerIndex);
          }

          /// <summary>
          /// Event handler for when the Multiplayer menu entry is selected.
          /// </summary>
          void MultiplayerMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               //ScreenManager.AddScreen(new NetworkStateManagement.MainMenuScreen(), e.PlayerIndex);
          }

          /// <summary>
          /// Event handler for when the Help & Options menu entry is selected.
          /// </summary>
          void HelpAndOptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.AddScreen(new HelpAndOptionsMenuScreen(), e.PlayerIndex);
          }

          /// <summary>
          /// Event handler for when the Extras menu entry is selected.
          /// </summary>
          void ExtrasMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.AddScreen(new ExtrasMenuScreen(), e.PlayerIndex);
          }

          /// <summary>
          /// Event handler for when the Purchase Game menu entry is selected.
          /// This is only present in Trial Mode.
          /// </summary>
          void PurchaseGameEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               if (!AvatarTypingGame.CurrentPlayer.GamerInfo.CanBuyGame())
               {
                    SimpleGuideMessageBox.ShowMessageBox(ControllingPlayer, "Purchasing Game Disabled",
                         "You cannot purchase the Full version of Avatar Typing. To purchase the Full version, please make sure you are signed into Live, on a non-Guest account, and have the appropriate Privileges.",
                         new string[] { "OK" }, 0, MessageBoxIcon.Warning);

                    return;
               }

               // Do Purchase Guide Stuff
               if (Guide.IsTrialMode)
               {
                    Guide.ShowMarketplace(AvatarTypingGame.CurrentPlayer.GamerInfo.PlayerIndex);

                    // Set SaveEnabled to true?
               }
          }

          /// <summary>
          /// Event handler for when the Sentence Editor menu entry is selected.
          /// </summary>
          void SentenceEditorMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.AddScreen(new WordEditorScreen(), e.PlayerIndex);
          }

          void ScoreMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.AddScreen(new GlobalHighScores(), e.PlayerIndex);
          }

          /// <summary>
          /// When the user cancels the main menu, ask if they want to exit the game.
          /// </summary>
          protected override void OnCancel(PlayerIndex playerIndex)
          {
               const string message = "Are you sure you want to exit the game?";

               MessageBoxWithKeyboardScreen confirmExitMessageBox = new MessageBoxWithKeyboardScreen(message);

               confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

               ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
          }

          /// <summary>
          /// Event handler for when the user selects Accept on the "are you sure
          /// you want to exit" message box.
          /// </summary>
          void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
          {
               if (Guide.IsTrialMode)
               {
                    ScreenManager.AddScreen(new BuyScreen(), e.PlayerIndex);
               }

               else
               {
#if XBOX
                    // Set the request flag
                    if (StorageManager.SaveRequested == false)
                    {
                         StorageManager.SaveRequested = true;
                    }
#endif
               }

          }

          #endregion

          #region Update

          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               //if (IsActive)
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               #region Handle Menu Differences When In Trial / Guest or Non-Live Profiles

               // Handle the appearance of Purchase Game Menu Entry.
               if (!Guide.IsTrialMode)
               {
                    if (purchaseGameMenuEntry != null)
                    {
                         if (MenuEntries.Contains(purchaseGameMenuEntry))
                         {
                              MenuEntries.RemoveAt(MenuEntries.Count - 1);
                              SelectedMenuEntry = 0;

                              AvatarTypingGame.GamePurchased = true;
                              AvatarTypingGame.AwardData.GamePurchased = true;
                         }
                    }
               }

               #endregion

               #region Handle Pending Save Request

               // If a save is pending, save as soon as the storage device is chosen
               if ((StorageManager.SaveRequested))
               {
                    StorageDevice device = StorageManager.Device;

                    if (device != null && device.IsConnected)
                    {
                         AvatarTypingGame.SaveGame(device);
                    }

                    // Reset the request flag
                    StorageManager.SaveRequested = false;

                    //ScreenManager.Game.Exit();

                    AvatarTypingGame.OnlineSyncManager.stop(delegate() { ScreenManager.Game.Exit(); },
                         true);
               }

               #endregion
          }

          #endregion

          #region Draw

          public override void Draw(GameTime gameTime)
          {
               //if (IsActive)
               {
                    base.Draw(gameTime);
               }
          }

          #endregion
     }
}
