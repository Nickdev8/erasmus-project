using UnityEngine;

public class Healing : Healthbar
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Healing power-up collected!");
            if (Health < 5)
            {
                Health += 1;
                UnityEngine.Debug.Log("Player healed! Current Health: " + Health);
                Destroy(gameObject);
            }
        }
    }
}
