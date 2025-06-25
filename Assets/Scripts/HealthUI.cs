using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public Image HeartPrefab;
    public Sprite FullHeartSprite;
    public Sprite EmptyHeartSprite;

    private List<Image> mHearts = new List<Image>();

    public void SetMaxHearts (int maxHearts)
    {
        foreach (Image heart in mHearts)
        {
            Destroy(heart.gameObject);
        }
        mHearts.Clear();

        for (int i = 0; i < maxHearts; i++)
        {
            Image newHeart = Instantiate(HeartPrefab, transform);
            newHeart.sprite = FullHeartSprite;
            newHeart.color = Color.red;
            mHearts.Add(newHeart);
        }
    }

    public void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < mHearts.Count; i++)
        {
            if (i < currentHealth)
            {
                mHearts[i].sprite = FullHeartSprite;
                mHearts[i].color = Color.red;
            }
            else
            {
                mHearts[i].sprite = EmptyHeartSprite;
                mHearts[i].color = Color.white;
            }
        }
    }
}
