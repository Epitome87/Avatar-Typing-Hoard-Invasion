#region File Description
//-----------------------------------------------------------------------------
// AwardUnlockedScreen.cs
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
using PixelEngine.AchievementSystem;
using PixelEngine.Avatars;
using PixelEngine.Graphics;
using PixelEngine.Menu;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
using PixelEngine.Text;
#endregion

namespace PixelEngine
{
     /// <summary>
     /// The Award Unlocked screen is a Pop Up screen.
     /// This screen is displayed when the player unlocks an "Award".
     /// It simply shows the Award information, shows the player's Avatar
     /// being surprised / excited, and waits for the player to press A.
     /// </summary>
     public class AwardUnlockedScreen : MenuScreen
     {
          #region Fields

          private SpriteBatch spriteBatch;
          private ContentManager content;
          private SpriteFont ChatFont;

          GameResourceTexture2D gradientTexture;

          private Vector2 textPosition = new Vector2(EngineCore.ScreenCenter.X, 400);

          // The Award earned.
          private PixelEngine.AchievementSystem.Achievement award;
          private string awardString;

          // The player's avatar.
          private Avatar playerAvatar;

          // For rendering the player's avatar to screen coordinates.
          private Vector3 fakeCameraPosition = new Vector3(0f, 0f, -4.0f);
          private Vector3 renderPosition = new Vector3(0f, -1f, 0f);

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public AwardUnlockedScreen(PixelEngine.AchievementSystem.Achievement awardObtained)
               : base("Award Unlocked!")
          {
               TransitionOnTime = TimeSpan.FromSeconds(1.0);
               TransitionOffTime = TimeSpan.FromSeconds(0.5);

               this.MenuTitleColor = Color.Gold;

               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.FoundSecret;
               }

               award = awardObtained;

               awardString = string.Format("{0}\n{1}\n", award.Title, award.Description);

               // Create our menu entries.
               MenuEntry continueMenuEntry = new MenuEntry("Exit Award Unlocked Screen");

               // Hook up menu event handlers.
               continueMenuEntry.Selected += ContinueMenuEntrySelected;

               continueMenuEntry.Position = new Vector2(continueMenuEntry.Position.X, EngineCore.GraphicsInformation.ScreenHeight - 250);

               continueMenuEntry.SelectedColor = Color.Gold;

               continueMenuEntry.BorderColor = Color.Gold * (150f / 255f);

               // Add entries to the menu.
               MenuEntries.Add(continueMenuEntry);
          }

          public override void LoadContent()
          {
               base.LoadContent();

               if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

               ChatFont = TextManager.EnemyFont;

               spriteBatch = ScreenManager.SpriteBatch;

               playerAvatar = new Avatars.Avatar(ScreenManager.Game);
               playerAvatar.LoadUserAvatar(EngineCore.ControllingPlayer.Value);
               playerAvatar.PlayAnimation(AvatarAnimationPreset.MaleSurprised, true);
               playerAvatar.LightingEnabled = false;

               gradientTexture = ResourceManager.LoadTexture(@"Textures\Gradients\Gradient_BlackToWhite7");
          }

          #endregion

          #region Handle Input

          /// <summary>
          /// Event handler for when the Continue Game menu entry is selected.
          /// </summary>
          void ContinueMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               base.OnCancel(e.PlayerIndex);
          }

          public override void HandleInput(InputState input)
          {
               if (input == null)
                    throw new ArgumentNullException("input");

               // Look up inputs for the active player profile.
               //int playerIndex = (int)ControllingPlayer.Value;
               PlayerIndex playerIndex;
               int playerIndex1 = (int)ControllingPlayer;
               KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex1];
               GamePadState gamePadState = input.CurrentGamePadStates[playerIndex1];

               // The game pauses either if the user presses the pause button, or if
               // they unplug the active gamepad. This requires us to keep track of
               // whether a gamepad was ever plugged in, because we don't want to pause
               // on PC if they are playing with a keyboard and have no gamepad at all!
               bool gamePadDisconnected = !gamePadState.IsConnected &&
                                          input.GamePadWasConnected[playerIndex1];

               /*
               if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
               {
                    ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
               }*/

               if (input.IsMenuCancel(ControllingPlayer, out playerIndex))
               {
                    base.OnCancel(playerIndex);
               }

               if (input.IsNewButtonPress(Buttons.A, ControllingPlayer, out playerIndex))
               {
                    this.ExitScreen();
               }
          }

          #endregion

          #region Update

          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               if (IsActive)
               {
                    playerAvatar.Update(gameTime);
               }
          }

          #endregion

          #region Draw

          public override void Draw(GameTime gameTime)
          {
               ScreenManager.GraphicsDevice.Clear(Color.Black);

               MySpriteBatch.Begin();
               MySpriteBatch.Draw(gradientTexture.Texture2D, new Rectangle(0, 0, 1280, 720), Color.CornflowerBlue);
               MySpriteBatch.End();

               playerAvatar.DrawToScreen(gameTime, fakeCameraPosition, renderPosition);

               base.Draw(gameTime);

               spriteBatch.Begin();

               TextManager.DrawCentered(false, ScreenManager.Font, awardString, 
                    textPosition, Color.LightGreen, 1.25f);

               spriteBatch.End();
          }

          #endregion
     }
}
