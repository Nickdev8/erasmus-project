using System.Collections;
using UnityEngine;

public class enemyScript : MonoBehaviour
{
    [Header("Enemy Health Points:")]
    [SerializeField] private int maxHp;
    [SerializeField] private int currentHp;
    private SpriteRenderer spriteRenderer;

    // Update is called once per frame
    private void Start()
    {
        maxHp = 3;
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHp = maxHp;
    }
    public void TakeDamage(int damageTaken)
    {
        StartCoroutine(blinkRed());
        currentHp -= damageTaken;
        {
            if (currentHp <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
    private IEnumerator blinkRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }
}


