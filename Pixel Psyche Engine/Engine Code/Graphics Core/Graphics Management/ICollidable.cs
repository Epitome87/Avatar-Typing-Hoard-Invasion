
#region Using Statements
using System;
using Microsoft.Xna.Framework;
#endregion

namespace PixelEngine
{
     /// <summary>
     /// Dictates if an object is collidable in the scene.
     /// </summary>
     public interface ICollidable
     {
          #region Interface Fields

          /// <summary>
          /// BoundingSphere for this object.
          /// All ICollidable objects need a BoundingSphere 
          /// for simple collision detection.
          /// </summary>
          BoundingSphere BoundingSphere
          {
               get;
          }

          /// <summary>
          /// Fires when the object has collided with another.
          /// </summary>
          //event EventHandler OnCollision;

          #endregion
     }
}