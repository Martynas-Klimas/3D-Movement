using System.Collections;
using UnityEngine;

public class Rifle : Gun
{
    protected override void HandleShooting()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Fire(); // fire holding
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Fire(); // fire tapping
        }
    }
}
