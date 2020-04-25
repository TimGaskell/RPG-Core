using System;
using System.Collections;
using RPG.Control;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement {
    public class Portal : MonoBehaviour {
        enum DestinationIdentifier {
            A, B, C, D, E
        }

        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeInTime = 2f;
        [SerializeField] float fadeWaitTime = 0.5f;

        private void OnTriggerEnter(Collider other) {
            if (other.tag == "Player") {
                StartCoroutine(Transition());
            }
        }

        /// <summary>
        /// Transition sequence of loading the player into a new area via scene loading. The scene fades out through UI elements and saves the current level. Once done it will load the new scene completely before transitioning over.
        /// It then loads the data from the previous level over and moves player to the spawn location of the new scene. It saves the new scene and fades the scene in with UI elements
        /// </summary>
        /// <returns> null</returns>
        private IEnumerator Transition() {
            if (sceneToLoad < 0) {
                Debug.LogError("Scene to load not set.");
                yield break;
            }

            DontDestroyOnLoad(gameObject);

            Fader fader = FindObjectOfType<Fader>();
            
            PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            playerController.enabled = false;

            yield return fader.FadeOut(fadeOutTime); //Fades out scene with UI

            //Save Current Level
            SavingWrapper wrapper = GameObject.FindObjectOfType<SavingWrapper>(); ;
            wrapper.Save(); //Saves data from current level

            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            PlayerController newPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            newPlayerController.enabled = false;

            //Load Current Level
            wrapper.Load();

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            wrapper.Save(); //Save data from new level.

            yield return new WaitForSeconds(fadeWaitTime);
            fader.FadeIn(fadeInTime);

            newPlayerController.enabled = true;
            Destroy(gameObject);
        }

        /// <summary>
        /// Causes the player to be moved to the spawn location and rotation of the portal its traveling to.
        /// </summary>
        /// <param name="otherPortal"> Portal script attached to the portal game object player is moving to</param>
        private void UpdatePlayer(Portal otherPortal) {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().enabled = false;
            player.transform.position = otherPortal.spawnPoint.position;
            player.transform.rotation = otherPortal.spawnPoint.rotation;
            player.GetComponent<NavMeshAgent>().enabled = true;
        }

        /// <summary>
        /// Searches for all portals in a scene. Returns a singular portal if it is not the same portal and if the destinations are set the same. 
        /// </summary>
        /// <returns> Portal script attached to portal game object that player will go to </returns>
        private Portal GetOtherPortal() {
            foreach (Portal portal in FindObjectsOfType<Portal>()) {
                if (portal == this) continue;
                if (portal.destination != destination) continue;

                return portal;
            }

            return null;
        }
    }
}