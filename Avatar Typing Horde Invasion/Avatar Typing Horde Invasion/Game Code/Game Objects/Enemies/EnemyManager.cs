#region File Description
//-----------------------------------------------------------------------------
// EnemyManager.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PixelEngine.Graphics;
using PixelEngine.Screen;
#endregion

namespace AvatarTyping
{
     /// <remarks>
     /// The Enemy Manager handles the drawing and updating of all Enemies within it.
     /// It Spawns Waves and adds / removes Enemies when necessary. It also ensures 
     /// Enemies have, for the most part, unique TypingWord.Text properties.
     /// </remarks>
     public class EnemyManager : DrawableGameComponent
     {
          #region SpawnLocation Enum

          public enum SpawnLocation
          {
               Empty,
               FarLeft,
               Left,
               Right,
               FarRight,
          }

          #endregion

          #region Fields

          public List<TypingEnemy> aliveEnemies = new List<TypingEnemy>();
          private List<TypingEnemy> dyingEnemies = new List<TypingEnemy>();

          private Random random = new Random();
          private TypingEnemy currentTarget;
          public bool isTargetSelected;
          private float ElapsedTime = 0.0f;

          // Meta-Data for Player.
          private int enemiesKilled;
          private int enemiesEscaped;
          private int perfectKills;
          private int speedKills;
          private int score;
          private float killTime;
          private uint waveNumber;
          private bool waveDestroyed;

          public int numberOfEnemiesSpawned = 0;

          public List<SpawnLocation> usedSpawnLocations = new List<SpawnLocation>();

          public Player currentPlayer = AvatarTypingGame.CurrentPlayer;

          public const float MIN_SPAWN_Z = 35f;
          public const float MAX_SPAWN_Z = 20f;

          private int numberOfSimultaneousEnemies = 3;

          private AvatarAnimation normalAnimation = new AvatarAnimation(AvatarAnimationPreset.MaleAngry);

          // A List of scripted enemies that should be spawned.
          public List<TypingEnemy> scriptedEnemies = new List<TypingEnemy>();
          public List<EnemyType> scriptedEnemyTypes = new List<EnemyType>();

          public bool IsScriptedWave = true;

          #endregion

          int charactersTyped;

          #region Properties

          /// <summary>
          /// Amount of Enemies killed in this Enemy Manager.
          /// </summary>
          public int EnemiesKilled
          {
               get { return enemiesKilled; }
               set { enemiesKilled = value; }
          }

          /// <summary>
          /// Amount of Enemies who escaped in this Enemy Manager.
          /// </summary>
          public int EnemiesEscaped
          {
               get { return enemiesEscaped; }
               set { enemiesEscaped = value; }
          }

          /// <summary>
          /// Amount of Perfect Kills made against this Enemy Manager.
          /// </summary>
          public int PerfectKills
          {
               get { return perfectKills; }
               set { perfectKills = value; }
          }

          /// <summary>
          /// Amount of Speed Kills made against this Enemy Manager.
          /// </summary>
          public int SpeedKills
          {
               get { return speedKills; }
               set { speedKills = value; }
          }

          /// <summary>
          /// Score obtained on this Enemy Manager.
          /// </summary>
          public int Score
          {
               get { return score; }
               set { score = value; }
          }

          /// <summary>
          /// Amount of Kill Time made against this Enemy Manager.
          /// </summary>
          public float KillTime
          {
               get { return killTime; }
               set { killTime = value; }
          }

          public List<float> GetWaveData()
          {
               List<float> waveData = new List<float>();
               waveData.Add(WaveNumber);
               waveData.Add(Score);
               waveData.Add(EnemiesKilled);
               waveData.Add(KillTime);
               waveData.Add(PerfectKills);
               waveData.Add(SpeedKills);
               waveData.Add(EnemiesEscaped);

               return waveData;
          }

          /// <summary>
          /// Amount of Enemies in the Enemy Manager.
          /// </summary>
          public int Size
          {
               get { return aliveEnemies.Count; }
          }

          /// <summary>
          /// The current Wave Number being played.
          /// </summary>
          public uint WaveNumber
          {
               get { return waveNumber; }
               set { waveNumber = value; }
          }

