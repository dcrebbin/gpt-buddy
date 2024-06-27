using System.Collections;
using UnityEngine;

public class BuddyController : MonoBehaviour
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

    public Vector3 offset = new Vector3(0, -1, 0);
    public void Start()
    {
        talkingCoroutine = Talking();
        _cameraRig = FindObjectOfType<OVRCameraRig>();
        transcriptionText = FindObjectOfType<UiElement>();
    }

    public void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            LookAtPlayer();
        }
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
        var offsetCamera = new Vector3(cameraRigTransform.position.x + offset.x, cameraRigTransform.position.y + offset.y, cameraRigTransform.position.z + offset.z);
        Vector3 prevTransform = transform.eulerAngles;
        transform.LookAt(offsetCamera);
        transform.eulerAngles = new Vector3(prevTransform.x, transform.eulerAngles.y, prevTransform.z);
        headAnchor.transform.LookAt(offsetCamera);
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
