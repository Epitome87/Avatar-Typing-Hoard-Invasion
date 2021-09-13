#region File Description
//-----------------------------------------------------------------------------
// GameplayBackgroundScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine;
using PixelEngine.CameraSystem;
using PixelEngine.Graphics;
#endregion

namespace AvatarTyping
{
     /// <remarks>
     /// A BackgroundScreen for the Start / Main-Menu in Avatar Typing.
     /// This is a BackgroundScreen to prevent re-loading assets and losing
     /// the state of the camera angles / enemies.
     /// </remarks>
     public class GameplayBackgroundScreen : BackgroundScreen
     {
          #region Fields

          //private Skybox skybox;
          private Texture2D skyTexture;
          private Model3D groundModel = new Model3D();
          private Model3D entranceModel = new Model3D();
          private Model3D castleTowersModel = new Model3D();
          private Model3D gateModel = new Model3D();

          //private CinematicEvent triangularZoomEvent;
          private EnemyManager enemyManager;

          Random random = new Random();
          Vector3 position = new Vector3(0);
          int currentEnemy = 0;

          public static bool isUpdate = true;
          
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

          Vector3 housePosition = new Vector3(5f, 0.25f, 85.0f);
          Vector3 hillPosition = new Vector3(-0.5f, 12.5f, 120f);

          Vector3 pumpkinPosition = new Vector3(-3.5f, 0f, 4.25f);
          Vector3 graveTallPosition = new Vector3(2f, 0f, 5f);
          Vector3 gatePosition = new Vector3(-5f, 0f, 5f);

          Color[] graveColors = new Color[] 
          { 
               Color.Black, Color.Gray, Color.DarkBlue 
          };

          Color[] treeColors = new Color[]
          {
               Color.Black, Color.Gray, Color.White, Color.Brown, Color.DarkGreen
          };

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public GameplayBackgroundScreen()
          {
               TransitionOnTime = TimeSpan.FromSeconds(0.5);
               TransitionOffTime = TimeSpan.FromSeconds(0.5);  
          }

          Quad quad;
          VertexDeclaration quadVertexDecl;
          BasicEffect quadEffect;

          public override void LoadContent()
          {
               quad = new Quad(Vector3.Zero, Vector3.Backward, Vector3.Up, 50, 50);

               skyTexture = PixelEngine.EngineCore.Game.Content.Load<Texture2D>(@"Textures\Sky_Night");

               // Doesn't work in XNA 4.0:
               quadEffect = new BasicEffect(EngineCore.GraphicsDevice);

               quadEffect.EnableDefaultLighting();

               quadEffect.World = Matrix.CreateTranslation(new Vector3(0f, 100f, 0f))
                    * Matrix.CreateScale(0.025f);
               quadEffect.View = CameraManager.ActiveCamera.ViewMatrix;
               quadEffect.Projection = CameraManager.ActiveCamera.ProjectionMatrix;
               quadEffect.TextureEnabled = true;
               quadEffect.Texture = skyTexture;

               // Doesn't work in XNA 4.0:
               quadVertexDecl = new VertexDeclaration(VertexPositionNormalTexture.VertexDeclaration.GetVertexElements());

               Random random = new Random();

               // Load Enemies
               enemyManager = new EnemyManager(EngineCore.Game);
               enemyManager.IsScriptedWave = false;

               for (int i = currentEnemy; i < currentEnemy + 1; i++)
               {
                    TypingEnemy enemy = new NormalTypingEnemy(position, enemyManager);
                    float randomX = -3f + (float)random.NextDouble() * (2f * 3f); //- 3 + (float)random.NextDouble() * 6;
                    float randomZ = 50 + (float)random.NextDouble() * 40;
                    enemy.WorldPosition = Matrix.CreateTranslation(randomX, 0.0f, randomZ) *
                                             Matrix.CreateRotationY(MathHelper.ToRadians(180.0f)) *
                                             Matrix.CreateScale(1f);

                    enemy.Position = new Vector3(randomX, 0, randomZ);

                    enemy.Avatar.LoadRandomAvatar();
                    enemyManager.AddEnemy(enemy);
               }

               // Clear the SceneGraphManager.
               SceneGraphManager.RemoveObjects();

               #region Initialize Haunted House

               hauntedHouse.Model.LoadContent(@"Models\\Graveyard_HouseFencesGraves");
               hauntedHouse.Position = housePosition;
               hauntedHouse.Rotation = new Quaternion(115f, hauntedHouse.Rotation.Y, hauntedHouse.Rotation.Z, 0f);
               hauntedHouse.Rotation = new Quaternion(hauntedHouse.Rotation.X, 90f, hauntedHouse.Rotation.Z, 0f);
               hauntedHouse.Rotation = new Quaternion(hauntedHouse.Rotation.X, hauntedHouse.Rotation.Y, -154.5f, 0f);
               hauntedHouse.World = Matrix.CreateScale(0.5f, 0.5f * 1.2f, 0.5f * 0.80f)
                    * Matrix.CreateRotationX(MathHelper.ToRadians(hauntedHouse.Rotation.X))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(hauntedHouse.Rotation.Y))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(hauntedHouse.Rotation.Z))
                    * Matrix.CreateTranslation(hauntedHouse.Position);

