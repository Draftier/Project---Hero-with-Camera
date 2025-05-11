using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UI;

// Finally added comments to everything 5/9/2025
// Class used to control player movement and shooting 
public class PlayerMovement : MonoBehaviour
{
    // Store prefab for projectile to instantiate later
    public GameObject projectilePrefab;
    // Store whether or not player is using mouse or keyboard to control movement
    public bool mouseDrive;
    // Store the current drive mode as a string
    public static string driveMode = "Mouse";
    // Store whether or not the player is currently shooting
    public bool isShooting = false;
    // Store the speed of the projectile
    public float projectileSpeed = 0.2f;
    // Store the speed of the player
    private float speed = 13.0f;
    // Store the horizontal input from the player
    private float horizontalInput;
    // Store the rotation speed of the player
    private float rotationSpeed = 60.0f;
    // Store the waypoint manager to be used for setting waypoint visibility
    public WayPointManager waypointManager;
    // Store the image for the left cooldown bar and right cooldown bar
    public Image leftBar;
    public Image rightBar;

    // Store the current charge of the cooldown bar
    float charge = 0f;

    // Function to switch between mouse and keyboard drive modes
    private void switchDriveMode()
    {
        mouseDrive = !mouseDrive;
        driveMode = mouseDrive ? "Mouse" : "Key";
    }

    // Coroutine to handle firing projectiles every 0.2 seconds
    private IEnumerator FireProjectile()
    {
        // Boolean used to prevent two fireprojectile coroutines from running at the same time
        isShooting = true;

        // Instantiate a projectile and ignore collision with player when space is held down
        while (Input.GetKey(KeyCode.Space))
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
            Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), GetComponent<Collider2D>());

            // Set cooldown bar to full instantly when projectile is fired
            charge = 1f;
            leftBar.fillAmount = charge;
            rightBar.fillAmount = charge;
            yield return new WaitForSeconds(projectileSpeed);
        }
        // Boolean used to prevent two fireprojectile coroutines from running at the same time
        isShooting = false;
    }

    void Start()
    {
        // Initialize bar fill, starting speed, and movement type
        mouseDrive = true; 
        leftBar.fillAmount = 0;
        rightBar.fillAmount = 0;

        Vector3 forward = transform.up;
        transform.Translate(forward * speed * Time.deltaTime, Space.World);
    }

    void Update()
    {
        // Switch drive mode when M is pressed
        if (Input.GetKeyDown(KeyCode.M))
        {
            switchDriveMode();
        }

        // Fire projectile when space is pressed and player is not already shooting
        if (Input.GetKeyDown(KeyCode.Space) && !isShooting)
        {
            StartCoroutine(FireProjectile());
            
        }

        // If the player is not shooting and the charge is greater than 0, decrease the cooldown bar fill amount
        // Done outside of coroutine due to bugs with coroutines running at the same time
        if (isShooting && charge > 0)
        {
            // Cooldown bar goes down in between shots
            charge -= Time.deltaTime / projectileSpeed;
            charge = Mathf.Clamp01(charge);

            leftBar.fillAmount = charge;
            rightBar.fillAmount = charge;
        }
        else if (!isShooting && charge > 0)
        {
            // Cooldown bar goes down when not shooting
            charge -= Time.deltaTime;
            charge = Mathf.Clamp01(charge);

            leftBar.fillAmount = charge;
            rightBar.fillAmount = charge;
        }

        // Quit the game when Q is pressed in build and in editor
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }

        // Toggle whether or not planes will visit waypoints sequentially
        if (Input.GetKeyDown(KeyCode.J))
        {
            Enemy.sequential = !Enemy.sequential;
        }

        // Toggle whether or not waypoints are visible
        if (Input.GetKeyDown(KeyCode.H))
        {
            waypointManager.HideWayPoints();
        }

        // Initialize starting movement of the player
        horizontalInput = Input.GetAxis("Horizontal");
        transform.Rotate(0, 0, -(horizontalInput * rotationSpeed * Time.deltaTime));
        Vector3 forward = transform.up;
        if (mouseDrive)
        {
            // If the player is using mouse drive, set the speed to 0 and move towards the mouse position
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePosition;
        }
        else
        {
            // Accelerate "up" when the up arrow is pressed
            if(Input.GetKey(KeyCode.UpArrow))
            {
                speed += 50f * Time.deltaTime;
                speed = Mathf.Clamp(speed, 0f, 200f);
            }
            // Decelerate "down" when the down arrow is pressed
            else if(Input.GetKey(KeyCode.DownArrow))
            {
                speed -= 40 * Time.deltaTime;
                speed = Mathf.Clamp(speed, -80f, 200f);
            }
            // Commented out as player doesn't decelerate to 0 in sample because in space?
            // else
            // {
            //     // If the player is not pressing up or down, decelerate to 0 unless initial movement not interrupted
            //     // I think because that's what it was like in sample webgl
            //     if(speed > 0 && !initialMovement)
            //     {
            //         speed-= 10f * Time.deltaTime;
            //         speed = Mathf.Clamp(speed, 0f, 200f);
            //     }
            //     else if(speed < 0 && !initialMovement)
            //     {
            //         speed += 10f * Time.deltaTime;
            //         speed = Mathf.Clamp(speed, -80f, 200f);
            //     }
            // }
        }
        // Move the player forward in the direction they are facing
        transform.Translate(forward * speed * Time.deltaTime, Space.World);
    }
}
