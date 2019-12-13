using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class DataCollection : MonoBehaviour
{
    public GameObject carSpawner;
    public GameObject simulator;
    
    void Start() {
        System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ".";

        System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;    
    }

    void Update()
    {  
        foreach (Transform car in carSpawner.transform)
        {
            StreamWriter w = new StreamWriter("data/car_data.csv", append: true);
            float speedCur = car.GetComponent<FollowRoad>().speedCur;
            int simulationNumber = simulator.GetComponent<Simulator>().simulationNumber;
            int ticker = simulator.GetComponent<Simulator>().ticker;
            w.WriteLine(string.Format("{0},{1},{2},{3}", simulationNumber, car.GetInstanceID(), ticker, speedCur.ToString("0.00")));
            w.Flush();
            w.Close();
        }
        
    }
}