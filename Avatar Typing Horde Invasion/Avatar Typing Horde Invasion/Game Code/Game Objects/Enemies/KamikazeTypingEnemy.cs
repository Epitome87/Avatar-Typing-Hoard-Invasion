#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.Screen;
using PixelEngine.Audio;
#endregion

namespace AvatarTyping
{
     /// <summary>
     /// A Suicide Typing Enemy is one with zero tolerance toward missed keystrokes!
     /// A Suicide Typing Enemy will explode upon an incorrect keystroke.
     /// </summary>
     public class SuicideTypingEnemy : TypingEnemy
     {
          #region Fields

          private bool didCommitSuicide = false;

          SpriteFont font;

          #endregion

          #region Initialization

          public SuicideTypingEnemy(Vector3 position, EnemyManager enemyManager)
               : base(PixelEngine.EngineCore.Game, enemyManager)
          {
               this.Speed = (1.0f / 2.0f) * (int)(AvatarTypingGameSettings.Difficulty + 1) * (0.50f);

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
               this.BasePoints = 150;
               this.BonusPoints = 150;
               this.ElapsedTime = 0.0f;
               this.IsTarget = false;
               this.WasMissed = false;

               this.ShotSound = "KeyPress";

               this.WordList.Add(new TypingWord(this.GenerateWord(true)));

               this.Color = Color.DarkRed;

               this.Initialize();
          }

          protected override void LoadContent()
          {
               base.LoadContent();

               font = PixelEngine.Text.TextManager.MenuFont;
          }

          protected override void UnloadContent()
          {
               base.UnloadContent();
          }

          #endregion

          #region Update

          public override void Update(GameTime gameTime)
          {
               base.Update(gameTime);
          }

          #endregion

          #region Draw

          /// <summary>
          /// Draws the Suicide Typing Enemy enemy.
          /// </summary>
          public override void Draw(GameTime gameTime, SpriteFont font)
          {
               base.Draw(gameTime, font);
          }

          #endregion

          #region Overridden Methods

          /// <summary>
          /// Overridden OnMiss Method.
          /// For a Suicide Enemy, a Miss leads to the Enemy's Suicide!
          /// </summary>
          public override void OnMiss()
          {
               CommitSuicide();
          }

          /// <summary>
          /// Overridden CheckSpeedBonus Method.
          /// For a Suicide Enemy, we don't want to award bonus Speed points
          /// if the Suicide Enemy commited suicide.
          /// </summary>
          /// <returns></returns>
          public override bool CheckSpeedBonus()
          {
               // No Speed Bonus if Speed Requirement isn't met, or if 
               // the Enemy committed suicide!
               if (this.ElapsedTime > this.SpeedBonusRequirement || this.didCommitSuicide)
                    return false;

               else
               {
                    BasePoints += BonusPoints;
                    return true;
               }
          }

          public override bool IsCollision(Vector3 playerPosition)
          {
               return (base.IsCollision(playerPosition) || WasMissed);
          }

          /// <summary>
          /// Overridden OnKilled method.
          /// 
          /// A Suicide Enemy is different in that we award no Bonus upon death
          /// if the Suicide Enemy committed suicide, and was therefore not 
          /// technically killed by the player.
          /// </summary>
          public override void OnKilled()
          {
               // If the Enemy did not commit suicide...
               if (!this.didCommitSuicide)
               {
                    // Handle his death normally.
                    base.OnKilled();
               }

               // THIS IS NEWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW
               // If the Enemy did commit suicide...
               else
               {
                    // Don't reward points!
                    this.BasePoints = 0;

                    // The enemy is no longer alive, and thus is not the target.
                    this.IsAlive = false;
                    this.IsTarget = false;
                    this.IsDying = true;

                    this.IsActive = false;

                    this.EnemyManager.usedSpawnLocations.Remove(SpawnLocation);

                    AudioManager.PlayCue(this.DeathSound);

                    // Make the enemy's avatar begin the Faint animation.
                    this.Faint();
               }
          }

          #endregion

          #region Suicide-Specific Methods

          /// <summary>
          /// Explodes the Suicide Enemy, sending shrapnel toward the player.
          /// </summary>
          public void CommitSuicide()
          {
               base.OnMiss();
               this.didCommitSuicide = true;
               //this.WasKilledByPlayer = false;
               this.OnKilled();
          }

          #endregion
     }
}