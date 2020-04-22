using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Core;
using System;

namespace RPG.Control {
    public class PlayerController : MonoBehaviour {

        Health health;

        private void Start() {

            health = GetComponent<Health>();
        }

        private void Update() {

            if (health.IsDead()) return;

            if (InteractWithCombat())return;
            if(InteractWithMovement())return;
        }

        /// <summary>
        /// Function that checks if the user is currently hovering over a combat target game object. If they are and are clicking, the player begins an attack at that target
        /// </summary>
        /// <returns> True or False if hovering over a Combat Target </returns>
        private bool InteractWithCombat() {

            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());

            foreach(RaycastHit hit in hits) {

                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                if (target == null) continue;

                if (!GetComponent<Fighter>().CanAttack(target.gameObject)) { //checks if character has health or is dead. Don't want to target dead characters.
                    continue;
                }

                if (Input.GetMouseButtonDown(0)) {
                    GetComponent<Fighter>().Attack(target.gameObject);                   
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Function that checks if the user is currently hovering over a place on the terrain that can be moved to. If they are and are clicking, the player will move to that point where the mouse is
        /// </summary>
        /// <returns> True or False if hovering an area where the player can move </returns>
        private bool InteractWithMovement() {

            Ray ray = GetMouseRay();
            RaycastHit hit;

            bool hasHit = Physics.Raycast(ray, out hit);

            if (hasHit) {
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) {
                    GetComponent<Mover>().StartMoveAction(hit.point);
                }
                return true;
            }
            return false;
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