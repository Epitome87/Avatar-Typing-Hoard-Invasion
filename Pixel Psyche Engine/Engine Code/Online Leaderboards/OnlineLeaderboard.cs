

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PixelEngine.Screen;
#endregion

namespace PixelEngine
{
     public class OnlineLeaderboard
     {
          #region Fields

          // The current page being read / displayed on the Leaderboard.
          private int pageNumber = 0;

          // The amount of Leaderboard entries per page.
          private const int pageSize = 10;

          // Filter what mode's highscores are visible.
          public int gametypeFilter = 0;

          // Filter what player's highscores are visible.
          private int personFilter = 0;

          private string strPersonFilter;

          private string strModeFilter;

          // An array of top score entries, creating the current "page" the Leaderboard displays.
          private Leaderboards.TopScoreEntry[] page = new Leaderboards.TopScoreEntry[pageSize];

          // A means to hold, save, and load the high scores on the Leaderboard.
          public Leaderboards.TopScoreListContainer HighScoreSaveData;

          #endregion

          private bool showAll;
          private bool showFriends;
          private bool showMe;

          #region Properties

          public int PageNumber
          {
               get { return pageNumber; }
               set { pageNumber = value; }
          }

          public Leaderboards.TopScoreEntry[] Page
          {
               get { return page; }
               set {
                    int i = 0;
                    foreach (Leaderboards.TopScoreEntry entry in value)
                    {
                         page[i] = value[i];
                         i++;
                    }
                    //page = value; 
               }
          }

          public bool ShowAll
          {
               get { return showAll; }
               set { showAll = value; }
          }

          public bool ShowFriends
          {
               get { return showFriends; }
               set { showFriends = value; }
          }

          public bool ShowMe
          {
               get { return showMe; }
               set { showMe = value; }
          }

          public string CurrentPersonFilter
          {
               get { return strPersonFilter; }
          }

          public string CurrentModeFilter
          {
               get { return strModeFilter; }
          }

          #endregion

          #region Handle Input

          public bool wasInput = false;

          public void HandleInput(InputState input, PlayerIndex pIndex)
          {
               PlayerIndex playerIndex;

               wasInput = false;

               #region Handle Person Filtering Input

               if (input.IsNewButtonPress(Buttons.X, pIndex, out playerIndex)
                 || input.IsNewKeyPress(Keys.Enter, null, out playerIndex) || input.IsNewKeyPress(Keys.Space, null, out playerIndex))
               {
                    personFilter++;

                    if (personFilter > 2)
                    {
                         personFilter = 0;
                    }

                    switch (personFilter)
                    {
                         // Search Overall.
                         case 0:
                              showAll = true;
                              showFriends = false;
                              showMe = false;
                              strPersonFilter = "All";
                              break;

                         // Search Friends.
                         case 1:
                              showFriends = true;
                              showAll = false;
                              showMe = false;
                              strPersonFilter = "Friends";
                              break;

                         // Search My Scores.
                         case 2:
                              showMe = true;
                              showAll = false;
                              showFriends = false;
                              strPersonFilter = "My Score";
                              break;
                    }

                    wasInput = true;
               }

               #endregion

               #region Handle Game-Type Filtering Input

               if ((input.IsNewButtonPress(Buttons.DPadLeft, pIndex, out playerIndex))
                || (input.IsNewKeyPress(Keys.Left, null, out playerIndex)))
               {
                    // We changed Game Filters, so go back to Page 1.
                    pageNumber = 0;

                    if (gametypeFilter == 0)
                         gametypeFilter = 2;// gametypes.Length;

                    --gametypeFilter;

                    //strModeFilter = "ARCADE";


                    wasInput = true;
               }

               if ((input.IsNewButtonPress(Buttons.DPadRight, pIndex, out playerIndex))
                 || (input.IsNewKeyPress(Keys.Right, null, out playerIndex)))
               {

                    // We are changing the Game Filter, so we want to return to Page 1.
                    pageNumber = 0;

                    ++gametypeFilter;

                    if (gametypeFilter == 2)//gametypes.Length)
                         gametypeFilter = 0;

                    //strModeFilter = "SURVIVAL";


                    wasInput = true;
               }

               #endregion

               #region Handle Page Scrolling Input

               if ((input.IsNewButtonPress(Buttons.DPadUp, pIndex, out playerIndex))
                 || (input.IsNewKeyPress(Keys.Up, null, out playerIndex)))
               {
                    if (pageNumber <= 0)
                    {
                         pageNumber = (HighScoreSaveData.getFullListSize(gametypeFilter) - 1) / pageSize;
                    }

                    else
                    {
                         --pageNumber;
                    }


                    wasInput = true;
               }

               if ((input.IsNewButtonPress(Buttons.DPadDown, pIndex, out playerIndex))
                 || (input.IsNewKeyPress(Keys.Down, null, out playerIndex)))
               {
                    ++pageNumber;

                    if (pageNumber > (HighScoreSaveData.getFullListSize(gametypeFilter) - 1) / pageSize)
                    {
                         pageNumber = 0;
                    }


                    wasInput = true;
               }

               #endregion

               if (gametypeFilter == 0)
                    strModeFilter = "ARCADEEEE";
               else
                    strModeFilter = "SURVIVALLLL";
          }

          #endregion

          #region Update

          public void Update(GameTime gameTime)
          {
          }

          #endregion

          #region Draw

          public void Draw(GameTime gameTime)
          {
          }

          #endregion
     }
}
