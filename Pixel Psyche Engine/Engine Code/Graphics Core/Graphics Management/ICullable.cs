#region Using Statements
using PixelEngine.Graphics;
using Microsoft.Xna.Framework;
#endregion

namespace PixelEngine.Graphics
{
     public interface ICullable : ISceneObject
     {
          #region Interface Fields

          bool DrawBoundingSphere
          {
               get;
               set;
          }

          bool Culled
          {
               get;
               set;
          }

          bool BoundingSphereCreated
          {
               get;
               set;
          }

          BoundingSphere BoundingSphere
          {
               get;
          }

          #endregion
     }
}