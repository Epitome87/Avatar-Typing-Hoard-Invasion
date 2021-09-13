#region File Description
//-----------------------------------------------------------------------------
// DancingTypingEnemy.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.Audio;
using PixelEngine.Avatars;
#endregion

namespace AvatarTyping
{
     /// <summary>
     /// Defines what animation the avatar should be using
     /// </summary>
     public enum AnimationPlaybackMode { All, RunAndPound, RunAndCry, RunAndCelebrate, Faint };

     /// <summary>
     /// A Dancing Typing Enemy is one which "Dances" once the Player has
     /// targetted him.
     /// 
     /// While "Dancing", other non-Dancing Typing Enemies will stand in place and
     /// cheer the Dancer on. This means they will not move and thus won't pose a threat.
     /// Therefore Dancing Typing Enemies provide an extra layer of strategy.
     /// 
     /// When a Dancing Typing Enemy is killed or On-Targetted, the other enemies
     /// stop cheering in place.
     /// </summary>
     public class DancingTypingEnemy : TypingEnemy
     {
          #region Fields

          bool isDancing = false;
          Random randomizer = new Random();
          SpriteFont font;

          AvatarBaseAnimation celebrateAnimation;

          Cue dancingCue;

          #endregion

          #region Properties

          #endregion

          #region Overridden Methods

          public override void OnTargetted()
          {
               base.OnTargetted();

               isDancing = true;

               if (!dancingCue.IsPlaying)
                    dancingCue.Play();

               else if (dancingCue.IsPaused)
                    dancingCue.Resume();
          }

          public override void OnUntargetted()
          {
               Undance();

               base.OnUntargetted();

               if (dancingCue.IsPlaying)
                    dancingCue.Pause();
          }

          public override void OnEscaped()
          {
               Undance();

               base.OnEscaped();

               if (dancingCue.IsPlaying)
                   dancingCue.Stop(AudioStopOptions.Immediate);
          }

          public override void OnHit()
          {
               base.OnHit();
          }

          public override void OnMiss()
          {
               base.OnMiss();
          }

          public override void OnKilled()
          {
               // If this Enemy isn't the target, yet it was killed...
               // It must have been from an Explosive Enemy. That means
               // the other Enemies aren't dancing, so there's no reason
               // to make them Undance.
               if (this.IsTarget)
               {
                    Undance();
               }
 
               base.OnKilled();

               if (dancingCue.IsPlaying)
                    dancingCue.Stop(AudioStopOptions.Immediate);
          }

          #endregion

          #region Initialization

          public DancingTypingEnemy(Vector3 position, EnemyManager enemyManager)
               : base(PixelEngine.EngineCore.Game, enemyManager)
          {
               this.Speed = 1.0f * (int)(AvatarTypingGameSettings.Difficulty + 1) * (0.50f);

               if (AvatarTypingGameSettings.Difficulty == Difficulty.Hard ||
                   AvatarTypingGameSettings.Difficulty == Difficulty.Insane)
               {
                    // Make their speed initially slower than originally.
                    this.Speed *= 0.90f;
               }

               // Now add 5% speed each Wave.
               this.Speed *= MathHelper.Clamp(
                    (1.0f + (0.05f * enemyManager.WaveNumber)), 1.0f, 1.75f);

               this.Position = position;
               this.BasePoints = 250;
               this.BonusPoints = 250;
               this.ElapsedTime = 0.0f;
               this.IsTarget = false;
               this.WasMissed = false;

               this.ShotSound = "KeyPress";
               this.DeathSound = "Explosion";

               this.WordList.Add(new TypingWord(this.GenerateWord(true)));

               this.Color = Color.LightPink;

               this.dancingCue = AudioManager.GetCue("SpazzmaticaPolka");

               this.Initialize();
          }

