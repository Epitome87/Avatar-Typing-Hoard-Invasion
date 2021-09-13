#region File Description
//-----------------------------------------------------------------------------
// AchievementList.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using PixelEngine.AchievementSystem;
using System;
#endregion

namespace AvatarTyping
{
     /// <remarks>
     /// AchievementList is simply a helper class to initialize the
     /// game-specific Achievements. Achievement creation is done in this
     /// file, as well as linking each delegate to an appropriate Progression Checking
     /// method.
     /// </remarks>
     public static class AchievementList
     {
          #region Achievement Progression Checking Methods

          #region Gimmick, "Free" Achievements

          /// <summary>
          /// A CheckProgressDelegate for checking the progress of
          /// achievements related to purchasing Avatar Typing.
          /// </summary>
          public static bool GamePurchasedProgressFunction(int targetValue)
          {
               return AvatarTypingGame.AwardData.GamePurchased;
          }

          /// <summary>
          /// A CheckProgressDelegate for checking if the Player is
          /// a friend of "i Epitome i"s.
          /// </summary>
          public static bool DevelopersFriendProgressFunction(int targetValue)
          {
               if (AvatarTypingGame.CurrentPlayer == null)
                    return false;

               if (AvatarTypingGame.CurrentPlayer.GamerInfo.Gamer == null)
                    return false;

               if (AvatarTypingGame.CurrentPlayer.GamerInfo.Gamer.IsGuest ||
                   !AvatarTypingGame.CurrentPlayer.GamerInfo.Gamer.IsSignedInToLive)
                    return false;

               Microsoft.Xna.Framework.GamerServices.FriendCollection friends;
               
               // Grab the Player's Friend List.
               friends = AvatarTypingGame.CurrentPlayer.GamerInfo.Gamer.GetFriends();

               // Iterate through all the Player's friends...
               foreach (Microsoft.Xna.Framework.GamerServices.FriendGamer friend in friends)
               {
                    // If they have "i Epitome i" as a friend, Achievement Unlocked!
                    if (friend.Gamertag == "I ePiToMe I")
                    {
                         return true;
                    }
               }

               // Otherwise, they receive no Achievement.
               return false;
          }

          /// <summary>
          /// A CheckProgressDelegate for checking the progress of
          /// achievements related to watching the Credits.
          /// </summary>
          public static bool CreditsWatchedProgressFunction(int targetValue)
          {
               return AvatarTypingGame.AwardData.CreditsWatched;
          }

          /// <summary>
          /// A CheckProgressDelegate for checking the progress of
          /// achievements related to custom sentence creation.
          /// </summary>
          /// <param name="targetValue">The amount of custom sentences to check for.</param>
          /// <returns>Returns true if there exit targetValue amount of custom sentences.</returns>
          public static bool SentencesCreatedProgressFunction(int targetValue)
          {
               if (SentenceDatabase.UserSentences != null)
                    return (SentenceDatabase.UserSentences.Count >= targetValue);

               else
                    return false;
          }

          public static bool VibrationSetProgressFunction(int targetValue)
          {
               return AvatarTypingGame.AwardData.VibrationSet;
          }

          public static bool NaughtyWordProgressFunction(int targetValue)
          {
               if (SentenceDatabase.UserSentences != null)
               {
                    foreach (string word in SentenceDatabase.UserSentences)
                    {
                         if (word.ToLower().Contains("penis") || word.ToLower().Contains("fuck") || word.ToLower().Contains("pussy") ||
                              word.ToLower().Contains("dick") || word.ToLower().Contains("cock") || word.ToLower().Contains("bitch") ||
                              word.ToLower().Contains("whore"))
                         {
                              return true;
                         }
                    }

                    return false;
                    /*
                    return
                         SentenceDatabase.UserSentences.Contains("Penis".ToLower()) || SentenceDatabase.UserSentences.Contains("penis") ||
                         SentenceDatabase.UserSentences.Contains("Fuck")  || SentenceDatabase.UserSentences.Contains("fuck")  ||
                         SentenceDatabase.UserSentences.Contains("Pussy") || SentenceDatabase.UserSentences.Contains("pussy") ||
                         SentenceDatabase.UserSentences.Contains("Dick")  || SentenceDatabase.UserSentences.Contains("dick")  ||
                         SentenceDatabase.UserSentences.Contains("Cock")  || SentenceDatabase.UserSentences.Contains("cock");
                    */
               }

               else
                    return false;
          }

          #endregion

