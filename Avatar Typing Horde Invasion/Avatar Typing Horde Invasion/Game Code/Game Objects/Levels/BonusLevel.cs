#region File Description
//-----------------------------------------------------------------------------
// BonusLevel.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace AvatarTyping
{
     /// <remarks>
     /// A small environment with collections of items and enemies.
     /// The level owns the player and controls the game's win and lose
     /// conditions as well as scoring.
     /// 
     /// A Bonus Level consists of Bonus Points which the player can obtain.
     /// </remarks>
     public class BonusLevel : Level
     {
          #region Fields

          private float timeLimit;
          private uint maxCoins;
          private uint collectedCoins;

          // Art assets...

          #endregion

          #region Properties

          /// <summary>
          /// The Time in which the player has to collect bonus points.
          /// </summary>
          public float TimeLimit
          {
               get { return timeLimit; }
               set { timeLimit = value; }
          }

          /// <summary>
          /// The maximum possible amount of bonus points that can be collected.
          /// </summary>
          public uint MaxCoins
          {
               get { return maxCoins; }
               set { maxCoins = value; }
          }

          /// <summary>
          /// The amount of bonus points the Player has currently collected.
          /// </summary>
          public uint CollectedCoins
          {
               get { return collectedCoins; }
               set { collectedCoins = value; }
          }

          #endregion

          #region Initialization

          public BonusLevel(Game game)
               : base(game)
          {
          }

          #endregion

          #region Handle Input

          #endregion

          #region Update

          #endregion

          #region Draw

          #endregion

          #region Helper Draw Methods

          #region Draw Overlay

          protected override void DrawOverlay(GameTime gameTime)
          {
               base.DrawOverlay(gameTime);
          }

          #endregion

          #endregion
     }
}