#region File Description
//-----------------------------------------------------------------------------
// TypingEnemy.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PixelEngine;
using PixelEngine.Audio;
using PixelEngine.Avatars;
using PixelEngine.CameraSystem;
using PixelEngine.Graphics;
using PixelEngine.ResourceManagement;
using PixelEngine.Screen;
using PixelEngine.Text;
#endregion

namespace AvatarTyping
{
     #region Helper IComparer Class

     /// <summary>
     /// Compares to Enemy's Priorities.
     /// </summary>
     public class CompareByPrioty : IComparer<TypingEnemy>
     {

          public CompareByPrioty()
          {
          }

          // Implementing the Compare method
          public int Compare(TypingEnemy obj1, TypingEnemy obj2)
          {
               TypingEnemy Temp1 = obj1;
               TypingEnemy Temp2 = obj2;

               if (Temp2.Position.Z < Temp1.Position.Z)
               {
                    return 1;
               }

               if (Temp2.Position.Z > Temp1.Position.Z)
               {
                    return -1;
               }

               else
               {
                    return 0;
               }
          }

          public int Compare1(TypingEnemy obj1, TypingEnemy obj2)
          {
               TypingEnemy Temp1 = obj1;
               TypingEnemy Temp2 = obj2;

               Vector2 testPosition2 = GraphicsHelper.ConvertToScreenspace(Temp2.Avatar.WorldMatrix);
               Vector2 testPosition1 = GraphicsHelper.ConvertToScreenspace(Temp1.Avatar.WorldMatrix);

               if (testPosition2.Y < testPosition1.Y)
               {
                    return 1;
               }

               if (testPosition2.Y > testPosition1.Y)
               {
                    return -1;
               }

               else
               {
                    return 0;
               }
          }
     }

     #endregion

     #region EnemyType Enum

     public enum EnemyType
     {
          Normal,        // 0
          Fast,
          Kamikaze,
          Explosive,
          Deflatable,    // 4
          Dancing,
          Backward,
          Horde,
          Boss,          // 8
     }

     #endregion

     /// <remarks>
     /// An abstract Typing Enemy. 
     /// Describes the Enemy as well as his sentences / key lists.
     /// </remarks>
     public abstract class TypingEnemy : DrawableGameComponent, ICameraTrackable
     {
          protected int DamageDoneToPlayer = 1;

          #region Fields

          public EnemyManager.SpawnLocation SpawnLocation = EnemyManager.SpawnLocation.Empty;

          // For creating random word lists for the enemies.
          private Random random = new Random();

          // Enemies should be managed (for spawn / AI reasons).
          private EnemyManager enemyManager;

          // Stores the Enemy's appearance, as an Avatar.
          private Avatar avatar;

          // Sounds (asset name).
          private string shotSound;
          private string missedSound;
          private string deathSound;
          private string escapeSound;

          // Booleans to track Enemy status.
          private bool isAlive;
          private bool isDying;
          private bool isTarget;
          private bool wasMissed;
          private bool isPerfectKill;
          private bool isSpeedKill;
          private bool wasKilledByPlayer;

          public bool IsActive = false;
          public Color Color = Color.DarkOrange;

          private float yVelocity;

          private Vector3 position;
          private Matrix worldPosition;

          private List<TypingWord> wordList;

          private int basePoints;
          private int bonusPoints;
          private float speedBonusRequirement;

          private float elapsedTime;
          protected float elapsedDyingTime;
          private float elapsedKillTime;

          // For the "Shot Word" effect.
          private List<char> shotLetters = new List<char>();
          private List<Vector2> shotLettersPositions = new List<Vector2>();
          private List<float> shotLettersRotation = new List<float>();

          protected SpriteFont enemyFont;

          protected GameResourceTexture2D blankTexture;
          protected GameResourceTexture2D borderTexture;

          private Vector2 sentencePosition;
          private Rectangle sentenceBorder;

          private bool isSentenceReversed;

          private bool showSentence = true;

          #endregion

          private float shotWordSpeed = 5.0f;
          private float shotWordRotationSpeed = 5.0f;

          /// <summary>
          /// The velocity at which "Shot" characters fling off the screen.
          /// </summary>
          public float ShotWordSpeed
          {
               get { return shotWordSpeed; }
               set { shotWordSpeed = value; }
          }

          /// <summary>
          /// The rate at which "Shot" characters rotate.
          /// </summary>
          public float ShotWordRotationSpeed
          {
               get { return shotWordRotationSpeed; }
               set { shotWordRotationSpeed = value; }
          }

          #region ICameraTrackable Fields

          public Vector3 TrackedPosition
          {
               get { return this.Position; }
               set { this.Position = value; }
          }

          public Matrix TrackedWorldMatrix
          {
               get { return this.WorldPosition; }
          }

          #endregion

          #region Properties

          /// <summary>
          /// Gets the EnemyManager currently manage the Enemy.
          /// </summary>
          public EnemyManager EnemyManager
          {
               get { return enemyManager; }
               set { enemyManager = value; }
          }

          /// <summary>
          /// A 3D Model representing the physical appearance of the Typing Enemy.
          /// </summary>
          public Avatar Avatar
          {
               get { return avatar; }
               set { avatar = value; }
          }

          /// <summary>
          /// Name of the sound file used to represent sound played when enemy is shot.
          /// </summary>
          public string ShotSound
          {
               get { return shotSound; }
               set { shotSound = value; }
          }

          /// <summary>
          /// Name of the sound file used to represent sound played when enemy is missed.
          /// </summary>
          public string MissedSound
          {
               get { return missedSound; }
               set { missedSound = value; }
          }

          /// <summary>
          /// Name of the sound file used to represent sound played when enemy dies.
          /// </summary>
          public string DeathSound
          {
               get { return deathSound; }
               set { deathSound = value; }
          }

          /// <summary>
          /// Name of the sound file used to represent sound played when enemy escapes.
          /// </summary>
          public string EscapeSound
          {
               get { return escapeSound; }
               set { escapeSound = value; }
          }

          /// <summary>
          /// Returns true if the Enemy is alive, false otherwise.
          /// </summary>
          public bool IsAlive
          {
               get { return isAlive; }
               set { isAlive = value; }
          }

          /// <summary>
          /// Returns true if the Enemy is not alive, false otherwise.
          /// </summary>
          public bool IsDead
          {
               get { return !isAlive; }
               set { isAlive = !value; }
          }

          /// <summary>
          /// Returns true if the Enemy is dying (fainting to the ground), false otherwise.
          /// </summary>
          public bool IsDying
          {
               get { return isDying; }
               set { isDying = value; }
          }

