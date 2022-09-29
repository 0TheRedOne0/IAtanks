using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM : MonoBehaviour
{
    private enum FSMStates
    {
        Patrol, Chase, Aim, Shoot, Evade
    }

    [SerializeField]
    private FSMStates currentState = FSMStates.Patrol;

    public GameObject bullet;
    public GameObject turret;
    public Transform playerTransform;
    public GameObject bulletSpawnPoint;
    int HP = 3;
    public float turnVelocity;
    private bool assign;

    public float timerAim;

    public List<GameObject> pointList;
    private int Index;

    private int health = 100;

    public float curSpeed, targetSpeed;
    public float rotSpeed = 150.0f;
    public float turretRotSpeed = 10.0f;
    public float maxForwardSpeed = 30.0f;
    public float maxBackwardSpeed = -30.0f;
    public float shootRate = 0.5f;
    private float elapsedTime;

    public float chaseRadius = 200f;
    public float AttackRadius = 100f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case FSMStates.Patrol:
                UpdatePatrol();

                break;

            case FSMStates.Chase:
                UpdateChase();
                break;

            case FSMStates.Aim:
                UpdateAim();
                break;

            

            case FSMStates.Evade:
                UpdateEvade();
                break;
        }
    }

    void UpdatePatrol()
    {
        if (Vector3.Distance(transform.position, playerTransform.position) <= chaseRadius) {
            currentState = FSMStates.Chase;
            return;
        }
        if (assign == false)
        {
            assign = true;
            Index = Random.Range(0, pointList.Count);
        }
        GameObject Target = pointList[Index];

        if (Target)
        {
            if (Vector3.Distance(transform.position, Target.transform.position) > 1f)
            {

                // Get the point along the ray that hits the calculated distance.
                Vector3 rayHitPoint = Target.transform.position;

                Quaternion targetRotation = Quaternion.LookRotation(rayHitPoint - transform.position);
                turret.transform.rotation = Quaternion.Slerp(turret.transform.rotation, targetRotation, Time.deltaTime * turretRotSpeed);

                targetSpeed = maxForwardSpeed;
                //transform.LookAt(new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z));
               transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotSpeed);


                //Determine current speed
                curSpeed = Mathf.Lerp(curSpeed, targetSpeed, 7.0f * Time.deltaTime);
                transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);
                
            }
            else
            {
                assign = false;
            }
        }
        
    }

    void UpdateChase()
    {
        assign = false;

        if (Vector3.Distance(transform.position, playerTransform.position) > chaseRadius)
        {
            currentState = FSMStates.Patrol;
            return;
        }
        
        GameObject Target =playerTransform.gameObject;

        if (Target)
        {
            if (Vector3.Distance(transform.position, Target.transform.position) > AttackRadius)
            {

                // Get the point along the ray that hits the calculated distance.
                Vector3 rayHitPoint = Target.transform.position;

                Quaternion targetRotation = Quaternion.LookRotation(rayHitPoint - transform.position);
                turret.transform.rotation = Quaternion.Slerp(turret.transform.rotation, targetRotation, Time.deltaTime * turretRotSpeed);

                targetSpeed = maxForwardSpeed;
                //transform.LookAt(new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotSpeed);


                //Determine current speed
                curSpeed = Mathf.Lerp(curSpeed, targetSpeed, 7.0f * Time.deltaTime);
                transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);

            }
            else
            {
                timerAim = 0;
                currentState = FSMStates.Aim;
            }
           
        }
    }

    void UpdateAim()
    {
        if (timerAim < 1.5f)

        {
            timerAim += Time.deltaTime;
            if (timerAim < 1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(playerTransform.transform.position - transform.position);
                turret.transform.rotation = Quaternion.Slerp(turret.transform.rotation, targetRotation, Time.deltaTime * turretRotSpeed);
            }
        }
        else {
            Instantiate(bullet, bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.rotation);
            currentState = FSMStates.Chase; 
        }

        
    }

    

    void UpdateEvade()
    {
        assign = false;

        if (timerAim > 3f)
        {
            currentState = FSMStates.Chase;
            return;
        }

        GameObject Target = playerTransform.gameObject;

        if (Target)
        {



            // Get the point along the ray that hits the calculated distance.
            Vector3 rayHitPoint = Target.transform.position;

            Quaternion targetRotation = Quaternion.LookRotation(rayHitPoint - transform.position);
            turret.transform.rotation = Quaternion.Slerp(turret.transform.rotation, targetRotation, Time.deltaTime * turretRotSpeed);

            targetSpeed = maxForwardSpeed;
            //transform.LookAt(new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotSpeed);


            //Determine current speed
            curSpeed = Mathf.Lerp(curSpeed, targetSpeed, 7.0f * Time.deltaTime);
            transform.Translate(Vector3.back * Time.deltaTime * curSpeed);
            timerAim += Time.deltaTime;

            
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            HP--;
            Debug.Log(HP);
            if (HP <= 0)
            {
                Destroy(this.gameObject);
            }

            if (currentState == FSMStates.Chase)
            {
                timerAim = 0f;
                currentState = FSMStates.Evade;
            }

        }
    }
    private void FixedUpdate()
    {

    }
}
