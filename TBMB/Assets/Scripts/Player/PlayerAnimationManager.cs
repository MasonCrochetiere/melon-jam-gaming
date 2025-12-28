using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public enum MaskType { Bag, Jump, Dash, Ball, None }

public class PlayerAnimationManager : MonoBehaviour
{
    [SerializeField] float DashAnimationDuration = 0.25f;
    [SerializeField] const float delayBeforeBag = 4f;

    [SerializeField] SpriteLibraryAsset bagLibrary;
    [SerializeField] SpriteLibraryAsset jumpLibrary;
    [SerializeField] SpriteLibraryAsset dashLibrary;
    [SerializeField] SpriteLibraryAsset ballLibrary;
    [SerializeField] SpriteLibraryAsset noneLibrary;

    [SerializeField] List<Material> particleMaterials;
    [SerializeField] ParticleSystem maskSwitchParticle;
    ParticleSystemRenderer particleRenderer;

    [SerializeField] List<MaskTrailItem> trailItems;
    int unlockIndex = 0;
    [SerializeField] float trailSwitchDelay = 0.1f;

    [SerializeField] ParticleSystem footstepParticle;
    [SerializeField] ParticleSystem landingParticle;
    [SerializeField] ParticleSystem bounceParticle;
 
    MaskType currentType;
    Coroutine maskSwitchCoroutine;

    Animator animator;
    SpriteLibrary spriteLibrary;
    SpriteResolver resolver;
    SpriteRenderer spriteRenderer;

    bool overrideIdle = false;
    bool onGround = true;

    float moveValueX;

    bool dashOverride = false;
    bool ballOverride = false;

    public float lastRotation = 0f;

    bool auraUnlocked = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteLibrary = GetComponent<SpriteLibrary>();
        resolver = GetComponent<SpriteResolver>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        particleRenderer = maskSwitchParticle.GetComponent<ParticleSystemRenderer>();

        foreach (MaskTrailItem item in trailItems)
        {
            item.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        animator.SetBool("OnGround", onGround);
        animator.SetFloat("MoveXMagnitude", Mathf.Abs(moveValueX));

        if (!dashOverride && !ballOverride)
        {
            if (moveValueX < 0)
            {
                lastRotation = 180f;
            }
            else if (moveValueX > 0)
            {
                lastRotation = 0f;
            }

            gameObject.transform.rotation = Quaternion.Euler(0f, lastRotation, 0f);
        }
    }

    public void SetMaskType(MaskType maskType)
    {
        if (currentType == MaskType.None)
            return;


        if (spriteRenderer.enabled)
        {
            // Play the particle of the old mask flying off
            if (currentType != maskType)
            {
                switch (currentType)
                {
                    case MaskType.Bag:
                        particleRenderer.material = particleMaterials[0];
                        break;
                    case MaskType.Jump:
                        particleRenderer.material = particleMaterials[1];
                        break;
                    case MaskType.Dash:
                        particleRenderer.material = particleMaterials[2];
                        break;
                    case MaskType.Ball:
                        particleRenderer.material = particleMaterials[3];
                        break;
                    case MaskType.None:
                        particleRenderer.material = particleMaterials[4];
                        break;
                }
                maskSwitchParticle.Play();
            }
        }

        // Set the new mask by using the sprite library and resolver.
        switch (maskType)
        {
            case MaskType.Bag:
                spriteLibrary.spriteLibraryAsset = bagLibrary;
                break;
            case MaskType.Jump:
                spriteLibrary.spriteLibraryAsset = jumpLibrary;
                break;
            case MaskType.Dash:
                spriteLibrary.spriteLibraryAsset = dashLibrary;
                break;
            case MaskType.Ball:
                spriteLibrary.spriteLibraryAsset = ballLibrary;
                break;
            case MaskType.None:
                spriteLibrary.spriteLibraryAsset = noneLibrary;
                break;
        }

        resolver.ResolveSpriteToSpriteRenderer();

        resolver.enabled = false;
        resolver.enabled = true;

        // Update the trail inventory accordingly
        foreach (MaskTrailItem item in trailItems)
        {
            if (item.type == maskType && item.gameObject.activeSelf)
            {
                item.SetType(currentType);
                item.sprite.enabled = false;

                StartCoroutine(SwitchParticleWithTrail(trailSwitchDelay, item));
                break;
            }
        }

        currentType = maskType;
    }

