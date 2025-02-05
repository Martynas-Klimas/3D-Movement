using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Gun settings")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private int maxAmmoMag = 20;
    [SerializeField] private int maxAmmo = 60;
    [SerializeField] private float reloadTime = 1f;
    [SerializeField] private Camera playerCamera;
    private int currAmmoMag;
    private State state;
    //particle system, etc.

    //todo functions:;
    // fire and deal damage
    // reduce currAmmoMag, when 0, play reload animation

    // Update is called once per frame

    private enum State
    {
        Firing,
        Reloading
    }

    void Awake()
    {
        currAmmoMag = maxAmmoMag;
    }

    void Update()
    {
        Fire();
    }
    
    private void Fire()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && currAmmoMag > 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit))
            {
                Debug.Log($"Shot fired, ammo in magazine: {currAmmoMag}");
                Enemy enemy = hit.transform.GetComponent<Enemy>();
                if (enemy != null) {
                    enemy.TakeDamage(damage);
                }
                state = State.Firing;
                currAmmoMag--;
            }
        }
    }
}
