#region File Description
//-----------------------------------------------------------------------------
// DeflatableTypingEnemy.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine;
using PixelEngine.Avatars;
using PixelEngine.ResourceManagement;
using PixelEngine.Text;
#endregion

namespace AvatarTyping
{
     /// <summary>
     /// A Deflatable Typing Enemy is one which shrinks. Each correct keystroke 
     /// deflates the enemy, causing it to faint when it shrinks to pint-size.
     /// </summary>
     public class DeflatingTypingEnemy : TypingEnemy
     {
          #region Fields

          SpriteFont font;

          private float size;
          private float growthSize;

          // Helper variables.
          private bool isStunned = false;
          private float elapsedStunTime = 0.0f;

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

          private bool isRunning = false;

          #region Overridden Methods

          /// <summary>
          /// Plays the custom Run animation for the Enemy's Avatar.
          /// </summary>
          public override void Run()
          {
               if (isRunning)
               {
                    Avatar.PlayAnimation(AnimationType.Run, true);
               }

               else
               {
                    Avatar.PlayAnimation(AnimationType.Walk, true);
               }
          }

          /// <summary>
          /// Overridden OnHit().
          /// 
          /// Calls upon base.OnHit(), but also increments
          /// the Enemy's Size by GrowthSize.
          /// </summary>
          public override void OnHit()
          {
               base.OnHit();
               this.Size += this.GrowthSize;
          }

          /// <summary>
          /// Overridden OnMiss().
          /// 
          /// Calls upon base.OnMiss(), but also decrements 
          /// the Enemy's Size by GrowthSize.
          /// </summary>
          public override void OnMiss()
          {
               base.OnMiss();
               this.Size -= this.GrowthSize;
          }

          /// <summary>
          /// Overridden OnSentenceComplete.
          /// 
          /// Calls upon the base method, but also plays an Anger animation,
          /// as well as stunning the enemy for a few moments.
          /// </summary>
          public override void OnSentenceComplete()
          {
               base.OnSentenceComplete();

               // Deflatable-Specific:

               // Play Anger Animation!
               Avatar.PlayAnimation(AvatarAnimationPreset.MaleAngry, false);

               // This enemy is stunned!
               isStunned = true;
               elapsedStunTime = 0.0f;

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

          #endregion

          #region Initialization

          /// <summary>
          /// DeflatableTypingEnemy Constructor.
          /// Creates a new Deflatable Typing Enemy.
          /// </summary>
          /// <param name="position">Where to Spawn the Enemy.</param>
          /// <param name="enemyManager">The EnemyManager to manager this Enemy.</param>
          public DeflatingTypingEnemy(Vector3 position, EnemyManager enemyManager)
               : base(EngineCore.Game, enemyManager)
          {
               this.Speed = (1.20f / 2.0f) * (int)(AvatarTypingGameSettings.Difficulty + 1) * (0.50f);

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
               this.BasePoints = 100;
               this.BonusPoints = 100;
               this.ElapsedTime = 0.0f;
               this.IsTarget = false;
               this.WasMissed = false;
               this.ShotSound = "KeyPress";

               for (int i = 0; i < 3; i++)
               {
                    this.WordList.Add(new TypingWord(this.GenerateSmallWord(8, true)));
               }

               this.SpeedBonusRequirement = this.WordList[0].Length / 5.0f;

               this.Color = Color.Blue;

               this.Size = 4.0f;
               this.GrowthSize = -0.15f;

               this.Initialize();
          }

          protected override void LoadContent()
          {
               Avatar.PlayAnimation(AnimationType.Walk, true);

               enemyFont = TextManager.MenuFont;

               blankTexture = ResourceManager.LoadTexture(@"Textures\Blank Textures\Blank_Rounded_Small_WithBorder");
               borderTexture = ResourceManager.LoadTexture(@"Textures\Blank Textures\Border_Small");

               font = TextManager.MenuFont;
          }

          protected override void UnloadContent()
          {
               base.UnloadContent();
          }

          #endregion

          #region Update

          /// <summary>
          /// Overridden TypingEnemy Update method.
          /// Updates the Deflatable Typing Enemy.
          /// 
          /// Extends the Update logic to include stunning the
          /// Enemy when he is hit, for ~1.5 seconds.
          /// </summary>
          public override void Update(GameTime gameTime)
          {
               // Update their Shot-off Characters.
               UpdateShotWords();

               if (this.IsDying)
               {
                    // Increment how long he's been in his Dying state!
                    elapsedDyingTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // And update his Avatar!
                    Avatar.Update(gameTime);

                    // Return because we don't want to update his position or increase his ElapsedTime.
                    return;
               }

               // Deflatable Enemy specific logic.
               if (isStunned)
               {
                    // Increment how long the Enemy has been stunned.
                    elapsedStunTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    this.Position = new Vector3(this.Position.X, 0, this.Position.Z + this.Speed / 25f);

                    // Check to see if the Enemy has been in a Stun state for long enough.
                    if (elapsedStunTime >= 1.5f)
                    {
                         // In testing phase: If dancing is occuring while this enemy is stunned...
                         // We don't want his Run animation overriding his cheer animation.
                         if (!DancingTypingEnemy.AreWeDancing)
                         {
                              // Put the Enemy back in a Running animation.
                              Avatar.PlayAnimation(AnimationType.Run, true);
                         }

                         // Reset this counter and bool, as he is no longer Stunned!
                         elapsedStunTime = 0.0f;
                         isStunned = false;

                         // We've angered him, so now he's Running!
                         isRunning = true;
                         this.Speed = this.Speed * 1.20f;
                    }
               }

               this.Position = new Vector3(this.Position.X, 0, this.Position.Z - this.Speed / 25f);
               this.Avatar.Position = this.Position;
               this.Avatar.Scale = this.Size;

               if (this.IsTarget)
               {
                    // Increment how long we've been fighting this Enemy's Current Sentence!
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
          /// Draws the Deflatable Typing Enemy.
          /// Calls upon base.Draw.
          /// </summary>
          public override void Draw(GameTime gameTime, SpriteFont font)
          {
               base.Draw(gameTime, font);
          }

          #endregion
     }
}