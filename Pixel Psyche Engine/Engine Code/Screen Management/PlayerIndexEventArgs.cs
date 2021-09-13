#region File Description
//-----------------------------------------------------------------------------
// PlayerIndexEventArgs.cs
// Matt McGrath, with help provided by XNA Community Game Platform.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
#endregion

namespace PixelEngine.Screen
{
     /// <summary>
     /// Custom event argument which includes the index of the player who
     /// triggered the event. This is used by the MenuEntry.Selected event.
     /// </summary>
     public class PlayerIndexEventArgs : EventArgs
     {
          #region Fields

          private PlayerIndex playerIndex;

          #endregion

          #region Properties

          /// <summary>
          /// Gets the index of the player who triggered this event.
          /// </summary>
          public PlayerIndex PlayerIndex
          {
               get { return playerIndex; }
          }

          #endregion

          #region Constructor

          /// <summary>
          /// Constructor.
          /// </summary>
          public PlayerIndexEventArgs(PlayerIndex playerIndex)
          {
               this.playerIndex = playerIndex;
          }

          #endregion
     }
}
