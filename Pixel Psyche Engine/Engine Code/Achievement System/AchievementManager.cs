#region File Description
//-----------------------------------------------------------------------------
// AchievementManager.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PixelEngine.Screen;
#endregion

namespace PixelEngine.AchievementSystem
{
     /// <summary>
     /// A singleton which manages "Achievements". Checks the progress of Achievements, determines when to 
     /// unlock new ones, and keeps track of which Achievements have been Unlocked.
     /// </summary>
     public class AchievementManager : DrawableGameComponent
     {
          #region Singleton

          /// <summary>
          /// The singleton for the Achievement Manager.
          /// </summary>
          private static AchievementManager awardManager = null;

          #endregion

          #region Fields

          private static List<Achievement> achievements = new List<Achievement>();
          private static List<Achievement> queuedAchievements = new List<Achievement>();
          private static List<Achievement> queuedPopUps = new List<Achievement>();
          
          public static bool isPopUpOnScreen = false;
          private static bool isUnlock = false;
          public static bool IsUpdating = false;

          // Set to the amount of ms to wait before checking achievement progress.
          private const float UpdateInterval = 2000.0f;
          private float ElapsedTime = 0.0f;

          private SpriteBatch spriteBatch;
          private SpriteFont font;
          private Texture2D blankTexture;

          private static Texture2D defaultPicture;

          private static List<bool> isUnlockedList;

          private bool traceEnabled;

          #endregion

          #region Properties

          /// <summary>
          /// Returns a List of Achievement objects being managed by this
          /// AchievementManager.
          /// </summary>
          public static List<Achievement> Achievements
          {
               get { return achievements; }
          }

          /// <summary>
          /// Returns the number of Achievements being managed by this
          /// AchievementManager.
          /// </summary>
          public static int Count
          {
               get { return achievements.Count; }
          }

          /// <summary>
          /// Returns true if Achievements can be set to "Unlocked", false otherwise.
          /// If false, earned Achievements are queued for Unlocking at a later, more
          /// appropriate time (such as when not on a gameplay screen).
          /// </summary>
          public static bool IsUnlockNow
          {
               get { return isUnlock; }
               set { isUnlock = value; }
          }

          /// <summary>
          /// If true, the manager prints out a list of all the screens
          /// each time it is updated. This can be useful for making sure
          /// everything is being added and removed at the right times.
          /// </summary>
          public bool TraceEnabled
          {
               get { return traceEnabled; }
               set { traceEnabled = value; }
          }

          /// <summary>
          /// Gets or Sets a List of all Achievements "IsUnlocked" status.
          /// Useful for procedures such as loading and saving data, where we 
          /// only want to save whether or not an Achievement has been Unlocked,
          /// and not the actual Achievement itself.
          /// </summary>
          public static List<bool> IsUnlockedList
          {
               get
               {
                    IsUnlockedList = new List<bool>();

                    foreach (Achievement award in Achievements)
                    {
                         isUnlockedList.Add(award.IsUnlocked);
                    }

                    return isUnlockedList;
               }

               set { isUnlockedList = value; }
          }

          #endregion

          #region Initialization

          /// <summary>
          /// Achievement Manager Constructor.
          /// </summary>
          /// <param name="game"></param>
          private AchievementManager(Game game)
               : base(game)
          {
          }

          /// <summary>
          /// Initialize the Achievement Manager DrawableGameComponent.
          /// </summary>
          /// <param name="game"></param>
          public static void Initialize(Game game)
          {
               awardManager = new AchievementManager(game);

               if (game != null)
               {
                    game.Components.Add(awardManager);
               }
          }

          /// <summary>
          /// Load your graphics content.
          /// </summary>
          protected override void LoadContent()
          {
               base.LoadContent();

               // Load content belonging to the screen manager.
               ContentManager content = Game.Content;

               spriteBatch = new SpriteBatch(GraphicsDevice);
               font = content.Load<SpriteFont>(@"Fonts\MenuFont_Border");
               blankTexture = content.Load<Texture2D>(@"Textures\Blank Textures\Blank");
               defaultPicture = content.Load<Texture2D>(@"Achievements\Award");

               // Tell each of the Achievements to load their content.
               foreach (Achievement award in achievements)
               {
                    award.LoadContent();
               }

               // Set each Achievement's Picture as the default one.
               for (int i = 0; i < achievements.Count; i++)
               {
                    achievements[i].Picture = defaultPicture;
               }
          }

          #endregion

          #region Draw