          #region Life-Time Achievements

          /// <summary>
          /// A CheckProgressDelegate for checking the progress of
          /// achievements related to the player's Total Kills.
          /// </summary>
          public static bool TotalKillsProgressFunction(int targetValue)
          {
               return (AvatarTypingGame.AwardData.TotalKills >= targetValue);
          }

          /// <summary>
          /// A CheckProgressDelegate for checking the progress of
          /// achievements related to the player's Total Kills.
          /// </summary>
          public static bool TotalSpeedKillsProgressFunction(int targetValue)
          {
               return (AvatarTypingGame.AwardData.TotalSpeedKills >= targetValue);
          }

          /// <summary>
          /// A CheckProgressDelegate for checking the progress of
          /// achievements related to the player's Total Kills.
          /// </summary>
          public static bool TotalPerfectKillsProgressFunction(int targetValue)
          {
               return (AvatarTypingGame.AwardData.TotalPerfectKills >= targetValue);
          }

          #endregion

          #region Streak-Based Achievements

          /// <summary>
          /// A CheckProgressDelegate for checking the progress of
          /// achievements related to the player's Total Kills.
          /// </summary>
          public static bool SpeedKillStreakProgressFunction(int targetValue)
          {
               return (AvatarTypingGame.AwardData.SpeedStreak >= targetValue);
          }

          /// <summary>
          /// A CheckProgressDelegate for checking the progress of
          /// achievements related to the player's Total Kills.
          /// </summary>
          public static bool PerfectKillStreakProgressFunction(int targetValue)
          {
               return (AvatarTypingGame.AwardData.AccuracyStreak >= targetValue);
          }

          #endregion

          #region Level-Based Achievements

          public static bool WaveCompleteNormalProgressFunction(int targetValue)
          {
               return AvatarTypingGameSettings.Difficulty == Difficulty.Normal &
                    AvatarTypingGame.AwardData.CurrentWave >= targetValue;
          }

          public static bool WaveCompleteHardProgressFunction(int targetValue)
          {
               return AvatarTypingGameSettings.Difficulty == Difficulty.Hard &
                    AvatarTypingGame.AwardData.CurrentWave >= targetValue;
          }

          public static bool WaveCompleteInsaneProgressFunction(int targetValue)
          {
               return AvatarTypingGameSettings.Difficulty == Difficulty.Insane &
                    AvatarTypingGame.AwardData.CurrentWave >= targetValue;
          }

          public static bool SurviveTimeProgressFunction(int targetValue)
          {
               return AvatarTypingGame.AwardData.CurrentSurvivalTime >= targetValue * 60;
          }

          #endregion

          #endregion

          #region Achievement Initialization

