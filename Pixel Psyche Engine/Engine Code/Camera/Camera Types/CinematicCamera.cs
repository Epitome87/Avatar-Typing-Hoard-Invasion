#region File Description
//-----------------------------------------------------------------------------
// CinematicCamera.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PixelEngine.Screen;
using PixelEngine.Text;
#endregion

namespace PixelEngine.CameraSystem
{
     /// <summary>
     /// Inherits from the base Camera class and provides functionality
     /// specific to a "Cinematic" camera. This Camera is controlled
     /// by a series of destination points and angles, rather than 
     /// by user input. This class also allows for special effect-type
     /// camera actions.
     /// </summary>
     public class CinematicCamera : Camera
     {
          #region Fields

          private static List<Vector3> cameraDestinations = new List<Vector3>();
          private static List<CameraEffect> cameraEffects = new List<CameraEffect>();
          private static CameraEffect currentCameraEffect = null;

          #endregion

          #region Properties

          public static List<CameraEffect> EffectList
          {
               get { return cameraEffects; }
               set { cameraEffects = value; }
          }

          #endregion

          #region CinematicCamera Constructors

          /// <summary>
          /// CinematicCamera Constructor.
          /// </summary>
          public CinematicCamera(Viewport viewPort)
               : base(viewPort)
          {
               currentCameraEffect = null;

               Reset(viewPort);
          }

          /// <summary>
          /// CinematicCamera Constructor requiring a List, which
          /// is used to create a list of Destinations for the camera.
          /// </summary>
          protected CinematicCamera(Viewport viewPort, List<Vector3> list)
               : base(viewPort)
          {
               Reset(viewPort);

               cameraDestinations = list;
          }

          /// <summary>
          /// CinematicCamera Constructor requiring a List, which
          /// is used to create a list of Destinations for the camera.
          /// </summary>
          protected CinematicCamera(Viewport viewPort, List<CameraEffect> effects)
               : base(viewPort)
          {
               Reset(viewPort);

               cameraEffects = effects;
          }

          protected new void Reset(Viewport viewport)
          {
               Position = new Vector3(0f, 1f, 5.0f);

               LookAt = new Vector3(0f, 1f, 0f);
               ViewMatrix = Matrix.CreateLookAt(Position, LookAt, Vector3.Up);
               FieldOfView = 45f;

               NearViewPlane = 0.01f;
               FarViewPlane = 200.0f; // 100

               ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(FieldOfView),
                            AspectRatio, NearViewPlane, FarViewPlane);
          }

          #endregion

          #region Update

          public override void Update(GameTime gameTime)
          {
               // Update the Frustum and ViewMatrix via Camera.Update.
               base.Update(gameTime);

               // If the CinematicCamera is not performing an effect, no further updating is needed.
               if (currentCameraEffect == null)
               {
                    return;
               }

               // If the current camera effect is still running, perform its update logic.
               else if (currentCameraEffect.IsRunning)
               {
                    currentCameraEffect.Update(gameTime, this);
               }

               // If the current camera effort is now finished, it is no longer the current effect.
               if (currentCameraEffect.IsFinished)
               {
                    // If Destination reached, remove it from list, go to next one.
                    if (cameraEffects.Count > 0)
                    {
                         currentCameraEffect = cameraEffects[0];
                         cameraEffects.RemoveAt(0);
                    }

                    else
                    {
                         cameraEffects.Remove(currentCameraEffect);
                         currentCameraEffect = null;
                    }
               }
          }

          #endregion

          #region Camera Actions

          public static void Move(float RunTime, Vector3 Destination)
          {
               if (currentCameraEffect == null)
                    currentCameraEffect = new MoveEffect(RunTime, Destination);
          }

          public static void Point(float RunTime, Vector3 Destination)
          {
               if (currentCameraEffect == null)
                    currentCameraEffect = new PointEffect(RunTime, Destination);
          }

          public static void Rotate(float RunTime, Vector3 Destination)
          {
               if (currentCameraEffect == null)
                    currentCameraEffect = new RotateEffect(RunTime, Destination);
          }

          public static void Swivel(float RunTime, Vector3 Destination)
          {
               if (currentCameraEffect == null)
                    currentCameraEffect = new SwivelEffect(RunTime, Destination);
          }

          public static void Shake()
          {
          }

          public static void Zoom(float RunTime, float Distance, float FovExpansion)
          {
               if (currentCameraEffect == null)
                    currentCameraEffect = new ZoomEffect(RunTime, Distance, FovExpansion);
          }

          public static void PrepareContraZoom(Vector3 TargetPosition, float FrameWidth)
          {
               if (currentCameraEffect == null)
                    currentCameraEffect = new PrepareContraEffect(TargetPosition, FrameWidth);
          }

