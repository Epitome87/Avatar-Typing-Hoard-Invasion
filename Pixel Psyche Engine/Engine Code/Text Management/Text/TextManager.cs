#region File Description
//-----------------------------------------------------------------------------
// TextHandler.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PixelEngine.Graphics;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
#endregion

namespace PixelEngine.Text
{
     /// <remarks>
     /// Helper class which makes drawing strings simpler.
     /// Fonts are pre-loaded and accessed through simple names.
     /// If the calling method does not supply a Font, a default one is used.
     /// 
     /// Note: TextManager automatically handles the Update and Drawing of TextObjects
     /// only if they are added via TextManager.Add(). If working with non-TextObject text,
     /// TextManager still provides useful methods for working with text--but it does not
     /// Update and Draw them automatically.
     /// 
     /// Essentially, all rendering of text could be done through this class rather than
     /// SpriteBatch.DrawString() calls.
     /// </remarks>
     public class TextManager : DrawableGameComponent
     {
          #region Enums

          /// <summary>
          /// Enum representing a Font Type.
          /// </summary>
          public enum FontType
          {
               TitleFont,          
               MenuFont,
               EnemyFont,
               ShadowedFont
          };

          /// <summary>
          /// Enum representing a Format Type.
          /// </summary>
          public enum FormatType
          {
               TopLeft,
               TopCenter,
               TopRight,

               MidLeft,
               Center,
               MidRight,

               BottomLeft,
               BottomCenter,
               BottomRight
          };

          #endregion

          #region Singleton

          private static TextManager textHandler = null;

          #endregion

          #region Fields

          private ContentManager content;
          private static SpriteBatch spriteBatch;

          public static List<GameResourceFont> Fonts;
          private static GameResourceFont DefaultFont;
          private static List<TextObject> textObjects;

          private static bool isInitialized;

          #endregion

          #region Properties

          public static bool IsInitialized
          {
               get { return isInitialized; }
               set { isInitialized = value; }
          }

          /// <summary>
          /// Gets the "Menu" Font. Quick way to access it.
          /// </summary>
          public static SpriteFont MenuFont
          {
               get { return Fonts[(int)FontType.MenuFont].SpriteFont; }
          }

          /// <summary>
          /// Gets the "Title" Font. Quick way to access it.
          /// </summary>
          public static SpriteFont TitleFont
          {
               get { return Fonts[(int)FontType.TitleFont].SpriteFont; }
          }

          /// <summary>
          /// Gets the "Enemy" Font. Quick way to access it.
          /// </summary>
          public static SpriteFont EnemyFont
          {
               get { return Fonts[(int)FontType.EnemyFont].SpriteFont; }
          }

          /// <summary>
          /// Gets the "Shadowed" Font. Quick way to access it.
          /// </summary>
          public static SpriteFont ShadowedFont
          {
               get { return Fonts[(int)FontType.ShadowedFont].SpriteFont; }
          }

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          private TextManager(Game game) 
               : base(game)
          {
               Fonts = new List<GameResourceFont>();
               textObjects = new List<TextObject>();
          }

          /// <summary>
          /// Initializes the Text Manager and adds it as a Game Component.
          /// </summary>
          public static void Initialize(Game game)
          {
               textHandler = new TextManager(game);

               if (game != null)
               {
                    game.Components.Add(textHandler);
               }
          }

          /// <summary>
          /// Override DrawableGameComponent.Initialize method.
          /// Simply calls GameComponent.Initialize and sets
          /// boolean IsInitialized to true.
          /// </summary>
          public override void Initialize()
          {
               base.Initialize();

               isInitialized = true;
          }

          /// <summary>
          /// Loads Content such as the various Fonts.
          /// </summary>
          protected override void LoadContent()
          {
               // Load content belonging to the screen manager.
               if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

               GameResourceFont font;

               font = ResourceManager.LoadFont(@"Fonts\TitleFont_Large");
               Fonts.Add(font);

               font = ResourceManager.LoadFont(@"Fonts\MenuFont_Border");
               Fonts.Add(font);

               font = ResourceManager.LoadFont(@"Fonts\ChatFont");
               Fonts.Add(font);

               DefaultFont = font;

               //font = ResourceManager.LoadFont(@"Fonts\ShadowedFont");
               //Fonts.Add(font);
          }

