#region File Description
//-----------------------------------------------------------------------------
// PlayerBackgroundScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.Graphics;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
using PixelEngine.Text;
#endregion

namespace AvatarTyping
{
     /// <summary>
     /// A BackgroundScreen for the Start / Main-Menu in Avatar Typing.
     /// This is a BackgroundScreen to prevent re-loading assets and losing
     /// the state of the camera angles / enemies.
     /// </summary>
     public class PlayerBackgroundScreen : BackgroundScreen
     {
          #region Fields

          SpriteBatch spriteBatch;
          ContentManager content;

          Texture2D gamerPic = AvatarTypingGame.CurrentPlayer.GamerInfo.GamerPicture;

          GameResourceTexture2D smallGradientTexture;
          GameResourceTexture2D blankTexture;
          GameResourceTexture2D borderTexture_Black;
          GameResourceTexture2D borderTexture_White;

          public static bool isActive = true;


          public Player player = AvatarTypingGame.CurrentPlayer; // Use to be Private

          public bool DrawWithoutCamera = false;
          public bool ShowAsGradient = true;
          public bool ShowBorder = true;
          public bool ShowGamerTag = true;
          public bool ShowGamerPic = true;
          

          public Vector3 playerPosition = new Vector3(0.95f, 0.95f, 0.0f);
          public Rectangle border = new Rectangle(200, 200, 350, 450);

          public Color BorderColor = Color.Yellow * (100f / 255f);
          public Color OutlineColor = Color.Black;

          public float playerScale = 0.9f;
          public float borderScale = 1.0f;

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public PlayerBackgroundScreen()
          {
               TransitionOnTime = TimeSpan.FromSeconds(0.5);
               TransitionOffTime = TimeSpan.FromSeconds(0.5);
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
               if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

               spriteBatch = ScreenManager.SpriteBatch;

               smallGradientTexture = ResourceManager.LoadTexture(@"Textures\Gradients\Gradient_BlackToWhite6");
               blankTexture = ResourceManager.LoadTexture(@"Textures\Blank Textures\Blank_Rounded_WithBorder");
               borderTexture_White = ResourceManager.LoadTexture(@"Textures\Blank Textures\Border_White");
               borderTexture_Black = ResourceManager.LoadTexture(@"Textures\Blank Textures\Border");


               borderScale = playerScale / 0.9f;

               border = new Rectangle(200, 200, (int)(350 * borderScale), (int)(450 * borderScale));
          }

          /// <summary>
          /// Unloads graphics content for this screen.
          /// </summary>
          public override void UnloadContent()
          {
               if (content != null)
                    content.Unload();
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
               if (!isActive)
                    return;

               player.Update(gameTime);

               base.Update(gameTime, otherScreenHasFocus, false);
          }

          #endregion

          #region Draw

          /// <summary>
          /// Draws the background screen.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               player.Avatar.Position = playerPosition;
               player.Avatar.Rotation = Vector3.Zero;
               player.Avatar.Scale = playerScale;
               player.Avatar.LightingEnabled = false;

               MySpriteBatch.Begin();

               if (ShowBorder)
               {
                    if (ShowAsGradient)
                    {
                         Rectangle newBorder;
                         newBorder = new Rectangle(border.X + 2, border.Y + 2, 
                              border.Width - 4, border.Height - 4);

                         MySpriteBatch.Draw(smallGradientTexture.Texture2D, newBorder, BorderColor);
                    }

                    else
                         MySpriteBatch.Draw(blankTexture.Texture2D, border, BorderColor);

                    if (OutlineColor == Color.White)
                    {
                         MySpriteBatch.Draw(borderTexture_White.Texture2D, border, OutlineColor);
                    }

                    else
                    {
                         MySpriteBatch.Draw(borderTexture_Black.Texture2D, border, OutlineColor);
                    }
               }

               MySpriteBatch.End();

               if (DrawWithoutCamera)
               {
                    if (AvatarTypingGame.CurrentPlayer != null)
                    {
                         AvatarTypingGame.CurrentPlayer.Avatar.DrawToScreen(gameTime,
                              new Vector3(0f, 1f, -5f), new Vector3(0, 0.20f, 0f));
                    }
               }

               else
               {
                    if (AvatarTypingGame.CurrentPlayer != null)
                    {
                         AvatarTypingGame.CurrentPlayer.Draw(gameTime);
                    }
               }

               MySpriteBatch.Begin();

               if (ShowGamerTag)
               {
                    //MySpriteBatch.DrawString(ScreenManager.Font, player.GamerInfo.GamerTag,
                    //     new Vector2(border.X + 65, border.Y + 330), Color.White);

                    TextManager.DrawCentered(false, ScreenManager.Font, player.GamerInfo.GamerTag,
                         new Vector2(border.X + (border.Width / 2), border.Y + 330), Color.White);
               }

               if (ShowGamerPic)
               {
                    MySpriteBatch.Draw(gamerPic, new Vector2(border.X + 135, border.Y + 370), Color.White);
               }

               MySpriteBatch.End();
          }

          #endregion
     }
}
