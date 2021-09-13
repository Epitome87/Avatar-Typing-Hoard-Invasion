#region File Description
//-----------------------------------------------------------------------------
// MenuEntry.cs
// Matt McGrath, with help provided by:
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
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

namespace PixelEngine.Menu
{
     /// <summary>
     /// Helper class represents a single entry in a MenuScreen. By default this
     /// just draws the entry text string, but it can be customized to display menu
     /// entries in different ways. This also provides an event that will be raised
     /// when the menu entry is selected.
     /// </summary>
     public class MenuEntry
     {
          #region Fields

          private string text;
          private Vector2 menuPosition;
          private Vector2 origin;
          private bool isCenter;

          // Fields to help customize an Entry's color appearance..
          private Color selectedColor = Color.White;
          private Color unselectedColor = Color.White;

          // Fields to provide a Menu Entry "Description".
          private string menuDescription = String.Empty;
          private Color descriptionColor = Color.CornflowerBlue;
          private Vector2 descriptionPosition;

          // Fields to help customize an Entry's appearance.
          private bool showIcon = false;
          private bool showPlainBorder = false;
          public Color BorderColor = Color.DarkOrange * (50f / 255f);

          protected static GameResourceTexture2D ButtonIcon;
          protected static GameResourceTexture2D blankTexture;

          private float fontScale = 1.0f;
          private float descriptionFontScale = 1.0f;

          private float additionalVerticalSpacing = 0.0f;

          // NEW FEATURES


          public bool useCustomBorderSize = false;
          private bool showGradientBorder = true;

          // The actual texture for the border.
          private static GameResourceTexture2D GradientBorderTexture;

          // For the border texture size and scale.
          public Vector2 menuEntryBorderSize = new Vector2(500, 100);
          public Vector2 menuEntryBorderScale = new Vector2(1.0f, 1.0f);

          // For the border texture colors.
          private Color selectedBorderColor = Color.DarkOrange;
          private Color unselectedBorderColor = Color.CornflowerBlue;

          private Vector2 renderedPosition = new Vector2();

          // END NEW FEATURES

          private Rectangle menuEntryRectangle;

          private bool isPulsating = true;

          /// <summary>
          /// Tracks a fading selection effect on the entry.
          /// </summary>
          /// <remarks>
          /// The entries transition out of the selection effect when they are deselected.
          /// </remarks>
          float selectionFade;

          #endregion

          #region Properties

          /// <summary>
          /// Gets or sets the text of this menu entry.
          /// </summary>
          public string Text
          {
               get { return text; }
               set { text = value; }
          }

          /// <summary>
          /// Gets or sets the text of this menu entry.
          /// </summary>
          public string Description
          {
               get { return menuDescription; }
               set { menuDescription = value; }
          }

          /// <summary>
          /// Gets or sets the IsCenter of this menu entry.
          /// </summary>
          /// <remarks>
          /// If set to True, Menu Entries are centered on screen.
          /// If set to False, they are left-aligned.
          /// </remarks>
          public bool IsCenter
          {
               get { return isCenter; }
               set { isCenter = value; }
          }

          /// <summary>
          /// Gets or sets the Color the Entry is while Selected.
          /// </summary>
          public Color SelectedColor
          {
               get { return selectedColor; }
               set { selectedColor = value; }
          }

          /// <summary>
          /// Gets or sets the Color the Entry is while Unselected.
          /// </summary>
          public Color UnselectedColor
          {
               get { return unselectedColor; }
               set { unselectedColor = value; }
          }

          /// <summary>
          /// Gets or sets the Color the Description string is.
          /// </summary>
          public Color DescriptionColor
          {
               get { return descriptionColor; }
               set { descriptionColor = value; }
          }

          /// <summary>
          /// Gets or sets the position the Description string is at.
          /// </summary>
          public Vector2 DescriptionPosition
          {
               get { return descriptionPosition; }
               set { descriptionPosition = value; }
          }

          /// <summary>
          /// Gets or sets whether or not to show the Button Icon
          /// beside the Menu Entry.
          /// </summary>
          public bool ShowIcon
          {
               get { return showIcon; }
               set { showIcon = value; }
          }

          /// <summary>
          /// Gets or sets whether or not to show the transparent
          /// border highlighting the selected Menu Entry.
          /// </summary>
          public bool ShowBorder
          {
               get { return showPlainBorder; }
               set
               {
                    showPlainBorder = value;

                    // We can't have both a plain and gradient border,
                    // so if we're showing the plain, don't show the gradient.
                    if (showPlainBorder)
                    {
                         showGradientBorder = false;
                    }
               }
          }

