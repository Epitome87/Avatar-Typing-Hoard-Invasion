#region File Description
//-----------------------------------------------------------------------------
// Avatar.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using CustomAvatarAnimation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.CameraSystem;
using PixelEngine.Graphics;
#endregion

namespace PixelEngine.Avatars
{
     /// <summary>
     /// Defines what animation the avatar should be using
     /// </summary>
     public enum AvatarAnimationPlaybackMode { StandType, Faint };

     /// <remarks>
     /// A wrapper class for Avatar-related functionality:
     /// Renderer / Description, Custom Animations / Expression.
     /// </remarks>
     public class Avatar : DrawableGameComponent, ILightable, ICollidable
     {
          #region Fields

          // The AvatarManager which is to manage this Avatar.
          private AvatarManager avatarManager;

          // The AvatarDescription and AvatarRenderer that are used to render the Avatar.
          private AvatarRenderer avatarRenderer;
          private AvatarDescription avatarDescription;

          // Animations that will be used.
          private AvatarBaseAnimation currentAvatarAnimation;
          private AvatarExpression currentFacialExpression = new AvatarExpression();

          // The Position of the Avatar.
          private Vector3 position = Vector3.Zero;

          // The Scale for the Avatar. Must be uniform.
          private float scale = 1.0f;

          // The Rotation of the Avatar.
          private Vector3 rotation = Vector3.Zero;

          private bool isAnimating = false;         // USE TO BE FALSE: THIS IS THE ONLY TIME IT WAS EVER SET TO FALSE. SETTING TO TRUE MIGHT FIX MY GUN GLITCH?!
          private bool isAnimationLooping = false;

          // A random number generator for picking new idle animations
          private Random random = new Random();

          // Shadow fields.
          // Create the array of matrices that will hold bone transforms for shadows.
          private Matrix[] shadowTransforms = new Matrix[71];

          // The Plane to cast the shadow along.
          private Plane plane = new Plane(Vector3.Up, 0);

          // The rotation of the light for the shadow.
          private float lightRotation = 0.0f;

          // Enable Shadowing?
          private bool isShadowEnabled = false;

          // List of the avatar bones in world space - for object placement.
          private List<Matrix> bonesWorldSpace;

          #endregion

          #region ILightable Fields

          /// <summary>
          /// Gets or Sets the Ambient Light Color used to render the Avatar.
          /// </summary>
          public Vector3 AmbientLightColor
          {
               get { return ambientLightColor; }
               set { ambientLightColor = value; }
          }
          private Vector3 ambientLightColor = Color.Gray.ToVector3();

          /// <summary>
          /// Gets or sets the light color used to draw the avatar on the Xbox
          /// </summary>
          public Vector3 LightColor
          {
               get { return lightColor; }
               set { lightColor = value; }
          }
          private Vector3 lightColor = Color.CornflowerBlue.ToVector3();

          /// <summary>
          /// Gets or sets the light direction used to draw the avatar on the Xbox
          /// </summary>
          public Vector3 LightDirection
          {
               get { return lightDirection; }
               set { lightDirection = value; }
          }
          private Vector3 lightDirection = new Vector3(1, 1, 0);

          public bool LightingEnabled
          {
               get { return lightingEnabled; }
               set { lightingEnabled = value; }
          }
          bool lightingEnabled = true;

          #endregion

          #region Default Lighting Values

          public Vector3 DefaultAmbient = new Vector3(0.55f);
          public Vector3 DefaultLightColor = new Vector3(0.4f);
          public Vector3 DefaultLightDirection = new Vector3(-0.5f, -0.6123f, -0.6123f);

          #endregion

          #region ICollidable Fields

          private BoundingSphere boundingSphere = new BoundingSphere();

          #endregion

          #region Properties

          /// <summary>
          /// Gets or sets the bones in world space - for object placement, for example.
          /// 
          /// Note: Currently we must set this value ourselves, so calling Get is only
          /// meaningful if we ourselves are assigning the property a value previously.
          /// </summary>
          public List<Matrix> BonesInWorldSpace
          {
               get { return bonesWorldSpace; }
               set { bonesWorldSpace = value; }
          }

          /// <summary>
          /// Gets or sets the AvatarManager of this Avatar.
          /// </summary>
          public AvatarManager AvatarManager
          {
               get { return avatarManager; }
               set { avatarManager = value; }
          }

          /// <summary>
          /// Gets or Sets The World Matrix of the Avatar.
          /// </summary>
          public Matrix WorldMatrix
          {
               get
               {
                    return
                         Matrix.CreateScale(Scale) *
                         Matrix.CreateFromYawPitchRoll(
                              MathHelper.ToRadians(Rotation.Y),
                              MathHelper.ToRadians(Rotation.X),
                              MathHelper.ToRadians(Rotation.Z)) *
                         Matrix.CreateTranslation(Position);
               }
          }

          /// <summary>
          /// Gets or Sets the Position (in the world) of the Avatar.
          /// </summary>
          public Vector3 Position
          {
               get { return position; }
               set { position = value; }
          }

          /// <summary>
          /// Gets or Sets the Scale the Avatar is rendered in.
          /// </summary>
          public float Scale
          {
               get { return scale; }
               set { scale = value; }
          }

          /// <summary>
          /// Gets or Sets the Scale the Avatar is rendered in.
          /// </summary>
          public Vector3 Rotation
          {
               get { return rotation; }
               set { rotation = value; }
          }

