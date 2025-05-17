using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
// Class used to control whether or not a camera is going to focus on an enemy
// Class may also be used to later control enemy behaviors specific to enemies, later
public class EnemyManager : MonoBehaviour
{
    // Store the enemyManager instance for other classes to access
    public static EnemyManager instance;
    // Store the camera manager to be used for focusing on enemies
    public CameraManager cameraManager;
    // Store the enemy to be focused on
    private Enemy currentlyChasingEnemy;
    // Store the enemy text to be used for displaying the current enemy being chased
    public TextMeshProUGUI enemyText;
    private void Awake()
    {
        // Set instance to this instance of the enemy manager
        instance = this;
    }

    // Method used to focus camera on most recent enemy that is chasing player
    public void FocusCameraOnChasingEnemy(Enemy enemy)
    {
        // Check if the camera manager is not null and if the enemy is not null
        if (cameraManager != null)
        {
            // If cameraManager not null, then set the camera to focus on the enemy
            // And set enemy cam text to active
            enemyText.text = "Enemy Chase Cam: Active";
            cameraManager.FocusOnObject(enemy.gameObject);
        }
        // Set the currently chasing enemy to the enemy that is being chased
        currentlyChasingEnemy = enemy;
    }

    // Method used to unfocus the camera from the enemy
    public void StopCameraFocusIfChaseEnded(Enemy enemy)
    {
        // Check if the given enemy is same as currently chasing enemy and if 
        // CameraManager is not null
        if (currentlyChasingEnemy == enemy && cameraManager != null)
        {
            // If all true then set chase cam to off and disable monitoring
            // On enemy
            enemyText.text = "Enemy Chase Cam: Shutt Off";
            cameraManager.DisableMonitoring();
            currentlyChasingEnemy = null;
        }
    }
}
