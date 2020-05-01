using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat {
    public class WeaponPickUp : MonoBehaviour {

        [SerializeField] Weapon weapon = null;
        [SerializeField] float respawnTime = 5;

        /// <summary>
        /// Responsible for equipping the weapon when the player hits the trigger zone. After its picked up starts the re spawn time for the weapon
        /// </summary>
        /// <param name="other">Collider of entity that entered trigger</param>
        private void OnTriggerEnter(Collider other) {
            
            if(other.gameObject.tag == "Player") {

                other.GetComponent<Fighter>().EquipWeapon(weapon);
                StartCoroutine(HideForSeconds(respawnTime));
                
            }
        }

        /// <summary>
        /// Hides the pickup when triggered. After a set amount of time, makes the pickup visible again
        /// </summary>
        /// <param name="seconds"> Time till re spawn of weapon </param>
        /// <returns> null </returns>
        private IEnumerator HideForSeconds(float seconds) {

            ShowPickUp(false);
            yield return new WaitForSeconds(seconds);
            ShowPickUp(true);


        }

        /// <summary>
        /// Shows or makes pick up invisible
        /// </summary>
        /// <param name="shouldShow"> Bool of whether weapon pickup should be shown</param>
        private void ShowPickUp(bool shouldShow) {

            GetComponent<Collider>().enabled = shouldShow;
            foreach(Transform child in transform) {

                child.gameObject.SetActive(shouldShow);
            }
        }


    }

}