          /// <summary>
          /// Gets or Sets the AvatarRenderer used to render the Avatar.
          /// </summary>
          public AvatarRenderer AvatarRenderer
          {
               get { return avatarRenderer; }
               set { avatarRenderer = value; }
          }

          /// <summary>
          /// Gets or Sets the AvatarDescription property. 
          /// </summary>
          public AvatarDescription AvatarDescription
          {
               get { return avatarDescription; }
               set
               {
                    avatarDescription = value;

                    // The Renderer needs to know when the description has changed.
                    AvatarRenderer = new AvatarRenderer(avatarDescription, true);
               }
          }

          /// <summary>
          /// Gets or Sets the AvatarAnimation of the Avatar.
          /// </summary>
          public AvatarBaseAnimation AvatarAnimation
          {
               get { return currentAvatarAnimation; }
               set { currentAvatarAnimation = value; }
          }

          /// <summary>
          /// Gets or Sets the AvatarExpression of the Avatar.
          /// </summary>
          public AvatarExpression AvatarExpression
          {
               get { return currentFacialExpression; }
               set { currentFacialExpression = value; }
          }

          /// <summary>
          /// Gets the BoundingSphere (calculated automatically 
          /// based on the Avatar's Position and Height.
          /// </summary>
          public BoundingSphere BoundingSphere
          {
               get
               {
                    boundingSphere.Center = position + Vector3.Up * (avatarDescription.Height * 0.5f);
                    boundingSphere.Radius = avatarDescription.Height * 0.5f;
                    return boundingSphere;
               }
          }

          /// <summary>
          /// Gets or Sets whether or not Shadowing is enabled.
          /// If True, an dummy Avatar is used and flatten to represent
          /// the Avatar's shadow.
          /// 
          /// Default: False.
          /// 
          /// Annoyances: Shadows won't always be fully black.
          /// </summary>
          public bool IsShadowEnabled
          {
               get { return isShadowEnabled; }
               set { isShadowEnabled = value; }
          }

          #endregion

          #region Initialization

          public Avatar(Game game)
               : base(game)
          {
               Position = Vector3.Zero;
               Rotation = Vector3.Zero;
               Scale = 1.0f;

               base.Initialize();
          }

          public Avatar(Game game, AvatarBaseAnimation animation)
               : base(game)
          {
               base.Initialize();
               AvatarAnimation = animation;
          }

          public Avatar(Game game, float scale)
               : base(game)
          {
               Position = Vector3.Zero;
               Rotation = Vector3.Zero;
               Scale = scale;

               base.Initialize();
          }

          public Avatar(Game game, Vector3 position)
               : base(game)
          {
               Position = position;
               Rotation = Vector3.Zero;
               Scale = 1.0f;

               base.Initialize();
          }

          public override void Initialize()
          {
               base.Initialize();
          }
          /// <summary>
          /// Load all graphical content.
          /// </summary>
          protected override void LoadContent()
          {
               base.LoadContent();
          }

          protected override void UnloadContent()
          {
               base.UnloadContent();
          }

          #endregion

          #region Updating

          /// <summary>
          /// Updates the Avatar. Checks for a valid Renderer and Animation, updating the Animation if valid.
          /// </summary>
          public override void Update(GameTime gameTime) // Wasn't override until 3-4-2011
          {
               this.lightRotation = 15.5f;

               // No reason to update animations if we can't even render!
               if (avatarRenderer == null)
               {
                    return;
               }

               // No reason to update animations if we are not animating!
               if (!isAnimating)
               {
                    return;
               }

               // No need for switch-case anymore!!!
               if (this.AvatarAnimation != null)
               {
                    this.AvatarAnimation.Update(gameTime.ElapsedGameTime, isAnimationLooping);

                    // New for object placement as of 2/27/2011
                    // Update the current animation and world space bones
                    if (avatarRenderer.State == AvatarRendererState.Ready)
                    {
                         BonesToWorldSpace(avatarRenderer, this.AvatarAnimation, bonesWorldSpace);
                    }
               }
          }

          public void Update(TimeSpan elapsedAnimationTime, bool loop)
          {
               this.lightRotation = 15.5f;

               // No reason to update animations if we can't even render!
               if (avatarRenderer == null)
               {
                    return;
               }

               // No reason to update animations if we are not animating!
               if (!isAnimating)
               {
                    return;
               }

               // No need for switch-case anymore!!!
               if (this.AvatarAnimation != null)
               {
                    this.AvatarAnimation.Update(elapsedAnimationTime, isAnimationLooping);

                    // New for object placement as of 2/27/2011
                    // Update the current animation and world space bones
                    if (avatarRenderer.State == AvatarRendererState.Ready)
                    {
                         BonesToWorldSpace(avatarRenderer, this.AvatarAnimation, bonesWorldSpace);
                    }
               }
          }

          #endregion

          #region Drawing

          /// <summary>
          /// Overriden Component Draw. Calls AvatarRenderer.Draw if the AvatarRender is not null.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               // Can't render if Avatar Renderer is null!
               if (avatarRenderer == null)
                    return;

               #region World, Projection, and View Set-Up

               // Ensure AvatarRenderer knows the Avatar's camera-based location.
               avatarRenderer.World = WorldMatrix;
               avatarRenderer.Projection = CameraManager.ActiveCamera.ProjectionMatrix;
               avatarRenderer.View = CameraManager.ActiveCamera.ViewMatrix;

