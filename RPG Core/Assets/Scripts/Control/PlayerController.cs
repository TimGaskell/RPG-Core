using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Resources;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control {
    public class PlayerController : MonoBehaviour {

        Health health;
        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavMeshProjectionDistance = 1f;

        [System.Serializable]
        struct CursorMapping {

            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;

        }

        private void Awake() {
            health = GetComponent<Health>();
        }


        private void Update() {

            if (InteractWithUI()) {
                SetCursor(CursorType.UI);
                return;
            }
            if (health.IsDead()) {
                SetCursor(CursorType.None);
                return;
            }

            if (InteractWithComponent()) return;

            if(InteractWithMovement())return;

            SetCursor(CursorType.None);
        }

        /// <summary>
        /// Main function for determining what the cursor is hovering over and if it can be interacted with. The object is deemed interactable if the object has an IRaycastable Interface implemented in it.
        /// This calls for the component to handle what happens on the ray cast and assigns the cursor to a different image. If there are no interactable objects it returns false.
        /// </summary>
        /// <returns> True or false if cursor is hovering interactable object </returns>
        private bool InteractWithComponent() {
            RaycastHit[] hits = RayCastAllSorted();
            foreach (RaycastHit hit in hits) {

                IRayCastable[] rayCastables = hit.transform.GetComponents<IRayCastable>();
                foreach(IRayCastable rayCastable in rayCastables) {
                    if (rayCastable.HandRaycast(this)) {
                        SetCursor(rayCastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Sorts raycast hits by the distance in the scene. Closest hits are first. 
        /// </summary>
        /// <returns> Array of sorted raycast hits by smallest distance to largest </returns>
        RaycastHit[] RayCastAllSorted() {

            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            float[] distances = new float[hits.Length];

           for(int i =0; i< distances.Length; i++) {

                distances[i] = hits[i].distance;
                
           }

            Array.Sort(distances, hits);
            return hits;
        }

        /// <summary>
        /// Determines if the cursor is currently hovered on a UI element.
        /// </summary>
        /// <returns> True or false if cursor over UI</returns>
        private bool InteractWithUI() {
            return EventSystem.current.IsPointerOverGameObject();
        }

        /// <summary>
        /// Sets the games cursor icon based on the enum type passed in.
        /// </summary>
        /// <param name="type"> Enum type of cursor </param>
        private void SetCursor(CursorType type) {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        /// <summary>
        /// Gets the correct cursor mapping struct based on the CursorType being looked for
        /// </summary>
        /// <param name="type"> Enum type of cursor</param>
        /// <returns> CursorMapping struct for specific cursor type</returns>
        private CursorMapping GetCursorMapping(CursorType type) {

            foreach(CursorMapping mapping in cursorMappings) {
                if(mapping.type == type) {
                    return mapping;
                }
            }
            return cursorMappings[0];

        }

        /// <summary>
        /// Function that checks if the user is currently hovering over a place on the terrain that can be moved to. If they are and are clicking, the player will move to that point where the mouse is
        /// </summary>
        /// <returns> True or False if hovering an area where the player can move </returns>
        private bool InteractWithMovement() {

            Vector3 target;
            bool hasHit = RayCastNavMesh(out target);

            if (hasHit) {
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) {
                    GetComponent<Mover>().StartMoveAction(target,1f);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Function determining if the mouse if currently hovered over a piece of terrain that has a baked nav mesh. It samples the ray cast hit and determines if it hits a nav mesh in a specified radius around the hit.
        /// </summary>
        /// <param name="target"> Vector 3 target location on terrain </param>
        /// <returns>True or false if mouse is hovered over navmesh </returns>
        private bool RayCastNavMesh(out Vector3 target) {

            target = new Vector3();

            Ray ray = GetMouseRay();
            RaycastHit hit;
            bool hasHit = Physics.Raycast(ray, out hit);

            if (!hasHit) return false;

            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(hit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);

            if (!hasCastToNavMesh) return false;

            target = navMeshHit.position;
            return true;

        }

        /// <summary>
        /// Produces a ray at the position of the mouse location
        /// </summary>
        /// <returns> Ray Location of mouse location </returns>
        private static Ray GetMouseRay() {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }

}