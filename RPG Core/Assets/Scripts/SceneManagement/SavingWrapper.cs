using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.SceneManagement {

    public class SavingWrapper : MonoBehaviour {

        const string defaultSaveFile = "save";
        [SerializeField] float fadeInTime = 0.5f;

        private IEnumerator Start() {
            Fader fader = GameObject.FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
            yield return fader.FadeIn(fadeInTime);

        }

        // Update is called once per frame
        void Update() {

            if (Input.GetKeyDown(KeyCode.L)) {
                Load();
            }
            else if (Input.GetKeyDown(KeyCode.S)) {
                Save();
            }

        }
        /// <summary>
        /// Calls Load from SavingSystem.
        /// </summary>
        public void Load() {
            GetComponent<SavingSystem>().Load(defaultSaveFile);
        }

        /// <summary>
        /// Calls save from saving system
        /// </summary>
        public void Save() {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }
    }
}

  
