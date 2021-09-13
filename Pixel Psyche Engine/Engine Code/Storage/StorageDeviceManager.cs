#region File Description
//-----------------------------------------------------------------------------
// StorageDeviceManager.cs
// Copyright (C) Matt McGrath. All rights reserved.
// Borrowed from this great tutorial: 
// http://www.xnawiki.com/index.php?title=StorageDeviceManager
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace PixelEngine.Storage
{
     #region StorageDeviceSelectorEventResponse Enum
     
     /// <summary>
     /// Defines an action in response to a StorageDeviceEventArgs
     /// </summary>
     public enum StorageDeviceSelectorEventResponse
     {
          /// <summary>
          /// Do nothing.
          /// </summary>
          None,

          /// <summary>
          /// Prompt the user to select a new storage device.
          /// </summary>
          Prompt,

          /// <summary>
          /// Force the user to select a new storage device.
          /// </summary>
          Force
     }

     #endregion

     #region StorageDeviceEventArgs Class

     public class StorageDeviceEventArgs : EventArgs
     {
          /// <summary>
          /// Gets or sets the desired response to the event.
          /// </summary>
          public StorageDeviceSelectorEventResponse EventResponse { get; set; }
     }

     #endregion

     #region StorageDevicePromptEventArgsClass

     public class StorageDevicePromptEventArgs : EventArgs
     {
          /// <summary>
          /// Gets whether or not the user has chosen to select a new device.
          /// If true, the StorageDeviceManager will automatically prompt for 
          /// the new device.
          /// </summary>
          public bool PromptForDevice { get; set; }
     }

     #endregion

     #region StorageDeviceManager Class - Derives From GameComponent

     /// <summary>
     /// A GameComponent class which acts as a Manager for Storage Devices.
     /// Allows for all functionality related to storage.
     /// </summary>
     public class StorageDeviceManager : GameComponent
     {
          #region Fields

          // The text used for the four prompts used by the manager.
          private const string ReselectStorageDeviceText =
             "No storage device was selected. Would you like to re-select the storage device?";

          private const string DisconnectReselectDeviceText =
             "An active storage device has been disconnected. Would you like to select a new storage device?";

          private const string ForceReselectDeviceText =
             "No storage device was selected. A storage device is required to continue. Select Ok to choose a storage device.";

          private const string ForceDisconnectReselectText =
             "An active storage device has been disconnected. " +
             "A storage device is required to continue. Select Ok to choose a storage device.";

          // Was the device connected last frame?
          private bool wasDeviceConnected;

          // Should the Guide.BeginShowStorageDeviceSelector be called?
          private bool showDeviceSelector;

          // Should we prompt the user to optionally have them select a new device for canceling the selector?
          private bool promptToReSelectDevice;

          // Should we prompt the user to force them to select a new device for canceling the selector?
          private bool promptToForceReselect;

          // should we prompt the user to optionally have them select a new device after a device disconnect?
          private bool promptForDisconnect;

          // Should we prompt the user to force them to select a new device after a device disconnect?
          private bool promptForDisconnectForced;

          // Keep one instance of each of the event arguments to avoid garbage creation
          private readonly StorageDeviceEventArgs eventArgs = new StorageDeviceEventArgs();
          private readonly StorageDevicePromptEventArgs promptEventArgs = new StorageDevicePromptEventArgs();

          #endregion

          #region Properties

          /// <summary>
          /// Fired when a StorageDevice is successfully selected.
          /// </summary>
          public event EventHandler DeviceSelected;

          /// <summary>
          /// Fired when the StorageDevice selector is canceled.
          /// </summary>
          public event EventHandler<StorageDeviceEventArgs> DeviceSelectorCanceled;

          /// <summary>
          /// Fired when the non-forced reselect prompt is closed.
          /// </summary>
          public event EventHandler<StorageDevicePromptEventArgs> DevicePromptClosed;

          /// <summary>
          /// Fired when the StorageDevice becomes disconnected.
          /// </summary>
          public event EventHandler<StorageDeviceEventArgs> DeviceDisconnected;

          /// <summary>
          /// Gets the StorageDevice being managed.
          /// </summary>
          public StorageDevice Device { get; private set; }

          /// <summary>
          /// Gets the player (if any) used for the StorageDevice.
          /// </summary>
          public PlayerIndex? Player { get; private set; }

          /// <summary>
          /// Gets or sets the player to prompt if the storage device is player-agnostic.
          /// </summary>
          public PlayerIndex PlayerToPrompt { get; set; }

          /// <summary>
          /// Gets the amount of space required on the StorageDevice.
          /// </summary>
          public int RequiredBytes { get; private set; }

          #endregion

          #region Initialization

          /// <summary>
          /// Creates a new player-agnostic StorageDevice with no required amount of free space.
          /// </summary>
          /// <param name="game">
          /// The game to which the StorageDeviceManager will be added. The component does not add itself.
          /// </param>
          public StorageDeviceManager(Game game)
               : this(game, null, 0) { }

          /// <summary>
          /// Creates a new player-specific StorageDevice wiht no required amount of free space.
          /// </summary>
          /// <param name="game">
          /// The game to which the StorageDeviceManager will be added. The component does not add itself.
          /// </param>
          /// <param name="player">The player to prompt for the StorageDevice.</param>
          public StorageDeviceManager(Game game, PlayerIndex player)
               : this(game, player, 0) { }

          /// <summary>
          /// Creates a new player-agnostic StorageDevice with a required amount of free space.
          /// </summary>
          /// <param name="game">
          /// The game to which the StorageDeviceManager will be added. The component does not add itself.
          /// </param>
          /// <param name="requiredBytes">The amount of space (in bytes) required.</param>
          public StorageDeviceManager(Game game, int requiredBytes)
               : this(game, null, requiredBytes) { }

          /// <summary>
          /// Creates a new player-specific StorageDevice with a required amount of free space.
          /// </summary>
          /// <param name="game">
          /// The game to which the StorageDeviceManager will be added. The component does not add itself.
          /// </param>
          /// <param name="player">The player to prompt for the StorageDevice.</param>
          /// <param name="requiredBytes">The amount of space (in bytes) required.</param>
          // we cast the player argument to a PlayerIndex? to tell the compiler to call the private constructor.
          public StorageDeviceManager(Game game, PlayerIndex player, int requiredBytes)
               : this(game, (PlayerIndex?)player, requiredBytes) { }

          private StorageDeviceManager(Game game, PlayerIndex? player, int requiredBytes)
               : base(game)
          {
               // store the arguments
               Player = player;
               RequiredBytes = requiredBytes;

               PlayerToPrompt = PlayerIndex.One;
          }

          #endregion

          #region Update

          public override void Update(GameTime gameTime)
          {
               // if the device has just become disconnected, fire the event to see if we need to prompt for a new one
               if (Device != null && !Device.IsConnected && wasDeviceConnected)
                    FireDeviceDisconnectedEvent();

               // use a try/catch in case of the following conditions:
               // 1) GamerServicesComponent is not added. In this case Guide.IsVisible throws an exception.
               // 2) Guide.IsVisible returns false but Guide opens (from user input) before the code displays
               //    the Guide. This would cause the Guide to throw an exception.
               try
               {
                    // if the Guide is not visible...
                    if (!Guide.IsVisible)
                    {
                         // if we are to show the device selector...
                         if (showDeviceSelector)
                         {
                              // don't show device selector next frame; necessary if the user
                              // has only one storage device.
                              showDeviceSelector = false;

                              // show the selector based on whether we have a player-specific or
                              // player-agnostic storage device.
                              if (Player.HasValue)
                              {
                                   // Doesn't work in XNA 4.0:
                                   /*
                                   Guide.BeginShowStorageDeviceSelector(
                                      Player.Value,
                                      RequiredBytes,
                                      0,
                                      deviceSelectorCallback,
                                      null);
                                   */

                                   // Change To: Almost Positive
                                   StorageDevice.BeginShowSelector(Player.Value, RequiredBytes, 0, deviceSelectorCallback, null);
                              }
                              else
                              {
                                   // Doesn't work in XNA 4.0:
                                   /*
                                   Guide.BeginShowStorageDeviceSelector(
                                      RequiredBytes,
                                      0,
                                      deviceSelectorCallback,
                                      null);
                                   */

                                   // Change To: Almost Positive
                                   StorageDevice.BeginShowSelector(RequiredBytes, 0, deviceSelectorCallback, null);
                              }
                         }

                         // if we are prompting to see if the user wants a new device due to canceling the selector...
                         else if (promptToReSelectDevice)
                         {
                              if (Player.HasValue)
                              {
                                   Guide.BeginShowMessageBox(
                                      Player.Value,
                                      "Reselect Storage Device?",
                                      ReselectStorageDeviceText,
                                      new[] { "Yes. Select new device.", "No. Continue without device." },
                                      0,
                                      MessageBoxIcon.None,
                                      reselectPromptCallback,
                                      null);
                              }
                              else
                              {
                                   Guide.BeginShowMessageBox(
                                      "Reselect Storage Device?",
                                      ReselectStorageDeviceText,
                                      new[] { "Yes. Select new device.", "No. Continue without device." },
                                      0,
                                      MessageBoxIcon.None,
                                      reselectPromptCallback,
                                      null);
                              }
                         }

                         // if we are prompting to see if the user wants a new device due to a disconnect...
                         else if (promptForDisconnect)
                         {
                              if (Player.HasValue)
                              {
                                   Guide.BeginShowMessageBox(
                                      Player.Value,
                                      "Storage Device Disconnected",
                                      DisconnectReselectDeviceText,
                                      new[] { "Yes. Select new device.", "No. Continue without device." },
                                      0,
                                      MessageBoxIcon.None,
                                      reselectPromptCallback,
                                      null);
                              }
                              else
                              {
                                   Guide.BeginShowMessageBox(
                                      "Storage Device Disconnected",
                                      DisconnectReselectDeviceText,
                                      new[] { "Yes. Select new device.", "No. Continue without device." },
                                      0,
                                      MessageBoxIcon.None,
                                      reselectPromptCallback,
                                      null);
                              }
                         }

                         // if we are prompting to force a reselect of the device due to canceling the selector...
                         else if (promptToForceReselect)
                         {
                              if (Player.HasValue)
                              {
                                   Guide.BeginShowMessageBox(
                                      Player.Value,
                                      "Reselect Storage Device",
                                      ForceReselectDeviceText,
                                      new[] { "Ok" },
                                      0,
                                      MessageBoxIcon.None,
                                      forcePromptCallback,
                                      null);
                              }
                              else
                              {
                                   Guide.BeginShowMessageBox(
                                      "Reselect Storage Device",
                                      ForceReselectDeviceText,
                                      new[] { "Ok" },
                                      0,
                                      MessageBoxIcon.None,
                                      forcePromptCallback,
                                      null);
                              }
                         }

                         // if we are prompting to force a reselect of the device due to a disconnect...
                         else if (promptForDisconnectForced)
                         {
                              if (Player.HasValue)
                              {
                                   Guide.BeginShowMessageBox(
                                      Player.Value,
                                      "Storage Device Disconnected",
                                      ForceDisconnectReselectText,
                                      new[] { "Ok" },
                                      0,
                                      MessageBoxIcon.None,
                                      forcePromptCallback,
                                      null);
                              }
                              else
                              {
                                   Guide.BeginShowMessageBox(
                                      "Storage Device Disconnected",
                                      ForceDisconnectReselectText,
                                      new[] { "Ok" },
                                      0,
                                      MessageBoxIcon.None,
                                      forcePromptCallback,
                                      null);
                              }
                         }
                    }
               }

               // catch and write out any relevant exceptions for later debugging
               catch (GamerServicesNotAvailableException e)
               {
                    Debug.WriteLine(e.Message);
               }

               catch (GuideAlreadyVisibleException e)
               {
                    Debug.WriteLine(e.Message);
               }

               // store the state of the device's connection
               wasDeviceConnected = Device != null && Device.IsConnected;
          }

          #endregion
     
          #region Public PromptForDevice Method

          /// <summary>
          /// Instructs the manager to prompt the user to select a new device.
          /// </summary>
          public void PromptForDevice()
          {
               // simply flip to true
               showDeviceSelector = true;
          }

          #endregion

          #region Private Callback / Event Methods

          /// <summary>
          /// The callback used for either of our forced reselect prompts.
          /// </summary>
          /// <param name="ar">The prompt results.</param>
          private void forcePromptCallback(IAsyncResult ar)
          {
               // No more need to prompt.
               promptToForceReselect = false;
               promptForDisconnectForced = false;

               // Just have to end it.
               Guide.EndShowMessageBox(ar);

               // Get the device
               showDeviceSelector = true;
          }

          /// <summary>
          /// The callback used for either of our non-forced reselect prompts.
          /// </summary>
          /// <param name="ar">The prompt results.</param>
          private void reselectPromptCallback(IAsyncResult ar)
          {
               // No more need to prompt.
               promptForDisconnect = false;
               promptToReSelectDevice = false;

               // Get the result of the message box.
               int? choice = Guide.EndShowMessageBox(ar);

               // Get the device if the user chose the first option.
               showDeviceSelector = choice.HasValue && choice.Value == 0;

               // Fire an event for the game to know the result of the prompt.
               promptEventArgs.PromptForDevice = showDeviceSelector;
               if (DevicePromptClosed != null)
                    DevicePromptClosed(this, promptEventArgs);
          }

          /// <summary>
          /// The callback used for the device selector.
          /// </summary>
          /// <param name="ar">The selector results.</param>
          private void deviceSelectorCallback(IAsyncResult ar)
          {
               // Get the chosen device
               Device = StorageDevice.EndShowSelector(ar);

               // If a device was chosen...
               if (Device != null)
               {
                    // Fire the event.
                    if (DeviceSelected != null)
                         DeviceSelected(this, EventArgs.Empty);
               }

               // Otherwise
               else
               {
                    // Initialize the Event Args.
                    eventArgs.EventResponse = StorageDeviceSelectorEventResponse.Prompt;

                    // Fire the cancelation event to allow customization of the process.
                    if (DeviceSelectorCanceled != null)
                         DeviceSelectorCanceled(this, eventArgs);

                    // Handle the results of the event.
                    HandleEventArgResults();
               }
          }

          /// <summary>
          /// Fires off the event for a device becoming disconnected and handles the result.
          /// </summary>
          private void FireDeviceDisconnectedEvent()
          {
               // initialize the event args
               eventArgs.EventResponse = StorageDeviceSelectorEventResponse.Prompt;

               // fire the disconnection event to allow customization of the process
               if (DeviceDisconnected != null)
                    DeviceDisconnected(this, eventArgs);

               // handle the results of the event
               HandleEventArgResults();
          }

          /// <summary>
          /// Handles the result of the DeviceSelectorCanceled or DeviceDisconnected events.
          /// </summary>
          private void HandleEventArgResults()
          {
               // clear the Device reference
               Device = null;

               // determine the next action...
               switch (eventArgs.EventResponse)
               {
                    // will have the manager prompt the user with the option of reselecting the storage device
                    case StorageDeviceSelectorEventResponse.Prompt:
                         if (wasDeviceConnected)
                              promptForDisconnect = true;
                         else
                              promptToReSelectDevice = true;
                         break;

                    // will have the manager prompt the user that the device must be selected
                    case StorageDeviceSelectorEventResponse.Force:
                         if (wasDeviceConnected)
                              promptForDisconnectForced = true;
                         else
                              promptToForceReselect = true;
                         break;

                    // will have the manager do nothing
                    default:
                         promptForDisconnect = false;
                         promptForDisconnectForced = false;
                         promptToForceReselect = false;
                         showDeviceSelector = false;
                         break;
               }
          }

          #endregion
     }

     #endregion
}