          /// <summary>
          /// Returns true if the Enemy is being targetted, false otherwise.
          /// </summary>
          public bool IsTarget
          {
               get { return isTarget; }
               set { isTarget = value; }
          }

          /// <summary>
          /// Returns true if the player has Missed the Enemy at least once.
          /// Affects the player's score and multiplyers.
          /// </summary>
          public bool WasMissed
          {
               get { return wasMissed; }
               set { wasMissed = value; }
          }

          /// <summary>
          /// Returns true if the player has a Perfect Kill against the Enemy.
          /// Affects the player's score and multipliers
          /// </summary>
          public bool IsPerfectKill
          {
               get { return isPerfectKill; }
               set { isPerfectKill = value; }
          }

          /// <summary>
          /// Returns true if the player has a Speed Kill against the Enemy.
          /// Affects the player's score and multipliers.
          /// </summary>
          public bool IsSpeedKill
          {
               get { return isSpeedKill; }
               set { isSpeedKill = value; }
          }

          /// <summary>
          /// Returns true if the enemy was Killed by the Player (and not other means),
          /// false otherwise.
          /// </summary>
          public bool WasKilledByPlayer
          {
               get { return wasKilledByPlayer; }
               set { wasKilledByPlayer = value; }
          }

          /// <summary>
          /// The Velocity the enemy is traveling at.
          /// </summary>
          public float Speed
          {
               get { return yVelocity; }
               set { yVelocity = value; }
          }

          /// <summary>
          /// Position in world space of the bottom center of this enemy.
          /// </summary>
          public Vector3 Position
          {
               get { return position; }
               set { position = value; }
          }

          /// <summary>
          /// Position in world space of the bottom center of this enemy.
          /// </summary>
          public Matrix WorldPosition
          {
               get { return worldPosition; }
               set { worldPosition = value; }
          }

          /// <summary>
          /// A List of TypingWord objects wielded by the enemy.
          /// </summary>
          public List<TypingWord> WordList
          {
               get { return wordList; }
               set { wordList = value; }
          }

          /// <summary>
          /// The amount of base points awarded to the player when defeated.
          /// </summary>
          public int BasePoints
          {
               get { return basePoints; }
               set { basePoints = value; }
          }

          /// <summary>
          /// The amount of extra points awarded to the player when the Enemy
          // is defeated with a Speed or Perfect Bonus.
          /// </summary>
          public int BonusPoints
          {
               get { return bonusPoints; }
               set { bonusPoints = value; }
          }

          /// <summary>
          /// The requirement needed for the player to earn a Speed Bonus.
          /// </summary>
          public float SpeedBonusRequirement
          {
               get { return speedBonusRequirement; }
               set { speedBonusRequirement = value; }
          }

          /// <summary>
          /// The elapsed time since the enemy was first Targetted.
          /// </summary>
          public float ElapsedTime
          {
               get { return elapsedTime; }
               set { elapsedTime = value; }
          }

          /// <summary>
          /// The elapsed time since the Enemy was first Defeated.
          /// </summary>
          public float ElapsedDyingTime
          {
               get { return elapsedDyingTime; }
               set { elapsedDyingTime = value; }
          }

          /// <summary>
          /// The elapsed time since the Enemy...Uh, not sure!
          /// </summary>
          public float ElapsedKillTime
          {
               get { return elapsedKillTime; }
               set { elapsedKillTime = value; }
          }

          /// <summary>
          /// Whether or not the Enemies TypingWord Text 
          /// is reversed.
          /// </summary>
          public bool IsSentenceReversed
          {
               get { return isSentenceReversed; }
               set { isSentenceReversed = value; }
          }

          /// <summary>
          /// Gets or sets the position of the Enemy's sentence.
          /// </summary>
          public Vector2 SentencePosition
          {
               get { return sentencePosition; }
               set { sentencePosition = value; }
          }

          /// <summary>
          /// Gets or sets the Rectangle which borders an Enemy's Sentence.
          /// </summary>
          public Rectangle SentenceBorder
          {
               get { return sentenceBorder; }
               set { sentenceBorder = value; }
          }

          /// <summary>
          /// Whether or not the Enemy's Sentence should be shown (drawn).
          /// </summary>
          public bool ShowSentence
          {
               get { return showSentence; }
               set { showSentence = value; }
          }

          #endregion

          /// <summary>
          /// Gets or sets the amount of characters that have been
          /// typed in this Enemy's WordList.
          /// 
          /// This number is updated in OnSentenceComplete().
          /// Therefore it is only updated when a kill (for most enemies) or
          /// a sub-kill (for bosses and deflatable enemies) is obtained.
          /// </summary>
          public int CharactersTyped
          {
               get { return charactersTyped; }
               set { charactersTyped = value; }
          }

          private int charactersTyped;

          /// <summary>
          /// Gets the total count of characters in 
          /// this Enemy's entire WordList.
          /// </summary>
          public int CharacterCount
          {
               get
               {
                    int count = 0;

                    foreach (TypingWord typingWord in WordList)
                    {
                         count += typingWord.CharacterCount;
                    }

                    return count;
               }
          }

          #region Initialization

          /// <summary>
          /// Instanciates a TypingEnemy.
          /// </summary>
          /// <param name="game">The current Game instance.</param>
          /// <param name="enemyManager">The EnemyManager to handle this TypingEnemy.</param>
          public TypingEnemy(Game game, EnemyManager enemyManager)
               : base(game)
          {
               this.Position = new Vector3(0);
               this.Position = new Vector3(0, 0, 0);
               this.Speed = 0.0f;
               this.IsAlive = true;
               this.IsTarget = false;
               this.WasMissed = false;
               this.SpeedBonusRequirement = 8.0f;
               this.WordList = new List<TypingWord>();
               this.ShotSound = "KeyPress";
               this.MissedSound = "Mistype";
               this.DeathSound = "MenuScroll";
               this.EscapeSound = "";
               this.EnemyManager = enemyManager;

               // Create the Enemy's Avatar.
               this.Avatar = new Avatar(game);
               this.Avatar.Scale = 1.75f;

               // NEW AS OF 3-7-2011: JUST TESTING
               this.Avatar.LoadRandomAvatar();
          }

          /// <summary>
          /// Initializes the enemy manager component.
          /// </summary>
          public override void Initialize()
          {
               base.Initialize();
          }

