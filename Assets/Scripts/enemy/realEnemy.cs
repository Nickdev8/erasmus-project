using UnityEngine;

public class realEnemy : MonoBehaviour
{
    private float speed = 2.0f;
    [SerializeField] private Transform[] points;
    private SpriteRenderer spriteRenderer;

    private int i;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    void Update()
    {
        if (Vector2.Distance(transform.position, points[i].position) < 0.25f)
        {
            i++;
            if (i >= points.Length)
            {
                i = 0;
            }
        }
        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);

        spriteRenderer.flipX = (transform.position.x - points[i].position.x) > 0;

    }
}