using System;
using TMPro;
using UnityEngine;

// Class used to control spawning of enemies and updating score UI
public class PlaneManager : MonoBehaviour
{
    // Store prefab for enemy to instantiate later
    public GameObject enemyPrefab;
    
    // Store the text mesh pro UI element to update UI
    public TextMeshProUGUI TextMeshPro;

    // Store the spawn range for the enemies
    private float spawnRangeX;
    private float spawnRangeY;

    private Vector2 GenerateSpawnPosition()
    {
        // Generate random spawn position within the screen bounds
        float spawnX = UnityEngine.Random.Range(-spawnRangeX, spawnRangeX);
        float spawnY = UnityEngine.Random.Range(-spawnRangeY, spawnRangeY);
        return new Vector2(spawnX, spawnY);
    }

    // Method used to spawn enemies at random positions based on given number and screen bounds
    void SpawnEnemy(int numEnemies)
    {
        for (int i = 0; i < numEnemies; i++)
        {
            Vector2 spawnPosition = GenerateSpawnPosition();
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }

    // Method used to update the score UI with current game state
    public void UpdateScore()
    {
        TextMeshPro.text = "WAYPOINTS: (" + Enemy.waypointMode + ") HERO: Driver(" + PlayerMovement.driveMode + ") TouchedEnemy(" + Enemy.TouchedEnemy   
        + ") EGG OnScreen(" + Projectile.projectileCount + ") ENEMY: Count(" + Enemy.enemyCount + ")" + " Destroyed(" + Enemy.destroyed + ") Waypoints Visibility ("
        + WayPointManager.waypointVisibility + ")";
    }
    
    void Start()
    {
        // Initiailize screen bounds and spawn range to be 90% of the screen size
        Vector3 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        spawnRangeX = screenBounds.x * 0.9f; 
        spawnRangeY = screenBounds.y * 0.9f; 

        // Spawn initial enemies
        SpawnEnemy(Enemy.enemyCount);
    }

    // Update is called once per frame
    void Update()
    {
        // Spawn enemy when enemycount is less than 10
        if (Enemy.enemyCount < 10)
        {
            SpawnEnemy(1);
        }
        UpdateScore();
    }
}
