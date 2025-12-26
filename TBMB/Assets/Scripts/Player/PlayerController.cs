using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public enum MoveCoroutineType { PlayerForces, SpeedClamp, Gravity }
public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    InventoryManager inventory;

    public Vector2 moveInput { get; private set; }
    Vector2 lastPositiveMoveInput;
    public bool jumpDown;

    [Header("Move Variables")]
    [SerializeField] float moveAcceleration = 100f;
    [SerializeField] float maxSpeed = 2f;

    [SerializeField] float jumpVelocity = 7f;

    [SerializeField] float groundFriction = 4f;
    [SerializeField] float airFriction = 2f;

    [SerializeField] float airControlMultiplier = 0.4f;

    [Header("Dash Variables")]
    [SerializeField] float dashForce = 30f;
    [Tooltip("Duration to disable gravity and max speed")]
    [SerializeField] float dashDuration = 0.4f;
    [Tooltip("We re-enable player forces shortly before the end of the dash to smooth the player out of it. " +
        "This should be shorter than Dash Duration")]
    [SerializeField] float dashMoveDisableDuration = 0.35f;

    DashPoint dashPoint;
 
    public bool moveActivated = true;

    public bool onGround = false;

    private Coroutine clampCoroutine;
    public bool clampEnabled = true;

    private Coroutine playerForcesCoroutine;
    private Coroutine gravityCoroutine;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        inventory = GetComponent<InventoryManager>();
    }

    public void initializeInput(InputSystem_Actions.PlayerActions actions)
    {
        actions.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>(); // moveInput =
        actions.Move.performed += ctx => lastPositiveMoveInput = ctx.ReadValue<Vector2>();
        actions.Move.canceled += ctx => moveInput = Vector2.zero;

        actions.Jump.performed += ctx => jumpDown = true;
        actions.Jump.canceled += ctx => jumpDown = false;

        actions.Sprint.started += ctx => Dash();
    }

    private void FixedUpdate()
    {
        onGround = Physics2D.CircleCast(gameObject.transform.position, 0.5f, Vector2.down, 0.05f);

        if (moveActivated)
        {
            Move();
            CheckJump();
        }
    }

    void CheckJump()
    {
        if (jumpDown && onGround && inventory.CheckItem(ItemList.Jump))
        {
            rb.linearVelocityY = jumpVelocity;

            jumpDown = false;
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
        dashPoint = target;
        // call some event in here
        // not sure if hard override method is best but it's okay for now
    }

    public void RemoveDashPoint(DashPoint target)
    {
        if (dashPoint == target)
        {
            dashPoint = null;
            // call some event in here
        }
    }

    void Dash()
    {
        if (dashPoint != null)
        {
            rb.linearVelocity = GetDashAngleFromPoint(dashPoint.transform.position) * dashForce;
        }
        else
        {
            rb.linearVelocity = new Vector2(lastPositiveMoveInput.x * dashForce, 0);
        }      

        RunCoroutine(MoveCoroutineType.SpeedClamp, dashDuration);
        RunCoroutine(MoveCoroutineType.Gravity, dashDuration);
        RunCoroutine(MoveCoroutineType.PlayerForces, dashMoveDisableDuration);
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

    Vector2 GetDashAngleFromPoint(Vector2 point)
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

        // Convert degrees to radians and create unit vector
        float radians = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
    }
}