using System;
using UnityEngine;

public class waffleScript : MonoBehaviour
{
    [SerializeField] private float waffleSpeed;
    [SerializeField] private Rigidbody2D waffleRB;
    [SerializeField] private int waffleDamage;


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
