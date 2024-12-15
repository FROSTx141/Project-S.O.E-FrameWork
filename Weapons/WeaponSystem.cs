
using UnityEngine;
using TMPro;


public class WeaponSystem : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;

    [SerializeField] private Camera cam;
    [SerializeField] private Transform attackPoint;
    private RaycastHit rayHit;
    [SerializeField] private float bulletRange;
    [Tooltip("FireRate always 0.1 for it to Work")]
    [SerializeField] private float fireRate, reloadTime;
    [SerializeField] private bool isAutomatic;
    [SerializeField] private int magSize;
    [SerializeField] private int reserveAmmo;
    private int ammoLeft;
    private bool isShooting, readyToShoot, Reloading;

    //VFx
    [Header("VFX")]
    [SerializeField] private  ParticleSystem muzzleFlash;
    [SerializeField] private GameObject MuzzleFlashParent;
    [SerializeField] private  TextMeshProUGUI ammoDisplay;
    [SerializeField] private  GameObject HitMarker;
    [SerializeField] private GameObject bulletShellPrefab;
    [SerializeField] private Transform EjectPoint;
    [SerializeField] private float shellEjectForce = 5f;
    [SerializeField] private float shellTorque = 100f;
    

    [Header("Impact Effects and Feedback")]
    [SerializeField] private ParticleSystem impactEffect;
    [SerializeField] private ParticleSystem concreteImpact;
    [SerializeField] private ParticleSystem metalImpact;
    [SerializeField] private ParticleSystem woodImpact;
    [SerializeField] private ParticleSystem enemyImpact;



    //Reload
    private bool reload;
    

    //Refernces
    [Header("References")]
    private Recoil recoil_cs;
    [SerializeField] private GameObject Gun;

    [Header("Scriptable Audio Object")]
    public AudioConfigScriptableObject audioConfig;
    [SerializeField] private AudioSource ShootingAudioSource;

    /*[Header("Audio")]
    [Range(0, 1f)]
    [SerializeField] private float Volume = 1f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip ARClip;*/
    
    
    

    private void Awake() 
    {
        ammoLeft = magSize;
        readyToShoot = true;
        HitMarker.SetActive(false);
        recoil_cs = GetComponentInParent<Recoil>();
        ShootingAudioSource = GetComponent<AudioSource>();
       
    }
    

     void Update() 
    {

        //Set ammo Display
        if(ammoDisplay != null)
        {
            ammoDisplay.SetText(ammoLeft + " / " + reserveAmmo);
        }


        if(isShooting && readyToShoot && !Reloading && ammoLeft > 0)
        {
            PerformedShot();
            
        }
       

       

        inputManager.onFoot.Fire.started += ctx => StartShot();
        inputManager.onFoot.Fire.canceled += ctx => EndShot();
        inputManager.onFoot.Reload.performed += ctx => Reload();

       if(reserveAmmo <= 0)
       {
            Invoke("Reload", 100);
       }

       

       
        
    }


    public void StartShot()
    {
        isShooting = true;
        PlayMuzzleFlash();
        
    }

    public void EndShot()
    {
        isShooting = false;
        //MuzzleFlash Disable/Stop
        StopMuzzleFlash();
    }

    public void PerformedShot()
    {
        readyToShoot = false;
        audioConfig.PlayShootingClip(ShootingAudioSource, ammoLeft == 1);
        //audioSource.PlayOneShot(ARClip, Volume);
        //Debug.Log("Played", ARClip);
        
        


        Vector3 direction = cam.transform.forward;

        if(Physics.Raycast(cam.transform.position, direction, out rayHit, bulletRange))
        {
            Debug.Log(rayHit.collider.gameObject.name);
            if(rayHit.collider.CompareTag("Enemy"))
            {
                HitMarkerActive();
                Invoke("HitMarkerDisable", 0.2f);
            }

            //ImpactEffectVFX
            ParticleSystem selectedEffect = GetParticlePrefabForLayer(rayHit.collider.gameObject.layer);

            if (selectedEffect != null)
            {
                Debug.Log($"Instantiating particle system: {selectedEffect.name}");

                ParticleSystem effect = Instantiate(selectedEffect, rayHit.point, Quaternion.LookRotation(rayHit.normal));

                // Ensure the instantiated object is valid
                if (effect != null)
                {
                    Debug.Log("Particle system instantiated successfully.");
                    Destroy(effect.gameObject, effect.main.duration);
                }
                else
                {
                    Debug.LogWarning("Failed to instantiate the particle system.");
                }
            }
            else
            {
                Debug.LogWarning("No particle system assigned for this layer.");
            }
            ///Impact Effect End
            ///
            ///
            ///
            //Shell Ejectiong Logic
            GameObject shell = Instantiate(bulletShellPrefab, EjectPoint.position, EjectPoint.rotation);

            Rigidbody rb = shell.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.AddForce(EjectPoint.right * shellEjectForce, ForceMode.Impulse);

                rb.AddTorque(Random.onUnitSphere * shellTorque, ForceMode.Impulse);
            }
            Destroy(shell, 2f);


            
        }


        ammoLeft--;

        if(ammoLeft >= 0)
        {
            Invoke("ResetShot", fireRate);
            

            if(!isAutomatic)
            {
                EndShot();
            }
        }

        if(ammoLeft <= 0)
        {
            recoil_cs.enabled = false;
            MuzzleFlashParent.SetActive(false);
            audioConfig.PlayOutOfAmmoClip(ShootingAudioSource);
            
        }
        else
        {
            recoil_cs.enabled = true;
            MuzzleFlashParent.SetActive(true);  
        }




    }

    private void ResetShot()
    {
        readyToShoot = true;
       
    }


   public void Reload()
{
    // Prevent reload if reserve ammo is 0 or the magazine is already full
    if (reserveAmmo <= 0 || ammoLeft == magSize)
    {
        Reloading = false;
        return; // Exit without doing anything
    }

    reload = true;
    Reloading = true;

    // Start the reload process
    Invoke("ReloadFinish", reloadTime);
    Gun.GetComponent<Animator>().SetBool("Reload", reload);
    
}


