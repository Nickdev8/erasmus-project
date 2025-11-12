using UnityEngine;

public class enemyScript : MonoBehaviour
{
    [Header("Enemy Health Points:")]
    [SerializeField] private int maxHp;
    [SerializeField] private int currentHp;

    // Update is called once per frame
    private void Start()
    {
        maxHp = 3;
        currentHp = maxHp;
    }
    public void TakeDamage(int damageTaken)
    {
        currentHp -= damageTaken;
        {
            if(currentHp <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
