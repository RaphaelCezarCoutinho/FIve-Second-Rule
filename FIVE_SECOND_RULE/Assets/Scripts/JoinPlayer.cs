using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinPlayer : MonoBehaviour
{

    public Text nameTextbox;
    public Image characterCard;
    public Image placeholder;
    public int placeholderId = 1;

    public GameObject Thumbs;
    public GameObject PlusPoint;

    public Animator anim;

    public GameObject HotSeatPlayer;
    public GameObject SteppingUpPlayer;
    public GameObject SteppingInPlayer;

    private int MoveToCenterOrReturn=0;
    private Vector3 StartPosition;

    public enum State
    {
        WAITING = 0,
        SELECTED = 1,
        REVEAL_TEAM = 2,
        HIDDEN = 3,
        OUT=4,
        MOVE_CENTER_GROW = 5,
        UPNEXT_POSITION = 6,

        PLACE_1=7,
        PLACE_2 = 8,
        PLACE_3 = 9,
        PLACE_4 = 10,
        PLACE_5 = 11,
        PLACE_6 = 12,


        TELEPORT_IN=13,
    }

    private Player data;

    private void Awake()
    {
        GetComponent<Animator>().Rebind();
        this.setState(State.WAITING);
        //placeholder.sprite = Resources.Load<Sprite>("Placeholders/Join_" + placeholderId);      
    }

    // Update is called once per frame
    void Update()
    {
        if (MoveToCenterOrReturn == 1)
        {


            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1.7f, 1.7f, 1.7f), 2 * Time.deltaTime);

            gameObject.transform.Find("CharacterSprite").position = Vector3.MoveTowards(gameObject.transform.Find("CharacterSprite").position, new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0), 400f * Time.deltaTime);
            gameObject.transform.Find("NameLabel").position = gameObject.transform.Find("CharacterSprite").position + gameObject.transform.Find("CharacterSprite").TransformDirection(new Vector3(0, -200, 0));
        }

        else if (MoveToCenterOrReturn == 2)
        {
            gameObject.transform.Find("CharacterSprite").position = Vector3.MoveTowards(gameObject.transform.Find("CharacterSprite").position, new Vector3(Screen.width * 0.7f, Screen.height * 0.5f, 0), 400f * Time.deltaTime);
            gameObject.transform.Find("NameLabel").position = gameObject.transform.Find("CharacterSprite").position + gameObject.transform.Find("CharacterSprite").TransformDirection(new Vector3(0, -100, 0));

        }
       

    }

 

    public void refreshData(Player playerData)
    {
        nameTextbox.text = playerData.name;

        //characterCard.sprite = Resources.Load<Sprite>("Characters/" + playerData.characterIndex + "-Neutral");
        data = playerData;
    }

    public void setName(string playerName)
    {

        nameTextbox.text = playerName;
    }

    public string getName()
    {

        return data.name;
    }

    public int getScore() {

        return data.score;
    }

    public string getId()
    {

        return data.id;
    }

    public void playAudioSource()
    {

        gameObject.GetComponent<AudioSource>().Play();
    }


    IEnumerator DelayAudio(float delay)
    {
        
        yield return new WaitForSeconds(delay);
        playAudioSource();
    }

    public void displayThumbs( bool up) {

        if (up)
        {

            GameObject Thumbs_prefab = Instantiate(Thumbs, gameObject.transform.Find("CharacterSprite"));
            Animator Thumbs_animator = Thumbs_prefab.GetComponentInChildren<Animator>();
            Thumbs_animator.SetBool("ThumbUp", true);
        }

        else {


            GameObject Thumbs_prefab = Instantiate(Thumbs, gameObject.transform.Find("CharacterSprite"));
            Animator Thumbs_animator = Thumbs_prefab.GetComponentInChildren<Animator>();
            Thumbs_animator.SetBool("ThumbDown", true);

        }
    }

    public void displayGainPoint() {

        GameObject GainPoint_prefab = Instantiate(PlusPoint, gameObject.transform.Find("CharacterSprite"));
        GainPoint_prefab.transform.localPosition = new Vector3(0, -55);
        Animator GainPoint_animator = GainPoint_prefab.GetComponentInChildren<Animator>();

        GainPoint_animator.SetBool("GainPoint", true);
    
    }

    public void KickHotSeatPlayer() {

        HotSeatPlayer.GetComponent<Animator>().Play("getKicked");    
    }

    public void PlayerStepUp()
    {
        SteppingUpPlayer.GetComponent<Animator>().Play("Step_Up");
    }

    public void PlayerStepIn()
    {
       SteppingInPlayer.GetComponent<Animator>().Play("Step_In");
    }


    public void setState(State state)
    {
        switch (state)
        {
            //    case State.WAITING:
            //        if (!gameObject.activeSelf) { gameObject.SetActive(true); };
            //      characterCard.enabled = false;
            //    break;

            case State.MOVE_CENTER_GROW:
                if (!gameObject.activeSelf) { gameObject.SetActive(true); };
                characterCard.enabled = true;
                StartPosition = gameObject.transform.position;
                MoveToCenterOrReturn = 1;
                break;
   
            case State.UPNEXT_POSITION:
                if (!gameObject.activeSelf) { gameObject.SetActive(true); };
                characterCard.enabled = true;
                MoveToCenterOrReturn = 2;
                break;
            case State.SELECTED:
                if (!gameObject.activeSelf) { gameObject.SetActive(true); };
                anim.SetTrigger("Character"+data.characterIndex + "_trigger");
                characterCard.enabled = true;
                StartCoroutine(DelayAudio(0.5f));
                
                break;
            case State.HIDDEN:
                gameObject.SetActive(false);
                break;
            case State.PLACE_1:
                anim.SetTrigger("Character" + data.characterIndex + "_trigger");
                characterCard.enabled = true;
                anim.SetBool("Scale_1", true);
                break;
            case State.PLACE_2:
                anim.SetTrigger("Character" + data.characterIndex + "_trigger");
                characterCard.enabled = true;
                anim.SetBool("Scale_2", true);
                break;
            case State.PLACE_3:
                anim.SetTrigger("Character" + data.characterIndex + "_trigger");
                characterCard.enabled = true;
                anim.SetBool("Scale_3", true);
                break;
            case State.PLACE_4:
                anim.SetTrigger("Character" + data.characterIndex + "_trigger");
                characterCard.enabled = true;
                anim.SetBool("Scale_4", true);
                break;
            case State.PLACE_5:
                anim.SetTrigger("Character" + data.characterIndex + "_trigger");
                characterCard.enabled = true;
                anim.SetBool("Scale_5", true);
                break;
            case State.PLACE_6:
                anim.SetTrigger("Character" + data.characterIndex + "_trigger");
                characterCard.enabled = true;
                anim.SetBool("Scale_6", true);
                break;
            case State.TELEPORT_IN:
                if (!gameObject.activeSelf) { gameObject.SetActive(true); };
                anim.SetBool("Teleport_in", true);
                anim.SetTrigger("Character" + data.characterIndex + "_trigger");
                characterCard.enabled = true;
                
                break;

        }
    }
}

