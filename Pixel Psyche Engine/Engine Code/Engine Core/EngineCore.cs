#region File Description
//-----------------------------------------------------------------------------
// EngineCore.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.Storage;
#endregion

namespace PixelEngine
{
     /// <remarks>
     /// The EngineCore handles the initialization of the PixelEngine's sub-components.
     /// It also stores global game data, such as the Game and ControllingPlayer.
     /// The EngineCore also serves as an interface between the actual game and
     /// the PixelEngine.
     /// </remarks>
     public class EngineCore : Microsoft.Xna.Framework.Game
     {
          #region Fields

          private static Game game = null;
          private static GraphicsDeviceManager graphicsDeviceManager = null;
          private static PlayerIndex? controllingPlayer;
          private static GraphicsInfo graphicsInformation;

          private static Vector2 screenCenter;
          private static float resolutionScale;
          private static Vector3 resolutionScale3D;

          #endregion

          protected static StorageDeviceManager storageDeviceManager;

          public static StorageDeviceManager StorageDeviceManager
          {
               get { return storageDeviceManager; }
               set { storageDeviceManager = value; }
          }








          // Define the viewports that we wish to render to. We will draw two viewports:
          // If Two Players:
          // - The top half of the screen
          // - The bottom half of the screen
          // If 3-4 Players:
          // - The top, left quarter of the screen.
          // - The top, right quarter of the screen.
          // - The bottom, left quarter of the screen.
          // - The bottom, right quarter of the screen.
          public static Viewport playerOneViewport;
          public static Viewport playerTwoViewport;
          public static Viewport playerThreeViewport;
          public static Viewport playerFourViewport;

          public static Viewport[] SplitscreenViewports = new Viewport[4];

          public static int numberOfPlayers = 2;


          #region Properties

          /// <summary>
          /// Gets the current instance of the  Game.
          /// </summary>
          public static Game Game
          {
               get { return game; }
          }

          /// <summary>
          /// Gets or Sets the GraphicsDeviceManager.
          /// </summary>
          public static GraphicsDeviceManager GraphicsDeviceManager
          {
               get { return graphicsDeviceManager; }
               set { graphicsDeviceManager = value; }
          }

          /// <summary>
          /// Gets the GraphicsDevice.
          /// 
          /// Returns Game.GraphicsDevice, so Game must be valid.
          /// Notes: Should we really get this property from Game?
          /// </summary>
          public new static GraphicsDevice GraphicsDevice
          {
               get { return Game.GraphicsDevice; }
          }

          /// <summary>
          /// Gets or Sets the ContentManager.
          /// 
          /// Returns Game.Content, so Game must be valid.
          /// Notes: Should we really get this property from Game?
          /// </summary>
          public new static ContentManager Content
          {
               get { return Game.Content; }
               set { Game.Content = value; }
          }

          /// <summary>
          /// Gets or Sets the GraphicsInfo structure.
          /// </summary>
          public static GraphicsInfo GraphicsInformation
          {
               get { return graphicsInformation; }
               set { graphicsInformation = value; }
          }

          /// <summary>
          /// Gets the index of the player who is currently controlling this screen,
          /// or null if it is accepting input from any player. This is used to lock
          /// the game to a specific player profile. The main menu responds to input
          /// from any connected gamepad, but whichever player makes a selection from
          /// this menu is given control over all subsequent screens, so other gamepads
          /// are inactive until the controlling player returns to the main menu.
          /// </summary>
          public static PlayerIndex? ControllingPlayer
          {
               get { return controllingPlayer; }
               //internal 
               set { controllingPlayer = value; }
          }

          /// <summary>
          /// Center of the screen.
          /// </summary>
          public static Vector2 ScreenCenter
          {
               get { return screenCenter; }
               set { screenCenter = value; }
          }

          /// <summary>
          /// Scale determined by the current resolution divided by the native resolution.
          /// Used to render the game identical on all resolutions.
          /// </summary>
          public static float ResolutionScale
          {
               get { return graphicsInformation.ScreenWidth / 1280f; }
               set { resolutionScale = value; }
          }

          /// <summary>
          /// Scale determined by the current resolution divided by the native resolution.
          /// Used to render the game identical on all resolutions.
          /// 
          /// NOTES: This is never even used it seems!
          /// </summary>
          public static Vector3 ResolutionScale3D
          {
               get
               {
                    return new Vector3(graphicsInformation.ScreenWidth / 1280f,
                         graphicsInformation.ScreenHeight / 720f, 1.0f);
               }

               set { resolutionScale3D = value; }
          }

