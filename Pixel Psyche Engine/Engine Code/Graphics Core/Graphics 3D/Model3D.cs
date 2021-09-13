#region File Description
//-----------------------------------------------------------------------------
// Model3D.cs
// Author: Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.CameraSystem;
using PixelEngine.Screen;
#endregion

namespace PixelEngine.Graphics
{
     /// <summary>
     /// Defines a a wrapper for a Model object.
     /// </summary>
     public class Model3D
     {
          #region Fields

          // The actual Model.
          public Model model;

          // Lighting Properties.
          public Color AmbientLightColor = Color.White;
          public Color DiffuseColor = Color.White;
          public Color EmissiveColor = Color.White;
          public Color SpecularColor = Color.White;

          // Fog Properties.
          public bool IsFogEnabled = false;
          public float FogStart = -20.0f;
          public float FogEnd = 50.0f;
          public Color FogColor = Color.CornflowerBlue;
       
          #endregion

          #region Properties (Simply uses those of Model's)

          // Summary:
          //     Gets a collection of ModelBone objects which describe how each mesh in the
          //     Meshes collection for this model relates to its parent mesh. Reference page
          //     contains links to related code samples.
          public ModelBoneCollection Bones
          {
               get { return Model.Bones; }
          }
          //
          // Summary:
          //     Gets a collection of ModelMesh objects which compose the model. Each ModelMesh
          //     in a model may be moved independently and may be composed of multiple materials
          //     identified as ModelMeshPart objects. Reference page contains links to related
          //     code samples.
          public ModelMeshCollection Meshes
          {
               get { return Model.Meshes; }
          }
          //
          // Summary:
          //     Gets the root bone for this model.
          public ModelBone Root 
          {
               get { return Model.Root; }
          }
          //
          // Summary:
          //     Gets or sets an object identifying this model.
          public object Tag
          {
               get { return Model.Tag; }
               set { Model.Tag = value; }
          }

          // Summary:
          //     Copies a transform of each bone in a model relative to all parent bones of
          //     the bone into a given array. Reference page contains links to related code
          //     samples.
          //
          // Parameters:
          //   destinationBoneTransforms:
          //     The array to receive bone transforms.
          public void CopyAbsoluteBoneTransformsTo(Matrix[] destinationBoneTransforms)
          {
               Model.CopyAbsoluteBoneTransformsTo(destinationBoneTransforms);
          }
          //
          // Summary:
          //     Copies an array of transforms into each bone in the model. Reference page
          //     contains links to related conceptual articles.
          //
          // Parameters:
          //   sourceBoneTransforms:
          //     An array containing new bone transforms.
          public void CopyBoneTransformsFrom(Matrix[] sourceBoneTransforms)
          {
               Model.CopyBoneTransformsFrom(sourceBoneTransforms);
          }

          //
          // Summary:
          //     Copies each bone transform relative only to the parent bone of the model
          //     to a given array. Reference page contains links to related code samples.
          //
          // Parameters:
          //   destinationBoneTransforms:
          //     The array to receive bone transforms.
          public void CopyBoneTransformsTo(Matrix[] destinationBoneTransforms)
          {
               Model.CopyBoneTransformsTo(destinationBoneTransforms);
          }

          #endregion

          #region Properties

          public Model Model
          {
               get { return model; }
               set { model = value; }
          }

          #endregion

          #region Initialization

          public Model3D()
          {
               // Doesn't work in XNA 4.0:
               //model = new Model();

               // Change To: NO IDEA!
          }

          public void LoadContent(string assetLocation)
          {
               model = EngineCore.Game.Content.Load<Model>(assetLocation);
          }

          #endregion

          #region Public Draw Methods

          /// <summary>
          /// Draw the Model3D at the specified World Matrix.
          /// </summary>

