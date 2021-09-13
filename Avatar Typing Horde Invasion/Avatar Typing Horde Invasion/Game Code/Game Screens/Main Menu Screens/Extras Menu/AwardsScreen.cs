#region File Description
//-----------------------------------------------------------------------------
// AwardsScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine;
using PixelEngine.AchievementSystem;
using PixelEngine.Graphics;
using PixelEngine.Screen;
#endregion

namespace AvatarTyping
{
     /// <remarks>
     /// The Awards screen, found in the Options Menu, is a Menu Screen.
     /// It displays a list of possible Awards for the player to obtain.
     /// Awards which have been unlocked will appear in one color, while 
     /// locked awards will appear gray.
     /// </remarks>
     public class AwardsScreen : PagedMenuScreen
     {
          #region Fields

          SpriteBatch spriteBatch;
          ContentManager content;

          Vector2 textPosition = new Vector2(EngineCore.ScreenCenter.X, 0);

          Texture2D lockTexture;
          Texture2D pic;
          Texture2D blank;

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public AwardsScreen() : base("A w a r d s", 64, 7)
          {
               this.TransitionOnTime = TimeSpan.FromSeconds(1.5);
               this.TransitionOffTime = TimeSpan.FromSeconds(1.0);
               
               this.MenuTitleColor = Color.CornflowerBlue;

               for (int i = 0; i < AchievementManager.Count; i++)
               {
                    if (i >= MenuEntries.Count)
                         break;

                    MenuEntries[i].Text = AchievementManager.Achievements[i].Title;
                    MenuEntries[i].Description = AchievementManager.Achievements[i].Description;
                    MenuEntries[i].Selected += AwardSelected;
                    MenuEntries[i].SelectedColor = Color.Pink;// Color.White;// Color.DarkOrange;
                    MenuEntries[i].IsCenter = true;
                    MenuEntries[i].BorderColor = Color.White;
                    
                    
                    MenuEntries[i].AdditionalVerticalSpacing = 10;
                    MenuEntries[i].Position = new Vector2(MenuEntries[i].Position.X, MenuEntries[i].Position.Y + 15);
                    MenuEntries[i].menuEntryBorderSize = new Vector2(695, 100);
                    MenuEntries[i].menuEntryBorderScale = new Vector2(1.0f, 0.80f);
                    MenuEntries[i].IsPulsating = false;
               }
          }

          public override void LoadContent()
          {
               base.LoadContent();

               if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

               lockTexture = content.Load<Texture2D>(@"Achievements\Lock_Clear");

               blank = content.Load<Texture2D>(@"Textures\Blank Textures\blank");

               spriteBatch = ScreenManager.SpriteBatch;

               pic = content.Load<Texture2D>(@"Achievements\Award");
               for (int i = 0; i < AchievementManager.Achievements.Count; i++)
               {
                    AchievementManager.Achievements[i].Picture = pic;
               }
          }

          #endregion

          #region Handle Input

          public override void HandleInput(InputState input)
          {
               base.HandleInput(input);
          }

          #endregion

          #region Menu Entry Events

          /// <summary>
          /// Event handler for when the Difficulty menu entry is selected.
          /// </summary>
          void AwardSelected(object sender, PlayerIndexEventArgs e)
          {
          }

          #endregion

          #region Update

          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
          }

          #endregion

          #region Draw

          public override void Draw(GameTime gameTime)
          {
               ScreenManager.FadeBackBufferToBlack(255 * 3 / 5);

               // Doesn't work in XNA 4.0:
               spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

               Color color = Color.White;

               for (int i = 0; i < AchievementManager.Count; i++)
               {
                    if (SelectedMenuEntry == i)
                    {
                         if (AchievementManager.Achievements[i].IsUnlocked)
                         {
                              
                         }

                         else
                         {
                              MySpriteBatch.Draw(lockTexture, 
                                   new Rectangle((int)MenuEntries[SelectedMenuEntry].Position.X + (int)(MenuEntries[SelectedMenuEntry].menuEntryBorderSize.X / 2),
                                                 (int)MenuEntries[SelectedMenuEntry].RenderedPosition.Y - (int)MenuEntries[SelectedMenuEntry].GetTrueHeight() / 1,
                                                 75, 75), 
                                   Color.White);
                         }
                    }
               }

               for (int i = 0; i < this.MenuEntries.Count; i++)
               {
                    if (!AchievementManager.Achievements[i].IsUnlocked)
                    {
                         this.MenuEntries[i].UnselectedColor = Color.Gray;
                         this.MenuEntries[i].SelectedColor = Color.Gray;
                    }

                    else
                    {
                         this.MenuEntries[i].UnselectedColor = Color.White;
                         this.MenuEntries[i].SelectedColor = Color.White;
                    }
               }

               spriteBatch.End();

               base.Draw(gameTime);
          }

          #endregion
     }
}
