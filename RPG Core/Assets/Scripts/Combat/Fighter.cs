using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;

namespace RPG.Combat {
    public class Fighter : MonoBehaviour {

        [SerializeField] float WeaponRange = 2f;
        Transform target;

        Mover mover;

        private void Start() {

            mover = GetComponent<Mover>();

        }

        private void Update() {

            if (target == null) return;

            if (!GetIsInRange()) {
                mover.MoveTo(target.position);
            }
            else {
                mover.Stop();
            }
        }

        /// <summary>
        /// Check if character is in range depending on weapon
        /// </summary>
        /// <returns> True or False if the character is in range of the target assigned </returns>
        private bool GetIsInRange() {
            return Vector3.Distance(transform.position, target.position) < WeaponRange;
        }

        /// <summary>
        /// Sets the combat target
        /// </summary>
        /// <param name="CombatTarget"> Combat Target </param>
        public void Attack(CombatTarget CombatTarget) {

            target = CombatTarget.transform;

        }

        /// <summary>
        /// Sets the combat target to null
        /// </summary>
        public void Cancel() {

            target = null;
        }
    }

}