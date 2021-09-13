#region File Description
//-----------------------------------------------------------------------------
// Player.cs
// Author: Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PixelEngine;
using PixelEngine.Avatars;
using PixelEngine.CameraSystem;
using PixelEngine.Graphics;
using PixelEngine.Screen;
using PixelEngine.Text;
#endregion

namespace AvatarTyping
{
     #region Projectile Class

     public class Bullet
     {
          #region Fields

          public float Speed;
          public Vector3 Position;
          public Vector3 Velocity;
          public Vector3 TargetPosition;
          public bool RemoveBullet;

          #endregion

          #region Initialization

          public Bullet()
          {
               Speed = 2.0f;
               Position = new Vector3(0);
               Velocity = new Vector3(0);
               TargetPosition = new Vector3(0);
               RemoveBullet = false;
          }

          #endregion

          #region Updating

          public void Update()
          {
               Position =
                    new Vector3(Position.X - Velocity.X, Position.Y - Velocity.Y, Position.Z - Velocity.Z);

               if (Position.Z > TargetPosition.Z)
               {
                    // Remove.
                    RemoveBullet = true;
               }
          }

          #endregion
     }

     #endregion

     /// <summary>
     /// Defines what animation the avatar should be using
     /// </summary>
     public enum PlayerAnimationPlaybackMode { StandType, Faint };

     /// <remarks>
     /// Defines a Player, including his Avatar appearance.
     /// </remarks>
     public class Player : DrawableGameComponent, ICameraTrackable
     {
          #region Fields

          public GamerInformation GamerInfo;
          private Avatar avatar;
          private int health;
          private bool isAlive;
          private Vector3 position;
          private Matrix defaultWorldMatrix;
          
          // For the Player's Combo Meter.
          private float comboMeter = 0.0f;
          private bool earnedExtraLife = false;
          private float elapsedHealthEarnedTime = 0.0f;

          private List<PixelEngine.AchievementSystem.Achievement> earnedAchievements = new List<PixelEngine.AchievementSystem.Achievement>();

          // World Matrix used for rendering (Just a helper variable).
          private Matrix world;

          // AvatarAnimation that defines the Player's default animation.
          private AvatarAnimation normalAnimation = new AvatarAnimation(AvatarAnimationPreset.MaleIdleCheckHand);

          private Model3D gunModel;

          public List<Bullet> BulletList = new List<Bullet>();
          Model3D bulletModel;

          public static bool FiredBullet = false;
          public static Vector3 TargetLocation = new Vector3();

          #region Not Used.

          // Store the current and last gamepad state
          GamePadState currentGamePadState = new GamePadState();
          //GamePadState lastGamePadState = new GamePadState();

          // The following constants control the camera's default position.
          const float CameraDefaultArc = MathHelper.Pi / 10;
          const float CameraDefaultRotation = MathHelper.Pi;
          const float CameraDefaultDistance = 2.5f;

          // Camera values.
          float cameraArc = CameraDefaultArc;
          float cameraRotation = CameraDefaultRotation;
          float cameraDistance = CameraDefaultDistance;

          #endregion

          #region Camera-Related Fields

          // Set the avatar position and rotation variables.
          static Vector3 avatarPosition = new Vector3(0, 0, 0f);

          static Vector3 avatarHeadOffset = new Vector3(0, 1.25f, 0);

          //static float avatarYaw = 0;
          //static float avatarPitch = 0;

          // Set the direction the camera points without rotation.
          static Vector3 cameraReference = new Vector3(0, 0, 10);

          static Vector3 thirdPersonReference = new Vector3(0, 5, -5);

          // Set rates in world units per 1/60th second 
          // (the default fixed-step interval).
          //static float rotationSpeed = 2f / 60f;
          //static float forwardSpeed = 5f / 60f;

          #endregion

          #endregion

          public bool IsTyping = false;

          AvatarBaseAnimation standAnimation;
          AvatarBaseAnimation defensiveAnimation;

          #region Properties

          /// <summary>
          /// Gets or sets the Player's Avatar.
          /// </summary>
          public Avatar Avatar
          {
               get { return avatar; }
               set { avatar = value; }
          }

          /// <summary>
          /// Gets or sets the default World Matrix for the Player.
          /// </summary>
          public Matrix DefaultWorldMatrix
          {
               get { return defaultWorldMatrix; }
               set { defaultWorldMatrix = value; }
          }

          /// <summary>
          /// Gets or sets the Player's Health.
          /// </summary>
          public int Health
          {
               get { return health; }
               set 
               {
                    if (value < 0)
                    {
                         health = 0;
                    }

                    else
                    {
                         health = value;
                    }
               }
          }

