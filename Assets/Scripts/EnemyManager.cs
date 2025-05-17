using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    public List<Enemy> enemies = new List<Enemy>();
    public CameraManager cameraManager;
    private Enemy currentlyChasingEnemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        instance = this;
    }

    public void AddEnemy(Enemy enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }

    public void RemoveEnemy(Enemy enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }
    }

    public void FocusCameraOnChasingEnemy(Enemy enemy)
    {
        if (cameraManager != null)
        {
            cameraManager.FocusOnObject(enemy.gameObject);
        }
        currentlyChasingEnemy = enemy;
    }
    
    public void StopCameraFocusIfChaseEnded(Enemy enemy)
    {
        if (currentlyChasingEnemy == enemy && cameraManager != null)
        {
            cameraManager.DisableMonitoring();
            currentlyChasingEnemy = null;
        }
    }
}
