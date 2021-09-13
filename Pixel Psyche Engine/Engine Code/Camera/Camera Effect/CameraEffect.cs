#region File Description
//-----------------------------------------------------------------------------
// CameraEffect.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
#endregion

namespace PixelEngine.CameraSystem
{
     #region CinematicEvent structure.

     /// <remarks>
     /// Defines a structure to create a Cinematic Event for a Camera.
     /// A Cinematic Event contains a list of Camera Effects to be carried out
     /// in step, thus producing a cinematic look.
     /// </remarks>
     public struct CinematicEvent
     {
          List<CameraEffect> effects;
          bool isLooping;
          int currentEffectIndex;

          public CinematicEvent(List<CameraEffect> effectList, bool loop)
          {
               effects = new List<CameraEffect>();
               effects = effectList;

               isLooping = loop;
               currentEffectIndex = 0;
          }

          /// <summary>
          /// Update the CinematicEvent. Updates the current CameraEffect, 
          /// moving on to the next one or looping the current one as needed.
          /// </summary>
          public void Update(GameTime gameTime)
          {
               // If the CinematicCamera is not performing an effect, no further updating is needed.
               if (effects == null)
                    return;

               if (effects.Count <= 0)
                    return;

               if (effects[currentEffectIndex] == null)
                    return;

               // If the current camera effect is still running, perform its update logic.
               else if (effects[currentEffectIndex].IsRunning)
               {
                    effects[currentEffectIndex].Update(gameTime, Camera.GetInstance());
               }

               // If the current camera effect is now finished, it is no longer the current effect.
               if (effects[currentEffectIndex].IsFinished)
               {
                    // If Destination reached, remove it from list, go to next one.
                    if (effects.Count > 0)
                    {
                         if (isLooping)
                         {
                                   currentEffectIndex = (currentEffectIndex + 1) % effects.Count;
                         }

                         else
                         {
                              effects.RemoveAt(currentEffectIndex);
                         }


                         if (currentEffectIndex < effects.Count)
                              effects[currentEffectIndex].GetType().GetMethod("Reset").Invoke(effects[currentEffectIndex], null);
                    }
               }
          }
     };

     #endregion

     /// <remarks>
     /// Base class which allows one to create their own styles / effects
     /// for the Cinematic Camera.
     /// </remarks>
     public abstract class CameraEffect
     {
          #region Fields

          // Floats to track run-time and timing.
          protected float runTime;
          protected float step;

          // Bools to stack running status.
          protected bool isRunning;
          protected bool isFinished;

          // Helper, internal variables.
          protected float elapsedTime;
          protected float remainingTime;

          #endregion

          #region Properties

          /// <summary>
          /// Amount of milliseconds the TextEffect should take to complete
          /// its custom display process.
          /// </summary>
          public float RunTime
          {
               get { return runTime; }
               set { runTime = value; }
          }

          /// <summary>
          /// Helper timing variable used to internally calculate the amount of
          /// milliseconds that each "step" (whose definition depends on the TextEffect
          /// type) is to last.
          /// </summary>
          protected float Step
          {
               get { return step; }
               set { step = value; }
          }

          /// <summary>
          /// Gets or sets whether the Camera Effect is running.
          /// </summary>
          public bool IsRunning
          {
               get { return isRunning; }
               set { isRunning = value; }
          }

          /// <summary>
          /// Gets or sets whether the Camera Effect is finished running.
          /// </summary>
          public bool IsFinished
          {
               get { return isFinished; }
               set { isFinished = value; }
          }

          #endregion

          #region Initialization

          /// <summary>
          /// TextEffect Constructor.
          /// </summary>
          protected CameraEffect(float runTime)
          {
               this.RunTime = runTime;
          }

          #endregion

          #region Virtual Methods

          /// <summary>
          /// Override this method with the custom TextEffect's 
          /// Initialize method.
          /// </summary>
          public virtual void Initialize() { }

          /// <summary>
          /// Override this method with the custom CameraEffect's Update logic.
          /// </summary>
          /// <param name="gameTime">GameTime from the XNA Game instance.</param>
          public virtual void Update(GameTime gameTime, Camera camera) { }

          /// <summary>
          /// Override this method with the custom CameraEffect's Draw logic.
          /// </summary>
          /// <param name="gameTime">GameTime from the XNA Game instance.</param>
          public virtual void Draw(GameTime gameTime, Camera camera) { }

          public virtual void Reset() { }

          #endregion
     }

     #region Swivel Camera Effect (NOT COMPLETED YET)

     public class SwivelEffect : CameraEffect
     {
          #region Fields

          float elapsedTime;
          float remainingTime;
          float moveDistance;
          Vector3 startingPosition;
          Vector3 destinationPosition;
          Vector3 moveDistanceVector;

          #endregion

          #region Initialization

          public SwivelEffect(float runTime, float distance)
               : base(runTime)
          {
               moveDistance = distance;
               elapsedTime = 0.0f;
               remainingTime = runTime;
               startingPosition = CameraManager.ActiveCamera.LookAt;

               IsRunning = true;
               IsFinished = false;
          }

          public SwivelEffect(float runTime, float degreesX, float degreesY, float degreesZ)
               : base(runTime)
          {
               moveDistanceVector = new Vector3(degreesX, degreesY, degreesZ);
               elapsedTime = 0.0f;
               remainingTime = runTime;
               startingPosition = CameraManager.ActiveCamera.LookAt;

               IsRunning = true;
               IsFinished = false;
          }

          public SwivelEffect(float runTime, Vector3 destination)
               : base(runTime)
          {
               destinationPosition = destination;
               startingPosition = CameraManager.ActiveCamera.LookAt;

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

               //Matrix yawMatrix = Matrix.CreateFromAxisAngle(Camera.ViewMatrix.Up, (Step * MathHelper.ToRadians(moveDistanceVector.Y)));

              // Camera.LookAt = Vector3.Transform(Camera.LookAt, yawMatrix);

               CameraManager.ActiveCamera.LookAt = new Vector3(startingPosition.X,
                                             startingPosition.Y * (float)Math.Sin(MathHelper.ToRadians(Step * moveDistanceVector.Y)),
                                             startingPosition.Z);

               remainingTime -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
          }

          #endregion

          private void RotateX(float degrees)
          {
               // only the Y changes
               CameraManager.ActiveCamera.LookAt = new Vector3(CameraManager.ActiveCamera.LookAt.X,
                                   startingPosition.Y + (float)Math.Cos(MathHelper.ToRadians(Step * moveDistanceVector.X)),
                                   CameraManager.ActiveCamera.LookAt.Z);

               //To apply Yaw (Y rotation) we need to rotate the look and right vectors around the up vector:

               Matrix yawMatrix = Matrix.CreateFromAxisAngle(CameraManager.ActiveCamera.ViewMatrix.Up, degrees);

               CameraManager.ActiveCamera.LookAt = Vector3.Transform(CameraManager.ActiveCamera.LookAt, yawMatrix);
          }

          private void RotateY(float degrees)
          {

          }

          private void RotateZ(float degrees)
          {
          }
     }

     #endregion
}