          /// <summary>
          /// Loads a particular enemy sprite sheet and sounds.
          /// </summary>
          protected override void LoadContent()
          {
               Avatar.PlayAnimation(AnimationType.Run, true);

               enemyFont = TextManager.MenuFont;

               blankTexture = ResourceManager.LoadTexture(@"Textures\Blank Textures\Blank_Rounded_Small_WithBorder");
               borderTexture = ResourceManager.LoadTexture(@"Textures\Blank Textures\Border_Small");
          }

          /// <summary>
          /// Called when graphics resources need to be unloaded. Override this method
          /// to unload any component-specific graphics resources.
          /// </summary>
          protected override void UnloadContent()
          {
               //this.EnemyManager.Dispose();
               //this.EnemyManager = null;

               if (this.Avatar != null)
               {
                    // Remove reference to the Avatar Animation.
                    this.Avatar.AvatarAnimation = null;

                    // Dispose Avatar resources if it's not null.
                    this.Avatar.Dispose();

                    // Set the Avatar to null to free up memory.
                    this.Avatar = null;
               }

               // Clear the WordList List.
               if (this.WordList != null)
                    this.WordList.Clear();

               // Clear the shotLetters List.
               if (shotLetters != null)
                    shotLetters.Clear();

               // Clear the shotLettersPositions List.
               if (shotLettersPositions != null)
                    shotLettersPositions.Clear();

               // Clear the shotLettersRotation List.
               if (shotLettersRotation != null)
                    shotLettersRotation.Clear();

               // Set the Lists to null to free up memory.
               this.WordList = null;
               this.shotLetters = null;
               this.shotLettersPositions = null;
               this.shotLettersRotation = null;

               //this.Dispose(true);
          }

          /// <summary>
          /// Call this manually to unload TypingEnemy resources.
          /// </summary>
          public void Unload()
          {
               // Simply call the overridden UnloadContent.
               UnloadContent();
          }

          #endregion

          #region Handle Input

          bool shift = false;

          public virtual void HandleInput(InputState input, PlayerIndex? ControllingPlayer, PlayerIndex playerIndex)
          {
               if (!this.IsTarget)
                    return;

               shift = false;

               if (input.IsKeyDown(Keys.LeftShift, null, out playerIndex)
                    || input.IsKeyDown(Keys.RightShift, null, out playerIndex))
               {
                    // Don't make it count against the player.
                    shift = true;
               }

               // If a Key was pressed, and If we have not finish the current target's word...
               if (input.IsNewKeyPress(null, out playerIndex) && this.WordList != null && this.WordList[0].Length > 0)  // Added the this.WordList != null part 3-1-2011
               {
                    // Check if the Key pressed is the key necessary to Hurt the Enemy.
                    if (input.IsNewKeyPress(this.WordList[0].KeyList[0], null, out playerIndex))
                    {
                         // If the word requires a Shift...
                         if (!KeysHelper.IsShiftRequired(this.WordList[0].Text[0]) ||
                              (KeysHelper.IsShiftRequired(this.WordList[0].Text[0]) && shift))
                         {
                              // Hit the enemy!
                              this.OnHit();

                              // If the Enemy's Word is of length 0 or less, he's dead.
                              if (this.WordList[0].Length <= 0 || !this.IsAlive)
                              {
                                   // New for WPM logic
                                   this.CharactersTyped += this.WordList[0].CharacterCount;
                                   // End

                                   this.WordList.RemoveAt(0);

                                   this.OnSentenceComplete();

                                   if (this.WordList.Count <= 0)
                                   {
                                        this.OnKilled();
                                   }
                              }
                         }

                         else if (KeysHelper.IsShiftRequired(this.WordList[0].Text[0]) && !shift)
                         {
                              this.OnMiss();
                         }
                    }

                    // Else we must have Missed the Enemy, losing our Bonus chance.
                    else
                    {
                         if (!input.IsNewKeyPress(Keys.LeftShift, null, out playerIndex)
                              && !input.IsNewKeyPress(Keys.RightShift, null, out playerIndex))
                         {
                              this.OnMiss();
                         }
                    }
               }
          }

          #endregion

          #region Update

          /// <summary>
          /// Updates the Enemy.
          /// 
          /// Checks if the Enemy is dying, sets the Enemy's Avatar 
          /// World Matrix appropriately, elapses a timer variable if the Enemy
          /// is currently targetted, and finally updates the Enemy's Avatar.
          /// </summary>
          public override void Update(GameTime gameTime)
          {
               //if (Avatar == null)
               //     return;

               // Update their Shot-off Characters.
               UpdateShotWords();

               // If this Enemy is Dying...
               if (this.IsDying)
               {
                    // Increment how long he's been in his Dying state!
                    elapsedDyingTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // And update his Avatar!
                    Avatar.Update(gameTime);

                    // Return because we don't want to update his position or increase his ElapsedTime.
                    return;
               }

               // Update the Enemy's Position (and thus Avatar Position).
               this.Position = new Vector3(this.Position.X, 0, this.Position.Z - this.Speed / 25f);
               this.Avatar.Position = this.Position;
               this.WorldPosition = Matrix.CreateRotationY(MathHelper.ToRadians(0.0f)) *
                                    Matrix.CreateTranslation(this.Position) *
                                    Matrix.CreateScale(1f);

               // If this Enemy is the Target...
               if (this.IsTarget)
               {
                    // Increment how long we've been fighting this Enemy's Current Sentence!
                    this.ElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // Increment how long we've been fighting / targetting this Enemy!
                    this.ElapsedKillTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
               }

               Avatar.Update(gameTime);
          }

          /// <summary>
          /// Updates the Shot Words list.
          /// Updates their position and rotation, and removes them
          /// when they fall off the screen.
          /// </summary>
          public void UpdateShotWords()
          {
               for (int i = 0; i < shotLetters.Count; i++)
               {
                    // Update the Shot Words positions; shift them left-ward.
                    shotLettersPositions[i] = new Vector2(shotLettersPositions[i].X - shotWordSpeed, shotLettersPositions[i].Y);

                    // Update the Shot Words rotations; rotate them by 5 degrees.
                    shotLettersRotation[i] += MathHelper.ToRadians(shotWordRotationSpeed);

                    // If they are no longer visible, remove them.
                    if (shotLettersPositions[i].X < -10)
                    {
                         shotLetters.RemoveAt(0);
                         shotLettersPositions.RemoveAt(0);
                         shotLettersRotation.RemoveAt(0);
                    }
               }
          }

          #endregion

          #region Draw

