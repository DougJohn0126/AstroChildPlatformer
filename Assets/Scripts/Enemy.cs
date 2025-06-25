using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    private Transform Player;
    public float ChaseSSpeed = 3f;
    public float JumpForce = 2f;
    public LayerMask GroundLayer;

    private Rigidbody2D mRB;
    private bool IsGrounded;
    private bool ShouldJump;

    public int Damage = 1;

    public int MaxHealth = 3;
    private int mCurrentHealth;
    private SpriteRenderer mSpriteRender;
    private Color ogColor;

    private bool mIsFacingRight = true;
    public float HorizontalMovement;

    [Header("Loot")]
    public List<LootItem> LootTable = new List<LootItem>();

    // Start is called before the first frame update
    void Start()
    {
        mRB = GetComponent<Rigidbody2D>();
        Player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        mSpriteRender = GetComponent<SpriteRenderer>();
        mCurrentHealth = MaxHealth;
        ogColor = mSpriteRender.color;
    }

    // Update is called once per frame
    void Update()
    {
        IsGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1f, GroundLayer);

        float direction = Mathf.Sign(Player.position.x - transform.position.x);

        bool isPlayerAbove = Physics2D.Raycast(transform.position, Vector2.up, 3f, 1 << Player.gameObject.layer);

        if (IsGrounded)
        {
            mRB.velocity = new Vector2(direction * ChaseSSpeed, mRB.velocity.y);
            HorizontalMovement = direction;
            Flip();

            RaycastHit2D groundInFront = Physics2D.Raycast(transform.position, new Vector2(direction, 0), 2f, GroundLayer);

            RaycastHit2D gapAhead = Physics2D.Raycast(transform.position + new Vector3(direction, 0, 0), Vector2.down, 2f, GroundLayer);

            RaycastHit2D platformAbove = Physics2D.Raycast(transform.position, Vector2.up, 3f, GroundLayer);

            if (!groundInFront.collider && !gapAhead.collider)
            {
                ShouldJump = true;
            }
            else if (isPlayerAbove && platformAbove.collider)
            {
                ShouldJump = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (IsGrounded && ShouldJump)
        {
            ShouldJump = false;
            Vector2 direction = (Player.position - transform.position).normalized;

            Vector2 jumpDirection = direction * JumpForce;

            mRB.AddForce(new Vector2(jumpDirection.x, JumpForce), ForceMode2D.Impulse);
            
        }
    }

    public void TakeDamage(int damage)
    {
        mCurrentHealth -= damage;
        StartCoroutine(FlashWhite());
        if (mCurrentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashWhite()
    {
        mSpriteRender.color = Color.white;
        yield return new WaitForSeconds(0.2f);
        mSpriteRender.color = ogColor;

    }

    private void Flip()
    {
        if (mIsFacingRight && HorizontalMovement < 0 || !mIsFacingRight && HorizontalMovement > 0)
        {
            mIsFacingRight = !mIsFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1;
            transform.localScale = ls;

           
        }
    }
    private void Die()
    {
        foreach (LootItem lootItem in LootTable)
        {
            if(Random.Range(0f, 100f) <= lootItem.DropChance)
            {
                InstantiateLoot(lootItem.ItemPrefab);
                break;
            }
        }
        Destroy(gameObject);
    }

    private void InstantiateLoot(GameObject loot)
    {
        if (loot)
        {
            GameObject droppedLoot = Instantiate(loot, transform.position, Quaternion.identity);
            droppedLoot.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
}
