#region File Description
//-----------------------------------------------------------------------------
// CameraManager.cs
// Author: Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Collections.Generic;
using Microsoft.Xna.Framework;
#endregion

namespace PixelEngine.CameraSystem
{
     /// <remarks>
     /// Defines a Manager for a Camera object.
     /// Camera objects can be added and managed internally.
     /// </remarks>
     public class CameraManager : GameComponent
     {
          #region Singleton

          private static CameraManager cameraManager = null;

          #endregion

          #region CameraNumber Enum

          public enum CameraNumber
          {
               Default,
               FirstPerson,
               ThirdPerson,
               Cinematic,
               FreeRoam,
          }

          #endregion

          #region Fields
          // Don't think HashTable exists in XNA 4.0 private static Hashtable cameras = new Hashtable();
          private static Dictionary<CameraNumber, Camera> cameras = new Dictionary<CameraNumber, Camera>();
          private static Camera activeCamera;
          private static bool isInitialized = false;

          #endregion
     
          #region Properties

          /// <summary>
          /// Gets whether the CameraManager is Initialized.
          /// </summary>
          public static bool Initialized
          {
               get { return isInitialized; }
          }
          
          /// <summary>
          /// Gets the Camera where all the action is taking place.
          /// </summary>
          public static Camera ActiveCamera
          {
               get { return activeCamera; }
          }

          #endregion

          #region Initialization

          /// <summary>Create the camera Managers.</summary>
          /// <param name="game">The game the CameraManager component should be attached to.</param>
          private CameraManager(Game game)
               : base(game)
          {
               Enabled = true;
          }

          /// <summary>Create the cameras.</summary>
          /// <param name="game">The game the CameraManager component should be attached to.</param>
          public static void Initialize(Game game)
          {
               cameraManager = new CameraManager(game);

               if (game != null)
               {
                    game.Components.Add(cameraManager);
               }
               
               AddCamera(new FirstPersonCamera(EngineCore.GraphicsDevice.Viewport), CameraNumber.FirstPerson);
               AddCamera(new ThirdPersonCamera(EngineCore.GraphicsDevice.Viewport), CameraNumber.ThirdPerson);
               AddCamera(new FreeRoamCamera(EngineCore.GraphicsDevice.Viewport), CameraNumber.FreeRoam);
               AddCamera(new CinematicCamera(EngineCore.GraphicsDevice.Viewport), CameraNumber.Cinematic);

               SetActiveCamera(CameraNumber.FirstPerson);
          }

          /// <summary>
          /// Override GameComponent.Initialize method.
          /// Simply calls GameComponent.Initialize and sets
          /// boolean IsInitialized to true.
          /// </summary>
          public override void Initialize()
          {
               base.Initialize();

               isInitialized = true;
          }

          #endregion

          #region Update

          /// <summary>
          /// Update the active Camera.
          /// </summary>
          /// <param name="gameTime"></param>
          public override void Update(GameTime gameTime)
          {
               base.Update(gameTime);
               activeCamera.Update(gameTime);
          }

          #endregion

          #region Public Methods

          /// <summary>
          /// Adds a new Camera to the CameraManager.
          /// </summary>
          /// <param name="newCamera">The new Camera to be added.</param>
          /// <param name="cameraLabel">The Enum CameraNumber label to associate with the new Camera.</param>
          public static void AddCamera(Camera newCamera, CameraNumber cameraNumber)
          {
               // Doesn't work in XNA 4.0: (Because I changed from Hashtable [not in 4.0] to Dictionary)
               //if (!cameras.Contains(cameraNumber))

               // Change To: ???
               if (!cameras.ContainsKey(cameraNumber))
               {
                    cameras.Add(cameraNumber, newCamera);
               }
          }

          /// <summary>
          /// Change the projection matrix of all cameras.
          /// </summary>
          /// <param name="aspectRatio">The Aspect Ratio to be used. 45 degrees is typical.</param>
          public static void SetAllCamerasProjectionMatrix(float aspectRatio)
          {
               foreach (Camera camera in cameras.Values)
               {
                    camera.ProjectionMatrix = 
                         Matrix.CreatePerspectiveFieldOfView(
                                            camera.FieldOfView, aspectRatio, camera.NearViewPlane, camera.FarViewPlane);
               }
          }

          /// <summary>
          /// Changes the active Camera by label.
          /// </summary>
          /// <param name="cameraLabel">The Enum CameraNumber of the Camera to be set as active.</param>
          public static void SetActiveCamera(CameraNumber cameraNumber)
          {
               if (cameras.ContainsKey(cameraNumber))
               {
                    activeCamera = cameras[cameraNumber] as Camera;
               }
          }

          #endregion
     }
}
