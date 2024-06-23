using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buddyController : MonoBehaviour
{
    public Material mouthOpened;
    public Material mouthSmiling;
    public bool isTalking = false;

    public SkinnedMeshRenderer head;
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
        head.material = mouthSmiling;
        isTalking = false;
        StopCoroutine(talkingCoroutine);
    }

    public void LookAtPlayer()
    {
        var cameraRigTransform = _cameraRig.centerEyeAnchor.transform;
        transform.LookAt(2 * transform.position - cameraRigTransform.position);
    }

    IEnumerator Talking()
    {
        while (isTalking)
        {
            head.material = mouthOpened;
            yield return new WaitForSeconds(talkingDelay);
            head.material = mouthSmiling;
            yield return new WaitForSeconds(talkingDelay);
        }
    }
}
