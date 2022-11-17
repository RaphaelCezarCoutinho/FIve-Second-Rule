using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class GameManager : MonoBehaviour
{
    public static string localTest = "http://localhost:3000";
    public static string webTest= "http://things-fsr.azurewebsites.net";

    public static string apiURL = webTest;
    public string gameID = "";

    public static event Action<GameData> OnGameUpdate;
    public static event Action<String> OnGameSceneChange;
    public static GameData currentGameData;

    // public GameState state;
    public static GameManager Instance { get; private set; }
    private long stateLastUpdated = 0;
    private long lastUpdated = 0;

    [HideInInspector]
    public int controllerNumber;
    [HideInInspector]
    public Color rColor;
    [SerializeField]
    private int changeSceneDelay = 3;
    [HideInInspector]
    public int gameNumber = 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            createGame();

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("start gamemanager");
    }

    void callBackend()
    {
        if (gameID != "")
        {
            StartCoroutine(getRequest(apiURL + "/game/" + gameID));
        }
    }    

    IEnumerator getRequest(string uri)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            print("Error While Sending: " + uwr.error);
        }
        else
        {
            currentGameData = JsonUtility.FromJson<GameData>(uwr.downloadHandler.text);
            updateGameData(currentGameData);
        }
    }

    void updateGameData(GameData gameData)
    {
        if (stateLastUpdated != gameData.stateLastUpdated)
        {
            stateLastUpdated = gameData.stateLastUpdated;
            switch (gameData.state)
            {

                case "ROUND": StartCoroutine(changeScene("ROUND")); break;
                case "TURN_REVEAL": StartCoroutine(changeScene("TURN_REVEAL")); break;
                case "PLAYER_READY": StartCoroutine(changeScene("PLAYER_READY")); break;
                case "GAME_PLAY": StartCoroutine(changeScene("GAME_PLAY")); break;
                case "POINT_VOTE": StartCoroutine(changeScene("POINT_VOTE")); break;
                case "GAME_OVER": StartCoroutine(changeScene("GAME_OVER")); break;

            }
            OnGameUpdate?.Invoke(gameData);
        }

        if (lastUpdated != gameData.lastUpdated)
        {
            lastUpdated = gameData.lastUpdated;
            OnGameUpdate?.Invoke(gameData);
        }
    }

    IEnumerator changeScene(string sceneKey)
    {
        OnGameSceneChange?.Invoke(sceneKey);
        yield return new WaitForSeconds(changeSceneDelay);
        UnitySceneManager.LoadScene(sceneKey);
    }

    void createGame()
    {
        StartCoroutine(postRequest(apiURL + "/game"));
    }
    
    public void startGame()
    {
        gameNumber = 1;
        StartCoroutine(postRequestAction(apiURL, "event", "START"));
    }

    public void continueGame()
    {
        gameNumber += 1;
        StartCoroutine(postRequestAction(apiURL, "event", "CONTINUE"));
    }

    public void restartGame()
    {
        StartCoroutine(postRequestAction(apiURL, "event", "RESTART"));
    }

    public void stopTime()
    {
       // StartCoroutine(postRequestAction(apiURL, "event", "STOP"));
    }


    public void startAppClock()
    {
         StartCoroutine(postRequestAction(apiURL, "event", "TIMESTART"));
    }

    IEnumerator postRequestAction(string uri, string key, string value)
    {
        WWWForm form = new WWWForm();
        form.AddField(key, value);
        UnityWebRequest uwr = UnityWebRequest.Post(apiURL + "/action/"+gameID, form);


        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(uwr.error);
            uwr.Dispose();
        }
        else
        {
            GameData gameData = JsonUtility.FromJson<GameData>(uwr.downloadHandler.text);

            uwr.Dispose();
        }
    }

    IEnumerator postRequest(string uri)
    {
        WWWForm form = new WWWForm();
        UnityWebRequest uwr = UnityWebRequest.Post(apiURL + "/game", form);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(uwr.error);
        }
        else
        {
            GameData gameData = JsonUtility.FromJson<GameData>(uwr.downloadHandler.text);

            gameID = gameData.id;
            Debug.Log("New GAME: " + gameID);
            InvokeRepeating("callBackend", 1, 1);
        }
    }


}

public enum GameState
{
    LOBBY,
    ROUND,
    TURN_REVEAL,
    PLAYER_READY,
    GAME_PLAY,
    POINT_VOTE,
    GAME_OVER
}


[Serializable]
public class Player
{
    public string id;
    public string name;
    public long characterIndex;
    public int score;
}
[Serializable]
public class Vote
{
    public Player player;
    public Boolean isYes;
}

[Serializable]
public class Mission
{
    public int players;
    public int good;
    public int bad;
    public Rounds rounds;
}

[Serializable]
public class Rounds
{
    public Round mission_1;
    public Round mission_2;
    public Round mission_3;
    public Round mission_4;
    public Round mission_5;
}

[Serializable]
public class Round
{
    public int players;
}

[Serializable]
public class GameData
{
    public string id;
    public Player[] players;
    public Player hotSeatPlayer;
    public Player lastHotSeatPlayer;
    public Player turnPlayer;
    public Player[] goodTeam;
    public Player[] badTeam;
    public Player[] squad;
    public Player leader;
    public Player winner;
    public int goodPoints;
    public int badPoints;
    public string lastPointTeam;
    public string winningTeam;
    public bool isSquadVoteYes;
    public bool isMissionVoteYes;
    public int round;
    public long timeDelta;
    public int lastPointAwarded;
    public Vote[] squadVotes;
    public Vote[] missionVotes;
    public string topic;

    
    public Vote[] hotSeatPointVotes;

    public string state;
    public long stateLastUpdated;
    public long lastUpdated;
    public Mission mission;
}



