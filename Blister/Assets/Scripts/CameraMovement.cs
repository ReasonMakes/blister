using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private const float TAU = 6.2831853071f;

    private float translationSpeed = 3f;
    private float zoomSpeed = 1f;
    private float translationSpeedEdging = 6f;
    private float screenEdgeMargin = 5f;
    private float rotationSensitivity = 0.1f;
    private float rotationYaw = 0f;
    private float rotationPitch = 0f;
    //private float rotationSensitivity = 1f;
    //private float rotationYaw = 0f;
    //private float rotationPitch = 0f;



    private void Start()
    {

    }

    private void Update()
    {
        //TRANSLATE CAMERA POSITION
        //Keys
        Vector3 forwardFloating = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
        Vector3 rightFloating = new Vector3(transform.right.x, 0f, transform.right.z).normalized;
        Vector3 vector = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) { vector += forwardFloating; }
        if (Input.GetKey(KeyCode.A)) { vector -= rightFloating; }
        if (Input.GetKey(KeyCode.S)) { vector -= forwardFloating; }
        if (Input.GetKey(KeyCode.D)) { vector += rightFloating; }
        vector = vector.normalized;
        transform.position += vector * translationSpeed * transform.position.y * Time.deltaTime;

        //Screen edging
        if (Input.mousePosition.x <= screenEdgeMargin)
        {
            transform.position -= rightFloating * translationSpeedEdging * transform.position.y * Time.deltaTime;
        }
        else if (Input.mousePosition.x >= Screen.width - screenEdgeMargin)
        {
            transform.position += rightFloating * translationSpeedEdging * transform.position.y * Time.deltaTime;
        }
        if (Input.mousePosition.y <= screenEdgeMargin)
        {
            transform.position -= forwardFloating * translationSpeedEdging * transform.position.y * Time.deltaTime;
        }
        else if (Input.mousePosition.y >= Screen.height - screenEdgeMargin)
        {
            transform.position += forwardFloating * translationSpeedEdging * transform.position.y * Time.deltaTime;
        }

        //Zoom
        if ((Input.mouseScrollDelta.y < 0) //|| Input.GetKey(KeyCode.Space))
            && transform.position.y < 100f) {
            transform.position += Vector3.up * zoomSpeed;
        }
        if ((Input.mouseScrollDelta.y > 0) //|| Input.GetKey(KeyCode.LeftControl))
            && transform.position.y > 5f) {
            transform.position -= Vector3.up * zoomSpeed;
        }

        //ROTATE CAMERA VIEW ANGLE
        //Specified rotation point
        if (Input.GetMouseButton(2)) {
            //Get pivot point
            //Vector3 grabbedVector3 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1.0f);
            Vector3 grabbedVector3 = new Vector3(Screen.width/2f, Screen.height/2f, 1.0f);
            Vector3 screenPointToWorldPoint = Camera.main.ScreenToWorldPoint(grabbedVector3);
            Vector3 cameraToGrabbed = (Camera.main.ScreenToWorldPoint(grabbedVector3) - Camera.main.transform.position).normalized;
            Ray ray = new Ray(screenPointToWorldPoint, cameraToGrabbed);
            if (Physics.Raycast(ray, out RaycastHit hit, 5000.0f, LayerMask.GetMask("Ground")))
            {
                //Debug ray path
                Debug.DrawLine(screenPointToWorldPoint, hit.point, Color.blue, 1.0f);

                //YAW
                //Get properties before rotation
                Vector3 localRotationEulerBefore = transform.localRotation.eulerAngles;

                //Orbit
                rotationYaw -= Input.GetAxisRaw("Mouse X") * rotationSensitivity;
                rotationYaw %= TAU;
                Vector3 floatingHitPoint = new Vector3(hit.point.x, Camera.main.transform.position.y, hit.point.z);
                float distanceToFloatingPivotPoint = (Camera.main.transform.position - floatingHitPoint).magnitude;
                transform.position = floatingHitPoint + new Vector3(
                    Mathf.Cos(rotationYaw) * distanceToFloatingPivotPoint,
                    0f,
                    Mathf.Sin(rotationYaw) * distanceToFloatingPivotPoint
                );

                //Look at pivot point
                Quaternion rotationToPivotPoint = Quaternion.LookRotation(hit.point - transform.position, Vector3.up);
                if (rotationToPivotPoint != Quaternion.identity)
                {
                    //Look at pivot point
                    transform.localRotation = rotationToPivotPoint;

                    //Preserve pitch
                    Vector3 localRotationEuler = transform.localRotation.eulerAngles;
                    transform.localRotation = Quaternion.Euler(
                        localRotationEulerBefore.x, //pitch
                        localRotationEuler.y,
                        localRotationEuler.z
                    );
                }

                ////PITCH
                ////Get properties before rotation
                //localRotationEulerBefore = transform.localRotation.eulerAngles;
                //
                ////Orbit
                //rotationPitch += Input.GetAxisRaw("Mouse Y") * rotationSensitivity;
                //rotationPitch %= TAU;
                //Vector3 groundCamera = new Vector3(Camera.main.transform.position.x, 0f, Camera.main.transform.position.z);
                //Vector3 groundHitPoint = new Vector3(hit.point.x, 0f, hit.point.z);
                //float distanceToGroundPivotPoint = (groundCamera - groundHitPoint).magnitude;
                //transform.position = groundHitPoint + new Vector3(
                //    Mathf.Cos(rotationYaw) * distanceToFloatingPivotPoint,
                //    Mathf.Sin(rotationPitch) * distanceToGroundPivotPoint,
                //    Mathf.Sin(rotationYaw) * distanceToFloatingPivotPoint
                //);
                //
                ////Look at pivot point
                //rotationToPivotPoint = Quaternion.LookRotation(hit.point - transform.position, Vector3.up);
                //if (rotationToPivotPoint != Quaternion.identity)
                //{
                //    //Look at pivot point
                //    transform.localRotation = rotationToPivotPoint;
                //
                //    //Preserve pitch
                //    Vector3 localRotationEuler = transform.localRotation.eulerAngles;
                //    transform.localRotation = Quaternion.Euler(
                //        localRotationEuler.x, //pitch
                //        localRotationEulerBefore.y,
                //        localRotationEulerBefore.z
                //    );
                //}
            }
        }
        
        ////First-person rotation
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