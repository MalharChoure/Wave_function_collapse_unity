using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class SpawnManager : MonoBehaviour
{//harsh
    public int numberOfWaves = 3;

    [Tooltip("In Seconds")]
    public float timeBetweenWaves = 10;
    
    private int currentWave = 0;
    
    private float currentTime = 300f;
    
    private bool isTimerRunning = false;
    
    public chunk_script _generator;
    
    private Transform player;
    
    public int numberOfEnemiesPerWave = 15;

    public Transform enemyPrefab; 
    
    public List<GameObject> spawnPoints;
    
    private List<GameObject> enemies;
    
    bool isGeneratingEnemies = false;

    public int minEnemyDistance = 20;

    public int maxEnemyDistance = 30;
    void Start()
    {
        currentTime = timeBetweenWaves;
        isTimerRunning = true;
        enemies = new List<GameObject>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        //GenerateNewWave();
        Debug.Log("generate wave called");
    }

    // Update is called once per frame
    void Update()
    {
        if (currentWave == numberOfWaves)
            return;
        if(_generator.isGridGenerated)
        {
            if(isTimerRunning)
            {
                currentTime -= Time.deltaTime;
                if(currentTime<=0 && currentWave<numberOfWaves)
                {
                    isTimerRunning = false;

                    if (!isGeneratingEnemies)
                    {
                        GenerateNewWave();

                    }

                }
                
            }
        }
    }


    private void GenerateNewWave()
    {
        isGeneratingEnemies = true;
        currentTime = timeBetweenWaves;
        currentWave++;
        spawnPoints = new List<GameObject>();
        spawnPoints = _generator.GetSpawnPoints(player, numberOfEnemiesPerWave, minEnemyDistance,maxEnemyDistance);

        foreach (GameObject t in spawnPoints)
        {
            Transform enemy = Instantiate(enemyPrefab, t.transform.position, Quaternion.identity);
            enemy.gameObject.GetComponentInChildren<AIDestinationSetter>().target = player;
           enemies.Add(enemy.gameObject);
        }

        isGeneratingEnemies = false;
        isTimerRunning = true;
    }

    private void OnDrawGizmos()
    {

        foreach (GameObject t in spawnPoints)
        {
            //Transform enemy = Instantiate(enemyPrefab, t.transform.position, Quaternion.identity);
            //enemy.gameObject.GetComponent<AIDestinationSetter>().target = player;
            //enemies.Add(enemy.gameObject);
            Gizmos.color = Color.white;
            Gizmos.DrawCube(t.transform.position,Vector3.one);
        }
    }
}
