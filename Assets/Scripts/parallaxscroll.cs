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

    private Vector3 lastCameraPosition;

    private void Awake()
    {
        lastCameraPosition = transform.position;
    }

    private void LateUpdate()
    {
        Vector3 delta = transform.position - lastCameraPosition;

        if (delta.sqrMagnitude <= Mathf.Epsilon)
        {
            return;
        }

        foreach (Layer layer in layers)
        {
            if (layer.transform == null)
            {
                continue;
            }

            Vector3 target = layer.transform.position + delta * layer.parallaxFactor;

            if (lockYAxis)
            {
                target.y = layer.transform.position.y;
            }

            layer.transform.position = target;
        }

        lastCameraPosition = transform.position;
    }
}
