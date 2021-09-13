#region File Description
//-----------------------------------------------------------------------------
// MenuScreen.cs
// Matt McGrath, with help provided by:
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.Audio;
using PixelEngine.Menu;
using PixelEngine.Text;
#endregion

namespace PixelEngine.Screen
{
     /// <remarks>
     /// Base class for screens that contain a menu of options. The user can
     /// move up and down to select an entry, or cancel to back out of the screen.
     /// </remarks>
     public abstract class MenuScreen : GameScreen
     {
          #region Fields

          // The list of menu entries found within this menu screen.
          protected List<MenuEntry> menuEntries = new List<MenuEntry>();

          // The Menu's Title, Position, Color and Scale to be rendered.
          protected string menuTitle;
          protected Vector2 menuTitlePosition = new Vector2(EngineCore.ScreenCenter.X, 100);
          protected Color menuTitleColor = Color.CornflowerBlue;
          protected float titleFontScale = 1.5f;

          // The speed at which the Menu Title transitions.
          protected float menuTitleTransitionSpeed = 100f;

          // The menu's sounds.
          protected string menuScrollSound = "MenuScroll";
          protected string buttonPressSound = "ButtonPress";

          // Begin with the first menu entry being the one selected.
          protected int selectedEntry = 0;

          protected int numberOfColumns = 1;

          #endregion

          #region Properties

          /// <summary>
          /// Gets the list of menu entries, so derived classes can add
          /// or change the menu contents.
          /// </summary>
          protected IList<MenuEntry> MenuEntries
          {
               get { return menuEntries; }
          }

          /// <summary>
          /// Get or Set the menu's title / header string.
          /// </summary>
          protected string MenuTitle
          {
               get { return menuTitle; }
               set { menuTitle = value; }
          }

          /// <summary>
          /// Get or Set the Color the menu's Title string is rendered in.
          /// </summary>
          protected Color MenuTitleColor
          {
               get { return menuTitleColor; }
               set { menuTitleColor = value; }
          }

          /// <summary>
          /// Get or Set the Position the menu's Title string is rendered at.
          /// </summary>
          protected Vector2 MenuTitlePosition
          {
               get { return menuTitlePosition; }
               set { menuTitlePosition = value; }
          }

          /// <summary>
          /// Get or Set the scale the Title string is rendered in.
          /// </summary>
          protected float MenuTitleFontScale
          {
               get { return titleFontScale; }
               set { titleFontScale = value; }
          }

          /// <summary>
          /// Get or Set the 0-indexed number representing which
          /// menu entry is currently selected.
          /// </summary>
          protected int SelectedMenuEntry
          {
               get { return selectedEntry; }
               set { selectedEntry = value; }
          }

          /// <summary>
          /// Get or Set the sound (asset name) that plays when
          /// the menu is scrolled through.
          /// </summary>
          protected string MenuScrollSound
          {
               get { return menuScrollSound; }
               set { menuScrollSound = value; }
          }

          /// <summary>
          /// Get or Set the sound (asset name) that plays when
          /// a button is pressed.
          /// </summary>
          protected string ButtonPressSound
          {
               get { return buttonPressSound; }
               set { buttonPressSound = value; }
          }

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public MenuScreen(string menuTitle)
          {
               this.menuTitle = menuTitle;

               TransitionOnTime = TimeSpan.FromSeconds(0.5f);
               TransitionOffTime = TimeSpan.FromSeconds(0.0f);
          }

          public override void LoadContent()
          {
               base.LoadContent();
               MenuEntry.LoadContent();
          }

          public override void UnloadContent()
          {

               base.UnloadContent();
          }
          #endregion

          #region Handle Input

          /// <summary>
          /// Responds to user input, changing the selected entry and accepting
          /// or cancelling the menu.
          /// </summary>
          public override void HandleInput(InputState input)
          {
               // Move to the previous menu entry?
               if (input.IsMenuUp(ControllingPlayer))
               {
                    selectedEntry--;

                    if (selectedEntry < 0)
                    {
                         if (menuEntries.Count == 0)
                              selectedEntry = 0;

                         else
                              selectedEntry = menuEntries.Count - 1;
                    }

                    // Only play the sound if there's a menu entry to scroll to.
                    if (menuEntries.Count > 1)
                         AudioManager.PlayCue(menuScrollSound);
               }

               // Move to the next menu entry?
               else if (input.IsMenuDown(ControllingPlayer))
               {
                    selectedEntry++;

                    if (selectedEntry >= menuEntries.Count)
                    {
                         selectedEntry = 0;
                    }

                    // Only play the sound if there's a menu entry to scroll to.
                    if (menuEntries.Count > 1)
                         AudioManager.PlayCue(menuScrollSound);
               }

               // Move to the left menu entry?
               else if (input.IsMenuLeft(ControllingPlayer))
               {
                    // The If is just a test; normally there's no If statement.
                    if (numberOfColumns > 1)
                    {
                         selectedEntry -=
                              (int)Math.Ceiling(menuEntries.Count / (float)numberOfColumns);

                         if (selectedEntry < 0)
                         {
                              selectedEntry = 0;
                         }
                    }

                    // Only play the sound if there's a menu entry to scroll to.
                    if (menuEntries.Count > 1)
                         AudioManager.PlayCue(menuScrollSound);
               }

               else if (input.IsMenuRight(ControllingPlayer))
               {
                    // The If is just a test; normally there's no If statement.
                    if (numberOfColumns > 1)
                    {
                         selectedEntry +=
                              (int)Math.Ceiling(menuEntries.Count / (float)numberOfColumns);

                         if (selectedEntry >= menuEntries.Count)
                              selectedEntry = menuEntries.Count - 1;
                    }

                    // Only play the sound if there's a menu entry to scroll to.
                    if (menuEntries.Count > 1)
                         AudioManager.PlayCue(menuScrollSound);
               }

               // Accept or cancel the menu? We pass in our ControllingPlayer, which may
               // either be null (to accept input from any player) or a specific index.
               // If we pass a null controlling player, the InputState helper returns to
               // us which player actually provided the input. We pass that through to
               // OnSelectEntry and OnCancel, so they can tell which player triggered them.
               PlayerIndex playerIndex;

               if (input.IsMenuSelect(ControllingPlayer, out playerIndex))
               {
                    OnSelectEntry(selectedEntry, playerIndex);
               }

               else if (input.IsMenuCancel(ControllingPlayer, out playerIndex))
               {
                    OnCancel(playerIndex);
               }
          }

