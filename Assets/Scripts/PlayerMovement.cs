using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using Physics2D = Nomnom.RaycastVisualization.VisualPhysics2D;
#else
using Physics2D = UnityEngine.Physics2D;
#endif
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public float movement;
    public float LookUpDown;
    public float speed;
    public bool isGrounded;
    public LayerMask groundLayer;
    [SerializeField] Transform groundCheck;
    private bool jumpInput;
    [SerializeField] private float jumpForce = 5f;
    public bool IsFacingRight = true;

    [Header("Assist")]
    [SerializeField] private float jumpBufferLength = 0.2f;
    [SerializeField] private float jumpBufferTimer;
    bool jumpBuffer;


    [SerializeField] private float cayoteTimeLength = 0.1f;

    [Header("Gravity")]
    public float gravityScale = 1f;
    [SerializeField] private float fallMultiplier = 1.5f;


    [Header("Camera")]
    CameraFollowObject cameraFollowObject;
    GameObject CameraFollowObject;
    float _fallSpeedYDampingChangeThreshold;

    [Header("Dash")]
    [SerializeField] private bool canDash = true;
    public bool isDashing = false;
    [SerializeField] private float dashForce;
    [SerializeField] private float dashingTime;
    [SerializeField] private float dashCoolDown;

    [Header("Transition")]
    [SerializeField] float TransitionJumpForce;

    [Header("Input")]
    public InputAction moveAction;
    public InputAction jumpAction;
    public InputAction lookUpDownAction;
    public InputAction dashAction;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        lookUpDownAction = InputSystem.actions.FindAction("LookUpDown");
        dashAction = InputSystem.actions.FindAction("Dash");

        CameraFollowObject = GameObject.Find("CameraFollowObject");
        cameraFollowObject = CameraFollowObject.GetComponent<CameraFollowObject>();
        _fallSpeedYDampingChangeThreshold = CameraManager.instance._fallSpeedYDampingChangeThreshold;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDashing)
        {
            return;
        }
        isGrounded = Physics2D.OverlapCapsule(groundCheck.position, new Vector2(0.72f, 0.1f), CapsuleDirection2D.Horizontal, 0, groundLayer);
        if (!Player.Instance.BlockInput)
        {
            InputManager();
            if (jumpAction.WasReleasedThisFrame())
            {
                if (rb.linearVelocity.y > 0)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0f);
                }
            }
            rb.linearVelocity = new Vector2(movement * speed, rb.linearVelocity.y);
            if (Player.Instance.HasDash)
            {
                Dash();
            }
        }
        if (isGrounded == true)
        {
            cayoteTimeLength = 0.1f;
        }
        else
        {
            cayoteTimeLength -= Time.deltaTime;
        }
        if (rb.linearVelocity.y >= 0.5f)
        {
            cayoteTimeLength = 0f;
            if (!CameraManager.instance.IsLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
            {
                CameraManager.instance.LerpedFromPlayerFalling = false;
                CameraManager.instance.LerpYDamping(false);
            }
        }
        else if (rb.linearVelocity.y <= 0.5f)
        {
            rb.gravityScale = gravityScale * fallMultiplier;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -20f));
            if (rb.linearVelocityY < _fallSpeedYDampingChangeThreshold && !CameraManager.instance.IsLerpingYDamping && !CameraManager.instance.LerpedFromPlayerFalling)
            {
                CameraManager.instance.LerpYDamping(true);
            }
        }
        else
        {
            rb.gravityScale = gravityScale;
        }
        // Process jump in the FixedUpdate method
        if (jumpBuffer == true)
        {
            jumpBufferTimer -= Time.deltaTime;
            if (jumpBufferTimer > 0 && (cayoteTimeLength > 0 || (isGrounded && jumpInput)))
            {
                jumpBuffer = false;
                rb.gravityScale = gravityScale;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                if (!jumpAction.IsPressed())
                {
                    if (rb.linearVelocity.y > 0)
                    {
                        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0f);
                    }
                }

                // Reset the jump buffer and cayote time states
                jumpInput = false;

            }
            else if (jumpBufferTimer <= 0)
            {
                jumpBuffer = false;
            }
        }

        if (jumpBuffer == false)
        {
            jumpBufferTimer = 0;
        }
    }
    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
        if (movement != 0)
        {
            TurnCheck();
        }
    }

    void Dash()
    {
        if (!canDash)
        {
            dashCoolDown -= Time.deltaTime;
        }
        if (dashCoolDown <= 0 && isGrounded)
        {
            canDash = true;
        }

        if (dashAction.WasPressedThisFrame() && canDash)
        {
            isDashing = true;
            StartCoroutine(Dashing());
        }
    }

    IEnumerator Dashing()
    {
        canDash = false;
        dashCoolDown = 0.3f;
        gravityScale = 0f;
        if (IsFacingRight)
        {
            rb.linearVelocity = new Vector2(1 * dashForce, 0f);
        }
        else
        {
            rb.linearVelocity = new Vector2(-1 * dashForce, 0f);
        }
        yield return new WaitForSeconds(dashingTime);
        gravityScale = 1f;
        isDashing = false;
    }

    void TurnCheck()
    {
        if (movement > 0 && !IsFacingRight)
        {
            Flip();
        }
        else if (movement < 0 && IsFacingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        if (IsFacingRight)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            IsFacingRight = !IsFacingRight;
            cameraFollowObject.CallTurn();
        }
        else
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            IsFacingRight = !IsFacingRight;
            cameraFollowObject.CallTurn();
        }
    }
    void InputManager()
    {
        movement = moveAction.ReadValue<float>();
        LookUpDown = lookUpDownAction.ReadValue<float>();
        if (jumpAction.WasPressedThisFrame())
        {
            jumpBuffer = true;
            jumpBufferTimer = jumpBufferLength;
            jumpInput = true;
        }

    }
    public IEnumerator TransitionMove()
    {
        if (IsFacingRight)
        {
            rb.linearVelocity = new Vector2(1 * speed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(-1 * speed, rb.linearVelocity.y);
        }
        yield return new WaitUntil(() => GameManager.Instance.LoadedLevel == true);
        GameManager.Instance.FinishTransition(0.5f);
        yield return new WaitUntil(() => Player.Instance.BlockInput == false);
        GameManager.Instance.LoadedLevel = false;

    }

    public IEnumerator TransitionJump()
    {
        rb.linearVelocityY = 20f;
        yield return new WaitUntil(() => GameManager.Instance.LoadedLevel == true);
        GameManager.Instance.FinishTransition(0.75f);
        gravityScale = 1f;
        if (IsFacingRight)
        {
            rb.linearVelocity = new Vector2(1 * speed, TransitionJumpForce);
        }
        else
        {
            rb.linearVelocity = new Vector2(-1 * speed, TransitionJumpForce);
        }
        yield return new WaitUntil(() => Player.Instance.BlockInput == false);
        GameManager.Instance.LoadedLevel = false;
    }
    public IEnumerator TransitionFall()
    {
        rb.linearVelocityX = 0;
        yield return new WaitUntil(() => GameManager.Instance.LoadedLevel == true);
        rb.linearVelocityY = -20f;
        GameManager.Instance.FinishTransition(0.3f);
        yield return new WaitUntil(() => Player.Instance.BlockInput == false);
        GameManager.Instance.LoadedLevel = false;
    }

}