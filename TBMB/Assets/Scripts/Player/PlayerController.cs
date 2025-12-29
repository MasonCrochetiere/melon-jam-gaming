using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public enum MoveCoroutineType { PlayerForces, SpeedClamp, Gravity }
public class PlayerController : MonoBehaviour
{
    [SerializeField] bool forceAllUnlocks = false;

    Rigidbody2D rb;
    InventoryManager inventory;
    [SerializeField] PlayerAnimationManager playerAnimationManager;
    [SerializeField] GameObject rotationBase;
    public Vector2 moveInput { get; private set; }
    Vector2 lastPositiveMoveInput;
    public bool jumpDownInput;

    [Header("Move Variables")]
    [SerializeField] float moveAcceleration = 100f;
    [SerializeField] float maxSpeed = 2f;

    [SerializeField] float jumpVelocity = 7f;

    [SerializeField] float groundFriction = 4f;
    [SerializeField] float airFriction = 2f;

    [SerializeField] float airControlMultiplier = 0.4f;

    [SerializeField] PhysicsMaterial2D basePhysicsMaterial;

    [SerializeField] float coyoteTimeDuration = 0.2f;
    float leaveGroundTime;
    bool coyoteActive;

    [SerializeField] float jumpBufferDuration = 0.2f;
    float jumpPressedTime;

    [SerializeField] float maximumFallSpeed = 25f;

    [Header("Dash Variables")]
    [SerializeField] float dashForce = 30f;
    [Tooltip("Duration to disable gravity and max speed")]
    [SerializeField] float dashDuration = 0.4f;
    [Tooltip("We re-enable player forces shortly before the end of the dash to smooth the player out of it. " +
        "This should be shorter than Dash Duration")]
    [SerializeField] float dashMoveDisableDuration = 0.35f;

    [Header("Ball Variables")]
    [SerializeField] PhysicsMaterial2D ballPhysicsMaterial;
    [SerializeField] float minimumBallDuration = 0.6f;
    [SerializeField] float rotationSpeed = 0.1f;

    [Header("Aura Variables")]
    [SerializeField] float auraAcceleration;
    [SerializeField] float auraMaxSpeed;

    [Header("Hazard Variables")]
    [SerializeField] float respawnDelay = 0.8f;
    [Tooltip("The player's speed is multiplied by this number")]
    [SerializeField] float glueMultiplier = 0.5f;

    [Header("Game Variables [DO NOT CHANGE]")]
    DashPoint dashPoint;
    bool dashAvailable;
 
    public bool moveActivated = true;

    public bool onGround = false;
    bool lastGrounded;

    private Coroutine clampCoroutine;
    public bool clampEnabled = true;

    private Coroutine playerForcesCoroutine;
    private Coroutine gravityCoroutine;

    private Vector2 respawnPoint;

    bool jumpUnlocked;
    bool ballUnlocked;
    bool dashUnlocked;

    bool playerDead = false;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        inventory = GetComponent<InventoryManager>();

