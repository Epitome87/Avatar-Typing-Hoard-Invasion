#region File Description
//-----------------------------------------------------------------------------
// ExplodingTypingEnemy.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.Graphics;
using PixelEngine.ResourceManagement;
#endregion

namespace AvatarTyping
{
     /// <remarks>
     /// An Exploding Typing Enemy is one which explodes once the Player has
     /// put a specified amount of damage into it. Each correct keystroke 
     /// enlarges the enemy, causing it to burst when its limit is reached.
     /// Upon exploding, all surrounding enemies are eliminated if a 
     /// Bonus was obtained; otherwise, the Player is damaged.
     /// </remarks>
     public class ExplodingTypingEnemy : TypingEnemy
     {
          #region Helper Particle Data Structure

          public struct ParticleData
          {
               public float BirthTime;
               public float MaxAge;
               public Vector2 OrginalPosition;
               public Vector2 Accelaration;
               public Vector2 Direction;
               public Vector2 Position;
               public float Scaling;
               public Color ModColor;
          }

          #endregion

          #region Explosion Particle Effect Methods

          private void AddExplosion(Vector2 explosionPos, int numberOfParticles, float size, float maxAge, GameTime gameTime)
          {
               for (int i = 0; i < numberOfParticles; i++)
                    AddExplosionParticle(explosionPos, size, maxAge, gameTime);

               float rotation = (float)randomizer.Next(10);
               Matrix mat = 
                    Matrix.CreateTranslation(-explosionTexture.Texture2D.Width / 2, -explosionTexture.Texture2D.Height / 2, 0) * 
                    Matrix.CreateRotationZ(rotation) * Matrix.CreateScale(size / (float)explosionTexture.Texture2D.Width * 2.0f) * 
                    Matrix.CreateTranslation(explosionPos.X, explosionPos.Y, 0);
          }

          private void AddExplosionParticle(Vector2 explosionPos, float explosionSize, float maxAge, GameTime gameTime)
          {
               ParticleData particle = new ParticleData();

               particle.OrginalPosition = explosionPos;
               particle.Position = particle.OrginalPosition;

               particle.BirthTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
               particle.MaxAge = maxAge;
               particle.Scaling = 0.25f;
               particle.ModColor = Color.White;

               float particleDistance = (float)randomizer.NextDouble() * explosionSize;
               Vector2 displacement = new Vector2(particleDistance, 0);
               float angle = MathHelper.ToRadians(randomizer.Next(360));
               displacement = Vector2.Transform(displacement, Matrix.CreateRotationZ(angle));

               particle.Direction = displacement * 2.0f;
               particle.Accelaration = -particle.Direction;

               particleList.Add(particle);
          }

          private void UpdateParticles(GameTime gameTime)
          {
               float now = (float)gameTime.TotalGameTime.TotalMilliseconds;
               for (int i = particleList.Count - 1; i >= 0; i--)
               {
                    ParticleData particle = particleList[i];
                    float timeAlive = now - particle.BirthTime;

                    if (timeAlive > particle.MaxAge)
                    {
                         particleList.RemoveAt(i);
                    }
                    else
                    {
                         float relAge = timeAlive / particle.MaxAge;
                         particle.Position = 0.5f * particle.Accelaration * relAge * relAge + particle.Direction * relAge + particle.OrginalPosition;

                         float invAge = 1.0f - relAge;
                         particle.ModColor = new Color(new Vector4(invAge, invAge, invAge, invAge));

                         Vector2 positionFromCenter = particle.Position - particle.OrginalPosition;
                         float distance = positionFromCenter.Length();
                         particle.Scaling = (50.0f + distance) / 200.0f;

                         particleList[i] = particle;
                    }
               }
          }

          private void DrawExplosion()
          {
               for (int i = 0; i < particleList.Count; i++)
               {
                    ParticleData particle = particleList[i];
                    
                    MySpriteBatch.Draw(explosionTexture.Texture2D, particle.Position, null, particle.ModColor, i, new Vector2(256, 256), particle.Scaling, SpriteEffects.None, 1);
               }
          }

