﻿using UnityEngine;

class CameraBobbing : MonoBehaviour
{
    [SerializeField] float bobFrequency;
    [SerializeField] float bobHorizontalAmplitude;
    [SerializeField] float bobVerticalAmplitude;
    [SerializeField] [Range(0, 1)] float headBobSmoothing;

    float walkingTime;
    Transform cameraPosition;

    void Start()
    {
        cameraPosition = MainLinks.Instance.Camera;
    }

    void Update()
    {
        if (!IsPlayerMoving())
        {
            walkingTime = 0;
            return;
        }

        walkingTime += Time.deltaTime;

        Vector3 targetCameraPosition = transform.position + CalculateHeadBobbingOffset(walkingTime);
        MainLinks.Instance.Camera.position = Vector3.Lerp(MainLinks.Instance.Camera.position, targetCameraPosition, headBobSmoothing);

        if ((MainLinks.Instance.Camera.position - targetCameraPosition).magnitude <= 0.001) MainLinks.Instance.Camera.position = targetCameraPosition;
    }

    Vector3 CalculateHeadBobbingOffset(float time)
    {
        float horizontalOffset = Mathf.Sin(time * bobFrequency) * bobHorizontalAmplitude;
        float verticalOffset = Mathf.Cos(time * bobFrequency * 2) * bobVerticalAmplitude;
        Vector3 offset = transform.right * horizontalOffset + transform.up * verticalOffset;

        return offset;
    }

    bool IsPlayerMoving()
    {
        float horizontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");

        return horizontalMove != 0 || verticalMove != 0;
    }
}