          /// <summary>
          /// Returns true if the Player is still Alive.
          /// </summary>
          public bool IsAlive
          {
               get { return isAlive; }
               set { isAlive = value; }
          }

          /// <summary>
          /// Gets or sets the Player's 3D Position.
          /// </summary>
          public Vector3 Position
          {
               get { return position; }
               set { position = value; }
          }

          /// <summary>
          /// Gets a list of Achievements the Player has obtained.
          /// </summary>
          public List<PixelEngine.AchievementSystem.Achievement> EarnedAchievements
          {
               get { return earnedAchievements; }
          }

          /// <summary>
          /// Gets or Sets the Player's Combo Meter.
          /// 
          /// Ranges from 0 - 100.
          /// </summary>
          public float ComboMeter
          {
               get { return comboMeter; }
               set { comboMeter = value; }
          }

          /// <summary>
          /// Gets or Sets if an Extra Life has been earned.
          /// </summary>
          public bool EarnedExtraLife
          {
               get { return earnedExtraLife; }
               set { earnedExtraLife = value; }
          }

          #endregion

          #region ICameraTrackable Properties

          /// <summary>
          /// Gets or sets the position being tracked by the Camera.
          /// In this case, the Tracked Position is the position of the
          /// player's avatar.
          /// </summary>
          public Vector3 TrackedPosition
          {
               get
               {
                    if (avatar != null)
                         return avatar.WorldMatrix.Translation;
                    return world.Translation;
               }

               set { world.Translation = value; }
          }

          /// <summary>
          /// Gets or sets the position (Matrix form) being tracked by the Camera.
          /// In this case, the Tracked World Matrix is the world matrix of the
          /// player's avatar.
          /// </summary>
          public Matrix TrackedWorldMatrix
          {
               get { return avatar.WorldMatrix; }
          }

          #endregion

          #region Initialization

          /// <summary>
          /// Constructs a new player.
          /// </summary>
          public Player(PlayerIndex playerIndex)
               : base(PixelEngine.EngineCore.Game)
          {
               this.isAlive = true;

               this.health = 10;

               this.GamerInfo = new GamerInformation(playerIndex);

               this.position = new Vector3(0f, 0f, 0f);

               // Set the ThirdPersonCamera to track the Player.
               //Camera.Track = this;

               this.avatar = new Avatar(EngineCore.Game, 1.0f);
               this.defaultWorldMatrix = this.avatar.WorldMatrix;
             
               // Create the Player's Avatar.
               avatar.LoadUserAvatar(playerIndex);

               // Set the Player's Avatar's animation.
               //avatar.PlayAnimation(normalAnimation, true);
               
               avatarPosition = position;
               Avatar.Rotation = new Vector3(0, 180, 0);
               Avatar.Scale = 1.0f;


               // Initialize the list of bones in world space
               Avatar.BonesInWorldSpace = new List<Matrix>(AvatarRenderer.BoneCount);

               for (int i = 0; i < AvatarRenderer.BoneCount; i++)
                    Avatar.BonesInWorldSpace.Add(Matrix.Identity);

               // Let an AvatarManager handle the Avatar.
               //avatar.AvatarManager = new AvatarManager(PixelEngine.EngineCore.Game);
               //avatar.AvatarManager.AddAvatar(this.avatar);
               
               LoadContent();

               // Load the preset animations
               standAnimation = new AvatarBaseAnimation(AvatarAnimationPreset.Stand0);
               defensiveAnimation = new AvatarBaseAnimation(AvatarAnimationPreset.MaleIdleLookAround);

               // List of the bone index values for the right arm and its children
               List<AvatarBone> bonesUsedInFirstAnimation = new List<AvatarBone>();
               List<AvatarBone> bonesUsedInSecondAnimation = new List<AvatarBone>();

               //bonesUsedInFirstAnimation.Add(AvatarBone.ToeLeft);
               bonesUsedInSecondAnimation.Add(AvatarBone.ShoulderRight);
               bonesUsedInSecondAnimation.Add(AvatarBone.WristRight);

               List<List<AvatarBone>> listOfBonesUsedForEachAnimation = new List<List<AvatarBone>>();
               listOfBonesUsedForEachAnimation.Add(bonesUsedInFirstAnimation);
               listOfBonesUsedForEachAnimation.Add(bonesUsedInSecondAnimation);

               List<AvatarBaseAnimation> animationsToCombine = new List<AvatarBaseAnimation>();
               animationsToCombine.Add(standAnimation);
               animationsToCombine.Add(new AvatarCustomAnimation(AvatarManager.LoadedAvatarAnimationData["Pull"]));

               // Play the regular Stand Animation by default.
               Avatar.PlayAnimation(standAnimation, true);

               
               standAndShoot = new AvatarMultipleAnimation(Avatar.AvatarRenderer, animationsToCombine, listOfBonesUsedForEachAnimation);
          }