          public override void Draw(GameTime gameTime)
          {
               /*
               Vector2 position = new Vector2(100, 100);
               Color color = Color.White;

               spriteBatch.Begin();

               foreach (Achievement award in achievements)
               {
                    string s = String.Format("{0}\n", award.Title);

                    if (award.IsUnlocked)
                         color = Color.Gold;
 
                    position.Y += 20;

                    spriteBatch.DrawString(font, s, position, color);
               }

               spriteBatch.End(); */
          }

          #endregion

          #region Update

          public override void Update(GameTime gameTime)
          {
               // If we're not updating the Achievement Manager, do nothing.
               if (!IsUpdating)
                    return;

               // Otherwise, increment elapsed time...
               ElapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

               // And check if we should do an update...
               if (ElapsedTime >= UpdateInterval)
               {
                    CheckAchievementProgress();
                    ElapsedTime = 0.0f;
               }

               // Update queued Achievements, if we have any.
               if (queuedAchievements.Count > 0)
               {
                    // If we want to unlock the achievement now...
                    //if (IsUnlockNow)
                    {
                         // ...We iterate through all queued achievements...
                         foreach (Achievement queuedAchievement in queuedAchievements)
                         {
                              // And finally, Unlock the Achievement!
                              UnlockAchievement(queuedAchievement, EngineCore.ControllingPlayer);
                         }

                         // Reset the list of queued Achievements.
                         queuedAchievements.Clear();
                    }
               }

               // Do we have Pop-Ups waiting?
               if (queuedPopUps.Count > 0)
               {
                    // If we finally have a free screen...
                    if (!isPopUpOnScreen)
                    {
                         // Show the Pop-Up!
                         PopUpAchievement(queuedPopUps[0], EngineCore.ControllingPlayer);

                         queuedPopUps.Remove(queuedPopUps[0]);
                    }
               }
          }

          #endregion

          #region Methods

          /// <summary>
          /// Adds an Achievement to be managed.
          /// Calls Achievement.LoadContent() immediately.
          /// </summary>
          /// <param name="award">The Achievement to add / manage.</param>
          public static void AddAchievement(Achievement award)
          {
               award.LoadContent();
               achievements.Add(award);
          }

          /// <summary>
          /// Checks the progress of all Achievements being managed.
          /// </summary>
          public static void CheckAchievementProgress()
          {
               foreach (Achievement award in achievements)
               {
                    // Do nothing if we've Unlocked the Achievement already!
                    if (award.IsUnlocked)
                         continue;

                    // Ensure the Achievement has a valid Progress Delegate!
                    if (award.ProgressDelegate != null)
                    {
                         // If an Achievement's progress is met...
                         if (award.ProgressDelegate(award.TargetValue))
                         {
                              // ...Unlock it!
                              UnlockAchievement(award, EngineCore.ControllingPlayer);
                         }
                    }
               }
          }

          /// <summary>
          /// Does the actual "Unlocking" logic for an Achievement.
          /// </summary>
          /// <param name="award">The Achievement to Unlock.</param>
          /// <param name="controllingPlayer">The PlayerIndex that Unlocked the Achievement.</param>
          public static void UnlockAchievement(Achievement award, PlayerIndex? controllingPlayer)
          {
               /* Are we Unlocking now?
                * This allows us to not show the Achievement "Unlocked", full-screen until
                * we want - like when we return to the main menu.
                * This is a superfluous screen, as the Achievement "Pop" effect is enough.
                * 
                * * NOTE: I could move award.UnlockAward() to unlock when the Pop-Up occurs, and
                * then get rid of the call to AwardUnlockedScreen alltogether.
                */
               
               //if (IsUnlockNow)
               {
                    //award.UnlockAward();
                    //ScreenManager.AddScreen(new AwardUnlockedScreen(award), controllingPlayer);
               }

               // We aren't Unlocking now...
               //else
               {
                    // If this Achievement isn't already queued.
                    if (!queuedAchievements.Contains(award))
                    {
                         award.UnlockAward(); // New.

                         // Add this Achievement to queued Achievement list.
                         queuedAchievements.Add(award);

                         // Add this Achievement to queued Pop-Up list.
                         queuedPopUps.Add(award);
                    }
               }
          }

          /// <summary>
          /// "Pops" an Achievement by calling adding an AwardPopUpScreen
          /// to the ScreenManager. This creates a Pop-Up effect similar to
          /// when an Xbox 360 Achievement is unlocked.
          /// </summary>
          /// <param name="award">The Achievement to "Pop".</param>
          /// <param name="controllingPlayer">The PlayerIndex to Pop the Achievement for.</param>
          public static void PopUpAchievement(Achievement award, PlayerIndex? controllingPlayer)
          {
               ScreenManager.AddPopupScreen(new AwardPopUpScreen(award), controllingPlayer);
          }

          #endregion
     }
}