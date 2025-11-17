using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class throwScript : MonoBehaviour
{
    public GameObject[] wafflePrefabs;
    public int waffleNumber;
    public Transform throwPoint;

    public int powerUpTime;
    public int speedUpTime;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("DamageUp"))
        {
            StartCoroutine(PowerUp());
            Destroy(other.gameObject);
        }
        else if(other.gameObject.CompareTag("SpeedUp"))
        {
            StartCoroutine(SpeedUp());
            Destroy(other.gameObject);
        }
    }
    public IEnumerator PowerUp()
    {
        waffleNumber = 1;
        print(waffleNumber);
        yield return new WaitForSeconds(powerUpTime);
        waffleNumber = 0;
        print(waffleNumber);
    }

    public IEnumerator SpeedUp()
    {
        waffleNumber = 2;
        print(waffleNumber);
        yield return new WaitForSeconds(speedUpTime);
        waffleNumber = 0;
        print(waffleNumber);
    }

    public void Shoot()
    {
        GameObject waffle = Instantiate(wafflePrefabs[waffleNumber], throwPoint.position, throwPoint.rotation);
    }

}
