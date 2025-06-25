using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HoldToLoadLevel : MonoBehaviour
{
    public float HoldDuration = 1;
    public Image FillCircle;

    private float HoldTimer = 0f;
    private bool IsHolding = false;

    public static event Action OnHoldComplete;
 

    // Update is called once per frame
    void Update()
    {
        if (IsHolding)
        {
            HoldTimer += Time.deltaTime;
            FillCircle.fillAmount = HoldTimer / HoldDuration;
            if (HoldTimer >= HoldDuration)
            {
                OnHoldComplete.Invoke();
                ResetHold();
            }
        }  
    }
    public void OnHold (InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsHolding = true;
        } else if (context.canceled)
        {
            ResetHold();
        }
    }

    private void ResetHold()
    {
        IsHolding = false;
        HoldTimer = 0;
        FillCircle.fillAmount = 0f;
    }
}
