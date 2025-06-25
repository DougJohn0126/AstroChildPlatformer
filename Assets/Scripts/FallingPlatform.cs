using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public float FallWait = 2f;
    public float DestroyWait = 1.0f;
    private bool mIsFalling;
    private Rigidbody2D mRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        mRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!mIsFalling && collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Fall());
        }
    }

    private IEnumerator Fall()
    {
        mIsFalling = true;
        yield return new WaitForSeconds(FallWait);
        mRigidbody.bodyType = RigidbodyType2D.Dynamic;
        Destroy(gameObject, DestroyWait);
    }
}