               #endregion

               #region Initialize Hill

               hill.Model.LoadContent(@"Models\\Graveyard_Hill");
               hillPosition = new Vector3(-0.5f, 12.5f, 180f);
               hill.Position = hillPosition;
               hill.Rotation = new Quaternion(115f, hill.Rotation.Y, hill.Rotation.Z, 0f);
               hill.Rotation = new Quaternion(hill.Rotation.X, 90f, hill.Rotation.Z, 0f);
               hill.Rotation = new Quaternion(hill.Rotation.X, hill.Rotation.Y, -154.5f, 0f);
               hill.World = Matrix.CreateScale(new Vector3(2f, 10f, 10f))
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

               #region Initialize Graves - Rounded

               // Left Graves: Column 1
               for (int i = 0; i < 10; i++)
               {
                    SceneObject grave = new SceneObject();
                    grave.Model.LoadContent(@"Models\Graveyard\Grave_Tall");

                    float randomScale = 2f * (float)random.NextDouble(); // Returns 0-2
                    randomScale = MathHelper.Clamp(randomScale, 0.90f, 1.5f);

                    Vector3 gravePosition = new Vector3(5f, -0.2f, 7.5f + i * 5); // 10f + i * 2

                    grave.Position = gravePosition;
                    grave.Rotation = new Quaternion(-90f, grave.Rotation.Y, grave.Rotation.Z, 0f);
                    grave.Rotation = new Quaternion(grave.Rotation.X, 180f, grave.Rotation.Z, 0f); //90
                    grave.Rotation = new Quaternion(grave.Rotation.X, grave.Rotation.Y, 0f, 0f); // 0 for Cross
                    grave.World = Matrix.CreateScale(0.01f * randomScale)
                         * Matrix.CreateRotationX(MathHelper.ToRadians(grave.Rotation.X))
                         * Matrix.CreateRotationY(MathHelper.ToRadians(grave.Rotation.Y))
                         * Matrix.CreateRotationZ(MathHelper.ToRadians(grave.Rotation.Z))
                         * Matrix.CreateTranslation(grave.Position);

                    grave.Model.AmbientLightColor = graveColors[random.Next(graveColors.Length)];

                    SceneGraphManager.AddObject(grave);
               }

               // Left Graves: Column 2
               for (int i = 0; i < 10; i++)
               {
                    SceneObject grave = new SceneObject();
                    grave.Model.LoadContent(@"Models\Graveyard\Grave_Tall");

                    float randomScale = 2f * (float)random.NextDouble(); // Returns 0-1.5
                    randomScale = MathHelper.Clamp(randomScale, 0.90f, 1.5f);

                    Vector3 gravePosition = new Vector3(7.5f, -0.2f, 7.5f + i * 5);

                    grave.Position = gravePosition;
                    grave.Rotation = new Quaternion(-90f, grave.Rotation.Y, grave.Rotation.Z, 0f);
                    grave.Rotation = new Quaternion(grave.Rotation.X, 180f, grave.Rotation.Z, 0f); //90
                    grave.Rotation = new Quaternion(grave.Rotation.X, grave.Rotation.Y, 0f, 0f); // 0 for Cross
                    grave.World = Matrix.CreateScale(0.01f * randomScale)//0.50f)
                         * Matrix.CreateRotationX(MathHelper.ToRadians(grave.Rotation.X))
                         * Matrix.CreateRotationY(MathHelper.ToRadians(grave.Rotation.Y))
                         * Matrix.CreateRotationZ(MathHelper.ToRadians(grave.Rotation.Z))
                         * Matrix.CreateTranslation(grave.Position);

                    grave.Model.AmbientLightColor = graveColors[random.Next(graveColors.Length)];

                    SceneGraphManager.AddObject(grave);
               }