          public static void InitializeAchievements()
          {
               #region Gimmick, "Free" Achievements

               AchievementManager.AddAchievement(new Achievement(
                    "Money, Please!", "You Purchased Avatar Typing: Horde Invasion", 0, GamePurchasedProgressFunction));

               AchievementManager.AddAchievement(new Achievement(
                    "Friends In High Places", "You Know The Developer", 0, DevelopersFriendProgressFunction));

               AchievementManager.AddAchievement(new Achievement(
                    "Recognized!", "You Fully Watched Credits", 0, CreditsWatchedProgressFunction));

               //AchievementManager.AddAchievement(new Achievement(
               //     "Oh! That's the Spot...", "You Set Vibration [On]\nAnd Stayed to Enjoy It", 0, VibrationSetProgressFunction));

               #endregion

               #region Secret Achievements.

               Achievement dirtyMindAchievement = new Achievement("Dirty Mind", 
                    "You Know What You Wrote...", 0, NaughtyWordProgressFunction);
               dirtyMindAchievement.IsSecret = true;

               AchievementManager.AddAchievement(dirtyMindAchievement);

               #endregion

               #region Sentence Creation Achievements

               AchievementManager.AddAchievement(new Achievement(
                    "Discoverer of Words!", "You Created 5 Custom Sentences", 5, SentencesCreatedProgressFunction));

               AchievementManager.AddAchievement(new Achievement(
                    "Creator of Words!", "You Created 10 Custom Sentences", 10, SentencesCreatedProgressFunction));

               AchievementManager.AddAchievement(new Achievement(
                    "Architect of Words!", "You Created 25 Custom Sentences", 25, SentencesCreatedProgressFunction));

               AchievementManager.AddAchievement(new Achievement(
                    "Master of Words!", "You Created 100 Custom Sentences", 100, SentencesCreatedProgressFunction));

               #endregion

               #region Life-Time Achievements.

               // Novice Achievements.
               AchievementManager.AddAchievement(new Achievement(
                    "Killer - Novice", "Obtain 100 Total Kills", 100, TotalKillsProgressFunction));
               
               AchievementManager.AddAchievement(new Achievement(
                    "Accuracy Killer - Novice", "Obtain 100 Perfect Kills", 100, TotalPerfectKillsProgressFunction));

               AchievementManager.AddAchievement(new Achievement(
                    "Speed Killer - Novice", "Obtain 100 Speed Kills", 100, TotalSpeedKillsProgressFunction));


               // Amateur Achievements.
               AchievementManager.AddAchievement(new Achievement(
                    "Killer - Amateur", "Obtain 500 Total Kills", 500, TotalKillsProgressFunction));

               AchievementManager.AddAchievement(new Achievement(
                    "Accuracy Killer - Amateur", "Obtain 500 Perfect Kills", 500, TotalPerfectKillsProgressFunction));

               AchievementManager.AddAchievement(new Achievement(
                    "Speed Killer - Amateur", "Obtain 500 Speed Kills", 500, TotalSpeedKillsProgressFunction));


               // Pro Achievements.
               AchievementManager.AddAchievement(new Achievement(
                    "Killer - Pro", "Obtain 1,000 Total Kills", 1000, TotalKillsProgressFunction));

               AchievementManager.AddAchievement(new Achievement(
                    "Accuracy Killer - Pro", "Obtain 1,000 Perfect Kills", 1000, TotalPerfectKillsProgressFunction));

               AchievementManager.AddAchievement(new Achievement(
                    "Speed Killer - Pro", "Obtain 1,000 Speed Kills", 1000, TotalSpeedKillsProgressFunction));


               // God Achievements.
               AchievementManager.AddAchievement(new Achievement(
                    "Killer - God", "Obtain 5,000 Total Kills", 5000, TotalKillsProgressFunction));

               AchievementManager.AddAchievement(new Achievement(
                    "Accuracy Killer - God", "Obtain 5,000 Perfect Kills", 5000, TotalPerfectKillsProgressFunction));

               AchievementManager.AddAchievement(new Achievement(
                    "Speed Killer - God", "Obtain 5,000 Speed Kills", 5000, TotalSpeedKillsProgressFunction));

               #endregion

               #region Streak-Based Awards.

               // Novice Achievements.
               AchievementManager.AddAchievement(new Achievement("Accuracy Streaker - Novice", "Obtain a 5x Perfect Kill Streak", 5, PerfectKillStreakProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Speedy Streaker - Novice", "Obtain a 5x Speed Kill Streak", 5, SpeedKillStreakProgressFunction));

               // Amateur Achievements.
               AchievementManager.AddAchievement(new Achievement("Accuracy Streaker - Amateur", "Obtain a 10x Perfect Kill Streak", 10, PerfectKillStreakProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Speedy Streaker - Amateur", "Obtain a 10x Speed Kill Streak", 10, SpeedKillStreakProgressFunction));

               // Apprentice Achievements.
               AchievementManager.AddAchievement(new Achievement("Accuracy Streaker - Apprentice", "Obtain a 15x Perfect Kill Streak", 15, PerfectKillStreakProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Speedy Streaker - Apprentice", "Obtain a 15x Speed Kill Streak", 15, SpeedKillStreakProgressFunction));

               // Expert Achievements.
               AchievementManager.AddAchievement(new Achievement("Accuracy Streaker - Expert", "Obtain a 25x Perfect Kill Streak", 25, PerfectKillStreakProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Speedy Streaker - Expert", "Obtain a 25x Speed Kill Streak", 25, SpeedKillStreakProgressFunction));

               // Master Achievements.
               AchievementManager.AddAchievement(new Achievement("Accuracy Streaker - Master", "Obtain a 50x Perfect Kill Streak", 50, PerfectKillStreakProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Speedy Streaker - Master", "Obtain a 50x Speed Kill Streak", 50, SpeedKillStreakProgressFunction));

               // God Achievements.
               AchievementManager.AddAchievement(new Achievement("Accuracy Streaker - God", "WHAT?! Obtain a 100x Perfect Kill Streak!", 100, PerfectKillStreakProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Speedy Streaker - God", "WHAT?! Obtain a 100x Speed Kill Streak!", 100, SpeedKillStreakProgressFunction));

               #endregion

               #region Level-Based Awards.

               #region Arcade Mode: Normal Awards

               AchievementManager.AddAchievement(new Achievement("Warming Up - Normal", "Complete Wave 1 On Normal", 2, WaveCompleteNormalProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Tingling Hands - Normal", "Complete Wave 2 On Normal", 3, WaveCompleteNormalProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Sore Hands - Normal", "Complete Wave 3 On Normal", 4, WaveCompleteNormalProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Aching Hands - Normal", "Complete Wave 4 On Normal", 5, WaveCompleteNormalProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Cramped Hands - Normal", "Complete Wave 5 On Normal", 6, WaveCompleteNormalProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Blistered Hands - Normal", "Complete Wave 10 On Normal", 11, WaveCompleteNormalProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Crippled Hands - Normal", "Complete Wave 15 On Normal", 16, WaveCompleteNormalProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Carpal Tunnel Syndrome - Normal", "Complete Wave 20 On Normal", 21, WaveCompleteNormalProgressFunction));

               #endregion

               #region Arcade Mode: Hard Awards

               AchievementManager.AddAchievement(new Achievement("Warming Up - Hard", "Complete Wave 1 On Hard", 2, WaveCompleteHardProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Tingling Hands - Hard", "Complete Wave 2 On Hard", 3, WaveCompleteHardProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Sore Hands - Hard", "Complete Wave 3 On Hard", 4, WaveCompleteHardProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Aching Hands - Hard", "Complete Wave 4 On Hard", 5, WaveCompleteHardProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Cramped Hands - Hard", "Complete Wave 5 On Hard", 6, WaveCompleteHardProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Blistered Hands - Hard", "Complete Wave 10 On Hard", 11, WaveCompleteHardProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Crippled Hands - Hard", "Complete Wave 15 On Hard", 16, WaveCompleteHardProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Carpal Tunnel Syndrome - Hard", "Complete Wave 20 On Hard", 21, WaveCompleteHardProgressFunction));
              
               #endregion

               #region Arcade Mode: Insane Awards

               AchievementManager.AddAchievement(new Achievement("Warming Up - Insane", "Complete Wave 1 On Insane", 2, WaveCompleteInsaneProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Tingling Hands - Insane", "Complete Wave 2 On Insane", 3, WaveCompleteInsaneProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Sore Hands - Insane", "Complete Wave 3 On Insane", 4, WaveCompleteInsaneProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Aching Hands - Insane", "Complete Wave 4 On Insane", 5, WaveCompleteInsaneProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Cramped Hands - Insane", "Complete Wave 5 On Insane", 6, WaveCompleteInsaneProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Blistered Hands - Insane", "Complete Wave 10 On Insane", 11, WaveCompleteInsaneProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Crippled Hands - Insane", "Complete Wave 15 On Insane", 16, WaveCompleteInsaneProgressFunction));
               AchievementManager.AddAchievement(new Achievement("Carpal Tunnel Syndrome - Insane", "Complete Wave 20 On Insane", 21, WaveCompleteInsaneProgressFunction));

               #endregion

               #region Survival Mode Awards

               AchievementManager.AddAchievement(new Achievement("1 Minute Survivor", "Survive for 1 Minute", 1, SurviveTimeProgressFunction));
               AchievementManager.AddAchievement(new Achievement("2 Minute Survivor", "Survive for 2 Minutes", 2, SurviveTimeProgressFunction));
               AchievementManager.AddAchievement(new Achievement("3 Minute Survivor", "Survive for 3 Minutes", 3, SurviveTimeProgressFunction));
               AchievementManager.AddAchievement(new Achievement("4 Minute Survivor", "Survive for 4 Minutes", 4, SurviveTimeProgressFunction));
               AchievementManager.AddAchievement(new Achievement("5 Minute Survivor", "Survive for 5 Minutes", 5, SurviveTimeProgressFunction));
               AchievementManager.AddAchievement(new Achievement("10 Minute Survivor", "Survive for 10 Minutes", 10, SurviveTimeProgressFunction));
               AchievementManager.AddAchievement(new Achievement("15 Minute Survivor", "Survive for 15 Minutes", 15, SurviveTimeProgressFunction));
               AchievementManager.AddAchievement(new Achievement("20 Minute Survivor", "Survive for 20 Minutes", 20, SurviveTimeProgressFunction));

               #endregion

               #endregion
          }

          #endregion
     }
}