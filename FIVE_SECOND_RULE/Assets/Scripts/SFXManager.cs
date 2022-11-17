using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public bool awardPoint;

    public AudioClip Round_Swoosh;
    public AudioClip Jump_Up;
    public AudioClip Spring_SFX;

    public AudioClip Jump_Up_2;

    public AudioClip Flip_Card;

    public AudioClip Drum_Roll;

    public AudioClip Timer_Click;

    public AudioClip Time_Up;

    public AudioClip Hit_off_couch;

    public AudioClip Gain_point;

    public AudioClip No_point;

    public AudioClip Vote_Sound;

    public bool StartTally;
    public bool EndTally;
    public AudioSource Tally_Points;

    public AudioClip Banner_Drop;

    public void playSwoosh()
    {

        gameObject.GetComponent<AudioSource>().PlayOneShot(Round_Swoosh, 0.9F);
    }

    public void playJumpUp()
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(Jump_Up, 0.9F);


    }

    public void playJumpUp2()
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(Jump_Up_2, 0.9F);


    }

    public void playSpringSFX()
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(Spring_SFX, 0.9F);


    }

    public void playFlipCard()
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(Flip_Card, 0.9F);

    }

    public void playDrumRoll()
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(Drum_Roll, 0.9F);

    }

    public void playTimerClick()
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(Timer_Click, 0.9F);

    }

    public void playTimeUp()
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(Time_Up, 0.9F);

    }

    public void playHitOffCouch()
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(Hit_off_couch, 0.9F);

    }

    public void playawardPoint()
    {

        Debug.Log(awardPoint);

        if(awardPoint==true) gameObject.GetComponent<AudioSource>().PlayOneShot(Gain_point, 0.9F);

        else if (awardPoint==false) gameObject.GetComponent<AudioSource>().PlayOneShot(No_point, 0.9F);
    }


    public void playVoteSound()
    {

        gameObject.GetComponent<AudioSource>().PlayOneShot(Vote_Sound, 0.9F);

       
    }

    public void playTallyStopTally()
    {
        if(StartTally)
            transform.parent.GetComponent<AudioSource>().Play();

        else if(EndTally)
            transform.parent.GetComponent<AudioSource>().Stop();
    }


    public void playBannerDropSound()
    {

        gameObject.GetComponent<AudioSource>().PlayOneShot(Banner_Drop, 0.9F);


    }

}
