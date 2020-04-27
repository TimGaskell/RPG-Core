using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace RPG.Saving
{
    [ExecuteAlways]
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField] string uniqueIdentifier = "";
        static Dictionary<string, SaveableEntity> globalLookup = new Dictionary<string, SaveableEntity>();

        /// <summary>
        /// Returns a unique Identifier for this object
        /// </summary>
        /// <returns> Unique string identifier </returns>
        public string GetUniqueIdentifier()
        {
            return uniqueIdentifier;
        }

        /// <summary>
        /// Retrieves all Components that have savable data that have an ISavable Interface in them. Each of component will return a dictionary 
        /// of savable data for their component and save it with a key to the script name
        /// </summary>
        /// <returns> Dictionary of Dictionaries with a script name key and dictionary of variables that need to be restored or singular object state instead of dictionary</returns>
        public object CaptureState()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                state[saveable.GetType().ToString()] = saveable.CaptureState();
            }
            return state;
        }

        /// <summary>
        /// Restores all Components that have savable data that have an ISavable Interface in them. If the current object has an Isavebale component with a key found in the dictionary then it 
        /// call for that script to restore its state with the appropriate dictionary for that script
        /// </summary>
        /// <param name="state">Typically Dictionary of Dictionaries with a script name key and dictionary of variables that need to be restored. Can refer to singular object state </param>
        public void RestoreState(object state)
        {
            Dictionary<string, object> stateDict = (Dictionary<string, object>)state;
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                string typeString = saveable.GetType().ToString();
                if (stateDict.ContainsKey(typeString))
                {
                    saveable.RestoreState(stateDict[typeString]);
                }
            }
        }

#if UNITY_EDITOR
        private void Update() {
            if (Application.IsPlaying(gameObject)) return;
            if (string.IsNullOrEmpty(gameObject.scene.path)) return; //Only creates identifier if game isn't running and not in a prefab editing scene

            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty property = serializedObject.FindProperty("uniqueIdentifier");
            
            if (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))  //Checks if has unique identifier or identifier is not unique
            {
                property.stringValue = System.Guid.NewGuid().ToString(); //Generates unique Identifier for Object
                serializedObject.ApplyModifiedProperties(); //Overrides uniqueIndentifier with new identifier
            }

            globalLookup[property.stringValue] = this;
        }
#endif

        /// <summary>
        /// Determines if a unique identifier is already being used by an object. 
        /// </summary>
        /// <param name="candidate"> String Unique identifier</param>
        /// <returns> True or False if the identifier is already being used </returns>
        private bool IsUnique(string candidate)
        {
            if (!globalLookup.ContainsKey(candidate)) return true; //If no object is using identifier

            if (globalLookup[candidate] == this) return true; //If identifier is this entity

            if (globalLookup[candidate] == null) //If dictionary deleted entity previously
            {
                globalLookup.Remove(candidate);
                return true;
            }

            if (globalLookup[candidate].GetUniqueIdentifier() != candidate) //If this entity has updated its key, update it
            {
                globalLookup.Remove(candidate);
                return true;
            }

            return false;
        }
    }
}