               #endregion

               #region Lighting

               // If we have Lighting Enabled, use specified light values.
               if (lightingEnabled)
               {
                    avatarRenderer.AmbientLightColor = this.AmbientLightColor;
                    avatarRenderer.LightColor = this.LightColor;
                    avatarRenderer.LightDirection = this.LightDirection;
               }

               // If not Enabled, just use the Default values XNA provides.
               else
               {
                    avatarRenderer.LightDirection = DefaultLightDirection;
                    avatarRenderer.LightColor = DefaultLightColor;
                    avatarRenderer.AmbientLightColor = DefaultAmbient;
               }

               #endregion

               #region Animation & Rendering

               // No need for switch-case anymore!!!
               
               // ************************************************
               // Don't need this null check, but let's add it:
               if (this.AvatarAnimation != null)
               {
                    this.AvatarAnimation.Draw(avatarRenderer);
               }

               #endregion

               #region Shadowing

               // Begin Shadowing.
               if (isShadowEnabled)
               {
                    // Make the light sources of the avatar dark.
                    Vector3 ambientColor = this.avatarRenderer.AmbientLightColor;
                    Vector3 lightColor = this.avatarRenderer.LightColor;
                    this.avatarRenderer.AmbientLightColor = this.avatarRenderer.LightColor =
                              -5000 * Vector3.One;

                    // Enable alpha blending.

                    // Doesn't work in XNA 4.0:
                    //GraphicsDevice.RenderState.AlphaBlendEnable = true;
                    //GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
                    //GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;

                    // Change To: No Idea!
                    GraphicsDevice.BlendState = BlendState.AlphaBlend;
                    GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                    GraphicsDevice.BlendState.ColorSourceBlend = Blend.SourceAlpha;

                    // Change the depth bias just a bit to avoid z-fighting.

                    // Doesn't work in XNA 4.0:
                    //float sourceDepthBias = GraphicsDevice.RenderState.DepthBias;
                    //GraphicsDevice.RenderState.DepthBias = -0.0001f;

                    // Change To: No Idea!

                    // Set the new light direction.
                    this.avatarRenderer.LightDirection = Vector3.Normalize(
                        Vector3.Right * 7.5f * (float)Math.Cos(lightRotation) +
                        Vector3.Forward * 15.0f * (float)Math.Sin(lightRotation) +
                        Vector3.Up * 10.0f);

                    // If the avatar is stepping over the floor, then move the plane 
                    // according to the "altitude" of the avatar in the world so as
                    // to calculate and cast shadows in the correct world position
                    // (also, take into account that in case of a "jump" movement, in a 
                    // "complete" shadow system you must reposition the shadow along the 
                    // floor taking into account the place where the light-ray hits the 
                    // floor while it points to the avatar; otherwise, it will stand still 
                    // as if the avatar never jumped in the first place).
                    this.plane.D = -this.avatarRenderer.World.Translation.Y;

                    // Calculate and set the world transform that will flatten the 
                    // avatar's geometry, taking into account the original rotation,
                    // scale and translation factors.
                    Matrix world = this.avatarRenderer.World;
                    this.avatarRenderer.World *= Matrix.CreateShadow(
                           this.avatarRenderer.LightDirection,
                           this.plane);

                    // No need for switch-case anymore!!!
                    this.AvatarAnimation.Draw(avatarRenderer);

                    // Reset all affected values.
                    this.avatarRenderer.World = world;
                    this.avatarRenderer.AmbientLightColor = ambientLightColor;
                    this.avatarRenderer.LightColor = lightColor;

                    // Doesn't work in XNA 4.0:
                    //GraphicsDevice.RenderState.DepthBias = sourceDepthBias;
                    //GraphicsDevice.RenderState.AlphaBlendEnable = false;

                    // Change To: ???
                    GraphicsDevice.BlendState = BlendState.Opaque;
                    GraphicsDevice.DepthStencilState = DepthStencilState.Default;

               }
               // End Shadowing.

               #endregion
          }