          /// <summary>
          /// Unloads Content, such as the List of Fonts and Text Objects.
          /// </summary>
          protected override void UnloadContent()
          {
               Fonts.Clear();
               textObjects.Clear();
          }

          /// <summary>
          /// Resets the Text Manager by clearing the List of Text Objects it contains.
          /// </summary>
          public static void Reset()
          {
               textObjects.Clear();
          }

          #endregion

          #region Add and Remove Text

          /// <summary>
          /// Adds a Text Object to be managed by this Text Manager.
          /// <param name="textObj">The Text Object to add.</param>
          /// </summary>
          public static void AddText(TextObject textObj)
          {
               textObjects.Add(textObj);
          }

          /// <summary>
          /// Removes the specified Text Object being managed by this Text Manager.
          /// <param name="textObj">The Text Object to remove.</param>
          /// <returns>Returns True if the Text Object was removed, false otherwise.</returns>
          /// </summary>
          public static bool RemoveText(TextObject textObj)
          {
               return textObjects.Remove(textObj);
          }

          /// <summary>
          /// Removes all Text Objects being managed by this Text Manager.
          /// </summary>
          public static void RemoveAll()
          {
               textObjects.Clear();
          }

          #endregion

          #region Main Draw Method

          /// <summary>
          /// Draws all Text Objects being managed by the Text Manager.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               spriteBatch = ScreenManager.SpriteBatch;
                
               try
               {              
                    spriteBatch.Begin();

                    foreach (TextObject textObj in textObjects)
                    {
                         textObj.Draw(gameTime);
                    }
                    
                    spriteBatch.End();
               }

               catch (System.InvalidOperationException)
               { } 
          }

          #endregion

          #region DrawString Overload Methods

          /// <summary>
          /// Helper method which overrides DrawString, calling MySpriteBatch instead.
          /// </summary>
          public static void DrawString(string s, FontType font, Vector2 pos, Color color)
          {
               spriteBatch = ScreenManager.SpriteBatch;

               MySpriteBatch.DrawString(Fonts[(int)font].SpriteFont, s, pos, color);
          }

          /// <summary>
          /// Helper method which overrides DrawString, calling MySpriteBatch instead.
          /// Draws the string in the center.
          /// </summary>
          public static void DrawString(string s, FontType font, Color color)
          {
               spriteBatch = ScreenManager.SpriteBatch;

               Vector2 textOrigin = Fonts[(int)font].SpriteFont.MeasureString(s) / 2;
               MySpriteBatch.DrawString(Fonts[(int)font].SpriteFont, s, new Vector2(1280 / 2, 0), color,
                    0.0f, textOrigin, 1.0f, SpriteEffects.None, 0.0f);
          }

          /// <summary>
          /// Helper method which overrides DrawString, calling MySpriteBatch instead.
          /// Hassle-Free DrawString: Uses the DefaultFont.
          /// </summary>
          public static void DrawString(string s, Vector2 pos, Color color)
          {
               spriteBatch = ScreenManager.SpriteBatch;

               MySpriteBatch.DrawString(DefaultFont.SpriteFont, s, pos, color,
                    0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.0f);
          }

          /// <summary>
          /// Helper method which overrides DrawString, calling MySpriteBatch instead.
          /// </summary>
          public static void DrawString(SpriteFont font, string s, Vector2 pos, Color color)
          {
               spriteBatch = ScreenManager.SpriteBatch;

               MySpriteBatch.DrawString(font, s, pos, color);
          }

          /// <summary>
          /// Helper method which overrides DrawString, calling MySpriteBatch instead.
          /// </summary>
          public static void DrawString(SpriteFont font, string s, Vector2 pos, Color color, float scale)
          {
               spriteBatch = ScreenManager.SpriteBatch;

               MySpriteBatch.DrawString(font, s, pos, color,
                    0.0f, new Vector2(0, 0), scale, SpriteEffects.None, 0.0f);
          }

