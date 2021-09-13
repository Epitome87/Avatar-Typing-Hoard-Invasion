#region File Description
//-----------------------------------------------------------------------------
// WaveCompleteScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using PixelEngine;
using PixelEngine.AchievementSystem;
using PixelEngine.Audio;
using PixelEngine.Avatars;
using PixelEngine.Graphics;
using PixelEngine.Menu;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
using PixelEngine.Storage;
using PixelEngine.Text;
#endregion

namespace AvatarTyping
{
     /// <remarks>
     /// A Screen containing the logic for when a Wave is Completed.
     /// Displays post-wave statistics such as number of kills, accuracy, etc.
     /// </remarks>
     public class WaveCompleteScreen : MenuScreen
     {
          #region Fields

          SpriteBatch spriteBatch;
          GameResourceFont ChatFont;
          Vector2 textPosition = new Vector2(EngineCore.ScreenCenter.X, 10);
          uint waveNumber;
          float score;
          //float scoreMultiplier;
          float enemiesKilled;
          float killTime;
          float perfects;
          float speedKills;
          float escapes;
          float wordsPerMinute;
          EnemyManager enemyManager;

          PlayerBackgroundScreen playerScreen = new PlayerBackgroundScreen();
          SaveGameData data = new SaveGameData();
          List<TextObject> textObjectList = new List<TextObject>();

          GameResourceTexture2D gradientTexture;

          #endregion

          ArcadeLevel theArcadeLevel;

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public WaveCompleteScreen(ArcadeLevel arcadeLevel, EnemyManager _enemyManager)
               : base("Wave " + (_enemyManager.WaveNumber - 1).ToString() + " Complete!")
          {
               TextManager.Reset();

               TransitionOnTime = TimeSpan.FromSeconds(2.5);
               TransitionOffTime = TimeSpan.FromSeconds(0.5);

               theArcadeLevel = arcadeLevel;
               enemyManager = _enemyManager;

               this.MenuTitleColor = Color.CornflowerBlue;

               // Create our menu entries.
               MenuEntry continueMenuEntry = new MenuEntry("Begin Next Wave");

               // Hook up menu event handlers.
               continueMenuEntry.Selected += ContinueMenuEntrySelected;

               continueMenuEntry.Position = new Vector2(continueMenuEntry.Position.X, 720 - 250);

               continueMenuEntry.SelectedColor = Color.Gold;// Color.White;

               continueMenuEntry.BorderColor = Color.Gold * (50f / 255f);

               // New
               continueMenuEntry.ShowGradientBorder = false;
               continueMenuEntry.ShowBorder = true;
               // End

               // Add entries to the menu.
               MenuEntries.Add(continueMenuEntry);

               waveNumber = enemyManager.WaveNumber;
               score = enemyManager.Score;
               //scoreMultiplier = ((int)(AvatarTypingGameSettings.Difficulty + 1) * (0.5f));
               enemiesKilled = enemyManager.EnemiesKilled;
               killTime = enemyManager.KillTime;
               perfects = enemyManager.PerfectKills;
               speedKills = enemyManager.SpeedKills;
               escapes = enemyManager.EnemiesEscaped;
               wordsPerMinute = (60 / enemyManager.KillTime) * enemyManager.CharactersTyped / 4.5f;

               // NEW
               if (float.IsNaN(wordsPerMinute))
               {
                    wordsPerMinute = 0f;
               }
               // END

               AvatarTypingGame.AwardData.CurrentWave = waveNumber;

               
               // Set the Save-Data info that's about to be saved.
               data.PlayerName = AvatarTypingGame.CurrentPlayer.GamerInfo.GamerTag;
               data.Level = waveNumber;
               data.HighScore = score;
               data.WordList = SentenceDatabase.UserSentences;

               data.IsUnlockedAchievement = AchievementManager.IsUnlockedList;
               data.AwardData = AvatarTypingGame.AwardData;

               data.Difficulty = (int)AvatarTypingGameSettings.Difficulty;
               data.FontSize = (int)AvatarTypingGameSettings.FontSize;

               ScreenManager.AddScreen(playerScreen, AvatarTypingGame.ControllingPlayer);
          }

