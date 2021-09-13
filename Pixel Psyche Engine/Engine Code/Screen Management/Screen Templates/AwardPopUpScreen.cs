#region File Description
//-----------------------------------------------------------------------------
// AwardPopUpScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.AchievementSystem;
using PixelEngine.Audio;
using PixelEngine.Graphics;
using PixelEngine.ResourceManagement;
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
     public class AwardPopUpScreen : MenuScreen
     {
          #region Fields

          private Achievement award;

          private GameResourceTexture2D achievementBorderTexture;
          private GameResourceTexture2D achievementTexture;
          private GameResourceTexture2D trophyTexture;

          private float elapsedTime = 0.0f;
          private float awardDisplayTime = 5.0f;

          private string awardPopSound = "AwardPop";
          private Color awardBackdropColor = new Color(50, 50, 50) * (150f / 255f);
          private Color awardTextColor = Color.White * (225f / 255f);
          private float awardScale = 0.75f;

          // Helper variables used for Update calculations.
          private Vector2 startingPosition = new Vector2(-500, 75);
          private bool playSound = true;

          #endregion

          #region Properties

          /// <summary>
          /// The time (seconds) the Award Pop-Up is displayed for.
          /// </summary>
          public float AwardDisplayTime
          {
               get { return awardDisplayTime; }
               set { awardDisplayTime = value; }
          }

          /// <summary>
          /// The sound (cue name) heard when an Award "Pops".
          /// </summary>
          public string AwardPopSound
          {
               get { return awardPopSound; }
               set { awardPopSound = value; }
          }

          /// <summary>
          /// The Color of the Award Pop-Up backdrop.
          /// </summary>
          public Color AwardBackdropColor
          {
               get { return awardBackdropColor; }
               set { awardBackdropColor = value; }
          }

          /// <summary>
          /// The Color of the Award Pop-Up text.
          /// </summary>
          public Color AwardTextColor
          {
               get { return awardTextColor; }
               set { awardTextColor = value; }
          }

          /// <summary>
          /// The Scale to render the Award Pop-Up in.
          /// </summary>
          public float AwardScale
          {
               get { return awardScale; }
               set { awardScale = value; }
          }

          #endregion

          #region Initialization

          /// <summary>
          /// AwardPopUpScreen Constructor.
          /// </summary>
          /// <param name="awardObtained">The Achievement that was obtained / will pop up.</param>
          public AwardPopUpScreen(Achievement awardObtained)
               : base("")
          {
               this.IsOverlayPopup = true;

               award = awardObtained;

               // Tell the AchievementManager that this pop-up screen is now active.
               AchievementManager.isPopUpOnScreen = true;
          }

          /// <summary>
          /// LoadContent Override.
          /// Loads assets necessary for popping our style of Achievements:
          /// A border Texture, a trophy Texture, etc.
          /// </summary>
          public override void LoadContent()
          {
               base.LoadContent();

               achievementBorderTexture = ResourceManagement.ResourceManager.LoadTexture(@"Textures\Blank Textures\Border_Wide_White");
               achievementTexture = ResourceManagement.ResourceManager.LoadTexture(@"Textures\Blank Textures\Blank_Rounded_Wide");//@"Textures\Blank Textures\Blank_Rounded_Wide_WithBorder");
               trophyTexture = ResourceManagement.ResourceManager.LoadTexture(@"Achievements\Award");

               AudioManager.PlayCue(awardPopSound);
          }

          #endregion

          #region Handle Input

          /// <summary>
          /// HandleInput Override.
          /// Does nothing, as this is not an interactive screen.
          /// </summary>
          /// <param name="input"></param>
          public override void HandleInput(InputState input)
          {
               // Do not allow input; it's just a pop-up!
          }

          #endregion

          #region Update

          /// <summary>
          /// Update Override.
          /// 
          /// Updates the Pop-Up Screen logic, such as updating the position of the Achievement earned,
          /// playing a sound as the Achievement flies into place, and making it fly off screen when it
          /// has been displayed for a sufficient amount of time.
          /// </summary>
          /// <param name="gameTime">GameTime.</param>
          /// <param name="otherScreenHasFocus">Whether or not another Screen has focus.</param>
          /// <param name="coveredByOtherScreen">Whether or not this screen is covered by another.</param>
          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

               if (elapsedTime <= awardDisplayTime)
               {
                    startingPosition.X += 10;

                    if (startingPosition.X >= (EngineCore.GraphicsInformation.ScreenWidth - AchievementRectangle.Width - 100)) // 500
                         startingPosition.X = (EngineCore.GraphicsInformation.ScreenWidth - AchievementRectangle.Width - 100); //650f;
               }

               else
               {
                    if (playSound)
                         AudioManager.PlayCue(awardPopSound);

                    playSound = false;

                    startingPosition.X += 10;

                    // If the Achievement reaches here, it's time to remove the PopUp Screen.
                    if (startingPosition.X > 1300)
                    {
                         AchievementManager.isPopUpOnScreen = false;
                         this.ExitPopupScreen();
                    }
               }
          }

          #endregion

          #region Draw

          private Rectangle AchievementRectangle = new Rectangle();

          public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
          {
               base.Draw(gameTime);

               string text = "AWARD UNLOCKED\n" + award.Title;
               
               Vector2 widthHeight = ScreenManager.Font.MeasureString(text) * awardScale;
               Rectangle textureRect = new Rectangle((int)startingPosition.X, (int)startingPosition.Y, (int)widthHeight.X + 20 + 75, (int)widthHeight.Y + 20);
               Rectangle trophyRect = new Rectangle((int)startingPosition.X + 6, 81, 75, textureRect.Height - 12);
               Vector2 textCenter = new Vector2(textureRect.Left + 10 + textureRect.Width / 2, (float)(textureRect.Top + textureRect.Height / 8 + textureRect.Height / 2.0f));
               

               // New
               AchievementRectangle = textureRect;
               // End


               MySpriteBatch.Begin();

               MySpriteBatch.Draw(achievementBorderTexture.Texture2D, textureRect, Color.Black);
               MySpriteBatch.Draw(achievementTexture.Texture2D, textureRect, awardBackdropColor);
               MySpriteBatch.Draw(trophyTexture.Texture2D, trophyRect, Color.Gold * (200f / 255f));
               TextManager.DrawCentered(true, ScreenManager.Font, text, textCenter, awardTextColor, awardScale);
               
               MySpriteBatch.End();
          }

          #endregion
     }
}