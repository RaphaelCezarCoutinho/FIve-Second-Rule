using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayScoreName : MonoBehaviour
{
    public GameObject NameObj;
    // Start is called before the first frame update
    public void PlayNameAnimation()
    {

        Animator anim = NameObj.GetComponent<Animator>();

        anim.Play("ScoreText1Fade");

    }
}
