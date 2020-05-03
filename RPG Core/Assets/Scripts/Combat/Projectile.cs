using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Resources;

namespace RPG.Combat {

    public class Projectile : MonoBehaviour {
        
        [SerializeField] float Speed = 1;
        [SerializeField] bool isHoming = true;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifeTime = 10f;
        [SerializeField] float lifeAfterImpact = 2f;
        [SerializeField] GameObject[] destroyOnHit = null;
        Health target = null;
        GameObject instigator = null;
        float damage = 0;

        private void Start() {

            transform.LookAt(GetAimLocation()); // Makes projectile face target

        }


        // Update is called once per frame
        void Update() {

            if (target == null) return;
            if (isHoming && !target.IsDead()) {
                transform.LookAt(GetAimLocation());
            }
            transform.Translate(Vector3.forward * Speed * Time.deltaTime);

        }

        /// <summary>
        /// Sets the target the projectile is heading toward. Sets how much damage this projectile does. Also sets the time for how long the projectile will stay in the scene
        /// </summary>
        /// <param name="target"> Target projectile is targeted at </param>
        /// <param name="damage"> Amount of damage projectile does to target </param>
        public void SetTarget(Health target, GameObject instigator, float damage) {

            this.target = target;
            this.damage = damage;
            this.instigator = instigator;

            Destroy(gameObject, maxLifeTime);

        }

        /// <summary>
        /// Gets the location the projectile should head towards. IF the target has a capsule collider it adjusts the height to aim for the center mass. 
        /// </summary>
        /// <returns> Vector 3 position where the projectile target should go to </returns>
        private Vector3 GetAimLocation() {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
           
            if(targetCapsule == null) {
                return target.transform.position;
            }
            return target.transform.position + Vector3.up * targetCapsule.height / 2;
        }

        /// <summary>
        /// Trigger event when projectile hits its target. The projectile does damage to the target and creates a hit effect is assigned. Once done the object is then destoryed
        /// </summary>
        /// <param name="other">Collider of entity that entered trigger</param>
        private void OnTriggerEnter(Collider other) {

            if (other.GetComponent<Health>() != target) return;
            if (target.IsDead()) return;
            target.TakeDamage(instigator, damage);
            Speed = 0;

            if (hitEffect != null) {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }
               
            foreach(GameObject toDestory in destroyOnHit) {
                Destroy(toDestory);
            }

            Destroy(gameObject,lifeAfterImpact);
            
        }
    }


}
