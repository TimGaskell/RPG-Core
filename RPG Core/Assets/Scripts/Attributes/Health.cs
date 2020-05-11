using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using RPG.Core;
using RPG.Stats;
using UnityEngine.Events;

namespace RPG.Attributes {
    public class Health : MonoBehaviour, ISaveable {

        [SerializeField] float regenerationPercentage = 70;
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] UnityEvent onDie;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float> {

        }

        float health = -1f;

        bool isDead = false;

        private void Start() {

            if (health < 0) {
                health = GetComponent<BaseStats>().GetStat(Stat.Health);
            }
            
        }

        private void OnEnable() {
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
        }

        private void OnDisable() {
            GetComponent<BaseStats>().onLevelUp -= RegenerateHealth;
        }

        /// <summary>
        /// Reduces the health of this character. If health hits 0, character executes Die()
        /// </summary>
        /// <param name="damage"> How much damage the character will take </param>
        public void TakeDamage(GameObject instigator, float damage) {

            print(gameObject.name + " took damage: " + damage);

            health = Mathf.Max(health - damage, 0);
           
            if (health == 0) {
                onDie.Invoke();
                Die();
                AwardExpereince(instigator);
            }
            else {
                takeDamage.Invoke(damage);
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
        /// Gets the fraction of how much health a character has between 0 and 1.
        /// </summary>
        /// <returns> float fraction of health between 0 and 1 </returns>
        public float GetFraction() {
            return health / GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        /// <summary>
        /// Function that determines how much health the character has as a percentage of its max health
        /// </summary>
        /// <returns> Percentage out of 100 for how much health the character has</returns>
        public float GetPercentage() {

            return 100 * GetFraction();
        }

        /// <summary>
        /// Gets the amount of health points the character currently has
        /// </summary>
        /// <returns> Current health points </returns>
        public float GetHealthPoints() {
            return health;
        }

        /// <summary>
        /// Gets the maximum health points of the character
        /// </summary>
        /// <returns> Maximum health points of character </returns>
        public float GetMaxHealthPoints() {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public void Heal(float HealthRestoreAmount) {

            health = Mathf.Min(health + HealthRestoreAmount, GetMaxHealthPoints());

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

        /// <summary>
        /// Regenerates a percentage of the maximum amount of health of the character
        /// </summary>
        private void RegenerateHealth() {

            float regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * (regenerationPercentage / 100);
            health = Mathf.Max(health, regenHealthPoints);
        }
    }
}
