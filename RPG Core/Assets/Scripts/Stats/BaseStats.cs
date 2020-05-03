using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Stats {
    public class BaseStats : MonoBehaviour {

        [Range(1,99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;

        /// <summary>
        /// Grabs the selected stat amount based on the level of the character
        /// </summary>
        /// <param name="stat"> Type of stat being looked at</param>
        /// <returns> float value of stat for that level </returns>
        public float GetStat(Stat stat) {
            return progression.GetStat(stat,characterClass,GetLevel());
        }

        /// <summary>
        /// Determines the level of the character based on the amount of experience it current has. Determines if it has leveled up if the classes
        /// experience to level up is exceed. 
        /// </summary>
        /// <returns> int level of the character </returns>
        public int GetLevel() {

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