          /// <summary>
          /// Overriden Component Draw. Calls AvatarRenderer.Draw if the AvatarRender is not null.
          /// </summary>
          public void DrawToScreen(GameTime gameTime, Vector3 fakeCameraPosition, Vector3 renderPosition)
          {
               // Can't render if Avatar Renderer is null!
               if (avatarRenderer == null)
                    return;

               #region World, Projection, and View Set-Up

               // Ensure AvatarRenderer knows the Avatar's camera-based location.
               avatarRenderer.World =
                        Matrix.CreateTranslation(renderPosition) *
                        Matrix.CreateRotationY(MathHelper.ToRadians(0.0f)) *
                        Matrix.CreateScale(1f);

               avatarRenderer.Projection = CameraManager.ActiveCamera.ProjectionMatrix;

               Vector3 LookAt = new Vector3(0f, 0f, 20f);

               avatarRenderer.View = Matrix.CreateLookAt(fakeCameraPosition, LookAt, Vector3.Up);

               #endregion

               #region Lighting

               // If we have Lighting Enabled, use specified light values.
               if (lightingEnabled)
               {
                    avatarRenderer.AmbientLightColor = this.AmbientLightColor;
                    avatarRenderer.LightColor = this.LightColor;
                    avatarRenderer.LightDirection = this.LightDirection;
               }

               // If not Enabled, just use the Default values XNA provides.
               else
               {
                    avatarRenderer.LightDirection = DefaultLightDirection;
                    avatarRenderer.LightColor = DefaultLightColor;
                    avatarRenderer.AmbientLightColor = DefaultAmbient;
               }

               #endregion

               #region Animation & Rendering

               // No need for switch-case anymore!!!
               this.AvatarAnimation.Draw(avatarRenderer);

               #endregion

               #region Shadowing

               // Begin Shadowing.
               if (isShadowEnabled)
               {
                    // Make the light sources of the avatar dark.
                    Vector3 ambientColor = this.avatarRenderer.AmbientLightColor;
                    Vector3 lightColor = this.avatarRenderer.LightColor;
                    this.avatarRenderer.AmbientLightColor = this.avatarRenderer.LightColor =
                              -5000 * Vector3.One;

                    // Enable alpha blending.
                    // Doesn't work in XNA 4.0:
                    //GraphicsDevice.RenderState.AlphaBlendEnable = true;
                    //GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
                    //GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;

                    // Change To: ???
                    GraphicsDevice.BlendState = BlendState.AlphaBlend;
                    GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                    // Change the depth bias just a bit to avoid z-fighting.
                    // Doesn't work in XNA 4.0:
                    //float sourceDepthBias = GraphicsDevice.RenderState.DepthBias;
                    //GraphicsDevice.RenderState.DepthBias = -0.0001f;

                    // Set the new light direction.
                    this.avatarRenderer.LightDirection = Vector3.Normalize(
                        Vector3.Right * 7.5f * (float)Math.Cos(lightRotation) +
                        Vector3.Forward * 15.0f * (float)Math.Sin(lightRotation) +
                        Vector3.Up * 10.0f);

                    // If the avatar is stepping over the floor, then move the plane 
                    // according to the "altitude" of the avatar in the world so as
                    // to calculate and cast shadows in the correct world position
                    // (also, take into account that in case of a "jump" movement, in a 
                    // "complete" shadow system you must reposition the shadow along the 
                    // floor taking into account the place where the light-ray hits the 
                    // floor while it points to the avatar; otherwise, it will stand still 
                    // as if the avatar never jumped in the first place).
                    this.plane.D = -this.avatarRenderer.World.Translation.Y;

                    // Calculate and set the world transform that will flatten the 
                    // avatar's geometry, taking into account the original rotation,
                    // scale and translation factors.
                    Matrix world = this.avatarRenderer.World;
                    this.avatarRenderer.World *= Matrix.CreateShadow(
                           this.avatarRenderer.LightDirection,
                           this.plane);

                    // No need for switch-case anymore!!!
                    this.AvatarAnimation.Draw(avatarRenderer);

                    // Reset all affected values.
                    this.avatarRenderer.World = world;
                    this.avatarRenderer.AmbientLightColor = ambientLightColor;
                    this.avatarRenderer.LightColor = lightColor;

                    // Doesn't work in XNA 4.0:
                    //GraphicsDevice.RenderState.DepthBias = sourceDepthBias;
                    //GraphicsDevice.RenderState.AlphaBlendEnable = false;

                    // Change To:
                    GraphicsDevice.BlendState = BlendState.Opaque;
                    GraphicsDevice.DepthStencilState = DepthStencilState.Default;
               }
               // End Shadowing.

               #endregion
          }

          #endregion

          #region Animation Playing

          public AvatarBaseAnimation PlayMultipleAnimation(bool loopAnimation, AvatarRenderer ar, List<AvatarBaseAnimation> animations, List<List<AvatarBone>> bones)
          {
               AvatarBaseAnimation theAnimation = null;

               // ...So create the animation as such!
               theAnimation = new AvatarMultipleAnimation(ar, animations, bones);

               // Set this as our Avatar's current animation.
               this.AvatarAnimation = (AvatarMultipleAnimation)theAnimation;

               isAnimationLooping = loopAnimation;
               isAnimating = true;

               //((AvatarMultipleAnimation)(this.AvatarAnimation)).CurrentPosition = TimeSpan.Zero;
               this.AvatarAnimation.CurrentPosition = TimeSpan.Zero;

               ((AvatarMultipleAnimation)(this.AvatarAnimation)).Expression = new AvatarExpression();

               return theAnimation;
          }

          public AvatarBaseAnimation PlayMultipleAnimation(List<bool> loopThisAnimation, AvatarRenderer ar, List<AvatarBaseAnimation> animations, List<List<AvatarBone>> bones)
          {
               AvatarBaseAnimation theAnimation = null;

               // ...So create the animation as such!
               theAnimation = new AvatarMultipleAnimation(loopThisAnimation, ar, animations, bones);

               // Set this as our Avatar's current animation.
               this.AvatarAnimation = (AvatarMultipleAnimation)theAnimation;

               isAnimationLooping = true;// loopAnimation;
               isAnimating = true;

               //((AvatarMultipleAnimation)(this.AvatarAnimation)).CurrentPosition = TimeSpan.Zero;
               this.AvatarAnimation.CurrentPosition = TimeSpan.Zero;

               ((AvatarMultipleAnimation)(this.AvatarAnimation)).Expression = new AvatarExpression();

               return theAnimation;
          }

