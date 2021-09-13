#region File Description
//-----------------------------------------------------------------------------
// PagedMenuScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PixelEngine.Audio;
using PixelEngine.Menu;
using PixelEngine.ResourceManagement;
using PixelEngine.Text;
using PixelEngine.Graphics;
#endregion

namespace PixelEngine.Screen
{
     /// <remarks>
     /// Inherits from MenuScreen to add functionality specific to a menu screen
     /// with multiple pages. Features such as the ability to flip between pages and
     /// rendering of the current page number / total page number are provided.
     /// 
     /// NOTE: Unlike MenuScreen, menu entries are created automatically in
     /// the constructor. The amount created is the product of ItemsPerPage and
     /// NumberOfPages.
     /// </remarks>
     public abstract class PagedMenuScreen : MenuScreen
     {
          #region Fields

          ContentManager content;

          private Vector2 textPosition = new Vector2(EngineCore.ScreenCenter.X, 0);

          protected const int MAX_PAGE_NUMBER = 50;
          protected const int MAX_PAGE_SIZE = 20;
          protected const int MAX_MENU_AMOUNT = 1000;

          protected uint currentPageNumber = 1;
          protected uint itemsPerPage;
          protected uint numberOfItems;
          protected uint numberOfPages;
          protected Color pageNumberColor = Color.White;
          protected Color pageBorderColor = Color.Black;

          protected bool showPageBorder = true;
          protected bool showPageNumber = true;
          protected float pageNumberFontScale = 1.0f;
          protected Vector2 pageNumberPosition = new Vector2(EngineCore.ScreenCenter.X, EngineCore.GraphicsInformation.ScreenHeight - 95);

          protected GameResourceTexture2D blankTexture;

          #endregion

          #region Properties

          /// <summary>
          /// Gets or sets the current Page being viewed.
          /// </summary>
          public uint CurrentPageNumber
          {
               get { return currentPageNumber; }
               set { currentPageNumber = value; }
          }

          /// <summary>
          /// Gets or sets the number of items displayed per page.
          /// </summary>
          public uint ItemsPerPage
          {
               get { return itemsPerPage; }
               set { itemsPerPage = value; }
          }

          /// <summary>
          /// Gets or sets the total number of items in this Menu Screen.
          /// </summary>
          public uint NumberOfItems
          {
               get { return numberOfItems; }
               set { numberOfItems = value; }
          }

          /// <summary>
          /// Gets or sets the number of Pages in this Menu Screen.
          /// </summary>
          public uint NumberOfPages
          {
               get { return numberOfPages; }
               set { numberOfPages = value; }
          }

          /// <summary>
          /// Gets or sets the Color the Page Number text is rendered in.
          /// Default: Color.White
          /// </summary>
          public Color PageNumberColor
          {
               get { return pageNumberColor; }
               set { pageNumberColor = value; }
          }

          /// <summary>
          /// Gets or sets the Color the Page Border the text is rendered within.
          /// Default: Color.Gray
          /// </summary>
          public Color PageBorderColor
          {
               get { return pageBorderColor; }
               set { pageBorderColor = value; }
          }

          /// <summary>
          /// Gets or sets whether or not to render the Page Number.
          /// Default: True.
          /// </summary>
          public bool ShowPageNumber
          {
               get { return showPageNumber; }
               set { showPageNumber = value; }
          }

          /// <summary>
          /// Gets or sets whether or not to render the border 
          /// surrounding the Page Number.
          /// Default: True.
          /// </summary>
          public bool ShowPageBorder
          {
               get { return showPageBorder; }
               set { showPageBorder = value; }
          }

          /// <summary>
          /// Gets or sets the Scale of the Font the Page Number is rendered in.
          /// Default: 1.0f
          /// </summary>
          public float PageNumberFontScale
          {
               get { return pageNumberFontScale; }
               set { pageNumberFontScale = value; }
          }

          /// <summary>
          /// Gets or sets the Position the Page Number is rendered at.
          /// Default: X: Center, Y: 100 from bottom.
          /// </summary>
          public Vector2 PageNumberPosition
          {
               get { return pageNumberPosition; }
               set { pageNumberPosition = value; }
          }

          #endregion

          #region Initialization

