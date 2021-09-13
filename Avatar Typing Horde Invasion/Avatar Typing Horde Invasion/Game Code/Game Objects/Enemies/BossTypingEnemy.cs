#region File Description
//-----------------------------------------------------------------------------
// BossTypingEnemy.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PixelEngine;
using PixelEngine.Audio;
using PixelEngine.Avatars;
using PixelEngine.CameraSystem;
using PixelEngine.Graphics;
using PixelEngine.Screen;
using PixelEngine.Text;
#endregion

namespace AvatarTyping
{
     #region Projectile Class

     public class Projectile
     {
          #region Fields

          public float Speed;
          public Vector3 Position;
          public Vector2 SentencePosition;
          public TypingWord Sentence;
          public bool IsTargetted;

          #endregion

          #region Initialization

          public Projectile()
          {
               Speed = 2.0f;
               Position = new Vector3(0, 0, 0);
               SentencePosition = new Vector2();
               IsTargetted = false;
          }

          #endregion

          #region Updating

          public void Update()
          {
               Position = new Vector3(Position.X,
                    Position.Y - (0.007f) * (Speed / 2.5f),
                    Position.Z - Speed / 25f);

               Matrix mat = Matrix.CreateTranslation(Position);

               SentencePosition = GraphicsHelper.ConvertToScreenspace(mat);
          }

          #endregion
     }

     #endregion

     /// <summary>
     /// A Boss Typing Enemy is one with Large Health size and Very Slow speed.
     /// A Boss will contain a series of Words, rather than just one.
     /// </summary>
     class BossTypingEnemy : TypingEnemy
     {
          #region Fields

          SpriteFont font;
          ContentManager content;

          public List<Projectile> ProjectileList = new List<Projectile>();
          Model3D projectileModel;

          float elapsedTimeSinceThrew = 0.0f;
          float timeSinceThrown = 0.0f;
          float timeBetweenThrows = 15.0f;
          bool isThrowing = false;

          float rotation = 0;

          AvatarBaseAnimation bossAngryAnimation = new AvatarBaseAnimation(AvatarAnimationPreset.MaleAngry);
          
          private float originalHealth;
          
          #endregion

          #region Initialization

          public BossTypingEnemy(Vector3 position, EnemyManager enemyManager)
               : base(EngineCore.Game, enemyManager)
          {
               this.Speed = 0.0f;
               this.Position = new Vector3(0, 0, 5f);

               for (int i = 0; i < ((5 + enemyManager.WaveNumber * 2)); i++)
               {
                    this.WordList.Add(new TypingWord(this.GenerateWord(true)));
               }

               originalHealth = WordList.Count;

               if (enemyManager.WaveNumber < 6)
               {
                    timeBetweenThrows = 12;
               }

               else if (enemyManager.WaveNumber < 11)
               {
                    timeBetweenThrows = 10;
               }

               else if (enemyManager.WaveNumber < 16)
               {
                    timeBetweenThrows = 8;
               }

               else if (enemyManager.WaveNumber < 21)
               {
                    timeBetweenThrows = 7.5f;
               }

               else
               {
                    timeBetweenThrows = 7.0f;
               }

               this.BasePoints = 10000;
               this.BonusPoints = 500;
               this.ElapsedTime = 0.0f;
               this.IsTarget = false;
               this.WasMissed = false;
               this.Color = Color.CornflowerBlue;
               this.Initialize();
          }

          protected override void LoadContent()
          {
               base.LoadContent();

               font = TextManager.MenuFont;


               if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

               projectileModel = new Model3D();
               projectileModel.Model = content.Load<Model>(@"Models\Ball");
               //projectileModel.AmbientLightColor = Color.Blue;
               projectileModel.EmissiveColor = Color.Orange;
               projectileModel.SpecularColor = Color.Blue;
          }

          #endregion

          #region Overridden Methods

          public override void OnSentenceComplete()
          {
               base.OnSentenceComplete();

               // Reset this fresh for the enemy's next sentence.
               this.WasMissed = false;

               this.ElapsedTime = 0.0f;

               if (this.WordList != null)
               {
                    if (this.WordList.Count > 0)
                    {
                         // Add 1.5 second padding since the timer starts immediately.
                         this.SpeedBonusRequirement = 1.5f + (this.WordList[0].Length / 5);
                    }
               }
          }

