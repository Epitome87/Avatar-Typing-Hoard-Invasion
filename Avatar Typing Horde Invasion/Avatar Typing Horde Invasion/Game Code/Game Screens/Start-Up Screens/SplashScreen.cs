#region File Description
//-----------------------------------------------------------------------------
// SplashScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine;
using PixelEngine.Audio;
using PixelEngine.Graphics;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
#endregion

namespace AvatarTyping
{
     /// <summary>
     /// The Splash screen is a simple screen which runs upon game start-up.
     /// The screen does not accept input; it merely displays for a few seconds,
     /// during which time the Studio Logo and Copyright information are displayed.
     /// </summary>
     public class SplashScreen : GameScreen
     {
          #region Fields

          SpriteBatch spriteBatch;
          ContentManager content;

          GameResourceFont ChatFont;
          Vector2 textPosition = new Vector2(1240 / 2, 0);
          float elapsedTime;
          float elapsedFrameTime;

          const int frames = 83;
          int currentFrame = 0;
          Texture2D[] studioLogo = new Texture2D[frames];
          Texture2D currentTexture;

          Texture2D[] butterflyTextures = new Texture2D[24];

          Texture2D[] currentButterflyTexture = new Texture2D[24];

          float[] butterflyRotations = new float[8];
          Color[] butterflyColors = new Color[8];

          const int NumberOfButterflies = 8;

          Random random = new Random();
          Color logoColor;
          Color butterflyColor;
          int colorIndex = 0;

          bool isFlyFaster = false;
          bool isButterfliesMove = true;
          bool isSpawnExtraButterflies = true;

          public struct Butterfly
          {
               public Texture2D Texture;
               public Vector2 Position;
               public float Rotation;
               public float Speed;
               public int CurrentFrame;
               public Color Color;
          };

          Butterfly[] Butterflies = new Butterfly[8];
          Butterfly[,] ExtraButterflies = new Butterfly[8, 8];

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public SplashScreen()
          {
               TransitionOnTime = TimeSpan.FromSeconds(0.5f);
               TransitionOffTime = TimeSpan.FromSeconds(1.0f);

               elapsedTime = 0f;
          }

          public override void LoadContent()
          {
               if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

               ChatFont = ResourceManager.LoadFont(@"Fonts\ChatFont");

               for (int i = 0; i < frames; i++)
               {
                    string textureName = "Studio Logo\\Frame" + i.ToString();
                    studioLogo[i] = content.Load<Texture2D>(@textureName);
               }

               currentTexture = studioLogo[0];

               spriteBatch = ScreenManager.SpriteBatch;

               for (int i = 0; i < 8; i++)
               {
                    string textureName = "Studio Logo\\Butterfly\\PixelButterfly" + (i + 1).ToString();
                    butterflyTextures[i] = content.Load<Texture2D>(@textureName);
               }

               for (int i = 0; i < 8; i++)
               {
                    string textureName = "Studio Logo\\Butterfly\\IntermediateButterfly" + (i + 1).ToString();
                    butterflyTextures[i + 8] = content.Load<Texture2D>(@textureName);
               }

               for (int i = 0; i < 8; i++)
               {
                    string textureName = "Studio Logo\\Butterfly\\SmoothButterfly" + (i + 1).ToString();
                    butterflyTextures[i + 16] = content.Load<Texture2D>(@textureName);
               }


               for (int i = 0; i < NumberOfButterflies; i++)
               {
                    Butterflies[i].Position = EngineCore.ScreenCenter;
               }

               Random random = new Random();

               for (int i = 0; i < NumberOfButterflies; i++)
               {
                    Butterflies[i].CurrentFrame = random.Next(8);
                    Butterflies[i].Texture = butterflyTextures[Butterflies[i].CurrentFrame];
               }

               butterflyRotations[0] = -45f;      // Top Left
               butterflyRotations[1] = -10f;      // Top Center
               butterflyRotations[2] = 45f;       // Top Right
               butterflyRotations[3] = 90f;       // Right
               butterflyRotations[4] = 135f;      // Bottom Right
               butterflyRotations[5] = 180;       // Bottom Center
               butterflyRotations[6] = 225f;      // Bottom Left
               butterflyRotations[7] = -90f;      // Left

               butterflyColors[0] = Color.Red;
               butterflyColors[1] = Color.Orange;
               butterflyColors[2] = Color.Yellow;
               butterflyColors[3] = Color.Green;
               butterflyColors[4] = Color.CornflowerBlue;// Color.Blue;
               butterflyColors[5] = Color.Indigo;
               butterflyColors[6] = Color.Violet;
               butterflyColors[7] = Color.Gray;

               for (int i = 0; i < NumberOfButterflies; i++)
               {
                    Butterflies[i].Rotation = butterflyRotations[i];
                    Butterflies[i].Color = butterflyColors[i];
               }

               for (int r = 0; r < NumberOfButterflies; r++)
               {
                    for (int c = 0; c < NumberOfButterflies; c++)
                    {
                         ExtraButterflies[r, c] = new Butterfly();
                         ExtraButterflies[r, c].Rotation = butterflyRotations[c];
                         ExtraButterflies[r, c].Color = butterflyColors[r];
                         ExtraButterflies[r, c].Texture = butterflyTextures[0];
                         ExtraButterflies[r, c].CurrentFrame = 0;
                         ExtraButterflies[r, c].Position = new Vector2();
                    }
               }

               AudioManager.PlayMusic("SplashScreen");
          }

