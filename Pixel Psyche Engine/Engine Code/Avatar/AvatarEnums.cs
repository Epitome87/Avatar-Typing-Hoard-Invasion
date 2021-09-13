#region File Description
//-----------------------------------------------------------------------------
// AvatarEnums.cs
// Matt McGrath
//-----------------------------------------------------------------------------
#endregion

namespace PixelEngine.Avatars
{
     #region CustomAvatarType Enum

     /// <summary>
     /// An Enum used for creating pre-customized Avatars.
     /// These Custom Avatars sport Downloadable Content, and
     /// are made to look more appealing than simple-dressed Avatars.
     /// </summary>
     public enum CustomAvatarType
     {
          #region Life-Style Collections

          // ???
          Monster,

          // Burton
          JailhouseOnePiece,

          // Halloween Costumes.
          WerewolfOutfit,
          MummyOutfit,

          // International Collection.
          MariachiOutfit,

          // Spring Collection.
          BunnyOutfit,

          #endregion

          #region Game Collections

          // A Kingdom for Keflings DLC.
          ArabianOutfit,
          NinjaBeeOutfit,

          // Assassin's Creed II DLC.
          Assassin_Black,
          Assassin_White,
          Guard_Medieval,

          // Bioshock 2 DLC.
          BigSisterOutfit,

          // Dante's Inferno DLC.
          DanteCostume,

          // Darksiders DLC.
          WarArmor,

          // Dead Space DLC.
          //ProtectiveSuit,

          // Final Fantasy 13
          HuntressUniform,

          // Gears of War 2 DLC. *Female
          CogArmor,

          // Halo 3 DLC. *Award
          ODSTArmor,

          // Metro 2033 DLC.
          RangerArmor,

          // Modern Warfare 2 DLC.
          ArmyRanger,

          // Monkey Island: SE DLC.
          Pirate_Red,

          // Perfect Dark DLC.
          PerfectAgentOutfit,

          // Red Dead Redemption
          Cowboy,

          // Resident Evil 4 DLC.
          LabCoat,

          // Saints Row 2
          HotdogOutfit,

          // Splinter Cell: Conviction DLC.
          SplintercellOutfit,

          // Star Wars: Empire Strikes Back 30th Anniversary DLC.
          Bossk,
          CP30,
          Chewbacca,
          HanSoloHothOutfit,
          ImperialSnowtrooper,

          // The Force Unleashed DLC.
          ApprenticeOutfit,
          DarkLordArmor,
          StormtrooperOutfit,

          // Toy Solidiers DLC.
          BritishCavalry,
          BritishElite,
          GermanElite,

          #endregion

          #region Famous People

          // Political Figures.
          Palin,
          GeorgeBush,
          McCain,
          Lincoln,

          // Television.
          Mulder,
          Scully,
          Norris,
          BillyMays,

          // Music.
          Lennon,
          Cobain,

          // Cartoon.
          ColonelSanders,
          MonopolyGuy,
          HankHill,

          // Video Games.
          Mario,
          DukeNukem,

          #endregion

          #region Custom Creations

          Jester_Purple,
          Jester_Red,
          Leprechaun,
          MadHatter,

          #endregion

          #region Psyche Pixel Development Team

          Matt,

          #endregion

          #region Friends

          DeathHawk,
          DodgeballBoy,
          Killthief,
          Mach,
          Sylent,
          Mike,
          Apotheosis,
          Daniel

          #endregion
     };

     #endregion

     #region BaseAnimationTypeEnum

     /// <summary>
     /// An Enum with possible "Base" Animation Types for the avatar.
     /// </summary>
     public enum BaseAnimationType
     {
          // An animation of type AvatarAnimation.
          BuiltIn, 
          
          // An animation of type CustomAnimation.
          Custom,

          // Multiple animations consisting of combinations of 
          // AvatarAnimation, CustomAnimation, or both.
          Custom_Multiple
     };

     #endregion
     
     #region AnimationType Enum

     /// <summary>
     /// An Enum with possible Animation Types for the avatar.
     /// </summary>
     public enum AnimationType
     {
          // Animation used for Running.
          Run, 

          // Animation used for Fainting.
          Faint, 

          // Animation used for Throwing.
          Throw, 
          
          // Animation used for Walking.
          Walk
     };

     #endregion
}