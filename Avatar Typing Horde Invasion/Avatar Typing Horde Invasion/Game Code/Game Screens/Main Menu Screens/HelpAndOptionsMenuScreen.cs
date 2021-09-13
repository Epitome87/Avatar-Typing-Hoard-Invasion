
#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using PixelEngine.Menu;
using PixelEngine.Screen;
#endregion

namespace AvatarTyping
{
     /// <remarks>
     /// A Menu Screen which presents menu entries for viewing
     /// Options, Controls, and How to Play.
     /// </remarks>
     public class HelpAndOptionsMenuScreen : MenuScreen
     {
          #region Fields

          MenuEntry optionsMenuEntry;
          MenuEntry controlsMenuEntry;
          MenuEntry howToPlayMenuEntry;
          MenuEntry enemyInfoMenuEntry;
          MenuEntry backMenuEntry;

          #endregion
          
          #region Initialization

          public HelpAndOptionsMenuScreen()
               : base("Help & Options")
          {
               this.TransitionOnTime = TimeSpan.FromSeconds(1.5f);
               this.TransitionOffTime = TimeSpan.FromSeconds(1.0f);

               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.ConfiguringSettings;
               }

               howToPlayMenuEntry = new MenuEntry("How To Play", "Learn How To Play Avatar Typing.");
               controlsMenuEntry = new MenuEntry("Controls", "Learn the Controls.");
               optionsMenuEntry = new MenuEntry("Settings", "Customize Difficulty, Audio Options,\nFont Size, & More.");
               enemyInfoMenuEntry = new MenuEntry("Enemy Information", "Learn About the Different Types of Enemies.");
               backMenuEntry = new MenuEntry("Back", "Return to the Main Menu.");

               backMenuEntry.Position = new Vector2(backMenuEntry.Position.X, backMenuEntry.Position.Y + 50);

               howToPlayMenuEntry.Selected += HowToPlayMenuEntrySelected;
               controlsMenuEntry.Selected += ControlsMenuEntrySelected;
               optionsMenuEntry.Selected += OptionsMenuEntrySelected;
               enemyInfoMenuEntry.Selected += EnemyInfoMenuEntrySelected;
               backMenuEntry.Selected += OnCancel;

               MenuEntries.Add(howToPlayMenuEntry);
               MenuEntries.Add(controlsMenuEntry);
               MenuEntries.Add(optionsMenuEntry);
               MenuEntries.Add(enemyInfoMenuEntry);
               MenuEntries.Add(backMenuEntry);

               foreach (MenuEntry entry in MenuEntries)
               {
                    entry.AdditionalVerticalSpacing = 20;
                    entry.Position = new Vector2(entry.Position.X, entry.Position.Y + 40);
                    entry.IsPulsating = false;
                    entry.ShowBorder = false;
                    entry.SelectedColor = entry.UnselectedColor;
               }

               backMenuEntry.IsPulsating = true;
          }

          #endregion

          #region Menu Events

          /// <summary>
          /// Event handler for when the How To Play menu entry is selected.
          /// </summary>
          void HowToPlayMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.AddScreen(new HowToPlayScreen(), e.PlayerIndex);
          }

          /// <summary>
          /// Event handler for when the How To Play menu entry is selected.
          /// </summary>
          void ControlsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.AddScreen(new ControlsMenuScreen(), e.PlayerIndex);
          }

          /// <summary>
          /// Event handler for when the How To Play menu entry is selected.
          /// </summary>
          void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.AddScreen(new SettingsMenuScreen(), e.PlayerIndex);
          }

          /// <summary>
          /// Event handler for when the How To Play menu entry is selected.
          /// </summary>
          void EnemyInfoMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.AddScreen(new EnemyInfoMenuScreen(), e.PlayerIndex);
          }

          protected override void OnCancel(Microsoft.Xna.Framework.PlayerIndex playerIndex)
          {
               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.AtMenu;
               }

               base.OnCancel(playerIndex);
          }

          #endregion

          #region Draw

          /// <summary>
          /// Draws the screen. This darkens down the gameplay screen
          /// that is underneath us, and then chains to the base MenuScreen.Draw.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 3 / 5);

               base.Draw(gameTime);
          }

          #endregion
     }
}