          /// <summary>
          /// Gets or sets whether or not we should show / use the 
          /// gradient texture for our menu entry's border.
          /// This type of texture will replace the plain transparent one.
          /// 
          /// Default value: True
          /// </summary>
          public bool ShowGradientBorder
          {
               get { return showGradientBorder; }
               set
               {
                    showGradientBorder = value;

                    // We can't have both a plain and gradient border,
                    // so if we're showing the gradient, don't show the plain.
                    if (showGradientBorder)
                    {
                         showPlainBorder = false;
                    }
               }
          }

          /// <summary>
          /// Gets or sets the color of the menu entry's border - when selected.
          /// </summary>
          public Color SelectedBorderColor
          {
               get { return selectedBorderColor; }
               set { selectedBorderColor = value; }
          }

          /// <summary>
          /// Gets or sets the Color of the menu entry's border - when unselected.
          /// </summary>
          public Color UnselectedBorderColor
          {
               get { return unselectedBorderColor; }
               set { unselectedBorderColor = value; }
          }

          /// <summary>
          /// Gets or sets the Position of this Menu Entry.
          /// </summary>
          public Vector2 Position
          {
               get { return menuPosition; }
               set { menuPosition = value; }
          }

          /// <summary>
          /// Gets or sets the Origin of this Menu Entry.
          /// </summary>
          public Vector2 Origin
          {
               get { return origin; }
               set { origin = value; }
          }

          /// <summary>
          /// Gets or sets the Scale of this Menu Entry's text.
          /// </summary>
          public float FontScale
          {
               get { return fontScale; }
               set { fontScale = value; }
          }

          /// <summary>
          /// Gets or sets the Scale of this 
          /// Menu Entry's Description text.
          /// </summary>
          public float DescriptionFontScale
          {
               get { return descriptionFontScale; }
               set { descriptionFontScale = value; }
          }

          /// <summary>
          /// Gets the Vertical Spacing used between this MenuEntry and the next.
          /// Default: The height of this MenuEntry.
          /// Note: Set AdditionalVerticalSpacing for less / more padding.
          /// </summary>
          public float VerticalSpacing
          {
               get
               {
                    return EngineCore.ResolutionScale * this.GetTrueHeight() *
                         this.FontScale + additionalVerticalSpacing;
               }
          }

          /// <summary>
          /// Gets or sets the Additional Vertical Spacing used between this
          /// MenuEntry and the next.
          /// Default: 0
          /// </summary>
          public float AdditionalVerticalSpacing
          {
               get { return additionalVerticalSpacing; }
               set { additionalVerticalSpacing = value; }
          }

          public Rectangle MenuEntryRectangle
          {
               get { return menuEntryRectangle; }

               set { menuEntryRectangle = value; }
          }

          public Vector2 RenderedPosition
          {
               get { return renderedPosition; }
               set { renderedPosition = value; }
          }

          /// <summary>
          /// Gets or sets whether or not this menu entry should
          /// have its text use a pulsating effect.
          /// </summary>
          public bool IsPulsating
          {
               get { return isPulsating; }
               set { isPulsating = value; }
          }

          #endregion

          #region Events

          /// <summary>
          /// Event raised when the menu entry is selected.
          /// </summary>
          public event EventHandler<PlayerIndexEventArgs> Selected;

          /// <summary>
          /// Method for raising the Selected event.
          /// </summary>
          protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
          {
               if (Selected != null)
                    Selected(this, new PlayerIndexEventArgs(playerIndex));
          }

          #endregion

          #region Initialization

          /// <summary>
          /// Constructs a new Menu Entry with the specified Text.
          /// </summary>
          public MenuEntry(string menuText)
          {
               text = menuText;

               menuPosition = new Vector2(PixelEngine.EngineCore.ScreenCenter.X, 0);

               descriptionPosition = new Vector2(
                    EngineCore.ScreenCenter.X, EngineCore.GraphicsInformation.ScreenHeight - 170);//180);

               IsCenter = true;
               ShowIcon = true;
               ShowBorder = false;
          }

          /// <summary>
          /// Constructs a new Menu Entry with the specified Text and Description.
          /// </summary>
          public MenuEntry(string text, string description)
               : this(text)
          {
               menuDescription = description;
          }

          private static GameResourceTexture2D button1Icon;
          private static GameResourceTexture2D button2Icon;

