#region File Description
//-----------------------------------------------------------------------------
// AvatarTypingGameSettings.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
#endregion

namespace AvatarTyping
{
     #region Difficulty Enum

     public enum Difficulty
     {
          Easy,
          Normal,
          Hard,
          Insane
     }

     #endregion

     #region FontSize Enum

     public enum FontSize
     {
          ExtraSmall,
          Small,
          Medium,
          Large,
          ExtraLarge,
     }

     #endregion

     /// <remarks>
     /// Inherits from GameSettings to expand on "game settings"
     /// that are not generic enough to be defined within GameSettings.
     /// </remarks>
     //[Serializable]
     public class AvatarTypingGameSettings : PixelEngine.GameSettings
     {
          #region Fields

          private static float textSize = 0.75f;
          private static FontSize fontSize = FontSize.Medium;
          private static Difficulty difficulty = Difficulty.Easy;

          private static int soundVolume = 8;
          private static int musicVolume = 8;

          #endregion

          #region Properties

          /// <summary>
          /// Size that gameplay-related Text is rendered in.
          /// Since Avatar Typing is unplayable if the player cannot see the text,
          /// this Game Setting is unique and therefore customization of it is critical.
          /// </summary>
          public static float TextSize
          {
               get 
               {
                    switch (fontSize)
                    {
                         case FontSize.ExtraSmall:
                              return 0.35f;

                         case FontSize.Small:
                              return 0.50f;

                         case FontSize.Medium:
                              return 0.75f;

                         case FontSize.Large:
                              return 1.0f;

                         case FontSize.ExtraLarge:
                              return  1.25f;

                         default:
                              return textSize;
                    }                         
               }

               set { textSize = value; }
          }

          /// <summary>
          /// Size that gameplay-related Text is rendered in.
          /// Since Avatar Typing is unplayable if the player cannot see the text,
          /// this Game Setting is unique and therefore customization of it is critical.
          /// </summary>
          public static FontSize FontSize
          {
               get { return fontSize; }
               set { fontSize = value; }
          }

          /// <summary>
          /// Gets or sets the Enum Difficulty of the game.
          /// </summary>
          public static Difficulty Difficulty
          {
               get { return difficulty; }
               set { difficulty = value; }
          }

          public static int SoundVolume
          {
               get { return soundVolume; }
               set { soundVolume = value; }
          }

          public static int MusicVolume
          {
               get { return musicVolume; }
               set { musicVolume = value; }
          }

          #endregion
     }
}