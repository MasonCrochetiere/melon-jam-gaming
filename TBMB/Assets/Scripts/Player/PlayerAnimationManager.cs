using System.Collections;
using UnityEngine;
using UnityEngine.U2D.Animation;

public enum MaskType { Bag, Jump, Dash, Ball, None }

public class PlayerAnimationManager : MonoBehaviour
{
    [SerializeField] float DashAnimationDuration = 0.25f;
    [SerializeField] float delayBeforeBag = 4f;

    [SerializeField] SpriteLibraryAsset bagLibrary;
    [SerializeField] SpriteLibraryAsset jumpLibrary;
    [SerializeField] SpriteLibraryAsset dashLibrary;
    [SerializeField] SpriteLibraryAsset ballLibrary;
    [SerializeField] SpriteLibraryAsset noneLibrary;
    MaskType currentType;
    Coroutine maskSwitchCoroutine;

    Animator animator;
    SpriteLibrary spriteLibrary;
    SpriteResolver resolver;

    bool overrideIdle = false;
    bool onGround = true;

    float moveValueX;

    bool dashOverride = false;
    bool ballOverride = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteLibrary = GetComponent<SpriteLibrary>();
        resolver = GetComponent<SpriteResolver>();
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
                gameObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
            else if (moveValueX > 0)
            {
                gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }
    }

    public void SetMaskType(MaskType maskType)
    {
        if (currentType == MaskType.None)
            return;

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
    }

    public void StartBagSwitchDelay()
    {
        if (maskSwitchCoroutine != null)
        {
            StopCoroutine(maskSwitchCoroutine);
        }
        maskSwitchCoroutine = StartCoroutine(ResetToBagRoutine(delayBeforeBag));
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

        SetMaskType(MaskType.Bag);
    }
}
