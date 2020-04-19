using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour
{

    private NavMeshAgent PlayerNavMesh;

    private void Start() {

        PlayerNavMesh = GetComponent<NavMeshAgent>();

    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            MoveToCursor();       
        }
        
    }

    /// <summary>
    /// Uses a physics ray cast to detect where the mouse is placed on the screen. If the ray cast hits an object on the map, the players destination will be set,
    /// causing the object to move to that location.
    /// </summary>
    private void MoveToCursor() {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        bool hasHit = Physics.Raycast(ray, out hit);

        if (hasHit) {
            PlayerNavMesh.destination = hit.point;
        }

    }
}