private void ReloadFinish()
{
    // If reserve ammo is 0, do not reload
    if (reserveAmmo <= 0)
    {
        Reloading = false;
        Gun.GetComponent<Animator>().SetBool("Reload", false);
        reload = false;
        return; // Exit the function
    }

    // Check if the magazine is already full
    if (ammoLeft == magSize)
    {
        Reloading = false;
        Gun.GetComponent<Animator>().SetBool("Reload", false);
        reload = false;
        return; // Exit the function if the magazine is full
    }

    // Calculate how much ammo is needed to fill the magazine
    int ammoNeeded = magSize - ammoLeft;

    // Determine how much ammo to add to the magazine
    int ammoToReload = Mathf.Min(ammoNeeded, reserveAmmo);

    // Update ammo counts
    ammoLeft += ammoToReload;
    reserveAmmo -= ammoToReload;

    // Finish reloading
    Reloading = false;
    Gun.GetComponent<Animator>().SetBool("Reload", false);
    reload = false;
}

//MuzzlFlash Function

private void PlayMuzzleFlash()
{
    if (muzzleFlash != null)
    {
        muzzleFlash.Stop();   // Stop the particle system if it's playing
        muzzleFlash.Clear();  // Clear any lingering particles
        muzzleFlash.Play();   // Start the particle system
    }
}

private void StopMuzzleFlash()
{
    muzzleFlash.Stop();
    muzzleFlash.Clear();
}

    private void HitMarkerActive()
    {
        HitMarker.SetActive(true);
    }
    private void HitMarkerDisable()
    {
        HitMarker.SetActive(false);
    }


    #region 
    //Impact function
private ParticleSystem GetParticlePrefabForLayer(int layer)
{
    // Get the layer name from the layer ID
    string layerName = LayerMask.LayerToName(layer);

    switch (layerName)
    {
        case "Wood": // Layer name "Wood"
            return woodImpact;
        case "Metal": // Layer name "Metal"
            return metalImpact;
        case "Concrete": // Layer name "Concrete"
            return concreteImpact;
        case "Enemy": // Layer name "Enemy"
            return enemyImpact;
        default:
            return impactEffect; // Fallback to default impact particle effect
    }
}

    #endregion


    

}