          #endregion

          #region Fields

          SpriteFont font;

          private float size;
          private float growthSize;

          // Fields for explosion creation.
          private List<ParticleData> particleList = new List<ParticleData>();
          private GameResourceTexture2D explosionTexture;
          private Random randomizer = new Random();
          private bool addExplosion = true;

          #endregion

          #region Properties

          /// <summary>
          /// Gets or Sets the Size of this Enemy.
          /// 
          /// When setting, Size is clamped between 0.5 and 5.
          /// </summary>
          public float Size
          {
               get { return size; }
               set
               {
                    size = MathHelper.Clamp(value, 0.5f, 4.0f);
               }
          }

          /// <summary>
          /// Gets or Sets how much the Enemy's Size is
          /// incremented by per correct key-stroke.
          /// 
          /// Use a negative number for negative growth!
          /// </summary>
          public float GrowthSize
          {
               get { return growthSize; }
               set { growthSize = value; }
          }

          #endregion

          #region Overridden Methods

          /// <summary>
          /// Overridden OnHit method.
          /// 
          /// Calls upon base.OnHit, but also increments
          /// the Enemy's Size by GrowthSize.
          /// </summary>
          public override void OnHit()
          {
               base.OnHit();
               this.Size += this.GrowthSize;
          }

          /// <summary>
          /// Overridden OnMiss method.
          /// 
          /// Calls upon base.OnMiss, but also decrements
          /// the Enemy's Size by GrowthSize.
          /// </summary>
          public override void OnMiss()
          {
               base.OnMiss();
               this.Size -= this.GrowthSize;
          }

          /// <summary>
          /// Overridden OnKilled method.
          /// 
          /// Calls upon base.OnKilled, but also makes the Enemy
          /// explode - either friendly or hostile depending on if a Bonus
          /// was earned.
          /// </summary>
          public override void OnKilled()
          {
               base.OnKilled();

               // If the Player earned any Bonus Kills, make the
               // explosion a friendly one.
               if (this.IsPerfectKill || this.IsSpeedKill)
               {
                    ExplodeFriendly();
               }

               // Otherwise, make the explosion an unfriendly one!
               else
               {
                    ExplodeUnfriendly();
               }
          }

          #endregion

          #region Initialization

          /// <summary>
          /// ExplodingTypingEnemy Constructor.
          /// Creates a new Exploding Typing Enemy.
          /// </summary>
          /// <param name="position">Where to Spawn the Enemy.</param>
          /// <param name="enemyManager">The EnemyManager to manager this Enemy.</param>
          public ExplodingTypingEnemy(Vector3 position, EnemyManager enemyManager)
               : base(PixelEngine.EngineCore.Game, enemyManager)
          {
               this.Speed = (1.5f / 2.0f) * (int)(AvatarTypingGameSettings.Difficulty + 1) * (0.50f);

               if (AvatarTypingGameSettings.Difficulty == Difficulty.Hard ||
                   AvatarTypingGameSettings.Difficulty == Difficulty.Insane)
               {
                    // Make their speed initially slower than originally.
                    this.Speed *= 0.90f;
               }

               // Now add 5% speed each Wave.
               this.Speed *= MathHelper.Clamp(
                    (1.0f + (0.05f * enemyManager.WaveNumber)), 1.0f, 1.75f);

               // End New Test

               this.Position = position;
               this.BasePoints = 200;
               this.BonusPoints = 200;
               this.ElapsedTime = 0.0f;
               this.IsTarget = false;
               this.WasMissed = false;

               this.ShotSound = "KeyPress";
               this.DeathSound = "Explosion";

               this.WordList.Add(new TypingWord(this.GenerateWord(true)));

               this.Color = Color.Yellow;

               this.Size = 1.0f;
               this.GrowthSize = 0.15f;

               this.Initialize();
          }

          /// <summary>
          /// Overridden LoadContent method.
          /// 
          /// Cals upon base.LoadContent, but also loads the explosion texture.
          /// </summary>
          protected override void LoadContent()
          {
               base.LoadContent();

               font = PixelEngine.Text.TextManager.MenuFont;

               explosionTexture = ResourceManager.LoadTexture(@"Textures\explosion");
          }