          public override void OnKilled()
          {
               base.OnKilled();

               //AudioManager.PlayMusic("One-eyed Maestro");//SpyVsSpy");
          }

          #endregion

          #region Handle Input

          bool shift;

          public void HandleProjectInput(InputState input, PlayerIndex? ControllingPlayer, PlayerIndex playerIndex)
          {
               foreach (Projectile projectile in ProjectileList)
               {
                    // If a Key was pressed, and If we have not finish the current target's word...
                    if (input.IsNewKeyPress(null, out playerIndex) && projectile.Sentence.Length > 0)
                    {
                         // Check if the Key pressed is the key necessary to Hurt the Enemy.
                         if (input.IsNewKeyPress(projectile.Sentence.KeyList[0], null, out playerIndex))
                         {
                              // If the word requires a Shift...
                              if (!KeysHelper.IsShiftRequired(projectile.Sentence.Text[0]) ||
                                   (KeysHelper.IsShiftRequired(projectile.Sentence.Text[0]) && shift))
                              {
                                   // Hit the enemy!
                                   AudioManager.PlayCue(this.ShotSound);

                                   // The Enemy's Word is now minus the first character.
                                   projectile.Sentence.Text =
                                        (projectile.Sentence.Text).Substring(1, projectile.Sentence.Text.Length - 1);


                                   // If the Enemy's Word is of length 0 or less, he's dead.
                                   if (projectile.Sentence.Length <= 0 || !this.IsAlive)
                                   {
                                        // New for WPM logic
                                        this.CharactersTyped += projectile.Sentence.CharacterCount;
                                        // End

                                        ProjectileList.Remove(projectile);
                                        return;
                                   }
                              }

                              else if (KeysHelper.IsShiftRequired(projectile.Sentence.Text[0]) && !shift)
                              {
                                   this.OnMiss();
                              }
                         }

                         // Else we must have Missed the Enemy, losing our Bonus chance.
                         else
                         {
                              if (!input.IsNewKeyPress(Keys.LeftShift, null, out playerIndex)
                                   && !input.IsNewKeyPress(Keys.RightShift, null, out playerIndex))
                              {
                                   this.OnMiss();
                              }
                         }
                    }
               }
          }

          public override void HandleInput(InputState input, PlayerIndex? ControllingPlayer, PlayerIndex playerIndex)
          {
               if (ProjectileList.Count > 0)
               {
                    HandleProjectInput(input, ControllingPlayer, playerIndex);
                    return;
               }

               if (!this.IsTarget)
                    return;

               shift = false;

               if (input.IsKeyDown(Keys.LeftShift, null, out playerIndex)
                    || input.IsKeyDown(Keys.RightShift, null, out playerIndex))
               {
                    // Don't make it count against the player.
                    shift = true;
               }

               // If a Key was pressed, and If we have not finish the current target's word...
               if (input.IsNewKeyPress(null, out playerIndex) && this.WordList[0].Length > 0)
               {
                    // Check if the Key pressed is the key necessary to Hurt the Enemy.
                    if (input.IsNewKeyPress(this.WordList[0].KeyList[0], null, out playerIndex))
                    {
                         // If the word requires a Shift...
                         if (!KeysHelper.IsShiftRequired(this.WordList[0].Text[0]) ||
                              (KeysHelper.IsShiftRequired(this.WordList[0].Text[0]) && shift))
                         {
                              // Hit the enemy!
                              this.OnHit();

                              // If the Enemy's Word is of length 0 or less, he's dead.
                              if (this.WordList[0].Length <= 0 || !this.IsAlive)
                              {
                                   // New for WPM logic
                                   this.CharactersTyped += this.WordList[0].CharacterCount;
                                   // End

                                   this.WordList.RemoveAt(0);

                                   this.OnSentenceComplete();

                                   if (this.WordList.Count <= 0)
                                   {
                                        this.OnKilled();
                                   }
                              }
                         }

                         else if (KeysHelper.IsShiftRequired(this.WordList[0].Text[0]) && !shift)
                         {
                              this.OnMiss();
                         }
                    }

                    // Else we must have Missed the Enemy, losing our Bonus chance.
                    else
                    {
                         if (!input.IsNewKeyPress(Keys.LeftShift, null, out playerIndex)
                              && !input.IsNewKeyPress(Keys.RightShift, null, out playerIndex))
                         {
                              this.OnMiss();
                         }
                    }
               }
          }

