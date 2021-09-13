#region File Description
//-----------------------------------------------------------------------------
// FortressStage.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using PixelEngine.Graphics;
#endregion

namespace AvatarTyping
{
     /// <remarks>
     /// Defines a FortressStage.
     /// 
     /// The FortressStage object simply serves as an easy means of initializing a set
     /// of SceneObjects and organizing them in a manner that forms a Fortress setting.
     /// 
     /// Like other Stage objects, it will most likely be used within a Level object to 
     /// compose the actual appearance of the Level (but not the Gameplay and logic).
     /// </remarks>
     public class FortressStage : Stage
     {
          #region Fields

          // Scene Models.
          private SceneObject ground = new SceneObject();
          private SceneObject entrance = new SceneObject();
          private SceneObject castleTowerLeft = new SceneObject();
          private SceneObject castleTowerRight = new SceneObject();
          private SceneObject gate = new SceneObject();
          private SceneObject hedge = new SceneObject();
          private SceneObject tree = new SceneObject();
          private SceneObject hauntedHouse = new SceneObject();
          private SceneObject hill = new SceneObject();

          // Keyboard Models.
          //private SceneObject keyboardBase = new SceneObject();
          //private SceneObject keyboardKeys = new SceneObject();

          // Positions of some Models.
          private Vector3 housePosition = new Vector3(3f, 0.25f, 70.0f);
          private Vector3 hillPosition = new Vector3(-0.5f, 12.5f, 120f);

          #endregion

          #region Initialization

          public FortressStage(Game game)
               : base(game)
          {
               this.QuadOrigin = Vector3.Zero;
               this.QuadNormal = Vector3.Forward;
               this.QuadUp = Vector3.Up;
               this.QuadWidth = 50;
               this.QuadHeight = 50;
          }

          public override void DisposeLevel()
          {
               base.DisposeLevel();
          }

          #endregion

          #region Load Stage's Content

          public override void LoadContent()
          {
               float zTranslation = 10f;

               SceneGraphManager.RemoveObjects();

               // Load the Hedge model.
               hedge.Model.LoadContent(@"Models\Hedge_Small");

               #region Initialize Keyboard

               // Load the Keyboard models.
               //keyboardBase.Model.LoadContent(@"Models\Keyboard_Base");
               //keyboardKeys.Model.LoadContent(@"Models\Keyboard_Keys");

               #endregion

               #region Initialize House

               hauntedHouse.Model.LoadContent(@"Models\\Graveyard_HouseFencesGraves");
               hauntedHouse.Position = housePosition;
               hauntedHouse.Rotation = new Quaternion(115f, hauntedHouse.Rotation.Y, hauntedHouse.Rotation.Z, 0f);
               hauntedHouse.Rotation = new Quaternion(hauntedHouse.Rotation.X, 90f, hauntedHouse.Rotation.Z, 0f);
               hauntedHouse.Rotation = new Quaternion(hauntedHouse.Rotation.X, hauntedHouse.Rotation.Y, -154.5f, 0f);
               hauntedHouse.World = Matrix.CreateScale(0.25f)
                    * Matrix.CreateRotationX(MathHelper.ToRadians(hauntedHouse.Rotation.X))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(hauntedHouse.Rotation.Y))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(hauntedHouse.Rotation.Z))
                    * Matrix.CreateTranslation(hauntedHouse.Position);  // X: Positive = left. Z: 

               #endregion

               #region Initialize Hill

