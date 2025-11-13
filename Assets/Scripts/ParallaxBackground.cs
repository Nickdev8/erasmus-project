using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxBackground : MonoBehaviour
{
    public ParallaxCamera parallaxCamera;
    readonly List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>();

    void OnEnable() => AttachCameraEvents(autoAssignIfMissing: true);

    void OnDisable() => DetachCameraEvents();

    void Start() => RefreshLayers();

    void RefreshLayers()
    {
        parallaxLayers.Clear();
        float viewWidth = CalculateCameraViewWidth();
        Transform cameraTransform = GetCameraTransform();

        for (int i = 0; i < transform.childCount; i++)
        {
            ParallaxLayer layer = transform.GetChild(i).GetComponent<ParallaxLayer>();

            if (layer != null)
            {
                layer.name = "Layer-" + i;
                layer.Configure(cameraTransform, viewWidth);
                parallaxLayers.Add(layer);
            }
        }
    }

    void Move(float delta)
    {
        foreach (ParallaxLayer layer in parallaxLayers)
        {
            layer.Move(delta);
        }
    }

    void AttachCameraEvents(bool autoAssignIfMissing)
    {
        if (parallaxCamera == null)
        {
            if (autoAssignIfMissing)
            {
                Camera mainCamera = Camera.main;

                if (mainCamera != null)
                {
                    parallaxCamera = mainCamera.GetComponent<ParallaxCamera>();

                    if (parallaxCamera == null)
                    {
                        parallaxCamera = mainCamera.gameObject.AddComponent<ParallaxCamera>();
                    }
                }
            }

            if (parallaxCamera == null)
            {
                Debug.LogWarning($"{nameof(ParallaxBackground)} does not have a {nameof(ParallaxCamera)} reference. Assign one in the inspector.", this);
                return;
            }
        }

        RefreshLayers();

        parallaxCamera.onCameraTranslate -= Move;
        parallaxCamera.onCameraTranslate += Move;
    }

    void DetachCameraEvents()
    {
        if (parallaxCamera != null)
        {
            parallaxCamera.onCameraTranslate -= Move;
        }
    }

    Transform GetCameraTransform()
    {
        if (parallaxCamera != null)
        {
            return parallaxCamera.transform;
        }

        Camera camera = Camera.main;
        return camera != null ? camera.transform : null;
    }

    float CalculateCameraViewWidth()
    {
        Camera cameraComponent = null;

        if (parallaxCamera != null)
        {
            cameraComponent = parallaxCamera.GetComponent<Camera>();
        }

        if (cameraComponent == null)
        {
            cameraComponent = Camera.main;
        }

        if (cameraComponent == null || !cameraComponent.orthographic)
        {
            return 0f;
        }

        return cameraComponent.orthographicSize * 2f * cameraComponent.aspect;
    }
}
