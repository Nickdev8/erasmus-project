using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxLayer : MonoBehaviour
{
    [Range(-2f, 2f)]
    public float parallaxFactor = 0.5f;

    [Header("Repeating")]
    [SerializeField] private bool loopHorizontally = true;
    [SerializeField] private float padding = 0.5f;
    [SerializeField] private bool autoSizeToView = true;
    [SerializeField] private int minimumSegments = 3;
    [SerializeField] private SpriteRenderer templateRenderer;

    private readonly List<Transform> activeSegments = new();
    private Transform cameraTransform;
    private float viewWidth;
    private float segmentWidth;
    private bool segmentsPrepared;
    private Vector3 templateLocalPosition;
    private Quaternion templateLocalRotation;
    private Vector3 templateLocalScale = Vector3.one;

    private void Awake()
    {
        CacheTemplateRenderer();
    }

    public void Configure(Transform cameraTransform, float viewWidth)
    {
        this.cameraTransform = cameraTransform;
        this.viewWidth = viewWidth;

        if (loopHorizontally)
        {
            PrepareSegments();
        }
    }

    public void Move(float delta)
    {
        transform.localPosition += Vector3.right * (delta * parallaxFactor);
        UpdateLooping();
    }

    private void PrepareSegments()
    {
        if (segmentsPrepared || !loopHorizontally)
        {
            return;
        }

        if (!CacheTemplateRenderer())
        {
            Debug.LogWarning($"{nameof(ParallaxLayer)} on {name} needs a {nameof(SpriteRenderer)} (on self or child) to build repeating layers.", this);
            loopHorizontally = false;
            return;
        }

        if (!Application.isPlaying)
        {
            // Do not spawn runtime helpers while editing.
            return;
        }

        if (autoSizeToView && viewWidth > 0f)
        {
            float currentWidth = templateRenderer.bounds.size.x;

            if (currentWidth < viewWidth)
            {
                float scaleFactor = (viewWidth + padding * 2f) / currentWidth;
                Vector3 scale = transform.localScale;
                scale.x *= scaleFactor;
                transform.localScale = scale;
            }
        }

        segmentWidth = templateRenderer.bounds.size.x;
        templateRenderer.enabled = false;

        activeSegments.Clear();

        int requiredSegments = Mathf.Max(minimumSegments, Mathf.CeilToInt((viewWidth + padding * 2f) / segmentWidth) + 2);
        float startingOffset = -segmentWidth * (requiredSegments - 1) * 0.5f;
        Vector3 baseOffset = templateLocalPosition;
        Quaternion baseRotation = templateLocalRotation;
        Vector3 baseScale = templateLocalScale;

        for (int i = 0; i < requiredSegments; i++)
        {
            SpriteRenderer segmentRenderer = CreateSegmentFromTemplate(templateRenderer, i);
            Transform segmentTransform = segmentRenderer.transform;
            Vector3 localPos = baseOffset + Vector3.right * (startingOffset + segmentWidth * i);
            segmentTransform.localPosition = localPos;
            segmentTransform.localRotation = baseRotation;
            segmentTransform.localScale = baseScale;
            activeSegments.Add(segmentRenderer.transform);
        }

        segmentsPrepared = true;
    }

    private SpriteRenderer CreateSegmentFromTemplate(SpriteRenderer template, int index)
    {
        GameObject segment = new GameObject($"{template.gameObject.name}_Segment_{index}");
        segment.transform.SetParent(transform, false);
        SpriteRenderer renderer = segment.AddComponent<SpriteRenderer>();
        renderer.sprite = template.sprite;
        renderer.color = template.color;
        renderer.flipX = template.flipX;
        renderer.flipY = template.flipY;
        renderer.sortingLayerID = template.sortingLayerID;
        renderer.sortingOrder = template.sortingOrder;
        renderer.spriteSortPoint = template.spriteSortPoint;
        renderer.sharedMaterial = template.sharedMaterial;
        renderer.drawMode = template.drawMode;
        renderer.size = template.size;
        renderer.maskInteraction = template.maskInteraction;
        return renderer;
    }

    private void UpdateLooping()
    {
        if (!loopHorizontally || !segmentsPrepared || cameraTransform == null || segmentWidth <= 0f || activeSegments.Count == 0)
        {
            return;
        }

        float halfView = (viewWidth * 0.5f) + padding;
        float cameraX = cameraTransform.position.x;
        bool movedSegment = false;

        for (int i = 0; i < activeSegments.Count; i++)
        {
            Transform segment = activeSegments[i];
            float deltaToCamera = cameraX - segment.position.x;

            if (deltaToCamera > halfView)
            {
                Transform last = activeSegments[activeSegments.Count - 1];
                Vector3 newLocalPos = last.localPosition + Vector3.right * segmentWidth;
                segment.localPosition = newLocalPos;

                activeSegments.RemoveAt(i);
                activeSegments.Add(segment);
                movedSegment = true;
                break;
            }

            if (-deltaToCamera > halfView)
            {
                Transform first = activeSegments[0];
                Vector3 newLocalPos = first.localPosition - Vector3.right * segmentWidth;
                segment.localPosition = newLocalPos;

                activeSegments.RemoveAt(i);
                activeSegments.Insert(0, segment);
                movedSegment = true;
                break;
            }
        }

        if (movedSegment)
        {
            // ensure list order matches world-space X for subsequent frames
            activeSegments.Sort((a, b) => a.position.x.CompareTo(b.position.x));
        }
    }

    private void OnDisable()
    {
        if (!segmentsPrepared)
        {
            return;
        }

        foreach (Transform segment in activeSegments)
        {
            if (segment != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(segment.gameObject);
                }
                else
                {
                    DestroyImmediate(segment.gameObject);
                }
            }
        }

        activeSegments.Clear();
        segmentsPrepared = false;

        if (templateRenderer != null)
        {
            templateRenderer.enabled = true;
        }
    }

    private bool CacheTemplateRenderer()
    {
        if (templateRenderer == null)
        {
            templateRenderer = GetComponent<SpriteRenderer>();

            if (templateRenderer == null)
            {
                foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>(includeInactive: true))
                {
                    if (renderer == null)
                    {
                        continue;
                    }

                    Transform rendererTransform = renderer.transform;

                    if (rendererTransform == transform)
                    {
                        templateRenderer = renderer;
                        break;
                    }

                    if (rendererTransform.parent != transform)
                    {
                        continue;
                    }

                    if (rendererTransform.name.Contains("_Segment_"))
                    {
                        continue;
                    }

                    templateRenderer = renderer;
                    break;
                }
            }
        }

        if (templateRenderer == null)
        {
            return false;
        }

        Transform templateTransform = templateRenderer.transform;
        templateLocalPosition = templateTransform.localPosition;
        templateLocalRotation = templateTransform.localRotation;
        templateLocalScale = templateTransform.localScale;
        return true;
    }
}
