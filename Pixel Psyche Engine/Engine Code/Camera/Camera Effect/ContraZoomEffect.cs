#region File Description
//-----------------------------------------------------------------------------
// ContraZoomEffect.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
#endregion

namespace PixelEngine.CameraSystem
{
     #region Contra-Zoom Camera Effect

     public class ContraZoomEffect : CameraEffect
     {
          #region Fields

          float startingDistanceFromTarget;
          float startingPosition;
          //float startingFov;
          float distanceMoved;

          #endregion

          #region Initialization

          public ContraZoomEffect(float runTime, float distance)
               : base(runTime)
          {
               startingDistanceFromTarget = distance;
               distanceMoved = 0.0f;
               elapsedTime = 0.0f;
               remainingTime = runTime;
               startingPosition = CameraManager.ActiveCamera.Position.Z;
               //startingFov = 45f;// Camera.FieldOfView;

               IsRunning = true;
               IsFinished = false;
          }

          #endregion

          #region Update

          public override void Update(GameTime gameTime, Camera camera)
          {
               if (remainingTime < 0.0f)
                    IsFinished = true;

               elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

               float Step = (runTime == 0 ? 1 :
                      MathHelper.Clamp((float)((runTime -
                          remainingTime) / runTime), 0, 1));

               float cameraDistance = startingDistanceFromTarget + (CameraManager.ActiveCamera.Position.Z - startingPosition);

               CameraManager.ActiveCamera.Position = new Vector3(CameraManager.ActiveCamera.Position.X, CameraManager.ActiveCamera.Position.Y, startingPosition - (Step * startingDistanceFromTarget));
               distanceMoved = Math.Abs(startingPosition - CameraManager.ActiveCamera.Position.Z);

               float width = 8.28427f;

               CameraManager.ActiveCamera.FieldOfView = MathHelper.ToDegrees((float)Math.Atan((width / (2 * startingDistanceFromTarget - distanceMoved))) / 2f);

               CameraManager.ActiveCamera.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(CameraManager.ActiveCamera.FieldOfView),
                            EngineCore.GraphicsDeviceManager.GraphicsDevice.Viewport.AspectRatio, 1.0f, 10000.0f);


               remainingTime -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
          }

