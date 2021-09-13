#region File Description
//-----------------------------------------------------------------------------
// StorageManager.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;
using PixelEngine.Screen;
using Microsoft.Xna.Framework;
using PixelEngine;
#endregion

namespace PixelEngine.Storage
{
     /// <summary>
     /// The Storage Manager manages storage devices (Memory Units, Hard-Drives).
     /// It handles functionality for saving and loading game data.
     /// </summary>
     public static class StorageManager
     {
          #region Fields

          private static bool gameSaveRequested = false;
          private static StorageDevice device;
          private static SaveGameData saveData;

          private static string saveName = "Avatar Typing: Horde Invasion - Save";

          // This field is gone in newer Engine. Why?
          private static bool savingEnabled = true;

          #endregion

          #region Properties

          public static bool SaveRequested
          {
               get { return gameSaveRequested; }
               set { gameSaveRequested = value; }
          }

          public static StorageDevice Device
          {
               get { return device; }
               set { device = value; }
          }

          public static SaveGameData SaveData
          {
               get { return saveData; }
               set { saveData = value; }
          }

          #endregion

          // XNA 4.0
          //To make life easier simply create a method to replace the storageDevice.OpenContainer...

          /// <summary>
          /// Synchronously opens storage container
          /// </summary>
          public static StorageContainer OpenContainer(StorageDevice storageDevice, string saveGameName)
          {
               IAsyncResult result = storageDevice.BeginOpenContainer(saveGameName, null, null);

               // Wait for the WaitHandle to become signaled.
               result.AsyncWaitHandle.WaitOne();

               StorageContainer container = storageDevice.EndOpenContainer(result);

               // Close the wait handle.
               result.AsyncWaitHandle.Close();

               return container;
          }

          #region Storage-Related Methods

          /// <summary>
          /// Internally sets the StorageDevice, requests a game save, 
          /// shows the storage device selector, then loads and saves
          /// a game.
          /// 
          /// Note: Currently not useful since we need to handle our save data
          /// ourselves before and after saving a game.
          /// </summary>
          /// <param name="result"></param>
          /// <param name="data"></param>
          public static void GetDevice(IAsyncResult result, SaveGameData data)
          {
               // Set the request flag
               if ((!Guide.IsVisible) && (gameSaveRequested == false))
               {
                    gameSaveRequested = true;

                    // Doesn't work in XNA 4.0:
                    //result = Guide.BeginShowStorageDeviceSelector(null, null);

                    // Change To:
                    result = StorageDevice.BeginShowSelector(null, null);
               }

               if ((gameSaveRequested) && (result.IsCompleted))
               {
                    // Doesn't work in XNA 4.0:
                    //device = Guide.EndShowStorageDeviceSelector(result);

                    // Change To:
                    device = StorageDevice.EndShowSelector(result);

                    if (device != null && device.IsConnected)
                    {
                         // Load the game off the given Device.
                         DoLoadGame(device);

                         // New from newer Engine. Not sure why?
                         // Now save the game to the given device,
                         // using hte data given to GetDevice().
                         DoSaveGame(device, data);
                    }

                    // Reset the request flag
                    gameSaveRequested = false;
               }

          }

          /// <summary>
          /// This method serializes a data object into
          /// the StorageContainer for this game.
          /// </summary>
          /// <param name="device"></param>
          public static void DoSaveGame(StorageDevice device, Object saveObject)
          {
               // Show "Now Saving" animation?
               ScreenManager.AddPopupScreen(new PixelEngine.SavePopUpScreen(), EngineCore.ControllingPlayer.Value);

               // Create the data to save.
               SaveGameData data = new SaveGameData();

               SaveData = data;

               // Open a storage container.
               StorageContainer container = OpenContainer(device, saveName);

               // Get the path of the save game.
               string filename = "savegame.sav";


               // Open the file, creating it if necessary.
               using (Stream stream = container.OpenFile(filename, FileMode.Create))
               {
                    // Convert the object to XML data and put it in the stream.
                    new XmlSerializer(typeof(SaveGameData)).Serialize(stream, saveObject);
               }

               // Close the file.
               //stream.Close();

               // Dispose the container, to commit changes.
               container.Dispose();
          }

