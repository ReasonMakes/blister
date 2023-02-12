using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    //References
    public Rigidbody rb;

    //Brain
    public enum Order
    {
        idle,
        move,
        attack,
        holdPosition,
        patrol
    };
    [System.NonSerialized] public Order orders = Order.idle;
    [System.NonSerialized] public Vector3 rallyPoint = Vector3.zero;

    //Physical properties
    [System.NonSerialized] public float speed = 7f;

    private void Update()
    {
        //Receive orders
        if (GetComponent<SelectionComponent>() != null)
        {
            //Probably a better way to do this so that EVERY unit doesn't have to run these checks and calculate rally points
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 50000.0f, LayerMask.GetMask("Ground")))
                {
                    //Always rally to a position on the ground
                    rallyPoint = new Vector3(hit.point.x, 0.5f, hit.point.z);

                    //Move there
                    orders = Order.move;
                }
            }
        }

        //Reduce speed when idle
        float currentSpeed = (orders == Order.idle) ? speed * 0.25f : speed;

        Vector3 moveVector = Vector3.zero;

        //Avoid other units
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            Vector3 vectorAwayFromOtherUnit = transform.position - transform.parent.GetChild(i).position;
            float distanceToOtherUnit = vectorAwayFromOtherUnit.magnitude;
            float thresholdToBoidBehavior = 4f;
            float strengthOfBoidBehavior = 0.75f;
            if (distanceToOtherUnit < thresholdToBoidBehavior)
            {
                moveVector += vectorAwayFromOtherUnit.normalized * (((thresholdToBoidBehavior - distanceToOtherUnit) / thresholdToBoidBehavior) * currentSpeed * strengthOfBoidBehavior);
            }
        }

        //Execute orders
        float rallyPointThreshold = 4f;
        if (
            orders == Order.move
            || orders == Order.attack
            || (
                orders == Order.idle
                && (rallyPoint - rb.position).magnitude > rallyPointThreshold
            )
        )
        {
            //Incorporate rally point into movement vector
            moveVector += (rallyPoint - rb.position).normalized * currentSpeed;

            //Go idle once arrived at rally point
            float distanceToRallyPoint = (rallyPoint - rb.position).magnitude;
            if (distanceToRallyPoint <= (currentSpeed * Time.deltaTime) || distanceToRallyPoint <= 0.09f)
            {
                //rb.position = rallyPoint;
                //rb.velocity = Vector3.zero;
                orders = Order.idle;
            }
        }

        //Move
        rb.velocity = moveVector.normalized * currentSpeed;

        //Always on the ground
        rb.transform.position = new Vector3(rb.transform.position.x, 0.5f, rb.transform.position.z);
    }
}