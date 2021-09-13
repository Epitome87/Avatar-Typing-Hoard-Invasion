
#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine;
using PixelEngine.Graphics;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
using PixelEngine.Text;
#endregion

namespace AvatarTyping
{
     #region Helper Text Class

     /* Text represents one line of coherent text on the screen. Multiple 
      * * columns on a single row would be expressed as multiple Text instances. */
     public class Text
     {
          public Text(Vector2 p, string s)
          {
               Pos = p;
               Str = s;
          }

          public Color Color = Color.White;
          public Vector2 Pos;
          public string Str;
     }

     #endregion

     /// <summary>
     /// A MenuScreen that displays a list of High Scores.
     /// </summary>
     public class GlobalHighScores : MenuScreen
     {
          #region Fields

          /* A random number generator that I can use at will (for non-deterministic things). */
          static Random r = new Random();

          /* Texts displayed for the current highscores screen */
          List<Text> hiscoreText = new List<Text>();

          /* Filter what highscores are visible. -1 means all types. */
          int gametypeFilter = 0;

          /* Texts used by the main screen. */
          List<Text> texts = new List<Text>();

          /* What texts are actually being displayed (highscores, or main screen) */
          List<Text> displayText;

          /* This game has four game types, 0 through 3. 
           * This array has one entry for each type to display the name of each type. */
          static string[] gametypes = new string[] { "Arcade", "Survival" };

          int searchForWhoState = 0;

          ContentManager content;
          SpriteFont buttonSpriteFont;

          string[] whoFilters = new string[3] { "Friends", "My Score", "Overall" };

          private GameResourceTexture2D blankTexture;
          private GameResourceTexture2D enterKeyTexture;

          OnlineLeaderboard avatarTypingLeaderboard = new OnlineLeaderboard();

          #endregion

          #region Initialization

          public GlobalHighScores()
               : base("High Scores\nAll Modes")
          {
          }

          /// <summary>
          /// LoadContent will be called once per game and is the place to load
          /// all of your content.
          /// </summary>
          public override void LoadContent()
          {
               if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

               // Load the Button Font Sprite sheet.
               buttonSpriteFont = content.Load<SpriteFont>(@"Buttons\xboxControllerSpriteFont");

               // BuildHighscoreTexts();
               // displayText = hiscoreText;


               enterKeyTexture = ResourceManager.LoadTexture(@"Buttons\EnterKey");

               blankTexture = ResourceManager.LoadTexture(@"Textures\Blank Textures\blank");





               AvatarTypingGame.LoadHighScores(PixelEngine.Storage.StorageManager.Device);

               BuildHighscoreTexts();
               displayText = hiscoreText;



               // NEW FOR LEADERBOARD OBJECT
               avatarTypingLeaderboard.HighScoreSaveData = AvatarTypingGame.HighScoreSaveData;
               avatarTypingLeaderboard.Page = page;
          }

          #endregion

          #region Disposal

          public override void UnloadContent()
          {
               /* ContentManager disponses all my content. */
          }

          #endregion

          const int pageSize = 10;
          Leaderboards.TopScoreEntry[] page = new Leaderboards.TopScoreEntry[pageSize];

          Vector2 pageNumberPosition = new Vector2(EngineCore.ScreenCenter.X, EngineCore.GraphicsInformation.ScreenHeight - 95);

          #region Handle Input

          public override void HandleInput(InputState input)
          {
               // Handle the Leaderboard's input.
               avatarTypingLeaderboard.HandleInput(input, ControllingPlayer.Value);

               // Only re-build the score table if input was found.
               if (avatarTypingLeaderboard.wasInput)
               {
                    BuildHighscoreTexts();
               }

               base.HandleInput(input);
          }

          #endregion

          #region Update

          public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                         bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               // Change the Menu Title to show the current filter and game mode.
               this.MenuTitle = avatarTypingLeaderboard.CurrentPersonFilter + " - " + avatarTypingLeaderboard.CurrentModeFilter;
          }

          #endregion

          #region Draw

          public override void Draw(GameTime gameTime)
          {
               ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 3 / 5);

               // Draw the currently active text.
               DrawTexts();

               MySpriteBatch.Begin();

               // Draw the Enter Key.
               MySpriteBatch.Draw(enterKeyTexture.Texture2D, new Rectangle(0, (int)pageNumberPosition.Y - (int)(EngineCore.ResolutionScale * ScreenManager.Font.LineSpacing / 2),
                    EngineCore.GraphicsInformation.ScreenWidth, (int)(EngineCore.ResolutionScale * ScreenManager.Font.LineSpacing)),
                    Color.Black);

