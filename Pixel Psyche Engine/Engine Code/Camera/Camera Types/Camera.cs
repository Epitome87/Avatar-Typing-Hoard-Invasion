#region File Description
//-----------------------------------------------------------------------------
// Camera.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PixelEngine.Screen;
#endregion

namespace PixelEngine.CameraSystem
{
     /// <remarks>
     /// Defines a base Camera class. 
     /// </remarks>
     public class Camera : ICameraTrackable
     {
          #region Singleton

          /// <summary>
          /// The singleton for this type.
          /// </summary>
          protected static Camera camera = null;

          #endregion

          #region Return Singleton Instance

          public static Camera GetInstance()
          {
               return camera;
          }

          #endregion

          #region Fields

          // Fields that describe basic camera characteristics.
          protected Vector3 position;
          private Vector3 lookAt;
          private Matrix viewMatrix;
          private Matrix projectionMatrix;
          private float aspectRatio;
          private float fieldOfView;
          private float nearViewPlane;
          private float farViewPlane;

          // An ICameraTrackable object which may be tied with the camera movement.
          protected static ICameraTrackable trackedObject;

          // The following constants control the camera's default position
          protected const float cameraDefaultArc = MathHelper.Pi / 10;
          protected const float cameraDefaultRotation = MathHelper.Pi;
          protected const float cameraDefaultDistance = 2.5f;

          // Camera values
          protected float arc = cameraDefaultArc;
          protected float rotation = cameraDefaultRotation;
          protected float distance = cameraDefaultDistance;

          public BoundingFrustum Frustum;// = new BoundingFrustum(ViewMatrix * ProjectionMatrix);

          #endregion

          #region Properties for Camera state

          /// <summary>
          /// Gets or sets the 3D Position of the Camera.
          /// </summary>
          public Vector3 Position
          {
               get { return position; }
               set { position = value; }
          }

          /// <summary>
          /// Gets or sets the LookAt of the Camera.
          /// </summary>
          public Vector3 LookAt
          {
               get { return lookAt; }
               set { lookAt = value; }
          }

          /// <summary>
          /// Gets or sets the ViewMatrix of the Camera.
          /// </summary>
          public Matrix ViewMatrix
          {
               get { return viewMatrix; }
               //protected 
               set { viewMatrix = value; }
          }

          /// <summary>
          /// Gets or sets the ProjectionMatrix of the Camera.
          /// </summary>
          public Matrix ProjectionMatrix
          {
               get { return projectionMatrix; }
               //protected set { projectionMatrix = value; }
               set { projectionMatrix = value; }
          }

          /// <summary>
          /// Gets or sets the AspectRatio of the Camera.
          /// </summary>
          public float AspectRatio
          {
               get { return aspectRatio; }
               set { aspectRatio = value; }
          }

          /// <summary>
          /// Gets or sets the Field of View of the Camera.
          /// </summary>
          public float FieldOfView
          {
               get { return fieldOfView; }
               set { fieldOfView = MathHelper.Clamp(value, 0.5f, 120f); }
          }

          /// <summary>
          /// Gets or sets the Near View Plane of the Camera.
          /// </summary>
          public float NearViewPlane
          {
               get { return nearViewPlane; }
               set { nearViewPlane = value; }
          }

          /// <summary>
          /// Gets or sets the Far View Plane of the Camera.
          /// </summary>
          public float FarViewPlane
          {
               get { return farViewPlane; }
               set { farViewPlane = value; }
          }
          
          #endregion

          #region Properties for Camera Tracking / Movement

          /// <summary>
          /// Gets the Default Arc of the Camera.
          /// </summary>
          public float DefaultArc
          {
               get { return cameraDefaultArc; }
          }

          /// <summary>
          /// Gets the Default Rotation of the Camera.
          /// </summary>
          public float DefaultRotation
          {
               get { return cameraDefaultRotation; }
          }

          /// <summary>
          /// Gets the Default Distance of the Camera.
          /// </summary>
          public float DefaultDistance
          {
               get { return cameraDefaultDistance; }
          }

          /// <summary>
          /// Gets or sets the Arc of the Camera.
          /// </summary>
          public float Arc
          {
               get { return arc; }
               set { arc = value; }
          }

          /// <summary>
          /// Gets or sets the Rotation of the Camera.
          /// </summary>
          public float Rotation
          {
               get { return rotation; }
               set { rotation = value; }
          }

          /// <summary>
          /// Gets or sets the Distance of the camera.
          /// </summary>
          public float Distance
          {
               get { return distance; }
               set { distance = value; }
          }

          #endregion

          #region ICameraTrackable Properties

          /// <summary>
          /// Gets or sets the ICameraTrackable object the Camera will track.
          /// </summary>
          public static ICameraTrackable TrackedObject
          {
               set { trackedObject = value; }
               get { return trackedObject; }
          }

          /// <summary>
          /// Gets or sets the 3D Position the Camera will track.
          /// </summary>
          public Vector3 TrackedPosition
          {
               get { return new Vector3(); }
               set { trackedObject.TrackedPosition = value; }
          }