          #endregion
     }

     #endregion

     #region Contra-Zoom Camera Effect

     public class ContraZoomConstantEffect : CameraEffect
     {
          #region Fields

          float startingDistanceFromTarget;
          Vector3 targetPosition;
          float startingPosition;
          float startingFov;
          float distanceMoved;
          float width;

          #endregion

          #region Initialization

          public ContraZoomConstantEffect(float runTime, Vector3 target)
               : base(runTime)
          {

               distanceMoved = 0.0f;
               elapsedTime = 0.0f;
               remainingTime = runTime;
               startingPosition = CameraManager.ActiveCamera.Position.Z;
               startingFov = CameraManager.ActiveCamera.FieldOfView;
               targetPosition = target;
               startingDistanceFromTarget = 
                    targetPosition.Z - CameraManager.ActiveCamera.Position.Z;

               width = startingDistanceFromTarget * (2f * (float)Math.Tan(MathHelper.ToRadians(startingFov / 2f)));
               IsRunning = true;
               IsFinished = false;
          }

          #endregion

          #region Update

          public override void Update(GameTime gameTime, Camera camera)
          {
               if (remainingTime < 0.0f)
                    IsFinished = true;

               elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

               float Step = (runTime == 0 ? 1 :
                      MathHelper.Clamp((float)((runTime -
                          remainingTime) / runTime), 0, 1));

               CameraManager.ActiveCamera.FieldOfView = startingFov + (Step * 45.0f);
               CameraManager.ActiveCamera.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(CameraManager.ActiveCamera.FieldOfView),
                            EngineCore.GraphicsDeviceManager.GraphicsDevice.Viewport.AspectRatio, 1.0f, 10000.0f);

               float cameraDistance = width / (2f * (float)Math.Tan(MathHelper.ToRadians(CameraManager.ActiveCamera.FieldOfView / 2f)));

               //distanceMoved = Math.Abs(startingPosition - Camera.Position.Z);       
               distanceMoved = Math.Abs(startingDistanceFromTarget - cameraDistance);

               CameraManager.ActiveCamera.Position = new Vector3(CameraManager.ActiveCamera.Position.X, CameraManager.ActiveCamera.Position.Y, startingPosition - distanceMoved);

               remainingTime -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
          }

          #endregion
     }

     #endregion

     #region Preparation for Contra-Zoom Effects

     public class PrepareContraEffect : CameraEffect
     {
          #region Fields

          float startingDistanceFromTarget;
          Vector3 targetPosition;
          float startingPosition;
          float startingFov;

          #endregion

          float frameWidth;

          #region Initialization

          public PrepareContraEffect(Vector3 target, float targetWidth)
               : base(0f)
          {
               frameWidth = targetWidth;

               startingPosition = CameraManager.ActiveCamera.Position.Z;
               startingFov = CameraManager.ActiveCamera.FieldOfView;
               targetPosition = target;

               CameraManager.ActiveCamera.FieldOfView = startingFov;

               startingDistanceFromTarget = frameWidth / (2f * (float)Math.Tan(MathHelper.ToRadians(startingFov / 2f)));
               CameraManager.ActiveCamera.Position = new Vector3(CameraManager.ActiveCamera.Position.X, CameraManager.ActiveCamera.Position.Y, (target.Z + startingDistanceFromTarget));

               IsRunning = false;
               IsFinished = true;
          }

          #endregion
     }

     #endregion

     #region Contra-Zoom With Non-Accelerating Field of View

     public class ContraZoomFovEffect : CameraEffect
     {
          #region Fields

          float startingDistanceFromTarget;
          Vector3 targetPosition;
          float startingPosition;
          float startingFov;
          float distanceMoved;

          #endregion

          float frameWidth;

          #region Initialization

          public ContraZoomFovEffect(float runTime, Vector3 target, float targetWidth)
               : base(runTime)
          {
               frameWidth = targetWidth;
               distanceMoved = 0.0f;
               elapsedTime = 0.0f;
               remainingTime = runTime;
               startingPosition = CameraManager.ActiveCamera.Position.Z;
               startingFov = CameraManager.ActiveCamera.FieldOfView;
               targetPosition = target;

               startingDistanceFromTarget = frameWidth / (2f * (float)Math.Tan(MathHelper.ToRadians(startingFov / 2f)));

               IsRunning = true;
               IsFinished = false;
          }

          #endregion

          #region Update

          public override void Update(GameTime gameTime, Camera camera)
          {
               if (remainingTime < 0.0f)
                    IsFinished = true;

               elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

               float Step = (runTime == 0 ? 1 :
                      MathHelper.Clamp((float)((runTime -
                          remainingTime) / runTime), 0, 1));

               CameraManager.ActiveCamera.FieldOfView = startingFov + (Step * 44.5f);
               CameraManager.ActiveCamera.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(CameraManager.ActiveCamera.FieldOfView),
                            EngineCore.GraphicsDeviceManager.GraphicsDevice.Viewport.AspectRatio, 1.0f, 10000.0f);

               float cameraDistance = frameWidth / (2f * (float)Math.Tan(MathHelper.ToRadians(CameraManager.ActiveCamera.FieldOfView / 2f)));

               //distanceMoved = Math.Abs(startingPosition - Camera.Position.Z);       
               distanceMoved = Math.Abs(startingDistanceFromTarget - cameraDistance);

               CameraManager.ActiveCamera.Position = new Vector3(CameraManager.ActiveCamera.Position.X, CameraManager.ActiveCamera.Position.Y, startingPosition - distanceMoved);

               remainingTime -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
          }

          #endregion
     }

     #endregion

     #region Contra-Zoom With Non-Accelerating Position

     public class ContraZoomPositionEffect : CameraEffect
     {
          #region Fields

          float startingDistanceFromTarget;
          Vector3 targetPosition;
          float startingPosition;
          float startingFov;
          //float distanceMoved;

          #endregion

          #region Properties

          public float StartingFov
          {
               get { return startingFov; }
               set { startingFov = value; }
          }

          #endregion

          float frameWidth;

          #region Initialization

          public ContraZoomPositionEffect(float runTime, Vector3 target, float targetWidth)
               : base(runTime)
          {
               frameWidth = targetWidth;
               //distanceMoved = 0.0f;
               elapsedTime = 0.0f;
               remainingTime = runTime;
               startingPosition = CameraManager.ActiveCamera.Position.Z;
               startingFov = CameraManager.ActiveCamera.FieldOfView;
               targetPosition = target;

               startingDistanceFromTarget = frameWidth / (2f * (float)Math.Tan(MathHelper.ToRadians(startingFov / 2f)));

               IsRunning = true;
               IsFinished = false;
          }

          #endregion

          #region Update

          public override void Update(GameTime gameTime, Camera camera)
          {
               if (remainingTime < 0.0f)
                    IsFinished = true;

               elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

               float Step = (runTime == 0 ? 1 :
                      MathHelper.Clamp((float)((runTime -
                          remainingTime) / runTime), 0, 1));


               CameraManager.ActiveCamera.Position = new Vector3(CameraManager.ActiveCamera.Position.X, CameraManager.ActiveCamera.Position.Y, startingPosition - (Step * startingDistanceFromTarget));

               float cameraDistance = startingDistanceFromTarget - Math.Abs((startingPosition - CameraManager.ActiveCamera.Position.Z));
             

               CameraManager.ActiveCamera.FieldOfView = MathHelper.Clamp(2f * MathHelper.ToDegrees((float)Math.Atan((frameWidth / (CameraManager.ActiveCamera.Position.Z - targetPosition.Z)) / 2f)),
                    0.5f, 45f);

               CameraManager.ActiveCamera.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(CameraManager.ActiveCamera.FieldOfView),
                            EngineCore.GraphicsDeviceManager.GraphicsDevice.Viewport.AspectRatio, 1.0f, 10000.0f);


               remainingTime -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
          }

          #endregion
     }

     #endregion
}