          /// <summary>
          /// Start playing the specified animation.
          /// </summary>
          /// <param name="animation">The animation to be played.</param>
          public AvatarBaseAnimation PlayAnimation(AnimationType animation, bool loopAnimation)
          {
               AvatarBaseAnimation theAnimation = null;

               if (AvatarManager.LoadedAvatarAnimationData.ContainsKey(animation.ToString()))
               {
                    theAnimation = new AvatarCustomAnimation(AvatarManager.LoadedAvatarAnimationData[animation.ToString()]);

                    // NEW THE ANSWER TO MY PRAYERS?!?!?!
                    this.AvatarAnimation = (AvatarCustomAnimation)theAnimation;
               }

               isAnimationLooping = loopAnimation;
               isAnimating = true;

               // NEW ANSWER TO PRAYERS
               //((AvatarCustomAnimation)(this.AvatarAnimation)).CurrentPosition = TimeSpan.Zero;
               //((AvatarCustomAnimation)(this.AvatarAnimation)).Expression = new AvatarExpression();
               this.AvatarAnimation.CurrentPosition = TimeSpan.Zero;
               ((AvatarCustomAnimation)this.AvatarAnimation).Expression = new AvatarExpression();

               return theAnimation;
          }

          /// <summary>
          /// 
          /// </summary>
          /// <param name="avatarAnimationPreset"></param>
          /// <param name="loopAnimation"></param>
          public void PlayAnimation(AvatarAnimationPreset avatarAnimationPreset, bool loopAnimation)
          {
               AvatarAnimation = new AvatarBaseAnimation(avatarAnimationPreset);

               isAnimationLooping = loopAnimation;
               isAnimating = true;
          }

          /// <summary>
          /// 
          /// </summary>
          /// <param name="avatarAnim"></param>
          /// <param name="loopAnimation"></param>
          public void PlayAnimation(AvatarBaseAnimation avatarAnim, bool loopAnimation)
          {
               AvatarAnimation = avatarAnim;

               isAnimationLooping = loopAnimation;
               isAnimating = true;
          }

          /// <summary>
          /// ONLY USED ON CREDITS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
          /// </summary>
          /// <param name="avatarAnim"></param>
          /// <param name="loopAnimation"></param>
          /// <returns></returns>
          public AvatarBaseAnimation PlayAndReturnAnimation(AvatarBaseAnimation avatarAnim, bool loopAnimation)
          {
               AvatarBaseAnimation theAnimation = avatarAnim;

               AvatarAnimation = theAnimation;
               isAnimationLooping = loopAnimation;
               isAnimating = true;

               return theAnimation;
          }

          /// <summary>
          /// THIS ONE IS NOT USED YET!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
          /// </summary>
          /// <param name="assetName"></param>
          public void PlayAnimation(string assetName)
          {
               // Load custom animations
               CustomAvatarAnimationData animationData;

               // Load the Throw animation.
               animationData = EngineCore.Game.Content.Load<CustomAvatarAnimationData>(@assetName);

               //customAnimation = new CustomAvatarAnimationPlayer(animationData.Name, animationData.Length, animationData.Keyframes);
          }

          #endregion

          #region Avatar Loading & Unloading

          /// <summary>
          /// Loads player one's avatar. If the user does not sign in then 
          /// after 3 seconds a random one is used
          /// </summary>
          public void LoadUserAvatar(PlayerIndex playerIndex, GameTime gameTime)
          {
               bool userLoaded = TryToLoadUserAvatar(playerIndex);

               // If they have not logged in after 3 seconds load a random avatar
               if (!userLoaded && gameTime.TotalGameTime.TotalSeconds > 3)
               {
                    LoadRandomAvatar();
               }
          }

          /// <summary>
          /// Loads player one's avatar. If the user does not sign in then 
          /// after 3 seconds a random one is used
          /// </summary>
          public void LoadUserAvatar(PlayerIndex playerIndex)
          {
               bool userLoaded = TryToLoadUserAvatar(playerIndex);

               // If they have not logged in after 3 seconds load a random avatar
               if (!userLoaded)
               {
                    LoadRandomAvatar();
               }
          }

          /// <summary>
          /// Load player one's avatar
          /// </summary>
          private bool TryToLoadUserAvatar(PlayerIndex playerIndex)
          {
               UnloadAvatar();

               // Check to see if the user is signed in
               if (Gamer.SignedInGamers[playerIndex] != null)
               {
                    // Get the users avatar description

                    // Doesn't work in XNA 4.0:
                    //avatarDescription = Gamer.SignedInGamers[playerIndex].Avatar;

                    // Change To: Semi-Positive
                    IAsyncResult avatarResult;
                    avatarResult = AvatarDescription.BeginGetFromGamer(Gamer.SignedInGamers[playerIndex], null, null);
                    avatarDescription = AvatarDescription.EndGetFromGamer(avatarResult);

                    // Is this description valid
                    if (avatarDescription.IsValid)
                    {
                         avatarRenderer = new AvatarRenderer(avatarDescription);
                    }

                    // If the description is not valid that means the user does not 
                    // have an avatar and we should load a random one
                    else
                    {
                         LoadRandomAvatar();
                    }

                    return true;
               }

               return false;
          }

          /// <summary>
          /// Load a random avatar
          /// </summary>
          public void LoadRandomAvatar()
          {
               UnloadAvatar();

               avatarDescription = AvatarDescription.CreateRandom();
               avatarRenderer = new AvatarRenderer(avatarDescription);
          }

