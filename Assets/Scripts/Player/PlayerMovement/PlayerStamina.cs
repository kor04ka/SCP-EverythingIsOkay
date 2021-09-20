using System;
using System.Collections;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(StaminaUseDisabler))]
public class PlayerStamina : MonoBehaviour
{
    [SerializeField] float m_regenerationSpeed;
    [SerializeField] float m_spendingSpeed;

    [SerializeField] float m_delayDuringRegeneration;
    [SerializeField] float m_delayBeforeRegenerationStart;

    [SerializeField] float m_staminaValue;

    [Inject] readonly MovementSpeed m_playerSpeed;
    [Inject] readonly PlayerMovement m_playerMovement;
    [Inject] readonly PlayerHealth m_playerHealth;

    WaitForSeconds m_timeoutBeforeRegeneration;
    WaitForSeconds m_timeoutDuringRegeneration;
    IEnumerator m_regenerationCoroutine;

    bool hadPlayerStamina;
    public float StaminaValue
    {
        get => m_staminaValue;
        set
        {
            m_staminaValue = value;
            OnStaminaValueChanged?.Invoke();
        }
    }
    public Action OnStaminaValueChanged { get; set; }

    public bool HasPlayerStamina
    {
        get 
        {
            bool hasPlayerStamina = StaminaValue > 0;
            if (!hasPlayerStamina && hadPlayerStamina)
            {
                OnStaminaRanOut?.Invoke();
            }
            hadPlayerStamina = hasPlayerStamina;
            return hasPlayerStamina;
        }
    }

    public float MaxStaminaAmount { get; set; }
    public Action OnStaminaRanOut { get; set; }
    public float SpendingSpeed { get => m_spendingSpeed; set => m_spendingSpeed = value; }

    void Awake()
    {
        m_regenerationCoroutine = RegenerateCoroutine();
        m_timeoutBeforeRegeneration = new WaitForSeconds(m_delayBeforeRegenerationStart);
        m_timeoutDuringRegeneration = new WaitForSeconds(m_delayDuringRegeneration); 
        MaxStaminaAmount = m_staminaValue;
    }

    void Start()
    {
        m_playerSpeed.OnPlayerRun += Burn;
        m_playerSpeed.OnPlayerStoppedRun += Regenerate;
        m_playerMovement.OnPlayerStoppedMoving += RegenerateAfterPlayerStopped;
        m_playerHealth.OnPlayerGetsDamage += StopRegeneration;
    }

    void RegenerateAfterPlayerStopped()
    {
        if (StaminaValue == MaxStaminaAmount || !m_playerSpeed.IsPlayerRunning) { return; }
        Regenerate();
    }

    void Burn()
    {
        StaminaValue -= SpendingSpeed * Time.deltaTime;
        StopRegeneration();
    }

    public void Regenerate()
    {
        StopRegeneration();
        StartCoroutine(m_regenerationCoroutine);
    }

    IEnumerator RegenerateCoroutine()
    {
        print("Started Regeneration");
        yield return m_timeoutBeforeRegeneration;
        print("Delay out");

        while (m_staminaValue < MaxStaminaAmount)
        {
            StaminaValue += m_regenerationSpeed * Time.deltaTime;
            yield return m_timeoutDuringRegeneration;
        }
    }

    public void StopRegeneration()
    {
        StopCoroutine(m_regenerationCoroutine);
        m_regenerationCoroutine = RegenerateCoroutine();
    }

    void OnDestroy()
    {
        m_playerSpeed.OnPlayerRun -= Burn;
        m_playerSpeed.OnPlayerStoppedRun -= Regenerate;
        m_playerMovement.OnPlayerStoppedMoving -= RegenerateAfterPlayerStopped;
        m_playerHealth.OnPlayerGetsDamage -= StopRegeneration;
    }
}


