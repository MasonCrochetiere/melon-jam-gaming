using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

public class DashPoint : MonoBehaviour
{
    [SerializeField] GameObject dashAngleViewer;
    [SerializeField] Animator dashAngleAnimator;

    [SerializeField] SpriteRenderer spriteToSwitch;
    [SerializeField] Sprite newSprite;

    [SerializeField] ParticleSystem particle;
    [SerializeField] GameObject pointLight;

    [SerializeField] float unbreakDelay = 4f;
    Coroutine unbreakRoutine;

    bool angleLocked = false;

    bool broken = false;
    private void Start()
    {
        if (dashAngleViewer == null)
            return;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().SetDashPoint(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().RemoveDashPoint(this);
        }
    }

    public void ShowDashAngle(bool value)
    {
        if (dashAngleViewer == null)
            return;

        if (value)
        {
            dashAngleAnimator.SetBool("PointActive", true);
        }
        else
        {
            dashAngleAnimator.SetBool("PointActive", false);
            angleLocked = false;
        }
    }

    public void LockAngle()
    {
        angleLocked = true;

        spriteToSwitch.sprite = newSprite;
        particle.Play();

        if (!broken)
        {
            pointLight.gameObject.SetActive(false);
            AudioManager.instance.PlayeOneShot2D(FMODEvents.instance.cameraBreak);
            broken = true;

            if (unbreakRoutine != null)
                StopCoroutine(unbreakRoutine);
            StartCoroutine(UnbreakCoroutine(unbreakDelay));
        } 
    }

    public void UpdateDashAngle(float angle)
    {
        if (dashAngleViewer == null || angleLocked)
            return;

        dashAngleViewer.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    IEnumerator UnbreakCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        pointLight.gameObject.SetActive(true);
        broken = false;
    }
}
