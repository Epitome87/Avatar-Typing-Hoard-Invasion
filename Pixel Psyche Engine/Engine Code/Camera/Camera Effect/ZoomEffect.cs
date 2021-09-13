#region File Description
//-----------------------------------------------------------------------------
// ZoomEffect.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace PixelEngine.CameraSystem
{
     /// <remarks>
     /// A CameraEffect which causes the Camera to zoom in or out.
     /// </remarks>
     public class ZoomEffect : CameraEffect
     {
          #region Fields

          float zoomDistance;
          float startingFov;
          float fovExpansion;

          #endregion

          #region Initialization

          public ZoomEffect(float runTime, float distance, float fov)
               : base(runTime)
          {
               zoomDistance = distance;
               elapsedTime = 0.0f;
               remainingTime = runTime;
               startingFov = CameraManager.ActiveCamera.FieldOfView;
               fovExpansion = fov;

               IsRunning = true;
               IsFinished = false;
          }

          public override void Reset()
          {
               startingFov = CameraManager.ActiveCamera.FieldOfView;

               elapsedTime = 0.0f;
               remainingTime = runTime;

               IsRunning = true;
               IsFinished = false;
          }

          #endregion

          #region Update

          public override void Update(GameTime gameTime, Camera camera)
          {
               if (remainingTime < 0.0f)
               {
                    IsFinished = true;
                    IsRunning = false;
               }

               elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

               float Step = (runTime == 0 ? 1 :
                      MathHelper.Clamp((float)((runTime -
                          remainingTime) / runTime), 0, 1));

               CameraManager.ActiveCamera.FieldOfView = startingFov - Step * fovExpansion;
               CameraManager.ActiveCamera.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(CameraManager.ActiveCamera.FieldOfView),
                    EngineCore.GraphicsDeviceManager.GraphicsDevice.Viewport.AspectRatio, 1.0f, 10000.0f);

               remainingTime = runTime - elapsedTime;
          }

          #endregion
     }
}