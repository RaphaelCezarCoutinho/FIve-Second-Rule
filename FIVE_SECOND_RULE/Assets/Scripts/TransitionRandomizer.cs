using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionRandomizer : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private AnimatorOverrideController[] transitionControllers = new AnimatorOverrideController[5];
    [SerializeField]
    private Image[] images = new Image[0];

    void Start()
    {
        animator = this.gameObject.GetComponent<Animator>();
        for (int i = 0; i < images.Length; i += 1)
        {
            images[i].color = GameManager.Instance.rColor;
        }

        animator.runtimeAnimatorController = transitionControllers[GameManager.Instance.controllerNumber];
        StartCoroutine(LateStart(3));
    }

    IEnumerator LateStart(float time)
    {
        yield return new WaitForSeconds(time);
        RandomizeTransition();
        RandomizeColor();
    }

    void RandomizeTransition()
    {
        int r = Random.Range(0, transitionControllers.Length);
        animator.runtimeAnimatorController = transitionControllers[r];
        GameManager.Instance.controllerNumber = r;
    }

    void RandomizeColor()
    {
        Color rColor = Random.ColorHSV(0f, 1f, 0.8f, 1f);
        GameManager.Instance.rColor = rColor;
        for(int i = 0; i < images.Length; i += 1)
        {
            images[i].color = rColor;
        }
    }
}
