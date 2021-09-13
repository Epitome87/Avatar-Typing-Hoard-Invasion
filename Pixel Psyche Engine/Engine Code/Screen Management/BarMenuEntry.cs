#region File Description
//-----------------------------------------------------------------------------
// BarMenuEntry.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.Screen;
#endregion

namespace PixelEngine.Menu
{
     /// <summary>
     /// Helper class represents a single "bar" entry in a MenuScreen. By default this
     /// just draws the entry text string along with "bars". This also provides an event 
     /// that will be raised when the menu entry is selected. This MenuEntry type is useful
     /// for Volume Sliders, like those found in an Options Menu.
     /// </summary>
     public class BarMenuEntry : MenuEntry
     {
          #region Fields

          private int numberOfBars;
          private int currentBar;
          private float value;
          private Color barSelectedColor;
          private Color barUnselectedColor;

          #endregion

          #region Properties

          /// <summary>
          /// The number of Bars used to represent this Menu Entry.
          /// </summary>
          public int NumberOfBars
          {
               get { return numberOfBars; }
               set { numberOfBars = value; }
          }

          /// <summary>
          /// The Current Bar selected.
          /// </summary>
          public int CurrentBar
          {
               get { return currentBar; }
               set { currentBar = value; }
          }

          /// <summary>
          /// The Value the selection represents.
          /// </summary>
          public float Value
          {
               get { return (float)CurrentBar / (float)NumberOfBars; }
               set { this.value = value; }
          }

          /// <summary>
          /// The Color which Selected / used bars appear as.
          /// </summary>
          public Color BarSelectedColor
          {
               get { return barSelectedColor; }
               set { barSelectedColor = value; }
          }

          /// <summary>
          /// The Color which Unselected / unused bars appear as.
          /// </summary>
          public Color BarUnselectedColor
          {
               get { return barUnselectedColor; }
               set { barUnselectedColor = value; }
          }

          #endregion

          #region Events

          /// <summary>
          /// Event raised when the menu entry is selected.
          /// </summary>
          public new event EventHandler<PlayerIndexEventArgs> Selected;

          /// <summary>
          /// Method for raising the Selected event.
          /// </summary>
          protected override internal void OnSelectEntry(PlayerIndex playerIndex)
          {
               this.currentBar = (++this.currentBar) % (numberOfBars + 1);
               base.OnSelectEntry(playerIndex);

               if (Selected != null)
                    Selected(this, new PlayerIndexEventArgs(playerIndex));
          }

          #endregion

          #region Initialization

          /// <summary>
          /// Constructs a new Menu Entry with the specified Text.
          /// </summary>
          public BarMenuEntry(string text) 
               : base(text)
          {
               numberOfBars = 10;
               currentBar = 5;
               barSelectedColor = Color.White * (222f / 250f);
               barUnselectedColor = Color.White * (50f / 250f); 
          }

          /// <summary>
          /// Constructs a new Menu Entry with the specified Text and Description.
          /// </summary>
          public BarMenuEntry(string text, string description)
               : base(text, description)
          {
               numberOfBars = 10;
               currentBar = 5;
               barSelectedColor = Color.White * (222f / 250f);
               barUnselectedColor = Color.White * (50f / 250f); 
          }

          #endregion

          #region Update

          public override void Update(MenuScreen screen, bool isSelected, Microsoft.Xna.Framework.GameTime gameTime)
          {
               base.Update(screen, isSelected, gameTime);
          }

          #endregion

          #region Draw

          public override void Draw(MenuScreen screen, Vector2 position, bool isSelected, GameTime gameTime)
          {
               // The regular MenuEntry rendering is done by calling MenuEntry.Draw().
               base.Draw(screen, position, isSelected, gameTime);

               // Draw text, centered on the middle of each line.
               SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
               
               SpriteFont font = ScreenManager.Font;

               Rectangle barPosition = 
                    new Rectangle((int)position.X - 100 + (int)font.MeasureString(this.Text).X / 2,
                              (int)position.Y - font.LineSpacing / 2 + (font.LineSpacing) - (font.LineSpacing / numberOfBars),
                              5, font.LineSpacing / (numberOfBars) );

               // Draw the Bars.
               for (int i = 0; i < numberOfBars; i++)
               {
                    // Determine the bar color based on whether it's selected or not.
                    Color barColor = (i < currentBar)
                         ? barSelectedColor : barUnselectedColor;

                    barPosition.X += 10;// 15;
                    barPosition.Height += font.LineSpacing / numberOfBars;
                    barPosition.Y = (int)(position.Y - font.LineSpacing / 2) + 
                                      (int)(font.LineSpacing - barPosition.Height) - 5; 
                    
                    spriteBatch.Draw(blankTexture.Texture2D, barPosition, null, barColor, 0.0f, new Vector2(), SpriteEffects.FlipVertically, 0.0f);
               }
          }

          #endregion
     }
}