using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buddyController : MonoBehaviour
{

    public GameObject mouthOpened;
    public GameObject mouthClosed;

    public bool isTalking = false;

    public float talkingDelay = 0.2f;
    // Start is called before the first frame update

    public void StartTalking()
    {
        isTalking = true;
        StartCoroutine(Talking());
    }

    public void StopTalking()
    {
        isTalking = false;
        mouthOpened.SetActive(false);
        mouthClosed.SetActive(true);
        StopCoroutine(Talking());
    }

    IEnumerator Talking()
    {
        while (true)
        {
            mouthOpened.SetActive(true);
            mouthClosed.SetActive(false);
            yield return new WaitForSeconds(talkingDelay);
            mouthOpened.SetActive(true);
            mouthClosed.SetActive(false);
        }
    }
}
