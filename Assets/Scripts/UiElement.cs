using System.Collections;
using UnityEngine;

public class UiElement : MonoBehaviour
{
    private const float _spawnDistanceFromCamera = 0.75f;
    private OVRCameraRig _cameraRig;
    public void Awake()
    {
        _cameraRig = FindObjectOfType<OVRCameraRig>();

    }

    public void Start()
    {
        StartCoroutine(SnapCanvasInFrontOfCamera());
    }


    private IEnumerator SnapCanvasInFrontOfCamera()
    {
        yield return 0; // wait one frame to make sure the camera is set up
        if (_cameraRig)
        {
            transform.position = _cameraRig.centerEyeAnchor.transform.position +
                                 _cameraRig.centerEyeAnchor.transform.forward * _spawnDistanceFromCamera;
        }
    }
}