          AvatarMultipleAnimation standAndShoot;

          /// <summary>
          /// Loads the Player's assets (Avatar).
          /// </summary>
          protected override void LoadContent()
          {
               gunModel = new Model3D();
               gunModel.Model = EngineCore.Content.Load<Model>(@"Models\Gun_Pink");

               bulletModel = new Model3D();
               bulletModel.Model = EngineCore.Content.Load<Model>(@"Models\Ball");
               bulletModel.EmissiveColor = Color.Blue;
               bulletModel.SpecularColor = Color.Blue;
          }

          protected override void UnloadContent()
          {
               this.Avatar.Dispose();
               this.Avatar = null;

               base.UnloadContent();
          }

          #endregion

          #region Update

          /// <summary>
          /// Update the Player (position, status, etc).
          /// </summary>
          public override void Update(GameTime gameTime)
          {
               // Testing. But why call AvatarManager.Update directly?
               // If it's a Manager object shouldn't we add it to the Game
               // and have it Update on its own?
               /*if (Avatar != null && Avatar.AvatarManager != null)
               {
                    Avatar.AvatarManager.Update(gameTime);
               }
               */

               #region Updating Bullets

               if (BulletList != null)
               {
                    foreach (Bullet bullet in BulletList)
                    {
                         bullet.Update();
                    }
               }

               if (BulletList != null)
               {
                    foreach (Bullet bullet in BulletList)
                    {
                         if (bullet.RemoveBullet)
                         {
                              BulletList.Remove(bullet);
                              break;
                         }
                    }
               }

               if (Player.FiredBullet)
               {
                         ShootBullet();

                         Player.FiredBullet = false;
               }

               #endregion

               Avatar.Update(gameTime);

               // Check the Player's Health to see if he's Dead.
               if (this.health <= 0)
               {
                    isAlive = false;
               }

               if (!IsTyping)
               {
                    if (!stoodAlready)
                    {
                         this.Stand();
                         stoodAlready = true;
                         typedAlready = false;
                    }
               }

               if (IsTyping)
               {
                    if (!typedAlready)
                    {
                         this.ReactToPlayerInput();
                         typedAlready = true;

                         // Fix
                         stoodAlready = false;
                    }
               }
          }

          private bool stoodAlready = false;
          private bool typedAlready = false;

          #endregion

          #region Handle Input

          #endregion

          #region Draw

          /// <summary>
          /// Draws the Player, first checking if its Avatar is registered with AvatarManager.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               //Avatar.AvatarManager.Draw(gameTime);

               #region Draw Weapon & Bullets
               
               if (weaponEquipped)
               {
                    DrawWeapon();

                    foreach (Bullet bullet in BulletList)
                    {
                         Matrix mat =
                              Matrix.CreateScale(0.0020f) * Matrix.CreateTranslation(bullet.Position); // 0.0025

                         bulletModel.DrawModel(mat);
                    }
               }
               
               #endregion

               if (Avatar.AvatarRenderer == null)
                    return;

               Avatar.Draw(gameTime);

               #region Draw Extra Life, when necessary

               if (EarnedExtraLife)
               {
                    elapsedHealthEarnedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (elapsedHealthEarnedTime <= 5000.0f)
                    {
                         DrawLifeEarned(gameTime);
                    }

                    else
                    {
                         elapsedHealthEarnedTime = 0.0f;
                         EarnedExtraLife = false;
                    }
               }

               #endregion
          }

          #endregion

          #region Public Methods

          /// <summary>
          /// Resets the player to life.
          /// </summary>
          /// <param name="position">The position to come to life at.</param>
          public void Spawn(Vector3 position)
          {
               Position = position;
               isAlive = true;
          }
          #endregion

          #region Avatar Animation Actions
 
          List<bool> toUpdate = new List<bool>{true, false};

          public void ReactToPlayerInput()
          {
               Avatar.PlayMultipleAnimation(toUpdate, Avatar.AvatarRenderer, standAndShoot.Animations, standAndShoot.TempListOfBoneList);
               
               // Will this fix the crusified-ish type animation glitch?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!!?!?!?!?!?!?!?!?!?!?!?!?!?!?
               //standAndShoot.CurrentPosition = new TimeSpan(0);
          }

