#region File Description
//-----------------------------------------------------------------------------
// ArcadeLevel.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine;
using PixelEngine.Avatars;
using PixelEngine.CameraSystem;
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
     /// An Arcade Level consists of Waves of Enemies.
     /// </remarks>
     public class ArcadeLevel : Level
     {
          #region Fields

          // Gameplay variables.          
          public const int MAX_WAVE = 25;

          // The background texture to be used.
          private GameResourceTexture2D skyTexture;

          // Defines a Quad to render the background to.
          private Quad quad;
          private VertexDeclaration quadVertexDecl;
          private BasicEffect quadEffect;

          private uint numberOfWaves;
          private uint currentWave;

          private int numberOfNewEnemiesPerWave = 5;
          private const int bossWaveNumber = 10;
          private int numberOfStartingEnemies = 0;

          private Random random = new Random();

          #endregion

          #region Properties

          /// <summary>
          /// Gets or sets the amount of waves this HordeLevel consists of. 
          /// </summary>
          public uint NumberOfWaves
          {
               get { return numberOfWaves; }
               set { numberOfWaves = value; }
          }

          /// <summary>
          /// Gets or sets the Wave number currently in progress.
          /// </summary>
          public uint CurrentWave
          {
               get { return currentWave; }
               set { currentWave = value; }
          }

          /// <summary>
          /// Gets or sets how many Enemies are to start with.
          /// </summary>
          public int NumberOfStartingEnemies
          {
               get { return numberOfStartingEnemies; }
               set { numberOfStartingEnemies = value; }
          }

          /// <summary>
          /// Gets or sets the amount of new enemies that appear each wave.
          /// For example, a value of 5 means there are 5 more enemies on Wave 2
          /// than on Wave 1.
          /// </summary>
          public int NumberOfNewEnemiesPerWave
          {
               get { return numberOfNewEnemiesPerWave; }
               set { numberOfNewEnemiesPerWave = value; }
          }

          /// <summary>
          /// Gets or sets the Wave number at which a Boss will appear.
          /// A value of 10 means a Boss will appear every 10th wave.
          /// </summary>
          public static int BossWaveNumber
          {
               get { return bossWaveNumber; }
          }

          /// <summary>
          /// Returns the total number of Enemies that will appear this Wave.
          /// </summary>
          public float TotalEnemiesThisWave
          {
               get
               {
                    return this.NumberOfStartingEnemies +
                         this.enemyManager.WaveNumber * this.NumberOfNewEnemiesPerWave;
               }
          }
          
          #endregion

          #region Initialization

          public ArcadeLevel(Game game, Stage theStage)
               : base(game)
          {
               // Initialize a new EnemyManager.
               enemyManager = new EnemyManager(EngineCore.Game);
                    enemyManager.WaveNumber = 1;
               // Never really used. numberOfWaves = 25;
               currentWave = enemyManager.WaveNumber;

               AvatarTypingGame.AwardData.SpeedStreak = 0;
               AvatarTypingGame.AwardData.AccuracyStreak = 0;

               this.Stage = theStage;

               Vector3 quadOrigin = this.Stage.QuadOrigin;
               Vector3 quadNormal = this.Stage.QuadNormal;
               Vector3 quadUp = this.Stage.QuadUp;
               float quadWidth = this.Stage.QuadWidth;
               float quadHeight = this.Stage.QuadHeight;

               // For the Level's backdrop. TEMPORARY SOLUTION.
               quad = new Quad(quadOrigin, quadNormal, quadUp, quadWidth, quadHeight); 
          }

          public override void LoadContent()
          {
               // Load the blank texture and border, used on various HUD items.
               blankTexture = ResourceManager.LoadTexture(@"Textures\Blank Textures\Blank_Rounded_Wide");
               borderTexture = ResourceManager.LoadTexture(@"Textures\Blank Textures\Border_Wide_White");

               // Load the Heart texture.
               heartTexture = ResourceManager.LoadTexture(@"Textures\Heart");

               // Load the Stage (Either Fortress or Graveyard thus far.)
               Stage.LoadContent();

               // Try to put this in Stage.cs
               skyTexture = ResourceManager.LoadTexture(@"Textures\Sky_Night");

               // Load Enemies. enemyManager = new EnemyManager(EngineCore.Game);

               // Load Wave.
               this.BeginWave();

               // Create the Player.
               thePlayer = new Player(AvatarTypingGame.CurrentPlayer.GamerInfo.PlayerIndex);
               thePlayer.Avatar.Scale = 1.75f;
               thePlayer.EquipWeapon();

               enemyManager.currentPlayer = thePlayer;

               quadEffect = new BasicEffect(EngineCore.GraphicsDevice);

               quadEffect.EnableDefaultLighting();

               quadEffect.World = Matrix.CreateTranslation(new Vector3(0f, 80f, 0f))
                    * Matrix.CreateScale(0.03f); // 0.025
               quadEffect.View = CameraManager.ActiveCamera.ViewMatrix;
               quadEffect.Projection = CameraManager.ActiveCamera.ProjectionMatrix;
               quadEffect.TextureEnabled = true;
               quadEffect.Texture = skyTexture.Texture2D;
               quadEffect.EmissiveColor = Color.CornflowerBlue.ToVector3();

               quadVertexDecl = new VertexDeclaration(VertexPositionNormalTexture.VertexDeclaration.GetVertexElements());

               // Set up the Camera.
               CameraManager.SetActiveCamera(CameraManager.CameraNumber.ThirdPerson);
               CameraManager.ActiveCamera.Position = new Vector3(0, 3.0f, -4.5f);
               CameraManager.ActiveCamera.LookAt = new Vector3(0f, 0.5f, 20f);


               // Set up the Camera.
               CameraManager.SetActiveCamera(CameraManager.CameraNumber.ThirdPerson);
               CameraManager.ActiveCamera.Position = new Vector3(0, 3.5f, -4.75f);
               CameraManager.ActiveCamera.LookAt = new Vector3(0f, 0.5f, 20f);

               FirstPersonCamera.headOffset = new Vector3(0, thePlayer.Avatar.AvatarDescription.Height * thePlayer.Avatar.Scale * 0.90f, 0);
          }

          #endregion

          #region Handle Input

          public override void HandleInput(InputState input)
          {
               base.HandleInput(input);

               CameraManager.ActiveCamera.HandleInput(input, EngineCore.ControllingPlayer);
               enemyManager.HandleInput(input, EngineCore.ControllingPlayer);
          }

          #endregion

          #region Update

          /// <summary>
          /// Overridden Update method.
          /// Updates the Enemy Manager and the Player.
          /// </summary>
          /// <param name="gameTime"></param>
          public override void Update(GameTime gameTime)
          {
               #region Check if it's time to Spawn a new Enemy

               // If we have less enemies on-screen than allowed, spawn a new one!
               if (this.CurrentEnemyManager.aliveEnemies.Count < this.CurrentEnemyManager.NumberOfSimultaneousEnemies)
               {
                    // If this wave uses scripted enemy types, generate from the scripted list.
                    if (this.CurrentEnemyManager.IsScriptedWave)
                         this.GenerateEnemy(this.CurrentEnemyManager.scriptedEnemyTypes[random.Next(this.CurrentEnemyManager.scriptedEnemyTypes.Count)]);

                    // Otherwise, just generate any random enemy type.
                    else
                    {
                         int randomInt = random.Next(23);
                         GenerateEnemy((EnemyType)randomInt);
                    }
               }

               #endregion

               #region Check if Wave is Over

               // If no enemies remain, and we are alive...
               if (this.CurrentEnemyManager.aliveEnemies.Count <= 0 && this.CurrentEnemyManager.currentPlayer.IsAlive)
               {
                    // If we're on the boss wave, we calculate if the wave is over accordingly.
                    if (this.CurrentEnemyManager.WaveNumber % bossWaveNumber == 0)
                    {
                         this.CurrentEnemyManager.WaveNumber++;
                         this.CurrentEnemyManager.RemoveAllEnemies();
                         this.CurrentEnemyManager.WaveDestroyed = true;
                    }

                    // Otherwise, we are on a regular wave; calculate if wave is over accordingly.
                    else if (this.CurrentEnemyManager.numberOfEnemiesSpawned >= (numberOfStartingEnemies + (this.CurrentEnemyManager.WaveNumber * numberOfNewEnemiesPerWave)))
                    {
                         this.CurrentEnemyManager.WaveNumber++;
                         this.CurrentEnemyManager.RemoveAllEnemies();
                         this.CurrentEnemyManager.WaveDestroyed = true;
                    }
               }

               #endregion

               // Update the Enemy Manager.
               enemyManager.Update(gameTime);

               #region Update the Player & his GamerPresence

               // New 3-9-11 for game over screen fix.
               thePlayer.EquipWeapon();

               // Update the Player.
               thePlayer.Update(gameTime);

               // Do we really need this each frame?
               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.Score;

                    signedInGamer.Presence.PresenceValue =
                         (int)(totalScore + Score);
               }

               // New
               thePlayer.IsTyping = false;
               
               if (this.CurrentEnemyManager.isTargetSelected)
               {
                    thePlayer.IsTyping = true;
               }
               

               #endregion
          }

          #endregion

          #region Draw

          /// <summary>
          /// Overridden Draw method.
          /// Draws the Arcade Level: Skybox, SceneGraph items, 
          /// the player, the enemies, and finally the HUD overlay.
          /// </summary>
          /// <param name="gameTime"></param>
          public override void Draw(GameTime gameTime)
          {
               foreach (EffectPass pass in quadEffect.CurrentTechnique.Passes)
               {
                    pass.Apply();

                    EngineCore.GraphicsDevice.DrawUserIndexedPrimitives
                        <VertexPositionNormalTexture>(PrimitiveType.TriangleList,
                        quad.Vertices, 0, 4,
                        quad.Indexes, 0, 2);
               }

               MySpriteBatch.Begin(BlendState.AlphaBlend);

               // Render the Scene.
               SceneGraphManager.Draw(gameTime);

               // Draw the player and enemies.
               thePlayer.Draw(gameTime); 
               
               MySpriteBatch.End();

               enemyManager.Draw(gameTime);

               MySpriteBatch.Begin();

               // Draw the HUD overlay.
               DrawOverlay(gameTime);

               MySpriteBatch.End();
          }

          /// <summary>
          /// Overridden Draw method.
          /// Draws the Arcade Level: Skybox, SceneGraph items, 
          /// the player, the enemies, and finally the HUD overlay.
          /// </summary>
          /// <param name="gameTime"></param>
          public void DrawAsPaused(GameTime gameTime)
          {
               foreach (EffectPass pass in quadEffect.CurrentTechnique.Passes)
               {
                    pass.Apply();

                    EngineCore.GraphicsDevice.DrawUserIndexedPrimitives
                        <VertexPositionNormalTexture>(PrimitiveType.TriangleList,
                        quad.Vertices, 0, 4,
                        quad.Indexes, 0, 2);
               }

               MySpriteBatch.Begin(BlendState.AlphaBlend);

               // Render the Scene.
               SceneGraphManager.Draw(gameTime);

               // Draw the player and enemies.
               thePlayer.Draw(gameTime);

               MySpriteBatch.End();

               enemyManager.DrawWithoutWords(gameTime);

               MySpriteBatch.Begin();

               // Draw the HUD overlay.
               DrawOverlay(gameTime);

               MySpriteBatch.End();
          }

          #endregion
          
          #region Draw Overlay Method

          protected override void DrawOverlay(GameTime gameTime)
          {
               DrawHeart(gameTime);
               DrawHealth(gameTime);
               DrawScore(gameTime);
               DrawCombo(gameTime);
               DrawPerfectStreak(gameTime);
               DrawSpeedStreak(gameTime);
          }

          #endregion

          #region Helper Draw Methods

          #region Draw Wave Number & Enemies Remaining

          /// <summary>
          /// Renders the Wave # text.
          /// </summary>
          /// <param name="gameTime"></param>
          public void DrawWave(GameTime gameTime)
          {
               string s = string.Format("{0}", CurrentWave.ToString());

               TextManager.DrawCentered(false, ScreenManager.Font, "WAVE", new Vector2(1280 - 190, 100), Color.Gold, 0.8f);
               TextManager.DrawCentered(false, ScreenManager.Font, s, new Vector2(1280 - 190, 130), Color.White, 0.8f);
          }

          /// <summary>
          /// Renders the # of Enemies Remaining.
          /// </summary>
          /// <param name="gameTime"></param>
          public void DrawEnemyRemaining(GameTime gameTime)
          {
               string s = string.Format("{0}",
                    (
                         (NumberOfStartingEnemies + (CurrentWave * NumberOfNewEnemiesPerWave)) -
                         EnemiesKilled - CurrentEnemyManager.EnemiesEscaped
                    ).ToString());

               if (CurrentWave % BossWaveNumber == 0)
               {
                    s = "1";
               }

               TextManager.DrawCentered(true, ScreenManager.Font, "ENEMIES\nREMAINING", new Vector2(1280 - 190, 590), Color.Gold, 0.6f);
               TextManager.DrawCentered(false, ScreenManager.Font, s, new Vector2(1280 - 190, 620), Color.White, 0.8f);
          }

          #endregion

          #region Draw Score

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

          public override void UnloadLevelContent()
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

          #region Level Logic Methods

          /// <summary>
          /// Called to signal Wave completion.
          /// The Wave Complete screen is presented before progressing to the next Wave.
          /// </summary>
          public void WaveComplete()
          {
               ScreenManager.AddScreen(new WaveCompleteScreen(this, enemyManager), PixelEngine.EngineCore.ControllingPlayer);
               totalScore += enemyManager.Score;
               currentWave = enemyManager.WaveNumber;

               thePlayer.GunRotation = 0f;

               // Unequip the weapon.
               thePlayer.UnequipWeapon();
          }

          #endregion

          #region Wave Initialization Methods

          /// <summary>
          /// Begins a new Wave for Arcade Mode.
          /// 
          /// Simply spawns 3 enemies and re-initializes statistics,
          /// such as Wave Score, Enemies Killed, Kill Time, Enemies Escaped.
          /// </summary>
          public void BeginWave()
          {
               // Get the list of enemy types to spawn.
               this.InitializeScriptedWave();

               this.enemyManager.numberOfEnemiesSpawned = 0;

               random = new Random();

               // Spawn only a few enemies at a time.
               for (int i = 0; i < this.enemyManager.NumberOfSimultaneousEnemies; i++)
               {
                    GenerateEnemy(this.enemyManager.scriptedEnemyTypes[random.Next(this.enemyManager.scriptedEnemyTypes.Count)]);
               }

               CompareByPrioty comparer = new CompareByPrioty();
               this.enemyManager.aliveEnemies.Sort(comparer);

               this.enemyManager.WaveDestroyed = false;

               this.enemyManager.Score = 0;
               this.enemyManager.EnemiesKilled = 0;
               this.enemyManager.KillTime = 0;
               this.enemyManager.PerfectKills = 0;
               this.enemyManager.SpeedKills = 0;
               this.enemyManager.EnemiesEscaped = 0;
               this.enemyManager.CharactersTyped = 0;
          }

          /// <summary>
          /// We call this before a Wave of Enemies is to be spawned.
          /// 
          /// Calling this first allows us to choose what type of Enemies
          /// will be spawned in the wave.
          /// 
          /// For example, we only want Normal Enemies for Wave #1, Normal and
          /// Speedster Enemies for Wave #2, etc.
          /// </summary>
          public void InitializeScriptedWave()
          {
               this.enemyManager.scriptedEnemyTypes.Clear();

               switch (this.enemyManager.WaveNumber)
               {
                    // Introduce the Normal enemy.
                    case 1:
                         // 100% chance for Normal Enemy!
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Normal);
                         break;

                    // Introduce the Fast enemy.
                    case 2:
                         // 66% chance for Normal Enemy.
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Normal);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Normal);

                         // 33% chance for Fast Enemy.
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Fast);
                         break;

                    // Introduce the Suicide enemy.
                    case 3:
                         // 50% chance for Normal Enemy.
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Normal);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Normal);

                         // 25% chance for Fast Enemy.
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Fast);

                         // 25% chance for Kamikaze Enemy.
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Kamikaze);
                         break;

                    // Introduce the Explosive enemy.
                    case 4:
                         // 40% chance for Normal Enemy.
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Normal);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Normal);

                         // 20% chance for Fast Enemy.
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Fast);

                         // 20% chance for Kamikaze Enemy.
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Kamikaze);

                         // 20% chance for Explosive Enemy.
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Explosive);
                         break;

                    // Introduce The Horde!!!
                    case 5:
                         // 100% chance to get a Horde Enemy!
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Horde);
                         break;

                    // Introduce the Deflatable enemy.
                    case 6:
                         // 33% chance for Normal Enemy.
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Normal);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Normal);

                         // 16.6% chance for Fast Enemy.
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Fast);

                         // 16.6% chance for Kamikaze Enemy.
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Kamikaze);

                         // 16.6% chance for Explosive Enemy.
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Explosive);

                         // 16.6% chance for Deflatable Enemy.
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Deflatable);
                         break;

                    // Introduce the Dancing enemy.
                    case 7:
                         // 37.5% chance for Normal Enemy.
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Normal);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Normal);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Normal);

                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Fast);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Kamikaze);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Explosive);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Deflatable);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Dancing);
                         break;

                    // Introduce the Backwards enemy.
                    case 8:
                         // 33.3% chance for Normal Enemy.
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Normal);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Normal);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Normal);

                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Fast);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Kamikaze);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Explosive);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Deflatable);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Dancing);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Backward);
                         break;

                    // Nothing new to introduce.
                    case 9:
                         // 33.3% chance for Normal Enemy.
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Normal);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Normal);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Normal);

                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Fast);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Kamikaze);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Explosive);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Deflatable);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Dancing);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Backward);
                         break;

                    // Spawn a Boss Enemy.
                    case 10:
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Boss);
                         break;

                    // Spawn a Horde Wave.
                    case 15:
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Horde);
                         break;

                    // Spawn a Boss Enemy.
                    case 20:
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Boss);
                         break;

                    case 25:
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Horde);
                         break;

                    default:
                         // 33.3% chance for Normal Enemy.
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Normal);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Normal);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Normal);

                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Fast);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Kamikaze);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Explosive);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Deflatable);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Dancing);
                         this.enemyManager.scriptedEnemyTypes.Add(EnemyType.Backward);
                         break;
               }
          }

          #endregion
          
          #region Generate Enemy Method

          /// <summary>
          /// Generates an Enemy of a certain type.
          /// </summary>
          private void GenerateEnemy(EnemyType typeOfEnemy)
          {
               #region Check if we even need to Spawn an Enemy yet

               // Don't generate an enemy if we spawned the max amount of enemies this Wave.
               if (this.enemyManager.numberOfEnemiesSpawned >= this.TotalEnemiesThisWave)
               {
                    if (this.NumberOfNewEnemiesPerWave > 0)
                         return;
               }

               // Don't generate an enemy if we're on a Boss Wave and already have 1 Enemy.
               if (this.enemyManager.WaveNumber % BossWaveNumber == 0)
               {
                    if (this.NumberOfNewEnemiesPerWave > 0 && this.enemyManager.numberOfEnemiesSpawned >= 1)
                         return;
               }

               #endregion

               Vector3 position = new Vector3(0);

               TypingEnemy newEnemy;

               if ((this.enemyManager.WaveNumber % BossWaveNumber) == 0)
               {
                    newEnemy = new BossTypingEnemy(position, this.enemyManager);
               }

               else
               {
                    newEnemy = this.enemyManager.GenerateEnemy(typeOfEnemy);
               }

               AvatarTyping.EnemyManager.SpawnLocation spawnLocation = FindEmptySpawnPoint();
               
               float xSpawnPosition = PlaceEnemyAtSpawnLocation(spawnLocation);
               float zSpawnPosition = EnemyManager.MIN_SPAWN_Z + (float)random.NextDouble() * EnemyManager.MAX_SPAWN_Z;

               newEnemy.Position = new Vector3(xSpawnPosition, 0, zSpawnPosition);

               newEnemy.WorldPosition = Matrix.CreateTranslation(newEnemy.Position) *
                                        Matrix.CreateRotationY(MathHelper.ToRadians(180.0f)) *
                                        Matrix.CreateScale(1.0f);

               // If we've spawaned a Backward Enemy, we reverse his Avatar!
               if (newEnemy.GetType().Equals(typeof(BackwardTypingEnemy)))
               {
                    newEnemy.WorldPosition = Matrix.CreateTranslation(newEnemy.Position) *
                         Matrix.CreateRotationY(MathHelper.ToRadians(0.0f)) *
                         Matrix.CreateScale(1.0f);

                    newEnemy.Avatar.Rotation = new Vector3(0, 180, 0);
               }

               #region Determine if we load a Boss, and which Avatar if so

               // Fight one of a handful of friends as a Boss on Wave #10!
               if (this.enemyManager.WaveNumber == BossWaveNumber * 1)
               {
                    // 10% chance to encounter Sylent's Avatar.
                    int randomFriend = random.Next(10);

                    if (randomFriend == 0)
                         newEnemy.Avatar.LoadAvatar(CustomAvatarType.Sylent);

                    // 90% chance to encounter random Avatar.
                    else
                         newEnemy.Avatar.LoadRandomAvatar();
               }

               // Fight Mach as a Boss on Wave #20!
               else if (this.enemyManager.WaveNumber == BossWaveNumber * 2)
               {
                    // 10% chance to encounter Mach's Avatar.
                    int randomFriend = random.Next(10);

                    if (randomFriend == 0)
                         newEnemy.Avatar.LoadAvatar(PixelEngine.Avatars.CustomAvatarType.Mach);
                    
                    // 90% chance to encounter random Avatar.
                    else
                         newEnemy.Avatar.LoadRandomAvatar();
               }

               // Fight Kill Thief as a Boss on Wave #30!
               else if (this.enemyManager.WaveNumber == BossWaveNumber * 3)
               {
                    // 10% chance to encounter Mach's Avatar.
                    int randomFriend = random.Next(10);

                    if (randomFriend == 0)
                         newEnemy.Avatar.LoadAvatar(PixelEngine.Avatars.CustomAvatarType.Killthief);

                    // 90% chance to encounter random Avatar.
                    else
                         newEnemy.Avatar.LoadRandomAvatar();
               }

               // Fight me as a Boss on Wave #40!
               else if (this.enemyManager.WaveNumber == BossWaveNumber * 4)
               {
                    newEnemy.Avatar.LoadAvatar(PixelEngine.Avatars.CustomAvatarType.Matt);
               }

               #endregion

               // Otherwise, we just want a random enemy.
               else
               {
                    // New as of 3-7-2011 since I noticed Dancing Enemies already load their avatar description.
                    //if (!newEnemy.GetType().Equals(typeof(DancingTypingEnemy)))
                    //     newEnemy.Avatar.LoadRandomAvatar();
               }

               // If the new Enemy is a Boss, play the Angry animation.
               if (newEnemy.GetType().Equals(typeof(BossTypingEnemy)))
               {
                    newEnemy.Avatar.PlayAnimation(AvatarAnimationPreset.MaleAngry, true);
               }


               newEnemy.IsActive = true;

               newEnemy.SpawnLocation = spawnLocation;

               this.enemyManager.AddEnemy(newEnemy);

               this.enemyManager.numberOfEnemiesSpawned++;
          }

          #endregion
          
          #region Spawn Location Methods

          private AvatarTyping.EnemyManager.SpawnLocation FindEmptySpawnPoint()
          {
               AvatarTyping.EnemyManager.SpawnLocation enemySpawn = AvatarTyping.EnemyManager.SpawnLocation.Empty;

               // Iterate through all SpawnLocation possibilities...
               for (int i = 1; i < 5; i++)
               {
                    // If we are not using this SpawnLocation, then...
                    if (!this.enemyManager.usedSpawnLocations.Contains((AvatarTyping.EnemyManager.SpawnLocation)(i)))
                    {
                         // Use it! Add it to the list of used spawn locations.
                         this.enemyManager.usedSpawnLocations.Add((AvatarTyping.EnemyManager.SpawnLocation)(i));

                         // The enemy's spawn will be this spawn.
                         enemySpawn = (AvatarTyping.EnemyManager.SpawnLocation)i;

                         // We have found a spawn, so break from the for-loop.
                         break;
                    }
               }

               return enemySpawn;
          }

          private float PlaceEnemyAtSpawnLocation(AvatarTyping.EnemyManager.SpawnLocation spawnLocation)
          {
               float xPosition = 0f;

               switch (spawnLocation)
               {
                    case AvatarTyping.EnemyManager.SpawnLocation.FarLeft:
                         xPosition = -3f;
                         break;

                    case AvatarTyping.EnemyManager.SpawnLocation.Left:
                         xPosition = -1.25f;
                         break;

                    case AvatarTyping.EnemyManager.SpawnLocation.Right:
                         xPosition = 1.25f;
                         break;

                    case AvatarTyping.EnemyManager.SpawnLocation.FarRight:
                         xPosition = 3f;
                         break;

                    default:
                         //xPosition = -3f + (float)random.NextDouble() * (2f * 3f);
                         
                         // Between -3 and -1.
                         xPosition = -3f + (float)random.NextDouble() * (2f);

                         int LeftOrRighSide = random.Next(2);

                         if (LeftOrRighSide == 0)
                         {
                              xPosition = Math.Abs(xPosition);
                         }
                         break;
               }

               return xPosition;
          }

          #endregion
     }
}