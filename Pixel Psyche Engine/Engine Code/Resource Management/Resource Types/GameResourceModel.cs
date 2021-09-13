
#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.CameraSystem;
#endregion

namespace PixelEngine.ResourceManagement
{
     /// <summary>
     /// Saves the transform matrix of the read-in model class and the bones of 
     /// the model’s initial state.
     /// </summary>
     public class ModelData
     {
          public Model model = null;
          public Matrix[] boneTransforms = null;
     }

     public class GameResourceModel : GameResourceBase
     {
          #region Fields

          ModelData modelData = new ModelData();

          #endregion

          #region Properties

          public ModelData ModelData
          {
               get { return modelData; }
          }

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          /// <param name="key">key name</param>
          /// <param name="assetName">asset name</param>
          /// <param name="resource">model resource</param>
          public GameResourceModel(string key, string assetName, Model resource)
               : base(key, assetName)
          {
               this.modelData.model = resource;

               this.modelData.boneTransforms = new Matrix[resource.Bones.Count];
               this.modelData.model.CopyBoneTransformsTo(this.modelData.boneTransforms);

               this.resource = (object)this.modelData;
          }

          #endregion

          #region Disposal

          protected override void Dispose(bool disposing)
          {
               if (disposing)
               {
                    if (modelData != null)
                    {
                         modelData.model = null;
                         modelData = null;
                    }
               }

               base.Dispose(disposing);
          }

          #endregion

          #region Public Draw Methods

          /// <summary>
          /// Draw the Model3D at the specified World Matrix.
          /// </summary>

          public void DrawModel(Matrix worldMatrix)
          {
               BoundingSphere totalBoundingSphere = new BoundingSphere();

               foreach (ModelMesh mesh in this.modelData.model.Meshes)
               {
                    BoundingSphere boundingSphere = mesh.BoundingSphere;
                    totalBoundingSphere = BoundingSphere.CreateMerged(totalBoundingSphere, boundingSphere);
               }

               //Inside your draw method
               ContainmentType currentContainmentType = ContainmentType.Disjoint;

               //For each gameobject
               //(If you have more than one mesh in the model, this wont work. Use BoundingSphere.CreateMerged() to add them together)
               BoundingSphere meshBoundingSphere = this.modelData.model.Meshes[0].BoundingSphere;

               totalBoundingSphere.Transform(worldMatrix);
               meshBoundingSphere.Transform(worldMatrix);

               currentContainmentType = CameraManager.ActiveCamera.Frustum.Contains(totalBoundingSphere);

               if (currentContainmentType != ContainmentType.Disjoint)
               {
                    //Draw gameobject
  
                    // No more RenderState in XNA 4.0:
                    
                    //EngineCore.GraphicsDevice.RenderState.AlphaBlendEnable = false;
                    //EngineCore.GraphicsDevice.RenderState.AlphaTestEnable = false;
                    //EngineCore.GraphicsDeviceManager.GraphicsDevice.RenderState.DepthBufferEnable = true;
                    
                    // Change to ??? (Not sure if this is correct):
                    EngineCore.GraphicsDevice.BlendState = BlendState.Opaque;
                    EngineCore.GraphicsDevice.DepthStencilState = DepthStencilState.Default;


                    EngineCore.GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
                    EngineCore.GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;

                    try
                    {
                         foreach (ModelMesh mesh in this.modelData.model.Meshes)
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
     }
}