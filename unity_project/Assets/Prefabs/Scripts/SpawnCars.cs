using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCars : MonoBehaviour
{
    //public List<GameObject> prefabs;
    //public GameObject markings;
    public GameObject spawnedObject;
    public int maxCars;
    //public int direction;
    public int delay;
    private int time_no_car_spawned;
    public GameObject road_left, road_right;
    public float speedLimit;

    // Update is called once per frame
    void Update()
    {
        if (time_no_car_spawned >= delay && transform.childCount < maxCars)
        {
            GameObject go = Instantiate(spawnedObject);
            
            go.GetComponent<FollowRoad>().roadLeft = road_left;
            go.GetComponent<FollowRoad>().roadRight = road_right;
            go.GetComponent<FollowRoad>().speedLimit = speedLimit;
            go.transform.position = transform.position;
            go.transform.parent = transform;

            time_no_car_spawned = 0;
        }
    
        time_no_car_spawned += 1;
    }

    /*
    public GameObject getRandomPrefab()
    {
        var random = new System.Random();
        int index = random.Next(prefabs.Count);
        return prefabs[index];
    }
    */
}
