#region File Description
//-----------------------------------------------------------------------------
// InputState.cs
// Matt McGrath, with help provided by:
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
#endregion

namespace PixelEngine.Screen
{
     /// <summary>
     /// Helper for reading input from keyboard and gamepad. This class tracks both
     /// the current and previous state of both input devices, and implements query
     /// methods for high level input actions such as "move up through the menu"
     /// or "pause the game".
     /// </summary>
     public class InputState
     {
          #region Fields

          public const int MaxInputs = 4;

          public readonly KeyboardState[] CurrentKeyboardStates;
          public readonly GamePadState[] CurrentGamePadStates;

          public readonly KeyboardState[] LastKeyboardStates;
          public readonly GamePadState[] LastGamePadStates;

          public readonly bool[] GamePadWasConnected;
          public readonly bool[] GamePadWasDisconnected;

          #endregion

          #region Initialization

          /// <summary>
          /// Constructs a new input state.
          /// </summary>
          public InputState()
          {
               CurrentKeyboardStates = new KeyboardState[MaxInputs];
               CurrentGamePadStates = new GamePadState[MaxInputs];

               LastKeyboardStates = new KeyboardState[MaxInputs];
               LastGamePadStates = new GamePadState[MaxInputs];

               GamePadWasConnected = new bool[MaxInputs];
               GamePadWasDisconnected = new bool[MaxInputs];
          }

          #endregion

          #region Update

          /// <summary>
          /// Reads the latest state of the keyboard and gamepad.
          /// </summary>
          public void Update()
          {
               for (int i = 0; i < MaxInputs; i++)
               {
                    LastKeyboardStates[i] = CurrentKeyboardStates[i];
                    LastGamePadStates[i] = CurrentGamePadStates[i];

                    CurrentKeyboardStates[i] = Keyboard.GetState((PlayerIndex)i);
                    CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);

                    // Keep track of whether a gamepad has ever been
                    // connected, so we can detect if it is unplugged.
                    if (CurrentGamePadStates[i].IsConnected)
                    {
                         GamePadWasConnected[i] = true;
                         GamePadWasDisconnected[i] = false;
                    }

                    // I added this; in testing.
                    if (GamePadWasConnected[i] && !CurrentGamePadStates[i].IsConnected)
                    {
                         GamePadWasDisconnected[i] = true;

                         // NEW: Only care if this is the ACTIVE CONTROLLER
                         if (PixelEngine.EngineCore.ControllingPlayer != null &&
                             i == (int)PixelEngine.EngineCore.ControllingPlayer)
                         {
                              int? controllerUnpluggedPrompt = SimpleGuideMessageBox.ShowMessageBox((PlayerIndex)i, "Controller Disconnected",
                                   "The active controller has been disconnected. Please reconnect Controller #" + (i + 1).ToString() 
                                   + " to continue.\n\nIf you are using a USB Keyboard, please ensure Controller #1 remains connected at all times.",
                                   new string[] { "OK" }, 0, MessageBoxIcon.Warning);
                         }
                    }
               }
          }

          #endregion

          #region Is Key Down / Is New Key Methods

          /// <summary>
          /// Helper for checking if a key is being pressed during this update. The
          /// controllingPlayer parameter specifies which player to read input for.
          /// If this is null, it will accept input from any player. When a keypress
          /// is detected, the output playerIndex reports which player pressed it.
          /// </summary>
          public bool IsKeyDown(Keys key, PlayerIndex? controllingPlayer,
                                              out PlayerIndex playerIndex)
          {
               if (controllingPlayer.HasValue)
               {
                    // Read input from the specified player.
                    playerIndex = controllingPlayer.Value;

                    int i = (int)playerIndex;

                    return (CurrentKeyboardStates[i].IsKeyDown(key));
               }
               else
               {
                    // Accept input from any player.
                    return (IsKeyDown(key, PlayerIndex.One, out playerIndex) ||
                            IsKeyDown(key, PlayerIndex.Two, out playerIndex) ||
                            IsKeyDown(key, PlayerIndex.Three, out playerIndex) ||
                            IsKeyDown(key, PlayerIndex.Four, out playerIndex));
               }
          }

