#region File Description
//-----------------------------------------------------------------------------
// StageSelectScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PixelEngine.Menu;
using PixelEngine.Screen;
#endregion

namespace AvatarTyping
{
     /// <remarks>
     /// A MenuScreen which presents the player with the two Modes to play.
     /// Upon choosing one of the Modes, the player is taken to the corresponding
     /// Gameplay Screen.
     /// 
     /// On this MenuScreen, the player's Avatar & Gamer Information is also displayed.
     /// </remarks>
     public class StageSelectScreen : MenuScreen
     {
          #region Fields

          SpriteBatch spriteBatch;
          ContentManager content;
          SpriteFont ChatFont;

          public static bool IsStageSelected = false;
          public static int StageSelected = 0;

          #endregion

          #region Properties

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public StageSelectScreen()
               : base("Stage Select")
          {
               TransitionOnTime = TimeSpan.FromSeconds(0.5f);
               TransitionOffTime = TimeSpan.FromSeconds(0.5f);

               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.StartingGame;
               }

               // Create our menu entries.
               MenuEntry fortressStageMenuEntry = new MenuEntry("Fortress", "Type your way through\na peaceful Fortress.\nDon't mind the impending\ndoom coming your way.");
               MenuEntry graveyardStageMenuEntry = new MenuEntry("Graveyard", "Type your way through\nthe Graveyard.\nUnless you're too scared...");

               // Hook up menu event handlers.
               fortressStageMenuEntry.Selected += FortressMenuSelected;
               graveyardStageMenuEntry.Selected += GraveyardMenuSelected;
               
               graveyardStageMenuEntry.Position = new Vector2(graveyardStageMenuEntry.Position.X + 200, graveyardStageMenuEntry.Position.Y + 50);
               fortressStageMenuEntry.Position = new Vector2(fortressStageMenuEntry.Position.X + 200, fortressStageMenuEntry.Position.Y + 75);

               graveyardStageMenuEntry.DescriptionPosition = new Vector2(graveyardStageMenuEntry.DescriptionPosition.X + 200, graveyardStageMenuEntry.DescriptionPosition.Y - 100);
               fortressStageMenuEntry.DescriptionPosition = new Vector2(fortressStageMenuEntry.DescriptionPosition.X + 200, fortressStageMenuEntry.DescriptionPosition.Y - 100);

               if (Guide.IsTrialMode)
               {
                    fortressStageMenuEntry.SelectedColor = Color.Gray;
                    fortressStageMenuEntry.UnselectedColor = Color.Gray;
                    fortressStageMenuEntry.Description = "\nNot Available In Trial Mode.";
               }

               // Add entries to the menu.
               MenuEntries.Add(graveyardStageMenuEntry);
               MenuEntries.Add(fortressStageMenuEntry);

               foreach (MenuEntry entry in MenuEntries)
               {
                    entry.AdditionalVerticalSpacing = 50;// 20;
                    entry.FontScale = 1.5f;
                    entry.IsPulsating = false;
                    entry.SelectedColor = entry.UnselectedColor;
                    entry.ShowIcon = false;
               }
          }

          public override void LoadContent()
          {
               base.LoadContent();

               if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

               spriteBatch = ScreenManager.SpriteBatch;

               ChatFont = ScreenManager.Font;
          }

          #endregion

          #region Menu Event Handlers

          /// <summary>
          /// Event handler for when an Unlockable menu entry is selected.
          /// </summary>
          void FortressMenuSelected(object sender, PlayerIndexEventArgs e)
          {
               if (Guide.IsTrialMode)
                    return;

               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.ArcadeMode;
               }

               StageSelected = 1;
               IsStageSelected = true;

               ScreenManager.RemoveScreen(this);
          }

          /// <summary>
          /// Event handler for when an Unlockable menu entry is selected.
          /// </summary>
          void GraveyardMenuSelected(object sender, PlayerIndexEventArgs e)
          {
               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.SurvivalMode;
               }

               StageSelected = 2;
               IsStageSelected = true;

               ScreenManager.RemoveScreen(this);
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

          #region Handle Input

          public override void HandleInput(InputState input)
          {
               if (input == null)
                    throw new ArgumentNullException("input");

               base.HandleInput(input);
               // Look up inputs for the active player profile.
               //int playerIndex = (int)ControllingPlayer.Value;
               PlayerIndex playerIndex;
               int playerIndex1 = (int)ControllingPlayer.Value;
               KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex1];
               GamePadState gamePadState = input.CurrentGamePadStates[playerIndex1];

               // The game pauses either if the user presses the pause button, or if
               // they unplug the active gamepad. This requires us to keep track of
               // whether a gamepad was ever plugged in, because we don't want to pause
               // on PC if they are playing with a keyboard and have no gamepad at all!
               bool gamePadDisconnected = !gamePadState.IsConnected &&
                                          input.GamePadWasConnected[(int)ControllingPlayer.Value];

               if (input.IsMenuCancel(ControllingPlayer, out playerIndex))
               {
                    base.OnCancel(playerIndex);

                    // Remove this screen and the screens it contains.
                    ScreenManager.RemoveScreen(this);
               }
          }

          #endregion

          #region Update

          public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
          }

          #endregion

          #region Draw

          public override void Draw(GameTime gameTime)
          {
               base.Draw(gameTime);
          }

          #endregion
     }
}