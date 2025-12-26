using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Move Variables")]
    [SerializeField] float moveAcceleration = 100f;
    [SerializeField] float maxSpeed = 2f;

    [SerializeField] float jumpVelocity = 7f;

    [SerializeField] float groundFriction = 4f;
    [SerializeField] float airFriction = 2f;

    [SerializeField] float airControlMultiplier = 0.4f;

    float baseDrag;
    public bool moveActivated = true;

    Rigidbody2D rb;

    public Vector2 moveInput { get; private set; }
    public bool jumpDown;

    public bool onGround = false;

    InventoryManager inventory;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        baseDrag = rb.linearDamping;

        inventory = GetComponent<InventoryManager>();
    }

    public void initializeInput(InputSystem_Actions.PlayerActions actions)
    {
        actions.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>(); // moveInput = 
        actions.Move.canceled += ctx => moveInput = Vector2.zero;

        actions.Jump.performed += ctx => jumpDown = true;
        actions.Jump.canceled += ctx => jumpDown = false;
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

        // clamp to a max speed to keep the acceleration on the move without a big mess
        rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocity.x, -speedClamp, speedClamp), rb.linearVelocityY);
    }

    public void SetMoveActivated(bool value)
    {
        moveActivated = value;
        if (!value)
        {
            rb.linearVelocity = Vector3.zero;
        }
    }
}