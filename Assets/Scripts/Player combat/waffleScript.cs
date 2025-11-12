using System;
using UnityEngine;

public class waffleScript : MonoBehaviour
{
    [SerializeField] protected float waffleSpeed;
    private Rigidbody2D waffleRB;
    [SerializeField] private int waffleDamage;


    private void Start()
    {
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
