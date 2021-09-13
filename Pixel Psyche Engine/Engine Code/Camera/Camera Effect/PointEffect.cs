#region File Description
//-----------------------------------------------------------------------------
// PointEffect.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace PixelEngine.CameraSystem
{
     public class PointEffect : CameraEffect
     {
          #region Fields

          float pointDifference;
          Vector3 startingLookAt;
          Vector3 destinationLookAt;
          Vector3 pointDifferenceVector;

          #endregion

          #region Initialization

          public PointEffect(float runTime, float distance)
               : base(runTime)
          {
               pointDifference = distance;
               elapsedTime = 0.0f;
               remainingTime = runTime;
               startingLookAt = CameraManager.ActiveCamera.LookAt;

               IsRunning = true;
               IsFinished = false;
          }

          public PointEffect(float runTime, float distanceX, float distanceY, float distanceZ)
               : base(runTime)
          {
               pointDifferenceVector = new Vector3(distanceX, distanceY, distanceZ);
               elapsedTime = 0.0f;
               remainingTime = runTime;
               startingLookAt = CameraManager.ActiveCamera.LookAt;

               IsRunning = true;
               IsFinished = false;
          }

          public PointEffect(float runTime, Vector3 destination)
               : base(runTime)
          {
               destinationLookAt = destination;
               startingLookAt = CameraManager.ActiveCamera.LookAt;

               pointDifferenceVector = startingLookAt - destination;
               elapsedTime = 0.0f;
               remainingTime = runTime;

               IsRunning = true;
               IsFinished = false;
          }

          public override void Reset()
          {
               startingLookAt = CameraManager.ActiveCamera.LookAt;
               pointDifferenceVector = startingLookAt - destinationLookAt;

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

               CameraManager.ActiveCamera.LookAt = new Vector3(startingLookAt.X - (Step * pointDifferenceVector.X),
                                   startingLookAt.Y - (Step * pointDifferenceVector.Y),
                                   startingLookAt.Z - (Step * pointDifferenceVector.Z));

               remainingTime = runTime - elapsedTime;
          }

          #endregion
     }
}