          /// <summary>
          /// Helper for checking if a key was newly pressed during this update. The
          /// controllingPlayer parameter specifies which player to read input for.
          /// If this is null, it will accept input from any player. When a keypress
          /// is detected, the output playerIndex reports which player pressed it.
          /// </summary>
          public bool IsNewKeyPress(Keys key, PlayerIndex? controllingPlayer,
                                              out PlayerIndex playerIndex)
          {
               if (controllingPlayer.HasValue)
               {
                    // Read input from the specified player.
                    playerIndex = controllingPlayer.Value;

                    int i = (int)playerIndex;

                    return (CurrentKeyboardStates[i].IsKeyDown(key) &&
                            LastKeyboardStates[i].IsKeyUp(key));
               }
               else
               {
                    // Accept input from any player.
                    return (IsNewKeyPress(key, PlayerIndex.One, out playerIndex) ||
                            IsNewKeyPress(key, PlayerIndex.Two, out playerIndex) ||
                            IsNewKeyPress(key, PlayerIndex.Three, out playerIndex) ||
                            IsNewKeyPress(key, PlayerIndex.Four, out playerIndex));
               }
          }

          /// <summary>
          /// Helper for checking if a key was newly pressed during this update. The
          /// controllingPlayer parameter specifies which player to read input for.
          /// If this is null, it will accept input from any player. When a keypress
          /// is detected, the output playerIndex reports which player pressed it.
          /// </summary>
          public bool IsNewKeyPress(PlayerIndex? controllingPlayer,
                                              out PlayerIndex playerIndex)
          {
               if (controllingPlayer.HasValue)
               {
                    // Read input from the specified player.
                    playerIndex = controllingPlayer.Value;

                    int i = (int)playerIndex;

                    for (int c = 0; c < 255; c++)
                    {
                         Keys key = (Keys)(c);

                         if ((CurrentKeyboardStates[i].IsKeyDown(key) &&
                           LastKeyboardStates[i].IsKeyUp(key)))
                              return true;
                    }

                    return false;
               }
               else
               {
                    // Accept input from any player.
                    return (IsNewKeyPress(PlayerIndex.One, out playerIndex) ||
                            IsNewKeyPress(PlayerIndex.Two, out playerIndex) ||
                            IsNewKeyPress(PlayerIndex.Three, out playerIndex) ||
                            IsNewKeyPress(PlayerIndex.Four, out playerIndex));
               }
          }

          #endregion

          #region Is Button Down / Is New Button Methods

          /// <summary>
          /// Helper for checking if a button is being pressed during this update.
          /// The controllingPlayer parameter specifies which player to read input for.
          /// If this is null, it will accept input from any player. When a button press
          /// is detected, the output playerIndex reports which player pressed it.
          /// </summary>
          public bool IsButtonDown(Buttons button, PlayerIndex? controllingPlayer,
                                                       out PlayerIndex playerIndex)
          {
               if (controllingPlayer.HasValue)
               {
                    // Read input from the specified player.
                    playerIndex = controllingPlayer.Value;

                    int i = (int)playerIndex;

                    return (CurrentGamePadStates[i].IsButtonDown(button));
               }
               else
               {
                    // Accept input from any player.
                    return (IsButtonDown(button, PlayerIndex.One, out playerIndex) ||
                            IsButtonDown(button, PlayerIndex.Two, out playerIndex) ||
                            IsButtonDown(button, PlayerIndex.Three, out playerIndex) ||
                            IsButtonDown(button, PlayerIndex.Four, out playerIndex));
               }
          }

