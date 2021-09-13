#region File Description
//-----------------------------------------------------------------------------
// BuyScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using PixelEngine;
using PixelEngine.Graphics;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
using PixelEngine.Text;
#endregion

namespace AvatarTyping
{
     /// <remarks>
     /// A PagedMenuScreen which advertises the game's features to potential buyers.
     /// This screen is presented upon the end of the Trial or when a user exits game.
     /// </remarks>
     public class BuyScreen : PagedMenuScreen
     {
          #region Fields

          GameResourceTexture2D enemyTexture;
          GameResourceTexture2D modeTexture;
          GameResourceTexture2D otherTexture;

          string featureDescription;

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public BuyScreen()
               : base("Buy Full Game!", 6, 2)
          {
               this.titleFontScale = 1.5f;
               this.PageNumberPosition = new Vector2(EngineCore.ScreenCenter.X + EngineCore.ScreenCenter.X / 2f, EngineCore.GraphicsInformation.ScreenHeight - 100);
               this.MenuTitleColor = Color.LightGreen;
               this.ShowPageBorder = false;
               this.ShowPageNumber = false;

               Vector2 menuPosition = new Vector2(0, 440f);

               MenuEntries[0].Text = "Purchase Game";
               MenuEntries[0].Selected += PurchaseSelected;
               MenuEntries[0].IsCenter = true;

               MenuEntries[1].Text = "Next Page";
               MenuEntries[1].Selected += ContinueSelected;
               MenuEntries[1].IsCenter = true;


               MenuEntries[2].Text = "Purchase Game";
               MenuEntries[2].Selected += PurchaseSelected;
               MenuEntries[2].IsCenter = true;

               MenuEntries[3].Text = "Next Page";
               MenuEntries[3].Selected += ContinueSelected;
               MenuEntries[3].IsCenter = true;


               MenuEntries[4].Text = "Purchase Game";
               MenuEntries[4].Selected += PurchaseSelected;
               MenuEntries[4].IsCenter = true;

               MenuEntries[5].Text = "Exit Game";
               MenuEntries[5].Selected += ExitGameSelected;
               MenuEntries[5].IsCenter = true;

               for (int i = 0; i < 6; i++)
               {
                    MenuEntries[i].Position += menuPosition;
                    MenuEntries[i].ShowBorder = false;
                    MenuEntries[i].ShowGradientBorder = false;
               }

          }

          public override void LoadContent()
          {
               base.LoadContent();

               enemyTexture = ResourceManager.LoadTexture(@"Trial Mode\BuyScreen_Enemies");
               modeTexture = ResourceManager.LoadTexture(@"Trial Mode\BuyScreen_Modes");
               otherTexture = ResourceManager.LoadTexture(@"Trial Mode\BuyScreen_Others");
          }

          #endregion

          #region Menu Entry Events

          /// <summary>
          /// Event handler for when the Difficulty menu entry is selected.
          /// </summary>
          void PurchaseSelected(object sender, PlayerIndexEventArgs e)
          {
               if (AvatarTypingGame.CurrentPlayer == null)
               {
                    return;
               }

               if (AvatarTypingGame.CurrentPlayer.GamerInfo == null)
               {
                    return;
               }

               if (!AvatarTypingGame.CurrentPlayer.GamerInfo.CanBuyGame())
               {

                    SimpleGuideMessageBox.ShowMessageBox(ControllingPlayer, "Purchasing Game Disabled",
                         "You cannot purchase the Full version of Avatar Typing: Horde Invasion. To purchase the Full version, please make sure you are signed into Live, on a non-Guest account, and have the appropriate Privileges.",
                         new string[] { "OK" }, 0, MessageBoxIcon.Warning);

                    return;
               }

               // Do Purchase Guide Stuff
               if (Guide.IsTrialMode)
               {
                    Guide.ShowMarketplace(AvatarTypingGame.CurrentPlayer.GamerInfo.PlayerIndex);
               }
          }

          void ContinueSelected(object sender, PlayerIndexEventArgs e)
          {
               base.OnNextPage();
          }

          void ExitGameSelected(object sender, PlayerIndexEventArgs e)
          {
               ScreenManager.Game.Exit();
          }

          #endregion

          #region Update

          public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               // The only way to reach this screen is if we are in Trial Mode.
               // If we are suddenly not in Trial Mode, the player bought the game.
               // So let's remove this nag-screen!
               if (!Guide.IsTrialMode)
               {
                    this.ExitScreen();
               }
          }

          #endregion

          #region Draw

          public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
          {
               ScreenManager.GraphicsDevice.Clear(Color.Black);

               MySpriteBatch.Begin();

               float fontSize = 1.0f;

               if (this.currentPageNumber == 1)
               {
                    MySpriteBatch.DrawCentered(modeTexture.Texture2D, EngineCore.ScreenCenter, Color.White);


                    featureDescription = "Arcade Mode";
                    TextManager.DrawAutoCentered(ScreenManager.Font, featureDescription, new Vector2(340f, 170f), Color.Gold, 1.0f);

                    featureDescription = "Fight enemies one\nwave at a time.\n\nHow many waves\ncan you defeat?";
                    TextManager.DrawAutoCentered(ScreenManager.Font, featureDescription, new Vector2(340f, 270), Color.CornflowerBlue, 0.9f);


                    featureDescription = "Survival Mode";
                    TextManager.DrawAutoCentered(ScreenManager.Font, featureDescription, new Vector2(940f, 410f), Color.Gold, 1.0f);

                    featureDescription = "Fight a never-ending\nstream of enemies.\n\nHow long\ncan you survive?";
                    TextManager.DrawAutoCentered(ScreenManager.Font, featureDescription, new Vector2(940f, 510f), Color.CornflowerBlue, 0.9f);
               }

               if (this.currentPageNumber == 2)
               {
                    MySpriteBatch.DrawCentered(enemyTexture.Texture2D,
                         new Vector2(EngineCore.ScreenCenter.X, EngineCore.ScreenCenter.Y), Color.White);


                    featureDescription = "8 different enemy types, including:\n\nLarge bosses!\nInflatable Enemies!\nDeflatable Enemies!\nKamikaze Enemies!\nDancing Enemies!";

                    TextManager.DrawAutoCentered(ScreenManager.Font, featureDescription,
                         new Vector2(EngineCore.ScreenCenter.X, EngineCore.ScreenCenter.Y), Color.CornflowerBlue, fontSize);
               }

               if (this.currentPageNumber == 3)
               {
                    MySpriteBatch.DrawCentered(otherTexture.Texture2D,
                         new Vector2(EngineCore.ScreenCenter.X, EngineCore.ScreenCenter.Y), Color.White);

                    featureDescription =
                         "Encounter & Type Over\n1,500 Unique Sentences!\n\n" +
                         "Create And Save\nYour Own Sentences!";

                    fontSize = 0.85f;

                    TextManager.DrawAutoCentered(ScreenManager.Font, featureDescription, new Vector2(350, 200f + 70f), Color.CornflowerBlue, fontSize);


                    featureDescription =
                         "New High Score?\nShow It Off To Friends!\n\n" +
                         "Unlock Over 60 Awards!";

                    TextManager.DrawAutoCentered(ScreenManager.Font, featureDescription, new Vector2(920, 540f - 70f), Color.CornflowerBlue, fontSize);
               }

               MySpriteBatch.End();

               base.Draw(gameTime);
          }

          #endregion
     }
}