          /// <summary>
          /// Causes the Player to stand, ala changing the Player's Avatar's
          /// animation to AvatarAnimationPreset.Stand.
          /// <param name="standType">The AvatarAnimationPreset.Stand to use. 0-7 are valid.</param>
          /// </summary>
          public void Stand(int standType)
          {
               standType = (int)MathHelper.Clamp(standType, 0, 7);

               this.Avatar.PlayAnimation((AvatarAnimationPreset)standType, true);
          }

          public void Stand()
          {
               if (!weaponEquipped)
                    return;

               Avatar.PlayAnimation(defensiveAnimation, true);
               defensiveAnimation.CurrentPosition = new TimeSpan(0);
          }

          #endregion

          #region Debug Helpers

          public virtual void PrintDebug()
          {
               string s = ToString();

               PixelEngine.Text.TextManager.DrawAutoCentered(PixelEngine.Screen.ScreenManager.Font,
                    s, new Vector2(PixelEngine.EngineCore.ScreenCenter.X, 500f), Color.White, 0.5f);
          }

          #endregion

          #region ToString Override

          public override string ToString()
          {
               return String.Format("Player's World Position: {0}", avatar.Position.ToString());
          }

          #endregion

          #region Helper Draw Extra Lives Earned

          /// <summary>
          /// Draws "+ Extra Life!" to the screen when the Player's
          /// Combo Meter is filled up.
          /// </summary>
          public void DrawLifeEarned(GameTime gameTime)
          {
               TextManager.DrawCentered(false, ScreenManager.Font, "+ Extra Life!", new Vector2(250, 720 - 150), Color.Gold, 0.8f);
          }

          #endregion

          private bool weaponEquipped = false;

          public void EquipWeapon()
          {
               weaponEquipped = true;
          }

          public void UnequipWeapon()
          {
               weaponEquipped = false;

               if (BulletList != null)
               {
                    BulletList.Clear();
               }
          }

          #region Helper Draw Weapon

          /// <summary>
          /// Draw the Weapon.
          /// </summary>
          private void DrawWeapon()
          {
               Vector3 gunPosition = new Vector3(-.15f, 0.0f, 0.1f);  // X = ? Y = ? . Z = Up to down of arm?   -.1 0 0 is almost perfect, needs to go up towards thumb more

               float targetRotationZ = -1 * (GunRotation);

               this.Avatar.Rotation = new Vector3(0f, 180f, 0f);
               this.Avatar.Rotation = new Vector3(this.Avatar.Rotation.Z, this.Avatar.Rotation.Y + targetRotationZ, this.Avatar.Rotation.Z);

               Quaternion rotation = new Quaternion();
               rotation = new Quaternion(210f, rotation.Y, rotation.Z, 0f); // -90 Tilts it left to right
               rotation = new Quaternion(rotation.X, 170f, rotation.Z, 0f); // 180 Swerves the handle?  Tilts the handle up?
               rotation = new Quaternion(rotation.X, rotation.Y, 35f, 0f); // 0  Swerves the handle. Positive = to the right. - targetRotationZ

               Matrix gunOffset = Matrix.CreateScale(0.0025f)
                    * Matrix.CreateRotationX(MathHelper.ToRadians(rotation.X))
                    * Matrix.CreateRotationY(MathHelper.ToRadians(rotation.Y))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(rotation.Z))
                    * Matrix.CreateTranslation(gunPosition);


               gunModel.AmbientLightColor = Color.Gray;     // Color to make objects that have ambient light.
               gunModel.DiffuseColor = Color.Gray;          // Color under pure white light.
               //effect.SpecularColor = Color.Blue;
               //effect.EmissiveColor = Color.Blue;

