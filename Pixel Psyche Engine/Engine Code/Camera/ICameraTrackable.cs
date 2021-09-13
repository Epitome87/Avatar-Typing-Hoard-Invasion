#region File Description
//-----------------------------------------------------------------------------
// ICameraTrackable.cs
// Author: Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace PixelEngine.CameraSystem
{
     /// <remarks> 
     /// Defines an Interface which objects that wish to be tracked by a camera must inherit from. 
     /// </remarks>
     public interface ICameraTrackable
     {
          /// <summary>
          /// The Vector3 which represents the tracked object's 3D position.
          /// </summary>
          Vector3 TrackedPosition
          {
               get;
               set;
          }

          /// <summary>
          /// The Matrix which represents the tracked object's World position.
          /// </summary>
          Matrix TrackedWorldMatrix
          {
               get;
          }
     }
}