          /// <summary>
          /// Draws the Typing Enemy; the Avatar itself, along with the word.
          /// </summary>
          public virtual void DrawWithoutCamera(GameTime gameTime, SpriteFont font, Vector3 fakeCameraPosition, Vector3 renderPosition)
          {
               //if (Avatar == null)
               //     return;

               MySpriteBatch.Begin(BlendState.AlphaBlend);

               // Draw the enemy (his / her avatar).
               Avatar.DrawToScreen(gameTime, fakeCameraPosition, renderPosition);

               MySpriteBatch.End();
          }

          /// <summary>
          /// Draws the Typing Enemy; the Avatar itself, 
          /// along with the Enemy's sentence.
          /// </summary>
          public virtual void Draw(GameTime gameTime, SpriteFont font)
          {
               //if (Avatar == null)
               //     return;

               MySpriteBatch.Begin(BlendState.AlphaBlend);

               // Brighten up the enemy if it is the targetted enemy.
               if (this.IsTarget)
               {
                    Avatar.LightDirection = new Vector3(-1, 1, 1);
                    Avatar.LightColor = Color.CornflowerBlue.ToVector3();
                    Avatar.AmbientLightColor = Color.White.ToVector3();
               }

               // Draw the enemy (his / her avatar).
               Avatar.Draw(gameTime);

               // If the enemy is dying...
               if (this.IsDying)
               {
                    // If Perfect Kill, alert the player.
                    if (this.IsPerfectKill)
                         DrawPerfectDisplay(gameTime);

                    // If Speed Kill, alert the player.
                    if (this.IsSpeedKill)
                         DrawSpeedDisplay(gameTime);
               }

               // Draw the enemy's word.
               //DrawWords(gameTime, font);

               // Draw the enemy's characters flinging off the screen.
               //DrawShotWords(gameTime, font);

               MySpriteBatch.End();
          }

          #region Helper Draw Methods - Draw Words, Draw Shot Words, Draw Displays

          /// <summary>
          /// Draws the Typing Enemy's word.
          /// </summary>
          public void DrawWords(GameTime gameTime, SpriteFont font)
          {
               if (!this.ShowSentence)
                    return;

               if (this.IsDead)
                    return;

               string word = "";

               if (this.WordList.Count > 0)
                    word = this.WordList[0].Text;

               // Test space showing
               if (word.Length > 0)
               {
                    if (word[0] == ' ')
                    {
                         string oldWord = word;

                         string newWord = word.Substring(1);

                         word = "_" + newWord;
                    }
               }
               // End test

               Vector2 textPosition = new Vector2(Position.X, Position.Y);
               Vector2 textOrigin = new Vector2(0, 0);
               Vector3 textPosition3D = GraphicsHelper.ToScreenspaceVector3(this.Avatar.WorldMatrix);

               textPosition = GraphicsHelper.ConvertToScreenspace(this.Avatar.WorldMatrix);
               textPosition.Y += 25f;

               float fontSize = AvatarTypingGameSettings.TextSize;

               if (this.Position.Z > CameraManager.ActiveCamera.Position.Z)
               {
                    if (IsTarget)
                    {
                         fontSize *= 2.0f;

                         Rectangle wordBorder = SentenceBorder;

                         textPosition = sentencePosition;
    
                         MySpriteBatch.Draw(borderTexture.Texture2D, wordBorder, Color.White);
                         MySpriteBatch.Draw(blankTexture.Texture2D, wordBorder, this.Color * 1f);
                         TextManager.DrawCentered(true, font, TextManager.WrapText(word, 200f), textPosition, Color.Red, fontSize); // Color.Red
                    }

                    else
                    {
                         Rectangle wordBorder = SentenceBorder;

                         textPosition = sentencePosition;

                         MySpriteBatch.Draw(borderTexture.Texture2D, wordBorder, Color.White);
                         MySpriteBatch.Draw(blankTexture.Texture2D, wordBorder, this.Color * (100f / 255f));

                         TextManager.DrawCentered(true, font, TextManager.WrapText(word, 200f), textPosition, Color.White, fontSize);
                    }
               }
          }

          /// <summary>
          /// Draws the Typing Enemy's typed characters with a "shot-off" effect.
          /// </summary>
          public void DrawShotWords(GameTime gameTime, SpriteFont font)
          {
               for (int i = 0; i < shotLetters.Count; i++)
               {
                    Vector2 textPosition = shotLettersPositions[i];
                    Vector2 textOrigin = new Vector2(0, 0);
                    string word = shotLetters[i].ToString();
                    float fontSize = AvatarTypingGameSettings.TextSize;

                    TextManager.DrawString(font, word, textPosition, Color.Red, shotLettersRotation[i],//(shotLettersRotation[i] += MathHelper.ToRadians(5.0f)),
                       textOrigin, fontSize * 2.0f);
               }
          }

          /// <summary>
          /// Draws "Perfect Kill!" to the screen upon an Enemy's death,
          /// if a Perfect Kill was obtained.
          /// </summary>
          public void DrawPerfectDisplay(GameTime gameTime)
          {
               TextManager.DrawCentered(false, ScreenManager.Font, "+ Perfect Kill!", new Vector2(250, 250), Color.CornflowerBlue, 0.75f);
          }

          /// <summary>
          /// Draws "Speed Kill!" to the screen upon an Enemy's death,
          /// if a Speed Kill was obtained.
          /// </summary>
          public void DrawSpeedDisplay(GameTime gameTime)
          {
               TextManager.DrawCentered(false, ScreenManager.Font, "+ Speed Kill!", new Vector2(250, 280), Color.LightGreen, 0.75f);
          }

          #endregion

          #endregion

          #region Collision Methods

          /// <summary>
          /// Event triggered if a collision is found.
          /// Returns true if the Enemy escaped from the player.
          /// </summary>
          /// <returns></returns>
          public virtual bool IsCollision(Vector3 playerPosition)
          {
               if (this.Position.Z < (playerPosition.Z - 1.0f) && this.IsAlive)
               {
                    return true;
               }

               return false;
          }

          /// <summary>
          /// Event triggered during a collision.
          /// Causes the colliding enemy to damage the player.
          /// </summary>
          /// <param name="thePlayer"></param>
          public virtual void OnCollide(Player thePlayer)
          {
               // Don't do anything if Player is null.
               if (thePlayer == null)
                    return;

               // Otherwise, subtract 1 from the Player's Health.
               thePlayer.Health -= this.DamageDoneToPlayer;

               // If the Player has no Health, he's dead!
               if (thePlayer.Health <= 0)
               {
                    this.IsAlive = false;
               }
          }

          #endregion

          #region Check For Bonus Methods

