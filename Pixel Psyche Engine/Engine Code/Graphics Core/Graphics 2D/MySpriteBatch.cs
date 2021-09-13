#region File Description
//-----------------------------------------------------------------------------
// Renderer.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PixelEngine.Screen;
#endregion

namespace PixelEngine.Graphics
{
     /// <summary>
     /// Helper class which basically wraps SpriteBatch functionality, in order
     /// to render with Resolution-dependancy. Consequently, the rendering will
     /// look the same on all HDTV / Monitor resolutions.
     /// 
     /// This class essentially allows clients of the PixelEngine to not use a scale
     /// factor for each of their SpriteBatch.Draw calls.
     /// </summary>
     public static class MySpriteBatch
     {
          #region Fields

          private static SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

          private static ContentManager content;
          private static SpriteFont buttonSpriteFont;

          #endregion

          #region Properties

          public static SpriteFont ButtonSpriteFont
          {
               get { return buttonSpriteFont; }
               set { buttonSpriteFont = value; }
          }

          #endregion

          #region Begin and End Methods

          public static void Begin()
          {
               spriteBatch.Begin();
          }

          // Doesn't work in XNA 4.0:

          public static void Begin(BlendState blendMode)
          {
                    //I had this before, like a dumbass! Seemed to slow the game down?!?!??!!?! spriteBatch.Begin(0, blendMode);
                    // XNA 3.1 Begin(SpriteBlendMode mode) is equal to this:
               spriteBatch.Begin(SpriteSortMode.Deferred, blendMode);
          }

          // Doesn't work in XNA 4.0:

          //public static void Begin(SpriteBlendMode blendMode, SpriteSortMode sortMode, SaveStateMode saveMode)
          public static void Begin(BlendState blendMode, SpriteSortMode sortMode)
          {
               spriteBatch.Begin(sortMode, blendMode);
          }

          public static void End()
          {
               spriteBatch.End();
          }

          #endregion

          #region SpriteBatch.Draw Wrapper Methods

          public static void Draw(Texture2D texture, Rectangle destination, Rectangle? source, Color color, float depthLayer)
          {
               spriteBatch.Draw(texture, destination, source, color, 0.0f, new Vector2(), SpriteEffects.None, depthLayer);
          }

          public static void Draw(Texture2D texture, Rectangle destination, Color color)
          {
               /*
               float originalWidth = destination.Width;
               float resolutionDependentWidth = EngineCore.ResolutionScale * destination.Width;
               
               float widthRatio = resolutionDependentWidth / (1.0f * originalWidth);

               spriteBatch.Draw(texture, new Vector2(destination.X, destination.Y), null, color,
                    0.0f, new Vector2(), EngineCore.ResolutionScale * widthRatio, SpriteEffects.None, 0.0f);
                * */

               spriteBatch.Draw(texture, destination, color);
          }

          public static void Draw(Texture2D texture, Rectangle destination, Rectangle? source, Color color)
          {
               spriteBatch.Draw(texture, destination, source, color);
          }

          public static void Draw(Texture2D texture, Vector2 position, Color color)
          {
               spriteBatch.Draw(texture, position, color);
          }

          public static void Draw(Texture2D texture, Vector2 position, Rectangle? sourceDestination,
               Color color)
          {
               spriteBatch.Draw(texture, position, sourceDestination, color, 0.0f,
                    new Vector2(), EngineCore.ResolutionScale, SpriteEffects.None, 0.0f);
          }

          public static void Draw(Texture2D texture, Vector2 position, Rectangle? sourceDestination,
              Color color, float rotation, Vector2 origin)
          {
               spriteBatch.Draw(texture, position, sourceDestination, color, rotation,
                    origin, EngineCore.ResolutionScale, SpriteEffects.None, 0.0f);
          }

          public static void Draw(Texture2D texture, Vector2 position, Rectangle? sourceDestination,
               Color color, float rotation, Vector2 origin, float scale)
          {
               spriteBatch.Draw(texture, position, sourceDestination, color, rotation,
                    origin, EngineCore.ResolutionScale * scale, SpriteEffects.None, 0.0f);
          }

          public static void Draw(Texture2D texture, Vector2 position, Rectangle? sourceDestination,
               Color color, float rotation, Vector2 origin, float scale, SpriteEffects spriteEffects, float layerDepth)
          {
               spriteBatch.Draw(texture, position, sourceDestination, color, rotation,
                    origin, EngineCore.ResolutionScale * scale, spriteEffects, layerDepth);
          }


          public static void DrawCentered(Texture2D texture, Vector2 position, Color color)
          {
               Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
               spriteBatch.Draw(texture, position, null, color, 0.0f,
                    origin, EngineCore.ResolutionScale, SpriteEffects.None, 0.0f);
          }

          public static void DrawCentered(Texture2D texture, Vector2 position, Color color, float scale)
          {
               Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
               spriteBatch.Draw(texture, position, null, color, 0.0f,
                    origin, EngineCore.ResolutionScale * scale, SpriteEffects.None, 0.0f);
          }