          /// <summary>
          /// Handler for when the user has chosen a menu entry.
          /// </summary>
          protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
          {
               if (entryIndex >= menuEntries.Count)
                    return;

               else
               {
                    menuEntries[selectedEntry].OnSelectEntry(playerIndex);
                    AudioManager.PlayCue(buttonPressSound);
               }
          }

          /// <summary>
          /// Handler for when the user has cancelled the menu.
          /// </summary>
          protected virtual void OnCancel(PlayerIndex playerIndex)
          {
               ExitScreen();
          }

          /// <summary>
          /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
          /// </summary>
          protected void OnCancel(object sender, PlayerIndexEventArgs e)
          {
               OnCancel(e.PlayerIndex);
          }

          #endregion

          #region Update

          /// <summary>
          /// Updates the Menu.
          /// </summary>
          public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                         bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               // Update each nested MenuEntry object.
               for (int i = 0; i < menuEntries.Count; i++)
               {
                    bool isSelected = IsActive && (i == selectedEntry);

                    menuEntries[i].Update(this, isSelected, gameTime);
               }
          }

          #endregion

          #region Draw

          /// <summary>
          /// Draws the Menu.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
               SpriteFont font = ScreenManager.Font;

               Vector2 position = new Vector2(0, 150);

               // Make the menu slide into place during transitions, using a
               // power curve to make things look more interesting (this makes
               // the movement slow down as it nears the end).
               float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

               if (ScreenState == ScreenState.TransitionOn)
                    position.X -= transitionOffset * 256;
               else
                    position.X += transitionOffset * 512;

               // New
               //This will get the # of rows we want in each column
               int maxRowsPerColumn =
                    (int)Math.Ceiling(menuEntries.Count / (float)numberOfColumns);

               // End

               spriteBatch.Begin();

               // Draw each menu entry in turn.
               for (int i = 0; i < menuEntries.Count; i++)
               {
                    MenuEntry menuEntry = menuEntries[i];

                    bool isSelected = IsActive && (i == selectedEntry);

                    menuEntry.Draw(this, new Vector2(position.X + menuEntries[i].Position.X,
                             position.Y + menuEntries[i].Position.Y), isSelected, gameTime);

                    position.Y += menuEntry.VerticalSpacing;

                    // We need to handle an odd # of menu entries different
                    // from an even # of entries so we mod to figure out
                    // if we need to start a new column.
                    if (maxRowsPerColumn % 2 == 2) // Even
                    {
                         if (i % maxRowsPerColumn == maxRowsPerColumn)
                         {
                              // We use 'hard' values in here to keep the rows and columns
                              // lined up perfectly. These will vary greatly from project to
                              // project based on what you want to do so make sure to play
                              // with them to get a result you like.
                              position.X += 485;

                              // This variable should be the same as the 'position' value
                              // set at the top of this method. It looks like this (for me)
                              // so make sure to match the Y value:
                              // Vector2 position = new Vector2(100, 150);
                              position.Y = 150;
                              //position.Y += menuEntry.VerticalSpacing;
                         }
                    }

                    else // Odd
                    {
                         if (i % maxRowsPerColumn == maxRowsPerColumn - 1)
                         {
                              // Same comments from above apply here.
                              position.X += 485;
                              position.Y = 150;
                         }
                    }
               }

               // Draw the menu title.
               Vector2 titlePosition = menuTitlePosition;
               Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
               Color titleColor = menuTitleColor * (TransitionAlpha / 255f);

               titlePosition.Y -= transitionOffset * menuTitleTransitionSpeed;

               TextManager.DrawCentered(false, font, menuTitle, titlePosition, titleColor, titleFontScale);

               spriteBatch.End();
          }

          #endregion
     }
}
