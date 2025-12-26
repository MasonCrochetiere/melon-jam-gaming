using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] GameObject target;
    [SerializeField, Range(0, 1)] float followEasingRatio = 0.3f;
    [SerializeField] Vector3 offset = new Vector3(0, 0, -10);

    [SerializeField] float targetZoom = 4;
    [SerializeField, Range(0, 1)] float zoomEasingRatio = 0.02f;

    // Update is called once per frame
    void FixedUpdate()
    {
        cam.transform.position = Vector3.Lerp(cam.transform.position, target.transform.position + offset, followEasingRatio);

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, zoomEasingRatio);
    }
}
