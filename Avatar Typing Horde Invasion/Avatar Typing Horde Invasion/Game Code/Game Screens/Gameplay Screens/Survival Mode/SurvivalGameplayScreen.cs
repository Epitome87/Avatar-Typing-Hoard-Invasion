#region File Description
//-----------------------------------------------------------------------------
// OnslaughtGameplayScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine;
using PixelEngine.AchievementSystem;
using PixelEngine.CameraSystem;
using PixelEngine.Graphics;
using PixelEngine.Screen;
using PixelEngine.Text;
#endregion

namespace AvatarTyping
{
     /// <remarks>
     /// A Screen for "Survival Mode", which consists of progressively
     /// harder Waves of Enemies the player must ward off.
     /// </remarks>
     public class SurvivalGameplayScreen : MenuScreen
     {
          #region Fields

          SpriteBatch spriteBatch;
          ContentManager content;
          SpriteFont ChatFont;

          Vector2 textPosition;
          Random random;

          float elapsedTime;

          ArcadeLevel openingLevel;

          #endregion

          #region Properties

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public SurvivalGameplayScreen(int stageToLoad)
               : base("")
          {
               AchievementManager.IsUnlockNow = false;

               TextManager.Reset();

               TransitionOnTime = TimeSpan.FromSeconds(0.5f);
               TransitionOffTime = TimeSpan.FromSeconds(0.5f);

               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.CampaignMode;
               }

               elapsedTime = 0f;

               random = new Random();
               textPosition = new Vector2(EngineCore.ScreenCenter.X, -200);

               Stage myStage = null;

               if (stageToLoad == 1)
               {
                    myStage = new FortressStage(EngineCore.Game);
               }

               else
               {
                    myStage = new GraveyardStage(EngineCore.Game);
               }

               openingLevel = new ArcadeLevel(EngineCore.Game, myStage);
          }

          public override void LoadContent()
          {
               if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

               ChatFont = ScreenManager.Font;
               spriteBatch = ScreenManager.SpriteBatch;

               openingLevel.LoadContent();

               openingLevel.CurrentEnemyManager.NumberOfSimultaneousEnemies = 4;
               openingLevel.NumberOfNewEnemiesPerWave = 0; //openingLevel.CurrentEnemyManager.NumberOfNewEnemiesPerWave  = 0;

               openingLevel.CurrentEnemyManager.IsScriptedWave = false;

               CameraManager.SetActiveCamera(CameraManager.CameraNumber.ThirdPerson);
               CameraManager.ActiveCamera.Reset(EngineCore.GraphicsDevice.Viewport);
               CameraManager.ActiveCamera.Position = new Vector3(0f, 7f, openingLevel.CurrentPlayer.Position.Z - 10f);
               CameraManager.ActiveCamera.LookAt = new Vector3(0f, 1.2f, 10f);
               CameraManager.ActiveCamera.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(35f),
                                          CameraManager.ActiveCamera.AspectRatio, CameraManager.ActiveCamera.NearViewPlane, CameraManager.ActiveCamera.FarViewPlane);
          }

          #endregion

          #region Handle Input

          public override void HandleInput(InputState input)
          {
               if (input == null)
                    throw new ArgumentNullException("input");

               // Look up inputs for the active player profile.
               int index = (int)ControllingPlayer.Value;

               // Pause if "Pause" was pressed, or if controller was disconnected 
               // (assuming it was ever connected in the first place).
               if (input.IsPauseGame(ControllingPlayer) || input.GamePadWasDisconnected[index])
               {
                    ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
               }

               openingLevel.HandleInput(input);
          }

          /// <summary>
          /// When the user cancels the main menu, ask if they want to exit the game.
          /// </summary>
          protected override void OnCancel(PlayerIndex playerIndex)
          {
               const string message = "Return to Main Menu?";

               MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message, "Accept", "Cancel");

               confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

               ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
          }

          /// <summary>
          /// Event handler for when the user selects Accept on the "are you sure
          /// you want to exit" message box.
          /// </summary>
          void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.RemoveScreen(this);
               GameplayBackgroundScreen.isUpdate = true;
               ScreenManager.AddScreen(new MainMenuScreen(), ControllingPlayer);
          }

          #endregion

          #region Update

          public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
          {
               // Update the Screen(s)
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               if (IsActive)
               {
                    elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    AvatarTypingGame.AwardData.CurrentSurvivalTime = elapsedTime;

                    // Update the actual Level logic.
                    openingLevel.Update(gameTime);

                    if (!openingLevel.CurrentPlayer.IsAlive)
                    {
                         ScreenManager.AddScreen(new GameOverSurvivalModeScreen(
                              openingLevel.CurrentPlayer, openingLevel.CurrentEnemyManager, openingLevel.CurrentEnemyManager.Score), 
                              openingLevel.CurrentPlayer.GamerInfo.PlayerIndex);
                    }
               }
          }

          #endregion

          #region Main Draw Method

          public override void Draw(GameTime gameTime)
          {
               spriteBatch.GraphicsDevice.Clear(Color.Black);

               //MySpriteBatch.Begin(SpriteBlendMode.AlphaBlend);
               
               openingLevel.Draw(gameTime);

               int minutes = (int)(AvatarTypingGame.AwardData.CurrentSurvivalTime / 60);
               int seconds = (int)(AvatarTypingGame.AwardData.CurrentSurvivalTime - (minutes * 60));

               string s = String.Format("{0}:{1}", minutes, seconds);

               MySpriteBatch.Begin();

               TextManager.DrawCentered(false, ScreenManager.Font, "Survival Time", new Vector2(1280 - 250, 100), Color.Gold, 0.75f);
               TextManager.DrawCentered(false, ScreenManager.Font, s, new Vector2(1280 - 250, 130), Color.White, 0.75f);
               
               MySpriteBatch.End();

               base.Draw(gameTime);
          }

          #endregion

          #region Unloading Content

          public override void UnloadContent()
          {
               // Have the Level release all of its content.
               openingLevel.UnloadLevelContent();
          }

          #endregion
     }
}