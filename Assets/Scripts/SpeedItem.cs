using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedItem : MonoBehaviour, iItem
{
    public static event Action<float> OnSpeedCollected;
    public float SpeedMultiplier = 1.5f;
    public void Collect()
    {
        OnSpeedCollected.Invoke(SpeedMultiplier);
        Destroy(gameObject);
    }

}