          /// <summary>
          /// Load relevant content (Button Icon texture, etc).
          /// </summary>
          public static void LoadContent()
          {
               ContentManager content;
               content = ScreenManager.Game.Content;

               // Icon to display the "A" Button next to Selected Entries.
               //ButtonIcon = ResourceManager.LoadTexture(@"Buttons\xboxControllerButtonA");

               // NEW AS OF 5-31-2011: Testing for alternating Buttin Icon images:
               button1Icon = ResourceManager.LoadTexture(@"Buttons\xboxControllerButtonA");
               button2Icon = ResourceManager.LoadTexture(@"Buttons\EnterKey");
               //ButtonIcon = button1Icon;




               // Blank texture used to highlight Selected Entries.
               blankTexture = ResourceManager.LoadTexture(@"Textures\Blank Textures\blank");

               // Texture used to border the Menu Entry.
               GradientBorderTexture = ResourceManager.LoadTexture(@"Textures\Blank Textures\Test3b");//Blank_Rounded_Wide_WithBorder");
          }

          #endregion

          #region Update

          private float buttonUpdatePeriod = 1000.0f;
          private float buttonElapsedTime = 0.0f;
          private bool showFirstIcon = true;

          /// <summary>
          /// Updates the menu entry.
          /// </summary>
          public virtual void Update(MenuScreen screen, bool isSelected,
                                                        GameTime gameTime)
          {
               // When the menu selection changes, entries gradually fade between
               // their selected and deselected appearance, rather than instantly
               // popping to the new state.
               float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

               if (isSelected)
                    selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
               else
                    selectionFade = Math.Max(selectionFade - fadeSpeed, 0);

               //if (isSelected)
               {
                    // NEW FOR ALTERNATING BUTTONS, 5-31-2011:
                    buttonElapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (buttonElapsedTime >= buttonUpdatePeriod)
                    {
                         /*
                         if (showFirstIcon)
                              ButtonIcon = button2Icon;

                         else
                              ButtonIcon = button1Icon;
                         */
                         buttonElapsedTime = 0.0f;
                         showFirstIcon = !showFirstIcon;
                    }
               }
          }

          #endregion

          #region Draw