               if (Avatar.BonesInWorldSpace != null)
                    gunModel.DrawModel(gunOffset * Avatar.BonesInWorldSpace[(int)AvatarBone.SpecialRight]);
          }

          #endregion

          #region Shooting Methods

          public float GunRotation = 0f;

          public void ShootBullet()
          {
               Random random = new Random();

               Bullet bullet = new Bullet();

               bullet.TargetPosition = Player.TargetLocation;
               bullet.Position = new Vector3(this.Position.X - 0.30f, this.Position.Y + 1.80f, this.Position.Z + 0.5f);

               Vector3 bulletOffet = new Vector3(-0.05f, 0.25f, 0.5f);// new Vector3(0f, 0.25f, 0.5f);

               if (Avatar.BonesInWorldSpace != null)
                    bullet.Position = bulletOffet + Avatar.BonesInWorldSpace[(int)AvatarBone.SpecialRight].Translation;

               bullet.Speed = 0.1f;
               bullet.Velocity = new Vector3(this.Position.X - bullet.TargetPosition.X, 0f, this.Position.Z - bullet.TargetPosition.Z);
               bullet.Velocity = Vector3.Normalize(bullet.Velocity);

               GunRotation = MathHelper.ToDegrees((float)Math.Atan2(bullet.Velocity.Z, bullet.Velocity.X)) + 90f;

               // If the Avatar is required to turn himself quite a bit...
               if (Math.Abs(this.Avatar.Rotation.Y - GunRotation) > 20f)
               {
                    // Let the process happen over time.
               }

               BulletList.Add(bullet);
          }

          #endregion          
 
          // Not used.

          #region Update Avatar Movement

          /// <summary>
          /// Update the avatars movement based on user input.
          /// </summary>
          private void UpdateAvatarMovement(GameTime gameTime)
          {
               // Create vector from the left thumbstick location
               Vector2 leftThumbStick = currentGamePadState.ThumbSticks.Left;

               // The direction for our Avatar
               Vector3 avatarForward = world.Forward;

               // The amount we want to translate
               Vector3 translate = Vector3.Zero;

               if (currentGamePadState.IsButtonDown(Buttons.A))
               {
                    avatar.Laugh();
               }
               // Clamp thumbstick to make sure the user really wants to move
               if (leftThumbStick.Length() > 0.2f)
               {
                    // Create our direction vector
                    leftThumbStick.Normalize();

                    // Find the new avatar forward
                    avatarForward.X = leftThumbStick.X;
                    avatarForward.Y = 0;
                    avatarForward.Z = -leftThumbStick.Y;
                    // Translate the thumbstick using the current camera rotation
                    avatarForward = Vector3.Transform(avatarForward,
                                                     Matrix.CreateRotationY(cameraRotation));
                    avatarForward.Normalize();

                    // Determine the amount of translation
                    translate = avatarForward
                                * ((float)gameTime.ElapsedGameTime.TotalMilliseconds
                                * 0.0009f);

                    // We are now walking
                    //currentType = AnimationType.Walk;
               }
               else
               {
                    // If we were walking last frame pick a random idle animation
                   // if (currentType == AnimationType.Walk)
                    {
                         //PlayRandomIdle();
                    }
               }

               // Update the world matrix
               world.Forward = avatarForward;

               // Normalize the matrix
               world.Right = Vector3.Cross(world.Forward, Vector3.Up);
               world.Right = Vector3.Normalize(world.Right);
               world.Up = Vector3.Cross(world.Right, world.Forward);
               world.Up = Vector3.Normalize(world.Up);

               // Add translation
               world.Translation += translate;

               //avatar.WorldMatrix = world;

          }

          #endregion

          #region Testing

          /// <summary>
          /// Move camera based on user input
          /// </summary>
          private void HandleCameraInput()
          {
               // should we reset the camera?
               if (currentGamePadState.Buttons.RightStick == ButtonState.Pressed)
               {
                    cameraArc = CameraDefaultArc;
                    cameraDistance = CameraDefaultDistance;
                    cameraRotation = CameraDefaultRotation;
               }

               // Update Camera
               cameraArc -= currentGamePadState.ThumbSticks.Right.Y * 0.05f;
               cameraRotation += currentGamePadState.ThumbSticks.Right.X * 0.1f;
               cameraDistance += currentGamePadState.Triggers.Left * 0.1f;
               cameraDistance -= currentGamePadState.Triggers.Right * 0.1f;

               // Limit the camera movement
               if (cameraDistance > 5.0f)
                    cameraDistance = 5.0f;
               else if (cameraDistance < 2.0f)
                    cameraDistance = 2.0f;

               if (cameraArc > MathHelper.Pi / 5)
                    cameraArc = MathHelper.Pi / 5;
               else if (cameraArc < -(MathHelper.Pi / 5))
                    cameraArc = -(MathHelper.Pi / 5);

               // Update the camera position
               Vector3 cameraPos = new Vector3(0, cameraDistance, cameraDistance);
               cameraPos = Vector3.Transform(cameraPos, Matrix.CreateRotationX(cameraArc));
               cameraPos = Vector3.Transform(cameraPos,
                                             Matrix.CreateRotationY(cameraRotation));

               cameraPos += world.Translation;

               // Create new view matrix
               Matrix view = Matrix.CreateLookAt(cameraPos, world.Translation + new Vector3(0, 1.2f, 0), Vector3.Up);

               // Set the new view on the avatar renderer
               if (avatar.AvatarRenderer != null)
               {
                    avatar.AvatarRenderer.View = view;
               }
          }

          #endregion
     }
}
