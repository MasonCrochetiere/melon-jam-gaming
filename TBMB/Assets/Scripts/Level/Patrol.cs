using UnityEngine;

public class Patrol : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    private Transform targetPoint;

    void Start()
    {
        targetPoint = pointB;
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            targetPoint = targetPoint == pointA ? pointB : pointA;

            Vector3 currentRotation = transform.rotation.eulerAngles;
            if (targetPoint == pointA)
            {
                transform.rotation = Quaternion.Euler(currentRotation.x, 180f, currentRotation.z);
            }
            else
            {
                transform.rotation = Quaternion.Euler(currentRotation.x, 0f, currentRotation.z);
            }
        }
    }
}