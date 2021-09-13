#region File Description
//-----------------------------------------------------------------------------
// EnemyIntroductionScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine;
using PixelEngine.Screen;
using PixelEngine.Text;
#endregion

namespace AvatarTyping
{
     public class EnemyIntroductionScreen : PagedMenuScreen
     {
          #region Fields

          private ContentManager content;
          private Texture2D backgroundTexture;

          private TypingEnemy enemy;
          private EnemyType enemyType;
          private string introText = String.Empty;

          EnemyManager manager = new EnemyManager(EngineCore.Game);

          TypingEffect txtEffect = new TypingEffect("", 25.0f);

          TextObject generalInfoText = new TextObject("", 
               new Vector2(EngineCore.ScreenCenter.X, EngineCore.ScreenCenter.Y - 25),
                    TextManager.FontType.MenuFont, Color.Gold, 0.0f, Vector2.Zero, 0.75f, true, new TypingEffect("", 15f));

          #endregion

          #region Properties

          public TypingEnemy Enemy
          {
               get { return enemy; }
               set { enemy = value; }
          }

          public string IntroText
          {
               get { return introText; }
               set { introText = value; }
          }

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public EnemyIntroductionScreen(EnemyType enemyToIntro, string enemyIntroText)
               : base(enemyIntroText, 3, 1)
          {
               /*
               CameraManager.SetActiveCamera(CameraManager.CameraNumber.ThirdPerson);
               CameraManager.ActiveCamera.Reset(EngineCore.GraphicsDevice.Viewport);
               CameraManager.ActiveCamera.Position = new Vector3(0, 2f, -3f);
               CameraManager.ActiveCamera.LookAt = new Vector3(0f, 0f, 20f);
               */

               TransitionOnTime = TimeSpan.FromSeconds(0.5);
               TransitionOffTime = TimeSpan.FromSeconds(0.5);

               enemyType = enemyToIntro;
               IntroText = enemyIntroText;

               for (int i = 0; i < this.NumberOfPages; i ++)
               {
                    MenuEntries[i].Text = "Exit Introduction";
                    MenuEntries[i].Selected += SkipSelected;
                    MenuEntries[i].IsCenter = true;
                    MenuEntries[i].Position = new Vector2(MenuEntries[i].Position.X, MenuEntries[i].DescriptionPosition.Y - 140);
                    MenuEntries[i].ShowBorder = false;
                    MenuEntries[i].SelectedColor = Color.White;
               }

               EnemyManager manager = new EnemyManager(EngineCore.Game);

               switch (enemyType)
               {
                    case EnemyType.Normal:
                         enemy = new NormalTypingEnemy(Vector3.Zero, manager);
                         break;

                    case EnemyType.Fast:
                         enemy = new FastTypingEnemy(Vector3.Zero, manager);
                         break;

                    case EnemyType.Kamikaze:
                         enemy = new SuicideTypingEnemy(Vector3.Zero, manager);
                         break;

                    case EnemyType.Explosive:
                         enemy = new ExplodingTypingEnemy(Vector3.Zero, manager);
                         break;

                    case EnemyType.Deflatable:
                         enemy = new DeflatingTypingEnemy(Vector3.Zero, manager);
                         break;

                    case EnemyType.Dancing:
                         enemy = new DancingTypingEnemy(Vector3.Zero, manager);
                         break;

                    case EnemyType.Backward:
                         enemy = new BackwardTypingEnemy(Vector3.Zero, manager);
                         break;

                    case EnemyType.Horde:
                         enemy = new HordeTypingEnemy(Vector3.Zero, manager);
                         break;

                    case EnemyType.Boss:
                         enemy = new BossTypingEnemy(Vector3.Zero, manager);
                         break;
               }

               enemy.Speed = 0.0f;
               enemy.Avatar.LoadRandomAvatar();
               enemy.Avatar.LightingEnabled = false;

               if (enemyType == EnemyType.Boss)
                    enemy.Avatar.PlayAnimation(AvatarAnimationPreset.MaleAngry, true);

               txtEffect.IsSoundEnabled = true;

               generalInfoText.IsCenter = true;
               generalInfoText.Text = GrabText();

               txtEffect.Text = generalInfoText.Text;
               generalInfoText.TextEffect = txtEffect;
               //generalInfoText.AddTextEffect(txtEffect);
          }

