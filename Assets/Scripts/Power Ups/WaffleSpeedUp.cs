using UnityEngine;

public class WaffleSpeedUp : waffleScript
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        print("Changed waffle");
        waffleSpeed = 150f;
        print(waffleSpeed);

    }
}