          /// <summary>
          /// Helper method which overrides DrawString, calling MySpriteBatch instead.
          /// </summary>
          public static void DrawString(SpriteFont font, string s, Vector2 pos, Color color, 
               Vector2 origin, float scale)
          {
               spriteBatch = ScreenManager.SpriteBatch;

               MySpriteBatch.DrawString(font, s, pos, color,
                    0.0f, origin, scale, SpriteEffects.None, 0.0f);
          }

          /// <summary>
          /// Helper method which overrides DrawString, calling MySpriteBatch instead.
          /// </summary>
          public static void DrawString(SpriteFont font, string word, Vector2 textPosition, Color color, 
               float rotation, Vector2 textOrigin, float scale)
          {
               spriteBatch = ScreenManager.SpriteBatch;

               MySpriteBatch.DrawString(font, word, textPosition, color, rotation,
                    textOrigin, scale, SpriteEffects.None, 0.0f);
          }

          /// <summary>
          /// Helper method which overrides DrawString, calling MySpriteBatch instead.
          /// </summary>
          public static void DrawString(SpriteFont font, string word, Vector2 textPosition, Color color,
               float rotation, Vector2 textOrigin, float scale, SpriteEffects spriteEffects, float depthLayer)
          {
               spriteBatch = ScreenManager.SpriteBatch;

               MySpriteBatch.DrawString(font, word, textPosition, color, rotation,
                    textOrigin, scale, spriteEffects, depthLayer);
          }

          /// <summary>
          /// Draws the string in the position specified by FormatType.
          /// </summary>
          public static void DrawString(string s, FontType font, Color color, FormatType formatType)
          {
               spriteBatch = ScreenManager.SpriteBatch;

               Vector2 textPos = new Vector2(0, 0);
               Vector2 textOrigin = new Vector2(0, 0);

               #region Switch (FormatType) Construct
               switch ((int)formatType)
               {
                    case (int)FormatType.TopLeft:
                         textPos = new Vector2(150, 150);
                         break;

                    case (int)FormatType.TopCenter:
                         textPos = new Vector2(EngineCore.ScreenCenter.X, 150);
                         textOrigin = Fonts[(int)font].SpriteFont.MeasureString(s) / 2;
                         break;

                    case (int)FormatType.TopRight:
                         textPos = new Vector2(EngineCore.ScreenCenter.X * 2 - 150, 150);
                         break;

                    case (int)FormatType.MidLeft:
                         textPos = new Vector2(150, EngineCore.ScreenCenter.Y);
                         break;

                    case (int)FormatType.Center:
                         textPos = EngineCore.ScreenCenter;
                         textOrigin = Fonts[(int)font].SpriteFont.MeasureString(s) / 2;
                         break;

                    case (int)FormatType.MidRight:
                         textPos = new Vector2(EngineCore.ScreenCenter.X * 2 - 50, EngineCore.ScreenCenter.Y);
                         break;

                    case (int)FormatType.BottomLeft:
                         textPos = new Vector2(150, EngineCore.ScreenCenter.Y * 2 - 150);
                         break;

                    case (int)FormatType.BottomCenter:
                         textPos = new Vector2(EngineCore.ScreenCenter.X, EngineCore.ScreenCenter.Y * 2 - 150);
                         textOrigin = Fonts[(int)font].SpriteFont.MeasureString(s) / 2;
                         break;

                    case (int)FormatType.BottomRight:
                         textPos = new Vector2(EngineCore.ScreenCenter.X * 2 - 150, EngineCore.ScreenCenter.Y * 2 - 150);
                         break;
               }
               #endregion

               MySpriteBatch.DrawString(Fonts[(int)font].SpriteFont, s, textPos, color,
                    0.0f, textOrigin, 1.0f, SpriteEffects.None, 0.0f);
          }

          #endregion

          #region Update

          public override void Update(GameTime gameTime)
          {
               foreach (TextObject textObj in textObjects)
               {
                    textObj.Update(gameTime);
               }
          }

          #endregion

          #region Public Methods

          /// <summary>
          /// Returns number of TextObjects in TextHandler.
          /// </summary>
          public static int Size()
          {
               return textObjects.Count;
          }