          public override void LoadContent()
          {
               gradientTexture = ResourceManager.LoadTexture(@"Textures\Sky_Night");

               ChatFont = ResourceManager.LoadFont(@"Fonts\MenuFont_Border");

               spriteBatch = ScreenManager.SpriteBatch;

               InitializeText();

               playerScreen.DrawWithoutCamera = true;
               playerScreen.playerScale = 0.65f;
               playerScreen.borderScale = playerScreen.playerScale / 0.9f;

               playerScreen.playerPosition = new Vector3(0f, 2.0f, 0.0f);
               playerScreen.border = new Rectangle(
                    (int)((1280 / 2) - ((350 * playerScreen.borderScale) / 2f)),
                    (int)((720 / 2) - 40 - ((450f * playerScreen.borderScale) / 2f)),
                    (int)(350f * playerScreen.borderScale),
                    (int)(450f * playerScreen.borderScale));

               playerScreen.ShowBorder = true;
               playerScreen.ShowGamerTag = false;
               playerScreen.ShowGamerPic = false;

               playerScreen.BorderColor = Color.Gold * 0.5f;//Color.Blue;
               playerScreen.OutlineColor = Color.Black;// Color.White;

               playerScreen.player.Avatar.AvatarAnimation = 
                    new AvatarBaseAnimation(AvatarAnimationPreset.Celebrate);


               AudioManager.PlayCue("DanseMacabre_BigChange");

#if XBOX
               // Set the request flag
               if (StorageManager.SaveRequested == false)
               {
                    StorageManager.SaveRequested = true;
               }
#endif
          }

          #region Initialize Text Objects

