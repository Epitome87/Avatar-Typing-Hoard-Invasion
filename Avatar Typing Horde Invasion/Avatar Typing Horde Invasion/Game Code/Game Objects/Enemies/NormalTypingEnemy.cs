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
     /// A Normal Typing Enemy is one with average Health size and average speed.
     /// </summary>
     public class NormalTypingEnemy : TypingEnemy
     {
          #region Fields

          SpriteFont font;

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor. A Normal Typing Enemy has unique values for its Fields,
          /// so we call the base instructor as well as changing more Fields.
          /// </summary>
          public NormalTypingEnemy(Vector3 position, EnemyManager enemyManager)
               : base(EngineCore.Game, enemyManager)
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
               
               this.Position = position;
               this.BasePoints = 50;
               this.BonusPoints = 50;
               this.ElapsedTime = 0.0f;
               this.IsTarget = false;
               this.WasMissed = false;
               this.ShotSound = "KeyPress";
               this.WordList.Add(new TypingWord(this.GenerateDatabaseOrCustomWord()));
               this.SpeedBonusRequirement = this.WordList[0].Length / 5.0f;
               this.Color = Color.White;

               this.Initialize();
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
     }
}