          /// <summary>Constructor.</summary>
          /// <param name="menuTitle">Title / header of the menu screen.</param>
          /// <param name="numPages">Number of pages for the menu screen.</param>
          /// <param name="itemsOnPage">Number of menu entries per page.</param>
          public PagedMenuScreen(string menuTitle, uint numOfItems, uint itemsOnPage)
               : base(menuTitle)
          {
               numberOfPages = numOfItems % itemsOnPage == 0 ? 
                    (numOfItems / itemsOnPage) : (numOfItems / itemsOnPage) + 1;
               itemsPerPage = itemsOnPage;
               numberOfItems = numOfItems;

               // Create all the Menu Entries for this Menu.
               for (int i = 0; i < (numOfItems); i++)
               {
                    // Stop creating new Menu Entries if we have reached the
                    // pre-determined limit.
                    if (i >= MAX_MENU_AMOUNT)
                         break;

                    // Create our Menu Entry.
                    MenuEntry tempMenuEntry = new MenuEntry(string.Empty);

                    // Hook up menu event handlers.

                    // Set the Color of the Menu Entry when "Selected".
                    tempMenuEntry.SelectedColor = Color.Red;

                    tempMenuEntry.IsCenter = false;

                    // Add Entry to the Menu.
                    MenuEntries.Add(tempMenuEntry);
               }
          }

          public override void LoadContent()
          {
               base.LoadContent();

               if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

               // Blank texture used to highlight Selected Entries.
               blankTexture = ResourceManager.LoadTexture(@"Textures\Blank Textures\blank");


               iconTex = ResourceManager.LoadTexture(@"Buttons\LeftArrow");
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

                    // For Page 1.
                    if (selectedEntry < 0)
                         selectedEntry = (int)( (currentPageNumber - 1) * itemsPerPage + itemsPerPage - 1 );

                    // For other Pages.
                    if (selectedEntry < ((currentPageNumber - 1) * itemsPerPage))
                    {
                         selectedEntry = (int)( ((currentPageNumber - 1) * itemsPerPage) + itemsPerPage - 1 );

                         if (selectedEntry > numberOfItems)
                              selectedEntry = (int)numberOfItems - 1;
                    }

                    // Only play the Scroll Sound if there's more than one Menu Entry.
                    if (menuEntries.Count != 1)
                         AudioManager.PlayCue(menuScrollSound);
               }

               // Move to the next menu entry?
               if (input.IsMenuDown(ControllingPlayer))
               {
                    selectedEntry++;

                    if (selectedEntry >= menuEntries.Count)
                         selectedEntry = (int)( (currentPageNumber - 1) * itemsPerPage );

                    if (selectedEntry >= (int)((currentPageNumber - 1) * itemsPerPage) + itemsPerPage)
                         selectedEntry = (int)((currentPageNumber - 1) * itemsPerPage);

                    // Only play the Scroll Sound if there's more than one Menu Entry.
                    if (menuEntries.Count != 1)
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

               if (input.IsNewButtonPress(Buttons.RightShoulder, ControllingPlayer, out playerIndex) ||
                   input.IsNewKeyPress(Keys.Right, null, out playerIndex))
               {
                    OnNextPage();
               }

               if (input.IsNewButtonPress(Buttons.LeftShoulder, ControllingPlayer, out playerIndex) ||
                   input.IsNewKeyPress(Keys.Left, null, out playerIndex))
               {
                    OnPreviousPage();
               }
          }

          #endregion

          #region Paged Menu Screen Events (OnNextPage and OnPreviousPage)

          protected virtual void OnNextPage()
          {
               currentPageNumber = (uint)MathHelper.Clamp(++currentPageNumber, 1, numberOfPages);

               selectedEntry = (selectedEntry + itemsPerPage) > MenuEntries.Count - 1 ?
                    MenuEntries.Count - 1 : (selectedEntry + (int)itemsPerPage);
          }

          protected virtual void OnPreviousPage()
          {
               currentPageNumber = (uint)MathHelper.Clamp(--currentPageNumber, 1, numberOfPages);

               selectedEntry = (selectedEntry - itemsPerPage) < 0 ?
                    0 : (selectedEntry - (int)itemsPerPage);
          }

          #endregion

          #region Update