          /// <summary>
          /// Returns True if Wave has been destroyed, false otherwise.
          /// </summary>
          public bool WaveDestroyed
          {
               get { return waveDestroyed; }
               set { waveDestroyed = value; }
          }

          /// <summary>
          /// Gets or sets how many Enemies are on-screen at any given time.
          /// </summary>
          public int NumberOfSimultaneousEnemies
          {
               get { return numberOfSimultaneousEnemies; }
               set { numberOfSimultaneousEnemies = value; }
          }

          #endregion

          public int CharactersTyped
          {
               get { return charactersTyped; }
               set { charactersTyped = value; }
          }

          #region Initialization

          public EnemyManager(Game game)
               : base(game)
          {
               WaveNumber = 1;
               WaveDestroyed = false;
          }

          #endregion

          #region Handle Input

          public void HandleInput(InputState input, PlayerIndex? ControllingPlayer)
          {
               PlayerIndex playerIndex = ControllingPlayer.Value;

               // Cancel Targetting?
               if (input.IsButtonDown(Buttons.RightShoulder, playerIndex, out playerIndex) ||
                   input.IsButtonDown(Buttons.LeftShoulder, playerIndex, out playerIndex) ||
                   input.IsKeyDown(Keys.Tab, null, out playerIndex))
               {
                    // We no longer have a valid target selected.
                    isTargetSelected = false;

                    // Reset the target to point to nothing.
                    currentTarget = null;

                    for (int i = 0; i < this.Size; i++)
                    {
                         // New
                         if (aliveEnemies[i].IsTarget)
                         {
                              aliveEnemies[i].OnUntargetted();
                         }
                         // End


                         // No enemy is the current target.
                         aliveEnemies[i].IsTarget = false;
                    }
               }

               // If we have not selected a target...
               if (!isTargetSelected)
               {
                    // Select a new one!
                    SelectNewTarget(input, null);
                    return;
               }

               // For each Enemy in the Enemy Manager...
               for (int i = 0; i < this.Size; i++)
               {
                    // Handle ANY PlayerIndex input.
                    aliveEnemies[i].HandleInput(input, null, playerIndex);
               }
          }

          public void HandleInput(InputState input, PlayerIndex? ControllingPlayer, out PlayerIndex playerIndex)
          {
               playerIndex = ControllingPlayer.Value;

               // Cancel Targetting?
               if (input.IsButtonDown(Buttons.RightShoulder, playerIndex, out playerIndex) ||
                   input.IsButtonDown(Buttons.LeftShoulder, playerIndex, out playerIndex) ||
                   input.IsKeyDown(Keys.Tab, null, out playerIndex))
               {
                    // We no longer have a valid target selected.
                    isTargetSelected = false;

                    // Reset the target to point to nothing.
                    currentTarget = null;

                    for (int i = 0; i < this.Size; i++)
                    {
                         // No enemy is the current target.
                         aliveEnemies[i].IsTarget = false;
                    }
               }

               // If we have not selected a target...
               if (!isTargetSelected)
               {
                    // Select a new one!
                    SelectNewTarget(input, null);
                    return;
               }

               // For each Enemy in the Enemy Manager...
               for (int i = 0; i < this.Size; i++)
               {
                    // Handle ANY PlayerIndex input.
                    aliveEnemies[i].HandleInput(input, null, playerIndex);
               }
          }

          #endregion

          #region Update

