using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Aspect;

public class NPCController : MonoBehaviour
{
    public Aspect.Affiliation targetAffiliation;
    public int playerHealth;

    private float detectionRate = 1.0f;
    private float elapsedTime = 0.0f;
    private int FieldOfView = 45;
    private int ViewDistance = 100;
    private Transform playerTrans;
    private Vector3 rayDirection;

    [Header("Patrol Path")]
    [SerializeField] public float moveSpeed;
    [SerializeField] public float rotationSpeed;
    [SerializeField] public List<Transform> Waypoints;
    [SerializeField] public int index = 0;

    void Start()
    {
        Aspect aspect = gameObject.AddComponent<Aspect>();
        aspect.affiliation = Aspect.Affiliation.NPC;
        Initialize();
        playerHealth = 100;

        moveSpeed = 1.0f;
        rotationSpeed = 45.0f;
    }
    void Update()
    {
        UpdateSense();
    }

    public void Initialize()
    {
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void UpdateSense()
    {
        elapsedTime += Time.deltaTime;
        if(this.GetComponent<SimpleFSM>() != null)
        {
            if (elapsedTime >= detectionRate && this.GetComponent<SimpleFSM>().currentState != SimpleFSM.NPCState.Idle)
            {
                DetectAspect();
                elapsedTime = 0.0f;
            }
        }
        else if (this.GetComponent<FactoryDesignPattern>() != null)
        {
            if (elapsedTime >= detectionRate && this.GetComponent <FactoryDesignPattern>().IsIdling == false )
            {
                DetectAspect();
                elapsedTime = 0.0f;
            }
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.GetComponent<SimpleFSM>() != null)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                this.GetComponent<SimpleFSM>().IsPlayerInRange = true;
                if (this.GetComponent<SimpleFSM>().currentState == SimpleFSM.NPCState.Attack)
                {
                    playerHealth -= 10;
                }
            }
        }
        else if (this.GetComponent<FactoryDesignPattern>() != null)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                this.GetComponent<FactoryDesignPattern>().IsPlayerInRange = true;
                if (this.GetComponent<FactoryDesignPattern>().IsAttaking == true)
                {
                    playerHealth -= 10;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
        if (this.GetComponent<SimpleFSM>() != null)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                this.GetComponent<SimpleFSM>().IsPlayerInRange = false;
            }
        }
        else if (this.GetComponent<FactoryDesignPattern>() != null)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                this.GetComponent<FactoryDesignPattern>().IsPlayerInRange = false;
            }
        }
    }

    private void DetectAspect()
    {
        rayDirection = (playerTrans.position - transform.position).normalized;

        if (Vector3.Angle(rayDirection, transform.forward) < FieldOfView)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, rayDirection, out hit, ViewDistance))
            {
                Aspect aspect = hit.collider.GetComponent<Aspect>();
                if (aspect != null)
                {
                    // Check the aspect
                    if (aspect.affiliation == targetAffiliation)
                    {
                        Debug.Log("Player Detected");
                        
                        if (this.GetComponent<SimpleFSM>() != null)
                        {
                            this.GetComponent<SimpleFSM>().IsPlayerDetected = true;
                        }
                        else if (this.GetComponent<FactoryDesignPattern>() != null)
                        {
                            this.GetComponent<FactoryDesignPattern>().IsPlayerDetected = true;
                        }
                    }
                }
            }
        }
        else
        {
            if (this.GetComponent<SimpleFSM>() != null)
            {
                this.GetComponent<SimpleFSM>().IsPlayerDetected = false;
            }
            else if (this.GetComponent<FactoryDesignPattern>() != null)
            {
                this.GetComponent<FactoryDesignPattern>().IsPlayerDetected = false;
            }
        }
        
    }

    private void OnDrawGizmos()
    {
        if (!Application.isEditor || playerTrans == null)
            return;

        Debug.DrawLine(transform.position, playerTrans.position, Color.red);
        Vector3 frontRayPoint = transform.position + (transform.forward * ViewDistance);

        Vector3 leftRayPoint = Quaternion.Euler(0, FieldOfView * 0.5f, 0) * frontRayPoint;
        Vector3 rightRayPoint = Quaternion.Euler(0, -FieldOfView * 0.5f, 0) * frontRayPoint;

        Debug.DrawLine(transform.position, frontRayPoint, Color.green);
        Debug.DrawLine(transform.position, leftRayPoint, Color.green);
        Debug.DrawLine(transform.position, rightRayPoint, Color.green);
    }

    public void FollowPath()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        transform.position = Vector3.Lerp(transform.position, Waypoints[index].position, moveSpeed / 10 * Time.deltaTime);
        if (Vector3.Distance(transform.position, Waypoints[index].position) < 1.0f)
        {
            index = (index + 1) % Waypoints.Count;
            Debug.Log($"waypoint index: {index}");
        }
    }

    public void Chase()
    {
        this.transform.position = transform.position = Vector3.Lerp(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position, moveSpeed * Time.deltaTime);
    }
}
