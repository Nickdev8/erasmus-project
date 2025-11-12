using UnityEngine;
using System;

/// <summary>
/// Emits an event whenever the camera translates so parallax layers can respond.
/// </summary>
[ExecuteInEditMode]
public class ParallaxCamera : MonoBehaviour
{
    public event Action<float> onCameraTranslate;

    private float previousCameraX;

    private void Start()
    {
        previousCameraX = transform.position.x;
    }

    private void LateUpdate()
    {
        float currentX = transform.position.x;
        float delta = currentX - previousCameraX;

        if (!Mathf.Approximately(delta, 0f))
        {
            onCameraTranslate?.Invoke(delta);
            previousCameraX = currentX;
        }
    }
}