          /// <summary>
          /// Checks if a Perfect Bonus was obtained from this Enemy.
          /// If True, Base Points is incremented by the Bonus amount.
          /// </summary>
          /// <returns>Returns True if WasMissed is false.</returns>
          public virtual bool CheckPerfectBonus()
          {
               if (this.WasMissed)
                    return false;

               else
               {
                    BasePoints += BonusPoints;
                    return true;
               }
          }

          /// <summary>
          /// Checks if a Speedy Bonus was obtained from this Enemy.
          /// If True, Base Points is incremented by Bonus amount.
          /// </summary>
          /// <returns>
          /// Returns True if the player typed the sentence
          /// at a rate > 5 characters per second.
          /// </returns>
          public virtual bool CheckSpeedBonus()
          {
               if (this.ElapsedTime > this.SpeedBonusRequirement)
                    return false;

               else
               {
                    BasePoints += BonusPoints;
                    return true;
               }
          }

          #endregion

          #region On Hit, Miss, Killed and Target Methods

          /// <summary>
          /// Called whenever an Enemy is hit.
          /// 
          /// Does all logic corresponding to being hit, such as
          /// getting rid of the shot character, flinging it off screen, 
          /// playing the Shot Sound effect, etc.
          /// </summary>
          public virtual void OnHit()
          {
               Vector2 textPosition = new Vector2();

               textPosition = GraphicsHelper.ConvertToScreenspace(this.Avatar.WorldMatrix);//this.WorldPosition);

               string word = this.WordList[0].Text;

               Vector2 stringCenter = enemyFont.MeasureString(word) / 2.0f;

               // TESTING
               textPosition = sentencePosition;
               // END TESTING

               // Now subtract the string center from the text position to find correct position.
               textPosition.X = (int)(textPosition.X - stringCenter.X);
               textPosition.Y = (int)(textPosition.Y - stringCenter.Y);

               this.shotLetters.Add(this.WordList[0].Text[0]);
               this.shotLettersPositions.Add(textPosition);
               this.shotLettersRotation.Add(0.0f);

               // The Enemy's Word is now minus the first character.
               this.WordList[0].Text =
                    (this.WordList[0].Text).Substring(1, this.WordList[0].Text.Length - 1);

               AudioManager.PlayCue(this.ShotSound);

               // TO DO: Flinch Animation.
               //this.Avatar.Blink_Left();

               Player.FiredBullet = true;
               Player.TargetLocation = this.Position;
          }

          /// <summary>
          /// Called whenever an Enemy is missed.
          /// 
          /// Does all logic corresponding to missing, such as
          /// setting WasMissed to true, playing hte Missed Sound effect,
          /// and making the Enemy's Avatar laugh.
          /// </summary>
          public virtual void OnMiss()
          {
               this.WasMissed = true;
               AudioManager.PlayCue(this.MissedSound);

               // TO DO: Laugh Animation.
               this.Avatar.Laugh();
          }

          /// <summary>
          /// Called whenever an Enemy's sentence is complete.
          /// 
          /// Does all logic corresponding to completing a sentence, 
          /// such as checking for Perfect / Speedy Bonuses, incrementing
          /// or resetting Accuracy / Speed Streaks, incrementing the player's
          /// Combo Meter, etc.
          /// </summary>
          public virtual void OnSentenceComplete()
          {
               // Check if the enemy was killed perfectly.
               if (CheckPerfectBonus())
               {
                    IsPerfectKill = true;

                    AvatarTypingGame.AwardData.AccuracyStreak++;
               }

               else
               {
                    IsPerfectKill = false;

                    AvatarTypingGame.AwardData.AccuracyStreak = 0;
               }

               // Check if the enemy was killed quickly.
               if (CheckSpeedBonus())
               {
                    IsSpeedKill = true;

                    AvatarTypingGame.AwardData.SpeedStreak++;
               }

               else
               {
                    isSpeedKill = false;

                    AvatarTypingGame.AwardData.SpeedStreak = 0;
               }


               // Award the Player his Combo Points.
               this.EnemyManager.currentPlayer.ComboMeter += (0.2f) *
                    (AvatarTypingGame.AwardData.AccuracyStreak +
                    AvatarTypingGame.AwardData.SpeedStreak) + 1;

               // If the Combo Meter is full, award Extra Life!
               if (this.EnemyManager.currentPlayer.ComboMeter >= 100f)
               {
                    float remainder = 0;

                    do
                    {
                         // This is needed so we can fill the new ComboMeter up
                         // with the remainder of the points earned.
                         remainder =
                              this.EnemyManager.currentPlayer.ComboMeter - 100f;

                         this.EnemyManager.currentPlayer.EarnedExtraLife = true;
                         this.EnemyManager.currentPlayer.ComboMeter = remainder;
                         this.EnemyManager.currentPlayer.Health++;
                    }
                    while (remainder >= 100f); // Allows multiple lives to be earned per update check.
               }
          }

          /// <summary>
          /// Called whenever an Enemy is Killed.
          /// 
          /// Does all logic corresponding to an Enemy being Killed,
          /// such as incrementing the player's Total Kills, and Total Speedy
          /// / Perfect Kills (if earned). Sets the Enemy as IsAlive = false,
          /// IsTarget = false, IsDying = true, and IsActive = false.
          /// 
          /// Finally, removes the Enemy's spawn point from the spawn list, 
          /// plays the Enemy's Death Sound effect, and plays the Faint animation.
          /// </summary>
          public virtual void OnKilled()
          {
               // Add +1 to the player's Total Kills!
               AvatarTypingGame.AwardData.TotalKills++;

               // Note: IsPerfectKill set from OnSentenceComplete().
               if (IsPerfectKill)
               {
                    AvatarTypingGame.AwardData.TotalPerfectKills++;
               }

               // Note: IsSpeedKill set from OnSentenceComplete().
               if (IsSpeedKill)
               {
                    AvatarTypingGame.AwardData.TotalSpeedKills++;
               }

               // The enemy is no longer alive, and thus is not the target.
               this.IsAlive = false;
               this.IsTarget = false;
               this.IsDying = true;

               this.IsActive = false;

               enemyManager.usedSpawnLocations.Remove(SpawnLocation);

               AudioManager.PlayCue(this.DeathSound);

               // Make the enemy's avatar begin the Faint animation.
               this.Faint();
          }

          /// <summary>
          /// Called whenever an Enemy Escapes.
          /// 
          /// Does all logic corresponding to Escaping, 
          /// such as setting IsActive = false and removing
          /// the Enemy's spawn position from the spawn list.
          /// </summary>
          public virtual void OnEscaped()
          {
               this.IsActive = false;

               enemyManager.usedSpawnLocations.Remove(SpawnLocation);
          }

