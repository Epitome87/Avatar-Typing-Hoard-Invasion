#region File Description
//-----------------------------------------------------------------------------
// FreeRoamCamera.cs
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
     /// <summary>
     /// Inherits from the base Camera class and provides functionality
     /// specific to a Free-Roaming camera. 
     /// </summary>
     public class FreeRoamCamera : Camera
     {
          #region Fields

          private const float MAX_CAMERA_HEIGHT = 500f;
          private const float MIN_CAMERA_HEIGHT = 1f;

          private const float MAX_CAMERA_LOOKAT_X = 500f;
          private const float MAX_CAMERA_LOOKAT_Y = 500f;
          private const float MAX_CAMERA_LOOKAT_Z = 500f;

          #endregion

          #region Properties

          #endregion

          #region Initialization

          public FreeRoamCamera(Viewport viewport)
               : base(viewport)
          {
               Reset(viewport);
          }

          protected new void Reset(Viewport viewport)
          {
               AspectRatio = viewport.AspectRatio;
               FieldOfView = 45f;

               NearViewPlane = 0.01f;
               FarViewPlane = 200.0f; // 100

               ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(FieldOfView),
                                           AspectRatio, NearViewPlane, FarViewPlane);

               Position = new Vector3(0f, 10f, 0f);
               LookAt = new Vector3(0.01f, 0f, 0f);
               ViewMatrix = Matrix.CreateLookAt(Position, LookAt, Vector3.Up);
          }

          #endregion

          #region Update

          public override void Update(GameTime gameTime)
          {
               // Update the Frustum and ViewMatrix via Camera.Update.
               base.Update(gameTime);
          }

          #endregion

          #region Handle Input

          public override void HandleInput(InputState input, PlayerIndex? ControllingPlayer)
          {
               PlayerIndex playerIndex = ControllingPlayer.Value;

               HandleInput(input, ControllingPlayer, playerIndex);
          }

          public override void HandleInput(InputState input, PlayerIndex? ControllingPlayer, PlayerIndex playerIndex)
          {
               base.HandleInput(input, ControllingPlayer, playerIndex);

               // Control camera aiming along the X-axis.
               Vector2 rightStick = input.CurrentGamePadStates[(int)ControllingPlayer.Value].ThumbSticks.Right;
               LookAt = new Vector3(MathHelper.Clamp(LookAt.X + rightStick.X / 10, -10, MAX_CAMERA_LOOKAT_X),
                                             LookAt.Y, LookAt.Z);

               // Control camera aiming along the Y-axis.
               rightStick = input.CurrentGamePadStates[(int)ControllingPlayer.Value].ThumbSticks.Right;
               LookAt = new Vector3(LookAt.X,
                    MathHelper.Clamp(LookAt.Y + rightStick.Y / 10, -10, MAX_CAMERA_LOOKAT_Y), LookAt.Z);


               // Control camera aiming along the Z-axis.
               if (input.IsNewButtonPress(Buttons.Y, PixelEngine.EngineCore.ControllingPlayer, out playerIndex))
               {
                    LookAt = new Vector3(LookAt.X, LookAt.Y,
                         MathHelper.Clamp(LookAt.Z - 1, -10, 10));
               } 
               // Control camera aiming along the Z-axis.
               if (input.IsNewButtonPress(Buttons.A, PixelEngine.EngineCore.ControllingPlayer, out playerIndex))
               {
                    LookAt = new Vector3(LookAt.X, LookAt.Y, MathHelper.Clamp(LookAt.Z + 1, -10, MAX_CAMERA_LOOKAT_Z));                   
               } 


               // Control camera movement along the X-axis.
               Vector2 leftStick = input.CurrentGamePadStates[(int)ControllingPlayer.Value].ThumbSticks.Left;
               Position = new Vector3(MathHelper.Clamp(Position.X + leftStick.X / 10, -10, 10),
                                             Position.Y, Position.Z);

               // Control camera movement along the Y-axis.
               leftStick = input.CurrentGamePadStates[(int)ControllingPlayer.Value].ThumbSticks.Left;
               Position = new Vector3(Position.X,
                                             MathHelper.Clamp(Position.Y + leftStick.Y / 10, MIN_CAMERA_HEIGHT, MAX_CAMERA_HEIGHT), Position.Z);

               // Control camera movement along the positive Z-axis.
               float leftTrigger = input.CurrentGamePadStates[(int)ControllingPlayer.Value].Triggers.Left;
               Position = new Vector3(Position.X, Position.Y,
                    MathHelper.Clamp(Position.Z + leftTrigger / 10, -10, 10));

               // Control camera movement along the negative Z-Axis.
               float rightTrigger = input.CurrentGamePadStates[(int)ControllingPlayer.Value].Triggers.Right;
               Position = new Vector3(Position.X, Position.Y,
                                             MathHelper.Clamp(Position.Z - rightTrigger / 10, -10, 10));
          }

          #endregion

          #region Public Methods

          public override void PrintDebug()
          {
               //string s = string.Format("Current Camera Type: {0}\n\nCamera Position: {1}\nCamera Field of View: {2}\n",
                 //   camera.GetType().ToString(), Camera.Position.ToString(), Camera.FieldOfView.ToString());
               string s = ToString();

               Text.TextManager.DrawAutoCentered(PixelEngine.Screen.ScreenManager.Font,
                    s, new Vector2(EngineCore.ScreenCenter.X, 200f), Color.White, 0.5f);
          }

          public override string ToString()
          {
               return base.ToString();
          }

          #endregion
     }
}