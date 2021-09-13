using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PixelEngine.Graphics;

namespace AvatarTyping
{
     /// <summary>
     /// A Stage is simply an environment in the game.
     /// It is different from a Level in that a Level describes the gameplay logic
     /// that will be taking place, whereas a Stage simply describes what that Level looks like.
     /// </summary>
     public abstract class Stage : DrawableGameComponent
     {
          public Vector3 QuadOrigin;
          public Vector3 QuadNormal;
          public Vector3 QuadUp;
          public float QuadWidth;
          public float QuadHeight;

          /// <summary>
          /// Stage Constructor.
          /// </summary>
          public Stage(Game game) 
               : base(game)
          {
          }

          new public virtual void LoadContent()
          {
               base.LoadContent();
          }

          public virtual void DisposeLevel()
          {
               SceneGraphManager.RemoveObjects();
          }
     }
}