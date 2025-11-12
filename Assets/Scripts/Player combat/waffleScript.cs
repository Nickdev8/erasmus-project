using UnityEngine;

public class waffleScript : MonoBehaviour
{
    protected float waffleSpeed;
    private Rigidbody2D waffleRB;
    protected int waffleDamage;


    private void Start()
    {
        waffleDamage = 1;
        waffleSpeed = 100f;
        waffleRB = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        waffleRB.linearVelocityX += waffleSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<enemyScript>().TakeDamage(waffleDamage);
            Destroy(gameObject);
        }
    }
}
