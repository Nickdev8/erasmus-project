using UnityEngine;
using UnityEngine.InputSystem;

public class throwScript : MonoBehaviour
{
    public GameObject wafflePrefabs;
    public Transform throwPoint;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
       GameObject waffle =  Instantiate(wafflePrefabs, throwPoint.position, throwPoint.rotation);
    }
}