          public static void SetScaleRange(int start, int amount, float scale)
          {
               if (start < 0 || (start + amount > textObjects.Count))
                    return;

               for (int i = start; i < amount; i++)
               {
                    textObjects[i].Scale = scale;
               }
          }

          /// <summary>
          /// Sets the AnimationEffect of all TextObjects to the same AnimationEffect.
          /// </summary>
          public static void SetAnimationAll()
          {
               foreach (TextObject textObj in textObjects)
               {
                    //textObj.TextEffect = new TypingEffect(2000.0f, textObj.Text);
                    textObj.AddTextEffect(new TypingEffect(2000.0f, textObj.Text));
               }
          }

          #endregion

          #region Formatting Methods

          /// <summary>
          /// Returns a string that has newline characters (\n) inserted into it at the points where
          // they would need to go to wrap a text string to a certain width.
          /// </summary>
          public static string WrapText(string Text, FontType FontName, float MaxLineWidth)
          {
               // Create an array (words) with one entry for each word in the passed text string
               string[] words = Text.Split(' ');

               // A StringBuilder lets us add to a string and finally return the result
               StringBuilder sb = new StringBuilder();

               // How long is the line we are currently working on so far
               float lineWidth = 0.0f;

               // Store a measurement of the size of a space in the font we are using.
               float spaceWidth = Fonts[(int)FontName].SpriteFont.MeasureString(" ").X;

               // Loop through each word in the string
               foreach (string word in words)
               {
                    Vector2 size;
                    size = Fonts[(int)FontName].SpriteFont.MeasureString(word);

                    // If this word will fit on the current line, add it and keep track
                    // of how long the line has gotten.
                    if (lineWidth + size.X < MaxLineWidth)
                    {
                         sb.Append(word + " ");
                         lineWidth += size.X + spaceWidth;
                    }
                    else
                    // otherwise, append a newline character to start a new line.  Add the
                    // word and a space, and set the size of the new line.
                    {
                         sb.Append("\n" + word + " ");
                         lineWidth = size.X + spaceWidth;
                    }
               }
               // return the resultant string
               return sb.ToString();
          }

          /// <summary>
          /// Overload for WrapText that uses the Default Text Manager Font.
          /// </summary>
          public static string WrapText(string Text, float MaxLineWidth)
          {
               return WrapText(Text, FontType.EnemyFont, MaxLineWidth);
          }

          public static void Draw(SpriteFont font, string text, Vector2 position, Color color,
               float rotation, Vector2 origin, float scale)
          {
               MySpriteBatch.DrawString(font, text, position, color, rotation,
                         origin, EngineCore.ResolutionScale * scale, SpriteEffects.None, 0.0f);
          }

          /// <summary>
          /// Draws (using SpriteBatch.DrawString()) a \n-separated string to where each
          /// line is centered on Position, rather than being centered but left-aligned
          /// with one another.
          /// </summary>
          public static void DrawCentered(bool autoCenter, SpriteFont font, string text, Vector2 position, Color color)
          {
               string[] lines = text.Split('\n');

               Vector2 origin;
               int size = lines.Length;

               if (autoCenter)
               {
                    position = new Vector2(position.X, position.Y -
                         ((float)(size - 1) / 2) * font.LineSpacing - (font.LineSpacing / 2));
               }

               else
               {
                    //position = new Vector2(position.X, position.Y - font.LineSpacing);
               }

               foreach (string line in lines)
               {
                    origin = new Vector2(font.MeasureString(line).X / 2, font.LineSpacing / 2);

                    MySpriteBatch.DrawString(font, line, position, color, 
                         0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);

                    position.Y += font.LineSpacing;
               }
          }

