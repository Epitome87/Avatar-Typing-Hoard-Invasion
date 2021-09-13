#region File Description
//-----------------------------------------------------------------------------
// CreditsScreen.cs
//
// Copyright (C) Matt McGrath.
// Derives from a modified XNA class provided by Microsoft.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PixelEngine.Text;
#endregion

namespace PixelEngine.Screen
{
     #region CreditEntry structure.

     public struct CreditEntry
     {
          public string role;
          public string[] names;

          public CreditEntry(string theRole, params string[] nameList)
          {
               role = theRole;
               names = nameList;
          }

          public override string ToString()
          {
               string result = String.Empty;// role;

               foreach (string name in names)
               {
                    result = result + "\n" + name;
               }

               return result;
          }
     };

     #endregion
     
     /// <remarks>
     /// The Credits screen is loaded upon the game's end, or via the Main Menu.
     /// This screen displays the list of those who participated in the game's creation.
     /// Along with their name and role, their avatar is displayed.
     /// </remarks>
     public class CreditScreen : MenuScreen
     {
          #region Fields

          protected SpriteBatch spriteBatch;
          protected ContentManager content;
          protected SpriteFont ChatFont;

          protected Color nameFontColor = Color.White;
          protected Color roleFontColor = Color.DarkOrange;
          protected float nameFontScale = 1.0f;
          protected float roleFontScale = 1.25f;

          protected List<CreditEntry> creditEntries = new List<CreditEntry>();

          protected int currentRole = 0;
          protected Vector2 textPosition = new Vector2(1280 / 2, -200);

          protected float scrollSpeed = 3.0f;
          protected float defaultScrollSpeed = 3.0f;
          protected float rushedScrollSpeed = 6.0f;

          protected bool isLooping = true;

          protected bool isUpwardMovement = true;
          protected float startingPosition;
          protected float endingPosition;

          protected Buttons rushButton = Buttons.Y;

          #endregion

          #region Properties

          /// <summary>
          /// The font Color the Role strings will be rendered in.
          /// </summary>
          public Color RoleColor
          {
               get { return roleFontColor; }
               set { roleFontColor = value; }
          }

          /// <summary>
          /// The font Color the Name strings will be rendered in.
          /// </summary>
          public Color NameColor
          {
               get { return nameFontColor; }
               set { nameFontColor = value; }
          }

          /// <summary>
          /// The font scale the Role string will be rendered in.
          /// </summary>
          public float RoleScale
          {
               get { return roleFontScale; }
               set { roleFontScale = value; }
          }

          /// <summary>
          /// The font scale the Name string will be rendered in.
          /// </summary>
          public float NameScale
          {
               get { return nameFontScale; }
               set { nameFontScale = value; }
          }

          /// <summary>
          /// Whether or not the Credits are to replay once they have finished.
          /// </summary>
          public bool IsLooping
          {
               get { return isLooping; }
               set { isLooping = value; }
          }

          /// <summary>
          /// The speed at which the Credits will scroll by.
          /// Note: Should be a positive value, as the IsUpwardScrolling 
          /// field determines whether this is negative or positive.
          /// </summary>
          public float ScrollSpeed
          {
               get { return scrollSpeed; }
               set { scrollSpeed = value; }
          }

          /// <summary>
          /// The speed at which the Credits scroll by while the user isn't "Rushing" them.
          /// In other words, the default speed when no user interference is made.
          /// 
          /// Note: Should be a positive value, as the IsUpwardScrolling 
          /// field determines whether this is negative or positive.
          /// </summary>
          public float UnrushedScrollSpeed
          {
               get { return defaultScrollSpeed; }
               set { defaultScrollSpeed = value; }
          }

          /// <summary>
          /// The speed at which the Credits scroll by while the user is "Rushing" them.
          /// Note: Should be a positive value, as the IsUpwardScrolling 
          /// field determines whether this is negative or positive.
          /// </summary>
          public float RushedScrollSpeed
          {
               get { return rushedScrollSpeed; }
               set { rushedScrollSpeed = value; }
          }

          /// <summary>
          /// Whether or not the Credits are scrolling in an Up direction.
          /// Set to false to have the Credits scroll downward.
          /// </summary>
          public bool IsUpwardScrolling
          {
               get { return isUpwardMovement; }
               set { isUpwardMovement = value; }
          }

          #endregion

          #region Initialization

          /// <summary>
          /// Constructor.
          /// </summary>
          public CreditScreen(string menuTitle)
               : base(menuTitle)
          {
               TransitionOnTime = TimeSpan.FromSeconds(0.0f);
               TransitionOffTime = TimeSpan.FromSeconds(0.0f);

               defaultScrollSpeed = scrollSpeed;

               foreach (SignedInGamer signedInGamer in SignedInGamer.SignedInGamers)
               {
                    signedInGamer.Presence.PresenceMode =
                         GamerPresenceMode.WatchingCredits;
               }
          }

          public override void LoadContent()
          {
               if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

               ChatFont = TextManager.MenuFont;//TextManager.TitleFont;
               spriteBatch = ScreenManager.SpriteBatch;  
          }

          #endregion

          #region Handle Input

          public override void HandleInput(InputState input)
          {
               base.HandleInput(input);

               scrollSpeed = defaultScrollSpeed;

               if (input.CurrentGamePadStates[(int)ControllingPlayer.Value].IsButtonDown(rushButton))
               {
                    scrollSpeed = rushedScrollSpeed;
               }
          }

          #endregion

          #region Update

          public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
          {
               base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

               if (IsActive)
               {
                    if (currentRole != creditEntries.Count)
                    {
                         if (isUpwardMovement)
                         {
                              textPosition.Y -= scrollSpeed;

                              if (textPosition.Y < 0f - 100f)
                              {
                                   textPosition.Y = EngineCore.GraphicsInformation.ScreenHeight + 200f;

                                   if (!isLooping)
                                   {
                                        currentRole = Math.Min(++currentRole, creditEntries.Count);
                                   }

                                   else
                                   {
                                        currentRole = (currentRole + 1) % (creditEntries.Count);
                                   }
                              }
                         }

                         else
                         {
                              textPosition.Y += scrollSpeed;

                              if (textPosition.Y >= EngineCore.GraphicsInformation.ScreenHeight + 100f)
                              {
                                   textPosition.Y = 0f - 200f;

                                   if (!isLooping)
                                   {
                                        currentRole = Math.Min(++currentRole, creditEntries.Count);
                                   }

                                   else
                                   {
                                        currentRole = (currentRole + 1) % (creditEntries.Count);
                                   }
                              }
                         }
                    }
               }
          }

          #endregion

          #region Draw

          public override void Draw(GameTime gameTime)
          {
               spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

               string t = creditEntries[currentRole].role;

               string s = creditEntries[currentRole].ToString();

               TextManager.DrawCentered(true, ChatFont, t, 
                    new Vector2(textPosition.X, textPosition.Y - 125), roleFontColor, (roleFontScale));

               TextManager.DrawCentered(true, ChatFont, s, textPosition, nameFontColor, nameFontScale);

               spriteBatch.End();

               base.Draw(gameTime);
          }

          #endregion

          #region Disposal

          public override void UnloadContent()
          {
               TextManager.RemoveAll();
          }

          #endregion
     }
}