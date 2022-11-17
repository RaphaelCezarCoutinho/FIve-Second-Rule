using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startServerStopButton : MonoBehaviour
{
    // Start is called before the first frame update
    public void startClock()
    {

        GameManager gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        gameManager.startAppClock();

    }
}
