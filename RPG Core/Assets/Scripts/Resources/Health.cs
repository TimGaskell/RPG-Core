using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using RPG.Core;
using RPG.Stats;

namespace RPG.Resources {
    public class Health : MonoBehaviour, ISaveable {

        float health = -1f;

        bool isDead = false;

        private void Start() {

            if (health < 0) {
                health = GetComponent<BaseStats>().GetStat(Stat.Health);
            }
        }

        /// <summary>
        /// Reduces the health of this character. If health hits 0, character executes Die()
        /// </summary>
        /// <param name="damage"> How much damage the character will take </param>
        public void TakeDamage(GameObject instigator, float damage) {

            health = Mathf.Max(health - damage, 0);
            if(health == 0) {
                Die();
                AwardExpereince(instigator);
            }

        }

        /// <summary>
        /// Gives experience points to the character that last did damage to them. Gives as much experience based upon the experience reward set in the base stats
        /// </summary>
        /// <param name="instigator"> GameObject that attacked this character</param>
        public void AwardExpereince(GameObject instigator) {

            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) return;

            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        /// <summary>
        /// Function that determines how much health the character has as a percentage of its max health
        /// </summary>
        /// <returns> Percentage out of 100 for how much health the character has</returns>
        public float GetPercentage() {

            return 100 *(health / GetComponent<BaseStats>().GetStat(Stat.Health));
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

        /// <summary>
        /// Saves the amount of health the character has
        /// </summary>
        /// <returns> Health of character </returns>
        public object CaptureState() {
            return health;
        }

        /// <summary>
        /// Restores the amount of health a character has after loading. Makes sure they are dead if health is 0.
        /// </summary>
        /// <param name="state"> object data being passed through </param>
        public void RestoreState(object state) {

            health = (float)state;
            if (health == 0) {
                Die();
            }
        }
    }
}