          public void InitializeText()
          {
               Color color = Color.Gold;// Color.CornflowerBlue;

               float yTranslation = 100;

               //textPosition.X -= xTranslation;
               textPosition.X = 330;
               textPosition.Y += yTranslation;

               // Text Objects for the various Post-Wave stat headings.
               TextObject enemiesKilledText = new TextObject("Enemies Killed",
                                              new Vector2(textPosition.X, textPosition.Y + 120));
               enemiesKilledText.Color = color;
               enemiesKilledText.Origin = ScreenManager.Font.MeasureString(enemiesKilledText.Text) / 2;

               TextObject perfectKillsText = new TextObject("Perfect Kills",
                                              new Vector2(textPosition.X, textPosition.Y + 210));
               perfectKillsText.Color = color;
               perfectKillsText.Origin = ScreenManager.Font.MeasureString(perfectKillsText.Text) / 2;

               TextObject speedKillsText = new TextObject("Speed Kills",
                               new Vector2(textPosition.X, textPosition.Y + 310));
               speedKillsText.Color = color;
               speedKillsText.Origin = ScreenManager.Font.MeasureString(speedKillsText.Text) / 2;




               //textPosition.X += (2 * xTranslation);
               textPosition.X = 1280f - 330f;

               TextObject escapedText = new TextObject("Enemies Escaped",
                               new Vector2(textPosition.X, textPosition.Y + 120));
               escapedText.Color = color;
               escapedText.Origin = ScreenManager.Font.MeasureString(escapedText.Text) / 2;

               TextObject wordsPerMinuteText = new TextObject("Words Per Minute",
                               new Vector2(textPosition.X, textPosition.Y + 210));
               wordsPerMinuteText.Color = color;
               wordsPerMinuteText.Origin = ScreenManager.Font.MeasureString(wordsPerMinuteText.Text) / 2;

               TextObject killSpeedText = new TextObject("Avg Kill Speed",
                               new Vector2(textPosition.X, textPosition.Y + 310));
               killSpeedText.Color = color;
               killSpeedText.Origin = ScreenManager.Font.MeasureString(killSpeedText.Text) / 2;



               textPosition.X = EngineCore.ScreenCenter.X;

               TextObject waveScoreText = new TextObject("Wave Score", new Vector2(textPosition.X, textPosition.Y + 400));
               waveScoreText.Color = color;
               waveScoreText.Scale = 1.5f;
               waveScoreText.Origin = ScreenManager.Font.MeasureString(waveScoreText.Text) / 2;
               //waveScoreText.TextEffect = new TypingEffect(2000.0f, waveScoreText.Text);
               waveScoreText.AddTextEffect(new TypingEffect(2000.0f, waveScoreText.Text));


               TextObject scoreNumText = new TextObject(string.Format(score.ToString()),//(score * scoreMultiplier).ToString()),
                    new Vector2(textPosition.X, textPosition.Y + 450));
               scoreNumText.Color = Color.White;
               scoreNumText.Scale = 1.5f;
               scoreNumText.Origin = ScreenManager.Font.MeasureString(scoreNumText.Text) / 2;
               //scoreNumText.TextEffect = new TypingEffect(2000.0f, scoreNumText.Text);
               scoreNumText.AddTextEffect(new TypingEffect(2000.0f, scoreNumText.Text));



               // Text Objects for the various Post-Wave stats.
               color = Color.White;

               //textPosition.X -= (xTranslation);
               textPosition.X = 330;

               TextObject killNumText = new TextObject(string.Format(enemiesKilled.ToString()),
                              new Vector2(textPosition.X, textPosition.Y + 160));
               killNumText.Color = color;
               killNumText.Origin = ScreenManager.Font.MeasureString(killNumText.Text) / 2;

               TextObject perfectNumText = new TextObject(string.Format(perfects.ToString()),
                              new Vector2(textPosition.X, textPosition.Y + 260));
               perfectNumText.Color = color;
               perfectNumText.Origin = ScreenManager.Font.MeasureString(perfectNumText.Text) / 2;

               TextObject speedNumText = new TextObject(string.Format(speedKills.ToString()),
                              new Vector2(textPosition.X, textPosition.Y + 360));
               speedNumText.Color = color;
               speedNumText.Origin = ScreenManager.Font.MeasureString(speedNumText.Text) / 2;


               //textPosition.X += (2 * xTranslation);
               textPosition.X = 1280f - (330);// (1280f / 3f);



               TextObject escapeNumText = new TextObject(string.Format(escapes.ToString()),
                              new Vector2(textPosition.X, textPosition.Y + 160));
               escapeNumText.Color = color;
               escapeNumText.Origin = ScreenManager.Font.MeasureString(escapeNumText.Text) / 2;


               
               float averageKillSpeed = killTime / enemiesKilled;
               string averageKillSpeedString = averageKillSpeed.ToString();

               if (float.IsNaN(averageKillSpeed) || averageKillSpeed == 0)
               {
                    averageKillSpeed = 0f;
                    averageKillSpeedString = "N/A";
               }
               
               TextObject killSpeedNumText;
               int length = averageKillSpeedString.Length;

               if (length < 4)
               {
                    if (averageKillSpeedString == "N/A")
                    {
                         killSpeedNumText = new TextObject(string.Format("{0}", averageKillSpeedString),
                              new Vector2(textPosition.X, textPosition.Y + 360));
                    }
                    else
                    {
                         killSpeedNumText = new TextObject(string.Format("{0} {1}", averageKillSpeedString, "sec"),
                                        new Vector2(textPosition.X, textPosition.Y + 360));
                    }
               }

               else
               {
                    if (averageKillSpeedString == "N/A")
                    {
                         killSpeedNumText = new TextObject(string.Format("{0}", averageKillSpeedString.Substring(0, 4)),
                              new Vector2(textPosition.X, textPosition.Y + 360));
                    }
                    else
                    {
                         killSpeedNumText = new TextObject(string.Format("{0} {1}", averageKillSpeedString.Substring(0, 4), "sec"),
                                        new Vector2(textPosition.X, textPosition.Y + 360));
                    }
               }

               killSpeedNumText.Color = color;
               killSpeedNumText.Origin = ScreenManager.Font.MeasureString(killSpeedNumText.Text) / 2;

               
               
               string s;
               string wordsPerMinuteString = wordsPerMinute.ToString();

               if (float.IsInfinity(wordsPerMinute))
               {
                    wordsPerMinuteString = "N/A";
               }

               TextObject wordsPerMinuteNumText;
               length = wordsPerMinuteString.Length;
               if (length < 5)
                    s = string.Format("{0}", wordsPerMinuteString);

               else
                    s = string.Format("{0}", wordsPerMinuteString.Substring(0, 5));

               wordsPerMinuteNumText = new TextObject(s, new Vector2(textPosition.X, textPosition.Y + 260));

               wordsPerMinuteNumText.Color = color;
               wordsPerMinuteNumText.Origin = ScreenManager.Font.MeasureString(wordsPerMinuteNumText.Text) / 2;

               // Add the stat heading texts to Text Handler.
               textObjectList.Add(enemiesKilledText);
               textObjectList.Add(perfectKillsText);
               textObjectList.Add(speedKillsText);
               textObjectList.Add(escapedText);
               textObjectList.Add(waveScoreText);
               textObjectList.Add(killSpeedText);
               textObjectList.Add(wordsPerMinuteText);

               // Add the stat texts to the Text Handler.
               textObjectList.Add(killNumText);
               textObjectList.Add(perfectNumText);
               textObjectList.Add(speedNumText);
               textObjectList.Add(escapeNumText);
               textObjectList.Add(scoreNumText);
               textObjectList.Add(killSpeedNumText);
               textObjectList.Add(wordsPerMinuteNumText);

               foreach (TextObject textObj in textObjectList)
               {
                    textObj.Scale = 1.0f; // 1.5 with other font.
                    textObj.AddTextEffect(new ReadingEffect(2000.0f, textObj.Text));
                    textObj.FontType = TextManager.FontType.MenuFont;
               }

               textObjectList[5].AddTextEffect(new TypingEffect(2000.0f, textObjectList[5].Text));

               waveScoreText.Scale = 1.45f; // 2.0 with other font.
               scoreNumText.Scale = 1.45f;  // 2.0 with other font.

               waveScoreText.Color = Color.CornflowerBlue;
          }

