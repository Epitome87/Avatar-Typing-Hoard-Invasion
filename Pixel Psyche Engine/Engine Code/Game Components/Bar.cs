
#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace PixelEngine
{
     /// <summary>
     /// This class represents a bar object
     /// that can be customised by its public
     /// properties.
     /// 
     /// Example applications are:
     ///     # Health
     ///     # Shield
     ///     # Speed
     /// </summary>
     /// <Author>Neil Reed</Author>
     /// <Email>NeoNeil@Hotmail.com</Email>
     /// <Version>1.0</Version>
     /// <Date>05/02/2010</Date>
     public class Bar
     {
          #region Fields

          // Generic Fields.
          private Vector2 position = Vector2.Zero;
          private bool visible = true;
          private UpdateType autoUpdateType = UpdateType.None;
          private float updateInterval = 1000.0f;

          // Bar-specific Fields.
          private int _Value = 0;
          private Color color = Color.Red;
          private Color borderColor = Color.Black;
          private Color emptyColor = Color.Gray;
          private int maxValue = 100;

          private Texture2D barImage = null;
          private Texture2D borderImage = null;
          private int width = 100;
          private int height = 20;
          private int borderOffSet = 5;
          private bool vertical = false;

          #region Only used for drawing bar

          private Rectangle recBorder;
          private Rectangle recBar;
          private int amountToDraw;
          private Rectangle recFill;
          private Vector2 barPosition;

          #endregion

          #endregion

          #region Properties

          /// <summary>
          /// If the object is visable (if false the object wont be drawn)
          /// </summary>
          public bool Visible
          {
               get { return visible; }
               set { visible = value; }
          }

          /// <summary>
          /// Enumerated update types
          /// for use with AutoUpdateType
          /// </summary>
          public enum UpdateType
          {
               None = 0,
               Increment = 1,
               Decrement = 2,
          }

          /// <summary>
          /// Use this to set if on the update call
          /// the object should increment or decrement or none
          /// </summary>
          public UpdateType AutoUpdateType
          {
               get { return autoUpdateType; }
               set { autoUpdateType = value; }
          }

          public float elapsedTime = 0f;

          /// <summary>
          /// The value of the bar, it will not
          /// exceed the Max or Min values unless
          /// they are set to null
          /// </summary>
          public int Value
          {
               get { return _Value; }
               set
               {
                    if ((value <= MaxValue) & (value >= 0))
                    {
                         //Set the value
                         _Value = value;
                         WorkOutRectangles();

                         //Check for Max value, if so fire the event
                         if (value == MaxValue)
                         {
                              if (this.AtMaxValue != null)
                              {
                                   this.AtMaxValue(this, new EventArgs());
                              }
                         }
                         // Check if value is at min value, if so fire the event
                         if (value == 0)
                         {
                              if (this.AtMinValue != null)
                              {
                                   this.AtMinValue(this, new EventArgs());
                              }
                         }
                    }
               }
          }

          /// <summary>
          /// The color for the bar
          /// </summary>
          public Color Color
          {
               get { return color; }
               set { color = value; }

          }

          /// <summary>
          /// The max value for the bar
          /// by default this is 100
          /// </summary>
          public int MaxValue
          {
               get { return maxValue; }
               set { maxValue = value; }
          }

          public float UpdateInterval
          {
               get { return updateInterval; }
               set { updateInterval = value; }
          }

          public int Height
          {
               get { return height; }
               set
               {
                    height = value;
                    WorkOutRectangles();
               }
          }

          public int Width
          {
               get { return width; }
               set
               {
                    width = value;
                    WorkOutRectangles();
               }
          }

          public Vector2 Position
          {
               get { return Position; }
               set
               {
                    Position = value;
                    WorkOutRectangles();
               }
          }

          /// <summary>
          ///  Sets the border thickness in pixels
          ///  Default value is 20, set to 0 for no border
          /// </summary>
          public int BorderOffSet
          {
               get { return borderOffSet; }
               set
               {
                    borderOffSet = value;
                    WorkOutRectangles();
               }
          }

          /// <summary>
          /// The border colour
          /// Default is black
          /// </summary>
          public Color BorderColour
          {
               get { return borderColor; }
               set { borderColor = value; }
          }

          /// <summary>
          /// This is the colour shown when the bar
          /// is empty.
          /// 
          /// The default is gray
          /// </summary>
          public Color EmptyColour
          {
               get { return emptyColor; }
               set { emptyColor = value; }
          }

          /// <summary>
          /// Allows you to specify your own image.
          /// Note: if image supplied border will
          /// no longer be drawn and width and height
          /// will be taken from the image properties.
          /// </summary>
          public Texture2D BarImage
          {
               get { return barImage; }
               set
               {
                    if (value != null)
                    {
                         Width = value.Width;
                         Height = value.Height;
                         WorkOutRectangles();
                    }
               }
          }

          /// <summary>
          /// If true the bar is drawn verticaly (top to bottom)
          /// Default is false
          /// </summary>
          public bool Vertical
          {
               get { return vertical; }
               set
               {

                    vertical = value;
                    WorkOutRectangles();


               }
          }

          public Bar(GraphicsDevice device)
          {
               borderImage = new Texture2D(device, 1, 1);
               Color[] blank = { Color.White };
               borderImage.SetData<Color>(blank);
               WorkOutRectangles();
          }

          #endregion

          #region Overrideable Events

          /// <summary>
          /// Fires when the object has reached the Min value
          /// </summary>
          public event EventHandler AtMinValue;

          /// <summary>
          /// Fires when the object has reached the Max value
          /// </summary>
          public event EventHandler AtMaxValue;

          #endregion

          #region Helper Methods

          private void WorkOutRectangles()
          {
               //Update the properties used for draw the bar
               //Calculations done here to reduce overheads at draw
               if (BarImage == null)
               {
                    recBar = new Rectangle(0, 0, Width - (BorderOffSet * 2), Height - (BorderOffSet * 2));

                    if (Vertical)
                    {
                         amountToDraw = (int)(recBar.Height * (float)Value / MaxValue);
                         recFill = new Rectangle(0, 0, Width - (BorderOffSet * 2), amountToDraw);
                    }

                    else
                    {
                         amountToDraw = (int)(recBar.Width * (float)Value / MaxValue);
                         recFill = new Rectangle(0, 0, amountToDraw, Height - (BorderOffSet * 2));
                    }

                    recBorder = new Rectangle(0, 0, Width, Height);
                    barPosition = new Vector2(Position.X + BorderOffSet, Position.Y + BorderOffSet);
               }

               else
               {
                    recBar = new Rectangle(0, 0, Width, Height);

                    if (Vertical)
                    {
                         amountToDraw = (int)(recBar.Height * (float)Value / MaxValue);
                         recFill = new Rectangle(0, 0, Width, amountToDraw);
                    }

                    else
                    {
                         amountToDraw = (int)(recBar.Width * (float)Value / MaxValue);
                         recFill = new Rectangle(0, 0, amountToDraw, Height);
                    }
               }

          }

          #endregion

          #region Public Methods

          /// <summary>
          /// Draws the bar only if the visable property is set to
          /// true.
          /// </summary>
          /// <param name="sb">Spritebatch, must be already set to begin</param>
          public void Draw(SpriteBatch spriteBatch)
          {
               if (Visible)
               {
                    if (BarImage == null)
                    {
                         spriteBatch.Draw(borderImage, Position, recBorder, BorderColour);
                         spriteBatch.Draw(borderImage, barPosition, recBar, EmptyColour);
                         spriteBatch.Draw(borderImage, barPosition, recFill, Color);
                    }

                    else
                    {
                         spriteBatch.Draw(BarImage, Position, recFill, Color);
                    }
               }
          }

          public void Update(GameTime gameTime)
          {
               elapsedTime += gameTime.ElapsedGameTime.Milliseconds;

               if (elapsedTime > UpdateInterval)
               {

                    switch (AutoUpdateType)
                    {
                         case UpdateType.None:
                              {
                                   break;
                              }

                         case UpdateType.Increment:
                              {
                                   Value++;
                                   break;
                              }

                         case UpdateType.Decrement:
                              {
                                   Value--;
                                   break;
                              }
                    }

                    elapsedTime -= UpdateInterval;
               }
          }

          #endregion
     }
}