          /// <summary>
          /// Load a random avatar of a specified gender.
          /// </summary>
          /// <param name="gender">The gender of the random avatar to be loaded..</param>
          public void LoadRandomAvatar(AvatarBodyType gender)
          {
               UnloadAvatar();

               avatarDescription = AvatarDescription.CreateRandom(gender);
               avatarRenderer = new AvatarRenderer(avatarDescription);
          }

          /// <summary>
          /// Load a Custom Avatar using the CustomAvatarPreset enum.
          /// </summary>
          /// <param name="avatarType">The pre-made avatar to be loaded.</param>
          public void LoadAvatar(CustomAvatarType avatarType)
          {
               UnloadAvatar();

               switch (avatarType)
               {
                    #region Switch through Custom Avatars

                    case CustomAvatarType.Matt:
                         avatarDescription = new AvatarDescription(AvatarDatabase.MattsAvatarDescription);
                         break;

                    case CustomAvatarType.DeathHawk:
                         avatarDescription = new AvatarDescription(AvatarDatabase.DeathhawkAvatarDescription);
                         break;

                    case CustomAvatarType.Killthief:
                         avatarDescription = new AvatarDescription(AvatarDatabase.KillthiefAvatarDescription);
                         break;

                    case CustomAvatarType.Mach:
                         avatarDescription = new AvatarDescription(AvatarDatabase.MachAvatarDescription);
                         break;

                    case CustomAvatarType.Sylent:
                         avatarDescription = new AvatarDescription(AvatarDatabase.SylentAvatarDescription);
                         break;

                    case CustomAvatarType.Mike:
                         avatarDescription = new AvatarDescription(AvatarDatabase.MikeAvatarDescription);
                         break;

                    case CustomAvatarType.Apotheosis:
                         avatarDescription = new AvatarDescription(AvatarDatabase.ApotheosisAvatarDescription);
                         break;

                    case CustomAvatarType.Daniel:
                         avatarDescription = new AvatarDescription(AvatarDatabase.DanAvatarDescription);
                         break;

                    case CustomAvatarType.Bossk:
                         avatarDescription = new AvatarDescription(AvatarDatabase.BosskAvatarDescription);
                         break;

                    case CustomAvatarType.CP30:
                         avatarDescription = new AvatarDescription(AvatarDatabase.CP30AvatarDescription);
                         break;

                    case CustomAvatarType.Chewbacca:
                         avatarDescription = new AvatarDescription(AvatarDatabase.ChewbaccaAvatarDescription);
                         break;

                    case CustomAvatarType.HanSoloHothOutfit:
                         avatarDescription = new AvatarDescription(AvatarDatabase.HanSoloHothAvatarDescription);
                         break;

                    case CustomAvatarType.ImperialSnowtrooper:
                         avatarDescription = new AvatarDescription(AvatarDatabase.ImperialSnowtrooperAvatarDescription);
                         break;

                    case CustomAvatarType.DarkLordArmor:
                         avatarDescription = new AvatarDescription(AvatarDatabase.DarkLordAvatarDescription);
                         break;

                    case CustomAvatarType.CogArmor:
                         avatarDescription = new AvatarDescription(AvatarDatabase.CogArmorAvatarDescription);
                         break;

                    case CustomAvatarType.StormtrooperOutfit:
                         avatarDescription = new AvatarDescription(AvatarDatabase.StormtrooperAvatarDescription);
                         break;

                    case CustomAvatarType.BritishCavalry:
                         avatarDescription = new AvatarDescription(AvatarDatabase.BritishCavalryAvatarDescription);
                         break;

                    case CustomAvatarType.ArabianOutfit:
                         avatarDescription = new AvatarDescription(AvatarDatabase.ArabianAvatarDescription);
                         break;

                    case CustomAvatarType.NinjaBeeOutfit:
                         avatarDescription = new AvatarDescription(AvatarDatabase.NinjaBeeAvatarDescription);
                         break;

                    case CustomAvatarType.PerfectAgentOutfit:
                         avatarDescription = new AvatarDescription(AvatarDatabase.PerfectAgentAvatarDescription);
                         break;

                    case CustomAvatarType.Cowboy:
                         avatarDescription = new AvatarDescription(AvatarDatabase.CowboyAvatarDescription);
                         break;

                    case CustomAvatarType.BunnyOutfit:
                         avatarDescription = new AvatarDescription(AvatarDatabase.BunnyAvatarDescription);
                         break;

                    case CustomAvatarType.ArmyRanger:
                         avatarDescription = new AvatarDescription(AvatarDatabase.ArmyRangerAvatarDescription);
                         break;

                    case CustomAvatarType.LabCoat:
                         avatarDescription = new AvatarDescription(AvatarDatabase.LabcoatAvatarDescription);
                         break;

                    case CustomAvatarType.BritishElite:
                         avatarDescription = new AvatarDescription(AvatarDatabase.BritishEliteAvatarDescription);
                         break;

                    case CustomAvatarType.HotdogOutfit:
                         avatarDescription = new AvatarDescription(AvatarDatabase.HotdogAvatarDescription);
                         break;

                    case CustomAvatarType.SplintercellOutfit:
                         avatarDescription = new AvatarDescription(AvatarDatabase.SplintercellAvatarDescription);
                         break;

                    case CustomAvatarType.DanteCostume:
                         avatarDescription = new AvatarDescription(AvatarDatabase.DanteAvatarDescription);
                         break;

                    case CustomAvatarType.GermanElite:
                         avatarDescription = new AvatarDescription(AvatarDatabase.GermanEliteAvatarDescription);
                         break;

                    case CustomAvatarType.MummyOutfit:
                         avatarDescription = new AvatarDescription(AvatarDatabase.MummyAvatarDescription);
                         break;

                    case CustomAvatarType.RangerArmor:
                         avatarDescription = new AvatarDescription(AvatarDatabase.RangerAvatarDescription);
                         break;

                    case CustomAvatarType.WarArmor:
                         avatarDescription = new AvatarDescription(AvatarDatabase.WarArmorAvatarDescription);
                         break;

                    case CustomAvatarType.BigSisterOutfit:
                         avatarDescription = new AvatarDescription(AvatarDatabase.BigSisterAvatarDescription);
                         break;

                    case CustomAvatarType.ApprenticeOutfit:
                         avatarDescription = new AvatarDescription(AvatarDatabase.ApprenticeAvatarDescription);
                         break;

                    case CustomAvatarType.WerewolfOutfit:
                         avatarDescription = new AvatarDescription(AvatarDatabase.WerewolfAvatarDescription);
                         break;

                    case CustomAvatarType.Assassin_Black:
                         avatarDescription = new AvatarDescription(AvatarDatabase.BlackAssassinAvatarDescription);
                         break;

                    case CustomAvatarType.Assassin_White:
                         avatarDescription = new AvatarDescription(AvatarDatabase.WhiteAssassinAvatarDescription);
                         break;

                    case CustomAvatarType.Guard_Medieval:
                         avatarDescription = new AvatarDescription(AvatarDatabase.GuardAvatarDescription);
                         break;

                    case CustomAvatarType.Pirate_Red:
                         avatarDescription = new AvatarDescription(AvatarDatabase.PirateBlackHatAvatarDescription);
                         break;

                    case CustomAvatarType.JailhouseOnePiece:
                         avatarDescription = new AvatarDescription(AvatarDatabase.PalePrisonerAvatarDescription);
                         break;

                    case CustomAvatarType.Monster:
                         avatarDescription = new AvatarDescription(AvatarDatabase.ShortMonsterAvatarDescription);
                         break;

                    case CustomAvatarType.MariachiOutfit:
                         avatarDescription = new AvatarDescription(AvatarDatabase.MexicanAvatarDescription);
                         break;

                    case CustomAvatarType.Palin:
                         avatarDescription = new AvatarDescription(AvatarDatabase.SarahPalinAvatarDescription);
                         break;

                    case CustomAvatarType.GeorgeBush:
                         avatarDescription = new AvatarDescription(AvatarDatabase.GeorgeBushAvatarDescription);
                         break;

                    case CustomAvatarType.McCain:
                         avatarDescription = new AvatarDescription(AvatarDatabase.McCainAvatarDescription);
                         break;

                    case CustomAvatarType.Lincoln:
                         avatarDescription = new AvatarDescription(AvatarDatabase.LincolnAvatarDescription);
                         break;

                    case CustomAvatarType.Mulder:
                         avatarDescription = new AvatarDescription(AvatarDatabase.MulderAvatarDescription);
                         break;

                    case CustomAvatarType.Scully:
                         avatarDescription = new AvatarDescription(AvatarDatabase.ScullyAvatarDescription);
                         break;

                    case CustomAvatarType.Norris:
                         avatarDescription = new AvatarDescription(AvatarDatabase.NorrisAvatarDescription);
                         break;

                    case CustomAvatarType.BillyMays:
                         avatarDescription = new AvatarDescription(AvatarDatabase.BillyMaysAvatarDescription);
                         break;

                    case CustomAvatarType.Lennon:
                         avatarDescription = new AvatarDescription(AvatarDatabase.LennonAvatarDescription);
                         break;

                    case CustomAvatarType.Cobain:
                         avatarDescription = new AvatarDescription(AvatarDatabase.CobainAvatarDescription);
                         break;

                    case CustomAvatarType.ColonelSanders:
                         avatarDescription = new AvatarDescription(AvatarDatabase.ColonelSandersAvatarDescription);
                         break;

                    case CustomAvatarType.MonopolyGuy:
                         avatarDescription = new AvatarDescription(AvatarDatabase.MonopolyGuyAvatarDescription);
                         break;

                    case CustomAvatarType.HankHill:
                         avatarDescription = new AvatarDescription(AvatarDatabase.HankHillAvatarDescription);
                         break;

                    case CustomAvatarType.Mario:
                         avatarDescription = new AvatarDescription(AvatarDatabase.MarioAvatarDescription);
                         break;

                    case CustomAvatarType.DukeNukem:
                         avatarDescription = new AvatarDescription(AvatarDatabase.DukeNukemAvatarDescription);
                         break;

                    case CustomAvatarType.Jester_Purple:
                         avatarDescription = new AvatarDescription(AvatarDatabase.JesterPurpleAvatarDescription);
                         break;

                    case CustomAvatarType.Jester_Red:
                         avatarDescription = new AvatarDescription(AvatarDatabase.JesterRedAvatarDescription);
                         break;

                    case CustomAvatarType.HuntressUniform:
                         avatarDescription = new AvatarDescription(AvatarDatabase.HuntressAvatarDescription);
                         break;

                    case CustomAvatarType.ODSTArmor:
                         avatarDescription = new AvatarDescription(AvatarDatabase.ODSTAvatarDescription);
                         break;

                    case CustomAvatarType.MadHatter:
                         avatarDescription = new AvatarDescription(AvatarDatabase.MadHatterAvatarDescription);
                         break;

                    case CustomAvatarType.Leprechaun:
                         avatarDescription = new AvatarDescription(AvatarDatabase.LeprechaunAvatarDescription);
                         break;

                    #endregion

                    default:
                         avatarDescription = AvatarDescription.CreateRandom();
                         break;
               }

               if (!avatarDescription.IsValid)
               {
                    avatarDescription = AvatarDescription.CreateRandom();
               }

               avatarRenderer = new AvatarRenderer(avatarDescription);
          }

