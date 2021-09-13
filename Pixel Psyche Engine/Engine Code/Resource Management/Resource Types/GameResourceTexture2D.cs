
#region Using Statements
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace PixelEngine.ResourceManagement
{
     public class GameResourceTexture2D : GameResourceBase
     {
          #region Fields

          Texture2D texture2D = null;

          #endregion

          #region Properties

          public Texture2D Texture2D
          {
               get { return texture2D; }
          }

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          /// <param name="key">key name</param>
          /// <param name="assetName">asset name</param>
          /// <param name="resource">texture resource</param>
          public GameResourceTexture2D(string key, string assetName, Texture2D resource)
               : base(key, assetName)
          {
               this.texture2D = resource;

               this.resource = (object)this.texture2D;
          }

          #endregion

          #region Disposal

          protected override void Dispose(bool disposing)
          {
               if (disposing)
               {
                    if (texture2D != null)
                    {
                         texture2D.Dispose();
                         texture2D = null;
                    }
               }

               base.Dispose(disposing);
          }

          #endregion
     }
}