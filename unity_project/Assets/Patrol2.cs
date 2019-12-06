using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrol2 : MonoBehaviour
{
    public List<Transform> points;
    private int destPoint = 0;
    public float speed;
    public Vector3 destination;

    // Start is called before the first frame update
    void Start()
    {
        GotoNextPoint();
    }

    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (points.Count == 0)
            return;

        // Set the agent to go to the currently selected destination.
        destination = points[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Count;
    }


    void Update()
    {
        // Choose the next destination point when the agent gets
        // close to the current one.
        if (Vector3.Distance(transform.position, destination) < 0.5f) {
            GotoNextPoint();
        }
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, destination, step);
    }
}