          /// <summary>
          /// Unloads the current avatar.
          /// </summary>
          private void UnloadAvatar()
          {
               // Dispose the current Avatar.
               if (avatarRenderer != null)
               {
                    avatarRenderer.Dispose();
                    avatarRenderer = null;
               }
          }

          #endregion

          #region Simplistic Speak Animation

          /// <summary>
          /// Makes the Avatar appear to be speaking. Very innacurate for true lip-synching.
          /// </summary>
          public void Speak()
          {
               currentFacialExpression.Mouth = (AvatarMouth)random.Next(7, 14);
          }

          /// <summary>
          /// // Makes the Avatar speak--this time in a more accurate, customized manner.
          /// </summary>
          public void Speak(AvatarMouth phonetic)
          {
               currentFacialExpression.Mouth = phonetic;
          }

          #endregion

          #region Expression-Based Animated Actions

          /// <summary>
          /// Makes the Avatar perform a "Blinking" action, via changes to AvatarExpression.AvatarEye.
          /// </summary>
          public void Blink_Left()
          {
               currentFacialExpression.LeftEye = AvatarEye.Blink;
          }

          /// <summary>
          /// Makes the Avatar perform a "Blinking" action, via changes to AvatarExpression.AvatarEye.
          /// </summary>
          public void Blink_Right()
          {
               currentFacialExpression.RightEye = AvatarEye.Blink;
          }

