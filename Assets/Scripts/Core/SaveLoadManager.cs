using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary; // Binary format file.
using System.IO; // Open - read and write files.

namespace KekeDreamLand
{
    public class SaveLoadManager : MonoBehaviour {

        public static string saveFileName = "playerProgress.sav";

        public static void SavePlayerProgress(PlayerProgress progress)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(Application.persistentDataPath + "/" + saveFileName, FileMode.Create);

            // Save current world and graph node of Boing and also states of each level.
            bf.Serialize(stream, progress);

            stream.Close();
        }

        public static PlayerProgress LoadPlayerProgress()
        {
            PlayerProgress loadedPlayerProgress = new PlayerProgress();

            if(File.Exists(Application.persistentDataPath + "/" + saveFileName))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream stream = new FileStream(Application.persistentDataPath + "/" + saveFileName, FileMode.Open);

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