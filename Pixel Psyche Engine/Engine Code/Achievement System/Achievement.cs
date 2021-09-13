#region File Description
//-----------------------------------------------------------------------------
// Achievement.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace PixelEngine.AchievementSystem
{
     /// <remarks>
     /// Represents an "Award".
     /// Contains properties such as Title, Description, and Picture.
     /// Requires an AchievementManager to properly coordinate tasks such as popping
     /// the Achievement up when its Unlock requirements are met.
     /// 
     /// Although the word "Achievement" is not allowed on XBL Indie Games, we can still
     /// use it here in code to make things simpler and more understandable.
     /// </remarks>
     public class Achievement
     {
          #region Fields

          private AchievementManager awardManager = null;
          private string title = "";
          private string description = "";
          private Texture2D picture = null;
          private int pointValue = 0;
          private bool isSecret = false;
          private bool isUnlocked = false;
          private int targetValue;

          #endregion

          #region Properties

          /// <summary>
          /// Gets the AchievementManager for this Achievement.
          /// </summary>
          public AchievementManager AwardManager
          {
               get { return awardManager; }
               set { awardManager = value; }
          }

          /// <summary>
          /// Returns the Title (name) of the Achievement.
          /// </summary>
          public string Title
          {
               get { return title; }
               protected set { title = value; }
          }

          /// <summary>
          /// Returns the Description (how to unlock) of the Achievement.
          /// </summary>
          public string Description
          {
               get 
               {
                    if (isSecret && !isUnlocked)
                         return "Unlock Award To See Description.";
                    
                    else
                         return description; 
               }

               protected set { description = value; }
          }

          /// <summary>
          /// Gets or Sets the Picture associated with this Achievement.
          /// </summary>
          public Texture2D Picture
          {
               get { return picture; }
               set { picture = value; }
          }

          /// <summary>
          /// Gets or Sets the Point Value of the Achievements.
          /// This is similar to an Achievement's Gamerscore.
          /// </summary>
          public int PointValue
          {
               get { return pointValue; }
               set { pointValue = value; }
          }

          /// <summary>
          /// Gets or Sets whether this Achievement has been Unlocked yet.
          /// </summary>
          public bool IsUnlocked
          {
               get { return isUnlocked; }
               set { isUnlocked = value; }
          }

          /// <summary>
          /// Gets or Sets whether this Achievement is a "Secret" Achievement.
          /// </summary>
          public bool IsSecret
          {
               get { return isSecret; }
               set { isSecret = value; }
          }

          /// <summary>
          /// Gets or Sets the value we want to check against in this Achievement's
          /// CheckProgressDelegate. For example, a TargetValue of 5 could mean the
          /// level we are checking if the player is on, such as:
          /// 
          /// if (targetValue == 5) Unlock Achievement for reaching level 5.
          /// </summary>
          public int TargetValue
          {
               get { return targetValue; }
               set { targetValue = value; }
          } 

          /// <summary>
          /// A Delegate definition for checking an Achievement's progress.
          /// </summary>
          /// <param name="targetValue">
          /// The value that marks when an Achievement's progress is complete.
          /// </param>
          /// <returns>Returns True if Achievement's progress is complete.</returns>
          public delegate bool CheckProgressDelegate(int targetValue);

          /// <summary>
          /// The actual Delegate for checking an Achievement's progress.
          /// </summary>
          public CheckProgressDelegate ProgressDelegate; 

          #endregion 

          #region Initialization

          /// <summary>
          /// Basic Achievement Constructor.
          /// </summary>
          /// <param name="name">The Title of the Achievement.</param>
          /// <param name="describe">The Description of the Achievement.</param>
          public Achievement(string name, string describe)
          {
               title = name;
               description = describe;
               picture = null;
          }

          /// <summary>
          /// Achievement Constructor with a full parameter list.
          /// </summary>
          /// <param name="name">The Title of the Achievement.</param>
          /// <param name="describe">The Description of the Achievement.</param>
          /// <param name="target">The Target Value for the Achievement.</param>
          /// <param name="progressDelegate">
          /// The CheckProgressDelegate method that will run the logic 
          /// for checking when the Achievement is to be Unlocked.
          /// </param>
          public Achievement(string name, string describe, int target, CheckProgressDelegate progressDelegate)
               : this(name, describe)
          {
               targetValue = target;
               ProgressDelegate = progressDelegate;
          }

          /// <summary>
          /// Achievement Constructor.
          /// </summary>
          /// <param name="name">The Title of the Achievement.</param>
          /// <param name="describe">The Description of the Achievement.</param>
          /// <param name="texture">The Picture associated with the Achievement.</param>
          public Achievement(string name, string describe, Texture2D texture)
          {
               title = name;
               description = describe;
               picture = texture;
          }

          /// <summary>
          /// Achievement Constructor.
          /// </summary>
          /// <param name="name">The Title of the Achievement.</param>
          /// <param name="describe">The Description of the Achievement</param>
          /// <param name="points">The Point Value of the Achievement.</param>
          /// <param name="target">The Target Value for the Achievement (when it is Unlocked).</param>
          /// <param name="progressDelegate">The CheckProgressDelegate method that will run the logic
          /// for checking when the Achievement is to be Unlocked - usually once "Target" value is reached.</param>
          public Achievement(string name, string describe, int points, int target, CheckProgressDelegate progressDelegate)
               : this(name, describe)
          {
               PointValue = points;
               targetValue = target;
               ProgressDelegate = progressDelegate;
          }

          /// <summary>
          /// Load content associated with an Achievement.
          /// Normally load any Achievement-specific assets, like this Achievement's Picture texture.
          /// </summary>
          public void LoadContent()
          {
          }

          #endregion

          #region Public Methods

          /// <summary>
          /// "Unlocks" an Achievement simply by setting
          /// IsUnlocked to true.
          /// </summary>
          public void UnlockAward()
          {
               this.IsUnlocked = true;
          }

          #endregion
     }
}