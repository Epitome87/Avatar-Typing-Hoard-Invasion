
#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace PixelEngine.Graphics
{
     /// <summary>
     /// Dictates if an object is lightable in the scene.
     /// </summary>
     public interface ILightable
     {
          #region Interface Fields

          Vector3 LightDirection
          {
               get;
               set;
          }

          Vector3 LightColor
          {
               get; 
          }

          Vector3 AmbientLightColor
          {
               get;
          }

          bool LightingEnabled
          {
               get;
               set;
          }

          #endregion
     }
}