using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;


namespace RPG.Stats {
    public class Experience : MonoBehaviour, ISaveable {

        [SerializeField] float experiencePoints = 0;

        /// <summary>
        /// Adds experience onto the total for the character
        /// </summary>
        /// <param name="experience"> Amount of experience gained</param>
        public void GainExperience(float experience) {

            experiencePoints += experience;
        }

        /// <summary>
        /// Get function for returning the amount of experience points the character has
        /// </summary>
        /// <returns> Characters experience points</returns>
        public float GetPoints() {
            return experiencePoints;
        }

        /// <summary>
        /// Saves the experience points for the character
        /// </summary>
        /// <returns> experience points </returns>
        public object CaptureState() {
            return experiencePoints;
        }

        /// <summary>
        /// Restores the experience points from a previous save
        /// </summary>
        /// <param name="state"> Experience points as an object type</param>
        public void RestoreState(object state) {

            experiencePoints = (float)state;
        }
    }

}