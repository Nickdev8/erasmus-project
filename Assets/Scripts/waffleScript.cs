using System;
using UnityEngine;

public class waffleScript : MonoBehaviour
{
    [SerializeField] private float waffleSpeed;
    [SerializeField] private Rigidbody2D waffleRB;

    // Update is called once per frame
    void Update()
    {
        waffleRB.linearVelocityX += waffleSpeed * Time.deltaTime;
    }
}