          public static void DrawCentered(bool autoCenter, SpriteFont font, string text, Vector2 position, Color color, float scale)
          {
               string[] lines = text.Split('\n');

               Vector2 origin;
               int size = lines.Length;

               if (autoCenter)
               {               
                    position = new Vector2(position.X, position.Y -
                         ((float)(size - 1) / 2) * (font.LineSpacing * scale) - ((font.LineSpacing * scale) / 2));
               }

               else
               {
                    //position = new Vector2(position.X, position.Y - font.LineSpacing * scale);
               }

               foreach (string line in lines)
               {
                    origin = new Vector2((font.MeasureString(line).X) / 2, (font.LineSpacing) / 2);

                    MySpriteBatch.DrawString(font, line, position, color,
                         0.0f, origin, scale, SpriteEffects.None, 0.0f);

                    position.Y += scale * font.LineSpacing;
               }
          }

          /// <summary>
          /// Draws (using SpriteBatch.DrawString()) a \n-separated string to where each
          /// line is centered on Position, rather than being centered but left-aligned
          /// with one another.
          /// Unlike DrawCentered, this method re-positions the text as needed, to ensure
          /// the middle line of the text is always in the center of the screen.
          /// </summary>
          public static void DrawAutoCentered(SpriteFont font, string text, Vector2 position, Color color, float scale)
          {
               // Added this recently to prevent null text.
               if (text == null)
                    text = "";

               string[] lines = text.Split('\n');

               Vector2 origin;
               int size = lines.Length;
               position = new Vector2(position.X, position.Y - 
                    ((float)(size - 1) / 2) * (font.LineSpacing * scale) - ((font.LineSpacing * scale) / 2));

               foreach (string line in lines)
               {
                    origin = new Vector2( (font.MeasureString(line).X) / 2, (font.LineSpacing) / 2);

                    MySpriteBatch.DrawString(font, line, position, color,
                         0.0f, origin, scale, SpriteEffects.None, 0.0f);

                    position.Y += scale * font.LineSpacing;
               }
          }

          public static void DrawCentered(bool autoCenter, SpriteFont font, string text, Vector2 position, Color color, 
               float rotation, Vector2 textOrigin, float scale, SpriteEffects spriteEffects, float depthLayer)
          {
               string[] lines = text.Split('\n');

               Vector2 origin;
               int size = lines.Length;

               if (autoCenter)
               {
                    position = new Vector2(position.X, position.Y -
                         ((float)(size - 1) / 2) * (font.LineSpacing * scale) - ((font.LineSpacing * scale) / 2));
               }

               else
               {
                    //position = new Vector2(position.X, position.Y - font.LineSpacing * scale);
               }

               foreach (string line in lines)
               {
                    origin = new Vector2((font.MeasureString(line).X) / 2, (font.LineSpacing) / 2);

                    MySpriteBatch.DrawString(font, line, position, color,
                         rotation, origin, scale, spriteEffects, depthLayer);

                    position.Y += scale * font.LineSpacing;
               }
          }

          #endregion

          #region (TEMP SPOT / WORK IN PROGRESS) ConvertToKey()

          public static string ConvertToString(Keys[] keysList)
          {
               string text = "";

               for (int i = 0; i < keysList.Length; i++)
               {
                    // The "!" Key.
                    if (keysList[i] == Keys.D1)
                    {
                         text = text + (char)(33);
                    }

                    // The "'" Key.
                    else if (keysList[i] == Keys.OemQuotes)
                    {
                         text = text + (char)(39);
                    }

                    // The "." Key.
                    else if (keysList[i] == Keys.OemPeriod)
                    {
                         text = text + (char)(46);
                    }

                    // No special case.
                    else
                         text = text + (char)((int)keysList[i]);
               }

               return text;
          }

          /*
          public static string ConvertToString(List<Keys> keysList)
          {
               string text = "";
               for (int i = 0; i < keysList.Count; i++)
               {
                    // The "!" Key.
                    if (keysList[i] == Keys.D1)
                    {
                         text = text + (char)(33);
                    }

                    // The "'" Key.
                    else if (keysList[i] == Keys.OemQuotes)
                    {
                         text = text + (char)(39);
                    }

                    // No special case.
                    else
                         text = text + (char)((int)keysList[i]);
               }


                         //myKeys[c] = (Keys)(int)(text[c]);
               return text;
          }
           * */

