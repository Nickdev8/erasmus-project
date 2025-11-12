using TMPro.EditorUtilities;
using UnityEngine;

public class WaffleDamageUp : waffleScript
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        waffleDamage = 1;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("DamageUp"))
        {
            waffleDamage ++;
            Destroy(other.gameObject);
        }
    }
}