          /// <summary>
          /// Loads graphics content for this screen. The background texture is quite
          /// big, so we use our own local ContentManager to load it. This allows us
          /// to unload before going from the menus into the game itself, wheras if we
          /// used the shared ContentManager provided by the Game class, the content
          /// would remain loaded forever.
          /// </summary>
          public override void LoadContent()
          {
               base.LoadContent();

               if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

               backgroundTexture = content.Load<Texture2D>(@"Textures\Gradients\Gradient_Star");
          }


          /// <summary>
          /// Unloads graphics content for this screen.
          /// </summary>
          public override void UnloadContent()
          {
               content.Unload();
          }

          protected override void OnCancel(PlayerIndex playerIndex)
          {
               base.OnCancel(playerIndex);
          }


          #endregion

          #region Menu Entry Events

          /// <summary>
          /// Event handler for when the Difficulty menu entry is selected.
          /// </summary>
          void NextPageSelected(object sender, PlayerIndexEventArgs e)
          {
               base.OnNextPage();  
          }

          void SkipSelected(object sender, PlayerIndexEventArgs e)
          {
               base.OnCancel(e.PlayerIndex);
          }

          protected override void OnNextPage()
          {
               base.OnNextPage();

               generalInfoText.Text = GrabText();

               txtEffect = new TypingEffect(generalInfoText.Text, 25.0f);
               txtEffect.IsSoundEnabled = true;

               generalInfoText.TextEffect = txtEffect;
               //generalInfoText.AddTextEffect(txtEffect);
          }

          protected override void OnPreviousPage()
          {
               base.OnPreviousPage();

               generalInfoText.Text = GrabText();

               txtEffect = new TypingEffect(generalInfoText.Text, 25.0f);
               txtEffect.IsSoundEnabled = true;

               generalInfoText.TextEffect = txtEffect;
               //generalInfoText.AddTextEffect(txtEffect);
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
               base.Update(gameTime, otherScreenHasFocus, false);

               Enemy.Update(gameTime);

               generalInfoText.Update(gameTime);
          }

          #endregion

          #region Draw

          /// <summary>
          /// Draws the background screen.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
               Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
               Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
               byte fade = TransitionAlpha;

               spriteBatch.GraphicsDevice.Clear(Color.Black);

               spriteBatch.Begin();

               // New
               Color color = Color.CornflowerBlue;
               if (Enemy != null)
               {
                    color = Enemy.Color;

                    if (color == Color.Black)
                    {
                         color = Color.Gray;
                    }
               }
               // End
               spriteBatch.Draw(backgroundTexture, fullscreen, color * (fade / 255f));

               spriteBatch.End();

               DrawEnemy(gameTime);

               // Doesn't work in XNA 4.0:
               //spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
               spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

               DrawIntroduction(gameTime);

               spriteBatch.End();

               base.Draw(gameTime);
          }

          #endregion

          #region Helper Draw Methods

          /// <summary>
          /// Draws the Enemy to be "Introduced".
          /// </summary>
          private void DrawEnemy(GameTime gameTime)
          {
               Enemy.Avatar.Position = new Vector3(0f, 0f, 6f);

               Enemy.DrawWithoutCamera(gameTime, ScreenManager.Font, new Vector3(0f, 1.0f, -5f), new Vector3(0f, 0f, 0f));
          }

          /// <summary>
          /// Draws a String representation of the Enemy's Introduction text.
          /// </summary>
          private void DrawIntroduction(GameTime gameTime)
          {
               generalInfoText.Draw(gameTime);
          }

