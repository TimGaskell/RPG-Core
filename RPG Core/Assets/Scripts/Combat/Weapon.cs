using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Combat {

    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject {

        [SerializeField] AnimatorOverrideController weaponOverride = null;
        [SerializeField] GameObject weaponPrefab = null;
        [SerializeField] float WeaponRange = 2f;
        [SerializeField] float WeaponDamage = 5f;
        [SerializeField] bool isRightHanded = true;

        /// <summary>
        /// Instantiates the weapon prefab to the characters hand transform. Overrides the attacking animation with the animation for the specific weapon
        /// </summary>
        /// <param name="RightHandTransform"> Transform of right hand for weapon</param>
        /// <param name="LeftHandTransform"> Transform of left hand for weapon</param>
        /// <param name="animator"> Animation component for character </param>
        public void SpawnWeapon(Transform RightHandTransform, Transform LeftHandTransform, Animator animator) {
            if (weaponPrefab != null) {

                Transform handTransform;

                if (isRightHanded) {
                    handTransform = RightHandTransform;
                }
                else {
                    handTransform = LeftHandTransform;
                }
                Instantiate(weaponPrefab, handTransform);

            }
            if (weaponOverride != null) {
                animator.runtimeAnimatorController = weaponOverride;
            }
        }

        /// <summary>
        /// Getter for this weapons damage
        /// </summary>
        /// <returns> Weapon Damage </returns>
        public float GetDamage() {
            return WeaponDamage;
        }

        /// <summary>
        /// Getter for this weapons range
        /// </summary>
        /// <returns> Weapons Range</returns>
        public float GetRange() {
            return WeaponRange;
        }

    }


}
