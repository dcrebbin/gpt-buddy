using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buddyController : MonoBehaviour
{
    public GameObject mouthOpened;
    public GameObject mouthClosed;
    public bool isTalking = false;
    public float talkingDelay = 0.1f;

    public IEnumerator talkingCoroutine;

    public float walkingSpeed = 0.05f;

    public float timer = 0;

    private OVRCameraRig _cameraRig;

    public void Start()
    {
        talkingCoroutine = Talking();
        _cameraRig = FindObjectOfType<OVRCameraRig>();
    }

    public void StartTalking()
    {
        LookAtPlayer();
        isTalking = true;
        StartCoroutine(talkingCoroutine);
    }

    public void StopTalking()
    {
        mouthOpened.SetActive(false);
        mouthClosed.SetActive(true);
        isTalking = false;
        StopCoroutine(talkingCoroutine);
    }

    public void LookAtPlayer()
    {
        //look at camera
        var cameraRigTransform = _cameraRig.centerEyeAnchor.transform;
        transform.LookAt(2 * transform.position - cameraRigTransform.position);
    }

    IEnumerator Talking()
    {
        while (isTalking)
        {
            mouthOpened.SetActive(true);
            mouthClosed.SetActive(false);
            yield return new WaitForSeconds(talkingDelay);
            mouthOpened.SetActive(false);
            mouthClosed.SetActive(true);
            yield return new WaitForSeconds(talkingDelay);
        }
    }
}
