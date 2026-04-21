using UnityEngine;

public class realEnemy : MonoBehaviour
{
    private float speed = 2.0f;
    [SerializeField] private Transform[] points;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    private int i;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Mathf.Abs(transform.position.x - points[i].position.x) < 0.25f)
        {
            i++;
            if (i >= points.Length)
            {
                i = 0;
            }
        }

        spriteRenderer.flipX = (transform.position.x - points[i].position.x) > 0;
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            Vector2 newPos = Vector2.MoveTowards(rb.position, points[i].position, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.fixedDeltaTime);
        }
    }
}