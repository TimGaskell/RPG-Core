using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Stats {
    public class BaseStats : MonoBehaviour {

        [Range(1,99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpParticleEffect = null;

        public event Action onLevelUp;

        int currentLevel = 0;

        private void Start() {

            currentLevel = CalculateLevel();

            Experience experience = GetComponent<Experience>();
            if(experience != null) {
                experience.onExperienceGained += UpdateLevel;
            }

        }

        /// <summary>
        /// Checks to see if the character has enough exp to level up. If so, instantiate the level up effect and any functions associated with leveling up in the onLevelUp Action.
        /// </summary>
        private void UpdateLevel() {

            int newLevel = CalculateLevel();
            if (newLevel > currentLevel) {

                currentLevel = newLevel;
                levelUpEffect();
                onLevelUp();
            }

        }

        /// <summary>
        /// If there is a level up effect, spawn it on this game object.
        /// </summary>
        private void levelUpEffect() {
            
            if(levelUpParticleEffect != null) {
                Instantiate(levelUpParticleEffect, transform);
            }
            
        }

        /// <summary>
        /// Grabs the selected stat amount based on the level of the character
        /// </summary>
        /// <param name="stat"> Type of stat being looked at</param>
        /// <returns> float value of stat for that level </returns>
        public float GetStat(Stat stat) {
            return progression.GetStat(stat,characterClass,GetLevel()) + GetAdditiveModifier(stat);
        }

        /// <summary>
        /// Gets all values of objects that can affect the effectiveness of a specific stat. It searches for any component with the interface of IModifierProvider and loops through all the values it returns.
        /// These values are added up and added to the stat.
        /// </summary>
        /// <param name="stat"> Type of stat being looked at </param>
        /// <returns> float of modifiers applied to the stat </returns>
        private float GetAdditiveModifier(Stat stat) {

            float total = 0;

            foreach(IModifierProvider provider in GetComponents<IModifierProvider>()) {

                foreach(float modifier in provider.GetAdditiveModifer(stat)) {

                    total += modifier;
                }
            }
            return total;
        }

        /// <summary>
        /// Gets the level of the character. If the level is below 1, calculate the level to be sure
        /// </summary>
        /// <returns> int Level of character</returns>
        public int GetLevel() {

            if(currentLevel < 1) {
                currentLevel = CalculateLevel();
            }
            return currentLevel;
        }

        /// <summary>
        /// Determines the level of the character based on the amount of experience it current has. Determines if it has leveled up if the classes
        /// experience to level up is exceed. 
        /// </summary>
        /// <returns> int level of the character </returns>
        public int CalculateLevel() {

            Experience experience = GetComponent<Experience>();

            if (experience == null) return startingLevel;

            float currentEXP = experience.GetPoints();
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
            for (int level = 1; level <= penultimateLevel; level++) {

                float EXPToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
                if(EXPToLevelUp > currentEXP) {
                    return level;
                }
            }
            return penultimateLevel + 1;
        }

    }
}

