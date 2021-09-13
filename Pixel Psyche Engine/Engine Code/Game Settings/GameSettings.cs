#region File Description
//-----------------------------------------------------------------------------
// GameSettings.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
#endregion

namespace PixelEngine
{
     /// <summary>
     /// Maintains Game Settings, such as those which describe audio and video properties.
     /// Normally, many of the properties here can be found and modified via an Options Menu.
     /// </summary>
     //[Serializable]
     public abstract class GameSettings
     {
          #region Properties

          protected string playerName = String.Empty;
          protected Rectangle safeArea = new Rectangle(0, 0, 1280, 720);
          //protected float soundVolume = 0.5f;
          //protected float musicVolume = 0.5f;
          protected bool vibration = false;

          #endregion

          #region Fields

          /// <summary>
          /// The controlling player's name--usually their Gamertag.
          /// </summary>
          public string PlayerName
          {
               get { return playerName; }
               set { playerName = value; }
          }

          /// <summary>
          /// The Safe Area, as customized by the user.
          /// </summary>
          public Rectangle SafeArea
          {
               get { return SafeArea; }
               set { safeArea = value; }
          }

          /*
          /// <summary>
          /// The Volume at which Sound is played.
          /// </summary>
          public float SoundVolume
          {
               get { return soundVolume; }
               set { soundVolume = value; }
          }

          /// <summary>
          /// The Volume at which Music is played.
          /// </summary>
          public float MusicVolume
          {
               get { return musicVolume; }
          }
          */

          /// <summary>
          /// Whether or not controller vibration is enabled.
          /// </summary>
          public bool Vibration
          {
               get { return vibration; }
               set { vibration = value; }
          }

          #endregion
     }
}