          /// <summary>
          /// Called whenever an Enemy is Targetted.
          /// 
          /// Does all logic corresponding to being Taretted, such
          /// as setting IsTarget = true and making the Enemy's
          /// Avatar laugh.
          /// </summary>
          public virtual void OnTargetted()
          {
               this.IsTarget = true;

               // TO DO: Startled Animation.
               this.Avatar.Laugh();
          }

          /// <summary>
          /// Called whenever an Enemy is UnTargetted.
          /// 
          /// Does all logic corresponding to being UnTaretted, such
          /// as setting IsTarget = false.
          /// </summary>
          public virtual void OnUntargetted()
          {
               this.IsTarget = false;
          }

          #endregion

          #region OnGetExploded Method

          public virtual void OnExplode(Vector2 explosionOrigin)
          {
               Vector2 textPosition = new Vector2();

               string word = this.WordList[0].Text;

               textPosition = sentencePosition;

               float rotation = 0f;

               foreach (char aChar in word)
               {
                    this.shotLetters.Add(aChar);
                    this.shotLettersPositions.Add(textPosition);
                    this.shotLettersRotation.Add(rotation);

                    rotation += MathHelper.ToRadians(15.0f);
                    textPosition = new Vector2(textPosition.X + 10f, textPosition.Y);
               }

               this.ShotWordRotationSpeed = 5f;
               this.ShotWordSpeed = 10f;

               // BURN THEM TO A CRISP!!!
               this.Avatar.LightColor = new Vector3(0, 0, 0);
               this.Avatar.LightDirection = new Vector3(0, 0, 0);
               this.Avatar.AmbientLightColor = new Vector3(-1, -1, -1);
          }

          #endregion

          #region Generate Enemy's Word List

          // New: TESTING PHASE
          public string GenerateLetter(bool isUnique)
          {
               string wordSelection = LetterDatabase.Alphabet[random.Next(LetterDatabase.Alphabet.Length)];

               bool keepSearching = isUnique;

               do
               {
                    keepSearching = false;

                    for (int i = 0; i < enemyManager.Size; i++)
                    {
                         // We will be accessing WordList[0], 
                         // so make sure it even exists!
                         if (enemyManager.GetEnemyAt(i).WordList.Count <= 0)
                              continue;

                         if (
                              // If this word equals another word, we don't want it!
                              wordSelection == enemyManager.GetEnemyAt(i).WordList[0].Text

                              // Since the waves are large compared to the size of possible Letters, we need this check.
                              && enemyManager.Size <= SentenceDatabase.Sentences.Length

                              && isUnique
                            )
                         {
                              keepSearching = true;
                              wordSelection = SentenceDatabase.Sentences[random.Next(SentenceDatabase.Sentences.Length)];
                              break;
                         }
                    }
               }
               while (keepSearching);

               return wordSelection;
          }

          /// <summary>
          /// Generates a TypingWord of less than the specified amount of characters.
          /// 
          /// *Note: This is used by Deflatable Enemies only.
          /// </summary>
          /// <param name="characterCount">The maximum amount of characters the TypingWord should contain.</param>
          /// <param name="isUnique">Whether or not we should accept words that are already on screen.</param>
          /// <returns>A random string from the Sentence Database, less than the specified length.</returns>
          public string GenerateSmallWord(int characterCount, bool isUnique)
          {
               if (AvatarTypingGameSettings.Difficulty == Difficulty.Easy)
               {
                    // Easy Mode, so generate a small, chat-pad compatible word.
                    return GenerateSmallChatPadWord(characterCount, isUnique);
               }

               string wordSelection = SentenceDatabase.Sentences[random.Next(SentenceDatabase.Sentences.Length)];

               bool keepSearching = isUnique;

               do
               {
                    keepSearching = false;

                    // If the word is > 12 characters, we don't want it!
                    if (wordSelection.Length > characterCount)
                    {
                         keepSearching = true;
                         wordSelection = SentenceDatabase.Sentences[random.Next(SentenceDatabase.Sentences.Length)];
                         //break;

                         // Break makes you leave the do-while loop! Why is it here?!?!?!
                    }

                    for (int i = 0; i < enemyManager.Size; i++)
                    {
                         // We will be accessing WordList[0], 
                         // so make sure it even exists!
                         if (enemyManager.GetEnemyAt(i).WordList.Count <= 0)
                              continue;

                         if (
                              // If this word equals another word, we don't want it!
                              wordSelection == enemyManager.GetEnemyAt(i).WordList[0].Text

                              // Since waves are so small, we no longer need this check.
                              //&& enemyManager.Size <= SentenceDatabase.Sentences.Length
                              && isUnique

                              // Or if the 1st letter is already on-screen, we don't want it!
                              || wordSelection[0] == enemyManager.GetEnemyAt(i).WordList[0].Text[0]

                              // Or if the word is > 12 characters, we don't want it!
                              //|| wordSelection.Length > characterCount
                            )
                         {
                              keepSearching = true;
                              wordSelection = SentenceDatabase.Sentences[random.Next(SentenceDatabase.Sentences.Length)];
                              break;
                         }
                    }
               }
               while (keepSearching);

               return wordSelection;
          }

          /// <summary>
          /// Generates a TypingWord of less than the specified amount of characters.
          /// The generated TypingWord is also compatible with Chat Pads (i.e no punctuation).
          /// 
          /// *Note: This is used by Deflatable Enemies only.
          /// </summary>
          /// <param name="characterCount">The maximum amount of characters the TypingWord should contain.</param>
          /// <param name="isUnique">Whether or not we should accept words that are already on screen.</param>
          /// <returns>A random string from the Sentence Database, less than the specified length and
          /// compatible with a Chat Pad.</returns>
          public string GenerateSmallChatPadWord(int characterCount, bool isUnique)
          {
               string wordSelection = "";

               wordSelection = SentenceDatabase.Sentences[random.Next(SentenceDatabase.Sentences.Length)];

               bool keepSearching = isUnique;

               do
               {
                    keepSearching = false;

                    // If the word is > 12 characters, we don't want it!
                    if (wordSelection.Length > characterCount)//12)
                    {
                         keepSearching = true;
                         wordSelection = SentenceDatabase.Sentences[random.Next(SentenceDatabase.Sentences.Length)];
                         //break;
                    }

                    for (int i = 0; i < enemyManager.Size; i++)
                    {
                         if (enemyManager.GetEnemyAt(i).WordList.Count <= 0)
                              continue;

                         if (wordSelection == enemyManager.GetEnemyAt(i).WordList[0].Text && enemyManager.Size <= SentenceDatabase.Sentences.Length
                             || wordSelection[0] == enemyManager.GetEnemyAt(i).WordList[0].Text[0]
                             || wordSelection.Length > characterCount)//12)
                         {
                              keepSearching = true;
                              wordSelection = SentenceDatabase.Sentences[random.Next(SentenceDatabase.Sentences.Length)];
                              break;
                         }
                    }
               }
               while (keepSearching);

               // Replace any untypeable character.
               wordSelection = wordSelection.Replace("\"", "");
               wordSelection = wordSelection.Replace("'", "");
               wordSelection = wordSelection.Replace(",", "");
               wordSelection = wordSelection.Replace("?", ".");
               wordSelection = wordSelection.Replace("-", " ");
               //wordSelection = wordSelection.Replace(".", "");
               wordSelection = wordSelection.Replace("!", "");
               wordSelection = wordSelection.Replace("&", "and");

               return wordSelection;
          }

