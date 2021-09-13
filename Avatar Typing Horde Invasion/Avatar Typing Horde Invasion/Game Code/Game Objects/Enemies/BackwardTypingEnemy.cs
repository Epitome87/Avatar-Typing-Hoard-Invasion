#region File Description
//-----------------------------------------------------------------------------
// NormalTypingEnemy.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine;
using PixelEngine.Text;
#endregion

namespace AvatarTyping
{
     /// <summary>
     /// A Backward Typing Enemy is one that reverses the sentences of those around it.
     /// </summary>
     public class BackwardTypingEnemy : TypingEnemy
     {
          #region Fields

          SpriteFont font;

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor. A Normal Typing Enemy has unique values for its Fields,
          /// so we call the base instructor as well as changing more Fields.
          /// </summary>
          public BackwardTypingEnemy(Vector3 position, EnemyManager enemyManager)
               : base(EngineCore.Game, enemyManager)
          {
               this.Speed = (1.50f / 2.0f) * (int)(AvatarTypingGameSettings.Difficulty + 1) * (0.50f);

               if (AvatarTypingGameSettings.Difficulty == Difficulty.Hard ||
                   AvatarTypingGameSettings.Difficulty == Difficulty.Insane)
               {
                    // Make their speed initially slower than originally.
                    this.Speed *= 0.90f;
               }

               // Now add 5% speed each Wave.
               this.Speed *= MathHelper.Clamp(
                    (1.0f + (0.05f * enemyManager.WaveNumber)), 1.0f, 1.75f);

               this.Position = position;
               this.BasePoints = 250;
               this.BonusPoints = 250;
               this.ElapsedTime = 0.0f;
               this.IsTarget = false;
               this.WasMissed = false;
               this.ShotSound = "KeyPress";
               this.WordList.Add(new TypingWord(this.GenerateWord(true)));
               this.SpeedBonusRequirement = this.WordList[0].Length / 3.0f;
               this.Color = Color.Black;

               this.Initialize();

               MakeSentencesReverse();
          }

          /// <summary>
          /// Loads the appropriate content for the Normal Typing Enemy.
          /// In this case, we want a unique Sprite and Font style for the enemy.
          /// </summary>
          protected override void LoadContent()
          {
               base.LoadContent();

               font = TextManager.MenuFont;
          }

          protected override void UnloadContent()
          {
               base.UnloadContent();
          }

          #endregion

          #region Update

          /// <summary>
          /// Updates the Normal Typing Enemy.
          /// </summary>
          public override void Update(GameTime gameTime)
          {
               base.Update(gameTime);
          }

          #endregion

          #region Draw

          /// <summary>
          /// Draws the Normal Typing Enemy.
          /// </summary>
          public override void Draw(GameTime gameTime, SpriteFont font)
          {
               base.Draw(gameTime, font);
          }

          #endregion

          #region Overridden OnKilled Method

          public override void OnKilled()
          {
               MakeSentencesUnreverse();

               base.OnKilled();
          }

          #endregion

          #region Backwards Enemy-Specific Methods

          /// <summary>
          /// Backward Enemy-specific method that reverses the Sentence of 
          /// the Enemy closest to the Player.
          /// </summary>
          public void MakeSentencesReverse()
          {
               foreach (TypingEnemy enemy in this.EnemyManager.GetEnemies())
               {
                    if (enemy.WordList.Count <= 0)
                         continue;

                    // Crashed here. Items = 0
                    if (enemy.WordList[0].Text.Length <= 0)
                         continue;
                    
                    string reverseSentence = "";
     
                    // Only reverse sentence if it isn't already reversed.
                    if (!enemy.IsSentenceReversed)
                    {
                         // Crashed here: i = -1
                         for (int i = enemy.WordList[0].Text.Length - 1; i >= 0; i--)
                         {
                              reverseSentence += enemy.WordList[0].Text[i];
                         }

                         enemy.WordList[0].Text = reverseSentence;

                         enemy.IsSentenceReversed = true;

                         break;
                    }
               }
          }

          /// <summary>
          /// Backward Enemy-specific method which un-reverses an Enemy Sentence,
          /// if an Enemy currently has a reversed one.
          /// 
          /// This is called during the OnDeath() method.
          /// </summary>
          public void MakeSentencesUnreverse()
          {
               foreach (TypingEnemy enemy in this.EnemyManager.GetEnemies())
               {
                    if (enemy.WordList.Count <= 0)
                         continue;

                    // Crashed here. Items = 0
                    if (enemy.WordList[0].Text.Length <= 0)
                         continue;

                    string unreverseSentence = "";

                    if (enemy.IsSentenceReversed)
                    {
                         // Crashed here: i = -1
                         for (int i = enemy.WordList[0].Text.Length - 1; i >= 0; i--)
                         {
                              unreverseSentence += enemy.WordList[0].Text[i];
                         }

                         enemy.WordList[0].Text = unreverseSentence;

                         // New testing
                         enemy.IsSentenceReversed = false;
                         // End

                         break;
                    }
               }
          }

          #endregion
     }
}