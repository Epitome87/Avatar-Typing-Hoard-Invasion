#region File Description
//-----------------------------------------------------------------------------
// GraphicsHelper.cs
// Matt McGrath.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using PixelEngine.CameraSystem;
#endregion

namespace PixelEngine.Graphics
{
     /// <summary>
     /// A static helper class which aids in commonly used Graphics-related tasks.
     /// </summary>
     public static class GraphicsHelper
     {
          #region World-To-Screenspace conversions.

          /// <summary>
          /// Convert a World Position Matrix into 2D Screenspace.
          /// </summary>
          public static Vector2 ConvertToScreenspace(Matrix worldPosition)
          {
               // Calculate screenspace of space position.
               Vector3 screenSpace3D = EngineCore.Game.GraphicsDevice.Viewport.Project(Vector3.Zero,
                                                                         CameraManager.ActiveCamera.ProjectionMatrix,
                                                                         CameraManager.ActiveCamera.ViewMatrix,
                                                                         worldPosition);


               Vector2 screenSpace2D = new Vector2();

               // Get 2D position from screenspace vector.
               screenSpace2D.X = screenSpace3D.X;
               screenSpace2D.Y = screenSpace3D.Y;

               return screenSpace2D;
          }

          /// <summary>
          /// Converts a World Position Matrix into a 3D Screenspace.
          /// Just ignore the Vector3.Z result if needed.
          /// </summary>
          public static Vector3 ToScreenspaceVector3(Matrix worldPosition)
          {
               // Calculate screenspace of space position.
               Vector3 screenSpace3D = EngineCore.Game.GraphicsDevice.Viewport.Project(Vector3.Zero,
                                                                         CameraManager.ActiveCamera.ProjectionMatrix,
                                                                         CameraManager.ActiveCamera.ViewMatrix,
                                                                         worldPosition);

               return screenSpace3D;
          }

          #endregion
     }
}