#region File Description
//-----------------------------------------------------------------------------
// ArcadeGameplayScreen.cs
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
using PixelEngine;
using PixelEngine.AchievementSystem;
using PixelEngine.Graphics;
using PixelEngine.Screen;
using PixelEngine.Text;
#endregion

namespace AvatarTyping
{
     /// <remarks>
     /// A Screen for "Arcade Mode", which consists of progressively
     /// harder Waves of Enemies the player must ward off.
     /// </remarks>
     public class ArcadeGameplayScreen : MenuScreen
     {
          #region Fields

          private SpriteBatch spriteBatch;
          private ContentManager content;
          private SpriteFont ChatFont;

          private  Vector2 textPosition;
          private Random random;

          private float elapsedTime;
          private bool updatedOnce;

          private ArcadeLevel openingLevel;

          private Counter roundCountdown = new Counter();
          private static bool introduceEnemy = true;

          #endregion

          #region Properties

          /// <summary>
          /// Whether or not we should "introduce" an Enemy.
          /// If True, an EnemyIntroductionScreen is shown before the Wave begins.
          /// This screen will describe the new Enemy type that will be appearing.
          /// </summary>
          public static bool IntroduceEnemy
          {
               get { return introduceEnemy; }
               set { introduceEnemy = value; }
          }

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public ArcadeGameplayScreen(int stageToLoad)
               : base("")
          {
               AchievementManager.IsUnlockNow = false;

               TextManager.Reset();

               TransitionOnTime = TimeSpan.FromSeconds(0.5f);
               TransitionOffTime = TimeSpan.FromSeconds(0.5f);

               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.ArcadeMode;
               }

               elapsedTime = 0f;

               random = new Random();
               textPosition = new Vector2(EngineCore.ScreenCenter.X, -200);

               if (stageToLoad == 1)
               {
                    Stage myStage = new FortressStage(EngineCore.Game);
                    openingLevel = new ArcadeLevel(EngineCore.Game, myStage);
               }

               else
               {
                    Stage myStage = new GraveyardStage(EngineCore.Game);
                    openingLevel = new ArcadeLevel(EngineCore.Game, myStage);
               }
          }

          public override void LoadContent()
          {
               if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

               ChatFont = ScreenManager.Font;
               spriteBatch = ScreenManager.SpriteBatch;

               openingLevel.LoadContent();
          }

          #endregion

          #region Handle Input

