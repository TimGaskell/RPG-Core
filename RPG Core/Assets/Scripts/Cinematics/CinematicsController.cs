using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using RPG.Core;
using RPG.Control;

namespace RPG.Cinematics {
    public class CinematicsController : MonoBehaviour {

        private GameObject player;


        private void Start() {
            GetComponent<PlayableDirector>().played += DisableControl;
            GetComponent<PlayableDirector>().stopped += EnableControl;
            player = GameObject.FindWithTag("Player");
        }


        /// <summary>
        /// Disables player actions once entering a cut scene. 
        /// </summary>
        /// <param name="pd"> Player Director</param>
        void DisableControl(PlayableDirector pd) {
        
            player.GetComponent<ActionScheduler>().CancelCurrentAction();
            player.GetComponent<PlayerController>().enabled = false;
        }

        /// <summary>
        /// Re enables player control once the cut scene has ended.
        /// </summary>
        /// <param name="pd"> Player Director</param>
        void EnableControl(PlayableDirector pd) {

            print("EnableControl");
            player.GetComponent<PlayerController>().enabled = true;
        }
     
    }
}