          bool showControllerIcons = true;
          float elapsedIconUpdate = 0.0f;
          float iconUpdateInterval = 5000.0f;
          GameResourceTexture2D iconTex;

          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               // NEW FOR ALTERNATING BUTTONS, 5-31-2011:
               elapsedIconUpdate += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
               if (elapsedIconUpdate >= iconUpdateInterval)
               {
                    elapsedIconUpdate = 0.0f;
                    showControllerIcons = !showControllerIcons;
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

               spriteBatch.Begin();

               // Draw each menu entry in turn.
               for (int i = (((int)currentPageNumber - 1) * (int)itemsPerPage); i < ((currentPageNumber - 1) * itemsPerPage) + itemsPerPage; i++)
               {
                    if (i >= numberOfItems)
                         break;
                    
                    MenuEntry menuEntry = MenuEntries[i];
                    
                    bool isSelected = IsActive && (i == selectedEntry);
                    
                    menuEntry.Draw(this, new Vector2(position.X + MenuEntries[i].Position.X,
                                  position.Y + MenuEntries[i].Position.Y), isSelected, gameTime);
                    
                    position.Y += menuEntry.VerticalSpacing;//EngineCore.ResolutionScale * menuEntry.GetTrueHeight() * menuEntry.FontScale;
               }

               // Draw the menu title.
               Vector2 titlePosition = new Vector2(EngineCore.ScreenCenter.X, 100);
               Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
               Color titleColor = menuTitleColor * (TransitionAlpha / 255f);

               titlePosition.Y -= transitionOffset * 100;

               TextManager.DrawCentered(false, font, menuTitle, titlePosition, titleColor, titleFontScale);

               // If we're showing the border around the Page information...
               if (showPageBorder)
               {
                    // Draw the box.
                    spriteBatch.Draw(blankTexture.Texture2D, new Rectangle(0, (int)pageNumberPosition.Y - (int)(EngineCore.ResolutionScale * pageNumberFontScale * font.LineSpacing / 2),
                         EngineCore.GraphicsInformation.ScreenWidth, (int)(EngineCore.ResolutionScale * font.LineSpacing * pageNumberFontScale)),
                         pageBorderColor);
               }

               // If we're showing the Page information...   
               if (ShowPageNumber)
               {
                    // Render the current page number.
                    TextManager.DrawCentered(false, ScreenManager.Font, "Page " + currentPageNumber.ToString() + " / " +
                         numberOfPages.ToString(), pageNumberPosition, pageNumberColor, PageNumberFontScale);


                    TextManager.DrawCentered(false, ScreenManager.Font, "Previous", new Vector2(pageNumberPosition.X - 350, pageNumberPosition.Y),
                   pageNumberColor, pageNumberFontScale);

                    TextManager.DrawCentered(false, ScreenManager.Font, "Next", new Vector2(pageNumberPosition.X + 350, pageNumberPosition.Y),
                         pageNumberColor, pageNumberFontScale);


                    string pageString = "Page 50 / 50";// +currentPageNumber.ToString() + " / " + numberOfPages.ToString();
                    Vector2 origin = new Vector2((font.MeasureString(pageString).X) / 2, (font.LineSpacing) / 2);
                    int lineSize = (int)font.MeasureString(pageString).X + 100;
                    int heightSize = (int)(font.LineSpacing * 0.90);

                    // NEW IF FOR 5-31-2011 ICON ALTERNATING:
                    if (showControllerIcons)
                    {
                         origin = new Vector2(MySpriteBatch.Measure360Button(Buttons.RightShoulder).X / 2,
                              MySpriteBatch.ButtonSpriteFont.LineSpacing / 2);


                         MySpriteBatch.Draw360Button(Buttons.RightShoulder, new Vector2(pageNumberPosition.X - (lineSize / 2), pageNumberPosition.Y),
                              Color.White, 0.0f, new Vector2(origin.X, origin.Y), 0.4f, SpriteEffects.None, 0f);


                         origin = new Vector2(MySpriteBatch.Measure360Button(Buttons.LeftShoulder).X / 2,
                              MySpriteBatch.ButtonSpriteFont.LineSpacing / 2);

                         MySpriteBatch.Draw360Button(Buttons.LeftShoulder, new Vector2(pageNumberPosition.X + (lineSize / 2), pageNumberPosition.Y),
                              Color.White, 0.0f, new Vector2(origin.X, origin.Y), 0.4f, SpriteEffects.None, 0f);
                    }

                    else
                    {
                         origin = new Vector2(iconTex.Texture2D.Width / 2, iconTex.Texture2D.Height / 2);

                         MySpriteBatch.Draw(iconTex.Texture2D, new Vector2(pageNumberPosition.X - (lineSize / 2), pageNumberPosition.Y), null,
                              Color.White, 0.0f, new Vector2(origin.X, origin.Y), 0.3f, SpriteEffects.None, 0f);

                         MySpriteBatch.Draw(iconTex.Texture2D, new Vector2(pageNumberPosition.X + (lineSize / 2), pageNumberPosition.Y), null,
                              Color.White, 0.0f, new Vector2(origin.X, origin.Y), 0.3f, SpriteEffects.FlipHorizontally, 0f);
                    }
               
               }

               spriteBatch.End();
          }

          #endregion
     }
}