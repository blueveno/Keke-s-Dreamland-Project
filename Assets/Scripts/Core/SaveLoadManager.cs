using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary; // Binary format file.
using System.IO; // Open - read and write files.

namespace KekeDreamLand
{
    public class SaveLoadManager : MonoBehaviour {

        public static string saveFileName = "playerProgress";
        public static string saveFileExtension = ".sav";

        /// <summary>
        /// Check that the file of the save slot specified exists.
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public static bool CheckSlot(int slot)
        {
            return File.Exists(Application.persistentDataPath + "/" + saveFileName + slot.ToString() + saveFileExtension);
        }

        /// <summary>
        /// Save player progress of the specified save.
        /// </summary>
        /// <param name="progress">Current player progress.</param>
        /// <param name="slot">save slot.</param>
        public static void SavePlayerProgress(PlayerProgress progress, int slot)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(Application.persistentDataPath + "/" + saveFileName + slot.ToString() + saveFileExtension, FileMode.Create);

            // Save current world and graph node of Boing and also states of each level.
            bf.Serialize(stream, progress);

            stream.Close();
        }

        /// <summary>
        /// Load the player progress of the specified save.
        /// </summary>
        /// <param name="slot">save slot</param>
        /// <returns>Player progress</returns>
        public static PlayerProgress LoadPlayerProgress(int slot)
        {
            PlayerProgress loadedPlayerProgress = new PlayerProgress();

            if(File.Exists(Application.persistentDataPath + "/" + saveFileName + slot.ToString() + saveFileExtension))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream stream = new FileStream(Application.persistentDataPath + "/" + saveFileName + slot.ToString() + saveFileExtension, FileMode.Open);

                loadedPlayerProgress = bf.Deserialize(stream) as PlayerProgress;

                stream.Close();
            }

            else
            {
                Debug.LogWarning("No save found !");
            }

            return loadedPlayerProgress;
        }

    }
}