    public void OnItemUnlocked(ItemList item)
    {
        trailItems[unlockIndex].gameObject.SetActive(true);

        unlockIndex++;

        if (item == ItemList.Aura)
            auraUnlocked = true;
    }

    public void StartBagSwitchDelay(float delay = delayBeforeBag)
    {
        if (maskSwitchCoroutine != null)
        {
            StopCoroutine(maskSwitchCoroutine);
        }
        maskSwitchCoroutine = StartCoroutine(ResetToBagRoutine(delay));
    }

    void StopBagSwitch()
    {
        if (maskSwitchCoroutine != null)
        {
            StopCoroutine(maskSwitchCoroutine);
        }
    }

    public void playAnimation(string name)
    {
        animator.SetTrigger(name);
    }

    public void setAnimationOverride(bool value)
    {
        overrideIdle = value;
        animator.SetBool("AnimOverride", overrideIdle);

        if (!value)
        {
            dashOverride = false;
            ballOverride = false;   
        }
    }

    public void playDashOverride(float angle)
    {
        dashOverride = true;
        setAnimationOverride(true);

        animator.SetTrigger("Dash");

        switch (angle)
        {
            case 30f:
                gameObject.transform.rotation = Quaternion.Euler(180f, 180f, 135f);
                break;
            case 150:                
                gameObject.transform.rotation = Quaternion.Euler(180f, 0f, 135f);
                break;
            case 210:
                gameObject.transform.rotation = Quaternion.Euler(180f, 0f, 45f);
                break;
            case 330:
                gameObject.transform.rotation = Quaternion.Euler(180f, 180f, 45f);
                break;
            case 90:
                if (moveValueX < 0)
                {
                    gameObject.transform.rotation = Quaternion.Euler(0f, 180f, -90f);
                }
                else if (moveValueX > 0)
                {
                    gameObject.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
                }
                break;
        }

        Invoke("EndDashOverride", DashAnimationDuration);

        StopBagSwitch();
        SetMaskType(MaskType.Dash);
    }

    void EndDashOverride()
    {
        if (!ballOverride)
            setAnimationOverride(false);
    }

    public void playBallOverride()
    {
        ballOverride = true;
        setAnimationOverride(true);

        animator.SetTrigger("Ball");

        StopBagSwitch();
        SetMaskType(MaskType.Ball);
    }

    public void setOnGround(bool value)
    {
        onGround = value;
    }

    public void setMoveValueX(float value)
    {
        moveValueX = value;
    }

    public void UpdateJump(bool value)
    {
        if (value)
        {
            SetMaskType(MaskType.Jump);
        }
        animator.SetBool("Jump", value);
    }

    IEnumerator ResetToBagRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (auraUnlocked)
            SetMaskType(MaskType.None);
        else
            SetMaskType(MaskType.Bag);
    }

    public void PlayFootstep()
    {
        if (!spriteRenderer.enabled)
            return;

        AudioManager.instance.PlayeOneShot2D(FMODEvents.instance.footstep);
        footstepParticle.Play();
    }

    public void PlayLanding()
    {
        if (!spriteRenderer.enabled)
            return;

        // need audio manager
        landingParticle.Play();
    }

    public void PlayBounce(Vector2 location, Quaternion normal)
    {
        if (!spriteRenderer.enabled)
            return;

        bounceParticle.transform.SetPositionAndRotation(location, normal);
        bounceParticle.Play();
    }
    public void PlayKill()
    {
        spriteRenderer.enabled = false;
    }

    public void PlayRespawn()
    {
        spriteRenderer.enabled = true;
    }

    public IEnumerator SwitchParticleWithTrail(float delay, MaskTrailItem item)
    {
        Debug.Log("COROUTINE STARTED!");

        yield return new WaitForSeconds(delay);

        Debug.Log("COROUTINE COMPLETE!");

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[1];

        maskSwitchParticle.GetParticles(particles);

        item.sprite.enabled = true;
        item.maskRB.transform.position = particles[0].position;
        item.maskRB.transform.rotation = Quaternion.Euler(0, 0, particles[0].rotation);
        item.maskRB.linearVelocity = particles[0].velocity;

        ParticleSystem.Particle p = particles[0];
        p.remainingLifetime = 0;
        particles[0] = p;

        maskSwitchParticle.SetParticles(particles);
    }
}
