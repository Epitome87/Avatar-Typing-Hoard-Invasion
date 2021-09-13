#region File Description
//-----------------------------------------------------------------------------
// GameOverSurvivalModeScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine;
using PixelEngine.CameraSystem;
using PixelEngine.Graphics;
using PixelEngine.Menu;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
using PixelEngine.Text;
using PixelEngine.Audio;
#endregion

namespace AvatarTyping
{
     /// <remarks>
     /// A MenuScreen which presents a "Game Over" message to the player.
     /// This screen is called upon dying in Survival Mode.
     /// 
     /// On this screen the Player is shown his final Survival Time, along with
     /// two menu entries: One for telling a friend his score, and the other
     /// for returning back to the Main Menu.
     /// </remarks>
     public class GameOverSurvivalModeScreen : MenuScreen
     {
          #region Fields

          GameResourceTexture2D skyTexture;

          Player losingPlayer;

          TextObject gameOverText;

          string survivalTime = String.Empty;

          #endregion

          #region Initialization

          public GameOverSurvivalModeScreen(Player theLosingPlayer, EnemyManager enemyManager, int finalScore)
               : base("G  A  M  E    O  V  E  R")
          {
               this.MenuTitleFontScale = 2.0f;

               AudioManager.PushMusic("GretaSting");

               losingPlayer = theLosingPlayer;

               losingPlayer.UnequipWeapon();

               losingPlayer.Avatar.Rotation = new Vector3(0, 0, 0);
               losingPlayer.Avatar.Scale = 2.5f;

               // Set up the cool blue lighting!
               losingPlayer.Avatar.LightDirection = new Vector3(1, 1, 0);
               losingPlayer.Avatar.LightColor = new Vector3(0, 0, 10);
               losingPlayer.Avatar.AmbientLightColor = Color.CornflowerBlue.ToVector3();

               losingPlayer.IsAlive = true;
               losingPlayer.Avatar.PlayAnimation(AvatarAnimationPreset.FemaleCry, true);
              
               int minutes = (int)(AvatarTypingGame.AwardData.CurrentSurvivalTime / 60);
               int seconds = (int)(AvatarTypingGame.AwardData.CurrentSurvivalTime - (minutes * 60));

               survivalTime = String.Format("{0}:{1}", minutes, seconds);

               gameOverText = new TextObject("Survival Time\n" + survivalTime, new Vector2(EngineCore.ScreenCenter.X, 250f));
               gameOverText.FontType = TextManager.FontType.MenuFont;
               gameOverText.IsCenter = true;
               gameOverText.Scale = 1.75f;
               gameOverText.Origin = gameOverText.Font.MeasureString(gameOverText.Text) / 2;
               gameOverText.Color = Color.Gold;// Color.LightGreen;
               gameOverText.AddTextEffect(new TypingEffect(5000f, gameOverText.Text));

               TransitionOnTime = TimeSpan.FromSeconds(5.0f);
               TransitionOffTime = TimeSpan.FromSeconds(1.0f);

               MenuEntry retryMenuEntry = new MenuEntry("Tell a Friend!", "Show Off Your Score!");
               MenuEntry quitMenuEntry = new MenuEntry("Main Menu", "You Lost. Return to Main Menu. Loser.");

               retryMenuEntry.DescriptionPosition = new Vector2(retryMenuEntry.DescriptionPosition.X, retryMenuEntry.DescriptionPosition.Y + 40f);
               quitMenuEntry.DescriptionPosition = new Vector2(quitMenuEntry.DescriptionPosition.X, quitMenuEntry.DescriptionPosition.Y + 40f);

               retryMenuEntry.Position = new Vector2(retryMenuEntry.Position.X, retryMenuEntry.DescriptionPosition.Y - 260);
               quitMenuEntry.Position = new Vector2(quitMenuEntry.Position.X, quitMenuEntry.DescriptionPosition.Y - 250);

               retryMenuEntry.Selected += TellFriendMenuEntrySelected;
               quitMenuEntry.Selected += QuitMenuEntrySelected;

               retryMenuEntry.BorderColor = Color.Gold * (50f / 255f);
               quitMenuEntry.BorderColor = Color.Gold * (50f / 255f);

               retryMenuEntry.SelectedColor = Color.Gold;
               quitMenuEntry.SelectedColor = Color.Gold;

               MenuEntries.Add(retryMenuEntry);
               MenuEntries.Add(quitMenuEntry);

               foreach (MenuEntry entry in MenuEntries)
               {
                    entry.ShowGradientBorder = false;
                    entry.ShowBorder = true;
               }

               CameraManager.ActiveCamera.Reset(EngineCore.GraphicsDevice.Viewport);
               CameraManager.SetActiveCamera(CameraManager.CameraNumber.FirstPerson);
               CameraManager.ActiveCamera.Position = new Vector3(0f, 3.0f, -5f);








               Random random = new Random();
               //string randomName = "Epitome" + random.Next(100).ToString();

               Leaderboards.TopScoreEntry leaderboardEntry = new Leaderboards.TopScoreEntry(AvatarTypingGame.CurrentPlayer.GamerInfo.Gamer.Gamertag, (int)AvatarTypingGame.AwardData.CurrentSurvivalTime);
               AvatarTypingGame.HighScoreSaveData.addEntry(1, leaderboardEntry, AvatarTypingGame.OnlineSyncManager);

               leaderboardEntry = new Leaderboards.TopScoreEntry("x SYLENT x", (int)AvatarTypingGame.AwardData.CurrentSurvivalTime);
               AvatarTypingGame.HighScoreSaveData.addEntry(1, leaderboardEntry, AvatarTypingGame.OnlineSyncManager);
               
               for (int i = 0; i < 12; i++)
               {
                    string randomName = "Epitome" + random.Next(100).ToString();
                    leaderboardEntry = new Leaderboards.TopScoreEntry(randomName, (int)AvatarTypingGame.AwardData.CurrentSurvivalTime);
                    AvatarTypingGame.HighScoreSaveData.addEntry(1, leaderboardEntry, AvatarTypingGame.OnlineSyncManager);
               }
               
               AvatarTypingGame.SaveHighScores(PixelEngine.Storage.StorageManager.Device);
          }

