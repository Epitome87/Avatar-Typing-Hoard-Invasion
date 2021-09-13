#region File Description
//-----------------------------------------------------------------------------
// Level.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PixelEngine.Graphics;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
using PixelEngine.Text;
#endregion

namespace AvatarTyping
{     
     /// <remarks>
     /// A small environment with collections of items and enemies.
     /// The level owns the player and controls the game's win and lose
     /// conditions as well as scoring.
     /// 
     /// Since there are various Level types, this class is abstract.
     /// </remarks>
     public abstract class Level : DrawableGameComponent
     {
          #region Fields

          // A List of Players playing this Level.
          private List<Player> players;

          // A Level contains a Stage - the physical layout of the Level.
          protected Stage stage;

          // A Level contains an EnemyManager, as there are Enemy objects within the Level.
          protected EnemyManager enemyManager;

          // A Level contains a Player, or Players.
          protected Player thePlayer;

          // A Level keeps track of a Score for the Player.
          public int totalScore = 0;

          // General Resources used in all Levels.
          protected GameResourceTexture2D heartTexture;
          protected GameResourceTexture2D blankTexture;
          protected GameResourceTexture2D borderTexture;

          #endregion

          #region Properties

          /// <summary>
          /// Gets or sets a List of Players (human-controlled) who are part of the Level.
          /// </summary>
          public List<Player> ActivePlayers
          {
               get { return players; }
               set { players = value; }
          }

          /// <summary>
          /// Gets or sets the Stage this Level contains;
          /// that is, an object that defines the appearance of the Level,
          /// contains SceneObjects, and adds those to the SceneGraphManager.
          /// </summary>
          public Stage Stage
          {
               get { return stage; }
               set { stage = value; }
          }

          /// <summary>
          /// Gets or sets the Player who is participating in this ArcadeLevel instance. 
          /// </summary>
          public Player CurrentPlayer
          {
               get { return thePlayer; }
               set { thePlayer = value; }
          }

          /// <summary>
          /// Gets or sets the Enemy Manager managing this ArcadeLevel instance's enemies.
          /// </summary>
          public EnemyManager CurrentEnemyManager
          {
               get { return enemyManager; }
               set { enemyManager = value; }
          }

          /// <summary>
          /// Gets the Player's Score obtained in this ArcadeLevel instance.
          /// </summary>
          public int Score
          {
               get { return enemyManager.Score; }
          }

          /// <summary>
          /// Gets the number of Enemies Killed during this ArcadeLevel instance.
          /// </summary>
          public int EnemiesKilled
          {
               get { return enemyManager.EnemiesKilled; }
          }

          #endregion

          #region Initialization

          /// <summary>
          /// Level Constructor.
          /// </summary>
          public Level(Game game) 
               : base(game)
          {
          }

          new public virtual void LoadContent()
          {
               base.LoadContent();
          }

          /*
          protected override void UnloadContent()
          {
               base.UnloadContent();
          }
          */
          protected void LoadPlayers()
          {
          }

          protected void LoadEnemies()
          {
          }

          protected void LoadScripts()
          {
          }

          protected void LoadWave()
          {
          }

          protected void LoadEnvironment()
          {
          }

          #endregion

          #region Handle Input

          public virtual void HandleInput(InputState input)
          {
          }

          #endregion

          #region Update

          #endregion

          #region Draw

          public override void Draw(GameTime gameTime)
          {
               // Draw 3D stuff.
               // Draw 2D stuff.
          }

          #endregion

          #region Draw Overlay

          /// <summary>
          /// Renders the Overlay / HUD used by the Level.
          /// 
          /// Deriving Level objects should define their own DrawOverlay
          /// and put all HUD-related rendering inside of it.
          /// </summary>
          /// <param name="gameTime"></param>
          protected virtual void DrawOverlay(GameTime gameTime)
          {
          }

          #endregion

          #region Helper Draw Methods

          #region Draw Heart / Health, Combo & Score

          /// <summary>
          /// Renders the Heart texture representing Health.
          /// </summary>
          /// <param name="gameTime"></param>
          protected void DrawHeart(GameTime gameTime)
          {
               // Bottom Left.
               MySpriteBatch.DrawCentered(heartTexture.Texture2D, new Vector2(190, 720 - 100), Color.DarkRed, 0.05f);
          }

          /// <summary>
          /// Renders the player's Health text.
          /// </summary>
          /// <param name="gameTime"></param>
          protected void DrawHealth(GameTime gameTime)
          {
               string s = string.Format(" x {0}", CurrentPlayer.Health.ToString());

               TextManager.DrawCentered(false, ScreenManager.Font, s, new Vector2(250, 720 - 100), Color.White, 0.8f);
          }

          /// <summary>
          /// Draws the player's Combo Meter.
          /// </summary>
          /// <param name="gameTime"></param>
          protected void DrawCombo(GameTime gameTime)
          {
               int startingX = 175;
               int startingY = 115;
               int width = 150;
               int height = 25;

               MySpriteBatch.Draw(blankTexture.Texture2D,
                    new Rectangle(startingX, startingY, width, height), Color.Gray * (100f / 255f));

               MySpriteBatch.Draw(blankTexture.Texture2D, new Rectangle(startingX, startingY,
                         (int)((CurrentPlayer.ComboMeter / 100) * width), height),
                         Color.Gold);

               MySpriteBatch.Draw(borderTexture.Texture2D,
                    new Rectangle(startingX, startingY, width, height), Color.Black);
          }

          /// <summary>
          /// Renders the player's current Score.
          /// </summary>
          /// <param name="gameTime"></param>
          private void DrawScore(GameTime gameTime)
          {
               string s = string.Format("{0}", (totalScore + Score).ToString());

               // Top Center.
               TextManager.DrawCentered(false, ScreenManager.Font, "SCORE", new Vector2(1280 / 2, 100f), Color.Gold, 0.8f);
               TextManager.DrawCentered(false, ScreenManager.Font, s, new Vector2(1280 / 2, 130), Color.White, 0.8f);
          }

          #endregion

          #region Draw Perfect & Speedy Kill Streaks

          /// <summary>
          /// Renders the "Perfect x" text.
          /// </summary>
          /// <param name="gameTime"></param>
          private void DrawPerfectStreak(GameTime gameTime)
          {
               string s = string.Format("Perfect x {0}", AvatarTypingGame.AwardData.AccuracyStreak.ToString());

               TextManager.DrawCentered(false, ScreenManager.Font, s, new Vector2(250f, 100f), Color.CornflowerBlue, 0.8f);
          }

          /// <summary>
          /// Renders the "Speedy x" text.
          /// </summary>
          /// <param name="gameTime"></param>
          private void DrawSpeedStreak(GameTime gameTime)
          {
               string s = string.Format("Speedy x {0}", AvatarTypingGame.AwardData.SpeedStreak.ToString());

               TextManager.DrawCentered(false, ScreenManager.Font, s, new Vector2(250f, 155f), Color.LightGreen, 0.8f);
          }

          #endregion

          #endregion

          #region Unloading Level Content

          public virtual void UnloadLevelContent()
          {
               this.UnloadContent();
          }

          protected override void UnloadContent()
          {
               if (enemyManager != null)
               {
                    enemyManager.Dispose();
                    enemyManager = null;
               }

               //SceneGraphManager.RemoveObjects();

               base.UnloadContent();
          }

          #endregion
     }
}