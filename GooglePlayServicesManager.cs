using UnityEngine;
using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine.SocialPlatforms;

public class GooglePlayServicesManager : MonoBehaviour
{
    private string mStatus;
    private Texture2D savedImage;
    public bool isSaving;
    private Double TotalPlaytime;
    private string savedDataString;
    private bool mSaved;
    private bool mLoaded;

    // Use this for initialization
    void Start()
    {
        savedDataString = "";
        mSaved = false;
        mLoaded = false;
        isSaving = false;
        mStatus = "";
        savedImage = null;
        TotalPlaytime = 0;

        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        // enables saving game progress.
        .EnableSavedGames()
        .Build();
        /*       // registers a callback to handle game invitations received while the game is not running.
               .WithInvitationDelegate(< callback method >)
               // registers a callback for turn based match notifications received while the
               // game is not running.
               .WithMatchDelegate(< callback method >)
               // require access to a player's Google+ social graph (usually not needed)
               .RequireGooglePlus()
               .Build();
       */

        PlayGamesPlatform.InitializeInstance(config);
        // recommended for debugging:
        PlayGamesPlatform.DebugLogEnabled = true;
        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();

    }

    /// <summary>
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    ///     SIGN IN / OUT SECTION
    ///     
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    // Sign Player into Google Play Services
    public void SignIn()
    {
        // authenticate user:
        Social.localUser.Authenticate((bool success) =>
        {
            // handle success or failure
            if(success)
            {
                Debug.Log("Log-In Successful");
            }
            else
            {
                Debug.Log("Failed To Log-In");
            }
        });
    }

    // Sign Playr out of Google Play Services
    public void SignOut()
    {
        // sign out
        PlayGamesPlatform.Instance.SignOut();
    }

    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    ///     PLAYER INFO SECTION
    ///     
    /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    // Player Avatar URL
    public string GetAvatarURL()
    {
        return ((PlayGamesLocalUser)Social.localUser).AvatarURL;
    }

    // Player Avatar Image (Texture2D)
    public Texture2D GetAvatarImage()
    {
        return Social.localUser.image;
    }

    // Player Stats
    public PlayerStats Get_PlayerStats()
    {
        PlayerStats plrStats = new PlayerStats();

        ((PlayGamesLocalUser)Social.localUser).GetStats((rc, stats) =>
        {
            // -1 means cached stats, 0 is succeess
            // see  CommonStatusCodes for all values.
            if(rc <= 0 && stats.HasDaysSinceLastPlayed())
            {
                Debug.Log("It has been " + stats.DaysSinceLastPlayed + " days");
            }
            plrStats = stats;
        });

        return plrStats;
    }

    // Load Friends
    public void Load_Friends()
    {
        Social.localUser.LoadFriends((ok) =>
        {
            Debug.Log("Friends loaded OK: " + ok);
            foreach(IUserProfile p in Social.localUser.friends)
            {
                Debug.Log(p.userName + " is a friend");
            }
        });
    }

    /// <summary>
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    ///     ACHIEVEMENTS SECTION
    ///     
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    // Unlock Achievement Based On ID
    public void Unlock_Achievement(string Achievement_ID)
    {
        Social.ReportProgress(Achievement_ID, 100.0f, (bool success) =>
        {
            // handle success or failure
            if(success)
            {
                Debug.Log("Achievement Unlocked");
            }
            else
            {
                Debug.Log("Failed To Unlock Achievement");
            }
        });
    }

    // Increment Achievement Based On ID
    public void Incremental_Achievement(string Achievement_ID, int No_Of_Steps)
    {
        PlayGamesPlatform.Instance.IncrementAchievement(
            Achievement_ID, No_Of_Steps, (bool success) =>
            {
                // handle success or failure
                if(success)
                {
                    Debug.Log("Achievement Incremented");
                }
                else
                {
                    Debug.Log("Failed To Increment Achievement");
                }
            });
    }

