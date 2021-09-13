#region File Description
//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.Menu;
using PixelEngine.Screen;
#endregion

namespace AvatarTyping
{
     /// <remarks>
     /// The Pause menu pops up over the game screen,
     /// giving the player options to resume or quit.
     /// </remarks>
     public class PauseMenuScreen : MenuScreen
     {
          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public PauseMenuScreen()
               : base("Game Paused")
          {
               // Flag that there is no need for the game to transition
               // off when the pause menu is on top of it.
               IsPopup = true;

               // Create our menu entries.
               MenuEntry resumeGameMenuEntry = new MenuEntry("Resume Game", "Continue Playing Avatar Typing.");
               MenuEntry optionsMenuEntry = new MenuEntry("- Help & Options -", "Get Help &\nCustomize Your Experience.");
               MenuEntry quitGameMenuEntry = new MenuEntry("Quit Game", "Quit & Return to the Main Menu.\nNote: Current Progress Will Be Lost.");

               // Hook up menu event handlers.
               resumeGameMenuEntry.Selected += OnCancel;
               optionsMenuEntry.Selected += HelpAndOptionsMenuEntrySelected;
               quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;


               // Set the menu entrys' positions.
               resumeGameMenuEntry.Position = new Vector2(resumeGameMenuEntry.Position.X, resumeGameMenuEntry.Position.Y + 50);
               optionsMenuEntry.Position = new Vector2(optionsMenuEntry.Position.X, optionsMenuEntry.Position.Y + 100);
               quitGameMenuEntry.Position = new Vector2(quitGameMenuEntry.Position.X, quitGameMenuEntry.Position.Y + 150);

               resumeGameMenuEntry.SelectedColor = Color.DarkOrange;
               optionsMenuEntry.SelectedColor = Color.DarkOrange;
               quitGameMenuEntry.SelectedColor = Color.DarkOrange;

               // Add entries to the menu.
               MenuEntries.Add(resumeGameMenuEntry);
               MenuEntries.Add(optionsMenuEntry);
               MenuEntries.Add(quitGameMenuEntry);

               foreach (MenuEntry entry in MenuEntries)
               {
                    entry.IsPulsating = false;
                    entry.ShowBorder = false;
                    entry.SelectedColor = entry.UnselectedColor;
               }

               quitGameMenuEntry.IsPulsating = true;
          }

          #endregion

          #region Menu Entry Events

          /// <summary>
          /// Event handler for when the Help & Options menu entry is selected.
          /// </summary>
          void HelpAndOptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.AddScreen(new HelpAndOptionsMenuScreen(), e.PlayerIndex);
          }

          #endregion

          #region Handle Input

          /// <summary>
          /// Event handler for when the Quit Game menu entry is selected.
          /// </summary>
          void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               const string message = "Are you sure you want to quit this game?";

               MessageBoxWithKeyboardScreen confirmQuitMessageBox = new MessageBoxWithKeyboardScreen(message);

               confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

               ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);
          }

          /// <summary>
          /// Event handler for when the user selects ok on the "are you sure
          /// you want to quit" message box. This uses the loading screen to
          /// transition from the game back to the main menu screen.
          /// </summary>
          void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.RemoveScreen(this);

               GameplayBackgroundScreen.isUpdate = true;

               ScreenManager.AddScreen(new MainMenuScreen(), e.PlayerIndex);
          }

          #endregion

          #region Draw

          /// <summary>
          /// Draws the pause menu screen. This darkens down the gameplay screen
          /// that is underneath us, and then chains to the base MenuScreen.Draw.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 4 / 5);

               base.Draw(gameTime);
          }

          #endregion
     }
}