          #endregion

          #region GraphicsInfo Struct

          /// <summary>
          /// Structure to store all fields relevant to Graphics.
          /// A GraphicsInfo object must be created (in the Game Code)
          /// and passed into the EngineCore Constructor, in order to 
          /// determine how the PixelEngine will be Initialized.
          /// </summary>
          public struct GraphicsInfo
          {
               private int screenWidth;
               private int screenHeight;
               private bool preferMultiSampling;
               private bool isFullScreen;

               /// <summary>
               /// Determines GraphicsDevice.PreferredBackBufferWidth.
               /// </summary>
               public int ScreenWidth
               {
                    get { return screenWidth; }
                    set { screenWidth = value; }
               }

               /// <summary>
               /// Determines GraphicsDevice.PreferredBackBufferHeight.
               /// </summary>
               public int ScreenHeight
               {
                    get { return screenHeight; }
                    set { screenHeight = value; }
               }

               /// <summary>
               /// Determines GraphicsDevice.PreferMultiSampling.
               /// </summary>
               public bool PreferMultiSampling
               {
                    get { return preferMultiSampling; }
                    set { preferMultiSampling = value; }
               }

               /// <summary>
               /// Determines GraphicsDevice.IsFullScreen.
               /// </summary>
               public bool IsFullScreen
               {
                    get { return isFullScreen; }
                    set { isFullScreen = value; }
               }
          }

          #endregion

          #region Constructor

          /// <summary>
          /// EngineCore Constructor. Sets the Game and various 
          /// GraphicsDeviceManager fields to default values.
          /// </summary>
          public EngineCore(GraphicsInfo graphicsInfo)
               : base()
          {
               game = this;
               graphicsDeviceManager = new GraphicsDeviceManager(this);

               graphicsInformation = graphicsInfo;
               graphicsDeviceManager.PreferredBackBufferWidth = graphicsInformation.ScreenWidth;
               graphicsDeviceManager.PreferredBackBufferHeight = graphicsInformation.ScreenHeight;
               graphicsDeviceManager.PreferMultiSampling = graphicsInformation.PreferMultiSampling;
               graphicsDeviceManager.IsFullScreen = graphicsInformation.IsFullScreen;

               // New: Testing
               //this.IsFixedTimeStep = false;
               // End New: Testing

               graphicsDeviceManager.PreparingDeviceSettings +=
                    new EventHandler<PreparingDeviceSettingsEventArgs>(
                         graphics_PreparingDeviceSettings);


               screenCenter = new Vector2(graphicsInformation.ScreenWidth / 2, graphicsInformation.ScreenHeight / 2);
          }

          #endregion

          #region EngineCore Initialization

