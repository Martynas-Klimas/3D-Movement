using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Gun settings")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private int maxAmmoMag = 20;
    [SerializeField] private float fireDelay = 0.2f;
    [SerializeField] private float reloadTime = 2f;
    [SerializeField] private Camera playerCamera;   
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject hitEffect;
    //maybe add max weapon ammo and ammo pickups in the future
    private int currAmmoMag;
    private float currFireDelay;
    private bool isReloading = false;
    //particle system, etc.

    //todo functions:;
    // fire and deal damage
    // reduce currAmmoMag, when 0, play reload animation

    void Awake()
    {
        currAmmoMag = maxAmmoMag;
        currFireDelay = fireDelay;
    }

    void Update()
    {   
        ResetFireDelay();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Fire();
        }
        if(currAmmoMag <= 0 || Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }
    }
    
    private void Fire()
    {
        if (currAmmoMag > 0 && !isReloading && currFireDelay <= 0f)
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit))
            {
                Debug.Log($"Shot fired, ammo in magazine: {currAmmoMag}");
                Enemy enemy = hit.transform.GetComponent<Enemy>();
                if (enemy != null) {
                    enemy.TakeDamage(damage);
                }
                //particle effects
                currAmmoMag--;
                currFireDelay = fireDelay;
                muzzleFlash.Play();
                GameObject impact = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 5f);
            }
        }
    }

    private IEnumerator Reload()
    {
        Debug.Log("Reloading");
        isReloading = true;
        currAmmoMag = maxAmmoMag;
        yield return new WaitForSeconds(reloadTime);
        isReloading = false;
    }

    private void ResetFireDelay()
    {
        if (currFireDelay <= 0f)
        {
            currFireDelay = 0f;
        }
        else
        {
            currFireDelay -= Time.deltaTime;
        }
    }
}
