#region File Description
//-----------------------------------------------------------------------------
// ControlsMenuScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine;
using PixelEngine.Graphics;
using PixelEngine.Menu;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
using PixelEngine.Text;
#endregion

namespace AvatarTyping
{
     /// <remarks>
     /// A Menu screen that displays the Controls.
     /// </remarks>
     public class ControlsMenuScreen : MenuScreen
     {
          #region Fields

          MenuEntry otherControlsMenuEntry;
          MenuEntry backMenuEntry;

          GameResourceTexture2D controlsTexture;
          GameResourceTexture2D keyboardTexture;

          bool showKeyboardControls = false;

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public ControlsMenuScreen()
               : base("C O N T R O L S")
          {
               this.TransitionOnTime = TimeSpan.FromSeconds(1.5f);
               this.TransitionOffTime = TimeSpan.FromSeconds(1.0f);

               this.numberOfColumns = 2;

               // Create our menu entries.
               otherControlsMenuEntry = new MenuEntry("Keyboard Controls");
               backMenuEntry = new MenuEntry("Back");

               backMenuEntry.DescriptionPosition = new Vector2(backMenuEntry.DescriptionPosition.X, backMenuEntry.DescriptionPosition.Y + 40f);
               backMenuEntry.Position = new Vector2(backMenuEntry.Position.X - 225, backMenuEntry.DescriptionPosition.Y - 160f);

               otherControlsMenuEntry.DescriptionPosition = new Vector2(otherControlsMenuEntry.DescriptionPosition.X, otherControlsMenuEntry.DescriptionPosition.Y + 40f);
               otherControlsMenuEntry.Position = new Vector2(otherControlsMenuEntry.Position.X - 225, otherControlsMenuEntry.DescriptionPosition.Y - 160f);//165f);

               // Hook up menu event handlers.
               otherControlsMenuEntry.Selected += OtherControlsMenuEntrySelected;
               backMenuEntry.Selected += OnCancel;

               // Add entries to the menu.
               MenuEntries.Add(otherControlsMenuEntry);
               MenuEntries.Add(backMenuEntry);

               foreach (MenuEntry entry in MenuEntries)
               {
                    entry.AdditionalVerticalSpacing = 5;

                    entry.menuEntryBorderSize = new Vector2(425, 100);
                    entry.IsPulsating = false;
                    entry.SelectedColor = entry.UnselectedColor;
               }

               backMenuEntry.IsPulsating = true;
          }

          public override void LoadContent()
          {
               base.LoadContent();

               controlsTexture = ResourceManager.LoadTexture(@"Menus\Controller");
               keyboardTexture = ResourceManager.LoadTexture(@"Menus\KeyboardControls");
          }

          #endregion

          #region Menu Entry Events

          void OtherControlsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               showKeyboardControls = !showKeyboardControls;

               if (showKeyboardControls)
               {
                    otherControlsMenuEntry.Text = "Gamepad Controls";
                    //otherControlsMenuEntry.Description = "View the Controls for the 360 Gamepad";
               }

               else
               {
                    otherControlsMenuEntry.Text = "Keyboard Controls";
                    //otherControlsMenuEntry.Description = "View the Controls for a USB Keyboard";
               }
          }

          #endregion

          #region Draw

          public override void Draw(GameTime gameTime)
          {
               ScreenManager.FadeBackBufferToBlack(255 * 3 / 5);

               base.Draw(gameTime);

               MySpriteBatch.Begin();

               Color color = Color.White * (TransitionAlpha / 255f);

               if (showKeyboardControls)
               {
                    MySpriteBatch.Draw(keyboardTexture.Texture2D,
                         new Vector2(EngineCore.ScreenCenter.X, EngineCore.ScreenCenter.Y + 20),
                         null, color, 0.0f,
                         new Vector2(keyboardTexture.Texture2D.Width / 2, keyboardTexture.Texture2D.Height / 2),
                         0.75f);

                    string selectionNote = TextManager.WrapText("Target an Enemy by typing the first letter of that Enemy's sentence.", 500f);

                    TextManager.DrawCentered(true, ScreenManager.Font, selectionNote,
                         new Vector2(EngineCore.ScreenCenter.X, EngineCore.ScreenCenter.Y - 170), color, 0.75f);
               }

               else
               {
                    MySpriteBatch.Draw(controlsTexture.Texture2D,
                         new Vector2(EngineCore.ScreenCenter.X, EngineCore.ScreenCenter.Y + 100), 
                         null, color, 0.0f,
                         new Vector2(controlsTexture.Texture2D.Width / 2, controlsTexture.Texture2D.Height / 2),
                         0.75f);

                    string keyboardNote = TextManager.WrapText("Note: A USB Keyboard or an Xbox 360 Text Messaging Kit ('Chat Pad') is needed to play this game.", 500f);

                    TextManager.DrawCentered(true, ScreenManager.Font, keyboardNote,
                         new Vector2(EngineCore.ScreenCenter.X, EngineCore.ScreenCenter.Y - 175), color, 0.75f);
               }

               MySpriteBatch.End();
          }

          #endregion
     }
}