          /// <summary>
          /// Allows each Enemy to run update logic.
          /// </summary>
          public override void Update(GameTime gameTime)
          {
               ElapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

               // Every 1 second we sort enemies by priority;
               // those closest to player should be targetted first.
               if (ElapsedTime >= 0.25f)
               {
                    CompareByPrioty comparer = new CompareByPrioty();
                    aliveEnemies.Sort(comparer);
                    ElapsedTime = 0.0f;
               }

               // Handle Sentence Collision.
               FormatEnemySentences();

               #region Update Alive & Dying Eneimes

               // Update all the alive enemies.
               foreach (TypingEnemy enemy in aliveEnemies)
               {
                    enemy.Update(gameTime);
               }

               // Update all the dying enemies.
               foreach (TypingEnemy enemy in dyingEnemies)
               {
                    enemy.Update(gameTime);
               }

               // Check for dying enemies which have become dead.
               foreach (TypingEnemy enemy in dyingEnemies)
               {
                    if (enemy.ElapsedDyingTime >= 5.0f)
                    {
                         if (dyingEnemies != null)
                         {
                              this.RemoveDyingEnemy(enemy, true);
                              break;
                         }
                    }
               }

               // Check for newly-dead enemies.
               foreach (TypingEnemy enemy in aliveEnemies)
               {
                    if (enemy.IsDead)
                    {
                         if (enemy.IsSpeedKill)
                         {
                              SpeedKills++;
                         }

                         if (enemy.IsPerfectKill)
                         {
                              PerfectKills++;
                         }

                         float scaledPoints = enemy.BasePoints * 
                              ((int)AvatarTypingGameSettings.Difficulty + 1) * 0.5f;

                         Score += (int)scaledPoints;

                         EnemiesKilled++;
                         KillTime += enemy.ElapsedKillTime;

                         // New WPM Count
                         CharactersTyped += enemy.CharactersTyped;

                         dyingEnemies.Add(enemy);

                         this.RemoveEnemy(enemy, false);

                         isTargetSelected = false;

                         break;
                    }
               }

               // Only check for escaping enemies if there are enemies present.
               if (aliveEnemies.Count > 0)
               {
                    TypingEnemy escapedEnemy = null;

                    foreach (TypingEnemy enemy in aliveEnemies)
                    {
                         if (currentPlayer == null)
                         {
                              if (enemy.IsCollision(new Vector3(0, 0, 0)))
                              {
                                   enemy.OnCollide(currentPlayer);

                                   enemy.OnEscaped();

                                   EnemiesEscaped++;
                                   escapedEnemy = enemy;

                                   this.RemoveEnemy(enemy, true);

                                   break;
                              }
                         }

                         else if (enemy.IsCollision(this.currentPlayer.Position))
                         {
                              enemy.OnCollide(currentPlayer);

                              enemy.OnEscaped();

                              EnemiesEscaped++;
                              escapedEnemy = enemy;

                              this.RemoveEnemy(enemy, true);

                              break;
                         }
                    }

                    // If our target was the one that escaped...
                    if (currentTarget == escapedEnemy)
                    {
                         // We no longer have a valid target selected.
                         isTargetSelected = false;

                         // Reset the target to point to nothing.
                         currentTarget = null;
                    }
               }

               #endregion
          }

          #endregion

          #region Draw

          /// <summary>
          /// Tells each Enemy to draw itself. Uses the default SpriteBatch and Font found in ScreenManager.
          /// </summary>
          public override void Draw(GameTime gameTime)
          {
               // Draw the Dead Enemies first!
               foreach (TypingEnemy enemy in dyingEnemies)
               {
                    enemy.Draw(gameTime, ScreenManager.Font);
               }

               // Draw the Alive Enemies next - just their Avatars!
               for (int i = aliveEnemies.Count - 1; i >= 0; i--)
               {
                    if (i >= aliveEnemies.Count)
                         break;

                    aliveEnemies[i].Draw(gameTime, ScreenManager.Font);
               }

               MySpriteBatch.Begin(BlendState.AlphaBlend);

               // Draw the Dead Enemies thirdly - this time their Shot Text!
               foreach (TypingEnemy enemy in dyingEnemies)
               {
                    enemy.DrawShotWords(gameTime, ScreenManager.Font);
               }

               // Draw the Alive Enemies last - this time their Text!
               for (int i = aliveEnemies.Count - 1; i >= 0; i--)
               {
                    if (i >= aliveEnemies.Count)
                         break;

                    aliveEnemies[i].DrawWords(gameTime, ScreenManager.Font);
                    aliveEnemies[i].DrawShotWords(gameTime, ScreenManager.Font);
               }

               MySpriteBatch.End();
          }

