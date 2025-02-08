using System.Collections;
using UnityEngine;

public class Pistol : Gun
{
    protected override void HandleShooting()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Fire();
        }
    }
}