          /// <summary>
          /// Makes the Avatar perform a "Laughing" action, via changes to AvatarExpression.Mouth.
          /// </summary>
          public void Laugh()
          {
               currentFacialExpression.Mouth = AvatarMouth.Laughing;
          }

          #endregion

          #region Helper BonesToWorldSpace - for putting objects on Avatars

          /// <summary>
          /// Updates a list of matrices to represent the location of the 
          /// avatar bones in world space with the avatar animation applied.
          /// </summary>
          public void BonesToWorldSpace(AvatarRenderer renderer, AvatarBaseAnimation animation,
                                                          List<Matrix> boneToUpdate)
          {
               // I ADDED TIHS 3-1-2011
               if (boneToUpdate == null)
                    return;
               // I ADDED THAT

               // Bind pose of the avatar. 
               // These positions are in local space, and are relative to the parent bone.
               IList<Matrix> bindPose = renderer.BindPose;

               IList<Matrix> animationPose;

               // The current animation pose. 
               // These positions are in local space, and are relative to the parent bone.
               animationPose = animation.CustomBoneTransforms;

               // List of parent bones for each bone in the hierarchy 
               IList<int> parentIndex = renderer.ParentBones;

               // Loop all of the bones.
               // Since the bone hierarchy is sorted by depth 
               // we will transform the parent before any child.
               for (int i = 0; i < AvatarRenderer.BoneCount; i++)
               {
                    // Find the transform of this bones parent.
                    // If this is the first bone use the world matrix used on the avatar
                    Matrix parentMatrix =  (parentIndex[i] != -1)
                                           ? boneToUpdate[parentIndex[i]] : renderer.World;

                    // Calculate this bones world space position
                    boneToUpdate[i] = 
                         Matrix.Multiply(Matrix.Multiply(animationPose[i], bindPose[i]), parentMatrix);
               }
          }
          /// <summary>
          /// Updates a list of matrices to represent the location of the 
          /// avatar bones in world space with the avatar animation applied.
          /// </summary>
          public void BonesToWorldSpace(AvatarRenderer renderer, IList<Matrix> boneTransforms,
                                                          List<Matrix> boneToUpdate)
          {
               // I ADDED TIHS 3-1-2011
               if (boneToUpdate == null)
                    return;
               // I ADDED THAT

               // Bind pose of the avatar. 
               // These positions are in local space, and are relative to the parent bone.
               IList<Matrix> bindPose = renderer.BindPose;

               // The current animation pose. 
               // These positions are in local space, and are relative to the parent bone.
               IList<Matrix> animationPose = boneTransforms;

               // List of parent bones for each bone in the hierarchy 
               IList<int> parentIndex = renderer.ParentBones;

               // Loop all of the bones.
               // Since the bone hierarchy is sorted by depth 
               // we will transform the parent before any child.
               for (int i = 0; i < AvatarRenderer.BoneCount; i++)
               {
                    // Find the transform of this bones parent.
                    // If this is the first bone use the world matrix used on the avatar
                    Matrix parentMatrix = (parentIndex[i] != -1)
                                           ? boneToUpdate[parentIndex[i]]
                                           : renderer.World;
                    // Calculate this bones world space position

                    boneToUpdate[i] = Matrix.Multiply(Matrix.Multiply(animationPose[i],
                                                                      bindPose[i]),
                                                                      parentMatrix);
               }
          }

          #endregion
     }
}
