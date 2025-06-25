using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : MonoBehaviour, iItem
{
    [SerializeField]
    public int m_HealAmount = 1;
    public static event Action<int> OnHealthCollect;
    public void Collect()
    {
        OnHealthCollect.Invoke(m_HealAmount);
        Destroy(gameObject);
    }

}
