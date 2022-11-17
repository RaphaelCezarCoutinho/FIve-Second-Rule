using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayScoreNum : MonoBehaviour
{


    public GameObject ScoreObj;
    // Start is called before the first frame update
    public void PlayScoreAnimation() {

        Animator anim = ScoreObj.GetComponent<Animator>();

        anim.Play("ScoreNum1Fade");
    
    }
}
