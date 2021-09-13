#region File Description
//-----------------------------------------------------------------------------
// SavePopUpScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.AchievementSystem;
using PixelEngine.Graphics;
using PixelEngine.Screen;
using PixelEngine.Text;
#endregion

namespace PixelEngine
{
     /// <remarks>
     /// A Pop-Up Screen which acts comparable to the 360's "Achievement Unlocked" pop-up.
     /// Although this is a screen, it should be used as if it were a Component. 
     /// 
     /// Default behavior: The Award Unlocked icon flies in from the left until it 
     /// arrives at its destination. A short sound is displayed with the icon, along
     /// with "Award Unlocked: 'Name of Award'". The icon disappears after 5 seconds.
     /// </remarks>
     public class SavePopUpScreen : MenuScreen
     {
          #region Fields

          private float elapsedTime = 0.0f;
 
          #endregion

          #region Properties

          #endregion

          #region Initialization

          public SavePopUpScreen()
               : base("")
          {
               this.IsOverlayPopup = true;
          }

          public override void LoadContent()
          {
               base.LoadContent();
          }

          #endregion

          #region Update

          string nowSavingText = ".";

          public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

               if (elapsedTime > 0)
               {
                    nowSavingText = ".";
               }
               if (elapsedTime >= 1)
               {
                    nowSavingText = "..";
               }
               if (elapsedTime >= 2)
               {
                    nowSavingText = "...";
               }

               if (elapsedTime >= 3)
               {
                    this.ExitPopupScreen();
               }
          }

          #endregion

          #region Draw

          public override void Draw(GameTime gameTime)
          {
               base.Draw(gameTime);

               MySpriteBatch.Begin();

               TextManager.DrawString(ScreenManager.Font, "! Saving" + nowSavingText, 
                    new Vector2(EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Left, EngineCore.GraphicsDevice.Viewport.TitleSafeArea.Top), 
                    Color.Gold, 0.75f);

               MySpriteBatch.End();
          }

          #endregion
     }
}