          public override void LoadContent()
          {
               base.LoadContent();

               skyTexture = ResourceManager.LoadTexture(@"Textures\Sky_Night");
          }

          public override void UnloadContent()
          {
               base.UnloadContent();
          }

          #endregion

          #region Menu Events

          /// <summary>
          /// Event handler for when the Play Game menu entry is selected.
          /// </summary>
          void TellFriendMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               if (losingPlayer == null)
                    return;

               if (losingPlayer.GamerInfo.Gamer == null)
                    return;

               if (losingPlayer.GamerInfo.Gamer.IsGuest ||
                   !AvatarTypingGame.CurrentPlayer.GamerInfo.Gamer.IsSignedInToLive)
                    return;

               Guide.ShowComposeMessage(losingPlayer.GamerInfo.PlayerIndex, 
                    "I just survived " + survivalTime + " on Avatar Typing: Horde Invasion's Survival Mode!", null);
          }

          /// <summary>
          /// Event handler for when the Play Game menu entry is selected.
          /// </summary>
          void QuitMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               losingPlayer.IsAlive = true;

               ScreenManager.RemoveScreen(this);

               GameplayBackgroundScreen.isUpdate = true;

               ScreenManager.AddScreen(new MainMenuScreen(), e.PlayerIndex);
          }

          /// <summary>
          /// When the user cancels the main menu, ask if they want to exit the game.
          /// </summary>
          protected override void OnCancel(PlayerIndex playerIndex)
          {
               // Do not let the Player back out of this screen.
          }

          /// <summary>
          /// Event handler for when the user selects Accept on the "are you sure
          /// you want to exit" message box.
          /// </summary>
          void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
          {
               losingPlayer.IsAlive = true;

               ScreenManager.RemoveScreen(this);

               GameplayBackgroundScreen.isUpdate = true;
               ScreenManager.AddScreen(new MainMenuScreen(), ControllingPlayer);
          }

          #endregion

          #region Update

          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               if (losingPlayer != null)
                    losingPlayer.Update(gameTime);

               gameOverText.Update(gameTime);

               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
          }

          #endregion

          #region Draw

          public override void Draw(GameTime gameTime)
          {
               ScreenManager.GraphicsDevice.Clear(Color.Black);

               MySpriteBatch.Begin();
               MySpriteBatch.Draw(skyTexture.Texture2D, new Rectangle(0, 0, 1280, 720), Color.White);
               //MySpriteBatch.Draw(gradientTexture.Texture2D, new Rectangle(0, 0, 1280, 720), Color.White);
               MySpriteBatch.End();

               if (losingPlayer != null)
                    losingPlayer.Draw(gameTime);

               MySpriteBatch.Begin();

               gameOverText.Draw(gameTime);

               MySpriteBatch.End();

               base.Draw(gameTime);
          }

          #endregion
     }
}