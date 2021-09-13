#region File Description
//-----------------------------------------------------------------------------
// StageIntroScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using PixelEngine.Screen;
#endregion

namespace AvatarTyping
{
     /// <summary>
     /// A short introductory screen that displays what level is about to begin.
     /// </summary>
     public class StageIntroScreen : MenuScreen
     {
          #region Fields

          EnemyManager enemyManager;
          ArcadeLevel theArcadeLevel;
          
          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public StageIntroScreen(ArcadeLevel arcadeLevel, EnemyManager _enemyManager)
               : base("")
          {
               TransitionOnTime = TimeSpan.FromSeconds(0.0);
               TransitionOffTime = TimeSpan.FromSeconds(0.0);

               theArcadeLevel = arcadeLevel;
               enemyManager = _enemyManager;
          }

          public override void HandleInput(InputState input)
          {

          }

          public override void LoadContent()
          {
               theArcadeLevel.BeginWave();
          }

          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
               this.ExitScreen();

               ArcadeGameplayScreen.IntroduceEnemy = true;
          }

          public override void Draw(GameTime gameTime)
          {
               base.Draw(gameTime);
          }

          #endregion
     }
}