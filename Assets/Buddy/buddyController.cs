using System.Collections;
using UnityEngine;

public class buddyController : MonoBehaviour
{
    public Material mouthOpened;
    public Material mouthSmiling;
    public bool isTalking = false;
    public UiElement transcriptionText;
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
        transcriptionText = FindObjectOfType<UiElement>();
        LookAtPlayer();
        MoveUiElement();
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
        transform.LookAt(cameraRigTransform.position);
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
