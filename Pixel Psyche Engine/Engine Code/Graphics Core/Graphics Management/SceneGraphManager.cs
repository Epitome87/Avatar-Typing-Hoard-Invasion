#region File Description
//-----------------------------------------------------------------------------
// SceneGraphManager.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Collections.Generic;
using Microsoft.Xna.Framework;
#endregion

namespace PixelEngine.Graphics
{
     public class SceneGraphManager : GameComponent
     {
          #region Static Fields

          public static List<SceneObject> sceneObjects = new List<SceneObject>();

          public static int CulledObjects = 0;
          public static int OccludedObjects = 0;

          #endregion

          #region Intialization

          public SceneGraphManager(Game game)
               : base(game)
          {
          }

          #endregion

          #region Draw

          public static void Draw(GameTime gameTime)
          {
               // Iterate and call each SceneObject's Draw method.
               foreach (SceneObject sceneObject in sceneObjects)
               {
                    sceneObject.Draw(gameTime);
               }
          }

          #endregion

          #region Public Add and Remove Methods

          public static void AddObject(SceneObject newObject)
          {
               sceneObjects.Add(newObject);
          }

          public static void RemoveObjects()
          {
               sceneObjects.Clear();
          }

          #endregion
     }
}