        respawnPoint = gameObject.transform.position;
    }

    public void initializeInput(InputSystem_Actions.PlayerActions actions)
    {
        actions.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>(); // moveInput =
        actions.Move.performed += ctx => lastPositiveMoveInput = ctx.ReadValue<Vector2>();
        actions.Move.canceled += ctx => moveInput = Vector2.zero;

        actions.Jump.performed += ctx => JumpStart();
        actions.Jump.canceled += ctx => JumpEnd();

        actions.Dash.started += ctx => Dash();

        actions.Ball.started += ctx => StartBall();
        actions.Ball.canceled += ctx => EndBall();

        actions.Reset.started += ctx => RespawnPlayer();
    }

    private void FixedUpdate()
    {
        GroundCheck();

        if (moveActivated)
        {
            Move();
            CheckJump();
        }

        if (rb.sharedMaterial == ballPhysicsMaterial)
        {
            SetRotation(Time.fixedDeltaTime);
        }

        UpdateDashPoint();
    }

    void GroundCheck()
    {
        lastGrounded = onGround;
        onGround = Physics2D.CircleCast(gameObject.transform.position, 0.5f, Vector2.down, 0.05f);
        playerAnimationManager.setOnGround(onGround);

        if (onGround && !lastGrounded)
        {
            playerAnimationManager.StartBagSwitchDelay();
            playerAnimationManager.PlayLanding();
            AudioManager.instance.PlayeOneShot2D(FMODEvents.instance.playerLand);
        }

        if (!onGround && lastGrounded)
        {
            leaveGroundTime = Time.time;

            coyoteActive = true;
        }

        if (Time.time - leaveGroundTime > coyoteTimeDuration)
        {
            coyoteActive = false;
        }

        if (onGround)
        {
            dashAvailable = true;
        }
    }

    void JumpStart()
    {
        jumpDownInput = true;

        jumpPressedTime = Time.time;
    }

    void JumpEnd()
    {
        jumpDownInput = false;

        if (rb.linearVelocityY > 0)
        {
            leaveGroundTime -= coyoteTimeDuration;
        }
    }

    void CheckJump()
    {
        if ((jumpDownInput || Time.time - jumpPressedTime < jumpBufferDuration)
            && (onGround || coyoteActive)  
            && (jumpUnlocked || forceAllUnlocks))
        {
            rb.linearVelocityY = jumpVelocity;

            playerAnimationManager.UpdateJump(true);

            coyoteActive = false;
            jumpPressedTime += jumpBufferDuration;

            AudioManager.instance.PlayeOneShot2D(FMODEvents.instance.jump);
        }

        if (rb.linearVelocityY < 0)
        {
            playerAnimationManager.UpdateJump(false);
        }    
    }

    void Move()
    {
        float speedClamp = maxSpeed;
        Vector2 moveInputValue = moveInput;
        moveInputValue.y = 0;

        // -------------------- SMOOTH INVERSION ----------------------- \\
        // These if statements are making redirecting movement go faster
        // so if you are moving left, then swap to moving right, you'll switch velocities super fast
        // should be snappy but not jarring
        if ((moveInputValue.x > 0 && rb.linearVelocityX < 0) || (moveInputValue.x < 0 && rb.linearVelocityX > 0))
        {
            rb.linearVelocity.Set(moveInputValue.x * moveAcceleration, rb.linearVelocityY);
        }
        // -------------------- PLAYER MOVEMENT ----------------------- \\

        // doing the actual movement
        rb.AddForce(moveInputValue * moveAcceleration * (onGround ? 1 : airControlMultiplier));

        Vector2 frictionForce = rb.linearVelocity;
        frictionForce.y = 0;
        frictionForce *= -1 * (onGround ? groundFriction : airFriction);
        rb.AddForce(frictionForce);

        // -------------------- CLAMP SPEED ----------------------- \\

        if (clampEnabled)
        {
            // clamp to a max speed to keep the acceleration on the move without a big mess
            rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocity.x, -speedClamp, speedClamp), rb.linearVelocityY);
        }

        if (rb.linearVelocityY < 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, Mathf.Clamp(rb.linearVelocityY, -1 * maximumFallSpeed, 0));
        }
        

        playerAnimationManager.setMoveValueX(moveInputValue.x);
    }

    public void SetMoveActivated(bool value)
    {
        moveActivated = value;
        if (!value)
        {
            rb.linearVelocity = Vector3.zero;
        }
    }

    public void SetDashPoint(DashPoint target)
    {
        if (dashPoint != null)
            dashPoint.ShowDashAngle(false);

        dashPoint = target;
        dashPoint.ShowDashAngle(true);
        // call some event in here
        // not sure if hard override method is best but it's okay for now

        dashAvailable = true;
    }

    public void RemoveDashPoint(DashPoint target)
    {
        if (dashPoint == target)
        {
            dashPoint.ShowDashAngle(false);
            dashPoint = null;
            // call some event in here
        }
    }

    void UpdateDashPoint()
    {
        if (dashPoint != null)
        {
            dashAngle = GetDashAngleFromPoint(dashPoint.transform.position);
            dashPoint.UpdateDashAngle(dashAngle);
        }
    }

    public float dashAngle = 0f;
    void Dash()
    {
        if (!dashAvailable || rb.sharedMaterial == ballPhysicsMaterial)
            return;

        if (!(dashUnlocked || forceAllUnlocks))
            return;

        if (!playerAnimationManager.getSpriteRendererEnabled())
            return;

        if (dashPoint != null)
        {
            dashPoint.LockAngle();

            // Convert degrees to radians and create unit vector
            float radians = dashAngle * Mathf.Deg2Rad;

            rb.linearVelocity = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * dashForce;
        }
        else
        {
            rb.linearVelocity = new Vector2(lastPositiveMoveInput.x * dashForce, 0);

            dashAngle = 90f;

            dashAvailable = false;
        }      

        RunCoroutine(MoveCoroutineType.SpeedClamp, dashDuration);
        RunCoroutine(MoveCoroutineType.Gravity, dashDuration);
        RunCoroutine(MoveCoroutineType.PlayerForces, dashMoveDisableDuration);

        playerAnimationManager.playDashOverride(dashAngle);
        AudioManager.instance.PlayeOneShot2D(FMODEvents.instance.dash);
    }

    void StartBall()
    {
        if (!(ballUnlocked || forceAllUnlocks))
            return;

        if (!playerAnimationManager.getSpriteRendererEnabled())
            return;

        rb.sharedMaterial = ballPhysicsMaterial;
        moveActivated = false;

        playerAnimationManager.playBallOverride();
    }

    void EndBall()
    {
        if (!playerAnimationManager.getSpriteRendererEnabled())
            return;

        rb.sharedMaterial = basePhysicsMaterial;
        moveActivated = true;

        playerAnimationManager.setAnimationOverride(false);
    }

    public void RunCoroutine(MoveCoroutineType type, float reenableInSeconds)
    {
        switch (type)
        {
            case MoveCoroutineType.PlayerForces:
                if (playerForcesCoroutine != null)
                    StopCoroutine(playerForcesCoroutine);
                playerForcesCoroutine = StartCoroutine(DisableMovement(reenableInSeconds));
                break;
            case MoveCoroutineType.SpeedClamp:
                if (clampCoroutine != null)
                    StopCoroutine(clampCoroutine);
                clampCoroutine = StartCoroutine(DisableClamp(reenableInSeconds));
                break;
            case MoveCoroutineType.Gravity:
                if (gravityCoroutine != null)
                    StopCoroutine(gravityCoroutine);
                gravityCoroutine = StartCoroutine(DisableGravity(reenableInSeconds));
                break;
        }
    }

    IEnumerator DisableClamp(float reenableInSeconds)
    {
        clampEnabled = false;

        yield return new WaitForSeconds(reenableInSeconds);

        clampEnabled = true;
    }

    IEnumerator DisableGravity(float reenableInSeconds)
    {
        rb.gravityScale = 0;

        yield return new WaitForSeconds(reenableInSeconds);

        rb.gravityScale = 1;
    }

    IEnumerator DisableMovement(float reenableInSeconds)
    {
        moveActivated = false;

        yield return new WaitForSeconds(reenableInSeconds);

        moveActivated = true;
    }

    float GetDashAngleFromPoint(Vector2 point)
    {
        Vector2 sourcePos = transform.position;
        Vector2 targetPos = point;

        float angle;

        if (targetPos.x > sourcePos.x && targetPos.y > sourcePos.y)
        {
            // Target is upper-right -> 30°
            angle = 30f;
        }
        else if (targetPos.x < sourcePos.x && targetPos.y > sourcePos.y)
        {
            // Target is upper-left -> 150°
            angle = 150f;
        }
        else if (targetPos.x < sourcePos.x && targetPos.y < sourcePos.y)
        {
            // Target is lower-left -> 210°
            angle = 210f;
        }
        else
        {
            // Target is lower-right -> 330°
            angle = 330f;
        }

        return angle;
    }

    void SetRotation(float time)
    {
        Vector2 movement = rb.linearVelocity * time;
        float distance = movement.magnitude * rotationSpeed;
        float angle = distance * (180 / Mathf.PI) / 0.5f;

        rotationBase.transform.localRotation = Quaternion.Euler(Vector3.forward * (angle * GetRawXVelocity() * -1)) * rotationBase.transform.localRotation;
    }

    float GetRawXVelocity()
    {
        if (rb.linearVelocityX > 0)
        {
            return 1;
        }
        else if (rb.linearVelocityY < 0)
        {
            return -1;
        }
        else
        {
            return rb.linearVelocityX;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (rb.sharedMaterial == ballPhysicsMaterial)
        {
            playerAnimationManager.PlayBounce(collision.contacts[0].point, Quaternion.Euler(collision.contacts[0].normal));
            AudioManager.instance.PlayeOneShot2D(FMODEvents.instance.playerBounce);

            dashAvailable = true;
        }
    }

    public void RespawnPlayer()
    {
        transform.position = respawnPoint;
        rb.linearVelocity = Vector3.zero;

        playerAnimationManager.PlayRespawn();
        EndBall();
        moveActivated = true;
        AudioManager.instance.PlayeOneShot2D(FMODEvents.instance.playerRespawn);

        playerDead = false;

        Debug.Log("RESPAWN");
    }

    public void SetRespawnPoint(Vector2 point)
    {
        respawnPoint = point;
    }

    public void KillPlayer()
    {
        if (playerDead)
            return;

        Debug.Log("KILL");

        playerDead = true;

        rb.linearVelocity = Vector3.zero;

        RunCoroutine(MoveCoroutineType.SpeedClamp, respawnDelay);
        RunCoroutine(MoveCoroutineType.Gravity, respawnDelay);
        RunCoroutine(MoveCoroutineType.PlayerForces, respawnDelay);

        StartCoroutine(Respawn(respawnDelay));

        playerAnimationManager.PlayKill();
        AudioManager.instance.PlayeOneShot2D(FMODEvents.instance.playerDeath);

        // death sound probably goes in here rather than anywhere else
        // make sure the sound can only play one at a time since otherwise it'll spam unfortunately
    }

    public void GluePlayer()
    {
        rb.linearVelocity *= glueMultiplier;

        // glue sound probably goes in here
        // make sure the sound can only play one at a time since otherwise it'll spam unfortunately
    }


    IEnumerator Respawn(float delay)
    {
        yield return new WaitForSeconds(delay);

        playerAnimationManager.lastRotation = 180f;
        RespawnPlayer();
    }

    public void CutsceneKill()
    {
        rb.linearVelocity = Vector3.zero;
        transform.position = respawnPoint;

        RunCoroutine(MoveCoroutineType.SpeedClamp, 7);
        RunCoroutine(MoveCoroutineType.Gravity, 7);
        RunCoroutine(MoveCoroutineType.PlayerForces, 7);

        playerAnimationManager.lastRotation = 180f;
        playerAnimationManager.PlayKill();
        playerAnimationManager.StartBagSwitchDelay(2);
    }

    public void CutsceneSetRotation()
    {
        playerAnimationManager.setMoveValueX(-1);
        playerAnimationManager.lastRotation = 180f;
    }

    public void ItemGet(ItemList item)
    {
        switch (item)
        {
            case ItemList.Jump:
                jumpUnlocked = true;
                break;
            case ItemList.Dash:
                dashUnlocked = true;
                break;
            case ItemList.Ball:
                ballUnlocked = true;
                break;
            case ItemList.Aura:
                maxSpeed = auraMaxSpeed;
                moveAcceleration = auraAcceleration;
                break;
        }
    }
}