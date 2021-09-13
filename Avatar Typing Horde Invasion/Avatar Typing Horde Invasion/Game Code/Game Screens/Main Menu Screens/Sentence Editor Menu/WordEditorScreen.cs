#region File Description
//-----------------------------------------------------------------------------
// WordEditorScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using PixelEngine.AchievementSystem;
using PixelEngine.Menu;
using PixelEngine.Screen;
using PixelEngine.Text;
using PixelEngine.Storage;
using PixelEngine;
#endregion

namespace AvatarTyping
{
     public class WordEditorScreen : MenuScreen
     {
          #region Fields

          private static IAsyncResult result;
          private bool createWordRequest = false;

          private bool saveRequested = false;


          //public static List<TypingWord> wordList = new List<TypingWord>();
          private string resultString = String.Empty;

          MenuEntry createWordMenuEntry;
          MenuEntry viewWordMenuEntry;
          MenuEntry backMenuEntry;

          #endregion

          #region Initialization

          public WordEditorScreen()
               : base("Sentence Editor")
          {
               // Create our menu entries.
               createWordMenuEntry = new MenuEntry("Create A Sentence!", "Create A Custom Sentence.\nThis Sentence Will Appear During Gameplay!");
               viewWordMenuEntry = new MenuEntry("View Your Sentences!", "View All Of The Custom \nSentences You Have Created!");
               backMenuEntry = new MenuEntry("Back", "Return to the Main Menu.\nSentences Will Be Saved Automatically.");

               createWordMenuEntry.Selected += CreateWordMenuEntrySelected;
               viewWordMenuEntry.Selected += ViewWordMenuEntrySelected;
               backMenuEntry.Selected += OnCancel;

               MenuEntries.Add(createWordMenuEntry);
               MenuEntries.Add(viewWordMenuEntry);
               MenuEntries.Add(backMenuEntry);

               foreach (MenuEntry entry in MenuEntries)
               {
                    entry.AdditionalVerticalSpacing = 20;
                    entry.Position = new Vector2(entry.Position.X, entry.Position.Y + 25);
                    entry.IsPulsating = false;
               }

               backMenuEntry.Position = new Vector2(backMenuEntry.Position.X, backMenuEntry.Position.Y + 150);
               backMenuEntry.IsPulsating = true;
          }

          #endregion

          #region Menu Events

          void CreateWordMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               if (SentenceDatabase.UserSentences.Count >= 100)
               {
                    return;
               }

               if (!Guide.IsVisible)
               {
                    result = Guide.BeginShowKeyboardInput(ControllingPlayer.Value, 
                         "Sentence Creation - Add A New Sentence", 
                         "Enter Custom Sentence - At Least 4 Characters in Size",
                         "", null, null);
                    
                    createWordRequest = true;
               }
          }

          void ViewWordMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.AddScreen(new UserWordsScreen(), AvatarTypingGame.CurrentPlayer.GamerInfo.PlayerIndex);
          }

          protected override void OnCancel(PlayerIndex playerIndex)
          {
#if XBOX
               //if (StorageManager.SaveRequested == false)
               if (saveRequested == false)
               {
                    //StorageManager.SaveRequested = true;
                    saveRequested = true;
               }
#endif
          }

          #endregion

          #region Update

          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               if (SentenceDatabase.UserSentences.Count >= 100)
               {
                    createWordMenuEntry.SelectedColor = Color.Gray;
                    createWordMenuEntry.UnselectedColor = Color.Gray;
                    createWordMenuEntry.Description =
                         "You Have Already Created 100 Sentences.\n" +
                         "You Can Still Modify Existing Sentences\n" +
                         "By Selecting 'View Your Sentences'.";
               }

               if (createWordRequest)
               {
                    string stringResult = String.Empty;

                    if (result.IsCompleted)
                    {
                         createWordRequest = false;

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

                         SentenceDatabase.UserSentences.Add(stringResult);

                         AvatarTypingGame.SaveGameData.WordList = SentenceDatabase.UserSentences;
                    }
               }

               if (saveRequested)
               {
                    StorageDevice device = StorageManager.Device;

                    AvatarTypingGame.SaveGame(device);

                    // Reset the request flag
                    saveRequested = false;

                    base.OnCancel(ControllingPlayer.Value);
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

               spriteBatch.Begin();

               TextManager.DrawCentered(true, ScreenManager.Font, resultString, PixelEngine.EngineCore.ScreenCenter, Color.White, 1.0f);

               spriteBatch.End();

               base.Draw(gameTime);
          }

          #endregion
     }
}