               // Draw the "Change Leaderboard" caption.
               Vector2 textPosition = new Vector2(525, 625);
               Vector2 origin = new Vector2(buttonSpriteFont.MeasureString("!").X / 2, buttonSpriteFont.LineSpacing / 2);
               int lineSize = (int)ScreenManager.Font.MeasureString("Change Leaderboard").X - 10;

               // Draw the "Change Game Modes" caption.
               TextManager.DrawCentered(false, ScreenManager.Font, "Change Game Modes", new Vector2(textPosition.X - 5, textPosition.Y), Color.White, 0.75f);
               MySpriteBatch.DrawString(buttonSpriteFont, "!", new Vector2(textPosition.X - (lineSize / 2), textPosition.Y),
                    Color.White, 0.0f, origin, 0.2f, SpriteEffects.None, 0f);

               // Draw the X Button sprite.
               textPosition = new Vector2(925, 625);
               origin = new Vector2(buttonSpriteFont.MeasureString("&").X / 2, buttonSpriteFont.LineSpacing / 2);
               lineSize = (int)ScreenManager.Font.MeasureString("My Score").X + 10;

               // Draw the current filter: All, Me, Friends, etc.
               TextManager.DrawCentered(false, ScreenManager.Font, whoFilters[searchForWhoState], textPosition, Color.White, 0.75f);

               // Draw the D-Pad sprite.
               MySpriteBatch.DrawString(buttonSpriteFont, "&", new Vector2(textPosition.X - (lineSize / 2), textPosition.Y),
                    Color.White, 0.0f, origin, 0.5f, SpriteEffects.None, 0f);

               MySpriteBatch.DrawCentered(enterKeyTexture.Texture2D, new Vector2(textPosition.X - (0.80f * lineSize), textPosition.Y), Color.LightGray, 0.2f);

               MySpriteBatch.End();