          #endregion

          #region Handle Input

          public override void HandleInput(InputState input)
          {
               if (input == null)
                    throw new ArgumentNullException("input");
          }

          #endregion

          #region Update

          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               if (!IsActive)
                    return;

               // Pulsate the size of the selected menu entry.
               double time = gameTime.TotalGameTime.TotalSeconds;

               float butterflyPositionY = 0.0f;

               butterflyPositionY = -2 * (float)Math.Sin(time * 6);


               // FOR BUTTERFLIES STARTING AT LOGO
               if (isButterfliesMove)
               {
                    float speed = 1.0f;

                    if (isFlyFaster)
                    {
                         speed *= 6.0f;

                         for (int c = 0; c < NumberOfButterflies; c++)
                         {
                              // Top Left
                              ExtraButterflies[0, c].Position.X -= speed * 1.75f;
                              ExtraButterflies[0, c].Position.Y -= speed * 1.75f;
                              ExtraButterflies[0, c].Position.Y -= butterflyPositionY;

                              // Top Center
                              ExtraButterflies[1, c].Position.X += 0;
                              ExtraButterflies[1, c].Position.Y -= speed * 1.25f * 1.75f;

                              // Top Right
                              ExtraButterflies[2, c].Position.X += speed * 1.75f;
                              ExtraButterflies[2, c].Position.Y -= speed * 1.75f;
                              ExtraButterflies[2, c].Position.Y -= butterflyPositionY;

                              // Horizontal Right
                              ExtraButterflies[3, c].Position.X += speed * 1.5f * 1.75f;
                              ExtraButterflies[3, c].Position.Y -= butterflyPositionY;

                              // Bottom Right
                              ExtraButterflies[4, c].Position.X += speed * 1.75f;
                              ExtraButterflies[4, c].Position.Y += speed * 1.75f;
                              ExtraButterflies[4, c].Position.Y += butterflyPositionY;

                              // Bottom Center
                              ExtraButterflies[5, c].Position.X += 0;
                              ExtraButterflies[5, c].Position.Y += speed * 1.25f * 1.75f;

                              // Bottom Left
                              ExtraButterflies[6, c].Position.X -= speed * 1.75f;
                              ExtraButterflies[6, c].Position.Y += speed * 1.75f;
                              ExtraButterflies[6, c].Position.Y += butterflyPositionY;

                              // Horizontal Left
                              ExtraButterflies[7, c].Position.X -= speed * (1.5f) * 1.75f;
                              ExtraButterflies[7, c].Position.Y -= butterflyPositionY;
                         }

                         for (int r = 0; r < NumberOfButterflies; r++)
                         {
                              for (int c = 0; c < NumberOfButterflies; c++)
                              {
                                   if (!isFlyFaster)
                                   {
                                        ExtraButterflies[r, c].CurrentFrame++;
                                        ExtraButterflies[r, c].CurrentFrame = ExtraButterflies[r, c].CurrentFrame % 8;
                                        ExtraButterflies[r, c].Texture = butterflyTextures[ExtraButterflies[r, c].CurrentFrame];
                                   }

                                   else
                                   {
                                        ExtraButterflies[r, c].CurrentFrame++;
                                        ExtraButterflies[r, c].CurrentFrame = ExtraButterflies[r, c].CurrentFrame % 8;
                                        ExtraButterflies[r, c].Texture = butterflyTextures[ExtraButterflies[r, c].CurrentFrame + 16];
                                   }
                              }
                         }
                    }

                    // Top Left
                    Butterflies[0].Position.X -= speed * 1.75f;
                    Butterflies[0].Position.Y -= speed * 1.75f;
                    Butterflies[0].Position.Y -= butterflyPositionY;

                    // Top Center
                    Butterflies[1].Position.X += 0;
                    Butterflies[1].Position.Y -= speed * 1.25f * 1.75f;

                    // Top Right
                    Butterflies[2].Position.X += speed * 1.75f;
                    Butterflies[2].Position.Y -= speed * 1.75f;
                    Butterflies[2].Position.Y -= butterflyPositionY;

                    // Horizontal Right
                    Butterflies[3].Position.X += speed * 1.5f * 1.75f;
                    Butterflies[3].Position.Y -= butterflyPositionY;

                    // Bottom Right
                    Butterflies[4].Position.X += speed * 1.75f;
                    Butterflies[4].Position.Y += speed * 1.75f;
                    Butterflies[4].Position.Y += butterflyPositionY;

                    // Bottom Center
                    Butterflies[5].Position.X += 0;
                    Butterflies[5].Position.Y += speed * 1.25f * 1.75f;

                    // Bottom Left
                    Butterflies[6].Position.X -= speed * 1.75f;
                    Butterflies[6].Position.Y += speed * 1.75f;
                    Butterflies[6].Position.Y += butterflyPositionY;

                    // Horizontal Left
                    Butterflies[7].Position.X -= speed * (1.5f) * 1.75f;
                    Butterflies[7].Position.Y -= butterflyPositionY;
               }

