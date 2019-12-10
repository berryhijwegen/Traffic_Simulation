using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class followroad : MonoBehaviour
{
    public GameObject road_left, road_right;
    private NavMeshAgent agent;
    public List<Transform> points_left;
    public List<Transform> points_right;
    private int destPoint = 0;
    private bool left = false;
    private int total_points;

    void Start()
    {
        foreach(Transform child in road_left.transform.Find("navpoints") )
        {
            points_left.Add(child.transform);
        }

        foreach (Transform child in road_right.transform.Find("navpoints"))
        {
            points_right.Add(child.transform);
        }

        total_points = Mathf.Min(points_left.Count, points_right.Count);

        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;

        GoToNextPoint(points_right);
    }

    void GoToNextPoint(List<Transform>points)
    {
        if (total_points == 0)
            return;

        agent.destination = points[destPoint].position;

        destPoint = (destPoint + 1) % points.Count;
    }

    void Update()
    {
        //Lane switching
        left = Random.Range(0, 10) < 9;   
        

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            if (left)
                GoToNextPoint(points_left);
            else
                GoToNextPoint(points_right);
        }
    }
}