          public static List<Keys> ConvertToKey(string text)
          {
               List<Keys> myKeys = new List<Keys>();

               // Do not retrieve Keys for a null string.
               if (text == null)
                    return myKeys;

               // For now, set each Key as the "A" Key.
               for (int i = 0; i < text.Length; i++)
               {
                    myKeys.Add(Keys.A);
               }

               // Iterate through the List of Keys...
               for (int c = 0; c < text.Length; c++)
               {
                    if (text[c] == '&')
                    {
                         myKeys[c] = Keys.D7;
                    }

                    else if (text[c] == '"')
                    {
                         myKeys[c] = Keys.OemQuotes;
                    }

                    // Check if the current character has a unique Key...
                    else if (text[c] == '\"')
                    {
                         myKeys[c] = Keys.OemComma;
                    }

                    else if (text[c] == ':')
                    {
                         myKeys[c] = Keys.OemSemicolon;
                    }

                    else if (text[c] == ';')
                    {
                         myKeys[c] = Keys.OemSemicolon;
                    }

                    // The "!" Key.
                    else if (text[c] == '!')
                    {
                         myKeys[c] = Keys.D1;
                    }

                    // The "'" Key.
                    else if (text[c] == '\'')
                    {
                         myKeys[c] = Keys.OemQuotes;
                    }

                    // The "," Key.
                    else if (text[c] == ',')
                    {
                         myKeys[c] = Keys.OemComma;
                    }

                    // The "." Key.
                    else if (text[c] == '.')
                    {
                         myKeys[c] = Keys.OemPeriod;
                    }

                    // The "-" Key.
                    else if (text[c] == '-')
                    {
                         myKeys[c] = Keys.OemMinus;
                    }

                    // The "+" Key.
                    else if (text[c] == '+')
                    {
                         myKeys[c] = Keys.OemPlus;
                    }

                    // The "?" Key.
                    else if (text[c] == '?')
                    {
                         myKeys[c] = Keys.OemQuestion;
                    }

                    // The top numerican keys: 0-9.
                    else if ((int)text[c] == 48)
                    {
                         myKeys[c] = Keys.D0;
                    }

                    else if ((int)text[c] == 49)
                    {
                         myKeys[c] = Keys.D1;
                    }

                    else if ((int)text[c] == 50)
                    {
                         myKeys[c] = Keys.D2;
                    }

                    else if ((int)text[c] == 51)
                    {
                         myKeys[c] = Keys.D3;
                    }

                    else if ((int)text[c] == 52)
                    {
                         myKeys[c] = Keys.D4;
                    }

                    else if ((int)text[c] == 53)
                    {
                         myKeys[c] = Keys.D5;
                    }

                    else if ((int)text[c] == 54)
                    {
                         myKeys[c] = Keys.D6;
                    }

                    else if ((int)text[c] == 55)
                    {
                         myKeys[c] = Keys.D7;
                    }

                    else if ((int)text[c] == 56)
                    {
                         myKeys[c] = Keys.D8;
                    }

                    else if ((int)text[c] == 57)
                    {
                         myKeys[c] = Keys.D9;
                    }


                    else if ((int)text[c] >= 97)
                    {
                         myKeys[c] = (Keys)((int)(text[c]) - 32);
                    }

                    // Else the Key is not unique, and can be found simply!
                    else
                         myKeys[c] = (Keys)(int)(text[c]);
               }

               return myKeys;
          }











         

