using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats {
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject {

        [SerializeField] ProgressionCharacterClass[] characterClasses = null;

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

        /// <summary>
        /// Function for getting a stats value based on the character class and the level the character currently is.
        /// It first builds a dictionary which contains each character class and a dictionary of its stats with values.
        /// It then searches that dictionary to determine the value of that specific stat for the specific character class
        /// </summary>
        /// <param name="stat"> Stat being looked at</param>
        /// <param name="characterClass"> Character class being looked at</param>
        /// <param name="level"> Level of the character </param>
        /// <returns> float value of the stat at that level</returns>
        public float GetStat(Stat stat, CharacterClass characterClass, int level) {

            BuildLookUp();

            float[] levels =  lookupTable[characterClass][stat];
            if (levels.Length < level) return 0;

            return levels[level - 1];
         
        }

        /// <summary>
        /// Creates a lookup table for all the character classes and stats.
        /// </summary>
        private void BuildLookUp() {
            if (lookupTable != null) return;

            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach (ProgressionCharacterClass progressionClass in characterClasses) {

                var statLookUptable = new Dictionary<Stat, float[]>();

                foreach (ProgressionStat progressionStat in progressionClass.stats) {

                    statLookUptable[progressionStat.stat] = progressionStat.levels;

                }
                    lookupTable[progressionClass.characterClass] = statLookUptable;
            }
        }

        /// <summary>
        /// Gets how many levels have been set for a given stat
        /// </summary>
        /// <param name="stat"> Stat being looked at</param>
        /// <param name="characterClass"> Character class being look at</param>
        /// <returns> int length of the assigned values to the stat </returns>
        public int GetLevels(Stat stat, CharacterClass characterClass) {
            BuildLookUp();

            float[] levels = lookupTable[characterClass][stat];
            return levels.Length;
        }

        [System.Serializable]
        class ProgressionCharacterClass {

            public CharacterClass characterClass;
            public ProgressionStat[] stats;

        }

        [System.Serializable]
        class ProgressionStat {
            public Stat stat;
            public float[] levels;

        }

    }
}