          /// <summary>
          /// Tells each Enemy to draw itself. Uses the default SpriteBatch and Font found in ScreenManager.
          /// </summary>
          public void DrawWithoutWords(GameTime gameTime)
          {
               // Draw the Dead Enemies first!
               foreach (TypingEnemy enemy in dyingEnemies)
               {
                    enemy.Draw(gameTime, ScreenManager.Font);
               }

               // Draw the Alive Enemies next - just their Avatars!
               for (int i = aliveEnemies.Count - 1; i >= 0; i--)
               {
                    if (i >= aliveEnemies.Count)
                         break;

                    aliveEnemies[i].Draw(gameTime, ScreenManager.Font);
               }
          }

          #endregion

          #region Public Add / Get Methods

          /// <summary>
          /// Adds a new Enemy to the Enemy Manager.
          /// </summary>
          public void AddEnemy(TypingEnemy enemy)
          {
               enemy.EnemyManager = this;
               aliveEnemies.Add(enemy);
          }

          /// <summary>
          /// Expose an array holding all the enemies. We return a copy rather
          /// than the real master list, because enemies should only ever be added
          /// or removed using the AddEnemy and RemoveEnemy methods.
          /// </summary>
          public TypingEnemy[] GetEnemies()
          {
               return aliveEnemies.ToArray();
          }

          /// <summary>
          /// Expose an array holding all the enemies. We return a copy rather
          /// than the real master list, because enemies should only ever be added
          /// or removed using the AddEnemy and RemoveEnemy methods.
          /// </summary>
          public TypingEnemy GetEnemyAt(int index)
          {
               return aliveEnemies[index];
          }

          #endregion

          #region Wave Initialization
          
          /// <summary>
          /// Generates an Enemy of the specified type.
          /// 
          /// Simply uses a switch-case to instanciate the correct TypingEnemy type.
          /// </summary>
          /// <param name="typeOfEnemy">The type of Enemy to generate.</param>
          /// <returns>The generated Enemy with default properties.</returns>
          public TypingEnemy GenerateEnemy(EnemyType typeOfEnemy)
          {
               Vector3 position = new Vector3();
               TypingEnemy enemy;

               switch ((int)typeOfEnemy)
               {
                    // Arcade Mode with Scripted Enabled will just use these.

                    case (int)EnemyType.Normal:
                         enemy = new NormalTypingEnemy(position, this);
                         break;

                    case (int)EnemyType.Fast:
                         enemy = new FastTypingEnemy(position, this);
                         break;

                    case (int)EnemyType.Kamikaze:
                         enemy = new SuicideTypingEnemy(position, this);
                         break;

                    case (int)EnemyType.Explosive:
                         enemy = new ExplodingTypingEnemy(position, this);
                         break;

                    case (int)EnemyType.Deflatable:
                         enemy = new DeflatingTypingEnemy(position, this);
                         break;

                    case (int)EnemyType.Dancing:
                         enemy = new DancingTypingEnemy(position, this);
                         break;

                    case (int)EnemyType.Backward:
                         enemy = new BackwardTypingEnemy(position, this);
                         break;

                    case (int)EnemyType.Horde:
                         enemy = new HordeTypingEnemy(position, this);
                         break;
  

                    // Survival Mode extends its choices to allow percentage-based appearance.

                    // Normal Enemy Chance: 6 / 23
                    case 8:
                         enemy = new NormalTypingEnemy(position, this);
                         break;

                    case 9:
                         enemy = new NormalTypingEnemy(position, this);
                          break;

                    case 10:
                         enemy = new NormalTypingEnemy(position, this);
                         break;

                    case 11:
                         enemy = new NormalTypingEnemy(position, this);
                         break;

                    case 12:
                         enemy = new NormalTypingEnemy(position, this);
                         break;

                    // Fast Enemy Chance: 3 / 20
                    case 13:
                         enemy = new FastTypingEnemy(position, this);
                         break;

                    case 14:
                         enemy = new FastTypingEnemy(position, this);
                         break;

                    // Suicide Enemy Chance: 3 / 20
                    case 15:
                         enemy = new SuicideTypingEnemy(position, this);
                         break;

                    case 16:
                         enemy = new SuicideTypingEnemy(position, this);
                         break;

                    // Explosive Enemy Chance: 3 / 20
                    case 17:
                         enemy = new ExplodingTypingEnemy(position, this);
                         break;

                    case 18:
                         enemy = new ExplodingTypingEnemy(position, this);
                         break;

                    // Deflating Enemy Chance: 2 / 20
                    case 19:
                         enemy = new DeflatingTypingEnemy(position, this);
                         break;

                    // Dancing Enemy Chance: 3 / 20
                    case 20:
                         enemy = new DancingTypingEnemy(position, this);
                         break;

                    case 21:
                         enemy = new DancingTypingEnemy(position, this);
                         break;

                    // Backward Enemy Chance: 2 / 20
                    case 22:
                         enemy = new BackwardTypingEnemy(position, this);
                         break;

                    // Normal Enemy if we somehow get an un-specified number.
                    default:
                         enemy = new NormalTypingEnemy(position, this);
                         break;
               }

               return enemy;
          }

