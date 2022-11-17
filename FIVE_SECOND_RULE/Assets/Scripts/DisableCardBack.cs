using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableCardBack : MonoBehaviour
{

    public GameObject cardBack;
    // Start is called before the first frame update
    public void disableCback() {

        cardBack.SetActive(false);
    
    }
}
