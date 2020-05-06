using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Resources;
using RPG.Control;

namespace RPG.Combat {

    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRayCastable {
        public CursorType GetCursorType() {
            return CursorType.Combat;
        }

        public bool HandRaycast(PlayerController callingController) {

            if (!callingController.GetComponent<Fighter>().CanAttack(gameObject)) { //checks if character has health or is dead. Don't want to target dead characters.
                return false;
            }

            if (Input.GetMouseButtonDown(0)) {
                callingController.GetComponent<Fighter>().Attack(gameObject);
            }
            
            return true;
        }
    }
}