               // Left Graves: Column 3
               for (int i = 0; i < 10; i++)
               {
                    SceneObject grave = new SceneObject();
                    grave.Model.LoadContent(@"Models\Graveyard\Grave_Tall");

                    float randomScale = 2.0f * (float)random.NextDouble(); // Returns 0-2
                    randomScale = MathHelper.Clamp(randomScale, 0.90f, 1.50f);

                    Vector3 gravePosition = new Vector3(10f, -0.2f, 7.5f + i * 5);

                    grave.Position = gravePosition;
                    grave.Rotation = new Quaternion(-90f, grave.Rotation.Y, grave.Rotation.Z, 0f);
                    grave.Rotation = new Quaternion(grave.Rotation.X, 180f, grave.Rotation.Z, 0f); //90
                    grave.Rotation = new Quaternion(grave.Rotation.X, grave.Rotation.Y, 0f, 0f); // 0 for Cross
                    grave.World = Matrix.CreateScale(0.01f * randomScale)//0.50f)
                         * Matrix.CreateRotationX(MathHelper.ToRadians(grave.Rotation.X))
                         * Matrix.CreateRotationY(MathHelper.ToRadians(grave.Rotation.Y))
                         * Matrix.CreateRotationZ(MathHelper.ToRadians(grave.Rotation.Z))
                         * Matrix.CreateTranslation(grave.Position);

                    grave.Model.AmbientLightColor = graveColors[random.Next(graveColors.Length)];

                    SceneGraphManager.AddObject(grave);
               }

               // Right Graves: Column 1
               for (int i = 0; i < 10; i++)
               {
                    SceneObject grave = new SceneObject();
                    grave.Model.LoadContent(@"Models\Graveyard\Grave_Tall");

                    float randomScale = 2f * (float)random.NextDouble(); // Returns 0-1.5
                    randomScale = MathHelper.Clamp(randomScale, 0.90f, 1.5f);

                    Vector3 gravePosition = new Vector3(-5f, -0.2f, 7.5f + i * 5);

                    grave.Position = gravePosition;
                    grave.Rotation = new Quaternion(-90f, grave.Rotation.Y, grave.Rotation.Z, 0f);
                    grave.Rotation = new Quaternion(grave.Rotation.X, 180f, grave.Rotation.Z, 0f); //90
                    grave.Rotation = new Quaternion(grave.Rotation.X, grave.Rotation.Y, 0f, 0f); // 0 for Cross
                    grave.World = Matrix.CreateScale(0.01f * randomScale)//0.50f)
                         * Matrix.CreateRotationX(MathHelper.ToRadians(grave.Rotation.X))
                         * Matrix.CreateRotationY(MathHelper.ToRadians(grave.Rotation.Y))
                         * Matrix.CreateRotationZ(MathHelper.ToRadians(grave.Rotation.Z))
                         * Matrix.CreateTranslation(grave.Position);

                    grave.Model.AmbientLightColor = graveColors[random.Next(graveColors.Length)];

                    SceneGraphManager.AddObject(grave);
               }

               // Right Graves: Column 2
               for (int i = 0; i < 10; i++)
               {
                    SceneObject grave = new SceneObject();
                    grave.Model.LoadContent(@"Models\Graveyard\Grave_Tall");

                    float randomScale = 2f * (float)random.NextDouble(); // Returns 0-1.5
                    randomScale = MathHelper.Clamp(randomScale, 0.90f, 1.5f);

                    Vector3 gravePosition = new Vector3(-7.5f, -0.2f, 7.5f + i * 5);

                    grave.Position = gravePosition;
                    grave.Rotation = new Quaternion(-90f, grave.Rotation.Y, grave.Rotation.Z, 0f);
                    grave.Rotation = new Quaternion(grave.Rotation.X, 180f, grave.Rotation.Z, 0f); //90
                    grave.Rotation = new Quaternion(grave.Rotation.X, grave.Rotation.Y, 0f, 0f); // 0 for Cross
                    grave.World = Matrix.CreateScale(0.01f * randomScale)//0.50f)
                         * Matrix.CreateRotationX(MathHelper.ToRadians(grave.Rotation.X))
                         * Matrix.CreateRotationY(MathHelper.ToRadians(grave.Rotation.Y))
                         * Matrix.CreateRotationZ(MathHelper.ToRadians(grave.Rotation.Z))
                         * Matrix.CreateTranslation(grave.Position);

                    grave.Model.AmbientLightColor = graveColors[random.Next(graveColors.Length)];

                    SceneGraphManager.AddObject(grave);
               }