               /* The highscores and network components don't currently draw anything, but 
                * other components might. Also, at some point in the future, the components 
                * may start drawing overlays to show diagnostic information. */
               base.Draw(gameTime);
          }

          #endregion

          #region Build High Score Texts

          /* Given the current highscore state, build a number of text items to display the 
           * scores on the screen. The text items are drawn inside the draw callback by this
           * game. */
          void BuildHighscoreTexts()
          {
               if (AvatarTypingGame.OnlineSyncManager == null)
               {
                    return;
               }


               hiscoreText.Clear();


               float yPosition = 180;
               float verticalSpacing = 24;


               if (avatarTypingLeaderboard.ShowAll)
               {
                    avatarTypingLeaderboard.HighScoreSaveData.fillPageFromFullList(avatarTypingLeaderboard.gametypeFilter, avatarTypingLeaderboard.PageNumber, avatarTypingLeaderboard.Page);

                    int i = 0;

                    foreach (Leaderboards.TopScoreEntry highScore in avatarTypingLeaderboard.Page)
                    {
                         i++;
                         if (highScore != null)
                         {
                              // For each score found, create the different columns of display text.
                              hiscoreText.Add(new Text(new Vector2(200 + 90, yPosition + verticalSpacing * i), highScore.RankAtLastPageFill.ToString()));
                              hiscoreText.Add(new Text(new Vector2(300 + 100, yPosition + verticalSpacing * i), highScore.Gamertag));

                              string statString = "";

                              if (avatarTypingLeaderboard.gametypeFilter == 1)
                              {
                                   int minutes = (int)(highScore.Score / 60);
                                   int seconds = (int)(highScore.Score - (minutes * 60));

                                   statString = String.Format("{0}:{1}", minutes, seconds);
                              }

                              else
                              {
                                   statString = highScore.Score.ToString();
                              }

                              hiscoreText.Add(new Text(new Vector2(600 + 100, yPosition + verticalSpacing * i), statString));
                         }

                    }
               }

               if (avatarTypingLeaderboard.ShowFriends)
               {
                    if ((AvatarTypingGame.CurrentPlayer != null) &&
                       (AvatarTypingGame.CurrentPlayer.GamerInfo.Gamer != null) &&
                       (AvatarTypingGame.CurrentPlayer.GamerInfo.Gamer.IsSignedInToLive) &&
                       (!AvatarTypingGame.CurrentPlayer.GamerInfo.Gamer.IsGuest))
                    {
                         // Build the friend list.
                         SignedInGamer currentGamer = AvatarTypingGame.CurrentPlayer.GamerInfo.Gamer;

                         avatarTypingLeaderboard.HighScoreSaveData.fillPageFromFilteredList(avatarTypingLeaderboard.gametypeFilter, 0, avatarTypingLeaderboard.Page, currentGamer);

                         int c = 0;
                         foreach (Leaderboards.TopScoreEntry highScore in avatarTypingLeaderboard.Page)
                         {
                              c++;
                              if (highScore != null)
                              {
                                   // For each score found, create the different columns of display text.
                                   hiscoreText.Add(new Text(new Vector2(200 + 90, yPosition + verticalSpacing * c), highScore.RankAtLastPageFill.ToString()));
                                   hiscoreText.Add(new Text(new Vector2(300 + 100, yPosition + verticalSpacing * c), highScore.Gamertag));

                                   string statString = "";

                                   if (avatarTypingLeaderboard.gametypeFilter == 1)
                                   {
                                        int minutes = (int)(highScore.Score / 60);
                                        int seconds = (int)(highScore.Score - (minutes * 60));

                                        statString = String.Format("{0}:{1}", minutes, seconds);
                                   }

                                   else
                                   {
                                        statString = highScore.Score.ToString();
                                   }

                                   hiscoreText.Add(new Text(new Vector2(600 + 100, yPosition + verticalSpacing * c), statString));
                              }

                         }

                    }
               }

               if (avatarTypingLeaderboard.ShowMe)
               {
                    avatarTypingLeaderboard.PageNumber = avatarTypingLeaderboard.HighScoreSaveData.fillPageThatContainsGamertagFromFullList(avatarTypingLeaderboard.gametypeFilter, avatarTypingLeaderboard.Page, AvatarTypingGame.CurrentPlayer.GamerInfo.Gamer.Gamertag);//page, AvatarTypingGame.CurrentPlayer.GamerInfo.Gamer.Gamertag);
                    avatarTypingLeaderboard.HighScoreSaveData.fillPageFromFullList(avatarTypingLeaderboard.gametypeFilter, avatarTypingLeaderboard.PageNumber, avatarTypingLeaderboard.Page);//page);

                    int i = 0;
                    foreach (Leaderboards.TopScoreEntry highScore in avatarTypingLeaderboard.Page)
                    {
                         i++;
                         if (highScore != null)
                         {
                              // For each score found, create the different columns of display text.
                              hiscoreText.Add(new Text(new Vector2(200 + 90, yPosition + verticalSpacing * i), highScore.RankAtLastPageFill.ToString()));
                              hiscoreText.Add(new Text(new Vector2(300 + 100, yPosition + verticalSpacing * i), highScore.Gamertag));


                              string statString = "";

                              if (avatarTypingLeaderboard.gametypeFilter == 1)
                              {
                                   int minutes = (int)(highScore.Score / 60);
                                   int seconds = (int)(highScore.Score - (minutes * 60));

                                   statString = String.Format("{0}:{1}", minutes, seconds);
                              }

                              else
                              {
                                   statString = highScore.Score.ToString();
                              }

                              hiscoreText.Add(new Text(new Vector2(600 + 100, yPosition + verticalSpacing * i), statString));
                         }

                    }
               }

               hiscoreText.Add(new Text(new Vector2(200 + 90, 140), "Rank"));
               hiscoreText.Add(new Text(new Vector2(300 + 100, 140), "Gamertag"));
               hiscoreText.Add(new Text(new Vector2(600 + 100, 140), "Score"));

               if (gametypeFilter >= 0 && gametypeFilter < gametypes.Length)
               {
                    if (avatarTypingLeaderboard.gametypeFilter == 1)
                    {
                         hiscoreText.Add(new Text(new Vector2(800 + 100, 140), "Time"));
                    }

                    else if (gametypes[gametypeFilter] == "Arcade")
                    {
                         hiscoreText.Add(new Text(new Vector2(800 + 100, 140), "Wave"));
                    }
               }

               foreach (Text t in hiscoreText)
               {
                    t.Color = Color.Gold;
               }
          }

          #endregion

          #region Helper Draw Methods

          /* Draw the currently active texts to screen. */
          public void DrawTexts()
          {
               if (displayText == null)
                    displayText = texts;

               MySpriteBatch.Begin();
               foreach (Text t in displayText)
               {
                    TextManager.DrawString(ScreenManager.Font, t.Str, t.Pos, t.Color, 0.75f); // Scale = 0.6
               }
               MySpriteBatch.End();
          }

          #endregion
     }
}