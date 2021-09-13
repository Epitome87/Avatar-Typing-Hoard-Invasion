#region File Description
//-----------------------------------------------------------------------------
// ExtrasMenuScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

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
     /// A MenuScreen which contains a list of other MenuScreens, 
     /// which represent "Extras" the Player can opt to view.
     /// 
     /// "Extras" include the Credits screen, the Awards screen, and the Tell A Friend screen.
     /// </remarks>
     public class ExtrasMenuScreen : MenuScreen
     {
          #region Fields

          MenuEntry creditsMenuEntry;
          MenuEntry awardsMenuEntry;
          MenuEntry tellFriendMenuEntry;
          MenuEntry backMenuEntry;

          #endregion

          #region Initialization

          public ExtrasMenuScreen()
               : base("E X T R A S")
          {
               this.TransitionOnTime = TimeSpan.FromSeconds(1.5f);
               this.TransitionOffTime = TimeSpan.FromSeconds(1.0f);

               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.WastingTime;
               }

               // Create our menu entries.
               creditsMenuEntry = new MenuEntry("View Credits", "Roll the Credits!");
               awardsMenuEntry = new MenuEntry("View Awards", "View the Awards You Have Earned!");
               tellFriendMenuEntry = new MenuEntry("Tell A Friend!", "Tell Friends About Avatar Typing: Horde Invasion.\nPlease?!");
               backMenuEntry = new MenuEntry("Back", "Return to the Main Menu.");

               backMenuEntry.Position = new Vector2(backMenuEntry.Position.X, backMenuEntry.Position.Y + 50);

               // Hook up menu event handlers.
               awardsMenuEntry.Selected += AwardsMenuEntrySelected; ;
               tellFriendMenuEntry.Selected += TellFriendMenuEntrySelected;
               creditsMenuEntry.Selected += CreditsMenuEntrySelected;
               backMenuEntry.Selected += OnCancel;

               // Add entries to the menu.
               MenuEntries.Add(awardsMenuEntry);
               MenuEntries.Add(creditsMenuEntry);
               MenuEntries.Add(tellFriendMenuEntry);
               MenuEntries.Add(backMenuEntry);

               foreach (MenuEntry entry in MenuEntries)
               {
                    entry.AdditionalVerticalSpacing = 20;
                    entry.Position = new Vector2(entry.Position.X, entry.Position.Y + 75);
                    entry.IsPulsating = false;
                    entry.SelectedColor = entry.UnselectedColor;
               }

               backMenuEntry.IsPulsating = true;
          }

          #endregion

          #region Menu Events

          /// <summary>
          /// Event handler for when the View Controls menu entry is selected.
          /// </summary>
          void AwardsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.AddScreen(new AwardsScreen(), e.PlayerIndex);
          }

          /// <summary>
          /// Event handler for when the View Controls menu entry is selected.
          /// </summary>
          void UnlockablesMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               //ScreenManager.AddScreen(new UnlockablesScreen(), e.PlayerIndex);
          }

          /// <summary>
          /// Event handler for when the Tell A Friend menu entry is selected.
          /// </summary>
          void TellFriendMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               if (Gamer.SignedInGamers[e.PlayerIndex] == null)
               {
                    SimpleGuideMessageBox.ShowMessageBox(ControllingPlayer,
                         "Feature Unavailable", "This feature requires a Player Profile that is signed into XBOX Live.",
                         new string[] { "OK" }, 0, MessageBoxIcon.Warning);

                    return;
               }

               if (Gamer.SignedInGamers[e.PlayerIndex].IsGuest ||
                   !Gamer.SignedInGamers[e.PlayerIndex].IsSignedInToLive)
               {
                    SimpleGuideMessageBox.ShowMessageBox(ControllingPlayer,
                         "Feature Unavailable",
                         "This feature requires a Player Profile that is signed into XBOX Live. The Player Profile also cannot be a Guest Profile.",
                         new string[] { "OK" }, 0, MessageBoxIcon.Warning);

                    return;
               }

               Guide.ShowComposeMessage(e.PlayerIndex,
                    "Check out Avatar Typing: Horde Invasion on the Indie Marketplace! It's only $1, and all of the profits go to needy developers.", 
                    Gamer.SignedInGamers[e.PlayerIndex].GetFriends());
          }

          /// <summary>
          /// Event handler for when the View Controls menu entry is selected.
          /// </summary>
          void CreditsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.AddScreen(new AvatarTypingCreditsScreen(), e.PlayerIndex);
          }

          protected override void OnCancel(PlayerIndex playerIndex)
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

          public override void Draw(GameTime gameTime)
          {
               ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 3 / 5);

               base.Draw(gameTime);
          }

          #endregion

          #region Update

          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               // Handle Guest / Non-Live Profile Menu Entry Disabling.
               if (AvatarTypingGame.CurrentPlayer != null & AvatarTypingGame.CurrentPlayer.GamerInfo.Gamer != null)
               {
                    if (!AvatarTypingGame.CurrentPlayer.GamerInfo.Gamer.IsSignedInToLive ||
                         AvatarTypingGame.CurrentPlayer.GamerInfo.Gamer.IsGuest)
                    {
                         tellFriendMenuEntry.UnselectedColor = Color.Gray;
                         tellFriendMenuEntry.SelectedColor = Color.Gray;
                    }

                    else
                    {
                         tellFriendMenuEntry.UnselectedColor = Color.White;
                         tellFriendMenuEntry.SelectedColor = Color.White;// Color.DarkOrange;
                    }
               }
          }

          #endregion
     }
}