          public static void DrawCentered(Texture2D texture, Vector2 position, Color color, Vector2 scale)
          {
               Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
               spriteBatch.Draw(texture, position, null, color, 0.0f,
                    origin, EngineCore.ResolutionScale * scale, SpriteEffects.None, 0.0f);
          }

          #endregion

          #region SpriteBatch.DrawString Wrapper Methods

          /// <summary>
          /// A call to SpriteBatch.DrawString which automatically scales 
          /// the text depending on the current resolution.
          /// </summary>
          public static void DrawString(SpriteFont font, string text, Vector2 position, Color color)
          {
               spriteBatch.DrawString(font, text, position, color,
                         0, new Vector2(), EngineCore.ResolutionScale, SpriteEffects.None, 0.0f);
          }

          public static void DrawString(SpriteFont font, string text, Vector2 position,
               Color color, float rotation, Vector2 origin, float scale, SpriteEffects spriteEffect, float layerDepth)
          {
               spriteBatch.DrawString(font, text, position, color,
                         rotation, origin, EngineCore.ResolutionScale * scale, spriteEffect, layerDepth);
          }

          #endregion

          #region SpriteBatch.Draw360Button Methods

          public static void Draw360Button(Buttons button, Vector2 position, Color color)
          {
               if (content == null)
               {
                    content = new ContentManager(ScreenManager.Game.Services, "Content");
               }

               if (buttonSpriteFont == null)
               {
                    // Load the Button Font Sprite sheet.
                    buttonSpriteFont = content.Load<SpriteFont>(@"Buttons\xboxControllerSpriteFont");
               }

               string buttonString = GetStringFromButton(button);


               spriteBatch.DrawString(buttonSpriteFont, buttonString, position, color);
          }

          public static void Draw360Button(Buttons button, Vector2 position,
               Color color, float rotation, Vector2 origin, float scale, SpriteEffects spriteEffect, float layerDepth)
          {
               if (content == null)
               {
                    content = new ContentManager(ScreenManager.Game.Services, "Content");
               }

               if (buttonSpriteFont == null)
               {
                    // Load the Button Font Sprite sheet.
                    buttonSpriteFont = content.Load<SpriteFont>(@"Buttons\xboxControllerSpriteFont");
               }

               string buttonString = GetStringFromButton(button);

               spriteBatch.DrawString(buttonSpriteFont, buttonString, position, color,
                    rotation, origin, scale, spriteEffect, layerDepth);
          }

          #region Helper Methods

          private static string GetStringFromButton(Buttons button)
          {
               string buttonString = "";

               switch (button)
               {
                    case Buttons.A:
                         buttonString = "'";
                         break;

                    case Buttons.B:
                         buttonString = ")";
                         break;

                    case Buttons.Back:
                         buttonString = "#";
                         break;

                    case Buttons.BigButton:
                         buttonString = "$";
                         break;

                    case Buttons.DPadDown:
                         buttonString = "!";
                         break;

                    case Buttons.DPadLeft:
                         buttonString = "!";
                         break;

                    case Buttons.DPadRight:
                         buttonString = "!";
                         break;

                    case Buttons.DPadUp:
                         buttonString = "!";
                         break;

                    case Buttons.LeftShoulder:
                         buttonString = "-";
                         break;

                    case Buttons.LeftStick:
                         buttonString = " ";
                         break;

                    case Buttons.LeftTrigger:
                         buttonString = ",";
                         break;

                    case Buttons.RightShoulder:
                         buttonString = "*";
                         break;

                    case Buttons.RightStick:
                         buttonString = "\"";
                         break;

                    case Buttons.RightTrigger:
                         buttonString = "+";
                         break;

                    case Buttons.Start:
                         buttonString = "%";
                         break;

                    case Buttons.X:
                         buttonString = "&";
                         break;

                    case Buttons.Y:
                         buttonString = "(";
                         break;
               }

               return buttonString;
          }

          #endregion

          #endregion

          #region MeasureString Methods

          /* These aren't really necessary, as they are accessible via a
           * SpriteFont. However, using these methods ensure the SpriteFont
           * is not null.
           * 
           * It also provides the ButtonSpriteFont for the user, so they don't
           * have to create, load, or specify it when rendering 360 buttons via
           * DrawString().
           * */

          public static Vector2 MeasureString(SpriteFont font, string text)
          {
               // Only attempt to measure string if font isn't null.
               if (font != null)
               {
                    return font.MeasureString(text);
               }

               // If the font is null, just return a 0 Vector2.
               return Vector2.Zero;
          }

          public static Vector2 Measure360Button(Buttons button)
          {
               // Create a new ContentManager if ours isn't valid.
               if (content == null)
               {
                    content = new ContentManager(ScreenManager.Game.Services, "Content");
               }

               // Load the ButtonSpriteFont if ours isn't valid.
               if (buttonSpriteFont == null)
               {
                    // Load the Button Font Sprite sheet.
                    buttonSpriteFont = content.Load<SpriteFont>(@"Buttons\xboxControllerSpriteFont");
               }

               return buttonSpriteFont.MeasureString(GetStringFromButton(button));
          }

          #endregion
     }
}