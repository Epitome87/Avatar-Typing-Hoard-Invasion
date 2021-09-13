#region File Description
//-----------------------------------------------------------------------------
// TextObject.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.Graphics;
using PixelEngine.Screen;
#endregion

namespace PixelEngine.Text
{
     /// <remarks>
     /// Contains the basic properties and functionality 
     /// needed when rendering more-robust text.
     /// </remarks>
     public class TextObject
     {
          #region AlignmentType Enum

          /// <summary>
          /// An Enum which allows for quick and friendlier text alignment.
          /// </summary>
          public enum AlignmentType
          {
               // Alignments at the very top.
               TopLeft = 1, TopCenter, TopRight,

               // Alignments at the top half.
               TopHalfLeft, TopHalfCenter, TopHalfRight,

               // Alignments at the very center.
               Centerleft, Center, CenterRight,

               // Alignments at the bottom half.
               BottomHalfLeft, BottomHalfCenter, BottomHalfRight,

               // Alignments at the very bottom.
               BottomLeft, BottomCenter, BottomRight
          }

          #endregion

          #region Fields

          private string text;
          private TextManager.FontType fontType;
          private Vector2 position;
          private Color color;
          private float rotation;
          private Vector2 origin;
          private float scale;
          private bool isVisible;
          private bool isCenter;
          private AlignmentType alignment;
          private List<TextEffect> textEffects;

          #endregion

          #region Properties

          /// <summary>
          /// Gets or sets the Text string that this Text Object will render.
          /// </summary>
          public string Text
          {
               get { return text; }
               set { text = value; }
          }

          /// <summary>
          /// Gets or sets the Enum FontType used to render this text with.
          /// </summary>
          public TextManager.FontType FontType
          {
               get { return fontType; }
               set { fontType = value; }
          }

          /// <summary>
          /// Gets the SpriteFont used to render this Text with.
          /// </summary>
          public SpriteFont Font
          {
               get { return TextManager.Fonts[(int)(fontType)].SpriteFont; }
          }

          /// <summary>
          /// Gets a collection of all the characters that are included
          /// with this Font.
          /// 
          /// This is useful for checking to ensure the Font doesn't 
          /// attempt to render a character it does not support.
          /// </summary>
          public ReadOnlyCollection<char> IncludedCharacters
          {
               get { return TextManager.Fonts[(int)(fontType)].SpriteFont.Characters; }
          }

          /// <summary>
          /// Position of the Text (where it is to be rendered).
          /// </summary>
          public Vector2 Position
          {
               get { return position; }
               set { position = value; }
          }

          /// <summary>
          /// Color the Text will be rendered in.
          /// </summary>
          public Color Color
          {
               get { return color; }
               set { color = value; }
          }

          /// <summary>
          /// Rotation of the Text.
          /// </summary>
          public float Rotation
          {
               get { return rotation; }
               set { rotation = value; }
          }

          /// <summary>
          /// Origin of the Text.
          /// </summary>
          public Vector2 Origin
          {
               get { return origin; }
               set { origin = value; }
          }

          /// <summary>
          /// Scale the Text is to be rendered with.
          /// </summary>
          public float Scale
          {
               get { return scale; }
               set { scale = value; }
          }

          /// <summary>
          /// Whether or not the Text is visible, and thus rendered or not.
          /// </summary>
          public bool IsVisible
          {
               get { return isVisible; }
               set { isVisible = value; }
          }

          /// <summary>
          /// Whether or not the Text is centered, and thus rendered 
          /// with each line centered or not.
          /// </summary>
          public bool IsCenter
          {
               get { return isCenter; }
               set { isCenter = value; }
          }

          /// <summary>
          /// Gets or Sets the AlignmentType of the TextObject.
          /// 
          /// For example, AlignmentType.Center would position
          /// the TextObject in the center of the screen.
          /// </summary>
          public AlignmentType Alignment
          {
               get { return alignment; }
               set
               {
                    alignment = value;
                    SetPositionByAlignment(value);
               }
          }

          /// <summary>
          /// TextEffect used when drawing. 
          /// Default is none.
          /// 
          /// Use this rather than AddTextEffect when you want to ensure
          /// that this is the only TextEffect active.
          /// </summary>
          public TextEffect TextEffect
          {
               get
               {
                    if (textEffects != null)
                    {
                         if (textEffects.Count > 0)
                         {
                              return textEffects[0]; // This could be null
                         }

                         else
                              return null;
                    }

                    else
                    {
                         return null;
                    }
               }

               set
               {
                    // Clear the list, then add this TextEffect.
                    textEffects.Clear();
                    textEffects.Add(value);
               }
          }

          #endregion

          #region Initialization

          /// <summary>
          /// Default Constructor.
          /// </summary>
          public TextObject()
          {
               text = "";
               alignment = AlignmentType.Centerleft;
               position = new Vector2();
               fontType = TextManager.FontType.EnemyFont;
               color = Color.Yellow;
               rotation = 0.0f;
               origin = new Vector2();
               scale = 1.0f;
               isVisible = true;
               textEffects = new List<TextEffect>();
          }

          /// <summary>
          /// Constructor which sets the Text and Position properties.
          /// </summary>
          public TextObject(string text, Vector2 position)
               : this()
          {
               Text = text;
               Position = position;
               textEffects = new List<TextEffect>();
          }