          /// <summary>
          /// Generates a sentence (with or without Chat Pad compability).
          /// The sentence is from the Sentence Database only.
          /// </summary>
          /// <param name="isUnique"></param>
          /// <returns></returns>
          public string GenerateWord(bool isUnique)
          {
               if (AvatarTypingGameSettings.Difficulty == Difficulty.Easy)
               {
                    return GenerateChatPadWord(isUnique);
               }

               string wordSelection = SentenceDatabase.Sentences[random.Next(SentenceDatabase.Sentences.Length)];

               bool keepSearching = isUnique;

               do
               {
                    keepSearching = false;

                    for (int i = 0; i < enemyManager.Size; i++)
                    {
                         if (enemyManager.GetEnemyAt(i).WordList.Count <= 0)
                              continue;

                         if (
                              (wordSelection == enemyManager.GetEnemyAt(i).WordList[0].Text
                              && enemyManager.Size <= SentenceDatabase.Sentences.Length
                              && isUnique)

                              ||
                              (wordSelection[0] == enemyManager.GetEnemyAt(i).WordList[0].Text[0])
                            )
                         {
                              keepSearching = true;
                              wordSelection = SentenceDatabase.Sentences[random.Next(SentenceDatabase.Sentences.Length)];
                              break;
                         }
                    }
               }
               while (keepSearching);

               return wordSelection;
          }

          /// <summary>
          /// Generates a Chat Pad compatible sentence.
          /// The sentence is from the Sentence Database only.
          /// </summary>
          /// <param name="isUnique"></param>
          /// <returns></returns>
          public string GenerateChatPadWord(bool isUnique)
          {
               string wordSelection = "";

               wordSelection = SentenceDatabase.Sentences[random.Next(SentenceDatabase.Sentences.Length)];

               bool keepSearching = isUnique;

               do
               {
                    keepSearching = false;

                    for (int i = 0; i < enemyManager.Size; i++)
                    {
                         if (enemyManager.GetEnemyAt(i).WordList.Count <= 0)
                              continue;

                         if (wordSelection == enemyManager.GetEnemyAt(i).WordList[0].Text && enemyManager.Size <= SentenceDatabase.Sentences.Length
                             || wordSelection[0] == enemyManager.GetEnemyAt(i).WordList[0].Text[0])
                         {
                              keepSearching = true;
                              wordSelection = SentenceDatabase.Sentences[random.Next(SentenceDatabase.Sentences.Length)];
                              break;
                         }
                    }
               }
               while (keepSearching);

               // Replace any untypeable character.
               wordSelection = wordSelection.Replace("\"", "");
               wordSelection = wordSelection.Replace("'", "");
               wordSelection = wordSelection.Replace(",", "");
               wordSelection = wordSelection.Replace("?", ".");
               wordSelection = wordSelection.Replace("-", " ");
               //wordSelection = wordSelection.Replace(".", "");
               wordSelection = wordSelection.Replace("!", "");
               wordSelection = wordSelection.Replace("&", "and");

               return wordSelection;
          }

          /// <summary>
          /// Generates a sentence (with or without Chat Pad compability).
          /// The sentence is from either the Sentence Database
          /// or the User Sentence list.
          /// 
          /// *Note: This is what NormalEnemies use.
          /// </summary>
          /// <returns></returns>
          public string GenerateDatabaseOrCustomWord()
          {
               if (AvatarTypingGameSettings.Difficulty == Difficulty.Easy)
               {
                    return GenerateDatabaseOrCustomChatPadWord();
               }

               string wordSelection = "";

               // If the User Sentence list is Empty...
               if (SentenceDatabase.UserSentences.Count <= 0)
               {
                    // Use a Haggle Sentence or Database Sentence?
                    int useHaggleSentences = random.Next(10);

                    // Use a Sentence Database sentence if we get don't get 1, ie 9/10 chance.
                    if (useHaggleSentences != 1)
                    {
                         return GenerateWord(true);
                    }

                    // Use a random Haggle Sentence.
                    int randomWordSelection = random.Next(6);

                    switch (randomWordSelection)
                    {
                         case 0:
                              wordSelection = "Your sentence here";
                              break;
                         case 1:
                              wordSelection = "Create my sentence";
                              break;
                         case 2:
                              wordSelection = "Give me text";
                              break;
                         case 3:
                              wordSelection = "Use the Sentence Creator";
                              break;
                         case 4:
                              wordSelection = "Make me talk";
                              break;
                         case 5:
                              wordSelection = "Edit Me";
                              break;
                    }

                    return wordSelection;
               }

               // Otherwise, grab a random User Sentence 20% of the time.
               int useCustomSentences = random.Next(5);

               // Use a Sentence Database sentence.
               if (useCustomSentences != 0)
               {
                    return GenerateWord(true);
               }

               // Use a User Sentence.
               wordSelection = SentenceDatabase.UserSentences[random.Next(SentenceDatabase.UserSentences.Count)];

               for (int i = 0; i < enemyManager.Size; i++)
               {
                    if (enemyManager.GetEnemyAt(i).WordList.Count <= 0)
                         continue;

                    if (wordSelection == enemyManager.GetEnemyAt(i).WordList[0].Text && enemyManager.Size <= SentenceDatabase.Sentences.Length)
                    {
                         wordSelection = SentenceDatabase.UserSentences[random.Next(SentenceDatabase.UserSentences.Count)];
                         break;
                    }
               }

               return wordSelection;
          }

