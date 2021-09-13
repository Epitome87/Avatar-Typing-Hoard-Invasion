#region File Description
//-----------------------------------------------------------------------------
// FastTypingEnemy.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace AvatarTyping
{
     /// <summary>
     /// A Fast Typing Enemy is one which moves quickly, but has a short Health Bar.
     /// </summary>
     public class FastTypingEnemy : TypingEnemy
     {
          #region Fields

          SpriteFont font;

          #endregion

          #region Initialization

          /// <summary>
          /// FastTypingEnemy Constructor.
          /// Creates a new Fast Typing Enemy.
          /// </summary>
          /// <param name="position">Where to Spawn the Enemy.</param>
          /// <param name="enemyManager">The EnemyManager to manager this Enemy.</param>
          public FastTypingEnemy(Vector3 position, EnemyManager enemyManager)
               : base(PixelEngine.EngineCore.Game, enemyManager)
          {
               this.Speed = (2.25f / 2.0f) * (int)(AvatarTypingGameSettings.Difficulty + 1) * (0.50f);

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
               this.BasePoints = 100;
               this.BonusPoints = 100;
               this.ElapsedTime = 0.0f;
               this.IsTarget = false;
               this.WasMissed = false;
               this.ShotSound = "KeyPress";
               this.WordList.Add(new TypingWord(this.GenerateWord(true)));
               this.SpeedBonusRequirement = this.WordList[0].Length / 5.0f;               // I added this 3-1-2011; why wasn't it here?
               this.Color = Color.DarkGreen;

               this.Initialize();
          }

          /// <summary>
          /// Overridden LoadContent method.
          /// 
          /// Cals upon base.LoadContent, but also loads this Enemy's font.
          /// </summary>
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
     }
}