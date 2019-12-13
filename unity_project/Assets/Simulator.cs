using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;


public class Simulator : MonoBehaviour
{
    public GameObject dataCollector;
    public GameObject carSpawner;
    public int ticker = 0;
    public int simulationDuration = 500;
    public int simulationNumber = 0;

    private List<(float, int, int)> allCombinations;
    private List<float> maxSpeedOptions = new List<float>() {3.5f, 4.0f, 4.5f, 5.0f};
    private List<int> spawnRateOptions = new List<int>() {60, 80, 100, 120}; 
    private List<int> maxCarsOptions = new List<int>() {10, 20, 40, 60}; 

    // Start is called before the first frame update
    void Start()
    {
        System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo) System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ".";

        System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

        allCombinations = getAllCombinations();
        startSimulation();
    }

    // Update is called once per frame
    void Update()
    {
        ticker++;
        if (ticker >= simulationDuration){
            endSimulation();
            if (simulationNumber == (allCombinations.Count - 1)){
                Debug.Break();
            }
            
            ticker = 0;
            simulationNumber += 1;

            startSimulation();
        }


    }

    List<(float, int, int)> getAllCombinations(){
        List<(float, int, int)> allCombinations = new List<(float, int, int)>();

        foreach (float speed in maxSpeedOptions)
        {
            foreach (int rate in spawnRateOptions)
            {
                foreach (int cars in maxCarsOptions)
                {
                    allCombinations.Add((speed, rate, cars));
                }
            }
        }

        return allCombinations;
    }

    void startSimulation()
    {
        (float, int, int) newCombination = allCombinations[simulationNumber];

        carSpawner.GetComponent<SpawnCars>().speedLimit = newCombination.Item1;
        carSpawner.GetComponent<SpawnCars>().delay = newCombination.Item2;
        carSpawner.GetComponent<SpawnCars>().maxCars = newCombination.Item3;

        saveSimulationData();
    }

    void endSimulation(){
        Debug.Log("Ending");
        foreach (Transform car in carSpawner.transform)
        {
            Destroy(car.gameObject);
        }
    }

    void saveSimulationData()
    {
        StreamWriter w = new StreamWriter("data/simulation_data.csv", append: true);

        w.WriteLine(string.Format("{0},{1},{2},{3}", simulationNumber, carSpawner.GetComponent<SpawnCars>().speedLimit.ToString("0.00"), carSpawner.GetComponent<SpawnCars>().delay, carSpawner.GetComponent<SpawnCars>().maxCars));
        w.Flush();
        w.Close();
    }
}
