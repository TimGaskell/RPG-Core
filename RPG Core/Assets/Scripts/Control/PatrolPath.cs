using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Control {
    public class PatrolPath : MonoBehaviour {

        const float waypointGizmoRadius = 0.3f;

        /// <summary>
        /// Draws each way point and connection gizmo's.
        /// </summary>
        private void OnDrawGizmos() {
            for(int i = 0; i < transform.childCount ; i++) {
                
                int j = GetNextIndex(i);
                Gizmos.DrawSphere(GetWaypoint(i), waypointGizmoRadius);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));
            }
        }

        /// <summary>
        /// Gets the next way point in the set. Returns first way point if its reached the end
        /// </summary>
        /// <param name="i"></param>
        /// <returns> Index of the next way point </returns>
        public int GetNextIndex(int i) {
            
            if(i + 1 >= transform.childCount) {
                return 0;
            }
            else {
                return i + 1;
            }
           
        }

        /// <summary>
        /// Returns vector 3 position of a way point based on its index. 
        /// </summary>
        /// <param name="i"> Index of the way point as the child </param>
        /// <returns> Position of way point </returns>
        public Vector3 GetWaypoint(int i) {
            return transform.GetChild(i).position;
        }
    }
}