          /// <summary>
          /// This method loads a serialized data object
          /// from the StorageContainer for this game.
          /// </summary>
          /// <param name="device"></param>
          public static bool DoLoadGame(StorageDevice device)
          {
               bool saveAlreadyExisted = true;

               // Open a storage container.
               StorageContainer container = OpenContainer(device, saveName);

               // Get the path of the save game.
               string filename = "savegame.sav";

              // Check to see whether the save exists.
              if (!container.FileExists(filename))
              {
                   // Notify the user there is no save.


                   container.CreateFile(filename);

                   saveAlreadyExisted = false;
              }

              // Open the file.
              else
               {
                    using (Stream stream = container.OpenFile(filename, FileMode.Open, FileAccess.Read))
                    {
                         // Read the data from the file.
                         XmlSerializer serializer = new XmlSerializer(typeof(SaveGameData));
                          
                         // Load game data.
                         SaveGameData data = (SaveGameData)serializer.Deserialize(stream);

                         SaveData = data;
                    }
               }

               // Close the file.
               //stream.Close();

               // Dispose the container.
               container.Dispose();

               return saveAlreadyExisted;
          }
          /// <summary>
          /// This method creates a file called demobinary.sav and places
          /// it in the StorageContainer for this game.
          /// </summary>
          /// <param name="device"></param>
          public static void DoCreate(StorageDevice device)
          {
               // Open a storage container.

               // Doesn't work in XNA 4.0:
               //StorageContainer container = device.OpenContainer(saveName);

               // Change To: ???
               IAsyncResult containerResult = device.BeginOpenContainer(saveName, null, null);
               StorageContainer container = device.EndOpenContainer(containerResult);

               // Add the container path to our file name.
               //string filename = Path.Combine(container.Path, "demobinary.sav");

               string filename = "demobinary.sav";

               // Create a new file.
               if (!File.Exists(filename))
               {
                    FileStream file = File.Create(filename);
                    file.Close();
               }
               // Dispose the container, to commit the data.
               container.Dispose();
          }
          /// <summary>
          /// This method illustrates how to open a file. It presumes
          /// that demobinary.sav has been created.
          /// </summary>
          /// <param name="device"></param>
          public static void DoOpen(StorageDevice device)
          {
               // Open a storage container.

               // Doesn't work in XNA 4.0:
               //StorageContainer container = device.OpenContainer(saveName);

               // Change To: ???
               IAsyncResult containerResult = device.BeginOpenContainer(saveName, null, null);
               StorageContainer container = device.EndOpenContainer(containerResult);

               // Add the container path to our file name.
               //string filename = Path.Combine(container.Path, "demobinary.sav");

               string filename = "demobinary.sav";

               FileStream file = File.Open(filename, FileMode.Open);
               file.Close();

               // Dispose the container.
               container.Dispose();
          }
          /// <summary>
          /// This method illustrates how to copy files.  It presumes
          /// that demobinary.sav has been created.
          /// </summary>
          /// <param name="device"></param>
          public static void DoCopy(StorageDevice device)
          {
               // Open a storage container.

               // Doesn't work in XNA 4.0:
               //StorageContainer container = device.OpenContainer(saveName);

               // Change To: ???
               IAsyncResult containerResult = device.BeginOpenContainer(saveName, null, null);
               StorageContainer container = device.EndOpenContainer(containerResult);

               // Add the container path to our file name.
               //string filename = Path.Combine(container.Path, "demobinary.sav");
               //string copyfilename = Path.Combine(container.Path, "copybinary.sav");

               string filename = "demobinary.sav";
               string copyfilename = "copybinary.sav";

               File.Copy(filename, copyfilename, true);

               // Dispose the container, to commit the change.
               container.Dispose();
          }
          /// <summary>
          /// This method illustrates how to rename files.  It presumes
          /// that demobinary.sav has been created.
          /// </summary>
          /// <param name="device"></param>
          public static void DoRename(StorageDevice device)
          {
               // Open a storage container.

               // Doesn't work in XNA 4.0:
               //StorageContainer container = device.OpenContainer(saveName);

               // Change To: ???
               IAsyncResult containerResult = device.BeginOpenContainer(saveName, null, null);
               StorageContainer container = device.EndOpenContainer(containerResult);

               // Add the container path to our file name.
               //string oldfilename = Path.Combine(container.Path, "demobinary.sav");
               //string newfilename = Path.Combine(container.Path, "renamebinary.sav");

               string oldfilename = "demobinary.sav";
               string newfilename = "renamebinary.sav";

               if (!File.Exists(newfilename))
                    File.Move(oldfilename, newfilename);

               // Dispose the container, to commit the change
               container.Dispose();
          }
          /// <summary>
          /// This method illustrates how to enumerate files in a 
          /// StorageContainer.
          /// </summary>
          /// <param name="device"></param>
          public static void DoEnumerate(StorageDevice device)
          {
               // Open a storage container.

               // Doesn't work in XNA 4.0:
               //StorageContainer container = device.OpenContainer(saveName);

               // Change To: ???
               IAsyncResult containerResult = device.BeginOpenContainer(saveName, null, null);
               StorageContainer container = device.EndOpenContainer(containerResult);

               // Doesn't work in XNA 4.0:
               ICollection<string> FileList = Directory.GetFiles(saveName);//container.Path);

               foreach (string filename in FileList)
               {
                    Console.WriteLine(filename);
               }

               // Dispose the container.
               container.Dispose();
          }
          /// <summary>
          /// This method deletes a file previously created by this demo.
          /// </summary>
          /// <param name="device"></param>
          public static void DoDelete(StorageDevice device)
          {
               // Open a storage container.

               // Doesn't work in XNA 4.0:
               //StorageContainer container = device.OpenContainer(saveName);

               // Change To: ???
               IAsyncResult containerResult = device.BeginOpenContainer(saveName, null, null);
               StorageContainer container = device.EndOpenContainer(containerResult);


               // Add the container path to our file name.
               //string filename = Path.Combine(container.Path, "demobinary.sav");

               string filename = "demobinary.sav";

               // Delete the new file.
               if (File.Exists(filename))
                    File.Delete(filename);

               // Dispose the container, to commit the change.
               container.Dispose();
          }
          /// <summary>
          /// This method opens a file using System.IO classes and the
          /// TitleLocation property.  It presumes that a file named
          /// ship.dds has been deployed alongside the game.
          /// </summary>
          public static void DoOpenFile()
          {
               FileStream file = OpenTitleFile("ship.dds", FileMode.Open, FileAccess.Read);
               Console.WriteLine("File Size: " + file.Length);
               file.Close();
          }

