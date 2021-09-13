

#region Using Statements
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace PixelEngine.ResourceManagement
{
     /// <summary>
     /// A resource element structure with SpriteFont class.
     /// When a font(.spritefont) file is loaded from the resource manager, 
     /// it gets stored here.
     /// </summary>
     public class GameResourceFont : GameResourceBase
     {
          #region Fields

          SpriteFont spriteFont = null;

          #endregion

          #region Properties

          public SpriteFont SpriteFont
          {
               get { return spriteFont; }
          }

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          /// <param name="key">key name</param>
          /// <param name="assetName">asset name</param>
          /// <param name="resource">sprite font resource</param>
          public GameResourceFont(string key, string assetName, SpriteFont resource)
               : base(key, assetName)
          {
               this.spriteFont = resource;

               this.resource = (object)this.spriteFont;
          }

          #endregion

          #region Disposal

          protected override void Dispose(bool disposing)
          {
               if (disposing)
               {
                    if (spriteFont != null)
                    {
                         spriteFont = null;
                    }
               }

               base.Dispose(disposing);
          }

          #endregion
     }
}