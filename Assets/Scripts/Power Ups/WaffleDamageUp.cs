using TMPro.EditorUtilities;
using UnityEngine;

public class WaffleDamageUp : waffleScript
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        print("Changed waffles");
        waffleDamage = 2;
        print(waffleDamage);
    }

}
