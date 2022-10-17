using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

using UnityEngine.UI;

public class TankController : MonoBehaviour
{
    public GameObject bullet;
    public GameObject turret;
    public GameObject bulletSpawnPoint;

    //Elementos para manejar el HP
    public float HP = 5f;
    public float maxHP = 5f;
    public Image barraHP;
    public const int vidaTotal = 5;
    //[SyncVar (hook = "CambioVida")] public int vidaActual = vidaTotal;

    public int vidaActual;

    public float curSpeed, targetSpeed;
    public float rotSpeed = 150.0f;
    public float turretRotSpeed = 10.0f;
    public float maxForwardSpeed = 30.0f;
    public float maxBackwardSpeed = -30.0f;
    public float shootRate = 0.5f;

    private float elapsedTime;

    PhotonView view;

    void OnEndGame() {
        // Don't allow any more control changes when the game ends
        this.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();

        if (view.IsMine)
        {
            vidaActual = vidaTotal;
            barraHP = GameObject.Find("Vida").GetComponent<Image>();
            FindObjectOfType<PhotonCamera>().setTarget(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (view.IsMine)
        {
            UpdateControl();
            UpdateWeapon();
            HPmanagement();
        }
    }

    void HPmanagement()
    {
        barraHP.fillAmount = HP / maxHP; 
    }

    void UpdateControl()
    {
        // Generate a plane that intersects the transform's position with an upwards normal.
        Plane playerPlane = new Plane(Vector3.up, transform.position);

        // Generate a ray from the cursor position
        Ray rayCast = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Determine the point where the cursor ray intersects the plane.

        // If the ray is parallel to the plane, Raycast will return false.
        if (playerPlane.Raycast(rayCast, out var hitDist))
        {
            // Get the point along the ray that hits the calculated distance.
            Vector3 rayHitPoint = rayCast.GetPoint(hitDist);

            Quaternion targetRotation = Quaternion.LookRotation(rayHitPoint - transform.position);
            turret.transform.rotation = Quaternion.Slerp(turret.transform.rotation, targetRotation, Time.deltaTime * turretRotSpeed);
        }

        if (Input.GetKey(KeyCode.W))
            targetSpeed = maxForwardSpeed;
        else if (Input.GetKey(KeyCode.S))
            targetSpeed = maxBackwardSpeed;
        else
            targetSpeed = 0f;

        if (Input.GetKey(KeyCode.A))
            transform.Rotate(0f, -rotSpeed * Time.deltaTime, 0f);
        else if (Input.GetKey(KeyCode.D))
            transform.Rotate(0f, rotSpeed * Time.deltaTime, 0f);

        //Determine current speed
        curSpeed = Mathf.Lerp(curSpeed, targetSpeed, 7.0f * Time.deltaTime);
        transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);
    }


    void UpdateWeapon() 
    {
        elapsedTime += Time.deltaTime;
        if (Input.GetMouseButtonDown(0))
        {
            if (elapsedTime >= shootRate)
            {
                //Reset the time
                elapsedTime = 0.0f;

                //Also Instantiate over the PhotonNetwork
                PhotonNetwork.Instantiate(bullet.name, bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.rotation);
            }
        }
    }


    void Die()
    {
        if (view.IsMine)
        {
            transform.position =  FindObjectOfType<PlayerSpawn>().Respawn();
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            HP--;

            if (HP <= 0)
            {

                HP = maxHP;

                Die();
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Heart")
        {

            HP++;
        }
    }
}
