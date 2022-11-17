using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGAudioManager : MonoBehaviour
{

    AudioSource bgMusic;

    public AudioClip Lobby_BGM;

    public AudioClip Voting_BGM;

    public AudioClip[] GamePlay_BGM;

    public AudioClip GameOver_BGM;

    public int BGM_index;
    // Start is called before the first frame update


    public static BGAudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        bgMusic=gameObject.GetComponent<AudioSource>();
        BGM_index = 0;
    }

    // Update is called once per frame
   public void playLobbyBGM()
    {

        bgMusic.clip = Lobby_BGM;
        bgMusic.Play();
    }

    public void playVotingBGM()
    {

        bgMusic.clip = Voting_BGM;
        bgMusic.Play();
    }

    public void playGameBGM()
    {

        bgMusic.clip = GamePlay_BGM[BGM_index];
        bgMusic.Play();

        BGM_index = (BGM_index + 1)% GamePlay_BGM.Length;
    }

    public void stopBGM()
    {


        StartCoroutine(FadeOut(bgMusic,1f ));
    }


    public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }




    public void playGameOverBGM()
    {
        bgMusic.clip = GameOver_BGM;
        bgMusic.Play();
    }
}
