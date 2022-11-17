using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextUpdater : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI text;

    void Awake()
    {
        text.text = GameManager.Instance.gameNumber.ToString();
    }
}