          /// <summary>
          /// Helper for checking if a button was newly pressed during this update.
          /// The controllingPlayer parameter specifies which player to read input for.
          /// If this is null, it will accept input from any player. When a button press
          /// is detected, the output playerIndex reports which player pressed it.
          /// </summary>
          public bool IsNewButtonPress(Buttons button, PlayerIndex? controllingPlayer,
                                                       out PlayerIndex playerIndex)
          {
               if (controllingPlayer.HasValue)
               {
                    // Read input from the specified player.
                    playerIndex = controllingPlayer.Value;

                    int i = (int)playerIndex;

                    return (CurrentGamePadStates[i].IsButtonDown(button) &&
                            LastGamePadStates[i].IsButtonUp(button));
               }
               else
               {
                    // Accept input from any player.
                    return (IsNewButtonPress(button, PlayerIndex.One, out playerIndex) ||
                            IsNewButtonPress(button, PlayerIndex.Two, out playerIndex) ||
                            IsNewButtonPress(button, PlayerIndex.Three, out playerIndex) ||
                            IsNewButtonPress(button, PlayerIndex.Four, out playerIndex));
               }
          }


          // MY OWN METHOD
          public bool IsNewLeftThumbstickPress(Vector2 direction, float threshold, PlayerIndex? controllingPlayer,
               out PlayerIndex playerIndex)
          {
               if (controllingPlayer.HasValue)
               {
                    // Read input from the specified player.
                    playerIndex = controllingPlayer.Value;

                    int i = (int)playerIndex;

                    // Joystick leaning left.
                    if (direction.X < 0)
                    {
                         return (CurrentGamePadStates[(int)controllingPlayer].ThumbSticks.Left.X < threshold &&
                              LastGamePadStates[(int)controllingPlayer].ThumbSticks.Left.X > threshold);
                    }

                    // Joystick leaning right.
                    else if (direction.X > 0)
                    {
                         return (CurrentGamePadStates[(int)controllingPlayer].ThumbSticks.Left.X > threshold &&
                              LastGamePadStates[(int)controllingPlayer].ThumbSticks.Left.X < threshold);
                    }

                    // Joystick leaning up.
                    else if (direction.Y > 0)
                    {
                         return (CurrentGamePadStates[(int)controllingPlayer].ThumbSticks.Left.Y > threshold &&
                              LastGamePadStates[(int)controllingPlayer].ThumbSticks.Left.Y < threshold);
                    }

                    // Joystick leaning down.
                    else //if (direction.Y < 0)
                    {
                         return (CurrentGamePadStates[(int)controllingPlayer].ThumbSticks.Left.Y < threshold &&
                              LastGamePadStates[(int)controllingPlayer].ThumbSticks.Left.Y > threshold);
                    }
               }
               else
               {
                    // Accept input from any player.
                    return (IsNewLeftThumbstickPress(direction, threshold, PlayerIndex.One, out playerIndex) ||
                            IsNewLeftThumbstickPress(direction, threshold, PlayerIndex.Two, out playerIndex) ||
                            IsNewLeftThumbstickPress(direction, threshold, PlayerIndex.Three, out playerIndex) ||
                            IsNewLeftThumbstickPress(direction, threshold, PlayerIndex.Four, out playerIndex));
               }
          }

          #endregion

          #region Menu / Game Related Input Methods