               // Right Graves: Column 2
               for (int i = 0; i < 10; i++)
               {
                    SceneObject grave = new SceneObject();
                    grave.Model.LoadContent(@"Models\Graveyard\Grave_Tall");

                    float randomScale = 2f * (float)random.NextDouble(); // Returns 0-1.5
                    randomScale = MathHelper.Clamp(randomScale, 0.90f, 1.5f);

                    Vector3 gravePosition = new Vector3(-10.0f, -0.2f, 7.5f + i * 5);

                    grave.Position = gravePosition;
                    grave.Rotation = new Quaternion(-90f, grave.Rotation.Y, grave.Rotation.Z, 0f);
                    grave.Rotation = new Quaternion(grave.Rotation.X, 180f, grave.Rotation.Z, 0f); //90
                    grave.Rotation = new Quaternion(grave.Rotation.X, grave.Rotation.Y, 0f, 0f); // 0 for Cross
                    grave.World = Matrix.CreateScale(0.01f * randomScale)//0.50f)
                         * Matrix.CreateRotationX(MathHelper.ToRadians(grave.Rotation.X))
                         * Matrix.CreateRotationY(MathHelper.ToRadians(grave.Rotation.Y))
                         * Matrix.CreateRotationZ(MathHelper.ToRadians(grave.Rotation.Z))
                         * Matrix.CreateTranslation(grave.Position);

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

          /// <summary>
          /// Unloads graphics content for this screen.
          /// </summary>
          public override void UnloadContent()
          {
               //content.Unload();
          }

          #endregion

          #region Update

          /// <summary>
          /// Updates the background screen. Unlike most screens, this should not
          /// transition off even if it has been covered by another screen: it is
          /// supposed to be covered, after all! This overload forces the
          /// coveredByOtherScreen parameter to false in order to stop the base
          /// Update method wanting to transition off.
          /// </summary>
          public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                         bool coveredByOtherScreen)
          {
               if (!isUpdate)
                    return;

               enemyManager.Update(gameTime);

               if (enemyManager.Size <= 1)
               {
                    TypingEnemy enemy = new NormalTypingEnemy(position, enemyManager);
                    float randomX = -3f + (float)random.NextDouble() * (2f * 3f);
                    float randomZ = 35 + (float)random.NextDouble() * 20;
                    enemy.WorldPosition = Matrix.CreateTranslation(randomX, 0.0f, randomZ) *
                                             Matrix.CreateRotationY(MathHelper.ToRadians(180.0f)) *
                                             Matrix.CreateScale(1f);

                    enemy.Position = new Vector3(randomX, 0, randomZ);

                    enemy.Avatar.LoadRandomAvatar();
                    enemyManager.AddEnemy(enemy);
               }

               base.Update(gameTime, otherScreenHasFocus, false);
          }

          #endregion

          #region Draw

          /// <summary>
          /// Draws the background screen.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               if (!isUpdate)
                    return;

               MySpriteBatch.Begin();
               MySpriteBatch.Draw(skyTexture, new Rectangle(0, 0, 1280, 720), Color.White);
               MySpriteBatch.End();

               /*
               EngineCore.GraphicsDevice.VertexDeclaration = quadVertexDecl;
               quadEffect.Begin();
               foreach (EffectPass pass in quadEffect.CurrentTechnique.Passes)
               {
                    pass.Begin();

                    EngineCore.GraphicsDevice.DrawUserIndexedPrimitives
                        <VertexPositionNormalTexture>(
                        PrimitiveType.TriangleList,
                        quad.Vertices, 0, 4,
                        quad.Indexes, 0, 2);

                    pass.End();
               }
               quadEffect.End();
               */

               //EngineCore.GraphicsDevice.RenderState.CullMode = CullMode.CullClockwiseFace;

               // Draw the Skybox.
               //skybox.Draw(CameraManager.ActiveCamera.ViewMatrix, CameraManager.ActiveCamera.ProjectionMatrix, CameraManager.ActiveCamera.Position);

               //EngineCore.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;

               // Render the Scene.
               SceneGraphManager.Draw(gameTime);

               //MySpriteBatch.Begin();
               enemyManager.Draw(gameTime);
               //MySpriteBatch.End();
          }

          #endregion
     }
}