          protected override void LoadContent()
          {
               base.LoadContent();

               font = PixelEngine.Text.TextManager.MenuFont;

               Avatar.AvatarDescription = AvatarDescription.CreateRandom();

               // Load the preset animations
               celebrateAnimation = new AvatarBaseAnimation(AvatarAnimationPreset.Celebrate);

               // List of the bone index values for the right arm and its children
               List<AvatarBone> bonesUsedInFirstAnimation = new List<AvatarBone>();
               List<AvatarBone> bonesUsedInSecondAnimation = new List<AvatarBone>();

               bonesUsedInFirstAnimation.Add(AvatarBone.ToeLeft);
               bonesUsedInSecondAnimation.Add(AvatarBone.ShoulderLeft);
               bonesUsedInSecondAnimation.Add(AvatarBone.ShoulderRight);

               List<List<AvatarBone>> listOfBonesUsedForEachAnimation = new List<List<AvatarBone>>();
               listOfBonesUsedForEachAnimation.Add(bonesUsedInFirstAnimation);
               listOfBonesUsedForEachAnimation.Add(bonesUsedInSecondAnimation);

               List<AvatarBaseAnimation> animationsToCombine = new List<AvatarBaseAnimation>();
               animationsToCombine.Add(new AvatarCustomAnimation(AvatarManager.LoadedAvatarAnimationData["Run"]));
               animationsToCombine.Add(celebrateAnimation);

               Avatar.PlayMultipleAnimation(true, Avatar.AvatarRenderer, animationsToCombine, listOfBonesUsedForEachAnimation);
          }

          protected override void UnloadContent()
          {
               base.UnloadContent();
          }

          #endregion

          #region Update

          /// <summary>
          /// 
          /// </summary>
          public override void Update(GameTime gameTime)
          {
               // Update their Shot-off Characters.
               UpdateShotWords();

               if (this.IsDying)
               {
                    elapsedDyingTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Avatar.Update(gameTime);

                    return;
               }

               this.Position = new Vector3(this.Position.X, 0, this.Position.Z - this.Speed / 25f);
               this.Avatar.Position = this.Position;
               this.WorldPosition = Matrix.CreateScale(3.0f) *
                                    Matrix.CreateRotationY(MathHelper.ToRadians(0.0f)) *
                                    Matrix.CreateTranslation(this.Position);

               if (this.IsTarget)
               {
                    // The following use to be in the other if statement.
                    Dance();
                    isDancing = false;
                    // The above use to be in the other if statement.

                    this.ElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // Increment how long we've been fighting / targetting this Enemy!
                    this.ElapsedKillTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
               }

               Avatar.Update(gameTime);
          }

          #endregion

          public static bool AreWeDancing = false;

          #region Dancing Enemy-Specific Methods

          /// <summary>
          /// Explodes the Exploding Enemy, sending shrapnel toward the 
          /// other Enemies.
          /// </summary>
          public void Dance()
          {
               AreWeDancing = true;

               foreach (TypingEnemy nearbyEnemy in EnemyManager.GetEnemies())
               {
                    if (nearbyEnemy == this)
                         continue;

                    if (nearbyEnemy.GetType().Equals(typeof(DancingTypingEnemy)))
                         continue;

                    nearbyEnemy.Position = new Vector3(nearbyEnemy.Position.X, 0,
                         nearbyEnemy.Position.Z + nearbyEnemy.Speed / 25f);

                    if (isDancing)
                         nearbyEnemy.Avatar.PlayAnimation(new AvatarBaseAnimation(AvatarAnimationPreset.Clap), true);
               }
          }

          /// <summary>
          /// Explodes the Exploding Enemy, sending shrapnel toward the 
          /// other Enemies.
          /// </summary>
          public void Undance()
          {
               AreWeDancing = false;

               foreach (TypingEnemy nearbyEnemy in EnemyManager.GetEnemies())
               {
                    if (nearbyEnemy == this)
                         continue;

                    if (nearbyEnemy.GetType().Equals(typeof(DancingTypingEnemy)))
                         continue;

                    nearbyEnemy.Position = new Vector3(nearbyEnemy.Position.X, 0,
                         nearbyEnemy.Position.Z + nearbyEnemy.Speed / 25f); // What's up with the 25? This means they move, just really slowly?

                    nearbyEnemy.Run();
               }
          }

          #endregion
     }
}