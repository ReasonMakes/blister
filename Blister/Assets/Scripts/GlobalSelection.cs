using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Sometimes you cannot reselect a unit you just deselected
//Canvas seems to be blocking raycast in world space
//Units can move into where box select WAS

public class GlobalSelection : MonoBehaviour
{
    [System.NonSerialized] public SelectedDictionary selectedTable;

    bool dragSelect;

    Vector3[] cubeVertices = new Vector3[8];
    public SelectionCube selectionCube;
    public GameObject defaultUnit;

    Vector3 mouseSelectPositionStart;
    Vector3 mouseSelectPositionEnd;

    //private bool debug = false;
    //private int debugIndex = 0;
    //private float debugTime = 0f;
    //private float debugPeriod = 1f;

    // Start is called before the first frame update
    void Start()
    {
        selectedTable = GetComponent<SelectedDictionary>();
        dragSelect = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug vertices
        //if (debug && Time.time > debugTime)
        //{
        //    Instantiate(defaultUnit, cubeVertices[debugIndex], Quaternion.identity);
        //
        //    //Iterate
        //    debugIndex++;
        //    debugTime = Time.time + debugPeriod;
        //
        //    //Break
        //    if (debugIndex >= 7)
        //    {
        //        debugIndex = 0;
        //        debug = false;
        //    }
        //}



        //1. when left mouse button clicked (but not released)
        if (Input.GetMouseButtonDown(0))
        {
            //Get mouse start position
            mouseSelectPositionStart = Input.mousePosition;
        }

        //2. while left mouse button held
        if (Input.GetMouseButton(0))
        {
            if ((mouseSelectPositionStart - Input.mousePosition).magnitude > 40)
            {
                dragSelect = true;
            }
        }

        //3. when mouse button comes up
        if (Input.GetMouseButtonUp(0))
        {
            if (dragSelect == false) //single select
            {
                Ray ray = Camera.main.ScreenPointToRay(mouseSelectPositionStart);

                if (Physics.Raycast(ray, out RaycastHit hit, 50000.0f, LayerMask.GetMask("Unit")))
                {
                    if (Input.GetKey(KeyCode.LeftShift)) //inclusive select
                    {
                        selectedTable.addSelected(hit.transform.gameObject);
                    }
                    else if (Input.GetKey(KeyCode.LeftControl)) //deselect one
                    {
                        selectedTable.removeSelected(hit.transform.gameObject);
                    }
                    else //exclusive selected
                    {
                        selectedTable.deselectAll();
                        selectedTable.addSelected(hit.transform.gameObject);
                    }
                }
                else //if no unit hit with raycast, deselect all UNLESS modifying selection with shift or control
                {
                    if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl))
                    {
                        selectedTable.deselectAll();
                    }
                }
            }
            else //Box select
            {
                //Get positions
                mouseSelectPositionEnd = Input.mousePosition;

                Vector2[] corners = getBoundingBox(mouseSelectPositionStart, mouseSelectPositionEnd);

                //Raycast to get the 3D bounding box from the 2D selection
                int i = 0;
                foreach (Vector2 corner in corners)
                {
                    //Define vectors
                    Vector3 cornerVector3 = new Vector3(corner.x, corner.y, 1.0f);
                    Vector3 screenPointToWorldPoint = Camera.main.ScreenToWorldPoint(cornerVector3);
                    Vector3 cameraToCorner = (Camera.main.ScreenToWorldPoint(cornerVector3) - Camera.main.transform.position).normalized;

                    //Raycast
                    Ray ray = new Ray(screenPointToWorldPoint, cameraToCorner);
                    if (Physics.Raycast(ray, out RaycastHit hit, 5000.0f, LayerMask.GetMask("Ground")))
                    {
                        //Debug ray path
                        Debug.DrawLine(screenPointToWorldPoint, hit.point, Color.green, 1.0f);
                    }

                    //Top of cube
                    cubeVertices[i + 4] = screenPointToWorldPoint;
                    //Bottom of cube
                    cubeVertices[i] = hit.point;

                    //Generates in this order:
                    //Bottom of cube first
                    //Top-left, top-right, bottom-left, bottom-right
                    //Then top of cube, following same pattern

                    //Iterate index
                    i++;
                }

                //Debug vertices
                //debugIndex = 0;
                //debug = true;
                //for (int j = 0; j <= 7; j++)
                //{
                //    Instantiate(defaultUnit, cubeVertices[j], Quaternion.identity);
                //}

                //Set the selection mode
                selectionCube.additiveSelection = true;
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    selectionCube.additiveSelection = false;
                }
                else if (!Input.GetKey(KeyCode.LeftShift))
                {
                    selectedTable.deselectAll();
                }

                //Generate the 3D bounding box mesh and select units within it
                selectionCube.CreateCube(cubeVertices);

                //End box select
                dragSelect = false;
            }
        }
    }

    //create a bounding box (4 corners in order) from the start and end mouse position
    private Vector2[] getBoundingBox(Vector2 p1, Vector2 p2)
    {
        // Min and Max to get 2 corners of rectangle regardless of drag direction.
        var bottomLeft = Vector3.Min(p1, p2);
        var topRight = Vector3.Max(p1, p2);

        // 0 = top left; 1 = top right; 2 = bottom left; 3 = bottom right;
        Vector2[] corners =
        {
            new Vector2(bottomLeft.x, topRight.y),
            new Vector2(topRight.x, topRight.y),
            new Vector2(bottomLeft.x, bottomLeft.y),
            new Vector2(topRight.x, bottomLeft.y)
        };

        return corners;
    }

    private void OnGUI()
    {
        if (dragSelect == true)
        {
            var rect = Utils.GetScreenRect(mouseSelectPositionStart, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        selectedTable.addSelected(other.gameObject);
    }

}