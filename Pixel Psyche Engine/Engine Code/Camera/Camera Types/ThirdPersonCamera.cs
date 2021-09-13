#region File Description
//-----------------------------------------------------------------------------
// ThirdPersonCamera.cs
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
     /// Inherits from the base Camera class and provides functionality
     /// specific to a Third-Person view camera. 
     /// </remarks>
     public class ThirdPersonCamera : Camera
     {
          #region Fields

          // Set the avatar position and rotation variables.
          static Vector3 avatarPosition = new Vector3(0, 0, 0f);
          static Vector3 avatarHeadOffset = new Vector3(0, 1.25f, 0);

          static float avatarYaw;
          static float avatarPitch;

          // Set the direction the camera points without rotation.
          static Vector3 cameraReference = new Vector3(0, 0, 1f);

          // New Vector(0, n, -n) give a Third-Person angle.
          static Vector3 thirdPersonReference = new Vector3(0, 5, -5);

          // Set rates in world units per 1/60th second 
          // (the default fixed-step interval).
          static float rotationSpeed = 5f / 60f;
          static float forwardSpeed = 2f / 60f;

          #endregion

          public static float AvatarYaw
          {
               get { return avatarYaw; }
               set { avatarYaw = value; }
          }

          public static float AvatarPitch
          {
               get { return avatarPitch; }
               set { avatarPitch = value; }
          }

          public static float RotationSpeed
          {
               get { return rotationSpeed; }
               set { rotationSpeed = value; }
          }

          public static float ForwardSpeed
          {
               get { return forwardSpeed; }
               set { forwardSpeed = value; }
          }

          #region Properties

          #endregion

          #region Initialization

          public ThirdPersonCamera(Viewport viewport)
               : base(viewport)
          {
               Reset(viewport);
          }

          public override void Reset(Viewport viewport)
          {
               base.Reset(viewport);
          }

          #endregion

          #region Update

          /// <summary>
          /// Updates the ThirdPersonCamera based on the target object's movement.
          /// </summary>
          public override void Update(GameTime gameTime)
          {
               UpdateCameraThirdPerson();
               
               // Update the Frustum and ViewMatrix via Camera.Update.
               base.Update(gameTime);
          }

          /// <summary>
          /// Updates the camera when its in the 3rd person state.
          /// </summary>
          private void UpdateCameraThirdPerson()
          {
               if (TrackedObject != null)
               {
                    Matrix rotationMatrix = Matrix.CreateRotationY(avatarYaw);

                    // Create a vector pointing the direction the camera is facing.
                    Vector3 transformedReference =
                        Vector3.Transform(thirdPersonReference, rotationMatrix);
   

                    // Calculate the position the camera is looking from.
                    Position = transformedReference + TrackedObject.TrackedPosition;// avatarPosition;

                    LookAt = TrackedObject.TrackedPosition;// avatarPosition;
               }
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
               if (Track != null)
               {
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

                    trackedObject.TrackedPosition = new Vector3(trackedObject.TrackedPosition.X, trackedObject.TrackedPosition.Y, trackedObject.TrackedPosition.Z + v.Z * leftStick.Y);
                    trackedObject.TrackedPosition = new Vector3(trackedObject.TrackedPosition.X + v.X * leftStick.Y, trackedObject.TrackedPosition.Y, trackedObject.TrackedPosition.Z);


                    Vector3 v2 = new Vector3(forwardSpeed, 0, 0);
                    v2 = Vector3.Transform(v2, sideMovement);

                    trackedObject.TrackedPosition = new Vector3(trackedObject.TrackedPosition.X, trackedObject.TrackedPosition.Y, trackedObject.TrackedPosition.Z - v.Z * leftStick.X);
                    trackedObject.TrackedPosition = new Vector3(trackedObject.TrackedPosition.X - v2.X * leftStick.X, trackedObject.TrackedPosition.Y, trackedObject.TrackedPosition.Z);
               }
                * */
               
               //HandleCameraInput();        
          }

          #endregion

          #region Public Methods

          public override void PrintDebug()
          {
               string s = ToString();

               Text.TextManager.DrawAutoCentered(PixelEngine.Screen.ScreenManager.Font,
                    s, new Vector2(EngineCore.ScreenCenter.X, 200f), Color.White, 0.5f);
          }

          public override string ToString()
          {
               string s = base.ToString();
               string track = string.Empty;
               Vector3 trackedPosition = new Vector3();

               if (Camera.TrackedObject != null)
               {
                    track = Camera.TrackedObject.ToString();
                    trackedPosition = Camera.TrackedObject.TrackedPosition;

                    s += string.Format("\nTracked Object: {0}\nTrackedPosition: {1}",
                         track.ToString(), trackedPosition.ToString());
               }

               else
                    s += string.Format("\nTracked Object: None\nTrackedPosition: None");


               return s;
          }

          #endregion

          #region TEMP TESTING
          
          /// <summary>
          /// Move camera based on user input
          /// </summary>
          public void HandleCameraInput()
          {
               KeyboardState keyboardState = Keyboard.GetState();
               GamePadState currentGamePadState = GamePad.GetState(PlayerIndex.One);

               // should we reset the camera?
               if (currentGamePadState.Buttons.RightStick == ButtonState.Pressed)
               {
                    arc = cameraDefaultArc;
                    distance = cameraDefaultDistance;
                    rotation = cameraDefaultRotation;
               }

               // Update Camera
               arc -= currentGamePadState.ThumbSticks.Right.Y * 0.05f;
               rotation += currentGamePadState.ThumbSticks.Right.X * 0.1f;
               distance += currentGamePadState.Triggers.Left * 0.1f;
               distance -= currentGamePadState.Triggers.Right * 0.1f;

               // Limit the camera movement
               if (distance > 5.0f)
                    distance = 5.0f;
               else if (distance < 2.0f)
                    distance = 2.0f;

               if (arc > MathHelper.Pi / 5)
                    arc = MathHelper.Pi / 5;
               else if (arc < -(MathHelper.Pi / 5))
                    arc = -(MathHelper.Pi / 5);

               // Update the camera position
               Vector3 cameraPos = new Vector3(0, distance, distance);
               position = new Vector3(0, distance, distance);
               position = Vector3.Transform(position, Matrix.CreateRotationX(arc));
               position = Vector3.Transform(position,
                                             Matrix.CreateRotationY(rotation));

               if (trackedObject != null)
               {
                    position += trackedObject.TrackedWorldMatrix.Translation;

                    // Create new view matrix
                    ViewMatrix = Matrix.CreateLookAt(position, trackedObject.TrackedWorldMatrix.Translation + new Vector3(0, 1.2f, 0), Vector3.Up);
               }
          }
          
          #endregion
     }
}