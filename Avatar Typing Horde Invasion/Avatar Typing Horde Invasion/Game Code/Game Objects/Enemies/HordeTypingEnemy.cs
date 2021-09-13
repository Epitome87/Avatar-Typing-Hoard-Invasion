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
using System;
#endregion

namespace AvatarTyping
{
     /// <summary>
     /// A Normal Typing Enemy is one with average Health size and average speed.
     /// </summary>
     public class HordeTypingEnemy : TypingEnemy
     {
          #region Fields

          SpriteFont font;

          Random random = new Random();

          string[] letters = new string[]
               {
                    "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", 
                    "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"
               };

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor. A Normal Typing Enemy has unique values for its Fields,
          /// so we call the base instructor as well as changing more Fields.
          /// </summary>
          public HordeTypingEnemy(Vector3 position, EnemyManager enemyManager)
               : base(EngineCore.Game, enemyManager)
          {
               this.Speed = (7.5f / 2.0f) * (int)(AvatarTypingGameSettings.Difficulty + 1) * (0.50f);

               // New Test

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
               this.BasePoints = 50;
               this.BonusPoints = 50;
               this.ElapsedTime = 0.0f;
               this.IsTarget = false;
               this.WasMissed = false;
               this.ShotSound = "KeyPress";
               this.WordList.Add(new TypingWord(GenerateLetter()));
               this.SpeedBonusRequirement = this.WordList[0].Length / 5.0f;
               this.Color = Color.LightGreen;

               // NEW: TESTING:
               this.DamageDoneToPlayer = 0;

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

          #region Update

          /// <summary>
          /// Updates the Normal Typing Enemy.
          /// 
          /// Simply calls upon base.Update.
          /// </summary>
          public override void Update(GameTime gameTime)
          {
               base.Update(gameTime);
          }

          #endregion

          #region Draw

          /// <summary>
          /// Draws the Normal Typing Enemy.
          /// 
          /// Simply calls base.Draw.
          /// </summary>
          public override void Draw(GameTime gameTime, SpriteFont font)
          {
               base.Draw(gameTime, font);
          }

          #endregion

          public string GenerateLetter()
          {
               string letter = letters[random.Next(26)];

               return letter;
          }
     }
}