          #endregion

          #region Update

          /// <summary>
          /// 
          /// </summary>
          public override void Update(GameTime gameTime)
          {
               //if (Avatar == null)
               //     return;

               if (ProjectileList != null)
               {
                    foreach (Projectile projectile in ProjectileList)
                    {
                         projectile.Update();
                    }
               }

               if (ProjectileList != null)
               {
                    foreach (Projectile projectile in ProjectileList)
                    {
                         if (projectile.Position.Z <= 0f)
                         {
                              ProjectileList.Remove(projectile);
                              AudioManager.PlayCue("Explosion");
                              EnemyManager.currentPlayer.Health--;
                              return;
                         }
                    }
               }

               if (ProjectileList != null && ProjectileList.Count < 1)
               {
                    this.ElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
               }

               // Update their Shot-off Characters.
               UpdateShotWords();

               if (this.IsDying)
               {
                    elapsedDyingTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Avatar.Update(gameTime);
                    return;
               }

               this.Position = new Vector3(0, 0, 20f);
               this.Avatar.Position = this.Position;
               this.Avatar.Scale = 4.0f;

               if (this.IsTarget) // This use to be commented out.
               {
                    //this.ElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    elapsedTimeSinceThrew += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    this.ElapsedKillTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
               }


               if (elapsedTimeSinceThrew >= timeBetweenThrows)
               {
                    isThrowing = true;
                    elapsedTimeSinceThrew = 0.0f;
                    Avatar.PlayAnimation(AnimationType.Throw, false);
               }

               if (isThrowing)
               {
                    timeSinceThrown += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (timeSinceThrown >= 0.75f)
                    {
                         timeSinceThrown = 0.0f;
                         ShootProjectile();
                         isThrowing = false;
                    }
               }

               Avatar.Update(gameTime);

               // If we are using an AvatarCustomAnimation, it must be the Throw animation...
               if (Avatar.AvatarAnimation.GetType() == typeof(AvatarCustomAnimation))
               {
                    // If the Throw animation is finished...
                    if (Avatar.AvatarAnimation.IsFinished)
                    {
                         // Play his generic, angry Animation!
                         Avatar.PlayAnimation(bossAngryAnimation, true);
                    }
               }
          }

          #endregion

          #region Draw

          public void DrawHealthBar(GameTime gameTime)
          {
               int startingX = 800 - 625;
               int startingY = 200 + 50;
               int width = 150;
               int height = 20;

               TextManager.DrawCentered(false, ScreenManager.Font, "Boss' Health", new Vector2(875f - 625, 170f + 50),
                    Color.CornflowerBlue, 0.75f);

               MySpriteBatch.Draw(blankTexture.Texture2D,
                    new Rectangle(startingX, startingY, width, height), Color.Gray * (100 / 255f));

               MySpriteBatch.Draw(blankTexture.Texture2D, new Rectangle(startingX, startingY,
                         (int)((WordList.Count / originalHealth) * width), height),
                         Color.LightGreen);

               MySpriteBatch.Draw(borderTexture.Texture2D,
                    new Rectangle(startingX, startingY, width, height), Color.Black);
          }

