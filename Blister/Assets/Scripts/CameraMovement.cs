using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private float translationSpeed = 3f;
    private float zoomSpeed = 1f;
    //private float rotationSensitivity = 1f;
    //private float rotationYaw = 0f;
    //private float rotationPitch = 0f;



    private void Start()
    {

    }

    private void Update()
    {
        //TRANSLATE CAMERA POSITION
        Vector3 vector = Vector3.zero;

        //First-person flying movement
        //if (Input.GetKey(KeyCode.W)) { vector += transform.forward; }
        //if (Input.GetKey(KeyCode.A)) { vector -= transform.right; }
        //if (Input.GetKey(KeyCode.S)) { vector -= transform.forward; }
        //if (Input.GetKey(KeyCode.D)) { vector += transform.right; }
        //if (Input.GetKey(KeyCode.Space)) { vector += transform.up; }
        //if (Input.GetKey(KeyCode.LeftControl)) { vector -= transform.up; }

        //RTS movement
        if (Input.GetKey(KeyCode.W)) { vector += Vector3.forward; }
        if (Input.GetKey(KeyCode.A)) { vector -= Vector3.right; }
        if (Input.GetKey(KeyCode.S)) { vector -= Vector3.forward; }
        if (Input.GetKey(KeyCode.D)) { vector += Vector3.right; }
        
        vector = vector.normalized;

        transform.position += vector * translationSpeed * transform.position.y * Time.deltaTime;

        //RTS zoom
        if ((Input.mouseScrollDelta.y < 0) //|| Input.GetKey(KeyCode.Space))
            && transform.position.y < 100f) {
            transform.position += Vector3.up * zoomSpeed;
        }
        if ((Input.mouseScrollDelta.y > 0) //|| Input.GetKey(KeyCode.LeftControl))
            && transform.position.y > 5f) {
            transform.position -= Vector3.up * zoomSpeed;
        }

        ////ROTATE CAMERA VIEW ANGLE
        //float rotationInputX = Input.GetAxisRaw("Mouse X") * rotationSensitivity;
        //rotationYaw += rotationInputX;
        //float rotationInputY = -Input.GetAxisRaw("Mouse Y") * rotationSensitivity;
        //rotationPitch += rotationInputY;
        //
        //transform.rotation = Quaternion.Euler(
        //    rotationPitch,
        //    rotationYaw,
        //    0f
        //);
    }
}
