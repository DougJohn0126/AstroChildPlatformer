using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D RB;
    public Animator PlayerAnimator;
    private bool mIsFacingRight = true;
    public ParticleSystem SmokeFX;
    public ParticleSystem SpeedFX;
    BoxCollider2D PlayerCollider2D;
    public Canvas mCanvas;


    [Header("Movement")]
    public float MoveSpeed = 5f;
    public float HorizontalMovement;
    private float mSpeedMultiplier = 1f;


    [Header("Jumping")]
    public float JumpPower =  10f;
    [SerializeField]  
    public int m_MaxJumps = 2;
    private int mJumpsRemaining;

    [Header("Dashing")]
    public float DashSpeed = 20f;
    public float DashDuration = 0.1f;
    public float DashCooldown = 0.1f;
    private bool mIsDashing;
    private bool mCanDash = true;
    private TrailRenderer mTrailRenderer;

    [Header("Ground Check")]
    public Transform GroundCheckPos;
    public Vector2 GroundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask GroundLayer;
    private bool mIsGrounded;
    private bool mIsOnPlatform;

    [Header("Gravity")]
    public float BaseGravity = 2f;
    [SerializeField]
    public float m_MaxFallSpeed = 18f;
    [SerializeField]
    public float m_FallSpeedMultiplier = 2f;

    [Header("Wall Check")]
    public Transform WallCheckPos;
    public Vector2 WallCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask WallLayer;

    [Header("Wall Movement")]
    public float WallSlideSpeed = 2;
    private bool mIsWallSliding;

    // Wall Jumping
    public bool IsWallJumpping;
    float WallJumpDirection;
    float WallJumpTime = 0.5f;
    float WallJumpTimer;
    public Vector2 WallJumpPower = new Vector2(5f, 10f);

    // Start is called before the first frame update
    void Start()
    {
        mTrailRenderer = GetComponent<TrailRenderer>();
        PlayerCollider2D = GetComponent<BoxCollider2D>();
        SpeedItem.OnSpeedCollected += StartSpeedBoost;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerAnimator.SetFloat("yVelocity", RB.velocity.y);
        PlayerAnimator.SetFloat("magnitude", RB.velocity.magnitude);
        PlayerAnimator.SetBool("isWallSliding", mIsWallSliding);

        if (mIsDashing)
        {
            return;
        }

        GroundCheck();
        ProcessGravity();
        ProcessWallSlide();
        ProcessWallJump();

        if (!IsWallJumpping)
        {
            RB.velocity = new Vector2(HorizontalMovement * MoveSpeed *mSpeedMultiplier, RB.velocity.y);
            Flip();
        }
        //Flip();
    }

    private void StartSpeedBoost (float multiplier)
    {
        StartCoroutine(SpeedBoostCoroutine(multiplier));
    }

    private IEnumerator SpeedBoostCoroutine(float multiplier)
    {
        mSpeedMultiplier = multiplier;
        SpeedFX.Play();
        yield return new WaitForSeconds(2f);
        mSpeedMultiplier = 1f;
        SpeedFX.Stop();
    }

    private void ProcessGravity()
    {
        if (RB.velocity.y < 0)
        {
            RB.gravityScale = BaseGravity * m_FallSpeedMultiplier;
            RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, - m_MaxFallSpeed));
        }
        else
        {
            RB.gravityScale = BaseGravity;
        }
    }

    private void ProcessWallSlide()
    {
        if (!mIsGrounded & WallCheck() & HorizontalMovement != 0)
        {
            mIsWallSliding = true;
            RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -WallSlideSpeed));
        }
        else
        {
            mIsWallSliding = false;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        HorizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && mCanDash)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {
        Physics2D.IgnoreLayerCollision(7, 8, true);
        mCanDash = false;
        mIsDashing = true;
        mTrailRenderer.emitting = true;
        float dashDirection = mIsFacingRight ? 1f : -1f;
        RB.velocity = new Vector2(dashDirection * DashSpeed, RB.velocity.y);
        yield return new WaitForSeconds(DashDuration);
        RB.velocity = new Vector2(0f, RB.velocity.y);
        mIsDashing = false;
        mTrailRenderer.emitting = false;
        Physics2D.IgnoreLayerCollision(7, 8, false);
        yield return new WaitForSeconds(DashCooldown);
        mCanDash = true;

    }

    public void Drop (InputAction.CallbackContext context)
    {
        if (context.performed && mIsGrounded && mIsOnPlatform && PlayerCollider2D.enabled)
        {
            StartCoroutine(DisablePlayerCollider(0.25f));
        }
    }

    private IEnumerator DisablePlayerCollider (float disableTime)
    {
        PlayerCollider2D.enabled = false;
        yield return new WaitForSeconds(disableTime);
        PlayerCollider2D.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Platform"))
        {
            mIsOnPlatform = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            mIsOnPlatform = false;
        }
    }

    public void Jump (InputAction.CallbackContext context)
    {
        if (mJumpsRemaining > 0)
        {
            if (context.performed)
            {
                RB.velocity = new Vector2(RB.velocity.x, JumpPower);
                mJumpsRemaining--;
                JumpEffects();
            }
            else if (context.canceled)
            {
                RB.velocity = new Vector2(RB.velocity.x, RB.velocity.y * 0.5f);
                mJumpsRemaining--;
                JumpEffects();
            }
        }

        if (context.performed && WallJumpTimer > 0f)
        {
            IsWallJumpping = true;
            RB.velocity = new Vector2(WallJumpDirection * WallJumpPower.x, WallJumpPower.y);
            WallJumpTimer = 0f;
            JumpEffects();
            if (transform.localScale.x != WallJumpDirection)
            {
                mIsFacingRight = !mIsFacingRight;
                Vector3 ls = transform.localScale;
                ls.x *= -1;
                transform.localScale = ls;
            }
            Invoke(nameof(CancelWallJump), WallJumpTime + 0.1f);
        }
    }

    private void JumpEffects()
    {
        Debug.Log("asdf");
        PlayerAnimator.SetTrigger("jump");
        SmokeFX.Play();
    }
        
    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(GroundCheckPos.position, GroundCheckSize, 0, GroundLayer))
        {
            mJumpsRemaining = m_MaxJumps;
            mIsGrounded = true;
        }else
        {
            mIsGrounded = false;
        }
    }

    private bool WallCheck()
    {
        return Physics2D.OverlapBox(WallCheckPos.position, WallCheckSize, 0, WallLayer);
    }

    private void Flip()
    {
        if (mIsFacingRight && HorizontalMovement < 0 || !mIsFacingRight && HorizontalMovement > 0)
        {
            mIsFacingRight = !mIsFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1;
            transform.localScale = ls;
            SpeedFX.transform.localScale = ls;
            float temp = (mIsFacingRight) ? mCanvas.transform.localScale.x : mCanvas.transform.localScale.x  * - 1;
            mCanvas.transform.localScale = new Vector3(temp, mCanvas.transform.localScale.y, mCanvas.transform.localScale.z);

            if (RB.velocity.y == 0)
            {
                SmokeFX.Play();
            }
        }
    }

    private void ProcessWallJump ()
    {
        if (mIsWallSliding)
        {
            IsWallJumpping = false;
            WallJumpDirection = -transform.localScale.x;
            WallJumpTimer = WallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }
        else if (WallJumpTimer > 0f)
        {
            WallJumpTimer -= Time.deltaTime;
        }
    }

    private void CancelWallJump()
    {
        IsWallJumpping = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(GroundCheckPos.position, GroundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(WallCheckPos.position, WallCheckSize);
    }
}