          /// <summary>
          /// Draws the animated enemy.
          /// </summary>
          public override void Draw(GameTime gameTime, SpriteFont font)
          {
               MySpriteBatch.Begin(BlendState.AlphaBlend);

               // Draw the enemy (his / her avatar).
               Avatar.Draw(gameTime);

               // If the enemy is dying...
               if (this.IsDying)
               {
                    // If Perfect Kill, alert the player.
                    if (this.IsPerfectKill)
                         DrawPerfectDisplay(gameTime);

                    // If Speed Kill, alert the player.
                    if (this.IsSpeedKill)
                         DrawSpeedDisplay(gameTime);
               }

               this.ShowSentence = false;

               if (ProjectileList.Count < 1)
               {
                    this.ShowSentence = true;

                    // Draw the enemy's word.
                    //DrawWords(gameTime, font);

                    // Draw the enemy's health bar.
                    DrawHealthBar(gameTime);
               }

               // Draw the enemy's characters flinging off the screen.
               DrawShotWords(gameTime, font);

               foreach (Projectile projectile in ProjectileList)
               {
                    Matrix mat = Matrix.CreateScale(0.01f) *
                         Matrix.CreateRotationX(MathHelper.ToRadians(rotation += 5.0f)) *
                         Matrix.CreateRotationY(MathHelper.ToRadians(rotation / 2.0f)) *
                         Matrix.CreateTranslation(new Vector3(projectile.Position.X, projectile.Position.Y, projectile.Position.Z));


                    projectileModel.DrawModel(mat);
                    DrawProjectileWord(projectile, gameTime, font);
               }

               MySpriteBatch.End();
          }

          public void DrawProjectileWord(Projectile projectile, GameTime gameTime, SpriteFont font)
          {
               string word = "";

               if (projectile.Sentence.Length > 0)
                    word = projectile.Sentence.Text;

               Vector2 textPosition = new Vector2(projectile.Position.X, projectile.Position.Y);
               Vector2 textOrigin = new Vector2(0, 0);

               Matrix mat = Matrix.CreateTranslation(projectile.Position);
               projectile.SentencePosition = GraphicsHelper.ConvertToScreenspace(mat);
               textPosition = projectile.SentencePosition;

               projectile.SentencePosition.Y -= 35f;
               textPosition.Y -= 35f;

               float fontSize = AvatarTypingGameSettings.TextSize;

               if (projectile.Position.Z > CameraManager.ActiveCamera.Position.Z)
               {
                    if (IsTarget)
                    {
                         fontSize *= 2.0f;

                         Vector2 widthHeight = font.MeasureString(TextManager.WrapText(word, 200f)) * fontSize;
                         widthHeight.X += 10f;
                         widthHeight.Y += 10f;

                         Rectangle wordBorder = new Rectangle((int)(textPosition.X - (widthHeight.X / 2f)), (int)(textPosition.Y - (12.5f * fontSize) - (widthHeight.Y / 2f)), (int)widthHeight.X, (int)widthHeight.Y);

                         MySpriteBatch.Draw(borderTexture.Texture2D, wordBorder, Color.White);
                         MySpriteBatch.Draw(blankTexture.Texture2D, wordBorder, Color.LightGreen * (1f));

                         TextManager.DrawCentered(true, font, TextManager.WrapText(word, 200f), textPosition, Color.Red, fontSize);
                    }

                    else
                    {
                         Vector2 widthHeight = font.MeasureString(TextManager.WrapText(word, 200f)) * fontSize;
                         widthHeight.X += 10f;
                         widthHeight.Y += 10f;

                         Rectangle wordBorder = new Rectangle((int)(textPosition.X - (widthHeight.X / 2f)), (int)(textPosition.Y - (12.5f * fontSize) - (widthHeight.Y / 2f)), (int)widthHeight.X, (int)widthHeight.Y);

                         MySpriteBatch.Draw(borderTexture.Texture2D, wordBorder, Color.White);
                         MySpriteBatch.Draw(blankTexture.Texture2D, wordBorder, Color.LightGreen * (100f / 255f));

                         TextManager.DrawCentered(true, font, TextManager.WrapText(word, 200f), textPosition, Color.White, fontSize);
                    }
               }
          }

          #endregion

          #region Projectile Shooting Methods

          public void ShootProjectile()
          {
               Random random = new Random();

               Projectile projectile = new Projectile();

               projectile.SentencePosition = GraphicsHelper.ConvertToScreenspace(this.WorldPosition);

               projectile.Position = new Vector3(this.Position.X, 3.0f, this.Position.Z + 1.0f);
               projectile.Speed = (2.5f / 2.0f) * (int)(AvatarTypingGameSettings.Difficulty + 1) * (0.50f);
               projectile.Sentence = new TypingWord(GenerateWord(true));

               ProjectileList.Add(projectile);
          }

          #endregion
     }
}