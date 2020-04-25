using System;
using UnityEngine;

namespace RPG.Core {
    public class PersitentObjectSpawner : MonoBehaviour {
        [SerializeField] GameObject persistentObjectPrefab;

        static bool hasSpawned = false;

        private void Awake() {
            if (hasSpawned) return;

            SpawnPersistentObjects();

            hasSpawned = true;
        }

        /// <summary>
        /// Spawns persistent Game object. Any objects that shouldn't be destroyed on load have to be attached to the parent object being spawned
        /// </summary>
        private void SpawnPersistentObjects() {
            GameObject persistentObject = Instantiate(persistentObjectPrefab);
            DontDestroyOnLoad(persistentObject);
        }
    }
}