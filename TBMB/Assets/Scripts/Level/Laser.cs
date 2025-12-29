using UnityEngine;

public class Laser : MonoBehaviour
{
    [Header("Laser Settings")]
    public Transform laserOrigin;
    public Vector2 direction = Vector2.right;
    public float maxDistance = 50f;
    public LayerMask hitLayers;

    [Header("Visuals")]
    public SpriteRenderer laserSprite;

    [Header("Timing")]
    public float onDuration = 2f;
    public float offDuration = 1f;

    private bool isOn = true;
    private float timer;

    [SerializeField] float timeBeforeKillAgain = 0.9f;
    bool hasKilled;

    private void Start()
    {
        timer = onDuration;
    }

    private void Update()
    {
        UpdateTimer();

        if (isOn)
        {
            UpdateLaser();
        }
        else
        {
            laserSprite.enabled = false;
        }
    }

    private void UpdateTimer()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            isOn = !isOn;
            timer = isOn ? onDuration : offDuration;
        }
    }

    private void UpdateLaser()
    {
        Vector2 origin = laserOrigin != null ? laserOrigin.position : (Vector2)transform.position;
        Vector2 dir = direction.normalized;

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, maxDistance, hitLayers);

        float laserLength = maxDistance;
        if (hit.collider != null)
        {
            //laserLength = hit.distance;

            if (hit.distance <= (maxDistance * 0.0625))
            {
                Debug.Log("DISTANCE IS " + hit.distance);
                if (hit.collider.gameObject.CompareTag("Player") && !hasKilled)
                {
                    hit.collider.gameObject.GetComponent<PlayerController>().KillPlayer();
                    hasKilled = true;

                    Invoke("ResetKilled", timeBeforeKillAgain);
                }
            }
        }
        else
        {
            //laserLength = maxDistance;
        }

        laserSprite.enabled = true;

        Vector2 endPoint = origin + dir * laserLength;
        laserSprite.transform.position = (origin + endPoint) / 2f;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        laserSprite.transform.rotation = Quaternion.Euler(0, 0, angle);

        Vector3 scale = laserSprite.transform.localScale;
        scale.x = laserLength;
        laserSprite.transform.localScale = scale;
    }

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;
    }

    void ResetKilled()
    {
        hasKilled = false;
    }
}