               hill.Model.LoadContent(@"Models\\Graveyard_Hill");
               hill.Position = hillPosition;
               hill.Rotation = new Quaternion(115f, hill.Rotation.Y, hill.Rotation.Z, 0f);
               hill.Rotation = new Quaternion(hill.Rotation.X, 90f, hill.Rotation.Z, 0f);
               hill.Rotation = new Quaternion(hill.Rotation.X, hill.Rotation.Y, -154.5f, 0f);
               hill.World = Matrix.CreateScale(0.5f)
                    * Matrix.CreateRotationX(MathHelper.ToRadians(hill.Rotation.X))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(hill.Rotation.Y))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(hill.Rotation.Z))
                    * Matrix.CreateTranslation(hill.Position);//new Vector3(-0.5f, 12.5f, 120f));  // X: Positive = left. Y: Positive = Toward screen. Z: 
               hill.Model.AmbientLightColor = Color.Green;

               #endregion

               #region Initialize Heart and Sky Box.

               #endregion

               #region Initialize the Ground.

               // Load Ground Model
               ground.Model.LoadContent(@"Models\Ground");
               ground.World = Matrix.CreateScale(0.5f) * Matrix.CreateTranslation(new Vector3());
               ground.Model.AmbientLightColor = Color.Green;
               ground.Model.EmissiveColor = Color.CornflowerBlue;

               #endregion

               #region Initialize Keyboard
               /*
               keyboardBase.World = Matrix.CreateScale(0.004f)
                    * Matrix.CreateTranslation(0 - 0.5f + 0.04f, 0.05f + 0.9f, 0f + 0.2f + 0.5f)
                    * Matrix.CreateRotationY(MathHelper.ToRadians(180f))
                    * Matrix.CreateRotationX(MathHelper.ToRadians(45f));

               keyboardKeys.World = Matrix.CreateScale(0.004f)
                        * Matrix.CreateTranslation(0.04f - 0.5f + 0.03f, 0.25f + 0.9f, 0.05f + 0.2f + 0.5f)
                        * Matrix.CreateRotationY(MathHelper.ToRadians(180f))
                        * Matrix.CreateRotationX(MathHelper.ToRadians(45f));
               */
               #endregion

               #region Initialize Castle

               Color castleAmbientColor = Color.Black;
               Color castleDiffuseColor = Color.CornflowerBlue;
               Color castleEmissiveColor = Color.CornflowerBlue;
               Color castleSpecularColor = Color.CornflowerBlue;

               // Load the Environment Model.
               entrance.Model.LoadContent(@"Models\Castle\CastleEntrance");
               entrance.World =
                    Matrix.CreateScale(0.1f, 0.075f, 0.075f)
                    * Matrix.CreateTranslation(new Vector3(0f, -40f + zTranslation, 0f))
                    * Matrix.CreateRotationX(MathHelper.ToRadians(-90f));
               entrance.Model.AmbientLightColor = castleAmbientColor;
               entrance.Model.DiffuseColor = castleDiffuseColor;
               entrance.Model.EmissiveColor = castleEmissiveColor;
               entrance.Model.SpecularColor = castleSpecularColor;

               // Castle: Left Tower
               castleTowerLeft.Model.LoadContent(@"Models\Castle\CastleTowerLeft");
               castleTowerLeft.World =
                    Matrix.CreateScale(0.075f)
                    * Matrix.CreateTranslation(new Vector3(-20f, -40f + zTranslation, 0f))
                    * Matrix.CreateRotationX(MathHelper.ToRadians(-90f));
               castleTowerLeft.Model.AmbientLightColor = castleAmbientColor;
               castleTowerLeft.Model.DiffuseColor = castleDiffuseColor;
               castleTowerLeft.Model.EmissiveColor = castleEmissiveColor;
               castleTowerLeft.Model.SpecularColor = castleSpecularColor;

               // Castle: Right Tower
               castleTowerRight.Model = castleTowerLeft.Model;
               castleTowerRight.World =
                    Matrix.CreateScale(0.075f)
                    * Matrix.CreateTranslation(new Vector3(20f, -40f + zTranslation, 0f))
                    * Matrix.CreateRotationX(MathHelper.ToRadians(-90f));
               castleTowerRight.Model.AmbientLightColor = castleAmbientColor;
               castleTowerRight.Model.DiffuseColor = castleDiffuseColor;
               castleTowerRight.Model.EmissiveColor = castleEmissiveColor;
               castleTowerRight.Model.SpecularColor = castleSpecularColor;

               gate.Model.LoadContent(@"Models\Castle\CastleGate");
               gate.World =
                    Matrix.CreateScale(0.1f)
                    * Matrix.CreateTranslation(new Vector3(0f, 15f + 7.5f, 40.5f - zTranslation))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(0f))
                    * Matrix.CreateRotationX(MathHelper.ToRadians(0f));
               gate.Model.AmbientLightColor = castleAmbientColor;
               gate.Model.DiffuseColor = castleDiffuseColor;
               gate.Model.EmissiveColor = castleEmissiveColor;
               gate.Model.SpecularColor = castleSpecularColor;

               #endregion

               #region Initialize Trees

               // Initialize the Tree objects.
               tree.Model.LoadContent(@"Models\Tree");
               tree.World = Matrix.CreateScale(0.6f)
                    * Matrix.CreateTranslation(new Vector3(-10, -20, 0))
                    * Matrix.CreateRotationX(MathHelper.ToRadians(-90f));
               tree.Model.AmbientLightColor = Color.Purple;

               SceneObject anotherTree = new SceneObject();
               anotherTree.Model.LoadContent(@"Models\Tree");
               anotherTree.World = Matrix.CreateScale(0.6f)
                    * Matrix.CreateTranslation(new Vector3(10, -20, 0))
                    * Matrix.CreateRotationX(MathHelper.ToRadians(-90f));
               anotherTree.Model.AmbientLightColor = Color.Blue;


               Color[] colors = new Color[3];
               colors[0] = Color.DarkGreen;
               colors[1] = Color.DarkOrange;
               colors[2] = Color.Crimson;

               Random random = new Random();

               for (int i = 0; i < 2; i++) // 6
               {
                    SceneObject aTree = new SceneObject();
                    aTree.Model.LoadContent(@"Models\Tree");
                    aTree.World = Matrix.CreateScale(0.75f)
                         * Matrix.CreateTranslation(new Vector3(-10 + (5 * i), -60, 0))
                         * Matrix.CreateRotationX(MathHelper.ToRadians(-90f));
                    aTree.Model.AmbientLightColor = colors[random.Next(3)];
                    SceneGraphManager.AddObject(aTree);
               }
               for (int i = 0; i < 2; i++) // 6
               {
                    SceneObject aTree = new SceneObject();
                    aTree.Model.LoadContent(@"Models\Tree");
                    aTree.World = Matrix.CreateScale(0.75f)
                         * Matrix.CreateTranslation(new Vector3(10 + (5 * i), -60, 0))
                         * Matrix.CreateRotationX(MathHelper.ToRadians(-90f));
                    aTree.Model.AmbientLightColor = colors[random.Next(3)];
                    SceneGraphManager.AddObject(aTree);
               }

               for (int i = 0; i < 6; i++) // 6
               {
                    SceneObject aTree = new SceneObject();
                    aTree.Model.LoadContent(@"Models\Tree");
                    aTree.World = Matrix.CreateScale(0.5f)
                         * Matrix.CreateTranslation(new Vector3(-5 + (3 * i), -85, 0))
                         * Matrix.CreateRotationX(MathHelper.ToRadians(-90f));
                    aTree.Model.AmbientLightColor = colors[random.Next(3)];
                    SceneGraphManager.AddObject(aTree);
               }

               #endregion

               #region Add the Scene Objects to the Scene Graph

               //SceneGraphManager.AddObject(keyboardBase);
               //SceneGraphManager.AddObject(keyboardKeys);

               SceneGraphManager.AddObject(gate);
               SceneGraphManager.AddObject(entrance);
               SceneGraphManager.AddObject(castleTowerLeft);
               SceneGraphManager.AddObject(castleTowerRight);
               SceneGraphManager.AddObject(tree);
               SceneGraphManager.AddObject(anotherTree);

               for (int i = 0; i < 9; i++)
               {
                    SceneObject aHedge = new SceneObject();
                    aHedge.Model.LoadContent(@"Models\Hedge_Small");
                    aHedge.World = Matrix.CreateScale(0.075f)
                         * Matrix.CreateTranslation(new Vector3(-7.75f, 4 * i, -2.0f))
                         * Matrix.CreateRotationX(MathHelper.ToRadians(90f));

                    if (i % 2 == 0)
                         aHedge.Model.AmbientLightColor = Color.DarkGreen;

                    else
                         aHedge.Model.AmbientLightColor = Color.LightGreen;

                    SceneGraphManager.AddObject(aHedge);
               }

               for (int i = 0; i < 9; i++) // 9
               {
                    SceneObject aHedge = new SceneObject();
                    aHedge.Model.LoadContent(@"Models\Hedge_Small");
                    aHedge.World = Matrix.CreateScale(0.075f)
                         * Matrix.CreateTranslation(new Vector3(7.75f, 4 * i, -2.0f))
                         * Matrix.CreateRotationX(MathHelper.ToRadians(90f));

                    if (i % 2 == 0)
                         aHedge.Model.AmbientLightColor = Color.DarkGreen;

                    else
                         aHedge.Model.AmbientLightColor = Color.LightGreen;

                    SceneGraphManager.AddObject(aHedge);
               }

               SceneGraphManager.AddObject(hauntedHouse);
               SceneGraphManager.AddObject(hill);

               foreach (SceneObject sceneObj in SceneGraphManager.sceneObjects)
               {
                    sceneObj.Model.IsFogEnabled = true;
                    sceneObj.Model.FogStart = 0f;
                    sceneObj.Model.FogEnd = 200f;
                    sceneObj.Model.FogColor = Color.CornflowerBlue;
               }
               SceneGraphManager.AddObject(ground);

               #endregion
          }

          #endregion
     }
}