               if (!isFlyFaster)
               {
                    for (int i = 0; i < Butterflies.Length; i++)
                    {
                         Butterflies[i].CurrentFrame++;
                         Butterflies[i].CurrentFrame = Butterflies[i].CurrentFrame % 8;
                         Butterflies[i].Texture = butterflyTextures[Butterflies[i].CurrentFrame];

                         if (!isButterfliesMove)
                         {
                              Butterflies[i].Texture = butterflyTextures[Butterflies[i].CurrentFrame + 8];
                         }
                    }
               }

               // Update elapsed time counters.
               elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
               elapsedFrameTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

               // Update the current frame of the Studio Logo text animation.
               if (elapsedFrameTime >= (1000.0f / 20.0f))
               {
                    elapsedFrameTime = 0.0f;

                    currentFrame++;
                    currentFrame = (int)MathHelper.Min(currentFrame, frames - 1);

                    currentTexture = studioLogo[currentFrame];
               }

               isButterfliesMove = true;

               // Between 2 and 6.2 seconds, we don't want the Butterflies to move.
               if (elapsedTime > 2.0f && elapsedTime < 7.2f)
               {
                    isButterfliesMove = false;
               }

               // At 6.2 seconds and above, we want the Butterflies to move faster.
               if (elapsedTime >= 7.2f)
               {
                    isFlyFaster = true;

                    // We also want to spawn 8x Butterflies - just once, though!
                    if (isSpawnExtraButterflies)
                    {
                         for (int r = 0; r < NumberOfButterflies; r++)
                         {
                              for (int c = 0; c < NumberOfButterflies; c++)
                              {
                                   ExtraButterflies[r, c].Position = Butterflies[c].Position;
                                   ExtraButterflies[r, c].Texture = Butterflies[c].Texture;
                              }
                         }

                         isSpawnExtraButterflies = false;
                    }
               }

               // After 7.5 seconds, we want to proceed to the Start Screen.
               if (elapsedTime >= 8.5f)
               {
                    ExitScreen();
                    ScreenManager.AddScreen(new GameplayBackgroundScreen(), ControllingPlayer);
                    ScreenManager.AddScreen(new StartScreen(), ControllingPlayer);
               }
          }

          #endregion

          #region Draw

          public override void Draw(GameTime gameTime)
          {
               logoColor = Color.CornflowerBlue * (this.TransitionAlpha / 255f);
               butterflyColor = Color.CornflowerBlue * (this.TransitionAlpha / 255f);

               if (!isFlyFaster)
               {
                    colorIndex++;
                    colorIndex %= (8 * 5);
                    logoColor = butterflyColors[colorIndex / 5] * (this.TransitionAlpha / 255f);
               }



               MySpriteBatch.Begin();

               // Render the Studio Logo's current frame of animation.
               MySpriteBatch.Draw(currentTexture, new Vector2(EngineCore.ScreenCenter.X, EngineCore.ScreenCenter.Y + 100),
                    null, logoColor, 0.0f, new Vector2(currentTexture.Width / 2, currentTexture.Height / 2), 2f);

               // FOR BUTTERFLIES STARTING AT LOGO
               if (isButterfliesMove)
               {
                    if (isFlyFaster)
                    {
                         for (int r = 0; r < NumberOfButterflies; r++)
                         {
                              for (int c = 0; c < NumberOfButterflies; c++)
                              {
                                   SpriteEffects effect = new SpriteEffects();
                                   effect = SpriteEffects.None;

                                   if (c == 0 || c == 6 || c == 7)
                                   {
                                        effect = SpriteEffects.FlipHorizontally;
                                   }

                                   MySpriteBatch.Draw(ExtraButterflies[r, c].Texture, ExtraButterflies[r, c].Position,
                                        null, ExtraButterflies[r, c].Color, MathHelper.ToRadians(ExtraButterflies[r, c].Rotation),
                                        new Vector2(ExtraButterflies[r, c].Texture.Width / 2, ExtraButterflies[r, c].Texture.Height / 2), 2.5f, effect, 0f);
                              }
                         }
                    }
               }

               if (!isFlyFaster)
               {
                    for (int i = 0; i < Butterflies.Length; i++)
                    {
                         SpriteEffects effect = new SpriteEffects();
                         effect = SpriteEffects.None;

                         if (i == 0 || i == 6 || i == 7)
                         {
                              effect = SpriteEffects.FlipHorizontally;
                         }

                         MySpriteBatch.Draw(Butterflies[i].Texture, Butterflies[i].Position,
                              null, logoColor, MathHelper.ToRadians(Butterflies[i].Rotation),
                              new Vector2(Butterflies[i].Texture.Width / 2, Butterflies[i].Texture.Height / 2), 2.5f, effect, 0f);
                    }
               }

               MySpriteBatch.End();

               base.Draw(gameTime);
          }

          #endregion
     }
}
