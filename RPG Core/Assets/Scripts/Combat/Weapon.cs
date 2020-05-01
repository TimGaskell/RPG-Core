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

            var overideController = animator.runtimeAnimatorController as AnimatorOverrideController; //Checks if animator controller is of type animatorOverrideController
           
            if (weaponOverride != null) {
                animator.runtimeAnimatorController = weaponOverride;
            }
            else if(overideController != null)  {   
                
               animator.runtimeAnimatorController = overideController.runtimeAnimatorController; //Changes back to default animation if there was an override and weapon doesn't have one
           
            }
        }

        /// <summary>
        /// Gets rid of the weapon that the character was using. Searches each hand to determine if the character is holding a weapon.
        /// </summary>
        /// <param name="rightHandTransform"> Right hand transform </param>
        /// <param name="leftHandTransform"> Left hand transform</param>
        private void DestroyOldWeapon(Transform rightHandTransform, Transform leftHandTransform) {

            Transform oldWeapon = rightHandTransform.Find(weaponName);
            if(oldWeapon == null) {
                oldWeapon = leftHandTransform.Find(weaponName);
            }
            if (oldWeapon == null) return;

            oldWeapon.name = "Destroying";
            Destroy(oldWeapon.gameObject);
        }

        /// <summary>
        /// Determines the hand weapon uses based on Bool isRightHanded.
        /// </summary>
        /// <param name="RightHandTransform">Right hand transform</param>
        /// <param name="LeftHandTransform"> Left hand transform</param>
        /// <returns> Transform weapon should use for hand </returns>
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

        /// <summary>
        /// Determines if weapon shoots projectiles
        /// </summary>
        /// <returns> True or false if weapon shoots projectiles </returns>
        public bool HasProjectile() {
            return projectile != null;
        }

        /// <summary>
        /// Responsible for instantiating a projectile in either the right or left hand. Also assigns the target for this projectile and damage
        /// </summary>
        /// <param name="RightHand"> Right hand Transform</param>
        /// <param name="LeftHand"> Left Hand transform </param>
        /// <param name="Target"> Target with health script</param>
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