          /// <summary>
          /// Constructor with parameters to set all Text Object properties.
          /// </summary>
          public TextObject(string text, Vector2 position, PixelEngine.Text.TextManager.FontType fontType,
               Color color, float rotation, Vector2 origin, float scale, bool isVisible, params TextEffect[] textEffect)
          {
               Text = text;
               Position = position;
               FontType = fontType;
               Color = color;
               Rotation = rotation;
               Origin = origin;
               Scale = scale;
               IsVisible = isVisible;
               textEffects = new List<TextEffect>();

               foreach (TextEffect effect in textEffect)
               {
                    textEffects.Add(effect);
               }
          }

          #endregion

          #region Update

          /// <summary>
          /// Updates the Text Object if it Is Visible and has a valid Text Effect.
          /// (Otherwise, no Update is needed.)
          /// </summary>
          public void Update(GameTime gameTime)
          {
               // Return if the Text isn't even Visible.
               if (!this.isVisible)
                    return;

               // Return if there is no valid TextEffect to update.
               if (this.textEffects == null)
                    return;

               // Otherwise, perform each TextEffect's Update logic.
               foreach (TextEffect effect in textEffects)
               {
                    if (effect != null)
                         effect.Update(gameTime, this);
               }
          }

          #endregion

          #region Draw

          /// <summary>
          /// Draws the Text Object if it Is Visible.
          /// If the Text Object has a valid Text Effect, that effect's Draw is called instead.
          /// </summary>
          public void Draw(GameTime gameTime)
          {
               // Return if the Text isn't even Visible.
               if (!this.isVisible)
                    return;

               SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

               // Ensure we have a valid List of TextEffects...
               if (this.textEffects != null)
               {
                    // If we have a valid List, but no TextEffects, render the text normally.
                    if (this.textEffects.Count < 1)
                    {
                         if (IsCenter)
                         {
                              TextManager.DrawCentered(false, this.Font, Text, Position, Color,
                                   Rotation, Origin, Scale, SpriteEffects.None, 0.0f);
                         }

                         else
                         {
                              MySpriteBatch.DrawString(this.Font, text, position, color,
                                                       rotation, origin, scale, SpriteEffects.None, 0);
                         }
                    }

                    // Otherwise, we have a valid List and at least 1 or more TextEffects, so render all of them!
                    else
                    {
                         foreach (TextEffect effect in textEffects)
                         {
                              // Only render if the effect is not null.
                              if (effect != null)
                              {
                                   effect.Draw(gameTime, this);
                              }
                         }
                    }
               }

               // Otherwise TextEffects is null, so just render the text normally.
               else
               {
                    if (IsCenter)
                    {
                         TextManager.DrawCentered(false, this.Font, Text, Position, Color,
                              Rotation, Origin, Scale, SpriteEffects.None, 0.0f);
                    }

                    else
                    {
                         MySpriteBatch.DrawString(this.Font, text, position, color,
                                                  rotation, origin, scale, SpriteEffects.None, 0);
                    }
               }
          }

          #endregion

          #region Helper Alignment Methods

          /// <summary>
          /// A Helper method which calculates the Text's Position
          /// based on the AlignmentType it is using.
          /// </summary>
          /// <param name="align"></param>
          private void SetPositionByAlignment(AlignmentType align)
          {
               Vector2 pos = new Vector2();

               // X Position
               if ((int)align % 2 == 0)
               {
                    pos.X = EngineCore.ScreenCenter.X;
               }

               else if ((int)align % 3 == 0)
               {
                    pos.X = 1280 - 125;
               }

               else
               {
                    pos.X = 125;
               }


               // Y Position.
               if ((int)align / 3.0f <= 1)
               {
                    pos.Y = 100;
               }

               else if ((int)align / 3.0f > 1 && (int)align / 3.0f <= 2)
               {
                    pos.Y = 0.5f * EngineCore.ScreenCenter.Y;
               }

               else if ((int)align / 3.0f > 2 && (int)align / 3.0f <= 3)
               {
                    pos.Y = EngineCore.ScreenCenter.Y;
               }

               else if ((int)align / 3.0f > 3 && (int)align / 3.0f <= 4)
               {
                    pos.Y = 1.5f * EngineCore.ScreenCenter.Y;
               }

               else
               {
                    pos.Y = 720 - 100;
               }

               Position = pos;
          }

          #endregion

          #region Public Add / Remove TextEffect Methods

          /// <summary>
          /// Adds another TextEffect to this TextObject's list 
          /// of TextEffects. This allows the TextObject to utilize
          /// more than one TextEffect, allowing for many and creative
          /// styles of strings to be rendered.
          /// </summary>
          /// <param name="effect">The TextEffect to be added.</param>
          /// <returns>
          /// Returns True if the effect was added.
          /// Returns False if the effect already exists.
          /// </returns>
          public bool AddTextEffect(TextEffect effect)
          {
               if (textEffects != null)
               {
                    if (!textEffects.Contains(effect))
                         textEffects.Add(effect);

                    return true;
               }

               return false;
          }

          /// <summary>
          /// Removes the specified TextEffect from this TextObject's list
          /// of TextEffects.
          /// </summary>
          /// <param name="effect">The TextEffect to be removed.</param>
          /// <returns>Returns True if the effect is or was removed.</returns>
          public bool RemoveTextEffect(TextEffect effect)
          {
               if (textEffects != null)
               {
                    textEffects.Remove(effect);

                    // Return True regardless of if this effect actually ever existed in the list;
                    // The point is, it's not present, therefore it being gone is technically True!
                    return true;
               }

               return false;
          }

          #endregion
     }
}