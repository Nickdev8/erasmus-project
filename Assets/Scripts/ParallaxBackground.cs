using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxBackground : MonoBehaviour
{
    public ParallaxCamera parallaxCamera;
    List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>();

    void OnEnable() => AttachCameraEvents(autoAssignIfMissing: true);

    void OnDisable() => DetachCameraEvents();

    void Start() => SetLayers();

    void SetLayers()
    {
        parallaxLayers.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            ParallaxLayer layer = transform.GetChild(i).GetComponent<ParallaxLayer>();

            if (layer != null)
            {
                layer.name = "Layer-" + i;
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
}