          public void DrawModel(Matrix worldMatrix)
          {
               BoundingSphere totalBoundingSphere = new BoundingSphere();

               foreach (ModelMesh mesh in this.Model.Meshes)
               {
                    BoundingSphere boundingSphere = mesh.BoundingSphere;
                    totalBoundingSphere = BoundingSphere.CreateMerged(totalBoundingSphere, boundingSphere);
               }

               //Inside your draw method
               ContainmentType currentContainmentType = ContainmentType.Disjoint;

               //For each gameobject
               //(If you have more than one mesh in the model, this wont work. Use BoundingSphere.CreateMerged() to add them together)
               BoundingSphere meshBoundingSphere = Model.Meshes[0].BoundingSphere;
               BoundingSphere transformedBoundingSphere = totalBoundingSphere.Transform(worldMatrix);

               currentContainmentType = CameraManager.ActiveCamera.Frustum.Contains(transformedBoundingSphere);

               if (currentContainmentType != ContainmentType.Disjoint)
               {
                    // Doesn't work in XNA 4.0:
                    // Old Way #1
                    //EngineCore.GraphicsDeviceManager.GraphicsDevice.RenderState.DepthBufferEnable = true;
                    //EngineCore.GraphicsDevice.RenderState.AlphaBlendEnable = false;
                    //EngineCore.GraphicsDevice.RenderState.AlphaTestEnable = false;
                    // End Old Way #1

                    // Change #1
                    // Change AlphaBlendEnable = false To: Semi-Positive
                    EngineCore.GraphicsDevice.BlendState = BlendState.Opaque;
                    EngineCore.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                    // End Change #1

                    /* This was MY FIX for 4.0 but this article suggests I don't need it?
                    // Change DepthBufferEnable = true to this:
                    DepthStencilState depthBufferState;
                    depthBufferState = new DepthStencilState();
                    depthBufferState.DepthBufferEnable = true;
                    EngineCore.GraphicsDevice.DepthStencilState = depthBufferState;
                    */



                    // Making things appear black or not at all in XNA 4.0.
                    EngineCore.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
                    //EngineCore.GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
                    //EngineCore.GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;

                    try
                    {
                         foreach (ModelMesh mesh in this.Model.Meshes)
                         {
                              foreach (BasicEffect effect in mesh.Effects)
                              {
                                   // ADDED FOR 4.0? MIGHT FIX IT
                                  // effect.TextureEnabled = true;

                                   effect.EnableDefaultLighting();
                                   effect.PreferPerPixelLighting = true;

                                   
                                   effect.World = worldMatrix;
                                   effect.View = CameraManager.ActiveCamera.ViewMatrix;
                                   effect.Projection = CameraManager.ActiveCamera.ProjectionMatrix;

                                   // TEMP TESTING
                                   if (!AmbientLightColor.Equals(Color.White))
                                        effect.AmbientLightColor = AmbientLightColor.ToVector3();

                                   if (!DiffuseColor.Equals(Color.White))
                                        effect.DiffuseColor = DiffuseColor.ToVector3();

                                   if (!EmissiveColor.Equals(Color.White))
                                        effect.EmissiveColor = EmissiveColor.ToVector3();

                                   if (!SpecularColor.Equals(Color.White))
                                        effect.SpecularColor = SpecularColor.ToVector3();

                                   if (IsFogEnabled)
                                   {
                                        effect.FogEnabled = true;
                                        effect.FogStart = FogStart;
                                        effect.FogEnd = FogEnd;
                                        effect.FogColor = FogColor.ToVector3();
                                   }

                                   else
                                   {
                                        effect.FogEnabled = false;
                                   }
                              }

                              mesh.Draw();
                         }
                    }
                
                    catch (System.InvalidCastException)
                    { }  
                
               }
               //Loop
          }

          /// <summary>
          /// Draw the Model3D at the specified World Matrix.
          /// </summary>