          /// <summary>
          /// Checks for a "menu select" input action.
          /// The controllingPlayer parameter specifies which player to read input for.
          /// If this is null, it will accept input from any player. When the action
          /// is detected, the output playerIndex reports which player pressed it.
          /// </summary>
          public bool IsMenuSelect(PlayerIndex? controllingPlayer,
                                   out PlayerIndex playerIndex)
          {
               return IsNewKeyPress(Keys.Space, controllingPlayer, out playerIndex) ||
                      IsNewKeyPress(Keys.Enter, controllingPlayer, out playerIndex) ||
                      IsNewButtonPress(Buttons.A, controllingPlayer, out playerIndex) ||
                      IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);
          }

          /// <summary>
          /// Checks for a "menu cancel" input action.
          /// The controllingPlayer parameter specifies which player to read input for.
          /// If this is null, it will accept input from any player. When the action
          /// is detected, the output playerIndex reports which player pressed it.
          /// </summary>
          public bool IsMenuCancel(PlayerIndex? controllingPlayer,
                                   out PlayerIndex playerIndex)
          {
               return IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) ||
                      IsNewButtonPress(Buttons.B, controllingPlayer, out playerIndex) ||
                      IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex);
          }

          /// <summary>
          /// Checks for a "menu up" input action.
          /// The controllingPlayer parameter specifies which player to read
          /// input for. If this is null, it will accept input from any player.
          /// </summary>
          public bool IsMenuUp(PlayerIndex? controllingPlayer)
          {
               PlayerIndex playerIndex;

               return IsNewKeyPress(Keys.Up, controllingPlayer, out playerIndex) ||
                      IsNewButtonPress(Buttons.DPadUp, controllingPlayer, out playerIndex) ||
                    //IsNewButtonPress(Buttons.LeftThumbstickUp, controllingPlayer, out playerIndex);
                      IsNewLeftThumbstickPress(new Vector2(0, 1), 0.5f, controllingPlayer, out playerIndex);

          }


          /// <summary>
          /// Checks for a "menu down" input action.
          /// The controllingPlayer parameter specifies which player to read
          /// input for. If this is null, it will accept input from any player.
          /// </summary>
          public bool IsMenuDown(PlayerIndex? controllingPlayer)
          {
               PlayerIndex playerIndex;

               return IsNewKeyPress(Keys.Down, controllingPlayer, out playerIndex) ||
                      IsNewButtonPress(Buttons.DPadDown, controllingPlayer, out playerIndex) ||
                    //IsNewButtonPress(Buttons.LeftThumbstickDown, controllingPlayer, out playerIndex);
                      IsNewLeftThumbstickPress(new Vector2(0, -1), -0.5f, controllingPlayer, out playerIndex);

          }

          /// <summary>
          /// Checks for a "menu left" input action.
          /// The controllingPlayer parameter specifies which player to read
          /// input for. If this is null, it will accept input from any player.
          /// </summary>
          public bool IsMenuLeft(PlayerIndex? controllingPlayer)
          {
               PlayerIndex playerIndex;

               return IsNewKeyPress(Keys.Left, controllingPlayer, out playerIndex) ||
                      IsNewButtonPress(Buttons.DPadLeft, controllingPlayer, out playerIndex) ||
                    //IsNewButtonPress(Buttons.LeftThumbstickLeft, controllingPlayer, out playerIndex);
                      IsNewLeftThumbstickPress(new Vector2(-1, 0), -0.5f, controllingPlayer, out playerIndex);
          }

          /// <summary>
          /// Checks for a "menu right" input action.
          /// The controllingPlayer parameter specifies which player to read
          /// input for. If this is null, it will accept input from any player.
          /// </summary>
          public bool IsMenuRight(PlayerIndex? controllingPlayer)
          {
               PlayerIndex playerIndex;

               return IsNewKeyPress(Keys.Right, controllingPlayer, out playerIndex) ||
                      IsNewButtonPress(Buttons.DPadRight, controllingPlayer, out playerIndex) ||
                    //IsNewButtonPress(Buttons.LeftThumbstickRight, controllingPlayer, out playerIndex);
                      IsNewLeftThumbstickPress(new Vector2(1, 0), 0.5f, controllingPlayer, out playerIndex);
          }


          /// <summary>
          /// Checks for a "pause the game" input action.
          /// The controllingPlayer parameter specifies which player to read
          /// input for. If this is null, it will accept input from any player.
          /// </summary>
          public bool IsPauseGame(PlayerIndex? controllingPlayer)
          {
               PlayerIndex playerIndex;

               return IsNewKeyPress(Keys.Escape, controllingPlayer, out playerIndex) ||
                    //IsNewButtonPress(Buttons.Back, controllingPlayer, out playerIndex) ||
                      IsNewButtonPress(Buttons.Start, controllingPlayer, out playerIndex);
          }

          #endregion
     }
}
