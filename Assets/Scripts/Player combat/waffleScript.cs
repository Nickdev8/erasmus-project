using System;
using UnityEngine;

public class waffleScript : MonoBehaviour
{
    [SerializeField] protected float waffleSpeed;
    private Rigidbody2D waffleRB;


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

}
