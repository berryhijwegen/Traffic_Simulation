using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowRoad : MonoBehaviour
{
    public  GameObject      roadLeft, roadRight;
    private NavMeshAgent    agent;
    public  List<Transform> pointsLeft;
    public  List<Transform> pointsRight;
    public  Transform       target;
    private int             destPoint                   = 0;
    private bool            left                        = false;
    private int             totalPoints                 = 0;
    private int             layerMask;
    private float           speedOriginal;
    private int             navPointsToLookAhead        = 3;
    private bool            raycast;
    private bool            raycast2;
    private Vector3         rayOrigin;
    private RaycastHit      m_Hit;
    private RaycastHit      m_Hit2;
    public  float           speedCur                    = 0f;
    public  float           speedLimit                  = 10.0f;
    public  float           randomSpeedLowerLimit       = 0.5f;
    public  float           randomSpeedUpperLimit       = 1.5f;
    public  float           speedMultLeftLane           = 1.2f;
    public  float           breakPercBeforeLaneSwitch   = 0.8f;
    public  float           m_MaxDistance               = 2.0f;
    private Vector3         ray2size;
    private bool            nextToCar                   = false;
    private int             notNextToCar                = 0;
    private float           distance                    = 5.2f;
    private bool            goingToRight                = false;

    void Start()
    {
        float beam_width = 1.4f;
        ray2size = new Vector3(0,-1,beam_width);
        m_MaxDistance = 2.0f;

        //Set up navpoints from roads gameobject
        initializeNavPoints();

        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        agent.speed *= Random.Range(randomSpeedLowerLimit, randomSpeedUpperLimit);
        speedOriginal = agent.speed;

        layerMask = LayerMask.GetMask("Agent");

        GoToNextPoint(pointsRight);
    }

    void Update()
    {
        
        // Pass by cars (Go to left lane if too close to other agent in front of agent)
        if (!left && destPoint > 1 && !goingToRight)
        {
            Vector3 direction = new Vector3(0,0,1);
            raycast = Physics.BoxCast(transform.position, transform.localScale - new Vector3(0,1,0), direction, out m_Hit, transform.rotation, m_MaxDistance) && !GameObject.ReferenceEquals( transform.gameObject, m_Hit.transform.gameObject);
            
            speedCur = Vector3.Project(agent.desiredVelocity, transform.forward).magnitude;
            bool x = speedCur < agent.speed * breakPercBeforeLaneSwitch;
            if (x || raycast)
            {
                left = true;
                destPoint += 2;
                GoToNextPoint(pointsLeft);
                agent.speed = Mathf.Min(agent.speed * speedMultLeftLane, speedLimit);
                raycast = false;
            }
        }
        
        // If agent is on left lane, check if it should go back to right lane
        if(left)
        {
            handleLeftLane();
        }
        
        // Check if agent reached position, if so set next point
        if (!agent.pathPending && agent.remainingDistance < 1)
        {
            if (left)
                GoToNextPoint(pointsLeft);
            else
                GoToNextPoint(pointsRight);
        }
    }


    // START FUNCTIONS

    void initializeNavPoints()
    {
        foreach(Transform child in roadLeft.transform.Find("navpoints") )
        {
            pointsLeft.Add(child.transform);
        }

        foreach (Transform child in roadRight.transform.Find("navpoints"))
        {
            pointsRight.Add(child.transform);
        }

        totalPoints = Mathf.Min(pointsLeft.Count, pointsRight.Count);
    }

    void GoToNextPoint(List<Transform> points)
    {
        //Destroy gameobject at end of lane, or if no navpoints are found
        if (totalPoints == 0 || destPoint >= totalPoints)
        {
            Destroy(gameObject);
            return;
        }

        agent.destination = points[destPoint].position;
        destPoint++;
        goingToRight = false;
    }


    // UPDATE FUNCTIONS

    void handleLeftLane()
    {
        distance = 5.2f;
        Vector3 direction = new Vector3(1,0,0);
        Vector3 relativePos = new Vector3(3,0,0);
        // the second argument, upwards, defaults to Vector3.up
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        
        raycast2 = Physics.BoxCast(transform.position, transform.localScale + ray2size, direction, out m_Hit2, rotation, distance) && !GameObject.ReferenceEquals(transform.gameObject, m_Hit2.transform.gameObject);
        if(raycast2)
        {
            nextToCar = true;
        }
        else if(nextToCar)
        {
            notNextToCar += 1;
        }

        if (notNextToCar >= 9){
            destPoint += 3;
            notNextToCar = 0;
            goingToRight = true;
            left = false;
            agent.speed = Mathf.Min(agent.speed * speedMultLeftLane, speedLimit);
        }
    }

    //Draw the BoxCast as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    void OnDrawGizmos()
    {
        
        if(!left){
            drawGizmosRight();
        }
        else 
        {
            drawGizmosLeft();
        }
    }

    void drawGizmosRight()
    {
        //Check if there has been a hit yet
        if (raycast)
        {
            Gizmos.color = Color.red;
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(transform.position, transform.forward * m_Hit.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(transform.position + transform.forward * m_Hit.distance, transform.localScale  - new Vector3(0,1,0));
        }

        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else
        {
            Gizmos.color = Color.green;
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(transform.position, transform.forward * m_MaxDistance);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(transform.position + transform.forward * m_MaxDistance, transform.localScale  - new Vector3(0,1,0));
        }
    }

    void drawGizmosLeft()
    {
        Vector3 direction = new Vector3(1,0,0);
        if (raycast2){
            Gizmos.color = Color.red;
            // Draw a Ray to the right of the GameObject towards the maximum distance
            Gizmos.DrawRay(transform.position, direction * m_Hit2.distance);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(transform.position + direction * m_Hit2.distance, transform.localScale + ray2size);
            
        }
        else {
            Gizmos.color = Color.green;
            // Draw a Ray to the right of the GameObject toward the maximum distance
            Gizmos.DrawRay(transform.position, direction * distance);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(transform.position + direction * distance, transform.localScale + ray2size);
        }
    }
}
