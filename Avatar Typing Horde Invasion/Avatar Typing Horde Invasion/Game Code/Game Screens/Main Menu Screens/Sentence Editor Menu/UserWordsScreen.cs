#region File Description
//-----------------------------------------------------------------------------
// UserWordsScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
using PixelEngine.Text;
using PixelEngine.Menu;
#endregion

namespace AvatarTyping
{
     /// <remarks>
     /// A PagedMenuScreen which displays all the custom sentences
     /// the player has created for the game.
     /// </remarks>
     class UserWordsScreen : PagedMenuScreen
     {
          #region Fields

          private GameResourceTexture2D gradientTexture;

          private static IAsyncResult result;
          private bool editWordRequest;
          private string resultString = String.Empty;
          private int menuEntryToEdit = 0;

          #endregion

          #region Initialization

          public UserWordsScreen()
               : base("", 100, 5)
          {
               for (int i = 0; i < 100; i++)
               {
                    MenuEntries[i].Text = "Sentence # " + (i + 1);
                    MenuEntries[i].Description = "Modify Sentence # " + (i + 1);
                    MenuEntries[i].Selected += MenuEntrySelected;
                    MenuEntries[i].ShowGradientBorder = false;
                    MenuEntries[i].ShowBorder = false;
                    MenuEntries[i].FontScale = MenuEntries[i].FontScale * 0.90f;

                    MenuEntries[i].SelectedColor = Color.DarkOrange;
               }

               SetMenuText();
          }

          /// <summary>
          /// Load this screen's assets.
          /// </summary>
          public override void LoadContent()
          {
               base.LoadContent();

               gradientTexture = ResourceManager.LoadTexture(@"Textures\Gradients\Gradient_BlackToWhite7");
          }

          #endregion

          #region Menu Entry Selected Events

          /// <summary>
          /// Set the Text property for each MenuEntry.
          /// 
          /// For this screen, we want to MenuEntry Text to be the
          /// user's custom sentences.
          /// </summary>
          void SetMenuText()
          {
               for (int i = 0; i < SentenceDatabase.UserSentences.Count; i++)
               {
                    if (i >= 100)
                         break;

                    MenuEntries[i].Text =
                         TextManager.WrapText(SentenceDatabase.UserSentences[i], 550); // 500
               }
          }

          /// <summary>
          /// The Event triggered when any MenuEntry is Selected.
          /// </summary>
          void MenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               if (this.SelectedMenuEntry >= SentenceDatabase.UserSentences.Count)
               {
                    return;
               }

               if (!Guide.IsVisible)
               {
                    menuEntryToEdit = this.SelectedMenuEntry;

                    result = Guide.BeginShowKeyboardInput(ControllingPlayer.Value, 
                         "Sentence Creation - Edit A Sentence", 
                         "Enter New Custom Sentence - At Least 4 Characters in Size",
                         MenuEntries[menuEntryToEdit].Text, null, null);
                    
                    editWordRequest = true;
               }
          }

          #endregion

          #region Handle Input

          public override void HandleInput(InputState input)
          {
               base.HandleInput(input);
          }

          #endregion

          #region Update

          /// <summary>
          /// Overridden Screen Update.
          /// 
          /// This Screen updates the MenuEntry Text if a request
          /// to edit a word has been made.
          /// </summary>
          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               if (editWordRequest)
               {
                    string stringResult = String.Empty;

                    if (result.IsCompleted)
                    {
                         editWordRequest = false;

                         stringResult = Guide.EndShowKeyboardInput(result);

                         if (String.IsNullOrEmpty(stringResult) || stringResult.Length < 4)
                         {
                              resultString = "Your sentence must contain\nmore than 4 characters.";
                              return;
                         }

                         if (stringResult.Length > 80)
                         {
                              resultString = "Your sentence must contain no more than 80 characters.";
                              return;
                         }

                         // If stringResult contains an unsupported character...
                         foreach (char character in stringResult)
                         {
                              if (!ScreenManager.Font.Characters.Contains(character))
                              {
                                   resultString = "Your sentence contains\nunsupported characters.\n";
                                   return;
                              }
                         }

                         resultString = "Your sentence was successfully\nadded to the game.";

                         if (menuEntryToEdit < 100)
                         {
                              SentenceDatabase.UserSentences[menuEntryToEdit] = stringResult;

                              AvatarTypingGame.SaveGameData.WordList = SentenceDatabase.UserSentences;

                              SetMenuText();
                         }
                    }
               }

               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
          }

          #endregion

          #region Draw

          public override void Draw(GameTime gameTime)
          {
               ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 3 / 5);

               SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
               SpriteFont font = ScreenManager.Font;

              // spriteBatch.GraphicsDevice.Clear(Color.Blue);

               spriteBatch.Begin();
               //spriteBatch.Draw(gradientTexture.Texture2D, new Rectangle(0, 0, 1280, 720), Color.CornflowerBlue);
               //TextManager.DrawCentered(true, ScreenManager.Font, resultString, PixelEngine.EngineCore.ScreenCenter, Color.White, 1.0f);
               spriteBatch.End();

               base.Draw(gameTime);
          }

          #endregion
     }
}