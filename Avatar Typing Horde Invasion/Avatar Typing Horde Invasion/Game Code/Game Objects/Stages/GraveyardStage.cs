#region File Description
//-----------------------------------------------------------------------------
// GraveyardStage.cs
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
     /// Defines a GraveyardStage.
     /// 
     /// The GraveyardStage object simply serves as an easy means of initializing a set
     /// of SceneObjects and organizing them in a manner that forms a Graveyard setting.
     /// 
     /// Like other Stage objects, it will most likely be used within a Level object to 
     /// compose the actual appearance of the Level (but not the Gameplay and logic).
     /// </remarks>
     public class GraveyardStage : Stage
     {
          #region Fields

          // Scene Models.
          private SceneObject ground = new SceneObject();
          private SceneObject hauntedHouse = new SceneObject();
          private SceneObject hill = new SceneObject();
          private SceneObject gate = new SceneObject();

          Vector3 pumpkinPosition = new Vector3(-3.5f, 0f, 4.25f);
          Vector3 housePosition = new Vector3(5f, 0.25f, 85.0f); // Z = 70f, then 75f then 80
          Vector3 hillPosition = new Vector3(-0.5f, 12.5f, 120f);
          Vector3 gatePosition = new Vector3(-5f, 0f, 5f);

          Color[] graveColors = new Color[] 
          { 
               Color.Black, Color.Gray, Color.DarkBlue 
          };

          Color[] treeColors = new Color[]
          {
               Color.Black, Color.Gray, Color.White, Color.Brown, Color.DarkGreen
          };

          //private GameResourceTexture2D skyTexture;


          Random random = new Random(555);

          #endregion

          #region Initialization

          public GraveyardStage(Game game)
               : base(game)
          {
               this.QuadOrigin = Vector3.Zero;
               this.QuadNormal = Vector3.Forward;
               this.QuadUp = Vector3.Up;
               this.QuadWidth = 180;
               this.QuadHeight = 180;
          }

          public override void DisposeLevel()
          {
               base.DisposeLevel();
          }
          
          #endregion

          #region Load Stage's Content

          public override void LoadContent()
          {
               // Clear the SceneGraphManager.
               SceneGraphManager.RemoveObjects();

               #region Initialize Haunted House

               hauntedHouse.Model.LoadContent(@"Models\\Graveyard_HouseFencesGraves");
               hauntedHouse.Position = housePosition;
               hauntedHouse.Rotation = new Quaternion(115f, hauntedHouse.Rotation.Y, hauntedHouse.Rotation.Z, 0f);
               hauntedHouse.Rotation = new Quaternion(hauntedHouse.Rotation.X, 90f, hauntedHouse.Rotation.Z, 0f);
               hauntedHouse.Rotation = new Quaternion(hauntedHouse.Rotation.X, hauntedHouse.Rotation.Y, -154.5f, 0f);
               hauntedHouse.World = Matrix.CreateScale(0.5f, 0.5f * 1.2f, 0.5f * 0.80f) // 0.5f for all  X = back-to-front. Y = side-to-side. y = .5 x 1.2 
                    * Matrix.CreateRotationX(MathHelper.ToRadians(hauntedHouse.Rotation.X))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(hauntedHouse.Rotation.Y))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(hauntedHouse.Rotation.Z))
                    * Matrix.CreateTranslation(hauntedHouse.Position);

               #endregion

               #region Initialize Hill

               hill.Model.LoadContent(@"Models\\Graveyard_Hill");
               hillPosition = new Vector3(-0.5f, 12.5f, 180f); // 120
               hill.Position = hillPosition;
               hill.Rotation = new Quaternion(115f, hill.Rotation.Y, hill.Rotation.Z, 0f);
               hill.Rotation = new Quaternion(hill.Rotation.X, 90f, hill.Rotation.Z, 0f);
               hill.Rotation = new Quaternion(hill.Rotation.X, hill.Rotation.Y, -154.5f, 0f);
               hill.World = Matrix.CreateScale(new Vector3(2f, 10f, 10f)) // x = 0.5
                    * Matrix.CreateRotationX(MathHelper.ToRadians(hill.Rotation.X))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(hill.Rotation.Y))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(hill.Rotation.Z))
                    * Matrix.CreateTranslation(hill.Position);
               hill.Model.AmbientLightColor = Color.DarkGreen;

               #endregion

               #region Initialize Pumpkin

               for (int i = 0; i < 2; i++)
               {
                    SceneObject pumpkin = new SceneObject();

                    pumpkin.Model.LoadContent(@"Models\Graveyard\Pumpkin");

                    Vector3 pumpkinPosition = new Vector3(-3.75f, 0f, 4.25f - 2.5f); // Z = 4.25

                    if (i == 1)
                         pumpkinPosition = new Vector3(-4.25f, 0f, 3f);

                    else
                         pumpkinPosition = new Vector3(4.25f, 0f, 3f);

                    pumpkin.Position = pumpkinPosition;
                    pumpkin.Rotation = new Quaternion(-90f, pumpkin.Rotation.Y, pumpkin.Rotation.Z, 0f);
                    pumpkin.Rotation = new Quaternion(pumpkin.Rotation.X, 180f + 30f, pumpkin.Rotation.Z, 0f);
                    pumpkin.Rotation = new Quaternion(pumpkin.Rotation.X, pumpkin.Rotation.Y, 0f, 0f);
                    pumpkin.World = Matrix.CreateScale(0.025f)
                         * Matrix.CreateRotationX(MathHelper.ToRadians(pumpkin.Rotation.X))
                         * Matrix.CreateRotationY(MathHelper.ToRadians(pumpkin.Rotation.Y))
                         * Matrix.CreateRotationZ(MathHelper.ToRadians(pumpkin.Rotation.Z))
                         * Matrix.CreateTranslation(pumpkin.Position);
                    pumpkin.Model.AmbientLightColor = Color.Orange;
                    pumpkin.Model.EmissiveColor = Color.Yellow;

                    SceneGraphManager.AddObject(pumpkin);
               }

               #endregion

               #region Initialize Graves

               int randomNum = 0;

               // Left Graves: Column 1
               for (int i = 0; i < 10; i++)
               {
                    SceneObject grave = new SceneObject();

                    randomNum = i  % 2;

                    if (randomNum == 0)
                         grave.Model.LoadContent(@"Models\Graveyard\Grave_Tall");

                    else
                         grave.Model.LoadContent(@"Models\Graveyard\Grave_Cross");

                    float randomScale = 2f * (float)random.NextDouble(); // Returns 0-2
                    randomScale = MathHelper.Clamp(randomScale, 0.90f, 1.5f);

                    Vector3 gravePosition = new Vector3(5f, -0.2f, 7.5f + i * 5); // 10f + i * 2

                    if (randomNum == 0)
                    {
                         grave.Position = gravePosition;
                         grave.Rotation = new Quaternion(-90f, grave.Rotation.Y, grave.Rotation.Z, 0f);
                         grave.Rotation = new Quaternion(grave.Rotation.X, 180f, grave.Rotation.Z, 0f); //90
                         grave.Rotation = new Quaternion(grave.Rotation.X, grave.Rotation.Y, 0f, 0f); // 0 for Cross
                         grave.World = Matrix.CreateScale(0.01f * randomScale)
                              * Matrix.CreateRotationX(MathHelper.ToRadians(grave.Rotation.X))
                              * Matrix.CreateRotationY(MathHelper.ToRadians(grave.Rotation.Y))
                              * Matrix.CreateRotationZ(MathHelper.ToRadians(grave.Rotation.Z))
                              * Matrix.CreateTranslation(grave.Position);
                    }

                    else
                    {
                         grave.Position = new Vector3(gravePosition.X, gravePosition.Y - 1.9f, gravePosition.Z - 2.5f);
                         grave.Rotation = new Quaternion(-90f, grave.Rotation.Y, grave.Rotation.Z, 0f);
                         grave.Rotation = new Quaternion(grave.Rotation.X, 90f, grave.Rotation.Z, 0f);
                         grave.Rotation = new Quaternion(grave.Rotation.X, grave.Rotation.Y, 0f, 0f);
                         grave.World = Matrix.CreateScale(0.75f)
                              * Matrix.CreateRotationX(MathHelper.ToRadians(grave.Rotation.X))
                              * Matrix.CreateRotationY(MathHelper.ToRadians(grave.Rotation.Y))
                              * Matrix.CreateRotationZ(MathHelper.ToRadians(grave.Rotation.Z))
                              * Matrix.CreateTranslation(grave.Position);
                    }

                    grave.Model.AmbientLightColor = graveColors[random.Next(graveColors.Length)];

                    SceneGraphManager.AddObject(grave);
               }

               // Left Graves: Column 2
               for (int i = 0; i < 10; i++)
               {
                    SceneObject grave = new SceneObject();

                    randomNum = i % 2;

                    if (randomNum != 0)
                         grave.Model.LoadContent(@"Models\Graveyard\Grave_Tall");

                    else
                         grave.Model.LoadContent(@"Models\Graveyard\Grave_Cross");

                    float randomScale = 2f * (float)random.NextDouble(); // Returns 0-1.5
                    randomScale = MathHelper.Clamp(randomScale, 0.90f, 1.5f);

                    Vector3 gravePosition = new Vector3(7.5f, -0.2f, 7.5f + i * 5);

                    if (randomNum != 0)
                    {
                         grave.Position = gravePosition;
                         grave.Rotation = new Quaternion(-90f, grave.Rotation.Y, grave.Rotation.Z, 0f);
                         grave.Rotation = new Quaternion(grave.Rotation.X, 180f, grave.Rotation.Z, 0f); //90
                         grave.Rotation = new Quaternion(grave.Rotation.X, grave.Rotation.Y, 0f, 0f); // 0 for Cross
                         grave.World = Matrix.CreateScale(0.01f * randomScale)
                              * Matrix.CreateRotationX(MathHelper.ToRadians(grave.Rotation.X))
                              * Matrix.CreateRotationY(MathHelper.ToRadians(grave.Rotation.Y))
                              * Matrix.CreateRotationZ(MathHelper.ToRadians(grave.Rotation.Z))
                              * Matrix.CreateTranslation(grave.Position);
                    }

                    else
                    {
                         grave.Position = new Vector3(gravePosition.X, gravePosition.Y - 1.9f, gravePosition.Z - 2f);
                         grave.Rotation = new Quaternion(-90f, grave.Rotation.Y, grave.Rotation.Z, 0f);
                         grave.Rotation = new Quaternion(grave.Rotation.X, 90f, grave.Rotation.Z, 0f);
                         grave.Rotation = new Quaternion(grave.Rotation.X, grave.Rotation.Y, 0f, 0f);
                         grave.World = Matrix.CreateScale(0.75f)
                              * Matrix.CreateRotationX(MathHelper.ToRadians(grave.Rotation.X))
                              * Matrix.CreateRotationY(MathHelper.ToRadians(grave.Rotation.Y))
                              * Matrix.CreateRotationZ(MathHelper.ToRadians(grave.Rotation.Z))
                              * Matrix.CreateTranslation(grave.Position);
                    }

                    grave.Model.AmbientLightColor = graveColors[random.Next(graveColors.Length)];

                    SceneGraphManager.AddObject(grave);
               }

               // Left Graves: Column 3
               for (int i = 0; i < 10; i++)
               {
                    SceneObject grave = new SceneObject();

                    randomNum = i % 2;

                    if (randomNum == 0)
                         grave.Model.LoadContent(@"Models\Graveyard\Grave_Tall");

                    else
                         grave.Model.LoadContent(@"Models\Graveyard\Grave_Cross");

                    float randomScale = 2.0f * (float)random.NextDouble(); // Returns 0-2
                    randomScale = MathHelper.Clamp(randomScale, 0.90f, 1.50f);

                    Vector3 gravePosition = new Vector3(10f, -0.2f, 7.5f + i * 5);

                    if (randomNum == 0)
                    {
                         grave.Position = gravePosition;
                         grave.Rotation = new Quaternion(-90f, grave.Rotation.Y, grave.Rotation.Z, 0f);
                         grave.Rotation = new Quaternion(grave.Rotation.X, 180f, grave.Rotation.Z, 0f); //90
                         grave.Rotation = new Quaternion(grave.Rotation.X, grave.Rotation.Y, 0f, 0f); // 0 for Cross
                         grave.World = Matrix.CreateScale(0.01f * randomScale)
                              * Matrix.CreateRotationX(MathHelper.ToRadians(grave.Rotation.X))
                              * Matrix.CreateRotationY(MathHelper.ToRadians(grave.Rotation.Y))
                              * Matrix.CreateRotationZ(MathHelper.ToRadians(grave.Rotation.Z))
                              * Matrix.CreateTranslation(grave.Position);
                    }

                    else
                    {
                         grave.Position = new Vector3(gravePosition.X, gravePosition.Y - 1.9f, gravePosition.Z - 2f);
                         grave.Rotation = new Quaternion(-90f, grave.Rotation.Y, grave.Rotation.Z, 0f);
                         grave.Rotation = new Quaternion(grave.Rotation.X, 90f, grave.Rotation.Z, 0f);
                         grave.Rotation = new Quaternion(grave.Rotation.X, grave.Rotation.Y, 0f, 0f);
                         grave.World = Matrix.CreateScale(0.75f)
                              * Matrix.CreateRotationX(MathHelper.ToRadians(grave.Rotation.X))
                              * Matrix.CreateRotationY(MathHelper.ToRadians(grave.Rotation.Y))
                              * Matrix.CreateRotationZ(MathHelper.ToRadians(grave.Rotation.Z))
                              * Matrix.CreateTranslation(grave.Position);
                    }

                    grave.Model.AmbientLightColor = graveColors[random.Next(graveColors.Length)];

                    SceneGraphManager.AddObject(grave);
               }

               // Right Graves: Column 1
               for (int i = 0; i < 10; i++)
               {
                    SceneObject grave = new SceneObject();

                    randomNum = i % 2;

                    if (randomNum == 0)
                         grave.Model.LoadContent(@"Models\Graveyard\Grave_Tall");

                    else
                         grave.Model.LoadContent(@"Models\Graveyard\Grave_Cross");

                    float randomScale = 2f * (float)random.NextDouble(); // Returns 0-1.5
                    randomScale = MathHelper.Clamp(randomScale, 0.90f, 1.5f);

                    Vector3 gravePosition = new Vector3(-5f, -0.2f, 7.5f + i * 5);

                    if (randomNum == 0)
                    {
                         grave.Position = gravePosition;
                         grave.Rotation = new Quaternion(-90f, grave.Rotation.Y, grave.Rotation.Z, 0f);
                         grave.Rotation = new Quaternion(grave.Rotation.X, 180f, grave.Rotation.Z, 0f); //90
                         grave.Rotation = new Quaternion(grave.Rotation.X, grave.Rotation.Y, 0f, 0f); // 0 for Cross
                         grave.World = Matrix.CreateScale(0.01f * randomScale)
                              * Matrix.CreateRotationX(MathHelper.ToRadians(grave.Rotation.X))
                              * Matrix.CreateRotationY(MathHelper.ToRadians(grave.Rotation.Y))
                              * Matrix.CreateRotationZ(MathHelper.ToRadians(grave.Rotation.Z))
                              * Matrix.CreateTranslation(grave.Position);
                    }

                    else
                    {
                         grave.Position = new Vector3(gravePosition.X, gravePosition.Y - 1.9f, gravePosition.Z - 2f);
                         grave.Rotation = new Quaternion(-90f, grave.Rotation.Y, grave.Rotation.Z, 0f);
                         grave.Rotation = new Quaternion(grave.Rotation.X, 90f, grave.Rotation.Z, 0f);
                         grave.Rotation = new Quaternion(grave.Rotation.X, grave.Rotation.Y, 0f, 0f);
                         grave.World = Matrix.CreateScale(0.75f)
                              * Matrix.CreateRotationX(MathHelper.ToRadians(grave.Rotation.X))
                              * Matrix.CreateRotationY(MathHelper.ToRadians(grave.Rotation.Y))
                              * Matrix.CreateRotationZ(MathHelper.ToRadians(grave.Rotation.Z))
                              * Matrix.CreateTranslation(grave.Position);
                    }

                    grave.Model.AmbientLightColor = graveColors[random.Next(graveColors.Length)];

                    SceneGraphManager.AddObject(grave);
               }

               // Right Graves: Column 2
               for (int i = 0; i < 10; i++)
               {
                    SceneObject grave = new SceneObject();

                    randomNum = i % 2;

                    if (randomNum != 0)
                         grave.Model.LoadContent(@"Models\Graveyard\Grave_Tall");

                    else
                         grave.Model.LoadContent(@"Models\Graveyard\Grave_Cross");

                    float randomScale = 2f * (float)random.NextDouble(); // Returns 0-1.5
                    randomScale = MathHelper.Clamp(randomScale, 0.90f, 1.5f);

                    Vector3 gravePosition = new Vector3(-7.5f, -0.2f, 7.5f + i * 5);

                    if (randomNum != 0)
                    {
                         grave.Position = gravePosition;
                         grave.Rotation = new Quaternion(-90f, grave.Rotation.Y, grave.Rotation.Z, 0f);
                         grave.Rotation = new Quaternion(grave.Rotation.X, 180f, grave.Rotation.Z, 0f); //90
                         grave.Rotation = new Quaternion(grave.Rotation.X, grave.Rotation.Y, 0f, 0f); // 0 for Cross
                         grave.World = Matrix.CreateScale(0.01f * randomScale)
                              * Matrix.CreateRotationX(MathHelper.ToRadians(grave.Rotation.X))
                              * Matrix.CreateRotationY(MathHelper.ToRadians(grave.Rotation.Y))
                              * Matrix.CreateRotationZ(MathHelper.ToRadians(grave.Rotation.Z))
                              * Matrix.CreateTranslation(grave.Position);
                    }

                    else
                    {
                         grave.Position = new Vector3(gravePosition.X, gravePosition.Y - 1.9f, gravePosition.Z - 2f);
                         grave.Rotation = new Quaternion(-90f, grave.Rotation.Y, grave.Rotation.Z, 0f);
                         grave.Rotation = new Quaternion(grave.Rotation.X, 90f, grave.Rotation.Z, 0f);
                         grave.Rotation = new Quaternion(grave.Rotation.X, grave.Rotation.Y, 0f, 0f);
                         grave.World = Matrix.CreateScale(0.75f)
                              * Matrix.CreateRotationX(MathHelper.ToRadians(grave.Rotation.X))
                              * Matrix.CreateRotationY(MathHelper.ToRadians(grave.Rotation.Y))
                              * Matrix.CreateRotationZ(MathHelper.ToRadians(grave.Rotation.Z))
                              * Matrix.CreateTranslation(grave.Position);
                    }

                    grave.Model.AmbientLightColor = graveColors[random.Next(graveColors.Length)];

                    SceneGraphManager.AddObject(grave);
               }

               // Right Graves: Column 3
               for (int i = 0; i < 10; i++)
               {
                    SceneObject grave = new SceneObject();

                    randomNum = i % 2;

                    if (randomNum == 0)
                         grave.Model.LoadContent(@"Models\Graveyard\Grave_Tall");

                    else
                         grave.Model.LoadContent(@"Models\Graveyard\Grave_Cross");

                    float randomScale = 2f * (float)random.NextDouble(); // Returns 0-1.5
                    randomScale = MathHelper.Clamp(randomScale, 0.90f, 1.5f);

                    Vector3 gravePosition = new Vector3(-10.0f, -0.2f, 7.5f + i * 5);

                    if (randomNum == 0)
                    {
                         grave.Position = gravePosition;
                         grave.Rotation = new Quaternion(-90f, grave.Rotation.Y, grave.Rotation.Z, 0f);
                         grave.Rotation = new Quaternion(grave.Rotation.X, 180f, grave.Rotation.Z, 0f); //90
                         grave.Rotation = new Quaternion(grave.Rotation.X, grave.Rotation.Y, 0f, 0f); // 0 for Cross
                         grave.World = Matrix.CreateScale(0.01f * randomScale)
                              * Matrix.CreateRotationX(MathHelper.ToRadians(grave.Rotation.X))
                              * Matrix.CreateRotationY(MathHelper.ToRadians(grave.Rotation.Y))
                              * Matrix.CreateRotationZ(MathHelper.ToRadians(grave.Rotation.Z))
                              * Matrix.CreateTranslation(grave.Position);
                    }

                    else
                    {
                         grave.Position = new Vector3(gravePosition.X, gravePosition.Y - 1.9f, gravePosition.Z - 2f);
                         grave.Rotation = new Quaternion(-90f, grave.Rotation.Y, grave.Rotation.Z, 0f);
                         grave.Rotation = new Quaternion(grave.Rotation.X, 90f, grave.Rotation.Z, 0f);
                         grave.Rotation = new Quaternion(grave.Rotation.X, grave.Rotation.Y, 0f, 0f);
                         grave.World = Matrix.CreateScale(0.75f)
                              * Matrix.CreateRotationX(MathHelper.ToRadians(grave.Rotation.X))
                              * Matrix.CreateRotationY(MathHelper.ToRadians(grave.Rotation.Y))
                              * Matrix.CreateRotationZ(MathHelper.ToRadians(grave.Rotation.Z))
                              * Matrix.CreateTranslation(grave.Position);
                    }

                    grave.Model.AmbientLightColor = graveColors[random.Next(graveColors.Length)];

                    SceneGraphManager.AddObject(grave);
               }

               #endregion

               #region Initialize Gates

               for (int i = 0; i < 2; i++)
               {
                    SceneObject smallGate = new SceneObject();

                    smallGate.Model.LoadContent(@"Models\Graveyard\Graveyard_SmallGate");

                    Vector3 smallGatePosition;

                    if (i == 0)
                    {
                         smallGatePosition = new Vector3(-5f - 0.5f, 0f, 4.25f); // Z = 5f
                    }

                    else
                    {
                         smallGatePosition = new Vector3(18.5f + 0.5f, 0f, 4.25f); // Z = 5f and then 3.5
                    }

                    smallGate.Position = smallGatePosition;
                    smallGate.Rotation = new Quaternion(0f, smallGate.Rotation.Y, smallGate.Rotation.Z, 0f);
                    smallGate.Rotation = new Quaternion(smallGate.Rotation.X, 0f, smallGate.Rotation.Z, 0f);
                    smallGate.Rotation = new Quaternion(smallGate.Rotation.X, smallGate.Rotation.Y, 0f, 0f);
                    smallGate.World = Matrix.CreateScale(0.025f, 0.025f * 1.25f, 0.025f) // 0.025f   Z = Actual Z
                         * Matrix.CreateRotationX(MathHelper.ToRadians(smallGate.Rotation.X))
                         * Matrix.CreateRotationY(MathHelper.ToRadians(smallGate.Rotation.Y))
                         * Matrix.CreateRotationZ(MathHelper.ToRadians(smallGate.Rotation.Z))
                         * Matrix.CreateTranslation(smallGate.Position);
                    SceneGraphManager.AddObject(smallGate);
               }
               #endregion

               #region Initialize Trees

               // Left Trees.
               for (int i = 0; i < 5; i++)
               {
                    SceneObject tree = new SceneObject();
                    tree.Model.LoadContent(@"Models\Graveyard\Tree_Dead");

                    Vector3 treePosition = new Vector3(14f, 0f, 8f + 8f * i);

                    float randomScale = 2f * (float)random.NextDouble(); // Returns 0-2
                    randomScale = MathHelper.Clamp(randomScale, 0.90f, 1.5f);

                    float randomRotation = random.Next(180);

                    tree.Position = treePosition;
                    tree.Rotation = new Quaternion(-90f, tree.Rotation.Y, tree.Rotation.Z, 0f);
                    tree.Rotation = new Quaternion(tree.Rotation.X, randomRotation, tree.Rotation.Z, 0f); //90
                    tree.Rotation = new Quaternion(tree.Rotation.X, tree.Rotation.Y, 0f, 0f); // 0 for Cross
                    tree.World = Matrix.CreateScale(0.1f * randomScale)
                         * Matrix.CreateRotationX(MathHelper.ToRadians(tree.Rotation.X))
                         * Matrix.CreateRotationY(MathHelper.ToRadians(tree.Rotation.Y))
                         * Matrix.CreateRotationZ(MathHelper.ToRadians(tree.Rotation.Z))
                         * Matrix.CreateTranslation(tree.Position);

                    tree.Model.AmbientLightColor = treeColors[random.Next(treeColors.Length)];

                    SceneGraphManager.AddObject(tree);
               }

               // Right Trees.
               for (int i = 0; i < 5; i++)
               {
                    SceneObject tree = new SceneObject();
                    tree.Model.LoadContent(@"Models\Graveyard\Tree_Dead");

                    Vector3 treePosition = new Vector3(-14f, 0f, 8f + 8f * i);

                    float randomScale = 2f * (float)random.NextDouble(); // Returns 0-2
                    randomScale = MathHelper.Clamp(randomScale, 0.90f, 1.5f);

                    float randomRotation = random.Next(180);

                    tree.Position = treePosition;
                    tree.Rotation = new Quaternion(-90f, tree.Rotation.Y, tree.Rotation.Z, 0f);
                    tree.Rotation = new Quaternion(tree.Rotation.X, randomRotation, tree.Rotation.Z, 0f); //90
                    tree.Rotation = new Quaternion(tree.Rotation.X, tree.Rotation.Y, 0f, 0f); // 0 for Cross
                    tree.World = Matrix.CreateScale(0.1f * randomScale)
                         * Matrix.CreateRotationX(MathHelper.ToRadians(tree.Rotation.X))
                         * Matrix.CreateRotationY(MathHelper.ToRadians(tree.Rotation.Y))
                         * Matrix.CreateRotationZ(MathHelper.ToRadians(tree.Rotation.Z))
                         * Matrix.CreateTranslation(tree.Position);

                    tree.Model.AmbientLightColor = treeColors[random.Next(treeColors.Length)];

                    SceneGraphManager.AddObject(tree);
               }

               // Further Left Trees.
               for (int i = 0; i < 10; i++)
               {
                    SceneObject tree = new SceneObject();
                    tree.Model.LoadContent(@"Models\Graveyard\Tree_Dead");

                    Vector3 treePosition = new Vector3(22f, 0f, 10 + 8f * i);

                    float randomScale = 2f * (float)random.NextDouble(); // Returns 0-2
                    randomScale = MathHelper.Clamp(randomScale, 0.90f, 1.5f);

                    float randomRotation = random.Next(180);

                    tree.Position = treePosition;
                    tree.Rotation = new Quaternion(-90f, tree.Rotation.Y, tree.Rotation.Z, 0f);
                    tree.Rotation = new Quaternion(tree.Rotation.X, randomRotation, tree.Rotation.Z, 0f);
                    tree.Rotation = new Quaternion(tree.Rotation.X, tree.Rotation.Y, 0f, 0f);
                    tree.World = Matrix.CreateScale(0.1f * randomScale)
                         * Matrix.CreateRotationX(MathHelper.ToRadians(tree.Rotation.X))
                         * Matrix.CreateRotationY(MathHelper.ToRadians(tree.Rotation.Y))
                         * Matrix.CreateRotationZ(MathHelper.ToRadians(tree.Rotation.Z))
                         * Matrix.CreateTranslation(tree.Position);

                    tree.Model.AmbientLightColor = treeColors[random.Next(treeColors.Length)];

                    SceneGraphManager.AddObject(tree);
               }

               // Further Right Trees.
               for (int i = 0; i < 10; i++)
               {
                    SceneObject tree = new SceneObject();
                    tree.Model.LoadContent(@"Models\Graveyard\Tree_Dead");

                    Vector3 treePosition = new Vector3(-22f, 0f, 10 + 8f * i);

                    float randomScale = 2f * (float)random.NextDouble(); // Returns 0-2
                    randomScale = MathHelper.Clamp(randomScale, 0.90f, 1.5f);

                    float randomRotation = random.Next(180);

                    tree.Position = treePosition;
                    tree.Rotation = new Quaternion(-90f, tree.Rotation.Y, tree.Rotation.Z, 0f);
                    tree.Rotation = new Quaternion(tree.Rotation.X, randomRotation, tree.Rotation.Z, 0f);
                    tree.Rotation = new Quaternion(tree.Rotation.X, tree.Rotation.Y, 0f, 0f);
                    tree.World = Matrix.CreateScale(0.1f * randomScale)
                         * Matrix.CreateRotationX(MathHelper.ToRadians(tree.Rotation.X))
                         * Matrix.CreateRotationY(MathHelper.ToRadians(tree.Rotation.Y))
                         * Matrix.CreateRotationZ(MathHelper.ToRadians(tree.Rotation.Z))
                         * Matrix.CreateTranslation(tree.Position);

                    tree.Model.AmbientLightColor = treeColors[random.Next(treeColors.Length)];

                    SceneGraphManager.AddObject(tree);
               }

               // Furthest Left Trees.
               for (int i = 0; i < 10; i++)
               {
                    SceneObject tree = new SceneObject();
                    tree.Model.LoadContent(@"Models\Graveyard\Tree_Dead");

                    Vector3 treePosition = new Vector3(31f, 0f, 20 + 8f * i);

                    float randomScale = 2f * (float)random.NextDouble(); // Returns 0-2
                    randomScale = MathHelper.Clamp(randomScale, 0.90f, 1.5f);

                    float randomRotation = random.Next(180);

                    tree.Position = treePosition;
                    tree.Rotation = new Quaternion(-90f, tree.Rotation.Y, tree.Rotation.Z, 0f);
                    tree.Rotation = new Quaternion(tree.Rotation.X, randomRotation, tree.Rotation.Z, 0f);
                    tree.Rotation = new Quaternion(tree.Rotation.X, tree.Rotation.Y, 0f, 0f);
                    tree.World = Matrix.CreateScale(0.1f * randomScale)
                         * Matrix.CreateRotationX(MathHelper.ToRadians(tree.Rotation.X))
                         * Matrix.CreateRotationY(MathHelper.ToRadians(tree.Rotation.Y))
                         * Matrix.CreateRotationZ(MathHelper.ToRadians(tree.Rotation.Z))
                         * Matrix.CreateTranslation(tree.Position);

                    tree.Model.AmbientLightColor = treeColors[random.Next(treeColors.Length)];

                    SceneGraphManager.AddObject(tree);
               }

               // Furthest Right Trees.
               for (int i = 0; i < 10; i++)
               {
                    SceneObject tree = new SceneObject();
                    tree.Model.LoadContent(@"Models\Graveyard\Tree_Dead");

                    Vector3 treePosition = new Vector3(-31f, 0f, 20 + 8f * i);

                    float randomScale = 2f * (float)random.NextDouble(); // Returns 0-2
                    randomScale = MathHelper.Clamp(randomScale, 0.90f, 1.5f);

                    float randomRotation = random.Next(180);

                    tree.Position = treePosition;
                    tree.Rotation = new Quaternion(-90f, tree.Rotation.Y, tree.Rotation.Z, 0f);
                    tree.Rotation = new Quaternion(tree.Rotation.X, randomRotation, tree.Rotation.Z, 0f);
                    tree.Rotation = new Quaternion(tree.Rotation.X, tree.Rotation.Y, 0f, 0f);
                    tree.World = Matrix.CreateScale(0.1f * randomScale)
                         * Matrix.CreateRotationX(MathHelper.ToRadians(tree.Rotation.X))
                         * Matrix.CreateRotationY(MathHelper.ToRadians(tree.Rotation.Y))
                         * Matrix.CreateRotationZ(MathHelper.ToRadians(tree.Rotation.Z))
                         * Matrix.CreateTranslation(tree.Position);

                    tree.Model.AmbientLightColor = treeColors[random.Next(treeColors.Length)];

                    SceneGraphManager.AddObject(tree);
               }

               #endregion

               #region Initialize Sky.

               //skyTexture = ResourceManager.LoadTexture(@"Textures\Sky_Night");

               #endregion

               #region Initialize the Ground.

               // Load Ground Model
               ground.Model.LoadContent(@"Models\Ground");
               ground.World = Matrix.CreateScale(0.5f * 2f) * Matrix.CreateTranslation(new Vector3()); // Scale = 0.5f
               ground.Model.AmbientLightColor = Color.Black;

               #endregion

               #region Initialize Keyboard - Possibly put in Level

               // Load the Keyboard models.
               //keyboardBase.Model.LoadContent(@"Models\Keyboard_Base");
               //keyboardKeys.Model.LoadContent(@"Models\Keyboard_Keys");

               /*
               keyboardBase.Position = new Vector3(0.5f, 0.5f, 0.5f);
               keyboardBase.Rotation = new Quaternion(-90f, keyboardBase.Rotation.Y, keyboardBase.Rotation.Z, 0f);
               keyboardBase.Rotation = new Quaternion(keyboardBase.Rotation.X, 0f, keyboardBase.Rotation.Z, 0f);
               keyboardBase.Rotation = new Quaternion(keyboardBase.Rotation.X, keyboardBase.Rotation.Y, 0f, 0f);
               keyboardBase.World = Matrix.CreateScale(0.5f)
                    * Matrix.CreateRotationX(MathHelper.ToRadians(keyboardBase.Rotation.X))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(keyboardBase.Rotation.Y))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(keyboardBase.Rotation.Z))
                    * Matrix.CreateTranslation(keyboardBase.Position);

               
               keyboardKeys.World = Matrix.CreateScale(0.004f)
                        * Matrix.CreateTranslation(0.04f - 0.5f + 0.03f, 0.25f + 0.9f, 0.05f + 0.2f + 0.5f)
                        * Matrix.CreateRotationY(MathHelper.ToRadians(180f))
                        * Matrix.CreateRotationX(MathHelper.ToRadians(45f));
               */
               #endregion

               #region Add the Scene Objects to the Scene Graph

               // Affected by Fog.
               //SceneGraphManager.AddObject(keyboardBase);
               //SceneGraphManager.AddObject(keyboardKeys);
               SceneGraphManager.AddObject(hauntedHouse);
               SceneGraphManager.AddObject(hill);
               
               foreach (SceneObject sceneObj in SceneGraphManager.sceneObjects)
               {
                    sceneObj.Model.IsFogEnabled = true;
                    sceneObj.Model.FogStart = 0f;
                    sceneObj.Model.FogEnd = 400f;
                    sceneObj.Model.FogColor = Color.CornflowerBlue;
               }

               // We don't want affected by Fog.
               SceneGraphManager.AddObject(ground);

               #endregion
          }

          #endregion
     }
}