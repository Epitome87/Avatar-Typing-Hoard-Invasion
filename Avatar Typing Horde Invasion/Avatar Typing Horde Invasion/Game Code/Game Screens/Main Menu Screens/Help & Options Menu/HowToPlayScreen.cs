#region File Description
//-----------------------------------------------------------------------------
// HowToPlayScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using PixelEngine;
using PixelEngine.Graphics;
using PixelEngine.Screen;
using PixelEngine.Text;
#endregion

namespace AvatarTyping
{
     /// <remarks>
     /// A Menu screen that displays a "How To Play" description.
     /// </remarks>
     public class HowToPlayScreen : PagedMenuScreen
     {
          #region Fields

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public HowToPlayScreen()
               : base("H o w  T o  P l a y", 8, 1)
          {
               this.TransitionOnTime = TimeSpan.FromSeconds(1.5f);
               this.TransitionOffTime = TimeSpan.FromSeconds(1.0f);

               for (int i = 0; i < 8; i++)
               {
                    MenuEntries[i].Text = "Back";
                    MenuEntries[i].Selected += OnCancel;
                    MenuEntries[i].IsCenter = true;
                    MenuEntries[i].DescriptionPosition = new Vector2(MenuEntries[i].DescriptionPosition.X, MenuEntries[i].DescriptionPosition.Y + 40f);
                    MenuEntries[i].Position = new Vector2(MenuEntries[i].Position.X, MenuEntries[i].DescriptionPosition.Y - 170);
                    MenuEntries[i].SelectedColor = Color.White;
               }
          }

          public override void LoadContent()
          {
               base.LoadContent();
          }

          #endregion

          #region Handle Input and Update


          protected override void OnCancel(PlayerIndex playerIndex)
          {
               base.OnCancel(playerIndex);
          }

          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
          }

          #endregion

          #region Draw

          public override void Draw(GameTime gameTime)
          {
               ScreenManager.FadeBackBufferToBlack(255 * 3 / 5);

               base.Draw(gameTime);

               MySpriteBatch.Begin();

               Color color = Color.White * (TransitionAlpha / 255f);

               string s = "";
               string heading = "";

               switch (this.CurrentPageNumber)
               {
                    case 1:
                         heading = "OBJECTIVE";
                         s = TextManager.WrapText("Type the sentences displayed by the enemies.\n\n", 550);
                         s += TextManager.WrapText("Complete the sentence before the enemy reaches you, otherwise you will lose health!", 550);

                         break;

                    case 2:
                         heading = "TARGETING AN ENEMY";
                         s =  TextManager.WrapText("To 'Target' an Enemy, simply type the first character in that Enemy's sentence.\n\n", 550);
                         s += TextManager.WrapText("If an Enemy is the 'Target', his sentence will appear in red text that is larger than the text found in other Enemy sentences.\n\n", 550);
                         s += TextManager.WrapText("While an Enemy is targeted, only that Enemy's sentence can be typed.", 550);

                         break;

                    case 3:
                         heading = "UNTARGETING AN ENEMY";
                         s =  TextManager.WrapText("To deselect the current 'Target', press LB or RB on the 360 Controller, or Tab on the Keyboard.\n\n", 550);
                         s += TextManager.WrapText("You should deselect a 'Target' when you feel another Enemy poses a larger threat.", 550);

                         break;

                    case 4:
                         heading = "SCORING";
                         s =  TextManager.WrapText("Upon defeating an Enemy, points are awarded to the player.\n\n", 550);
                         s += TextManager.WrapText("Two factors play a role in determining the amount of points earned:\n", 550);
                         s += TextManager.WrapText("1) The type of Enemy defeated.\n", 550);
                         s += TextManager.WrapText("2) Whether any Bonus Points were earned.", 550);

                         break;

                    case 5:
                         heading = "BONUS POINTS";
                         s = TextManager.WrapText("Perfect accuracy and Speedy typing will reward you with Bonus Points.\n\n", 550);
                         s += TextManager.WrapText("These extra points are marked by 'Perfect Kill' and 'Speedy Kill' bonuses.", 550);

                         break;

                    case 6:
                         heading = "STREAKING";
                         s = TextManager.WrapText("A 'Streak' is obtained by receiving 2 or more of the same Bonus in a row.\n\n", 550);
                         s += TextManager.WrapText("There are two type of 'Streaks':\n", 550);
                         s += TextManager.WrapText("1) Perfect Streak - Consecutive 'Perfect Kills'. \n", 550);
                         s += TextManager.WrapText("2) Speedy Streak - Consecutive 'Speedy Kills'.", 550);

                         break;

                    case 7:
                         heading = "COMBO METER";
                         s =  TextManager.WrapText("Receiving a Perfect Kill or Speedy Kill will cause your Combo Meter to fill.\n\n", 550);
                         s += TextManager.WrapText("Fill up the Combo Meter completely to earn an extra life!\n\n", 550);
                         s += TextManager.WrapText("The higher your Perfect Streak or Speedy Streak, the faster your Combo Meter fills.", 550);

                         break;

                    case 8:
                         heading = "PROGRESSION";
                         s =  TextManager.WrapText("Survive the wave of enemies to progress to the next wave.\n\n", 550);
                         s += TextManager.WrapText("With each wave increasing in difficulty, how long will you last?!", 550);

                         break;
               }

               // Render the Heading.

               color = Color.White * (TransitionAlpha / 255f);

               TextManager.DrawCentered(true, ScreenManager.Font, heading,
                    new Vector2(EngineCore.ScreenCenter.X, 200), color, 1.20f);
               

               // Render the Sub-Text.

               color = Color.Gold * (TransitionAlpha / 255f);

               TextManager.DrawCentered(true, ScreenManager.Font, s,
                    new Vector2(EngineCore.ScreenCenter.X, EngineCore.ScreenCenter.Y + 25), color, 1.0f);

               MySpriteBatch.End();
          }

          #endregion
     }
}
