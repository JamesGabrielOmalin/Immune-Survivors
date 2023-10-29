using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WaypointIndicator : MonoBehaviour
{
    private Coroutine trackingCoroutine;

    public Transform target; // This is the object we want to point to
    [SerializeField] RectTransform waypointImage;
    private Camera cam;
    [HideInInspector]
    public float screenOffset;

    void Start()
    {
        cam = Camera.main;
        waypointImage = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }

    public void TrackUnit(GameObject unit)
    {
        target = unit.transform;
        //trackingCoroutine = StartCoroutine(UpdateIndicator());

    }

    public void UntrackUnit()
    {
        //StopCoroutine(trackingCoroutine);
        target = null ;

    }

    void Update()
    {
       
    }


    public void UpdateIndicator()
    {
        if (target == null)
        {
            Debug.LogError("No target");
            return;
        }
        if (cam == null)
        {
            cam = Camera.main;

        }
        Vector3 screenPos = cam.WorldToScreenPoint(target.position);

        // Check if the target is behind the camera
        if (screenPos.z < 0)
        {
            screenPos *= -1; // invert the position
        }

        // Check if the target is off screen
        if (!IsInScreen(screenPos))
        {
            waypointImage.GetComponent<Image>().enabled = true;

            // Clamp the position to the screen bounds
            screenPos = ClampPosition(screenPos);
        }
        else
        {
            waypointImage.GetComponent<Image>().enabled = false;
        }

        // Calculate the direction from the center of the screen to the target position
        Vector3 direction = screenPos - (new Vector3(Screen.width, Screen.height, screenPos.z) / 2);

        // Calculate the angle between the target and the center of the screen
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate the waypoint image to point towards the target
        waypointImage.rotation = Quaternion.Euler(0, 0, angle);

        // Assign the calculated position to the waypoint image
        waypointImage.position = screenPos;
        
        
      
    }
    bool IsInScreen(Vector3 screenPosition)
    {
        return screenPosition.x > 0 && screenPosition.x < Screen.width && screenPosition.y > 0 && screenPosition.y < Screen.height;
    }

    Vector3 ClampPosition(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, screenOffset, Screen.width - screenOffset);
        position.y = Mathf.Clamp(position.y, screenOffset, Screen.height - screenOffset);

        return position;
    }
}
