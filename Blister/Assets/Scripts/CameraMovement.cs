using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private const float TAU = 6.2831853071f;

    private float translationSpeedKeys = 2f;
    private float translationSpeedEdging = 4f;
    private float edgingMargin = 5f;

    private float zoomSpeed = 1f;
    private float zoomDistance = 10f;
    private const float ZOOM_DISTANCE_MIN = 5f;
    private const float ZOOM_DISTANCE_MAX = 100f;

    private float rotationSensitivityMouse = 5f;//0.1f;
    private float rotationSensitivityKeys = 0.5f;//0.1f;
    private float rotationYaw = 0f;
    private float rotationPitch = 0f;

    private void Start()
    {

    }

    private void Update()
    {
        //TRANSLATE MOUNT
        //Keys
        Vector3 forwardGround = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
        Vector3 rightGround = new Vector3(transform.right.x, 0f, transform.right.z).normalized;
        Vector3 vector = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) { vector += forwardGround; }
        if (Input.GetKey(KeyCode.A)) { vector -= rightGround; }
        if (Input.GetKey(KeyCode.S)) { vector -= forwardGround; }
        if (Input.GetKey(KeyCode.D)) { vector += rightGround; }
        vector = vector.normalized;
        transform.parent.position += vector * translationSpeedKeys * zoomDistance * Time.deltaTime;

        //Screen edging
        if (Input.mousePosition.x <= edgingMargin)
        {
            transform.parent.position -= rightGround * translationSpeedEdging * zoomDistance * Time.deltaTime;
        }
        else if (Input.mousePosition.x >= Screen.width - edgingMargin)
        {
            transform.parent.position += rightGround * translationSpeedEdging * zoomDistance * Time.deltaTime;
        }
        if (Input.mousePosition.y <= edgingMargin)
        {
            transform.parent.position -= forwardGround * translationSpeedEdging * zoomDistance * Time.deltaTime;
        }
        else if (Input.mousePosition.y >= Screen.height - edgingMargin)
        {
            transform.parent.position += forwardGround * translationSpeedEdging * zoomDistance * Time.deltaTime;
        }

        //DISTANCE TO MOUNT
        //Zoom
        if ((Input.mouseScrollDelta.y < 0) //|| Input.GetKey(KeyCode.Space))
            && zoomDistance < ZOOM_DISTANCE_MAX) {
            zoomDistance += zoomSpeed;
        }
        if ((Input.mouseScrollDelta.y > 0) //|| Input.GetKey(KeyCode.LeftControl))
            && zoomDistance > ZOOM_DISTANCE_MIN) {
            zoomDistance -= zoomSpeed;
        }

        //ORBIT (ROTATE AROUND) MOUNT
        //Middle mouse + drag
        if (Input.GetMouseButton(2)) {
            rotationYaw += Input.GetAxisRaw("Mouse X") * rotationSensitivityMouse;
            rotationPitch += Input.GetAxisRaw("Mouse Y") * rotationSensitivityMouse;
        }
        //Q & E || arrow keys
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow))
        {
            rotationYaw += rotationSensitivityKeys;
        }
        if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.RightArrow))
        {
            rotationYaw -= rotationSensitivityKeys;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rotationPitch += rotationSensitivityKeys;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            rotationPitch -= rotationSensitivityKeys;
        }

        //Clamping
        rotationYaw %= 360f;// TAU;
        rotationPitch %= 360f;// TAU;
        rotationPitch = Mathf.Clamp(rotationPitch, 2f, 89f);

        //Rotation
        Quaternion orbitRotation = Quaternion.Euler(
            rotationPitch,
            rotationYaw,
            0f
        );

        //Always orbit parent
        Vector3 offset = orbitRotation * -Vector3.forward;
        transform.position = transform.parent.position + (offset * zoomDistance);

        //Look at mount
        transform.rotation = Quaternion.LookRotation(transform.parent.position - transform.position);
    }
}