          private string GrabText()
          {
               switch (enemyType)
               {
                    case EnemyType.Normal:

                         switch (this.CurrentPageNumber)
                         {
                              case 1:
                                   introText = "This is a Normal Enemy.\nThey are represented with White borders around their text.";
                                   break;
                              case 2:
                                   introText = "Normal Enemies have no special characteristics.";
                                   break;
                              case 3:
                                   introText = "Base Score: 50 Points\nSpeed Kill: + 50 Points\nPerfect Kill: + 50 Points";
                                   break;
                         }
                         break;

                    case EnemyType.Fast:

                         switch (this.CurrentPageNumber)
                         {
                              case 1:
                                   introText = "This is a Speedster Enemy.\nThey are represented with Green borders around their text.";
                                   break;
                              case 2:
                                   introText = "Speedster Enemies are 50% faster than Normal Enemies.";
                                   break;
                              case 3:
                                   introText = "Base Score: 100 Points\nSpeed Kill: + 100 Points\nPerfect Kill: + 100 Points";
                                   break;
                         }
                         break;

                    case EnemyType.Kamikaze:

                         switch (this.CurrentPageNumber)
                         {
                              case 1:
                                   introText = "This is a Kamikaze Enemy.\nThey are represented with Red borders around their text.";
                                   break;
                              case 2:
                                   introText = "Kamikaze Enemies have zero tolerance for mistypes;\nthey will faint upon an incorrect key-stroke!\nConsequently, no Points will be awarded to the player.";
                                   break;
                              case 3:
                                   introText = "Base Score: 150 Points\nSpeed Kill: + 150 Points\nPerfect Kill: + 150 Points";
                                   break;
                         }
                         break;

                    case EnemyType.Explosive:

                         switch (this.CurrentPageNumber)
                         {
                              case 1:
                                   introText = "This is an Explosive Enemy.\nThey are represented with Yellow borders around their text.";
                                   break;
                              case 2:
                                   introText = "Explosive Enemies increase in size until they finally burst.\n" +
                                               "If no Bonus is earned upon the enemy's defeat, the enemy's\n" +
                                               "explosion damages the player.\n" +
                                               "Otherwise, nearby enemies explode!";
                                   break;
                              case 3:
                                   introText = "Base Score: 200 Points\nSpeed Kill: + 200 Points\nPerfect Kill: + 200 Points";
                                   break;
                         }
                         break;

                    case EnemyType.Deflatable:

                         switch (this.CurrentPageNumber)
                         {
                              case 1:
                                   introText = "This is a Deflatable Enemy.\nThey are represented with Blue borders around their text.";
                                   break;
                              case 2:
                                   introText = "Deflatable Enemies decrease in size with each correct key-stroke.\n" +
                                               "Reducing the enemy to pint-size will cause him to faint.\n" +
                                               "Deflatable Enemies wield multiple sentences that must be typed.";
                                   break;
                              case 3:
                                   introText = "Base Score: 100 Points per sentence\nSpeed Kill: + 100 Points per sentence\nPerfect Kill: + 100 Points per sentence";
                                   break;
                         }
                         break;

                    case EnemyType.Dancing:

                         switch (this.CurrentPageNumber)
                         {
                              case 1:
                                   introText = "This is a Dancing Enemy.\nThey are represented with Pink borders around their text.";
                                   break;
                              case 2:
                                   introText = "Dancing Enemies have infectious charisma;\n" +
                                               "target them and all other enemies on screen will cheer in place.\n" +
                                               "When the Dancing Enemy is gone, the cheering is over!";
                                   break;
                              case 3:
                                   introText = "Base Score: 250 Points\nSpeed Kill: + 250 Points\nPerfect Kill: + 250 Points";
                                   break;
                         }
                         break;

                    case EnemyType.Backward:

                         switch (this.CurrentPageNumber)
                         {
                              case 1:
                                   introText = "This is a Backwards Enemy.\nThey are represented with Black borders around their text.";
                                   break;
                              case 2:
                                   introText = "Backwards Enemies love doing things backwards;\nthey even run backwards!\n" +
                                               "Their desire for everything backwards causes one nearby\n" + 
                                               "enemy to display his sentence in reverse order.\n";
                                   break;
                              case 3:
                                   introText = "Base Score: 250 Points\nSpeed Kill: + 250 Points\nPerfect Kill: + 250 Points";
                                   break;
                         }
                         break;

                    case EnemyType.Horde:

                         switch (this.CurrentPageNumber)
                         {
                              case 1:
                                   introText = "This is a Horde Enemy.\nThey are represented with Black borders around their text.";
                                   break;
                              case 2:
                                   introText = "Horde Enemies travel in large groups together. Their sentences contain\n" +
                                               "only one character, but they are tremendously fast.\n" +
                                               "Is the Horde free points, or certain death?";
                                   break;
                              case 3:
                                   introText = "Base Score: 250 Points\nSpeed Kill: + 250 Points\nPerfect Kill: + 250 Points";
                                   break;
                         }
                         break;

                    case EnemyType.Boss:

                         switch (this.CurrentPageNumber)
                         {
                              case 1:
                                   introText = "This is a Boss Enemy.\nThey are represented with Light Blue borders around their text.";
                                   break;
                              case 2:
                                   introText = "Boss Enemies wield many sentences - not just one!\n" +
                                               "Every so often they will throw projectiles toward the player.\n" +
                                               "Typing the projectile's sentence will destroy it.";
                                   break;
                              case 3:
                                   introText = "Base Score: 10,000 Points\nSpeed Kill: + 500 Points per sentence\nPerfect Kill: + 500 Points per sentence";
                                   break;
                         }
                         break;
               }

               return introText;
          }

          #endregion
     }
}