using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat {
    public class Fighter : MonoBehaviour, IAction {
      
        [SerializeField] float AttackDelay = 1f;
        [SerializeField] Transform RightHandTransform = null;
        [SerializeField] Transform LeftHandTransform = null;
        [SerializeField] Weapon defaultWeapon = null;
        
        Health target;
        Weapon currentWeapon = null;
        float timeSinceLastAttack = Mathf.Infinity;     

        private void Start() {

            EquipWeapon(defaultWeapon);

        }

        private void Update() {

            timeSinceLastAttack += Time.deltaTime;

            if (target == null) return;
            if (target.IsDead()) return;

            if (!GetIsInRange()) {
                GetComponent<Mover>().MoveTo(target.transform.position,1f);
            }
            else {
                GetComponent<Mover>().Cancel(); //Character stops moving
                AttackBehaviour();
            }
        }


        /// <summary>
        /// Check if character is in range depending on weapon
        /// </summary>
        /// <returns> True or False if the character is in range of the target assigned </returns>
        private bool GetIsInRange() {
            return Vector3.Distance(transform.position, target.transform.position) < currentWeapon.GetRange() ;
        }

        /// <summary>
        /// Handles the attacks for the character. Forces them to face the target and attack if the current time is longer than its attack delay.
        /// </summary>
        private void AttackBehaviour() {

            transform.LookAt(target.transform);

            if (timeSinceLastAttack > AttackDelay) {
                //This will trigger the Hit() Event
                TriggerAttack();
                timeSinceLastAttack = 0;
            }
        }

        /// <summary>
        /// Responsible for setting the current combatTarget for the character.
        /// </summary>
        /// <param name="CombatTarget"> Combat Target </param>
        public void Attack(GameObject CombatTarget) {

            GetComponent<ActionScheduler>().StartAction(this);
            target = CombatTarget.GetComponent<Health>();

        }

        /// <summary>
        /// Animation Event. Once triggered in the animation, the current target will take damage based on the weapon damage of the character
        /// </summary>
        void Hit() {
            if (target == null) return;
            
            if (currentWeapon.HasProjectile()) {
                currentWeapon.LaunchProjectile(RightHandTransform, LeftHandTransform, target);
            }
            else {
                target.TakeDamage(currentWeapon.GetDamage());
            }
        }

        void Shoot() {
            Hit();
        }

        /// <summary>
        /// Determines whether the character can target a combat target. If combat target has no health component or is "Dead" then it can't be targeted.
        /// </summary>
        /// <param name="combatTarget"></param>
        /// <returns></returns>
        public bool CanAttack(GameObject combatTarget) {

            if (combatTarget == null) return false;

            Health TargetToTest = combatTarget.GetComponent<Health>();
            return TargetToTest != null && !TargetToTest.IsDead();
        }


        /// <summary>
        /// Sets the combat target to null. Stops any attacking animations occurring
        /// </summary>
        public void Cancel() {
            TriggerStopAttack();
            target = null;

        }

        /// <summary>
        /// Sets triggers to begin attack animation
        /// </summary>
        private void TriggerAttack() {
            GetComponent<Animator>().ResetTrigger("StopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        /// <summary>
        /// Sets triggers to stop attack animation
        /// </summary>
        private void TriggerStopAttack() {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("StopAttack");
        }

        /// <summary>
        /// Equips a weapon to the character. Calls for the weapon to be spawned on the appropriate hand of the character
        /// </summary>
        /// <param name="weapon"></param>
        public void EquipWeapon(Weapon weapon) {

            currentWeapon = weapon;
            Animator animator = GetComponent<Animator>();
            weapon.SpawnWeapon(RightHandTransform, LeftHandTransform, GetComponent<Animator>());
        }
     
   
    }

}