    // Show Achievements UI
    public void Show_Achievements_UI()
    {
        // show achievements UI
        Social.ShowAchievementsUI();
    }

    // Check if achievement is Unlocked
    public bool Achievment_Unlocked(string Achievement)
    {
#if UNITY_ANDROID
        if(PlayGamesPlatform.Instance.GetAchievement(Achievement).IsUnlocked)
        {
            return true;
        }
        return false;
#endif
    }

    /// <summary>
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    ///     LEADERBOARD SECTION
    ///     
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    // Show Leaderboard UI
    public void Show_Leaderboard_UI()
    {
        // show leaderboard UI
        Social.ShowLeaderboardUI();
    }

    // Post Score To Leaderboard
    public void Post_Score_To_Leaderboard(int Score, string Leaderboard_ID)
    {
        // post score 12345 to leaderboard ID "Cfji293fjsie_QA")
        Social.ReportScore(Score, Leaderboard_ID, (bool success) =>
        {
            // handle success or failure
            if(success)
            {
                Debug.Log("Score Posted To Leaderboard");
            }
            else
            {
                Debug.Log("Failed To Post Score To Leaderboard");
            }
        });
    }

    // Show Specific Leaderboard UI
    public void Show_Leaderboard_UI(string Leaderboard_ID)
    {
        // show leaderboard UI
        PlayGamesPlatform.Instance.ShowLeaderboardUI(Leaderboard_ID);
    }

    // Load Leaderboard Scores
    public void Load_Leaderboard_Scores(string Leaderboard_ID, int Row_Count = 100)
    {
        PlayGamesPlatform.Instance.LoadScores(
        Leaderboard_ID,
        LeaderboardStart.PlayerCentered,
        Row_Count,
        LeaderboardCollection.Public,
        LeaderboardTimeSpan.AllTime,
        (data) =>
        {
            mStatus = "Leaderboard data valid: " + data.Valid;
            mStatus += "\n approx:" + data.ApproximateCount + " have " + data.Scores.Length;
        });
    }

    // Get Next Leaderboard Page
    public void GetNextPage(LeaderboardScoreData data)
    {
        PlayGamesPlatform.Instance.LoadMoreScores(data.NextPageToken, 10,
            (results) =>
            {
                mStatus = "Leaderboard data valid: " + data.Valid;
                mStatus += "\n approx:" + data.ApproximateCount + " have " + data.Scores.Length;
            });
    }

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    ///     EVENT SECTION
    ///     
    /// //////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    // Increment (Record) An Event
    public void Increment_Event(string Event_ID)
    {
        // Increments the event with Id "YOUR_EVENT_ID" by 1
        PlayGamesPlatform.Instance.Events.IncrementEvent(Event_ID, 1);
    }

    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    ///     SAVED GAME SELECTION SECTION
    ///     
    /// ///////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    // Show game save UI
    public void ShowSelectUI()
    {
        uint maxNumToDisplay = 3;
        bool allowCreateNew = false;
        bool allowDelete = true;

        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.ShowSelectSavedGameUI("Select saved game",
            maxNumToDisplay,
            allowCreateNew,
            allowDelete,
            OnSavedGameSelected);
    }

    // Use when open save file is selected
    public void OnSavedGameSelected(SelectUIStatus status, ISavedGameMetadata game)
    {
        if(status == SelectUIStatus.SavedGameSelected)
        {
            // handle selected game save
            LoadGameData(game);
        }
        else
        {
            // handle cancel or error
        }
    }

