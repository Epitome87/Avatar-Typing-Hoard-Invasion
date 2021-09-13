#region File Description
//-----------------------------------------------------------------------------
// MessageBoxScreen.cs
// Matt McGrath, with information borrowed from:
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using PixelEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.Graphics;
using PixelEngine;
using PixelEngine.Text;
using PixelEngine.ResourceManagement;
#endregion

namespace AvatarTyping
{
     /// <remarks>
     /// A popup message box screen, used to display "are you sure?"
     /// confirmation messages.
     /// </remarks>
     public class MessageBoxScreen : GameScreen
     {
          #region Fields

          protected string message;
          protected GameResourceTexture2D gradientTexture;
          protected GameResourceTexture2D blankTexture;

          protected GameResourceTexture2D acceptIcon;
          protected GameResourceTexture2D cancelIcon;

          protected string Accept;
          protected string Cancel;

          #endregion

          #region Events

          public event EventHandler<PlayerIndexEventArgs> Accepted;
          public event EventHandler<PlayerIndexEventArgs> Cancelled;

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor automatically includes the standard "A=ok, B=cancel"
          /// usage text prompt.
          /// </summary>
          public MessageBoxScreen(string message)
               : this(message, "Accept", "Cancel")
          { }

          /// <summary>
          /// Constructor lets the caller specify whether to include the standard
          /// "A=ok, B=cancel" usage text prompt.
          /// </summary>
          public MessageBoxScreen(string message, string accept, string cancel)
          {
               Accept = accept;
               Cancel = cancel;

               this.message = message;

               IsPopup = true;

               TransitionOnTime = TimeSpan.FromSeconds(0.5);
               TransitionOffTime = TimeSpan.FromSeconds(0.5);
          }

          /// <summary>
          /// Loads graphics content for this screen. This uses the shared ContentManager
          /// provided by the Game class, so the content will remain loaded forever.
          /// Whenever a subsequent MessageBoxScreen tries to load this same content,
          /// it will just get back another reference to the already loaded data.
          /// </summary>
          public override void LoadContent()
          {
               ContentManager content = ScreenManager.Game.Content;

               blankTexture = ResourceManager.LoadTexture(@"Textures\Blank Textures\Border_Wide_White");
               gradientTexture = ResourceManager.LoadTexture(@"Textures\Blank Textures\Blank_Rounded_Wide");

               acceptIcon = ResourceManager.LoadTexture(@"Buttons\xboxControllerButtonA");
               cancelIcon = ResourceManager.LoadTexture(@"Buttons\xboxControllerButtonB");
          }


          #endregion

          #region Handle Input

          /// <summary>
          /// Responds to user input, accepting or cancelling the message box.
          /// </summary>
          public override void HandleInput(InputState input)
          {
               PlayerIndex playerIndex;

               // We pass in our ControllingPlayer, which may either be null (to
               // accept input from any player) or a specific index. If we pass a null
               // controlling player, the InputState helper returns to us which player
               // actually provided the input. We pass that through to our Accepted and
               // Cancelled events, so they can tell which player triggered them.
               if (input.IsMenuSelect(ControllingPlayer, out playerIndex))
               {
                    // Raise the accepted event, then exit the message box.
                    if (Accepted != null)
                         Accepted(this, new PlayerIndexEventArgs(playerIndex));

                    ExitScreen();
               }
               else if (input.IsMenuCancel(ControllingPlayer, out playerIndex))
               {
                    // Raise the cancelled event, then exit the message box.
                    if (Cancelled != null)
                         Cancelled(this, new PlayerIndexEventArgs(playerIndex));

                    ExitScreen();
               }
          }


          #endregion

          #region Draw

          /// <summary>
          /// Draws the message box.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
               SpriteFont font = ScreenManager.Font;

               // Darken down any other screens that were drawn beneath the popup.
               ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 4 / 5);//2 / 3);

               // Center the message text in the viewport.
               Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
               Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
               Vector2 textSize = font.MeasureString(message);
               Vector2 textPosition = (viewportSize - textSize) / 2;

               // The background includes a border somewhat larger than the text itself.
               const int hPad = 32;
               const int vPad = 16;

               Rectangle backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
                                                             (int)textPosition.Y - vPad,
                                                             (int)textSize.X + hPad * 2,
                                                             (int)textSize.Y + vPad * 2);

               backgroundRectangle.Height *= 2;

               // Fade the popup alpha during transitions.
               Color color = Color.White * (TransitionAlpha / 255f);
               Color acolor = Color.Black * (60f / 255f);// Color.DarkBlue * (60f / 255f);
               Color color1 = Color.White * (60f / 255f);

               spriteBatch.Begin();

               // Draw the background rectangle.
               MySpriteBatch.Draw(gradientTexture.Texture2D, backgroundRectangle, acolor);
               //MySpriteBatch.Draw(blankTexture.Texture2D, backgroundRectangle, Color.DarkRed * (TransitionAlpha / 255f));

               TextManager.DrawCentered(false, font, message, EngineCore.ScreenCenter, color);

               textPosition = EngineCore.ScreenCenter;
               textPosition.Y += font.LineSpacing;
               TextManager.DrawCentered(false, font, Accept, textPosition, color);
               MySpriteBatch.Draw(acceptIcon.Texture2D, new Rectangle((int)textPosition.X - acceptIcon.Texture2D.Width / 2 - (int)font.MeasureString(Accept).X / 2,  (int)textPosition.Y - font.LineSpacing / 2, font.LineSpacing, font.LineSpacing),
                  null, color);

               textPosition.Y += font.LineSpacing; 
               TextManager.DrawCentered(false, font, Cancel, textPosition, color);
               MySpriteBatch.Draw(cancelIcon.Texture2D, new Rectangle((int)textPosition.X - cancelIcon.Texture2D.Width / 2 - (int)font.MeasureString(Cancel).X / 2, (int)textPosition.Y - font.LineSpacing / 2, font.LineSpacing, font.LineSpacing),
                  null, color);
               

               spriteBatch.End();
          }


          #endregion
     }
}
