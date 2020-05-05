using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.SceneManagement {

    public class SavingWrapper : MonoBehaviour {

        const string defaultSaveFile = "save";
        [SerializeField] float fadeInTime = 0.5f;

        private void Awake() {
            StartCoroutine(LoadLastScene());
        }

        private IEnumerator LoadLastScene() {      
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
            Fader fader = GameObject.FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
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
            else if (Input.GetKeyDown(KeyCode.Delete)) {
                Delete();
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

        /// <summary>
        /// Deletes the save file 
        /// </summary>
        public void Delete() {
            GetComponent<SavingSystem>().Delete(defaultSaveFile);
        }
    }
}

  