          /// <summary>
          /// Gets or sets the World Position the Camera will track.
          /// </summary>
          public Matrix TrackedWorldMatrix
          {
               get { return new Matrix(); }
          }

          #endregion

          #region Initialization

          /// <summary>
          /// Creates a Camera with the specified ViewPort.
          /// </summary>
          /// <param name="viewport">
          /// The Viewport to work with. (Used to retrieve screen's Aspect Ratio
          /// </param>
          protected Camera(Viewport viewport)
          {
               Reset(viewport);
          }

          /// <summary>Resets the Camera to its default state. </summary>
          /// <param name="viewport">
          /// The Viewport to work with. (Used to retrieve screen's Aspect Ratio
          /// </param>
          public virtual void Reset(Viewport viewport)
          {
               Frustum = new BoundingFrustum(ViewMatrix * ProjectionMatrix);

               aspectRatio = viewport.AspectRatio;
               fieldOfView = 45f;

               nearViewPlane = 0.01f;
               farViewPlane = 200.0f; // 100

               projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fieldOfView),
                                           aspectRatio, nearViewPlane, farViewPlane);//0.01f, 100.0f);

               Position = new Vector3(0f, 2f, 0f);
               LookAt = new Vector3(0f, 0f, 0f);
               ViewMatrix = Matrix.CreateLookAt(Position, LookAt, Vector3.Up);

               TrackedObject = null;
          }

          #endregion

          #region Update

          /// <summary>
          /// Simply updates the Camera's ViewMatrix via CreateLookAt(Position, LookAt, Vector3.Up).
          /// </summary>
          public virtual void Update(GameTime gameTime)
          {
               // Update the Frustum.
               Frustum.Matrix = viewMatrix * projectionMatrix;

               // If the Camera is tracking an object, the object's position should be used.
               if (trackedObject != null)
               {
                    viewMatrix = Matrix.CreateLookAt(Position, LookAt, Vector3.Up);
                    //viewMatrix = Matrix.CreateLookAt(trackedObject.TrackedPosition, LookAt, Vector3.Up);
               }

               // Otherwise, update the ViewMatrix with the camera's position.
               else
               {
                    viewMatrix = Matrix.CreateLookAt(Position, LookAt, Vector3.Up);
               }
          }

          #endregion

          #region Handle Input

          /// <summary>
          /// Overridable base input method. Allows swapping Active Camera via D-Pad.
          /// </summary>
          public virtual void HandleInput(InputState input, PlayerIndex? ControllingPlayer)
          {
               PlayerIndex playerIndex = ControllingPlayer.Value;

               HandleInput(input, ControllingPlayer, playerIndex);
          }

          /// <summary>
          /// Overridable base input method. Allows swapping Active Camera via D-Pad.
          /// </summary>
          public virtual void HandleInput(InputState input, PlayerIndex? ControllingPlayer, PlayerIndex playerIndex)
          {
               if (input.IsNewButtonPress(Buttons.DPadUp, PixelEngine.EngineCore.ControllingPlayer, out playerIndex))
               {
                    CameraManager.SetActiveCamera(CameraManager.CameraNumber.FirstPerson);
                    CameraManager.ActiveCamera.Reset(EngineCore.GraphicsDevice.Viewport);
               }

               if (input.IsNewButtonPress(Buttons.DPadDown, PixelEngine.EngineCore.ControllingPlayer, out playerIndex))
                    CameraManager.SetActiveCamera(CameraManager.CameraNumber.ThirdPerson);

               
               //if (input.IsNewButtonPress(Buttons.DPadLeft, PixelEngine.EngineCore.ControllingPlayer, out playerIndex))
               //     CameraManager.SetActiveCamera(CameraManager.CameraNumber.Cinematic);

               //if (input.IsNewButtonPress(Buttons.DPadRight, PixelEngine.EngineCore.ControllingPlayer, out playerIndex))
               //     CameraManager.SetActiveCamera(CameraManager.CameraNumber.FreeRoam);
               
               //if (input.IsNewButtonPress(Buttons.RightTrigger, PixelEngine.EngineCore.ControllingPlayer, out playerIndex))
                   // PixelEngine.SceneGraphs.SceneGraphManager.FitCameraToScene();
          }

          #endregion

          #region Public Methods

          /// <summary>
          /// Prints the Camera's information to the screen.
          /// </summary>
          public virtual void PrintDebug()
          {
               string s = ToString();

               Text.TextManager.DrawAutoCentered(PixelEngine.Screen.ScreenManager.Font,
                    s, new Vector2(EngineCore.ScreenCenter.X, 150f), Color.White, 0.5f);
          }

          #endregion

          #region ToString Method - Overriden

          /// <summary>
          /// Overridden ToString. Strings together the Camera's information.
          /// </summary>
          public override string ToString()
          {
               string s = string.Format("Current Camera Type: {0}\n\nCamera Position: {1}\nCamera Field of View: {2}\nCamera Look-At: {3}",
                    CameraManager.ActiveCamera.GetType().ToString(), Position.ToString(), FieldOfView.ToString(), LookAt.ToString());

               return s;
          }

          #endregion
     }
}