          #endregion

          #region Helper Remove Methods

          /// <summary>
          /// Removes an Enemy from the Enemy Manager.
          /// </summary>
          private void RemoveEnemy(TypingEnemy enemy, bool removeFromMemory)
          {
               if (aliveEnemies != null)
               {
                    aliveEnemies.Remove(enemy);

                    if (removeFromMemory)
                    {
                         enemy.Unload();
                         enemy = null;
                    }
               }
          }

          /// <summary>
          /// Removes an Enemy from the Enemy Manager.
          /// </summary>
          private void RemoveDyingEnemy(TypingEnemy enemy, bool removeFromMemory)
          {
               if (aliveEnemies != null)
               {
                    dyingEnemies.Remove(enemy);

                    if (removeFromMemory)
                    {
                         enemy.Unload();
                         enemy = null;
                    }
               }
          }

          /// <summary>
          /// Removes all Enemies from the Enemy Manager.
          /// </summary>
          public void RemoveAllEnemies()
          {
               if (aliveEnemies != null)
                    aliveEnemies.Clear();

               if (dyingEnemies != null)
                    dyingEnemies.Clear();
          }

          #endregion

          #region Select new Target

          private void SelectNewTarget(InputState input, PlayerIndex? ControllingPlayer)
          {
               PlayerIndex playerIndex;
               int playerIndex1 = 0;
               if (ControllingPlayer.HasValue)
                    playerIndex1 = (int)ControllingPlayer.Value;

               KeyboardState keyboardState = input.CurrentKeyboardStates[0];
               GamePadState gamePadState = input.CurrentGamePadStates[playerIndex1];

               for (int controllerIndex = 0; controllerIndex < 4; controllerIndex++)
               {
                    if (!isTargetSelected)
                    {
                         if (input.CurrentKeyboardStates[controllerIndex].GetPressedKeys().Length > 0 && this.Size > 0)
                         {
                              for (int i = 0; i < this.Size; i++)
                              {
                                   if (aliveEnemies[i].WordList.Count <= 0)
                                        continue;

                                   // Crashed cause Keylist = 0.
                                   if (input.IsNewKeyPress(aliveEnemies[i].WordList[0].KeyList[0], (PlayerIndex)controllerIndex, out playerIndex)
                                        && aliveEnemies[i].Position.Y > -10)
                                   {
                                        aliveEnemies[i].OnTargetted();
 
                                        // NEW TESTING
                                        // Hit the enemy!
                                        aliveEnemies[i].OnHit();

                                        // If the Enemy's Word is of length 0 or less, he's dead.
                                        if (aliveEnemies[i].WordList[0].Length <= 0 || !aliveEnemies[i].IsAlive)
                                        {
                                             // New for WPM logic
                                             // Instead of doing this, we should update Enemy.CharactersTyped.
                                             //this.CharactersTyped += aliveEnemies[i].WordList[0].CharacterCount;
                                             aliveEnemies[i].CharactersTyped += aliveEnemies[i].WordList[0].CharacterCount;
                                             // End

                                             aliveEnemies[i].WordList.RemoveAt(0);

                                             aliveEnemies[i].OnSentenceComplete();

                                             if (aliveEnemies[i].WordList.Count <= 0)
                                             {
                                                  aliveEnemies[i].OnKilled();
                                             }
                                        }
                                        // END NEW TESTING


                                        isTargetSelected = true;
                                        currentTarget = aliveEnemies[i];

                                        break;
                                   }
                              }
                         }
                    }
               }

          }