          #endregion

          #endregion

          #region Handle Input

          /// <summary>
          /// Event handler for when the Continue Game menu entry is selected.
          /// </summary>
          void ContinueMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               textObjectList.Clear();

               // Remove the Player Screen.
               ScreenManager.RemoveScreen(playerScreen);

               // And then remove this Wave Complete Screen.
               ScreenManager.RemoveScreen(this);

               // THIS IS NEWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW
               if (enemyManager.WaveNumber <= ArcadeLevel.MAX_WAVE)
               {
                    // Finally, load the Stage Introduction Screen via an Animated Loading Screen.
                    AnimatedLoadingScreen.Load(true, ControllingPlayer,
                         String.Format("Arcade Mode\nWave {0}", enemyManager.WaveNumber), false,
                         new StageIntroScreen(theArcadeLevel, enemyManager));
               }

               else
               {
                    AnimatedLoadingScreen.RemoveAllButGameplayScreen();
                    ScreenManager.AddScreen(new ArcadeModeCompleteScreen(theArcadeLevel), ControllingPlayer);
               }
          }

          public override void HandleInput(InputState input)
          {
               base.HandleInput(input);
          }

          protected override void OnCancel(PlayerIndex playerIndex)
          {
               // Do nothing.
          }

          #endregion

          #region Update

          public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               foreach (TextObject textObj in textObjectList)
               {
                    textObj.Update(gameTime);
               }

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
               }
          }

          #endregion

          #region Draw

          public override void Draw(GameTime gameTime)
          {
               MySpriteBatch.Begin();
               MySpriteBatch.Draw(gradientTexture.Texture2D, new Rectangle(0, 0, 1280, 720), Color.White);
               MySpriteBatch.End();

               playerScreen.Draw(gameTime);

               //base.Draw(gameTime);

               MySpriteBatch.Begin();

               foreach (TextObject textObj in textObjectList)
               {
                    textObj.Draw(gameTime);
               }

               MySpriteBatch.End();

               base.Draw(gameTime);
          }

          #endregion
     }
}