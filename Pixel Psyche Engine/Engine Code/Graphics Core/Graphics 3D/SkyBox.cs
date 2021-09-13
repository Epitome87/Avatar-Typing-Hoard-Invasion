#region File Description
//-----------------------------------------------------------------------------
// SkyBox.cs
// Matt McGrath.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace PixelEngine.Graphics
{
     /// <summary>
     /// Handles all of the aspects of working with a Skybox.
     /// </summary>
     public class Skybox : PixelEngine.Graphics.ISceneObject
     {
          #region Fields

          /// <summary>
          /// The skybox model, which will just be a cube
          /// </summary>
          private Model skyBox;

          /// <summary>
          /// The actual skybox texture
          /// </summary>
          private TextureCube skyBoxTexture;

          /// <summary>
          /// The effect file that the skybox will use to render
          /// </summary>
          private Effect skyBoxEffect;

          /// <summary>
          /// The size of the cube, used so that we can resize the box
          /// for different sized environments.
          /// </summary>
          private float size = 5f;

          #endregion

          #region Initialization

          /// <summary>
          /// Creates a new skybox
          /// </summary>
          /// <param name="skyboxTexture">the name of the skybox texture to use</param>
          public Skybox(string skyboxTexture, ContentManager Content)
          {
               skyBox = Content.Load<Model>("Skyboxes/SkyBoxModel");
               skyBoxTexture = Content.Load<TextureCube>(skyboxTexture);
               skyBoxEffect = Content.Load<Effect>("Skyboxes/Skybox");
          }

          #endregion

          #region Draw

          /// <summary>
          /// Does the actual drawing of the skybox with our skybox effect.
          /// There is no world matrix, because we're assuming the skybox won't
          /// be moved around.  The size of the skybox can be changed with the size
          /// variable.
          /// </summary>
          /// <param name="view">The view matrix for the effect</param>
          /// <param name="projection">The projection matrix for the effect</param>
          /// <param name="cameraPosition">The position of the camera</param>
          public void Draw(Matrix view, Matrix projection, Vector3 cameraPosition)
          {
               // Go through each pass in the effect, but we know there is only one...
               foreach (EffectPass pass in skyBoxEffect.CurrentTechnique.Passes)
               {
                    // Draw all of the components of the mesh, but we know the cube really
                    // only has one mesh
                    foreach (ModelMesh mesh in skyBox.Meshes)
                    {
                         // Assign the appropriate values to each of the parameters
                         foreach (ModelMeshPart part in mesh.MeshParts)
                         {
                              part.Effect = skyBoxEffect;
                              part.Effect.Parameters["World"].SetValue(
                                  Matrix.CreateScale(size) * Matrix.CreateTranslation(cameraPosition));
                              part.Effect.Parameters["View"].SetValue(view);
                              part.Effect.Parameters["Projection"].SetValue(projection);
                              part.Effect.Parameters["SkyBoxTexture"].SetValue(skyBoxTexture);
                              part.Effect.Parameters["CameraPosition"].SetValue(cameraPosition);
                         }

                         // Draw the mesh with the skybox effect
                         mesh.Draw();
                    }
               }
          }

          #endregion
     }
}
