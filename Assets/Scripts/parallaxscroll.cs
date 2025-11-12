using UnityEngine;

public class parallaxscroll : MonoBehaviour
{
    [System.Serializable]
    private struct Layer
    {
        public Transform transform;
        [Range(0f, 1f)] public float parallaxFactor;
    }

    [SerializeField] private Layer[] layers;
    [SerializeField] private bool lockYAxis = true;
    [SerializeField] private bool infiniteScrollX = true;
    [SerializeField] private bool infiniteScrollY;

    private Vector3 lastCameraPosition;
    private Vector2[] layerSizes;

    private void Awake()
    {
        CacheLayerBounds();
        lastCameraPosition = transform.position;
    }

    private void LateUpdate()
    {
        Vector3 delta = transform.position - lastCameraPosition;

        if (delta.sqrMagnitude <= Mathf.Epsilon)
        {
            return;
        }

        for (int i = 0; i < layers.Length; i++)
        {
            Layer layer = layers[i];

            if (layer.transform == null)
            {
                continue;
            }

            Vector3 target = layer.transform.position + delta * layer.parallaxFactor;

            if (lockYAxis)
            {
                target.y = layer.transform.position.y;
            }

            target = ApplyInfiniteScroll(target, i);
            layer.transform.position = target;
        }

        lastCameraPosition = transform.position;
    }

    private Vector3 ApplyInfiniteScroll(Vector3 target, int layerIndex)
    {
        if (layerSizes == null || layerIndex >= layerSizes.Length)
        {
            return target;
        }

        Vector2 size = layerSizes[layerIndex];

        if (infiniteScrollX && size.x > Mathf.Epsilon)
        {
            target.x = WrapAxis(transform.position.x, target.x, size.x);
        }

        if (!lockYAxis && infiniteScrollY && size.y > Mathf.Epsilon)
        {
            target.y = WrapAxis(transform.position.y, target.y, size.y);
        }

        return target;
    }

    private float WrapAxis(float cameraCoord, float layerCoord, float length)
    {
        if (length <= Mathf.Epsilon)
        {
            return layerCoord;
        }

        float diff = cameraCoord - layerCoord;

        if (Mathf.Abs(diff) < length)
        {
            return layerCoord;
        }

        float loops = Mathf.Floor(diff / length);
        return layerCoord + loops * length;
    }

    private void CacheLayerBounds()
    {
        if (layers == null || layers.Length == 0)
        {
            layerSizes = new Vector2[0];
            return;
        }

        if (layerSizes == null || layerSizes.Length != layers.Length)
        {
            layerSizes = new Vector2[layers.Length];
        }

        for (int i = 0; i < layers.Length; i++)
        {
            Transform layerTransform = layers[i].transform;
            layerSizes[i] = layerTransform == null ? Vector2.zero : CalculateLayerSize(layerTransform);
        }
    }

    private static Vector2 CalculateLayerSize(Transform layerTransform)
    {
        Renderer[] renderers = layerTransform.GetComponentsInChildren<Renderer>();

        if (renderers == null || renderers.Length == 0)
        {
            return Vector2.zero;
        }

        Bounds bounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        return new Vector2(bounds.size.x, bounds.size.y);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        CacheLayerBounds();
    }
#endif
}
