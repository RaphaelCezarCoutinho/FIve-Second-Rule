using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class SceneCode : MonoBehaviour
{


    public Text GameID;

    public Text GameRound;

    public Text MessagingText;

 
    public GameObject[] Players;
    public GameObject[] Scores;
    public GameObject[] Names;
    private int _lastPlayerCount = 0;

    public GameObject[] YesPlayers;
    public GameObject[] Noplayers;


    public GameObject DisplayPoint;
    public Sprite NoPoint;
    public Text squadCount;


    public Animator RoundCountAnimator;

    public Animator MessagingDropDownAnimator;

    public Animator WinnerCrown;
    public Animator WinnerBanner;

    public TextMeshProUGUI preCountDown;
    public TextMeshProUGUI TopicText;

    public Animator preCountDownAnimator;
    public Animator countdownAnimator;
    public Animator CardFlipAnimator;

    public GameObject CardBack;

    public Animator transition;

    void Awake()
    {
        GameManager.OnGameUpdate += updateScene;
        GameManager.OnGameSceneChange += OnGameSceneChange;
    }

    void OnDestroy()
    {
        GameManager.OnGameUpdate -= updateScene;
        GameManager.OnGameSceneChange -= OnGameSceneChange;
    }

    private void updateScene(GameData gameData)
    {
        if (gameData.state.ToUpper() != gameObject.scene.name.ToUpper()) { return; }


       
        switch (gameData.state)
        {
            case "LOBBY": LobbySceneUpdate(gameData); break;
            case "ROUND": RoundSceneUpdate(gameData); break;
            case "TURN_REVEAL": TurnRevealSceneUpdate(gameData); break;
            case "PLAYER_READY": PlayerReadySceneUpdate(gameData); break;
            case "GAME_PLAY": GamePlaySceneUpdate(gameData); break;
            case "POINT_VOTE": PointVoteSceneUpdate(gameData); break;
            case "GAME_OVER": GameOverSceneUpdate(gameData); break;
           
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // ValueTxt.text = GameManager.Instance.Value.ToString();
        Debug.Log("starting scene: " + gameObject.scene.name);

        if (GameManager.currentGameData != null)
            updateScene(GameManager.currentGameData);

        //Get background music
        GameObject bgMusic = GameObject.FindWithTag("BGAudioManager");
        BGAudioManager bgManager = bgMusic.GetComponent<BGAudioManager>();

        switch (gameObject.scene.name)
        {

            case "LOBBY": bgManager.playLobbyBGM();  break;
            case "POINT_VOTE": bgManager.playVotingBGM(); break;
            case "ROUND": break;
            case "GAME_OVER": bgManager.playGameOverBGM(); break;
            default: bgManager.playGameBGM(); break;
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //yesPointerAnim.SetTrigger("start");
        }
    }

    public void StartButton()
    {
        GameManager gameManager = GameManager.Instance;
        gameManager.startGame();
    }

    public void ContinueButton()
    {
        GameManager gameManager = GameManager.Instance;
        gameManager.continueGame();
    }

    public void NewGameButton()
    {
        GameManager gameManager = GameManager.Instance;
        gameManager.restartGame();
    }

    void OnGameSceneChange(string newSceneKey)
    {
        if (transition != null)
        {
            transition.SetTrigger("end");
        }

        GameObject bgMusic = GameObject.FindWithTag("BGAudioManager");
        BGAudioManager bgManager = bgMusic.GetComponent<BGAudioManager>();

        bgManager.stopBGM();
    }

    void LobbySceneUpdate(GameData gameData)
    {

        GameID.text = gameData.id;
        GameID.color = Color.white;
        updatePlayers(gameData, JoinPlayer.State.SELECTED);

        if (this._lastPlayerCount < gameData.players.Length && gameData.players.Length > 0)
        {
            
            this._lastPlayerCount = gameData.players.Length;
        }
    }

    void RoundSceneUpdate(GameData gameData)
    {

       

        switch (gameData.round) {

            

            case 1:
               
                StartCoroutine(delayAnimatorBool(1f, RoundCountAnimator, "IsRound1"));
                break;
            case 2:
                StartCoroutine(delayAnimatorBool(1f, RoundCountAnimator, "IsRound2"));
                break;
            case 3:
                StartCoroutine(delayAnimatorBool(1f, RoundCountAnimator, "IsRound3"));
                break;
            case 4:
                StartCoroutine(delayAnimatorBool(1f, RoundCountAnimator, "isRound4"));
                break;
            case 5:
                StartCoroutine(delayAnimatorBool(1f, RoundCountAnimator, "isRound5"));
                break;
            case 6:
                StartCoroutine(delayAnimatorBool(1f, RoundCountAnimator, "isRound6"));
                break;

        }
        //playThemeMusic(ingameTheme, 0.3f);
       // updatePlayers(gameData, JoinPlayer.State.SELECTED);
    }

    IEnumerator delayAnimatorBool(float delay, Animator animator, string animation_name)
    {

        Debug.Log(animation_name + "  hello from delayanimator  before delay" + delay);
        yield return new WaitForSeconds(delay);

        Debug.Log(animation_name + "  hello from delayanimator " +delay);
        animator.SetBool(animation_name, true);
    }

     void TurnRevealSceneUpdate(GameData gameData)
    {
        GameID.text = gameData.id;
        GameRound.text = "Round " + gameData.round;

        for (int i = 0; i < gameData.players.Length && i < 6; i++)
        {
            if (gameData.players[i].id == gameData.hotSeatPlayer.id) {

               

                    JoinPlayer player = Players[1].GetComponent<JoinPlayer>();
                    player.refreshData(gameData.players[i]);
                    player.setState(JoinPlayer.State.SELECTED);

                    StartCoroutine(delayAnim(player.GetComponent<Animator>(), "doKick"));

                    player = Players[2].GetComponent<JoinPlayer>();
                    player.refreshData(gameData.players[(i+1)%gameData.players.Length]);
                    player.setState(JoinPlayer.State.SELECTED);


                    player = Players[3].GetComponent<JoinPlayer>();
                    player.refreshData(gameData.players[(i + 2) % gameData.players.Length]);
                    player.setState(JoinPlayer.State.SELECTED);


            }

            if (gameData.lastHotSeatPlayer!=null && gameData.players[i].id == gameData.lastHotSeatPlayer.id)
            {



                JoinPlayer player = Players[0].GetComponent<JoinPlayer>();
                player.refreshData(gameData.players[i]);
                player.setState(JoinPlayer.State.SELECTED);

            }
        }

        if (gameData.hotSeatPlayer.id == gameData.turnPlayer.id)
        {

            MessagingText.text = gameData.hotSeatPlayer.name + " is starting the round";
            MessagingDropDownAnimator.SetBool("DropDown", true);
        }

        else {

            MessagingText.text = gameData.hotSeatPlayer.name + " has a chance to steal";
            MessagingDropDownAnimator.SetBool("DropDown", true);

        }
    }

   


   

   

    void PointVoteSceneUpdate(GameData gameData)
    {
        GameID.text = gameData.id;
        GameRound.text = "Round " + gameData.round;

        int YesInd = 0;
        int NoInd = 0;

        JoinPlayer HotSeatplayer = Players[0].GetComponent<JoinPlayer>();
        HotSeatplayer.refreshData(gameData.hotSeatPlayer);
        HotSeatplayer.setState(JoinPlayer.State.SELECTED);

        for (int i = 0; i < gameData.hotSeatPointVotes.Length; i++)
        {


            if (gameData.hotSeatPointVotes[i].isYes)
            {
                JoinPlayer player = YesPlayers[YesInd].GetComponent<JoinPlayer>();
                player.refreshData(gameData.hotSeatPointVotes[i].player);
                player.setState(JoinPlayer.State.TELEPORT_IN);
                YesInd++;
            }
            else
            {

                JoinPlayer player = Noplayers[NoInd].GetComponent<JoinPlayer>();
                player.refreshData(gameData.hotSeatPointVotes[i].player);
                player.setState(JoinPlayer.State.TELEPORT_IN);
                NoInd++;

            }
                
        }
        if (gameData.lastPointAwarded == 0)
        {


            DisplayPoint.GetComponent<SFXManager>().awardPoint = false;

            Image pointImage = DisplayPoint.GetComponent<Image>();
            pointImage.sprite = NoPoint;
            Animator pointAnim = DisplayPoint.GetComponent<Animator>();
            pointAnim.SetBool("DisplayP", true);

        }
        else if (gameData.lastPointAwarded == 1) {

            DisplayPoint.GetComponent<SFXManager>().awardPoint = true;

            Animator pointAnim = DisplayPoint.GetComponent<Animator>();
            pointAnim.SetBool("DisplayP", true);
        }
        

            MessagingText.text = "Look at your phone to vote";
        MessagingDropDownAnimator.SetBool("DropDown", true);
        

       

    }



    void PlayerReadySceneUpdate(GameData gameData) {


        JoinPlayer player = Players[0].GetComponent<JoinPlayer>();
        player.refreshData(gameData.hotSeatPlayer);
        player.setState(JoinPlayer.State.SELECTED);

        MessagingText.text = gameData.hotSeatPlayer.name + ", tap the button on your phone when ready";
        MessagingDropDownAnimator.SetBool("DropDown", true);

    }


    void GameOverSceneUpdate(GameData gameData) {

        System.Array.Sort(gameData.players, ScoreComparator);

        int[] placement = new int[gameData.players.Length];

        placement[0] = 1;


        for (int i = 1; i < gameData.players.Length && i < 6; i++)
        {
            if (gameData.players[i].score == gameData.players[i - 1].score)
            {

                placement[i] = placement[i - 1];

            }

            else placement[i] = placement[i - 1] + 1;
        }

        updatePlayers(gameData, JoinPlayer.State.SELECTED);

        for (int i = 0; i < gameData.players.Length && i < 6; i++)
        {
            JoinPlayer player = Players[i].GetComponent<JoinPlayer>();
            Text nameText = Names[i].GetComponent<Text>();
            Text scoreText = Scores[i].GetComponent<Text>();


            if(i== gameData.players.Length - 1)
            {
                player.GetComponent<SFXManager>().StartTally=true;

            }

            if (i == 0)
            {
                scoreText.GetComponent<SFXManager>().EndTally = true;

            }

            nameText.text = player.getName();
            scoreText.text = "" + player.getScore();

            switch (placement[i])
            {
                //Place 1 so all avatars same size
                case 1: StartCoroutine(delayScore(player, JoinPlayer.State.PLACE_1, gameData.players.Length-i-1)); break;
                case 2: StartCoroutine(delayScore(player, JoinPlayer.State.PLACE_1, gameData.players.Length-i-1)); break;
                case 3: StartCoroutine(delayScore(player, JoinPlayer.State.PLACE_1, gameData.players.Length-i-1)); break;
                case 4: StartCoroutine(delayScore(player, JoinPlayer.State.PLACE_1, gameData.players.Length-i-1)); break;
                case 5: StartCoroutine(delayScore(player, JoinPlayer.State.PLACE_1, gameData.players.Length-i-1)); break;
                case 6: StartCoroutine(delayScore(player, JoinPlayer.State.PLACE_1, gameData.players.Length-i-1)); break;

            }
            //switch (placement[i])
            //{

            //    case 1: player.setState(JoinPlayer.State.PLACE_1); break;
            //    case 2: player.setState(JoinPlayer.State.PLACE_2); break;
            //    case 3: player.setState(JoinPlayer.State.PLACE_3); break;
            //    case 4: player.setState(JoinPlayer.State.PLACE_4); break;
            //    case 5: player.setState(JoinPlayer.State.PLACE_5); break;
            //    case 6: player.setState(JoinPlayer.State.PLACE_6); break;

            //}
        }


        //MessagingText.text = gameData.players[0].name+" is the Winner";
    
    }


     int ScoreComparator(Player a, Player b)
    {
        return b.score - a.score;
       
    }


    void GamePlaySceneUpdate(GameData gameData) {        

        GameManager gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();        

        JoinPlayer player = Players[0].GetComponent<JoinPlayer>();
        player.refreshData(gameData.hotSeatPlayer);
        player.setState(JoinPlayer.State.SELECTED);
        bool startTurn = false;
       
        if(gameData.hotSeatPlayer.id== gameData.turnPlayer.id)
        {
            startTurn = true;
        }

       /* Debug.Log(next + " is the next number");
        JoinPlayer playerNext = Players[1].GetComponent<JoinPlayer>();
        playerNext.refreshData(gameData.players[next]);
        playerNext.setState(JoinPlayer.State.SELECTED);*/

        GameID.text = gameData.id;
        GameRound.text = "Round " + gameData.round;
        TopicText.text = gameData.topic;

        Debug.Log(gameData.timeDelta);
        StartCoroutine(preCountDownGamePlay(startTurn));
    }

    IEnumerator preCountDownGamePlay(bool startTurn) {



        if (startTurn)
        {

            yield return new WaitForSeconds(1f);
            CardFlipAnimator.SetBool("FlipCard", true);
            yield return new WaitForSeconds(1f);
        }

        else
        {
            CardFlipAnimator.SetBool("FlipCard", false);
            CardBack.SetActive(false);
            yield return new WaitForSeconds(1f);
        }
       
        
        preCountDownAnimator.SetBool("PreCountDownDropOff", true);
        countdownAnimator.SetBool("CountDownDropDown", true);
    }

    IEnumerator delayScore(JoinPlayer player, JoinPlayer.State state, int multiple)
    {
     
        yield return new WaitForSeconds(1f*multiple);
        player.setState(state);
    }

    IEnumerator delayAnim(Animator anim, string animationName)
    {

        yield return new WaitForSeconds(1.3f);
        anim.Play(animationName);
    }

    IEnumerator delayAnimSound(Animator animation, AudioSource audio, string trigger, float delay)
    {
        yield return new WaitForSeconds(delay);
        animation.SetTrigger(trigger);
        yield return new WaitForSeconds(.5f);
        audio.Play();
    }

    IEnumerator delayTransition(float delay, JoinPlayer player, Player playerData, JoinPlayer.State state)
    {
   
        yield return new WaitForSeconds(delay);

        player.refreshData(playerData);
        player.setState(state);
    }

   

 

    void updatePlayers(GameData gameData, JoinPlayer.State playerState)
    {
        for (int i = 0; i < gameData.players.Length && i < 6; i++)
        {
            JoinPlayer player = Players[i].GetComponent<JoinPlayer>();
            player.refreshData(gameData.players[i]);
            player.setState(playerState);
        }
    }

   

   

   

  


    void playThemeMusic(AudioClip selectedTheme, float volume)
    {

        GameObject currentAudio;
        currentAudio = GameObject.FindWithTag("game_Music");

        AudioSource audio = currentAudio.GetComponent<AudioSource>();

        audio.clip = selectedTheme;
        audio.volume = volume;
        audio.Play();
        Debug.Log("MADE IT PAST PLAY ");



    }

}
