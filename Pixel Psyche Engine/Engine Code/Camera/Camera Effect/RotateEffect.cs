#region File Description
//-----------------------------------------------------------------------------
// RotateEffect.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace PixelEngine.CameraSystem
{
     public class RotateEffect : CameraEffect
     {
          #region Fields

          float moveDistance;
          Vector3 startingPosition;
          Vector3 destinationPosition;
          Vector3 moveDistanceVector;

          #endregion

          #region Initialization

          public RotateEffect(float runTime, float distance)
               : base(runTime)
          {
               moveDistance = distance;
               elapsedTime = 0.0f;
               remainingTime = runTime;
               startingPosition = CameraManager.ActiveCamera.Position;

               IsRunning = true;
               IsFinished = false;
          }

          public RotateEffect(float runTime, float degreesX, float degreesY, float degreesZ)
               : base(runTime)
          {
               moveDistanceVector = new Vector3(degreesX, degreesY, degreesZ);
               elapsedTime = 0.0f;
               remainingTime = runTime;
               startingPosition = CameraManager.ActiveCamera.Position;

               IsRunning = true;
               IsFinished = false;
          }

          public RotateEffect(float runTime, Vector3 destination)
               : base(runTime)
          {
               destinationPosition = destination;
               startingPosition = CameraManager.ActiveCamera.Position;

               moveDistanceVector = destination;
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
                    IsFinished = true;

               elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

               float Step = (runTime == 0 ? 1 :
                      MathHelper.Clamp((float)((runTime -
                          remainingTime) / runTime), 0, 1));



               Matrix rotationMatrix = Matrix.CreateRotationX(MathHelper.ToRadians(Step * moveDistanceVector.X)) *
                                        Matrix.CreateRotationY(MathHelper.ToRadians(Step * moveDistanceVector.Y)) *
                                        Matrix.CreateRotationZ(MathHelper.ToRadians(Step * moveDistanceVector.Z));

               CameraManager.ActiveCamera.Position = Vector3.Transform(startingPosition, rotationMatrix);

               remainingTime -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
          }

          #endregion
     }
}