          public override void HandleInput(InputState input)
          {
               if (input == null)
                    throw new ArgumentNullException("input");

               // Look up inputs for the active player profile.
               int index = (int)ControllingPlayer.Value;

               // Don't allow input if Countdown is Active.
               if (roundCountdown != null && roundCountdown.Active)
               {
                    return;
               }

               // Pause if "Pause" was pressed, or if controller was disconnected 
               // (assuming it was ever connected in the first place).
               if (input.IsPauseGame(ControllingPlayer) || input.GamePadWasDisconnected[index])
               {
                    ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
               }

               // Let the Level handle its own input.
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
               //openingLevel.UnloadLevelContent();

               AvatarTypingGame.AwardData.CurrentWave = 0;

               ScreenManager.RemoveScreen(this);

               GameplayBackgroundScreen.isUpdate = true;

               ScreenManager.AddScreen(new MainMenuScreen(), e.PlayerIndex);
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
                    #region Enemy Introduction Screen Logic

                    if (introduceEnemy && 
                         (AvatarTypingGameSettings.Difficulty == Difficulty.Easy || AvatarTypingGameSettings.Difficulty == Difficulty.Normal))
                    {
                         switch (this.openingLevel.CurrentEnemyManager.WaveNumber)
                         {
                              case 1:
                                   ScreenManager.AddScreen(new EnemyIntroductionScreen(EnemyType.Normal, "Introducing\nNormal Enemy!"), ControllingPlayer);
                                   break;

                              case 2:
                                   ScreenManager.AddScreen(new EnemyIntroductionScreen(EnemyType.Fast, "Introducing\nSpeedster Enemy!"), ControllingPlayer);
                                   break;

                              case 3:
                                   ScreenManager.AddScreen(new EnemyIntroductionScreen(EnemyType.Kamikaze, "Introducing\nKamikaze Enemy!"), ControllingPlayer);
                                   break;

                              case 4:
                                   ScreenManager.AddScreen(new EnemyIntroductionScreen(EnemyType.Explosive, "Introducing\nExplosive Enemy!"), ControllingPlayer);
                                   break;

                              case 5:
                                   ScreenManager.AddScreen(new EnemyIntroductionScreen(EnemyType.Horde, "Introducing\nThe Horde!"), ControllingPlayer);
                                   break;

                              case 6:
                                   ScreenManager.AddScreen(new EnemyIntroductionScreen(EnemyType.Deflatable, "Introducing\nDeflatable Enemy!"), ControllingPlayer);
                                   break;

                              case 7:
                                   ScreenManager.AddScreen(new EnemyIntroductionScreen(EnemyType.Dancing, "Introducing\nDancing Enemy!"), ControllingPlayer);
                                   break;
                              
                              case 8:
                                   ScreenManager.AddScreen(new EnemyIntroductionScreen(EnemyType.Backward, "Introducing\nBackwards Enemy!"), ControllingPlayer);
                                   break;

                              case 10:
                                   ScreenManager.AddScreen(new EnemyIntroductionScreen(EnemyType.Boss, "Introducing\nThe Boss!"), ControllingPlayer);
                                   break;
                         }

                         introduceEnemy = false;
                    }

                    #endregion

                    elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    #region Countdown Logic

                    //if (roundCountdown.Active)
                    {
                         if (!updatedOnce)
                         {
                              openingLevel.Update(gameTime);

                              updatedOnce = true;

                              if (openingLevel.CurrentWave % 5 == 0 && openingLevel.CurrentWave % 2 != 0)
                              {
                                   roundCountdown = new Counter(5, 0, "Bonus Wave!\n", "Type!", "UI_Misc09", 1);
                              }

                              else
                              {
                                   roundCountdown = new Counter(5, 0, "Get Ready!\n", "Type!", "UI_Misc09", 1);
                              }
                              //return;
                         }
                         //return;
                    }

                    if (roundCountdown != null && roundCountdown.Active)
                    {
                         return;
                    }

                    #endregion

                    // Update the actual Level logic.
                    openingLevel.Update(gameTime);

                    #region Check for Game Over

                    // If the Current Player is no longer Alive...
                    if (!openingLevel.CurrentPlayer.IsAlive)
                    {
                         // Show the Game Over Screen.
                         ScreenManager.AddScreen(new GameOverArcadeModeScreen(openingLevel.CurrentPlayer, openingLevel.CurrentEnemyManager,
                                   openingLevel.totalScore), openingLevel.CurrentPlayer.GamerInfo.PlayerIndex);

                         // And Exit the Arcade Gameplay screen.
                         ExitScreen();
                    }

                    #endregion

                    #region Check if Wave has been Destroyed

                    // If the Current Wave has been destroyed...
                    if (openingLevel.CurrentEnemyManager.WaveDestroyed)
                    {
                         // Show the Wave Complete screen.
                         openingLevel.WaveComplete();

                         updatedOnce = false;
                    }

                    #endregion
               }
          }

          #endregion

          #region Main Draw Method

          public override void Draw(GameTime gameTime)
          {
               spriteBatch.GraphicsDevice.Clear(Color.Black);

               // If Countdown is going...
               if (roundCountdown != null && roundCountdown.Active)
               {
                    // Draw the level by calling EnemyManager.DrawWithoutWords.
                    // This will fix the sentence placement glitch and prevent cheating.
                    openingLevel.DrawAsPaused(gameTime);
               }

               else
               {
                    // Have the Level call its Draw logic.
                    openingLevel.Draw(gameTime);
               }

               MySpriteBatch.Begin(BlendState.AlphaBlend);

               // Have the Level render the Wave # HUD.
               openingLevel.DrawWave(gameTime);

               // Have the Level render the Enemies Remaining HUD.
               openingLevel.DrawEnemyRemaining(gameTime);

               MySpriteBatch.End();

               // If our Countdown isn't null, and it's in an Active state...
               if (roundCountdown != null && roundCountdown.Active)
               {
                    // Fade the Arcade Gameplay screen a little.
                    ScreenManager.FadeBackBufferToBlack(255 * 4 / 5);
                    
                    MySpriteBatch.Begin(BlendState.AlphaBlend);

                    // Render the Countdown text.
                    roundCountdown.DisplayCountdown(gameTime);
                    
                    MySpriteBatch.End();
               }

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