          public static char TranslateChar(Keys key, bool shift, bool capsLock, bool numLock)
	{
		switch (key)
		{
			case Keys.A: return TranslateAlphabetic('a', shift, capsLock);
			case Keys.B: return TranslateAlphabetic('b', shift, capsLock);
			case Keys.C: return TranslateAlphabetic('c', shift, capsLock);
			case Keys.D: return TranslateAlphabetic('d', shift, capsLock);
			case Keys.E: return TranslateAlphabetic('e', shift, capsLock);
			case Keys.F: return TranslateAlphabetic('f', shift, capsLock);
			case Keys.G: return TranslateAlphabetic('g', shift, capsLock);
			case Keys.H: return TranslateAlphabetic('h', shift, capsLock);
			case Keys.I: return TranslateAlphabetic('i', shift, capsLock);
			case Keys.J: return TranslateAlphabetic('j', shift, capsLock);
			case Keys.K: return TranslateAlphabetic('k', shift, capsLock);
			case Keys.L: return TranslateAlphabetic('l', shift, capsLock);
			case Keys.M: return TranslateAlphabetic('m', shift, capsLock);
			case Keys.N: return TranslateAlphabetic('n', shift, capsLock);
			case Keys.O: return TranslateAlphabetic('o', shift, capsLock);
			case Keys.P: return TranslateAlphabetic('p', shift, capsLock);
			case Keys.Q: return TranslateAlphabetic('q', shift, capsLock);
			case Keys.R: return TranslateAlphabetic('r', shift, capsLock);
			case Keys.S: return TranslateAlphabetic('s', shift, capsLock);
			case Keys.T: return TranslateAlphabetic('t', shift, capsLock);
			case Keys.U: return TranslateAlphabetic('u', shift, capsLock);
			case Keys.V: return TranslateAlphabetic('v', shift, capsLock);
			case Keys.W: return TranslateAlphabetic('w', shift, capsLock);
			case Keys.X: return TranslateAlphabetic('x', shift, capsLock);
			case Keys.Y: return TranslateAlphabetic('y', shift, capsLock);
			case Keys.Z: return TranslateAlphabetic('z', shift, capsLock);
			
			case Keys.D0: return (shift) ? ')' : '0';
			case Keys.D1: return (shift) ? '!' : '1';
			case Keys.D2: return (shift) ? '@' : '2';
			case Keys.D3: return (shift) ? '#' : '3';
			case Keys.D4: return (shift) ? '$' : '4';
			case Keys.D5: return (shift) ? '%' : '5';
			case Keys.D6: return (shift) ? '^' : '6';
			case Keys.D7: return (shift) ? '&' : '7';
			case Keys.D8: return (shift) ? '*' : '8';
			case Keys.D9: return (shift) ? '(' : '9';
			
			case Keys.Add:      return '+';
			case Keys.Divide:   return '/';
			case Keys.Multiply: return '*';
			case Keys.Subtract: return '-';

			case Keys.Space: return ' ';
			case Keys.Tab:   return '\t';

			case Keys.Decimal: if (numLock && !shift) return '.'; break;
			case Keys.NumPad0: if (numLock && !shift) return '0'; break;
			case Keys.NumPad1: if (numLock && !shift) return '1'; break;
			case Keys.NumPad2: if (numLock && !shift) return '2'; break;
			case Keys.NumPad3: if (numLock && !shift) return '3'; break;
			case Keys.NumPad4: if (numLock && !shift) return '4'; break;
			case Keys.NumPad5: if (numLock && !shift) return '5'; break;
			case Keys.NumPad6: if (numLock && !shift) return '6'; break;
			case Keys.NumPad7: if (numLock && !shift) return '7'; break;
			case Keys.NumPad8: if (numLock && !shift) return '8'; break;
			case Keys.NumPad9: if (numLock && !shift) return '9'; break;
			
			case Keys.OemBackslash:     return shift ? '|' : '\\';
			case Keys.OemCloseBrackets: return shift ? '}' : ']'; 
			case Keys.OemComma:         return shift ? '<' : ','; 
			case Keys.OemMinus:         return shift ? '_' : '-'; 
			case Keys.OemOpenBrackets:  return shift ? '{' : '['; 
			case Keys.OemPeriod:        return shift ? '>' : '.'; 
			case Keys.OemPipe:          return shift ? '|' : '\\';
			case Keys.OemPlus:          return shift ? '+' : '='; 
			case Keys.OemQuestion:      return shift ? '?' : '/'; 
			case Keys.OemQuotes:        return shift ? '"' : '\'';
			case Keys.OemSemicolon:     return shift ? ':' : ';'; 
			case Keys.OemTilde:         return shift ? '~' : '`'; 
		}
		
		return (char)0;
	}
	
	public static char TranslateAlphabetic(char baseChar, bool shift, bool capsLock)
	{
		return (capsLock ^ shift) ? char.ToUpper(baseChar) : baseChar;
	}


          #endregion
     }
}