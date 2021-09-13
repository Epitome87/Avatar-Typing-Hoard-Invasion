#region File Description
//-----------------------------------------------------------------------------
// EnemyInfoMenuScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using PixelEngine.Menu;
using PixelEngine.Screen;
#endregion

namespace AvatarTyping
{
     public class EnemyInfoMenuScreen : MenuScreen
     {
          #region Initialization

          /// <remarks>
          /// A Menu Screen that displays a list of Enemies.
          /// Clicking on an Enemy entry will display information on that Enemy.
          /// </remarks>
          public EnemyInfoMenuScreen()
               : base("Enemy Information")
          {
               this.TransitionOnTime = TimeSpan.FromSeconds(1.5f);
               this.TransitionOffTime = TimeSpan.FromSeconds(1.0f);

               this.numberOfColumns = 2;
               MenuEntry normalEnemy = new MenuEntry("Normal Enemy", "View Information About This Enemy");
               MenuEntry fastEnemy = new MenuEntry("Speedster Enemy", "View Information About This Enemy");
               MenuEntry kamikazeEnemy = new MenuEntry("Kamikaze Enemy", "View Information About This Enemy");
               MenuEntry explosiveEnemy = new MenuEntry("Explosive Enemy", "View Information About This Enemy");
               MenuEntry deflatableEnemy = new MenuEntry("Deflatable Enemy", "View Information About This Enemy");
               MenuEntry dancingEnemy = new MenuEntry("Dancing Enemy", "View Information About This Enemy");
               MenuEntry backwardEnemy = new MenuEntry("Backwards Enemy", "View Information About This Enemy");
               MenuEntry bossEnemy = new MenuEntry("Boss Enemy", "View Information About This Enemy");
               MenuEntry backMenuEntry = new MenuEntry("Back");

               backMenuEntry.Position = new Vector2(backMenuEntry.Position.X, backMenuEntry.Position.Y + 50f);

               normalEnemy.Selected += MenuEntrySelected;
               fastEnemy.Selected += MenuEntrySelected;
               kamikazeEnemy.Selected += MenuEntrySelected;
               explosiveEnemy.Selected += MenuEntrySelected;
               deflatableEnemy.Selected += MenuEntrySelected;
               dancingEnemy.Selected += MenuEntrySelected;
               backwardEnemy.Selected += MenuEntrySelected;
               bossEnemy.Selected += MenuEntrySelected;
               backMenuEntry.Selected += OnCancel;

               // Add entries to the menu.
               MenuEntries.Add(normalEnemy);
               MenuEntries.Add(fastEnemy);
               MenuEntries.Add(kamikazeEnemy);
               MenuEntries.Add(explosiveEnemy);
               MenuEntries.Add(deflatableEnemy);
               MenuEntries.Add(dancingEnemy);
               MenuEntries.Add(backwardEnemy);
               MenuEntries.Add(bossEnemy);
               //MenuEntries.Add(backMenuEntry);

               foreach (MenuEntry entry in MenuEntries)
               {
                    entry.AdditionalVerticalSpacing = 25;
                    entry.Position = new Vector2(entry.Position.X - 230, entry.Position.Y + 75);
                    entry.IsPulsating = false;
                    entry.menuEntryBorderSize = new Vector2(425, 100);
                    entry.SelectedColor = entry.UnselectedColor;
               }
          }

          #endregion

          #region Menu Entry Events

          /// <summary>
          /// Event handler for when the Extras menu entry is selected.
          /// </summary>
          void MenuEntrySelected(object sender, PlayerIndexEventArgs e)
          {
               EnemyType enemyType = EnemyType.Normal;
               string introductionString = "";

               switch (this.SelectedMenuEntry)
               {
                    case 0:
                         enemyType = EnemyType.Normal;
                         introductionString = "Introducing\nNormal Enemy!";
                         break;

                    case 1:
                         enemyType = EnemyType.Fast;
                         introductionString = "Introducing\nSpeedster Enemy!";
                         break;

                    case 2:
                         enemyType = EnemyType.Kamikaze;
                         introductionString = "Introducing\nKamikaze Enemy!";
                         break;

                    case 3:
                         enemyType = EnemyType.Explosive;
                         introductionString = "Introducing\nExplosive Enemy!";
                         break;

                    case 4:
                         enemyType = EnemyType.Deflatable;
                         introductionString = "Introducing\nDeflatable Enemy!";
                         break;

                    case 5:
                         enemyType = EnemyType.Dancing;
                         introductionString = "Introducing\nDancing Enemy!";
                         break;

                    case 6:
                         enemyType = EnemyType.Backward;
                         introductionString = "Introducing\nBackwards Enemy!";
                         break;

                    case 7:
                         enemyType = EnemyType.Boss;
                         introductionString = "Introducing\nBoss Enemy!";
                         break;
               }

               ScreenManager.AddScreen(new EnemyIntroductionScreen(enemyType, introductionString), e.PlayerIndex);
          }

          #endregion

          #region Draw

          public override void Draw(GameTime gameTime)
          {
               ScreenManager.FadeBackBufferToBlack(255 * 3 / 5);

               base.Draw(gameTime);
          }

          #endregion
     }
}