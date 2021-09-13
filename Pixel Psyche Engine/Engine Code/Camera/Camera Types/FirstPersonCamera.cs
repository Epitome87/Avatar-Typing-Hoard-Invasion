#region File Description
//-----------------------------------------------------------------------------
// FirstPersonCamera.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.Screen;
#endregion

namespace PixelEngine.CameraSystem
{
     /// <summary>
     /// Inherits from the base Camera class and provides functionality
     /// specific to a First-Person view camera. 
     /// </summary>
     public class FirstPersonCamera : Camera
     {
          #region Fields

          // Set rates in world units per 1/60th second (the default fixed-step interval).
          float rotationSpeed = 2f / 60f;
          float forwardSpeed = 5f / 60f;

          Vector3 avatarPosition = new Vector3(0f, 0f, -0.175f);//new Vector3(0f, -0.1f, -0.175f);//new Vector3(0f, -0.1f, -0.2f);
          Vector3 avatarHeadOffset = new Vector3(0f, 2f, 0f); // 0 1.25 0
          float avatarYaw;
          float avatarPitch;
          Matrix rotationMatrix; // = Matrix.CreateRotationY(avatarYaw);

          // Transform the head offset so the camera is 
          // positioned properly relative to the avatar.
          public static Vector3 headOffset = new Vector3(0, 1.6f, 0f); //new Vector3(0, 1, 0);// = Vector3.Transform(avatarHeadOffset, rotationMatrix);

          // Set the direction the camera points without rotation.
          Vector3 cameraReference = new Vector3(0, 0, 1);

          // Create a vector pointing the direction the camera is facing.
          Vector3 transformedReference ;//= Vector3.Transform(cameraReference, rotationMatrix);

          #endregion

          #region Properties

          public new Vector3 Position
          {
               get { return position; }
               set
               {
                    if (TrackedObject != null) position = TrackedObject.TrackedPosition;
                    else position = value;
               }
          }

          #endregion

          #region Initialization

          public FirstPersonCamera(Viewport viewPort)
               : base(viewPort)
          {
               Reset(viewPort);
          }

          public override void Reset(Viewport viewport)
          {
               base.Reset(viewport);

               NearViewPlane = 0.01f;
               FarViewPlane = 200.0f; // 100

               ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f),
                            viewport.AspectRatio, NearViewPlane, FarViewPlane);

               if (TrackedObject != null)
               {
                    Position = TrackedObject.TrackedPosition + headOffset;
               }

               else
               {
                    // Calculate the camera's current position.
                    Position = avatarPosition + headOffset;
               }

               // Calculate the position the camera is looking at.
               LookAt = transformedReference + Position;
               ViewMatrix = Matrix.CreateLookAt(Position, LookAt, Vector3.Up);
          }

          #endregion

          #region Update

          /// <summary>
          /// Overridden Update method. 
          /// </summary>
          public override void Update(GameTime gameTime)
          {
               UpdateCameraFirstPerson();

               // Update the Frustum and ViewMatrix via Camera.Update.
               base.Update(gameTime);
          }

          /// <summary>
          /// Updates the camera when its in the 1st person state.
          /// </summary>
          private void UpdateCameraFirstPerson()
          {
               if (TrackedObject != null)
               {
                    // Calculate the position the camera is looking from.
                    Position = TrackedObject.TrackedPosition + avatarHeadOffset;// avatarPosition;
               }

               else
               {
                    // Calculate the camera's current position.
                    //Position = avatarPosition + avatarHeadOffset; // This was commented out before 2-26-2011
               }

               Matrix rotationMatrix = Matrix.CreateRotationY(avatarYaw);

               Matrix rotationXMatrix = Matrix.CreateRotationX(avatarPitch);
               
               rotationMatrix = rotationXMatrix * rotationMatrix;

               // Create a vector pointing the direction the camera is facing.
               Vector3 transformedReference = Vector3.Transform(cameraReference, rotationMatrix);

               // Calculate the position the camera is looking at.
               LookAt = Position + transformedReference;
          }

          #endregion
          
          #region Input

          public override void HandleInput(InputState input, PlayerIndex? ControllingPlayer)
          {
               PlayerIndex playerIndex = ControllingPlayer.Value;

               HandleInput(input, ControllingPlayer, playerIndex);
          }

          public override void HandleInput(InputState input, PlayerIndex? ControllingPlayer, PlayerIndex playerIndex)
          {
               base.HandleInput(input, ControllingPlayer, playerIndex);

               /*
               Vector2 rightStick = input.CurrentGamePadStates[(int)ControllingPlayer.Value].ThumbSticks.Right;
               avatarYaw -= rotationSpeed * rightStick.X;

               rightStick = input.CurrentGamePadStates[(int)ControllingPlayer.Value].ThumbSticks.Right;
               avatarPitch -= rotationSpeed * rightStick.Y;
               avatarPitch = MathHelper.Clamp(avatarPitch, MathHelper.ToRadians(-45f), MathHelper.ToRadians(45f));

               Vector2 leftStick = input.CurrentGamePadStates[(int)ControllingPlayer.Value].ThumbSticks.Left;
               Matrix forwardMovement = Matrix.CreateRotationY(avatarYaw);
               Matrix sideMovement = Matrix.CreateRotationY(avatarYaw);
               Vector3 v = new Vector3(0, 0, forwardSpeed);
               v = Vector3.Transform(v, forwardMovement);

               avatarPosition.Z += v.Z * leftStick.Y;
               avatarPosition.X += v.X * leftStick.Y;


               Vector3 v2 = new Vector3(forwardSpeed, 0, 0);
               v2 = Vector3.Transform(v2, sideMovement);

               avatarPosition.Z -= v2.Z * leftStick.X;
               avatarPosition.X -= v2.X * leftStick.X;     
               */
          }

          #endregion

          #region Debug

          public override void PrintDebug()
          {
               string s = base.ToString();
               
               Text.TextManager.DrawAutoCentered(PixelEngine.Screen.ScreenManager.Font,
                    s, new Vector2(EngineCore.ScreenCenter.X, 150f), Color.White, 0.5f);
          }

          #endregion
     }
}