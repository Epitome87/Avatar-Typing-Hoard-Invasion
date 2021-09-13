
#region Using Statements
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace PixelEngine.ResourceManagement
{
     public class ResourceManager : DrawableGameComponent
     {
          #region Singleton

          private static ResourceManager resourceManager = null;

          #endregion

          #region Fields

          protected static bool debugTrace = false;
          protected static ContentManager contentManager = null;
          protected static long totalResourceMemory = 0;

          protected static Dictionary<string, GameResourceBase> ResourceStorage =
                                              new Dictionary<string, GameResourceBase>();

          private static bool isInitialized = false;

          #endregion

          #region Properties

          public ContentManager ContentManager
          {
               get { return contentManager; }
          }

          public bool IsInitialized
          {
               get { return isInitialized; }
               set { isInitialized = value; }
          }

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          /// <param name="game">game</param>
          /// <param name="contentRootDirectory">root directory path of the content</param>
          public ResourceManager(Game game, string contentRootDirectory)
               : base(game)
          {
               contentManager = new ContentManager(game.Services, contentRootDirectory);
          }

          /// <summary>
          /// Create the cameras.
          /// </summary>
          public static void Initialize(Game game)
          {
               resourceManager = new ResourceManager(game, "Content");

               if (game != null)
               {
                    game.Components.Add(resourceManager);
               }

               isInitialized = true;
          }

          #endregion

          #region Disposal

          /// <summary>
          /// Remove all resource elements.
          /// </summary>
          protected override void Dispose(bool disposing)
          {
               //  Delete all resources in the storage
               RemoveResourceAll(disposing);

               contentManager.Unload();
               contentManager.Dispose();
               contentManager = null;

               base.Dispose(true);
          }

          #endregion

          #region Public LoadContent Method

          /// <summary>
          /// loads a resource file in the content folder.
          /// </summary>
          /// <typeparam name="T">resource type (i.e. Model or Texture2D)</typeparam>
          /// <param name="key">resource key name</param>
          /// <param name="filePath">resource file name</param>
          /// <returns>resource element</returns>
          public static GameResourceBase LoadContent<T>(string key, string filePath)
          {
               if (EngineCore.Game.GraphicsDevice == null)
                    throw new InvalidOperationException("No graphics device.");

               GameResourceBase resource = FindResourceByKey(key);

               if (resource != null)
                    return resource;

               //  loads a resource by content manager
               T obj = contentManager.Load<T>(filePath);
               if (obj == null)
                    throw new ArgumentException("Fail to load content (" + key +
                                                        " : " + filePath + ")");

               if (obj is Texture2D)
               {
                    resource = new GameResourceTexture2D(key, filePath,
                                                        obj as Texture2D);
               }

               else if (obj is Model)
               {
                    resource = new GameResourceModel(key, filePath, obj as Model);
               }

               else if (obj is SpriteFont)
               {
                    resource = new GameResourceFont(key, filePath, obj as SpriteFont);
               }

               else
               {
                    throw new NotSupportedException("Not supported the resource");
               }

               if (debugTrace)
               {
                    System.Diagnostics.Debug.WriteLine(
                                        string.Format("Load Resource : {0} ({1})",
                                                    filePath, resource.ToString()));
               }

               if (AddResource(resource))
                    return resource;

               return null;
          }

          #endregion

          #region Public Add / Remove Resource Method

          /// <summary>
          /// adds a resource element.
          /// </summary>
          /// <param name="resource">resource element</param>
          public static bool AddResource(GameResourceBase resource)
          {
               if (resource == null)
                    throw new ArgumentNullException("resource");

               if (string.IsNullOrEmpty(resource.Key))
               {
                    throw new ArgumentException("The resource contains an invalid key.");
               }
               else if (ResourceStorage.ContainsKey(resource.Key))
               {
                    throw new ArgumentException(
                                    "The resource is already in the manager.");
               }

               ResourceStorage.Add(resource.Key, resource);

               return true;
          }

          /// <summary>
          /// removes a resource element by key name.
          /// </summary>
          /// <param name="key">resource key name</param>
          public bool RemoveResource(string key, bool disposing)
          {
               if (ResourceStorage.ContainsKey(key))
               {
                    if (debugTrace)
                    {
                         System.Diagnostics.Debug.WriteLine(
                             string.Format("Dispose Resource : {0} ({1})",
                                             ResourceStorage[key].AssetName,
                                             ResourceStorage[key].ToString()));
                    }

                    if (disposing)
                         ResourceStorage[key].Dispose();
               }

               return ResourceStorage.Remove(key);
          }

          /// <summary>
          /// removes a resource element by object.
          /// </summary>
          /// <param name="resource">a resource element</param>
          public bool RemoveResource(GameResourceBase resource, bool disposing)
          {
               return RemoveResource(resource.Key, disposing);
          }

          /// <summary>
          /// remove all resource elements.
          /// </summary>
          public void RemoveResourceAll(bool disposing)
          {
               foreach (string key in ResourceStorage.Keys)
               {
                    if (debugTrace)
                    {
                         System.Diagnostics.Debug.WriteLine(
                             string.Format("Dispose Resource : {0} ({1})",
                                             ResourceStorage[key].AssetName,
                                             ResourceStorage[key].ToString()));
                    }

                    if (disposing)
                         ResourceStorage[key].Dispose();
               }

               ResourceStorage.Clear();

               // Clean up some garbage
               GC.Collect();
          }

          /// <summary>
          /// removes a resource element by object.
          /// </summary>
          /// <param name="resource">resource element object</param>
          public bool RemoveResourceByObject(object resource, bool disposing)
          {
               //  Finding the resource in storage by object
               GameResourceBase res = FindResource(resource);
               if (res != null)
               {
                    return RemoveResource(res, disposing);
               }

               return false;
          }

          /// <summary>
          /// finds a resource element by key name.
          /// </summary>
          /// <param name="key">resource key name</param>
          public static GameResourceBase FindResourceByKey(string key)
          {
               //  Finding the resource in storage by key
               if (ResourceStorage.ContainsKey(key))
               {
                    return ResourceStorage[key];
               }

               return null;
          }

          /// <summary>
          /// finds a resource element by id.
          /// </summary>
          /// <param name="id">resource id number</param>
          public GameResourceBase FindResourceById(int id)
          {
               //  Finding the resource in storage by ID
               foreach (GameResourceBase resource in ResourceStorage.Values)
               {
                    if (resource.Id == id)
                         return resource;
               }

               return null;
          }

          /// <summary>
          /// finds a resource element by asset name.
          /// </summary>
          /// <param name="assetName">resource asset name</param>
          public GameResourceBase FindResourceByAssetName(string assetName)
          {
               //  Finding the resource in storage by name
               foreach (GameResourceBase resource in ResourceStorage.Values)
               {
                    if (resource.AssetName == assetName)
                         return resource;
               }

               return null;
          }

          /// <summary>
          /// finds a resource element by object.
          /// </summary>
          /// <param name="resource">resource element object</param>
          public GameResourceBase FindResource(object resource)
          {
               //  Finding the resource in storage
               foreach (GameResourceBase res in ResourceStorage.Values)
               {
                    if (res.Resource.Equals(resource))
                         return res;
               }

               return null;
          }

          #endregion

          #region Public Get Methods

          /// <summary>
          /// Gets a texture by key name.
          /// </summary>
          /// <param name="key">resource key name</param>
          public static GameResourceTexture2D GetTexture2D(string key)
          {
               return (GameResourceTexture2D)FindResourceByKey(key);
          }

          /// <summary>
          /// Gets a model resource by key name.
          /// </summary>
          /// <param name="key">resource key name</param>
          public static GameResourceModel GetModel(string key)
          {
               return (GameResourceModel)FindResourceByKey(key);
          }

          /// <summary>
          /// Gets a font resource by key name.
          /// </summary>
          /// <param name="key">resource key name</param>
          public static GameResourceFont GetFont(string key)
          {
               return (GameResourceFont)FindResourceByKey(key);
          }

          #endregion

          #region Public Load Methods

          /// <summary>
          /// Loads a texture file (i.e. tga or bmp).
          /// </summary>
          /// <param name="fileName">image file name in the content folder</param>
          /// <returns>resource element</returns>
          public static GameResourceFont LoadFont(string fileName)
          {
               string keyName = Path.GetFileName(fileName);

               //  Find the texture resource from ResourceManager by file name
               GameResourceFont resource = GetFont(keyName);

               //  If can't find stored resource
               if (resource == null)
               {
                    LoadContent<SpriteFont>(keyName, fileName);

                    resource = GetFont(keyName);
               }

               //  Can't get resource If loading failed!
               if (resource == null)
               {
                    throw new ArgumentException("Fail to loaded texture : " + fileName);
               }
               else if (resource.IsDisposed)
               {
                    throw new InvalidOperationException(
                        "Already disposed texture : " + fileName);
               }

               return resource;
          }

          /// <summary>
          /// Loads a texture file (i.e. tga or bmp).
          /// </summary>
          /// <param name="fileName">image file name in the content folder</param>
          /// <returns>resource element</returns>
          public static GameResourceTexture2D LoadTexture(string fileName)
          {
               string keyName = Path.GetFileName(fileName);

               //  Find the texture resource from ResourceManager by file name
               GameResourceTexture2D resource = GetTexture2D(keyName);

               //  If can't find stored resource
               if (resource == null)
               {
                    LoadContent<Texture2D>(keyName, fileName);

                    resource = GetTexture2D(keyName);
               }

               //  Can't get resource If loading failed!
               if (resource == null)
               {
                    throw new ArgumentException("Fail to loaded texture : " + fileName);
               }
               else if (resource.Texture2D.IsDisposed)
               {
                    throw new InvalidOperationException(
                        "Already disposed texture : " + fileName);
               }

               return resource;
          }

          /// <summary>
          /// Loads a texture file (i.e. tga or bmp).
          /// </summary>
          /// <param name="fileName">image file name in the content folder</param>
          /// <returns>resource element</returns>
          public static GameResourceModel LoadModel(string fileName)
          {
               string keyName = Path.GetFileName(fileName);

               //  Find the texture resource from ResourceManager by file name
               GameResourceModel resource = GetModel(keyName);

               //  If can't find stored resource
               if (resource == null)
               {
                    LoadContent<Model>(keyName, fileName);

                    resource = GetModel(keyName);
               }

               //  Can't get resource If loading failed!
               if (resource == null)
               {
                    throw new ArgumentException("Failed to loaded model : " + fileName);
               }

               else if (resource.IsDisposed)
               {
                    throw new InvalidOperationException(
                        "Already disposed model : " + fileName);
               }

               return resource;
          }

          #endregion
     }
}