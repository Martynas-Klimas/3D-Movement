using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    [SerializeField] private Transform[] weapons;
    [SerializeField] private float switchTime = 0.5f;
    
    private int weaponIndex = 0; // for changing weapons
    private int selectedWeapon = 0; // index which will be passed to weapons[] to select 
    private int previousWeapon = 0;
    private float timeSinceLastSwitch = 0;
    private void Start()
    {
        SetWeapons();
        SelectWeapon(selectedWeapon);
    }

    private void Update()
    {
        previousWeapon = selectedWeapon;
        if (Input.GetKeyDown(KeyCode.Q) && timeSinceLastSwitch <= 0f)
        {
            weaponIndex++;
            selectedWeapon = weaponIndex % weapons.Length;
        }
        if (previousWeapon != selectedWeapon)
        {
            SelectWeapon(selectedWeapon);
        }

        timeSinceLastSwitch -= Time.deltaTime;
    }

    private void SetWeapons()
    {
        for (int i = 0; i < transform.childCount; i++) {
            weapons[i] = transform.GetChild(i);
        }
    }

    private void SelectWeapon(int weaponNumber)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            //set active if condition is true, deactivate other wise
            weapons[i].gameObject.SetActive(i == weaponNumber);
            timeSinceLastSwitch = switchTime;
        }
    }
}