          protected override void UnloadContent()
          {
               base.UnloadContent();
          }

          #endregion

          #region Update

          /// <summary>
          /// Overridden TypingEnemy Update method.
          /// Updates the Exploding Typing Enemy.
          /// 
          /// Extends the Update logic to include adding explosions
          /// and updating the explosion particles, 
          /// </summary>
          public override void Update(GameTime gameTime)
          {
               // Update their Shot-off Characters.
               UpdateShotWords();

               if (this.IsDying)
               {
                    elapsedDyingTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Avatar.Update(gameTime);

                    if (particleList.Count > 0)
                         UpdateParticles(gameTime);

                    if (addExplosion)
                    {
                         Vector2 explosionPosition = GraphicsHelper.ConvertToScreenspace(this.WorldPosition);
                         AddExplosion(explosionPosition, 16, 45.0f * 10, 5000.0f, gameTime);
                         addExplosion = false;
                    }

                    return;
               }

               this.Position = new Vector3(this.Position.X, 0, this.Position.Z - this.Speed / 25f);
               this.Avatar.Position = this.Position;
               this.Avatar.Scale = this.Size;
               // Doesn't look like we need to set World Position?
               this.WorldPosition = Matrix.CreateScale(this.Size) *
                                    Matrix.CreateRotationY(MathHelper.ToRadians(0.0f)) *
                                    Matrix.CreateTranslation(this.Position);

               if (this.IsTarget)
               {
                    this.ElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // Increment how long we've been fighting / targetting this Enemy!
                    this.ElapsedKillTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
               }

               Avatar.Update(gameTime);
          }

          #endregion

          #region Draw

          /// <summary>
          /// Overridden TypingEnemy Draw method.
          /// Draws the Exploding Typing Enemy.
          /// Calls upon base.Draw, but also draws an explosion if there is one.
          /// </summary>
          public override void Draw(GameTime gameTime, SpriteFont font)
          {
               base.Draw(gameTime, font);

               MySpriteBatch.Begin(BlendState.Additive, SpriteSortMode.Deferred);
               DrawExplosion();
               MySpriteBatch.End();
          }
 
          #endregion

          #region Exploding Enemy-Specific Methods

          /// <summary>
          /// Explodes the Exploding Enemy, sending shrapnel toward the 
          /// other Non-Exploding Enemies.
          /// 
          /// This should be called when a Bonus was obtained from this Enemy.
          /// </summary>
          public void ExplodeFriendly()
          {
               foreach (TypingEnemy nearbyEnemy in EnemyManager.GetEnemies())
               {
                    // The Enemy shouldn't explode itself, obviously!
                    if (nearbyEnemy == this)
                         continue;

                    // Do not affect other Exploding Enemies (they're flame resistant!)
                    if (nearbyEnemy.GetType().Equals(typeof(ExplodingTypingEnemy)))
                         continue;

                    // The Enemy is now Killed.
                    nearbyEnemy.OnExplode(this.SentencePosition);

                    // He's now dead.
                    nearbyEnemy.OnKilled();
               }

               // BURN HIM TO A CRISP!!!
               this.Avatar.LightColor = new Vector3(0, 0, 0);
               this.Avatar.LightDirection = new Vector3(0, 0, 0);
               this.Avatar.AmbientLightColor = new Vector3(-1, -1, -1);
          }

          /// <summary>
          /// Explodes the Exploding Enemy, sending shrapnel toward the player.
          /// 
          /// This should be called when no Bonus was obtained from this Enemy.
          /// </summary>
          public void ExplodeUnfriendly()
          {
               // Hurt the Player 1 Health point.
               EnemyManager.currentPlayer.Health--;

               // BURN HIM TO A CRISP!!!
               this.Avatar.LightColor = new Vector3(0, 0, 0);
               this.Avatar.LightDirection = new Vector3(0, 0, 0);
               this.Avatar.AmbientLightColor = new Vector3(-1, -1, -1);
          }

          #endregion
     }
}