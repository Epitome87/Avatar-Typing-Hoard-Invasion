#region File Description
//-----------------------------------------------------------------------------
// ModeSelectScreen.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine;
using PixelEngine.Avatars;
using PixelEngine.CameraSystem;
using PixelEngine.Menu;
using PixelEngine.Screen;
#endregion

namespace AvatarTyping
{
     /// <remarks>
     /// A MenuScreen which presents the player with the two Modes to play.
     /// Upon choosing one of the Modes, the player is taken to the corresponding
     /// Gameplay Screen.
     /// 
     /// On this MenuScreen, the player's Avatar & Gamer Information is also displayed.
     /// </remarks>
     public class ModeSelectScreen : MenuScreen
     {
          #region Fields

          SpriteBatch spriteBatch;
          ContentManager content;
          SpriteFont ChatFont;

          PlayerBackgroundScreen playerScreen;

          public static bool IsModeSelected = false;
          public static int ModeSelected = 0;

          #endregion

          #region Properties

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public ModeSelectScreen()
               : base("Mode Select")
          {
               CameraManager.SetActiveCamera(CameraManager.CameraNumber.ThirdPerson);
               CameraManager.ActiveCamera.Reset(EngineCore.GraphicsDevice.Viewport);
               CameraManager.ActiveCamera.Position = new Vector3(0, 2f, -3f);
               CameraManager.ActiveCamera.LookAt = new Vector3(0f, 0f, 20f);


               TransitionOnTime = TimeSpan.FromSeconds(0.5f);
               TransitionOffTime = TimeSpan.FromSeconds(0.5f);

               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.StartingGame;
               }

               // Create our menu entries.
               MenuEntry arcadeModeMenuEntry = new MenuEntry("Arcade\nMode", "Type through enemy\nwaves,earning a score\nafter each one!");
               MenuEntry survivalModeMenuEntry = new MenuEntry("Survival\nMode", "Fight a never-ending\nwave of enemies!");

               // Hook up menu event handlers.
               arcadeModeMenuEntry.Selected += ArcadeMenuSelected;
               survivalModeMenuEntry.Selected += SurvivalMenuSelected;

               arcadeModeMenuEntry.Position = new Vector2(arcadeModeMenuEntry.Position.X + 200, arcadeModeMenuEntry.Position.Y + 50);
               survivalModeMenuEntry.Position = new Vector2(survivalModeMenuEntry.Position.X + 200, survivalModeMenuEntry.Position.Y + 75);

               arcadeModeMenuEntry.DescriptionPosition = new Vector2(arcadeModeMenuEntry.DescriptionPosition.X + 200, arcadeModeMenuEntry.DescriptionPosition.Y - 50);
               survivalModeMenuEntry.DescriptionPosition = new Vector2(survivalModeMenuEntry.DescriptionPosition.X + 200, survivalModeMenuEntry.DescriptionPosition.Y - 50);

               arcadeModeMenuEntry.DescriptionFontScale = 1.0f;// 0.75f;

               if (Guide.IsTrialMode)
               {
                    survivalModeMenuEntry.SelectedColor = Color.Gray;
                    survivalModeMenuEntry.UnselectedColor = Color.Gray;
                    survivalModeMenuEntry.Description = "\nNot Available In Trial Mode.";
               }

               // Add entries to the menu.
               MenuEntries.Add(arcadeModeMenuEntry);
               MenuEntries.Add(survivalModeMenuEntry);

               foreach (MenuEntry entry in MenuEntries)
               {
                    entry.AdditionalVerticalSpacing = 20;
                    entry.FontScale = 1.5f;
                    entry.IsPulsating = false;
                    entry.SelectedColor = entry.UnselectedColor;
                    entry.ShowIcon = false;
               }

               playerScreen = new PlayerBackgroundScreen();
               ScreenManager.AddScreen(playerScreen, AvatarTypingGame.ControllingPlayer);

               playerScreen.playerPosition = new Vector3(0.95f, 1.15f - 0.15f, 0.0f);//new Vector3(0.95f - 0.15f, 1.15f, 0.0f);
               playerScreen.border = new Rectangle(200, 150, 350, 450);//new Rectangle(200 + 25, 150, 350, 450);

               // New
               playerScreen.player.Avatar.AvatarAnimation = new AvatarBaseAnimation(AvatarAnimationPreset.Wave);
          }

          public override void LoadContent()
          {
               base.LoadContent();

               if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

               spriteBatch = ScreenManager.SpriteBatch;

               ChatFont = ScreenManager.Font;
          }

          #endregion

          #region Menu Event Handlers

          /// <summary>
          /// Event handler for when an Unlockable menu entry is selected.
          /// </summary>
          void ArcadeMenuSelected(object sender, PlayerIndexEventArgs e)
          {
               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.ArcadeMode;
               }

               ModeSelected = 1;

               ScreenManager.AddScreen(new StageSelectScreen(), e.PlayerIndex);
          }

          /// <summary>
          /// Event handler for when an Unlockable menu entry is selected.
          /// </summary>
          void SurvivalMenuSelected(object sender, PlayerIndexEventArgs e)
          {
               if (Guide.IsTrialMode)
                    return;

               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.SurvivalMode;
               }

               ModeSelected = 2;

               ScreenManager.AddScreen(new StageSelectScreen(), e.PlayerIndex);
          }

          protected override void OnCancel(PlayerIndex playerIndex)
          {
               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.AtMenu;
               }

               base.OnCancel(playerIndex);
          }

          #endregion

          #region Handle Input

          public override void HandleInput(InputState input)
          {
               if (input == null)
                    throw new ArgumentNullException("input");

               base.HandleInput(input);

               // Look up inputs for the active player profile.
               PlayerIndex playerIndex = ControllingPlayer.Value;

               if (input.IsMenuCancel(ControllingPlayer, out playerIndex))
               {
                    base.OnCancel(playerIndex);

                    // Remove this screen and the screens it contains.
                    ScreenManager.RemoveScreen(playerScreen);
                    ScreenManager.RemoveScreen(this);
               }
          }

          #endregion

          #region Update

          public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               if (StageSelectScreen.IsStageSelected)
               {
                    ScreenManager.RemoveScreen(playerScreen);
                    ScreenManager.RemoveScreen(this);

                    GameplayBackgroundScreen.isUpdate = false;

                    // Reset the Mode properties.
                    IsModeSelected = false;
                    int selectedMode = ModeSelected;
                    ModeSelected = 0;

                    // Reset the Stage properties.
                    StageSelectScreen.IsStageSelected = false;
                    int selectedStage = StageSelectScreen.StageSelected;
                    StageSelectScreen.StageSelected = 0;

                    if (selectedMode == 1)
                    {
                         AnimatedLoadingScreen.Load(true, EngineCore.ControllingPlayer, "Arcade Mode\nWave 1", true, new ArcadeGameplayScreen(selectedStage));
                    }

                    else if (selectedMode == 2)
                    {
                         AnimatedLoadingScreen.Load(true, EngineCore.ControllingPlayer, "Survival Mode", true, new SurvivalGameplayScreen(selectedStage));
                    }
               }
          }

          #endregion

          #region Draw

          public override void Draw(GameTime gameTime)
          {
               base.Draw(gameTime);
          }

          #endregion
     }
}