          #endregion

          #region Disposal

          protected override void UnloadContent()
          {
               this.RemoveAllEnemies();
          }

          #endregion

          #region Helper Methods

          private void FormatEnemySentences()
          {
               #region Revised Sentence Collision Handler

               // Recently put this here: TESTING PHASE!
               foreach (TypingEnemy enemy in aliveEnemies)
               {
                    // Gets each Enemy's true Sentence Rectangle.
                    enemy.GetSentenceRectangle();
               }

               // New outter loop:
               // Not really necessary for odd number of enemies, but doesn't hurt!
               for (int a = 0; a < 2; a++)
               {
                    float index = 0;

                    for (int i = aliveEnemies.Count - 1; i >= 0; i--)
                    {
                         index++;

                         if (i != 0)
                         {
                              if (aliveEnemies[i].SentenceBorder.Intersects(aliveEnemies[i - 1].SentenceBorder))
                              {
                                   int difference =
                                        aliveEnemies[i].SentenceBorder.Bottom - aliveEnemies[i - 1].SentenceBorder.Top;

                                   difference += 5;

                                   // If there is an odd number of Enemies...
                                   if (aliveEnemies.Count % 2 != 0)
                                   {
                                        if ((float)(index / (aliveEnemies.Count + 1)) < 0.5f) // alivenemies.Count + 1
                                        {
                                             // Move UP.
                                             difference *= 1;
                                        }

                                        else if ((float)(index / (aliveEnemies.Count + 1)) > 0.5f) // alivenemies.Count + 1
                                        {
                                             // Move DOWN.
                                             difference *= -1;
                                        }

                                        else
                                        {
                                             // Do not move.
                                             difference = 0;
                                        }
                                   }

                                   // New
                                   // Otherwise, there is an even number of Enemies...
                                   else
                                   {
                                        if ((float)(index / (aliveEnemies.Count)) <= 0.5f) // alivenemies.Count + 1
                                        {
                                             // Move UP.
                                             difference *= 1;
                                        }

                                        else if ((float)(index / (aliveEnemies.Count)) > 0.5f) // alivenemies.Count + 1
                                        {
                                             // Move DOWN.
                                             difference *= -1;
                                        }
                                   }
                                   // End new

                                   aliveEnemies[i].SentenceBorder = 
                                        new Rectangle(aliveEnemies[i].SentenceBorder.X, aliveEnemies[i].SentenceBorder.Y - difference,
                                             aliveEnemies[i].SentenceBorder.Width, aliveEnemies[i].SentenceBorder.Height);

                                   aliveEnemies[i].SentencePosition =
                                        new Vector2(aliveEnemies[i].SentencePosition.X, aliveEnemies[i].SentencePosition.Y - difference);
                              }
                         }

                         else if (i == 0)
                         {
                              if (aliveEnemies.Count <= 1)
                                   break;

                              // Compare to previous one.
                              if (aliveEnemies[i].SentenceBorder.Intersects(aliveEnemies[i + 1].SentenceBorder))
                              {
                                   int difference =
                                        aliveEnemies[i].SentenceBorder.Top - aliveEnemies[i + 1].SentenceBorder.Bottom;

                                   difference -= 5;

                                   aliveEnemies[i].SentenceBorder =
                                        new Rectangle(aliveEnemies[i].SentenceBorder.X, aliveEnemies[i].SentenceBorder.Y - difference,
                                             aliveEnemies[i].SentenceBorder.Width, aliveEnemies[i].SentenceBorder.Height);

                                   aliveEnemies[i].SentencePosition =
                                        new Vector2(aliveEnemies[i].SentencePosition.X, aliveEnemies[i].SentencePosition.Y - difference);
                              }
                         }
                    }
               }
               // End new outter loop.
               #endregion
          }

          #endregion
     }
}