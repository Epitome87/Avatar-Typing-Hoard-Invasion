#region File Description
//-----------------------------------------------------------------------------
// CreditsScreen.cs
//
// Copyright (C) Matt McGrath.
// Derives from a modified XNA class provided by Microsoft.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using PixelEngine;
using PixelEngine.Avatars;
using PixelEngine.CameraSystem;
using PixelEngine.Screen;
#endregion

namespace AvatarTyping
{
     /// <remarks>
     /// The Credits screen is loaded upon the game's end, or via the Main Menu.
     /// This screen displays the list of those who participated in the game's creation.
     /// Along with their name and role, their avatar is displayed.
     /// </remarks>
     public class AvatarTypingCreditsScreen : CreditScreen
     {
          #region Fields

          Avatar Mike = new Avatar(EngineCore.Game);
          Avatar Killthief = new Avatar(EngineCore.Game);

          Avatar Apoth = new Avatar(EngineCore.Game);
          Avatar Mach = new Avatar(EngineCore.Game);

          Avatar Dan = new Avatar(EngineCore.Game);
          Avatar Sylent = new Avatar(EngineCore.Game);

          #endregion

          #region Initialization

          // For the Avatars; used to represent the people in the Credits.
          private Avatar mattsAvatar;
          private AvatarBaseAnimation[] mattsAnimations = new AvatarBaseAnimation[7];

          // Keep track of what credit we are on.
          private int lastRole = 0;

          public AvatarTypingCreditsScreen()
               : base("")
          {
               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.WatchingCredits;
               }

               this.RoleScale = 2.5f;
               this.NameScale = 1.25f;

               this.UnrushedScrollSpeed = 3f;
               this.RushedScrollSpeed = 6f;

               creditEntries.Add(new CreditEntry("Programming", "Matt McGrath"));
               creditEntries.Add(new CreditEntry("Design", "Matt McGrath"));
               creditEntries.Add(new CreditEntry("Art", "Matt McGrath", "Models From Turbosquid.com"));
               creditEntries.Add(new CreditEntry("Music", "", "Matthew McFarland - Mattmcfarland.com", "Kevin MacLeod - Incompetech.com"));
               creditEntries.Add(new CreditEntry("Special Thanks", "", "BiblicalViolenc    Kill Thief XP", "l Apotheosis l                Paragon XI", "WereTh3Massacre                   x SylenT x"));
               creditEntries.Add(new CreditEntry("Thank You\nFor Playing!"));

               CameraManager.SetActiveCamera(CameraManager.CameraNumber.ThirdPerson);
               CameraManager.ActiveCamera.Reset(EngineCore.GraphicsDevice.Viewport);
               CameraManager.ActiveCamera.Position = new Vector3(0, 2f, -3f);
               CameraManager.ActiveCamera.LookAt = new Vector3(0f, 0f, 20f);

               mattsAvatar = new Avatar(ScreenManager.Game, new Vector3(0f, 0.5f, 0f));
               mattsAvatar.LoadAvatar(CustomAvatarType.Matt);

               mattsAnimations[0] = new AvatarBaseAnimation(AvatarAnimationPreset.MaleSurprised);
               mattsAnimations[1] = AvatarManager.LoadedPresetAvatarAnimation["Celebrate"];
               mattsAnimations[2] = AvatarManager.LoadedPresetAvatarAnimation["Clap"];
               mattsAnimations[3] = AvatarManager.LoadedPresetAvatarAnimation["Wave"];
               mattsAnimations[4] = AvatarManager.LoadedPresetAvatarAnimation["IdleLookAround"];
               mattsAnimations[5] = AvatarManager.LoadedPresetAvatarAnimation["IdleStretch"];
               mattsAnimations[6] = AvatarManager.LoadedPresetAvatarAnimation["Yawn"];

               mattsAvatar.PlayAnimation(mattsAnimations[0], true);

               Dan.LoadAvatar(CustomAvatarType.Daniel);
               Mike.LoadAvatar(CustomAvatarType.Mike);
               Sylent.LoadAvatar(CustomAvatarType.Sylent);
               Mach.LoadAvatar(CustomAvatarType.Mach);
               Killthief.LoadAvatar(CustomAvatarType.Killthief);
               Apoth.LoadAvatar(CustomAvatarType.Apotheosis);

               Mike.Position = new Vector3(0.75f, 0.5f, 0.25f);
               Killthief.Position = new Vector3(-0.75f, 0.5f, 0.25f);

               Apoth.Position = new Vector3(1.25f, 0.5f, 0.5f);
               Mach.Position = new Vector3(-1.25f, 0.5f, 0.5f);

               Dan.Position = new Vector3(1.75f, 0.5f, 0.75f);
               Sylent.Position = new Vector3(-1.75f, 0.5f, 0.75f);

               Mike.PlayAndReturnAnimation(AvatarManager.LoadedPresetAvatarAnimation["Wave"], true);
               Killthief.PlayAndReturnAnimation(AvatarManager.LoadedPresetAvatarAnimation["Laugh"], true);
               Apoth.PlayAndReturnAnimation(AvatarManager.LoadedPresetAvatarAnimation["Celebrate"], true);
               Mach.PlayAndReturnAnimation(AvatarManager.LoadedPresetAvatarAnimation["Cry"], true);
               Dan.PlayAndReturnAnimation(AvatarManager.LoadedPresetAvatarAnimation["Yawn"], true);
               Sylent.PlayAndReturnAnimation(AvatarManager.LoadedPresetAvatarAnimation["Confused"], true);
          }

          #endregion

          #region Menu Events

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

          #region Update

          public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               // The Credits are over; give the Player an Award!
               if (this.currentRole == creditEntries.Count - 1)
                    AvatarTypingGame.AwardData.CreditsWatched = true;

               if (this.currentRole != lastRole)
               {
                    mattsAvatar.PlayAnimation(mattsAnimations[currentRole], true);
               }

               lastRole = currentRole;

               // Update my handsome Avatar!
               mattsAvatar.Update(gameTime);

               // The Credits are over; give the Player an Award!
               if (this.currentRole == creditEntries.Count - 2)
               {
                    //TimeSpan duration = gameTime.ElapsedGameTime;
                    //duration = TimeSpan.FromTicks(duration.Ticks / 6);

                    Dan.Update(gameTime);
                    Mike.Update(gameTime);
                    Sylent.Update(gameTime);
                    Mach.Update(gameTime);
                    Killthief.Update(gameTime);
                    Apoth.Update(gameTime);
               }
          }

          #endregion

          #region Draw

          public override void Draw(GameTime gameTime)
          {
               ScreenManager.FadeBackBufferToBlack(255 * 3 / 5);

               mattsAvatar.Draw(gameTime);

               // The Credits are over; give the Player an Award!
               if (this.currentRole == creditEntries.Count - 2)
               {
                    Dan.Draw(gameTime);
                    Mike.Draw(gameTime);
                    Sylent.Draw(gameTime);
                    Mach.Draw(gameTime);
                    Killthief.Draw(gameTime);
                    Apoth.Draw(gameTime);
               }

               base.Draw(gameTime);
          }

          #endregion
     }
}