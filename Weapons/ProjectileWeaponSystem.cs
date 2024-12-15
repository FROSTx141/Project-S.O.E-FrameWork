
using UnityEngine;
using TMPro;

public class ProjectileWeaponSystem : MonoBehaviour
{
    public GameObject bullet;

    public float shootForce, upwardForce;

    //Gun Stats
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magSize, bulletsPerTap;
    int bulletsLeft, bulletsShot;

    //Bools
    public bool allowButtonHold;
    bool shooting, readyToShoot, reloading;
    public bool allowInvoke = true;

    //VFX
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammoDisplay;

    //Refernces
    public Camera cam;
    public Transform attackPoint;
    private InputManager inputManager;

    private void Awake() 
    {
        bulletsLeft = magSize;
        readyToShoot = true;    
    }
    private void Update() 
    {
        

        WeaponInput();

        //Set ammo Display
        if(ammoDisplay != null)
        {
            ammoDisplay.SetText(bulletsLeft / bulletsPerTap + " / " + magSize / bulletsPerTap);
        }
    }

    void WeaponInput()
    {
        if(allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        //Reloading Input
        if(Input.GetKeyDown(KeyCode.R) && bulletsLeft < magSize && !reloading) Reload();
        //Reload Automatically when Mag is Empty and trying to Shoot
        if(readyToShoot && shooting && !reloading && bulletsLeft <= 0) Reload();

        //Shooting Input
        if(readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsLeft = 0;

            Shoot();
        }

    }

    void Shoot()
    {
        readyToShoot = false;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        //Check If ray hits Something
        Vector3 targetPoint;
        if(Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
        {
            targetPoint = ray.GetPoint(075);
        }
            

        //Calculate direction from attackPoint to targetPoint
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        //Calculate Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calculate new Direction with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x,y,0);

        //Instantiate Bullet Prefab
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        //Rotate bullet to shooting Direction
        //currentBullet.transform.forward = directionWithSpread.normalized;

        //Adding Force to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        //Upward Force
        currentBullet.GetComponent<Rigidbody>().AddForce(cam.transform.up * upwardForce, ForceMode.Impulse);

        //Instantiate muzzleflash VFX
        /*if(muzzleFlash != null)
        {
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
        }*/

        

        bulletsLeft--;
        bulletsShot++;

        //Invoke ResetShot() If not alreadt Invoked
        if(allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }

        //If more bullets per Tap
        if(bulletsShot < bulletsPerTap && bulletsLeft > 0)
        {
            Invoke("Shoot", timeBetweenShots);
        }
    }


    void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime); 
    }

    void ReloadFinished()

    {
        bulletsLeft = magSize;
        reloading = false;
    }
}
