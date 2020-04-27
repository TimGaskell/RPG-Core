using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Saving
{
    public class SavingSystem : MonoBehaviour
    {
        /// <summary>
        /// Function responsible for loading the last scene that was saved. Determines if the saved scene build index is the same as the current scene build index. If not it will load the scene that was 
        /// saved and restore the states in that save file 
        /// </summary>
        /// <param name="saveFile"> Name of Save File</param>
        /// <returns> Null </returns>
        public IEnumerator LoadLastScene(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            if (state.ContainsKey("lastSceneBuildIndex"))
            {
                buildIndex = (int)state["lastSceneBuildIndex"];
            }
            yield return SceneManager.LoadSceneAsync(buildIndex);
            RestoreState(state);
        }

        /// <summary>
        /// Function used for saving the game
        /// </summary>
        /// <param name="saveFile"> String name of save file</param>
        public void Save(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            CaptureState(state);
            SaveFile(saveFile, state);
        }

        /// <summary>
        /// Function used for loading a previous save
        /// </summary>
        /// <param name="saveFile">String name of save file</param>
        public void Load(string saveFile)
        {
            RestoreState(LoadFile(saveFile));
        }

        /// <summary>
        /// Deletes save file
        /// </summary>
        /// <param name="saveFile"> string name of save file </param>
        public void Delete(string saveFile)
        {
            File.Delete(GetPathFromSaveFile(saveFile));
        }

        /// <summary>
        /// Returns a dictionary of <Sting,Object> by either creating a new one if there is no save file or deserializing an old save file. 
        /// This dictionary dictates what saveable entities scripts need to be restored. 
        /// </summary>
        /// <param name="saveFile"> String name of save file being loaded</param>
        /// <returns> Dictionary of dictionaries referring to each object and their dictionary of scripts that need to be restored</returns>
        private Dictionary<string, object> LoadFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);
            if (!File.Exists(path))
            {
                return new Dictionary<string, object>();
            }
            using (FileStream stream = File.Open(path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (Dictionary<string, object>)formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Saves the data to the selected save file. It rewrites or creates a file with the selected file name and serializes the save data into binary.
        /// The binary data is written to the save file. The saved data is a dictionary of dictionaries of each object key and dictionary of scripts that contain restorable data
        /// </summary>
        /// <param name="saveFile"> String name of file being created </param>
        /// <param name="state"> Serializable data being saved </param>
        private void SaveFile(string saveFile, object state)
        {
            string path = GetPathFromSaveFile(saveFile);
            print("Saving to " + path);
            using (FileStream stream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }
        }

        /// <summary>
        /// Captures the states of all savable entities in the scene. These states are saved in the state dictionary where each entity has a unique identifier. 
        /// This allows the loading to know which object relates to which state. Also logs what the current scene is to the save dictionary.
        /// </summary>
        /// <param name="state"> Dictionary of data being saved to a file </param>
        private void CaptureState(Dictionary<string, object> state)
        {
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                state[saveable.GetUniqueIdentifier()] = saveable.CaptureState();
            }

            state["lastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;
        }

        /// <summary>
        /// Loops through all objects that have a saveableEntity script attached to them. Determines whether the save file dictionary contains the unique identifier of the object.
        /// If it does, there is saved data to be restored and calls for it to be restored for the object.
        /// </summary>
        /// <param name="state"> Dictionary of saved data </param>
        private void RestoreState(Dictionary<string, object> state)
        {
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                string id = saveable.GetUniqueIdentifier();
                if (state.ContainsKey(id))
                {
                    saveable.RestoreState(state[id]);
                }
            }
        }

        /// <summary>
        /// Creates a file path for a save file. This file path leads to where unity places data files on an application. E.g. C:/Users/....
        /// </summary>
        /// <param name="saveFile"> String name for save file </param>
        /// <returns> File path for the save file </returns>
        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile + ".sav");
        }
    }
}