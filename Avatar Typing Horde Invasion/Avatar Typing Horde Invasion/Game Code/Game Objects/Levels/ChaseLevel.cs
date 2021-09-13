#region File Description
//-----------------------------------------------------------------------------
// ChaseLevel.cs
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
     /// A ChaseLevel type of Level is one where the Player is being chased
     /// or chasing an object (enemy, objective, etc).
     /// </remarks>
     public class ChaseLevel : Level
     {
          #region Fields

          private Vector3 startingPosition;
          private Vector3 endingPosition;
          private Vector3 chaserDistance;

          #endregion

          #region Properties

          public Vector3 StartingPosition
          {
               get { return startingPosition; }
               set { startingPosition = value; }
          }

          public Vector3 EndingPosition
          {
               get { return endingPosition; }
               set { endingPosition = value; }
          }

          public Vector3 ChaserDistance
          {
               get { return chaserDistance; }
               set { chaserDistance = value; }
          }

          #endregion

          #region Initizliation 

          public ChaseLevel(Game game)
               : base(game)
          {
               startingPosition = new Vector3();
               endingPosition = new Vector3();
               chaserDistance = new Vector3();
          }

          #endregion

          #region Handle Input

          #endregion

          #region Update

          #endregion

          #region Draw

          public override void Draw(GameTime gameTime)
          {
          }

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