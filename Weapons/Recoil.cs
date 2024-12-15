
using UnityEngine;

public class Recoil : MonoBehaviour
{
    [Header("Recoil Settings")]
    //[Range(0, 1)]
    //[SerializeField] private float recoilPerecnt = 0.3f;
    [Range(0, 2)]
    [SerializeField] private float recoverPercent = 0.7f;
    [Space]
    [SerializeField] private float recoilUp = 1f;
    [SerializeField] private float recoilBack = 0f;

    private Vector3 originalPosition;
    private Vector3 recoilVelocity = Vector3.zero;
    private float recoilLength;
    private float recoverLength;
    private bool recoiling;
    public bool recovering;

[Tooltip("Fire rate 10 to work")]
    public float fireRate;
    private float nextFire;
    
    public KeyCode fireKey = KeyCode.Mouse0; 

    [SerializeField] private InputManager inputManager;

    private void Start() 
    {
        originalPosition = transform.localPosition;

        recoilLength = 0;
        recoverLength = 1 / fireRate *recoverPercent;

       
    }

    private void Update() 
    {
        if (recoiling)
        {
            RecoilF();
        }

        if (recovering)
        {
            Recovering();
        }

       
        //inputManager.onFoot.Fire.performed += ctx => RecoilFire();
        //nextFire = 1 / fireRate;

        if (Input.GetKey(fireKey) && Time.time >= nextFire)
        {
            RecoilFire();
            nextFire = Time.time + (1 / fireRate); // Set the next allowed fire time
        }

            

      


    }

    void RecoilFire()
    {
        recoiling = true;
        recovering = false;
    }

    void RecoilF()
    {
          Vector3 finalPosition = new Vector3(originalPosition.x, originalPosition.y, originalPosition.z - recoilBack);

            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPosition, ref recoilVelocity, recoilLength);

            if (transform.localPosition == finalPosition)
            {
                recoiling = false;
                recovering = true;
            }

    }

    public void Recovering()
        {

            Vector3 finalPosition = originalPosition;

            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPosition, ref recoilVelocity, recoverLength);

            if (transform.localPosition == finalPosition)
            {
                recoiling = false;
                recovering = false;
            }
        }
}