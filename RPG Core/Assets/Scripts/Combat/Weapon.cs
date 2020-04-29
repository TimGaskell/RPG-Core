using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using System;

namespace RPG.Combat {

    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject {

        [SerializeField] AnimatorOverrideController weaponOverride = null;
        [SerializeField] GameObject weaponPrefab = null;
        [SerializeField] float WeaponRange = 2f;
        [SerializeField] float WeaponDamage = 5f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "Weapon";

        /// <summary>
        /// Instantiates the weapon prefab to the characters hand transform. Overrides the attacking animation with the animation for the specific weapon
        /// </summary>
        /// <param name="RightHandTransform"> Transform of right hand for weapon</param>
        /// <param name="LeftHandTransform"> Transform of left hand for weapon</param>
        /// <param name="animator"> Animation component for character </param>
        public void SpawnWeapon(Transform RightHandTransform, Transform LeftHandTransform, Animator animator) {

            DestroyOldWeapon(RightHandTransform, LeftHandTransform);
            
            if (weaponPrefab != null) {
                
                Transform handTransform = GetHandTransform(RightHandTransform, LeftHandTransform);
                GameObject weapon = Instantiate(weaponPrefab, handTransform);
                weapon.name = weaponName;

            }
            if (weaponOverride != null) {
                animator.runtimeAnimatorController = weaponOverride;
            }
        }

        private void DestroyOldWeapon(Transform rightHandTransform, Transform leftHandTransform) {

            Transform oldWeapon = rightHandTransform.Find(weaponName);
            if(oldWeapon == null) {
                oldWeapon = leftHandTransform.Find(weaponName);
            }
            if (oldWeapon == null) return;

            oldWeapon.name = "Destroying";
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetHandTransform(Transform RightHandTransform, Transform LeftHandTransform) {
            Transform handTransform;

            if (isRightHanded) {
                handTransform = RightHandTransform;
            }
            else {
                handTransform = LeftHandTransform;
            }

            return handTransform;
        }

        public bool HasProjectile() {
            return projectile != null;
        }

        public void LaunchProjectile(Transform RightHand, Transform LeftHand, Health Target) {

            Projectile projectileInstance = Instantiate(projectile, GetHandTransform(RightHand, LeftHand).position,Quaternion.identity);
            projectileInstance.SetTarget(Target,WeaponDamage);
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