          public static FileStream OpenTitleFile(string filename, FileMode mode, FileAccess access)
          {
               // Doesn't work in XNA 4.0:
               //string fullpath = Path.Combine(StorageContainer.TitleLocation, filename);

               // Change To: No Idea!
               string fullpath = filename;


               return File.Open(fullpath, mode, access);
          }

          #endregion

          #region DeviceChanged Event Handler

          /// <summary>
          /// Hook this Event up to StorageDevice.DeviceChanged in order to define the logic
          /// that occurs when a DeviceChanged Event occurs.
          /// 
          /// This Event occurs when a StorageDevice is removed or inserted.
          /// 
          /// For user-friendliness, this Event only bugs the player when the StorageDevice
          /// used to save the game has been disconnected.
          /// </summary>
          /// <param name="sender"></param>
          /// <param name="e"></param>
          public static void StorageDevice_DeviceChanged(object sender, EventArgs e)
          {
               if (!device.IsConnected)
               {
                    if (!Guide.IsVisible)
                    {
                         Guide.BeginShowMessageBox(EngineCore.ControllingPlayer.Value,
                              "Storage Device Removed",
                              "The Storage Device used to save game progress has been removed.\n" +
                              "Your progress will no longer be saved until the Storage Device is connected.\n" +
                              "You can also visit the Settings Menu for Storage Device configuration.",
                              new string[] { "OK" }, 0, MessageBoxIcon.Alert,
                              MessageBoxEnd, null);
                    }
               }
          }

          private static void MessageBoxEnd(IAsyncResult result)
          {
               int? dialogResult = Guide.EndShowMessageBox(result);

               if (dialogResult == null)
                    dialogResult = -1;
          }

          #endregion
     }
}