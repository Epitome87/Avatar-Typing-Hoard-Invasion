#region File Description
//-----------------------------------------------------------------------------
// SaveGameData.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statement
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace PixelEngine
{
     #region Serializable SaveGameData Structure

     //[Serializable]
     public struct SaveGameData
     {
          // Settings. Temp design until I make it its own Struct.
          public int FontSize;          // Convert to Enum.
          public int Difficulty;        // Convert to Enum.

          public int SoundVolume;
          public int MusicVolume;

          // General gameplay information.
          public string PlayerName;
          public uint Level;
          public float HighScore;

          // The Player's custom Sentences.
          public List<string> WordList;

          // Achievement data.
          public List<bool> IsUnlockedAchievement;
          public AchievementData AwardData;
     }
     
     #endregion

     #region Serializable AchievementData Structure

     //[Serializable]
     public struct AchievementData
     {
          // Gimmick achievement variables.
          public bool GamePurchased;
          public bool CreditsWatched;
          public bool VibrationSet;

          // Lifetime achievement variables.
          public uint TotalKills;
          public uint TotalSpeedKills;
          public uint TotalPerfectKills;

          // Streak-based achievement variables.
          public uint SpeedStreak;
          public uint AccuracyStreak;

          // Level-based achievement variables.
          public uint CurrentWave;
          public float CurrentSurvivalTime;
     };

     #endregion
}