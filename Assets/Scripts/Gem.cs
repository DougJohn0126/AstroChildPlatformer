using System;
using UnityEngine;

public class Gem : MonoBehaviour, iItem
{

    public static event Action<int> OnGemCollect;
    public int Worth = 5;

    public void Collect()
    {
        OnGemCollect.Invoke(Worth);
        Destroy(gameObject);
    }
}