          public void DrawModelAtScreen(Matrix worldMatrix)
          {
               BoundingSphere totalBoundingSphere = new BoundingSphere();

               foreach (ModelMesh mesh in this.Model.Meshes)
               {
                    BoundingSphere boundingSphere = mesh.BoundingSphere;
                    totalBoundingSphere = BoundingSphere.CreateMerged(totalBoundingSphere, boundingSphere);
               }

               //Inside your draw method
               ContainmentType currentContainmentType = ContainmentType.Disjoint;

               //For each gameobject
               //(If you have more than one mesh in the model, this wont work. Use BoundingSphere.CreateMerged() to add them together)
               BoundingSphere meshBoundingSphere = Model.Meshes[0].BoundingSphere;

               totalBoundingSphere.Transform(worldMatrix);
               meshBoundingSphere.Transform(worldMatrix);

               currentContainmentType = CameraManager.ActiveCamera.Frustum.Contains(totalBoundingSphere);

               if (currentContainmentType != ContainmentType.Disjoint)
               {
                    //Draw gameobject

                    // Doesn't work in XNA 4.0:
                    //EngineCore.GraphicsDeviceManager.GraphicsDevice.RenderState.DepthBufferEnable = true;
                    //EngineCore.GraphicsDevice.RenderState.AlphaBlendEnable = false;
                    //EngineCore.GraphicsDevice.RenderState.AlphaTestEnable = false;

                    // Change To: Semi-Positive
                    EngineCore.GraphicsDevice.BlendState = BlendState.Opaque;
                    EngineCore.GraphicsDevice.DepthStencilState = DepthStencilState.Default;


                    EngineCore.GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
                    EngineCore.GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;

                    Vector3 uh = new Vector3();

                    ScreenManager.GraphicsDevice.Viewport.Project(uh, CameraManager.ActiveCamera.ProjectionMatrix,
                         CameraManager.ActiveCamera.ViewMatrix, worldMatrix);
                    try
                    {
                         foreach (ModelMesh mesh in this.Model.Meshes)
                         {
                              foreach (BasicEffect effect in mesh.Effects)
                              {
                                   effect.EnableDefaultLighting();
                                   effect.PreferPerPixelLighting = true;
                                   effect.World = worldMatrix;
                                   effect.View = CameraManager.ActiveCamera.ViewMatrix;
                                   effect.Projection = CameraManager.ActiveCamera.ProjectionMatrix;

                                   // TEMP TESTING
                                   effect.AmbientLightColor = Color.Black.ToVector3();
                              }
                              mesh.Draw();
                         }
                    }
                    catch (System.InvalidCastException)
                    { }
               }
               //Loop
          }
          #endregion

          #region Draw with Cartoon Effects

          /// <summary>
          /// Helper for drawing the spinning model using the specified effect technique.
          /// </summary>
          public void DrawModel(Matrix world, Matrix view, Matrix projection,
                         string effectTechniqueName)
          {
               // Set suitable renderstates for drawing a 3D model.

               // Doesn't work in XNA 4.0:
               //RenderState renderState = ScreenManager.GraphicsDevice.RenderState;

               //renderState.AlphaBlendEnable = false;
               //renderState.AlphaTestEnable = false;
               //renderState.DepthBufferEnable = true;

               // Change To: ???
               EngineCore.GraphicsDevice.BlendState = BlendState.Opaque;
               EngineCore.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

               // Look up the bone transform matrices.
               Matrix[] transforms = new Matrix[Model.Bones.Count];

               Model.CopyAbsoluteBoneTransformsTo(transforms);

               // Draw the model.
               foreach (ModelMesh mesh in Model.Meshes)
               {
                    foreach (Effect effect in mesh.Effects)
                    {
                         // Specify which effect technique to use.
                         effect.CurrentTechnique = effect.Techniques[effectTechniqueName];

                         Matrix localWorld = transforms[mesh.ParentBone.Index] * world;

                         effect.Parameters["World"].SetValue(world);
                         effect.Parameters["View"].SetValue(CameraManager.ActiveCamera.ViewMatrix);
                         effect.Parameters["Projection"].SetValue(CameraManager.ActiveCamera.ProjectionMatrix);
                    }

                    mesh.Draw();
               }
          }

        

          #endregion
     }
}