using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;


public class Simulator : MonoBehaviour
{
    public  GameObject                  dataCollector;
    public  GameObject                  carSpawner;
    public  int                         ticker = 0;
    public  int                         simulationDuration = 500;
    public  int                         simulationNumber = 0;
    public  int                         combinationNumber = 0;
    private List<(float, int, int)>     allCombinations;
    private List<float>                 maxSpeedOptions = new List<float>() {3.5f, 4.0f, 4.5f, 5.0f};
    private List<int>                   spawnRateOptions = new List<int>() {60, 80, 100, 120}; 
    private List<int>                   maxCarsOptions = new List<int>() {10, 20, 40, 60}; 

    // Start is called before the first frame update
    void Start()
    {
        // Set decimal style to "." instead of "," to prevent csv issues.
        System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo) System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ".";

        System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

        allCombinations = getAllCombinations();
        startSimulation();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if simulation is over, if so go to next simulation with another combination.
        ticker++;
        if (ticker >= simulationDuration){
            endSimulation();
            if (simulationNumber >= (allCombinations.Count * 3 - 1)){
                Debug.Break();
            }
            
            if (combinationNumber >= allCombinations.Count - 1){
                combinationNumber = 0;
            }
            
            ticker = 0;
            combinationNumber +=1;
            simulationNumber += 1;

            startSimulation();
        }


    }


    // Function to get all combinations (max speed, spawn rate, max cars)
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


    // Function to start simulation by setting up new parameters and saving them to the csv file.
    void startSimulation()
    {
        (float, int, int) newCombination = allCombinations[combinationNumber];

        carSpawner.GetComponent<SpawnCars>().speedLimit = newCombination.Item1;
        carSpawner.GetComponent<SpawnCars>().delay = newCombination.Item2;
        carSpawner.GetComponent<SpawnCars>().maxCars = newCombination.Item3;

        saveSimulationData();
    }

    // Function to end simulation by destroying all objects.
    void endSimulation(){
        Debug.Log("Ending");
        foreach (Transform car in carSpawner.transform)
        {
            Destroy(car.gameObject);
        }
    }

    // Write simulation data to csv-file.
    void saveSimulationData()
    {
        StreamWriter w = new StreamWriter("data/simulation_data.csv", append: true);

        w.WriteLine(string.Format("{0},{1},{2},{3},{4}", simulationNumber, combinationNumber, carSpawner.GetComponent<SpawnCars>().speedLimit.ToString("0.00"), carSpawner.GetComponent<SpawnCars>().delay, carSpawner.GetComponent<SpawnCars>().maxCars));
        w.Flush();
        w.Close();
    }
}