          public static void ContraZoomFov(float RunTime, Vector3 TargetPosition, float FrameWidth)
          {
               if (currentCameraEffect == null)
                    currentCameraEffect = new ContraZoomFovEffect(RunTime, TargetPosition, FrameWidth);
          }

          public static void ContraZoomPosition(float RunTime, Vector3 TargetPosition, float FrameWidth)
          {
               if (currentCameraEffect == null)
                    currentCameraEffect = new ContraZoomPositionEffect(RunTime, TargetPosition, FrameWidth);
          }

          #endregion

          #region Public Methods

          /// <summary>
          /// Adds a CameraEffect to the CinematicCamera's effect list.
          /// The CameraEffect will be performed once all prior ones in the list are performed.
          /// </summary>
          public static void AddEffect(CameraEffect effect)
          {
               cameraEffects.Add(effect);

               if (currentCameraEffect == null)
                    currentCameraEffect = effect;
          }

          /// <summary>
          /// Adds a destination to the CinematicCamera's destination list.
          /// The CinematicCamera will move to the destination once all prior ones in the list are reached.
          /// </summary>
          public static void AddDestination(Vector3 destination)
          {
               cameraDestinations.Add(destination);
          }

          /// <summary>
          /// 
          /// </summary>
          public static void Reset()
          {
               cameraEffects.Clear();
               cameraDestinations.Clear(); 
          }

          #endregion

          #region Debug

          public override void PrintDebug()
          {
               string s = string.Empty;

               s = base.ToString();

               if (currentCameraEffect == null)
               {
                    s += string.Format("\nCurrent Camera Effect: None\n\n\n");
               }

               else
               {
                    s += string.Format("\nCurrent Camera Effect: {0}\n\n\n",
                         currentCameraEffect.GetType().ToString());
               }

               TextManager.DrawAutoCentered(PixelEngine.Screen.ScreenManager.Font,
                    s, new Vector2(EngineCore.ScreenCenter.X, 150f), Color.White, 0.5f);
          }

          #endregion

          #region Input (TEMPORARY)

          public override void HandleInput(InputState input, PlayerIndex? ControllingPlayer)
          {
               PlayerIndex playerIndex = ControllingPlayer.Value;

               HandleInput(input, ControllingPlayer, playerIndex);
          }

          public override void HandleInput(InputState input, PlayerIndex? ControllingPlayer, PlayerIndex playerIndex)
          {
               if (input == null)
                    throw new ArgumentNullException("input");

               // Look up inputs for the active player profile.
               int index = (int)PixelEngine.EngineCore.ControllingPlayer.Value;

               GamePadState gamePadState = input.CurrentGamePadStates[index];

               base.HandleInput(input, ControllingPlayer, playerIndex);

               if (input.IsNewButtonPress(Buttons.LeftTrigger, PixelEngine.EngineCore.ControllingPlayer, out playerIndex))
               {
                    CinematicCamera.Zoom(5000.0f, 0.0f, 44.0f);
               }

               if (input.IsNewButtonPress(Buttons.RightTrigger, PixelEngine.EngineCore.ControllingPlayer, out playerIndex))
               {
                    CinematicCamera.Zoom(5000.0f, 0.0f, -44.0f);
               }

               if (input.IsNewButtonPress(Buttons.LeftShoulder, PixelEngine.EngineCore.ControllingPlayer, out playerIndex))
               {
                    CinematicCamera.Rotate(5000.0f, new Vector3(0.0f, -45.0f, 0.0f));
               }

               if (input.IsNewButtonPress(Buttons.RightShoulder, PixelEngine.EngineCore.ControllingPlayer, out playerIndex))
               {
                    CinematicCamera.Rotate(5000.0f, new Vector3(0.0f, 45.0f, 0.0f));
               }

               if (input.IsNewButtonPress(Buttons.LeftThumbstickUp, PixelEngine.EngineCore.ControllingPlayer, out playerIndex))
               {
                    CinematicCamera.Move(5000.0f, new Vector3(0.0f, 1.0f, -1.0f));
               }

               if (input.IsNewButtonPress(Buttons.LeftThumbstickDown, PixelEngine.EngineCore.ControllingPlayer, out playerIndex))
               {
                    CinematicCamera.Move(5000.0f, new Vector3(0.0f, 1.0f, 20.0f));
               }

               if (input.IsNewButtonPress(Buttons.X, PixelEngine.EngineCore.ControllingPlayer, out playerIndex))
               {
                    CinematicCamera.ContraZoomPosition(5000f, new Vector3(0f, 1f, -3.0f), 1f);
               }
          }

          #endregion
     }
}