          /// <summary>
          /// Initialize the Components you want your game to utilize.
          /// TO-DO: This should probably be done outside the PixelEngine.
          /// </summary>
          protected override void Initialize()
          {
#if DEBUG
               // Initialize the Debugger tool, if in debug mode.
               //Debugger.Initialize(this);
               //Components.Add(new PixelEngine.DebugUtilities.SafeArea.SafeAreaOverlay(this));
               //PixelEngine.DebugUtilities.FpsCounter.Initialize(this);
#endif





               // Create the viewports:

               // If one Player:
               if (numberOfPlayers == 1)
               {
                    playerOneViewport = new Viewport
                    {
                         MinDepth = 0,
                         MaxDepth = 1,
                         X = 0,
                         Y = 0,
                         Width = GraphicsDevice.Viewport.Width,
                         Height = GraphicsDevice.Viewport.Height,
                    };
               }

               // If two Players:
               if (numberOfPlayers == 2)
               {
                    playerOneViewport = new Viewport
                    {
                         MinDepth = 0,
                         MaxDepth = 1,
                         X = 0,
                         Y = 0,
                         Width = GraphicsDevice.Viewport.Width,
                         Height = GraphicsDevice.Viewport.Height / 2,
                    };

                    playerTwoViewport = new Viewport
                    {
                         MinDepth = 0,
                         MaxDepth = 1,
                         X = 0,
                         Y = GraphicsDevice.Viewport.Height / 2,
                         Width = GraphicsDevice.Viewport.Width,
                         Height = GraphicsDevice.Viewport.Height / 2,
                    };
               }

               // If more than two Players:
               if (numberOfPlayers > 2)
               {
                    playerOneViewport = new Viewport
                    {
                         MinDepth = 0,
                         MaxDepth = 1,
                         X = 0,
                         Y = 0,
                         Width = GraphicsDevice.Viewport.Width / 2,
                         Height = GraphicsDevice.Viewport.Height / 2,
                    };

                    playerTwoViewport = new Viewport
                    {
                         MinDepth = 0,
                         MaxDepth = 1,
                         X = GraphicsDevice.Viewport.Width / 2,
                         Y = 0,
                         Width = GraphicsDevice.Viewport.Width / 2,
                         Height = GraphicsDevice.Viewport.Height / 2,
                    };

                    playerThreeViewport = new Viewport
                    {
                         MinDepth = 0,
                         MaxDepth = 1,
                         X = 0,
                         Y = GraphicsDevice.Viewport.Height / 2,
                         Width = GraphicsDevice.Viewport.Width / 2,
                         Height = GraphicsDevice.Viewport.Height / 2,
                    };

                    playerFourViewport = new Viewport
                    {
                         MinDepth = 0,
                         MaxDepth = 1,
                         X = GraphicsDevice.Viewport.Width / 2,
                         Y = GraphicsDevice.Viewport.Height / 2,
                         Width = GraphicsDevice.Viewport.Width / 2,
                         Height = GraphicsDevice.Viewport.Height / 2,
                    };
               }

               SplitscreenViewports[0] = playerOneViewport;
               SplitscreenViewports[1] = playerTwoViewport;
               SplitscreenViewports[2] = playerThreeViewport;
               SplitscreenViewports[3] = playerFourViewport;




               base.Initialize();
          }

          #endregion

          #region Draw

          /// <summary>
          /// This is called when the game should draw itself.
          /// </summary>
          protected override void Draw(GameTime gameTime)
          {
               graphicsDeviceManager.GraphicsDevice.Clear(Color.Black);

               // The real drawing happens inside the PixelEngine's components,
               // and especially the screen manager component.
               base.Draw(gameTime);
          }

          #endregion

          #region Graphics_PreparingDeviceSettings Event Handler

          void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
          {
               // Xbox 360 and most PCs support FourSamples/0 
               // (4x) and TwoSamples/0 (2x) antialiasing.
               PresentationParameters pp =
                   e.GraphicsDeviceInformation.PresentationParameters;
#if XBOX
               // Properties don't exist in XNA 4.0:
               //pp.MultiSampleQuality = 0;
               //pp.MultiSampleType = MultiSampleType.FourSamples;
               // Change to this:
               //pp.MultiSampleCount 
               return;
#else
            int quality = 0;
            GraphicsAdapter adapter = e.GraphicsDeviceInformation.Adapter;
            SurfaceFormat format = adapter.CurrentDisplayMode.Format;
            // Check for 4xAA
            if (adapter.CheckDeviceMultiSampleType(DeviceType.Hardware, format,
                false, MultiSampleType.FourSamples, out quality))
            {
                // even if a greater quality is returned, we only want quality 0
                pp.MultiSampleQuality = 0;
                pp.MultiSampleType =
                    MultiSampleType.FourSamples;
            }
            // Check for 2xAA
            else if (adapter.CheckDeviceMultiSampleType(DeviceType.Hardware, 
                format, false, MultiSampleType.TwoSamples, out quality))
            {
                // even if a greater quality is returned, we only want quality 0
                pp.MultiSampleQuality = 0;
                pp.MultiSampleType =
                    MultiSampleType.TwoSamples;
            }
            return;
#endif
          }

          #endregion

          /*          
          // These are never used apparently, yet they seem useful here?!
          protected static StorageDeviceManager storageDeviceManager;

          public static StorageDeviceManager StorageDeviceManager
          {
               get { return storageDeviceManager; }
               set { storageDeviceManager = value; }
          }
          protected void DeviceDisconnected(object sender, StorageDeviceEventArgs e)
          {
               // force the user to choose a new storage device
               e.EventResponse = StorageDeviceSelectorEventResponse.Force;
          }

          protected void DeviceSelectorCanceled(object sender, StorageDeviceEventArgs e)
          {
               // force the user to choose a new storage device
               e.EventResponse = StorageDeviceSelectorEventResponse.Force;
          }
          */
     }
}