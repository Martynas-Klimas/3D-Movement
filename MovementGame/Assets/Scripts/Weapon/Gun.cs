using System.Collections;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    // abstract gun class
    [Header("Gun settings")]
    [SerializeField] protected float damage = 10f;
    [SerializeField] protected int maxAmmoMag = 20;
    [SerializeField] protected float fireDelay = 0.2f;
    [SerializeField] protected float reloadTime = 2f;
    [SerializeField] protected Camera playerCamera;
    [SerializeField] protected ParticleSystem muzzleFlash;
    [SerializeField] protected GameObject hitEffect;
    
    //maybe add ammo pickups in the future and max overall ammo

    protected int currAmmoMag;
    protected float currFireDelay;
    protected bool isReloading = false;
    protected Animator animator;

    //for gun position reset
    protected Vector3 originalGunPosition;
    protected Quaternion originalGunRotation;


    protected virtual void Awake()
    {
        currAmmoMag = maxAmmoMag;
        currFireDelay = fireDelay;
        animator = GetComponent<Animator>();
        originalGunPosition = transform.localPosition;
        originalGunRotation = transform.localRotation;
    }

    protected virtual void Update()
    {
        ResetFireDelay();
        HandleShooting();

        
        if (currAmmoMag <= 0 || Input.GetKeyDown(KeyCode.R))
        {
            if (!isReloading && this.gameObject.activeSelf)
            {
                StartCoroutine(Reload());
            }
        }
    }

    protected void OnDisable()
    {
        isReloading = false;
        animator.SetBool("Reloading", false);
        // weapon switching while reloading works properly
        ResetGunPosition();
    }

    protected virtual void Fire()
    {
        if (currAmmoMag > 0 && !isReloading && currFireDelay <= 0f)
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit))
            {
                Debug.Log($"Shot fired, ammo in magazine: {currAmmoMag}");
                Enemy enemy = hit.transform.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }

                currAmmoMag--;
                currFireDelay = fireDelay;
                //particle effects
                muzzleFlash.Play();
                GameObject impact = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 5f);
            }
        }
    }

    protected virtual IEnumerator Reload()
    {
        Debug.Log("Reloading");
        isReloading = true;
        animator.SetBool("Reloading", true);
        yield return new WaitForSeconds(reloadTime);
        currAmmoMag = maxAmmoMag;
        animator.SetBool("Reloading", false);
        yield return new WaitForSeconds(0.25f);
        isReloading = false;
    }

    protected void ResetFireDelay()
    {
        currFireDelay = Mathf.Max(0f, currFireDelay - Time.deltaTime);
    }

    protected void ResetGunPosition()
    {
        transform.localPosition = originalGunPosition;
        transform.localRotation = originalGunRotation;
    }

    protected abstract void HandleShooting(); //method to override for pistol and rifle
}
