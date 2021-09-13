
#region Using Statements
using System;
#endregion

namespace PixelEngine.ResourceManagement
{
     public class GameResourceBase : IDisposable
     {
          #region Fields

          int id = -1;
          string keyName = String.Empty;
          string assetName = String.Empty;
          bool isDisposed;

          protected object resource = null;

          #endregion

          #region Properties

          public int Id
          {
               get
               {
                    if (id == 0)
                    {
                         id = GetHashCode();
                    }
                    return id;
               }
          }

          public string Key
          {
               get { return keyName; }
          }

          public string AssetName
          {
               get { return assetName; }
          }

          public object Resource
          {
               get { return resource; }
          }

          public bool IsDisposed
          {
               get { return isDisposed; }
          }

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          /// <param name="key">key name</param>
          /// <param name="assetName">asset name</param>
          public GameResourceBase(string key, string assetName)
          {
               this.keyName = key;
               this.assetName = assetName;
          }

          #endregion

          #region Disposal

          public void Dispose()
          {
               Dispose(true);
               GC.SuppressFinalize(this);
          }

          protected virtual void Dispose(bool disposing)
          {
               if (!isDisposed)
               {
                    if (disposing)
                    {
                         //if we're manually disposing,
                         //then managed content should be unloaded
                         resource = null;
                    }
                    isDisposed = true;
               }
          }

          #endregion
     }
}