using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;

namespace RPG.Combat {
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider {
      
        [SerializeField] float AttackDelay = 1f;
        [SerializeField] Transform RightHandTransform = null;
        [SerializeField] Transform LeftHandTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;
        
        Health target;
        WeaponConfig CurrentWeaponConfig = null;
        Weapon CurrentWeapon;
       
        float timeSinceLastAttack = Mathf.Infinity;

        private void Awake() {

            CurrentWeaponConfig = defaultWeapon;
            CurrentWeapon = SetupDefaultWeapon();
        }

        private void Start() {

            EquipWeapon(CurrentWeaponConfig);
        }

        private void Update() {

            timeSinceLastAttack += Time.deltaTime;

            if (target == null) return;
            if (target.IsDead()) return;

            if (!GetIsInRange(target.transform)) {
                GetComponent<Mover>().MoveTo(target.transform.position,1f);
            }
            else {
                GetComponent<Mover>().Cancel(); //Character stops moving
                AttackBehaviour();
            }
        }

        private Weapon SetupDefaultWeapon() {
            return EquipWeapon(defaultWeapon);           
        }


        /// <summary>
        /// Check if character is in range depending on weapon
        /// </summary>
        /// <returns> True or False if the character is in range of the target assigned </returns>
        private bool GetIsInRange( Transform targetTransform) {
            return Vector3.Distance(transform.position, targetTransform.position) < CurrentWeaponConfig.GetRange() ;
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

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

            if(CurrentWeapon != null) {
                CurrentWeapon.onHit();
            }

            if (CurrentWeaponConfig.HasProjectile()) {
                CurrentWeaponConfig.LaunchProjectile(RightHandTransform, LeftHandTransform, target,gameObject,damage);
            }
            else {
                
                target.TakeDamage(gameObject, damage);
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
            if (!GetComponent<Mover>().CanMoveTo(combatTarget.transform.position) &&
                !GetIsInRange(combatTarget.transform)) { 
                return false;
            }

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
        public Weapon EquipWeapon(WeaponConfig weapon) {

            CurrentWeaponConfig = weapon;      
            Animator animator = GetComponent<Animator>();
            CurrentWeapon = weapon.SpawnWeapon(RightHandTransform, LeftHandTransform, GetComponent<Animator>());
            return CurrentWeapon;
            
        }

        /// <summary>
        /// Returns the Health component of the target
        /// </summary>
        /// <returns> Returns the Health component of the target </returns>
        public Health GetTarget() {
            return target;
        }

        /// <summary>
        /// Saves the current weapon name 
        /// </summary>
        /// <returns> name of weapon name </returns>
        public object CaptureState() {

            return CurrentWeaponConfig.name;
        }

        /// <summary>
        /// Reloads the last weapon the player had equipped in the save
        /// </summary>
        /// <param name="state"> Object containing string name of weapon last equipped </param>
        public void RestoreState(object state) {

            string WeaponName = (string)state;
            WeaponConfig weapon = UnityEngine.Resources.Load<WeaponConfig>(WeaponName);
            EquipWeapon(weapon);

        }

        /// <summary>
        /// Function for returning any modifiers this script has for any specified stat passed in.
        /// </summary>
        /// <param name="stat"> Stat checking for modifiers</param>
        /// <returns> list of IEnumerable floats for the given stat </returns>
        public IEnumerable<float> GetAdditiveModifers(Stat stat) {
           if(stat == Stat.Damage) {
                yield return CurrentWeaponConfig.GetDamage();
            }
        }


        /// <summary>
        /// Function for returning any modifiers this script has for any specified stat passed in.
        /// </summary>
        /// <param name="stat"> Stat checking for modifiers</param>
        /// <returns> list of IEnumerable floats for the given stat </returns>
        public IEnumerable<float> GetPercentageModifiers(Stat stat) {
            if(stat == Stat.Damage) {
                yield return CurrentWeaponConfig.GetPercentageBonus();
            }
        }
    }
}