using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCars : MonoBehaviour
{
    public List<GameObject> prefabs;
    public List<GameObject> markings;
    public int maxCars;
    public int direction;
    public int delay;
    private int time_no_car_spawned;

    // Update is called once per frame
    void Update()
    {
        if (time_no_car_spawned >= delay && transform.childCount < maxCars){
            GameObject go = Instantiate(getRandomPrefab());
            foreach (gameObject marking in markings)
            {
                go.GetComponent.<Patrol2>().points.Add(marking.transform);
            }
            
            go.transform.position = transform.position;
            go.transform.parent = transform;

            time_no_car_spawned = 0;
        }
    
        time_no_car_spawned += 1;
    }


    public GameObject getRandomPrefab(){
        var random = new System.Random();
        int index = random.Next(prefabs.Count);
        return prefabs[index];
    }
}