    /// <summary>
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    ///     OPEN FILE HANDLER
    ///     
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    // Open Save File
    private void OpenSavedGame(string filename)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpened);
    }

    // Use to do something when Game Save File is Open
    public void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if(status == SavedGameRequestStatus.Success)
        {
            if(isSaving)
            {
                // handle reading or writing of saved game.
                Byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(GetDataString());
                TimeSpan playedTime = TimeSpan.FromSeconds(TotalPlaytime);
                SaveGame(game, data, playedTime);
            }
            else
            {
                LoadGameData(game);
            }
        }
        else
        {
            // handle error
        }
    }

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    ///     SAVE GAME SECTION
    ///         
    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    // Save Game Wrapper
    public void SAVE_GAME(string filename, string dataSring)
    {
        isSaving = true;
        LoadDataFromString(dataSring);
        OpenSavedGame(filename);
    }

    // Save Game Function
    void SaveGame(ISavedGameMetadata game, byte[] savedData, TimeSpan totalPlaytime)
    {
        savedImage = getScreenshot();

        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
        builder = builder
            .WithUpdatedPlayedTime(totalPlaytime)
            .WithUpdatedDescription("Saved game at " + DateTime.Now);
        if(savedImage != null)
        {
            // This assumes that savedImage is an instance of Texture2D
            // and that you have already called a function equivalent to
            // getScreenshot() to set savedImage
            // NOTE: see sample definition of getScreenshot() method below
            byte[] pngData = savedImage.EncodeToPNG();
            builder = builder.WithUpdatedPngCoverImage(pngData);
        }
        SavedGameMetadataUpdate updatedMetadata = builder.Build();
        savedGameClient.CommitUpdate(game, updatedMetadata, savedData, OnSavedGameWritten);
    }

    // Use to do something after Data has been saved
    public void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if(status == SavedGameRequestStatus.Success)
        {
            // handle reading or writing of saved game.
            Debug.Log("Game Data Saved To The Cloud SUCCESSFULLY !");
            mSaved = true;
        }
        else
        {
            // handle error
            mSaved = false;
        }
    }

    // Capture Screen as screenshot
    private Texture2D getScreenshot()
    {
        int textureWidth = 1024;
        int textureHeight = 700;

        // Create a 2D texture that is 1024x700 pixels from which the PNG will be
        // extracted
        Texture2D screenShot = new Texture2D(textureWidth, textureHeight);

        // Takes the screenshot from top left hand corner of screen and maps to top
        // left hand corner of screenShot texture
        screenShot.ReadPixels(
            new Rect(0, 0, Screen.width, (Screen.width / textureWidth) * textureHeight), 0, 0);
        return screenShot;
    }

    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    ///     LOAD GAME SECTION
    ///     
    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    // Load Game Wrapper
    public void LOAD_GAME(string filename)
    {
        isSaving = false;
        OpenSavedGame(filename);
    }

    // Load Game Data from Game MetaData
    private void LoadGameData(ISavedGameMetadata game)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.ReadBinaryData(game, OnSavedGameDataRead);
    }

    // Load Data Callback
    public void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] data)
    {
        if(status == SavedGameRequestStatus.Success)
        {
            // handle processing the byte array data
            LoadDataToString(data);
            mLoaded = true;
        }
        else
        {
            mLoaded = false;
            // handle error
        }
    }

    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    ///     DELETE GAME SAVE
    ///         
    /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    /// Delete Game Data
    void DeleteGameData(string filename)
    {
        // Open the file to get the metadata.
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.OpenWithAutomaticConflictResolution(filename, DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime, DeleteSavedGame);
    }

    public void DeleteSavedGame(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if(status == SavedGameRequestStatus.Success)
        {
            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.Delete(game);
        }
        else
        {
            // handle error
        }
    }

    /// <summary>
    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// 
    ///     HELPERS
    ///         
    /// ////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    /// <returns></returns>
    public string GetDataString()
    {
        return savedDataString;
    }

    public string[] GetDataStringArray() // ' ' (space) is the Split char
    {
        string[] array = savedDataString.Split(' ');

        return array;
    }

    public void LoadDataToString(byte[] data)
    {
        savedDataString = System.Text.ASCIIEncoding.ASCII.GetString(data);
    }

    public void LoadDataFromString(string data)
    {
        savedDataString = data;
    }

    public bool isSaved()
    {
        return mSaved;
    }

    public bool isLoaded()
    {
        return mLoaded;
    }
}