          /// <summary>
          /// Generates a Chat Pad compatible sentence.
          /// The sentence is from either the Sentence Database
          /// or the User Sentence list.
          /// </summary>
          /// <returns></returns>
          public string GenerateDatabaseOrCustomChatPadWord()
          {
               string wordSelection = "";

               // If the User Sentence list is Empty...
               if (SentenceDatabase.UserSentences.Count <= 0)
               {
                    // Use a Haggle Sentence or one from Database?
                    int useHaggleSentences = random.Next(10);

                    // Use a Sentence Database sentence.
                    if (useHaggleSentences != 1)
                    {
                         wordSelection = GenerateWord(true);

                         // Replace any untypeable character.
                         wordSelection = wordSelection.Replace("\"", "");
                         wordSelection = wordSelection.Replace("'", "");
                         wordSelection = wordSelection.Replace(",", "");
                         wordSelection = wordSelection.Replace("?", ".");
                         wordSelection = wordSelection.Replace("-", " ");
                         //wordSelection = wordSelection.Replace(".", "");
                         wordSelection = wordSelection.Replace("!", "");
                         wordSelection = wordSelection.Replace("&", "and");

                         return wordSelection;
                    }

                    // Use a random Haggle Sentence.
                    int randomWordSelection = random.Next(6);

                    switch (randomWordSelection)
                    {
                         case 0:
                              wordSelection = "Your sentence here";
                              break;
                         case 1:
                              wordSelection = "Create my sentence";
                              break;
                         case 2:
                              wordSelection = "Give me text";
                              break;
                         case 3:
                              wordSelection = "Use the Sentence Creator";
                              break;
                         case 4:
                              wordSelection = "Make me talk";
                              break;
                         case 5:
                              wordSelection = "Edit Me";
                              break;
                    }

                    return wordSelection;
               }

               // NOTE: If we reach this point, the UserSentence List is not empty!!!

               // Otherwise, grab a random User Sentence 20% of the time.
               int useCustomSentences = random.Next(5);

               // Use a Sentence Database sentence.
               if (useCustomSentences != 0)
               {
                    wordSelection = GenerateWord(true);

                    // Replace any untypeable character.
                    wordSelection = wordSelection.Replace("\"", "");
                    wordSelection = wordSelection.Replace("'", "");
                    wordSelection = wordSelection.Replace(",", "");
                    wordSelection = wordSelection.Replace("?", ".");
                    wordSelection = wordSelection.Replace("-", " ");
                    //wordSelection = wordSelection.Replace(".", "");
                    wordSelection = wordSelection.Replace("!", "");
                    wordSelection = wordSelection.Replace("&", "and");

                    return wordSelection;
               }

               // Use a User Sentence.
               wordSelection = SentenceDatabase.UserSentences[random.Next(SentenceDatabase.UserSentences.Count)];

               for (int i = 0; i < enemyManager.Size; i++)
               {
                    if (enemyManager.GetEnemyAt(i).WordList.Count <= 0)
                         continue;

                    if (wordSelection == enemyManager.GetEnemyAt(i).WordList[0].Text && enemyManager.Size <= SentenceDatabase.Sentences.Length)
                    {
                         wordSelection = SentenceDatabase.UserSentences[random.Next(SentenceDatabase.UserSentences.Count)];
                         break;
                    }
               }

               // Replace any untypeable character.
               wordSelection = wordSelection.Replace("\"", "");
               wordSelection = wordSelection.Replace("'", "");
               wordSelection = wordSelection.Replace(",", "");
               wordSelection = wordSelection.Replace("?", ".");
               wordSelection = wordSelection.Replace("-", " ");
               //wordSelection = wordSelection.Replace(".", "");
               wordSelection = wordSelection.Replace("!", "");
               wordSelection = wordSelection.Replace("&", "and");

               return wordSelection;
          }

          #endregion

          #region Animation Actions

          /// <summary>
          /// Plays the custom Run animation for the Enemy's Avatar.
          /// </summary>
          public virtual void Run()
          {
               Avatar.PlayAnimation(AnimationType.Run, true);
          }

          /// <summary>
          /// Plays the Walk animation for the Enemy's Avatar.
          /// 
          /// Also reduced the Enemy's Speed by half.
          /// </summary>
          protected void Walk()
          {
               this.Avatar.PlayAnimation(AnimationType.Walk, true);
               //this.Speed = this.Speed / 2.0f;
          }

          /// <summary>
          /// Plays the custom Faint animation for the Enemy's Avatar.
          /// </summary>
          protected void Faint()
          {
               Avatar.PlayAnimation(AnimationType.Faint, false);
          }

          #endregion

          #region Helper Methods

          /// <summary>
          /// Helper method which calculates the Rectangle 
          /// surrounding an Enemy's sentence. Used for collision
          /// detection to prevent overlapping sentence bubbles.
          /// </summary>
          /// <returns>Returns the Rectangle defining a sentence's bounds.</returns>
          public Rectangle GetSentenceRectangle()
          {
               SpriteFont font = ScreenManager.Font;

               string word = "";

               if (this.WordList.Count > 0)
                    word = this.WordList[0].Text;

               Vector2 textPosition = new Vector2(Position.X, Position.Y);
               Vector2 textOrigin = new Vector2(0, 0);
               Vector3 textPosition3D = GraphicsHelper.ToScreenspaceVector3(this.Avatar.WorldMatrix);

               textPosition = GraphicsHelper.ConvertToScreenspace(this.Avatar.WorldMatrix);
               textPosition.Y += 25f;

               sentencePosition = textPosition;

               float fontSize = AvatarTypingGameSettings.TextSize;

               Rectangle wordBorder = new Rectangle();

               if (this.Position.Z > CameraManager.ActiveCamera.Position.Z)
               {
                    if (IsTarget)
                    {
                         fontSize *= 2.0f;
                    }

                    Vector2 widthHeight = font.MeasureString(TextManager.WrapText(word, 200f)) * fontSize;

                    widthHeight.X += 20f; // 20
                    widthHeight.Y += 10f; // 10

                    // New
                    if (textPosition.Y > 550)
                    {
                         textPosition = new Vector2(textPosition.X, 550);
                         sentencePosition = textPosition;
                    }
                    // ENd

                    wordBorder = new Rectangle(
                         (int)(textPosition.X - (widthHeight.X / 2f) - 5),
                         (int)(textPosition.Y - (12.5f * fontSize) - (widthHeight.Y / 2f) - 4),
                         (int)widthHeight.X,
                         (int)widthHeight.Y);

                    SentenceBorder = wordBorder;
               }

               return wordBorder;
          }

          #endregion
     }
}
