
#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace PixelEngine.Graphics
{
     /// <summary>
     /// Test if an object is occluded in the scene.
     /// </summary>
     public interface IOcclusion : ICullable, ISceneObject
     {
          #region Interface Fields

          string OcclusionModelName
          {
               get;
               set;
          }

          OcclusionQuery Query
          {
               get;
          }

          bool Occluded
          {
               get;
               set;
          }

          #endregion

          #region Interface Properties

          void DrawCulling(GameTime gameTime);

          #endregion
     }
}
