﻿using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class StaminaUseDisabler : MonoBehaviour
{
    [Inject] readonly PlayerStamina m_playerStamina;

    float m_startSpendingValue;
    IEnumerator m_disableCoroutine;

    public Action<float> OnUseDisabled { get; set; }

    void Start()
    {
        m_startSpendingValue = m_playerStamina.SpendingSpeed;
    }

    public void Disable(float effectTime)
    {
        m_disableCoroutine = DisableCoroutine(effectTime);
        StartCoroutine(m_disableCoroutine);
    }

    public void StopDisabling()
    {
        print("dsa");
        StopCoroutine(m_disableCoroutine);
    }

    public IEnumerator DisableCoroutine(float effectTime)
    {
        OnUseDisabled.Invoke(effectTime);
        m_playerStamina.SpendingSpeed = 0;
        m_playerStamina.StaminaValue = m_playerStamina.MaxStaminaAmount;

        yield return new WaitForSeconds(effectTime);

        m_playerStamina.SpendingSpeed = m_startSpendingValue;
    }
}
