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
    private int             destPoint = 0;
    private bool            left = false;
    private int             totalPoints = 0;
    private int             layerMask;
    private float           speedOriginal;
    private int             navPointsToLookAhead = 3;
    public  float           speedCur = 0f;
    public  float           speedLimit = 5.0f;
    public  float           randomSpeedLowerLimit = 0.5f;
    public  float           randomSpeedUpperLimit = 1.5f;
    public  float           speedMultLeftLane = 1.2f;
    public  float           breakPercBeforeLaneSwitch = 0.8f;
    public  bool            debug_showray = false;


    void Start()
    {
        //Set up navpoints from roads gameobject
        foreach(Transform child in roadLeft.transform.Find("navpoints") )
        {
            pointsLeft.Add(child.transform);
        }

        foreach (Transform child in roadRight.transform.Find("navpoints"))
        {
            pointsRight.Add(child.transform);
        }

        totalPoints = Mathf.Min(pointsLeft.Count, pointsRight.Count);

        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        agent.speed *= Random.Range(randomSpeedLowerLimit, randomSpeedUpperLimit);
        speedOriginal = agent.speed;

        layerMask = LayerMask.GetMask("Agent");

        GoToNextPoint(pointsRight);
    }

    void GoToNextPoint(List<Transform> points)
    {
        //Destroy gameobject at end of lane, or if no navpoints are found
        if (totalPoints == 0 || destPoint == totalPoints)
        {
            Destroy(gameObject);
            return;
        }

        agent.destination = points[destPoint].position;
        destPoint++;
    }

    void Update()
    {
        bool blocked = false;
        Vector3 rayOrigin = transform.position + new Vector3(0, 0.0f, 2.0f);
        speedCur = Vector3.Project(agent.desiredVelocity, transform.forward).magnitude;

        // Pass by cars
        if (!left && destPoint > 1)
        {
            if (speedCur < agent.speed * breakPercBeforeLaneSwitch || Physics.Raycast(rayOrigin, pointsRight[Mathf.Clamp(destPoint + navPointsToLookAhead, 0, totalPoints - 1)].position, 3, layerMask))
            {
                blocked = true;
                left = true;
                destPoint += 2;
                agent.destination = pointsLeft[destPoint].position;
                agent.speed = Mathf.Min(agent.speed * speedMultLeftLane, speedLimit); //speed increase left lane
            }
        }

        // Debug: show rays for laneswitching
        if( debug_showray)
            Debug.DrawLine(transform.position + new Vector3(0, 0.3f, 0.7f), pointsRight[Mathf.Clamp(destPoint + navPointsToLookAhead, 0, totalPoints - 1)].position, blocked ? Color.red : Color.green);


        // Check if agent reached position, if so set next point
        if (!agent.pathPending && agent.remainingDistance < 1f)
        {
            if (left)
                GoToNextPoint(pointsLeft);
            else
                GoToNextPoint(pointsRight);
        }
    }
}

