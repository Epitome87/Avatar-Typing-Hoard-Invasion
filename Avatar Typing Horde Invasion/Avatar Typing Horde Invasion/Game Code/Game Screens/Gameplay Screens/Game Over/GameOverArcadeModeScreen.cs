
#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine;
using PixelEngine.Avatars;
using PixelEngine.Graphics;
using PixelEngine.Menu;
using PixelEngine.Screen;
using PixelEngine.Text;
using PixelEngine.ResourceManagement;
using PixelEngine.CameraSystem;
using PixelEngine.Audio;
#endregion

namespace AvatarTyping
{
     public class GameOverArcadeModeScreen : MenuScreen
     {
          #region Fields

          GameResourceTexture2D skyTexture;

          Player losingPlayer;

          TextObject gameOverText;

          float totalScore = 0;
          string scoreString = String.Empty;

          #endregion   

          #region Initialization

          public GameOverArcadeModeScreen(Player theLosingPlayer, EnemyManager enemyManager, int finalScore)
               : base("G  A  M  E    O  V  E  R")
          {
               this.MenuTitleFontScale = 2.0f;

               AudioManager.PushMusic("GretaSting");

               losingPlayer = theLosingPlayer;

               losingPlayer.UnequipWeapon();

               losingPlayer.Avatar.Rotation = new Vector3(0f, 0f, 0f);
               losingPlayer.Avatar.Scale = 2.5f;

               // Set up the cool blue lighting!
               losingPlayer.Avatar.LightDirection = new Vector3(1, 1, 0);
               losingPlayer.Avatar.LightColor = new Vector3(0, 0, 10);
               losingPlayer.Avatar.AmbientLightColor = Color.CornflowerBlue.ToVector3();

               losingPlayer.IsAlive = true;
               losingPlayer.Avatar.PlayAnimation(AvatarAnimationPreset.FemaleCry, true);


               totalScore = finalScore + enemyManager.Score;

               gameOverText = new TextObject("Total Score\n" + totalScore.ToString(), new Vector2(EngineCore.ScreenCenter.X, 250f));
               gameOverText.FontType = TextManager.FontType.MenuFont;
               gameOverText.IsCenter = true;
               gameOverText.Scale = 1.75f;
               gameOverText.Origin = gameOverText.Font.MeasureString(gameOverText.Text) / 2;
               gameOverText.Color = Color.Gold;// Color.LightGreen;
               gameOverText.AddTextEffect(new TypingEffect(5000f, gameOverText.Text));

               TransitionOnTime = TimeSpan.FromSeconds(5.0f);
               TransitionOffTime = TimeSpan.FromSeconds(1.0f);

               MenuEntry tellFriendMenuEntry = new MenuEntry("Tell a Friend!", "Show off your High Score!");
               MenuEntry quitMenuEntry = new MenuEntry("Main Menu", "You Lost. Return to Main Menu. Loser.");

               tellFriendMenuEntry.DescriptionPosition = new Vector2(tellFriendMenuEntry.DescriptionPosition.X, tellFriendMenuEntry.DescriptionPosition.Y + 40f);
               quitMenuEntry.DescriptionPosition = new Vector2(quitMenuEntry.DescriptionPosition.X, quitMenuEntry.DescriptionPosition.Y + 40f);

               tellFriendMenuEntry.Position = new Vector2(tellFriendMenuEntry.Position.X, tellFriendMenuEntry.DescriptionPosition.Y - 260);
               quitMenuEntry.Position = new Vector2(quitMenuEntry.Position.X, quitMenuEntry.DescriptionPosition.Y - 250);

               tellFriendMenuEntry.Selected += TellFriendMenuEntrySelected;
               quitMenuEntry.Selected += QuitMenuEntrySelected;

               tellFriendMenuEntry.BorderColor = Color.Gold * (50f / 255f);
               quitMenuEntry.BorderColor = Color.Gold * (50f / 255f);

               tellFriendMenuEntry.SelectedColor = Color.Gold;
               quitMenuEntry.SelectedColor = Color.Gold;

               MenuEntries.Add(tellFriendMenuEntry);
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
               Leaderboards.TopScoreEntry leaderboardEntry = new Leaderboards.TopScoreEntry(AvatarTypingGame.CurrentPlayer.GamerInfo.Gamer.Gamertag, (int)AvatarTypingGame.AwardData.CurrentSurvivalTime);
                    
               for (int i = 0; i < 50; i++)
               {
                    string randomName = "Paragon " + random.Next(100).ToString();
                    
                    leaderboardEntry = new Leaderboards.TopScoreEntry(randomName, (int)totalScore);
                    AvatarTypingGame.HighScoreSaveData.addEntry(0, leaderboardEntry, AvatarTypingGame.OnlineSyncManager);
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

               string waveString = (AvatarTypingGame.AwardData.CurrentWave).ToString();

               if (AvatarTypingGame.AwardData.CurrentWave <= 0)
                    waveString = "1";

               string scoreString = (totalScore).ToString();
               string difficultyString = AvatarTypingGameSettings.Difficulty.ToString();

               Guide.ShowComposeMessage(losingPlayer.GamerInfo.PlayerIndex,
                    "I progressed to Wave #" + waveString + " on " + difficultyString + 
                    " with a score of " + scoreString + " points on Avatar Typing: Horde Invasion's Arcade Mode!", null);
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

               ScreenManager.AddScreen(new MainMenuScreen(), e.PlayerIndex);
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