          /// <summary>
          /// Draws the menu entry. This can be overridden to customize the appearance.
          /// </summary>
          public virtual void Draw(MenuScreen screen, Vector2 position,
               bool isSelected, GameTime gameTime)
          {
               // NEW TESTING

               renderedPosition = position;

               // END NEW TEST

               // Color the menu entry according to whether it's selected.
               Color color = isSelected ? selectedColor : unselectedColor;

               // Pulsate the size of the selected menu entry.
               double time = gameTime.TotalGameTime.TotalSeconds;

               float pulsate = 0.0f;

               if (isPulsating)
               {
                    pulsate = (float)Math.Sin(time * 6) + 1;
               }

               float scale = fontScale + pulsate * 0.05f * selectionFade;

               // Modify the alpha to fade text out during transitions.
               color = color * (screen.TransitionAlpha / 255f);

               SpriteFont font = ScreenManager.Font;
               Vector2 buttonPos = new Vector2();
               float scaleFactor = EngineCore.ResolutionScale * fontScale;



               // Testing new position for buttons
               if (IsCenter)
               {
                    if (!useCustomBorderSize)
                    {
                         buttonPos =
                              new Vector2(position.X - (menuEntryBorderSize.X / 2) - 50, position.Y - ((scaleFactor * font.LineSpacing / 2)));


                         int buttonWidth = (int)(scaleFactor * this.GetTrueHeight());
                         buttonPos =
                              new Vector2(position.X - (menuEntryBorderSize.X / 2) - buttonWidth, position.Y - ((scaleFactor * font.LineSpacing / 2)));
                    }

                    else
                    {
                         buttonPos =
                              new Vector2(position.X - (menuEntryBorderSize.X / 2) - 50, position.Y);
                    }
               }

               else
               {
                    buttonPos = new Vector2(200f - (scaleFactor * font.LineSpacing * 2.0f), position.Y - (scaleFactor * font.LineSpacing / 2.0f));

                    buttonPos = new Vector2(200f - (scaleFactor * this.GetTrueHeight() * 2.0f), position.Y - (scaleFactor * this.GetTrueHeight() / 2.0f));
               }



               if (!useCustomBorderSize)
               {
                    /*
                    menuEntryRectangle = new Rectangle(
                         (int)position.X - (int)(menuEntryBorderSize.X / 2),
                         (int)position.Y - 10 - (int)(menuEntryBorderScale.Y * EngineCore.ResolutionScale * fontScale * font.LineSpacing / 2),
                         (int)(menuEntryBorderSize.X * menuEntryBorderScale.X),
                         (int)(EngineCore.ResolutionScale * font.LineSpacing * fontScale * menuEntryBorderScale.Y) + 20);
                    */
                    menuEntryRectangle =
                         new Rectangle(
                                   (int)position.X - (int)(menuEntryBorderSize.X / 2),
                                   (int)position.Y - 10 - (int)(menuEntryBorderScale.Y * EngineCore.ResolutionScale * fontScale * font.LineSpacing / 2),// this.GetTrueHeight() / 2),
                                   (int)(menuEntryBorderSize.X * menuEntryBorderScale.X),
                                   (int)(EngineCore.ResolutionScale * this.GetTrueHeight() * fontScale * menuEntryBorderScale.Y) + 20);
               }

               else
               {
                    menuEntryRectangle = new Rectangle(
                         (int)position.X - (int)(menuEntryBorderSize.X / 2),
                         (int)position.Y - 10 - (int)(menuEntryBorderScale.Y * EngineCore.ResolutionScale * fontScale * font.LineSpacing / 2),
                         (int)(menuEntryBorderSize.X * menuEntryBorderScale.X),
                         (int)(EngineCore.ResolutionScale * this.GetTrueHeight() * fontScale * menuEntryBorderScale.Y) + 20);
               }


               if (isSelected)
               {
                    TextManager.DrawCentered(false, font, menuDescription, descriptionPosition,
                          descriptionColor * (screen.TransitionAlpha / 255f),
                          descriptionFontScale);

                    if (ShowBorder)
                    {
                         // Draw the box.
                         MySpriteBatch.Draw(blankTexture.Texture2D, new Rectangle(0, (int)position.Y - (int)(EngineCore.ResolutionScale * fontScale * font.LineSpacing / 2),
                             PixelEngine.EngineCore.GraphicsInformation.ScreenWidth, (int)(EngineCore.ResolutionScale * this.GetTrueHeight() * fontScale)),//font.LineSpacing * fontScale)),
                             BorderColor);
                         // EDITED font.LineSpace to this.GetTrueHeight(). IN TESTING PHASESEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE
                    }

                    if (showGradientBorder)
                    {
                         // Draw the border Texture.

                         MySpriteBatch.Draw(GradientBorderTexture.Texture2D,
                              menuEntryRectangle, selectedBorderColor);
                    }

                    if (ShowIcon)
                    {
                         Color iconColor = Color.White * (screen.TransitionAlpha / 255f);
                         /*
                         {
                              MySpriteBatch.Draw(ButtonIcon.Texture2D,
                                   new Rectangle(
                                        (int)(buttonPos.X),
                                        (int)(buttonPos.Y),
                                        (int)(EngineCore.ResolutionScale * this.GetTrueHeight() * fontScale),
                                        (int)(EngineCore.ResolutionScale * this.GetTrueHeight() * fontScale)),
                                        iconColor);
                         }
                         */
                         if (showFirstIcon)
                         {
                              MySpriteBatch.Draw(button1Icon.Texture2D, new Rectangle(
                                                  (int)(buttonPos.X),
                                                  (int)(buttonPos.Y),
                                                  (int)(EngineCore.ResolutionScale * this.GetTrueHeight() * fontScale),
                                                  (int)(EngineCore.ResolutionScale * this.GetTrueHeight() * fontScale)),
                                                  iconColor);
                         }

                         else
                         {
                              MySpriteBatch.Draw(button2Icon.Texture2D, new Rectangle(
                                              (int)(buttonPos.X),
                                              (int)(buttonPos.Y),
                                              (int)(EngineCore.ResolutionScale * this.GetTrueHeight() * fontScale),
                                              (int)(EngineCore.ResolutionScale * this.GetTrueHeight() * fontScale)),
                                              iconColor);
                         }
                    }
               }

               // Else this menu entry is not selected.
               else
               {
                    if (showGradientBorder)
                    {
                         MySpriteBatch.Draw(GradientBorderTexture.Texture2D,
                              menuEntryRectangle, unselectedBorderColor);
                    }
               }

               // Draw text, centered on the middle of each line.
               if (IsCenter)
               {
                    TextManager.DrawCentered(false, font, text, position, color, scale);
               }

               else
               {
                    Origin = new Vector2(0, font.LineSpacing / 2.0f);

                    TextManager.Draw(font, text, new Vector2(200f, position.Y), color, 0,
                         Origin, scale);
               }
          }

          #endregion

          #region Public Methods

          /// <summary>
          /// Queries how much space this menu entry requires.
          /// </summary>
          public virtual int GetHeight(MenuScreen screen)
          {
               return ScreenManager.Font.LineSpacing;
          }

          /// <summary>
          /// Queries how much space this menu entry requires -
          /// line breaks included.
          /// </summary>
          /// <returns></returns>
          public virtual float GetTrueHeight()
          {
               return ScreenManager.Font.MeasureString(this.Text).Y;
          }

          #endregion
     }
}
