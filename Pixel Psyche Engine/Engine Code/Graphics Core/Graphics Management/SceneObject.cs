
#region Using Statements
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.CameraSystem;
using PixelEngine.Graphics;
#endregion

namespace PixelEngine.Graphics
{
     public class SceneObject : ISceneObject
     {
          #region Fields

          private bool readyToRender = false;
          private string modelName;
          private Model3D model;
          private Vector3 position = Vector3.Zero;
          private Vector3 scale = Vector3.One;
          private Quaternion rotation = Quaternion.Identity;
          private Matrix world;

          #endregion

          public bool IsToonRendered = false;

          #region Properties

          /// <summary>
          /// Is this object ready to render?
          /// </summary>
          public bool ReadyToRender
          {
               get { return readyToRender; }
               set { readyToRender = value; }
          }

          public string ModelName
          {
               get { return modelName; }
               set { modelName = value; }
          }

          public Model3D Model
          {
               get { return model; }
               set { model = value; }
          }
 
          /// <summary>
          /// The position of this object in 3d space.
          /// </summary>
          public Vector3 Position
          {
               get { return position; }
               set { position = value; }
          }
      
          public float Distance
          {
               get { return Vector3.Distance(CameraManager.ActiveCamera.Position, Position); }
          }

          /// <summary>
          /// Scale of the object.
          /// </summary>
          public Vector3 Scale
          {
               get { return scale; }
               set { scale = value; }
          }

          /// <summary>
          /// Yaw, pitch and roll of the object.
          /// </summary>
          public Quaternion Rotation
          {
               get { return rotation; }
               set { rotation = value; }
          }

          public virtual Matrix World
          {
               get
               {
                    return Matrix.CreateScale(this.Scale) *
                           Matrix.CreateFromQuaternion(this.Rotation) *
                           Matrix.CreateTranslation(this.Position);
               }

               set { world = value; }
          }

          #endregion

          #region Intialization 

          public SceneObject()
          {
               Model = new Model3D();
          }

          #endregion

          #region Draw Methods

          public void DrawCulling(GameTime gameTime)
          {
               if (this is ICullable)
               {
                    ((ICullable)this).Culled = false;

                    if (CameraManager.ActiveCamera.Frustum.Contains(((ICullable)this).BoundingSphere) == ContainmentType.Disjoint)
                    {
                         ((ICullable)this).Culled = true;
                    }
                    else
                    {
                         this.DrawCulling(gameTime);
                    }
               }
          }

          public void Draw(GameTime gameTime)
          {
               if (this is ICullable && ((ICullable)this).Culled)
               {
                    SceneGraphManager.CulledObjects++;
               }

               else if (this is IOcclusion && ((IOcclusion)this).Occluded)
               {
                    SceneGraphManager.OccludedObjects++;
               }

               else
               {
                    /*
                    if (!IsToonRendered)
                         Model.DrawModel(world);

                    else
                         Model.DrawModel(world,
                              CameraManager.ActiveCamera.ViewMatrix,
                              CameraManager.ActiveCamera.ProjectionMatrix, "Toon");
                     * */
                    Model.DrawModel(world);
               }
          }

          public void TestDraw(GameTime gameTime, string effectTechniqueName)
          {
               Model.DrawModel(world,
                              CameraManager.ActiveCamera.ViewMatrix,
                              CameraManager.ActiveCamera.ProjectionMatrix, effectTechniqueName);
          }

          /// <summary>
          /// Alters a model so it will draw using a custom effect, while preserving
          /// whatever textures were set on it as part of the original effects.
          /// </summary>
          public void ChangeEffectUsedByModel(Effect replacementEffect)
          {
               // Table mapping the original effects to our replacement versions.
               Dictionary<Effect, Effect> effectMapping = new Dictionary<Effect, Effect>();

               foreach (ModelMesh mesh in model.Meshes)
               {
                    try
                    {
                         // Scan over all the effects currently on the mesh.
                         foreach (BasicEffect oldEffect in mesh.Effects)
                         {
                              // If we haven't already seen this effect...
                              if (!effectMapping.ContainsKey(oldEffect))
                              {
                                   // Make a clone of our replacement effect. We can't just use
                                   // it directly, because the same effect might need to be
                                   // applied several times to different parts of the model using
                                   // a different texture each time, so we need a fresh copy each
                                   // time we want to set a different texture into it.

                                   // Doesn't work in XNA 4.0:
                                   //Effect newEffect = replacementEffect.Clone(replacementEffect.GraphicsDevice);

                                   // Change To: ???
                                   Effect newEffect = replacementEffect.Clone();

                                   // Copy across the texture from the original effect.
                                   newEffect.Parameters["Texture"].SetValue(oldEffect.Texture);

                                   newEffect.Parameters["TextureEnabled"].SetValue(oldEffect.TextureEnabled);

                                   effectMapping.Add(oldEffect, newEffect);
                              }
                         }
                    }
                    catch (System.InvalidCastException) { }

                    try
                    {
                         // Now that we've found all the effects in use on this mesh,
                         // update it to use our new replacement versions.
                         foreach (ModelMeshPart meshPart in mesh.MeshParts)
                         {
                              meshPart.Effect = effectMapping[meshPart.Effect];
                         }
                    }
                    catch (System.Collections.Generic.KeyNotFoundException) { }
               }
          }

          #endregion
     }
}