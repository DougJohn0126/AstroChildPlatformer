using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretArea : MonoBehaviour
{
    public float FadeDuration = 1.0f;
    private SpriteRenderer mSpriteRenderer;
    private Color mHiddenColor;
    private Coroutine mCurrentCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        mSpriteRenderer = GetComponent<SpriteRenderer>();
        mHiddenColor = mSpriteRenderer.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (mCurrentCoroutine != null)
            {
                StopCoroutine(mCurrentCoroutine);
            }
            mCurrentCoroutine = StartCoroutine(FadeSprite(true));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if (mCurrentCoroutine != null)
            {
                StopCoroutine(mCurrentCoroutine);
            }
            mCurrentCoroutine = StartCoroutine(FadeSprite(false));
        }
    }

    private IEnumerator FadeSprite(bool fadeOut)
    {
        Color startColor = mSpriteRenderer.color;
        Color targetColor = fadeOut ? new Color(mHiddenColor.r, mHiddenColor.g, mHiddenColor.b, 0f) : mHiddenColor;
        float timeFading = 0f;
        while (timeFading < FadeDuration)
        {
            mSpriteRenderer.color = Color.Lerp(startColor, targetColor, timeFading / FadeDuration);
            timeFading += Time.deltaTime;
            yield return null;
        }
        mSpriteRenderer.color = targetColor;
    }
}
