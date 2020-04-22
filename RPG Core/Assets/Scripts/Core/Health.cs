using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core {
    public class Health : MonoBehaviour {

        [SerializeField] float health = 100f;

        bool isDead = false;
       
        /// <summary>
        /// Reduces the health of this character. If health hits 0, character executes Die()
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(float damage) {

            health = Mathf.Max(health - damage, 0);
            if(health == 0) {
                Die();
            }

        }

        /// <summary>
        /// Sets isDead to True and sets trigger for death animation to play. Prevents animation playing if already dead.
        /// </summary>
        private void Die() {
            if (isDead) return;

            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        /// <summary>
        /// Getter method for returning whether the character is alive or not
        /// </summary>
        /// <returns> True or false if character is currently dead </returns>
        public bool IsDead() {
            return isDead;
        }
    }
}
