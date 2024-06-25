using System.Collections;
using UnityEngine;

public class buddyController : MonoBehaviour
{
    public Material mouthOpened;
    public Material mouthSmiling;
    public bool isTalking = false;
    public UiElement transcriptionText;
    public SkinnedMeshRenderer head;
    public GameObject headAnchor;
    public float talkingDelay = 0.1f;
    public IEnumerator talkingCoroutine;
    public float walkingSpeed = 0.05f;
    public float timer = 0;
    private OVRCameraRig _cameraRig;
    public void Start()
    {
        talkingCoroutine = Talking();
        _cameraRig = FindObjectOfType<OVRCameraRig>();
        transcriptionText = FindObjectOfType<UiElement>();
    }

    private void MoveUiElement()
    {
        Transform uiElementTransform = transcriptionText.transform;
        uiElementTransform.position = new Vector3(transform.position.x, transform.position.y + 0.65f, transform.position.z);
        uiElementTransform.LookAt(2 * uiElementTransform.position - _cameraRig.centerEyeAnchor.transform.position);
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
        var cameraToHead = cameraRigTransform.position - headAnchor.transform.position;
        var angle = Vector3.Angle(cameraToHead, Vector3.up);

        if (angle > 30)
        {
            transform.LookAt(cameraRigTransform.position);
            headAnchor.transform.LookAt(cameraRigTransform.position);
        }
        else
        {
            var lookPosition = new Vector3(cameraRigTransform.position.x, cameraRigTransform.position.y + 30f, cameraRigTransform.position.z);
            headAnchor.transform.LookAt(lookPosition);
        }
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
