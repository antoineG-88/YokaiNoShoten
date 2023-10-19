using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public static class SaveSystem
{
    public static string gameSaveFileName;
    public static string progressionSaveFileName;
    public static string saveFileExtension;
    public static string defaultSaveDirectoryName;
    public static string defaultGameDirectoryName;

    private static string savePath;

    /// <summary>
    /// Create the path to the directory where the save files will be created
    /// </summary>
    /// <param name="alternateSavePath">Create a default save path if set to null or empty</param>
    public static void SetSavePath(string alternateSavePath)
    {
        if (alternateSavePath != null && alternateSavePath != "")
        {
            savePath = alternateSavePath;
        }
        else
        {
            savePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            savePath = Path.Combine(savePath, "My Games");
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            savePath = Path.Combine(savePath, defaultGameDirectoryName);
            //savePath += defaultGameDirectoryName;
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            savePath = Path.Combine(savePath, defaultSaveDirectoryName);
            //savePath += defaultSaveDirectoryName;
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
        }
    }

    public static void SaveGameAndProgression(string zoneName)
    {
        if (savePath != "" && savePath != null)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Path.Combine(savePath, gameSaveFileName + saveFileExtension);

            FileStream stream = new FileStream(path, FileMode.Create);

            GameSave gameSave = new GameSave();
            formatter.Serialize(stream, gameSave);
            stream.Close();

            //Debug.Log("Player saved in " + path);
        }
        else
        {
            Debug.LogError("The savePath has not been set");
        }

        if (savePath != "" && savePath != null)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Path.Combine(savePath, progressionSaveFileName + saveFileExtension);

            ProgressionSave progressionSave = LoadProgressionSave();

            FileStream stream = new FileStream(path, FileMode.Create);
            if(progressionSave != null)
            {
                progressionSave.UpdateProgression(GameManager.currentChapter, false, 0f);
            }
            else
            {
                progressionSave = new ProgressionSave(GameManager.currentChapter);
            }

            formatter.Serialize(stream, progressionSave);
            stream.Close();
        }
        else
        {
            Debug.LogError("The savePath has not been set");
        }
    }

    public static void SaveNewFinishedGame(float clearTime)
    {
        if (savePath != "" && savePath != null)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Path.Combine(savePath, progressionSaveFileName + saveFileExtension);

            ProgressionSave progressionSave = LoadProgressionSave();

            FileStream stream = new FileStream(path, FileMode.Create);
            if (progressionSave == null)
            {
                progressionSave = new ProgressionSave(GameManager.currentChapter);
            }
            progressionSave.UpdateProgression(GameManager.currentChapter, true, clearTime);

            formatter.Serialize(stream, progressionSave);
            stream.Close();
        }
        else
        {
            Debug.LogError("The savePath has not been set");
        }
    }

    public static GameSave LoadGameSave()
    {
        //string path = savePath + zoneDataFileNamePrefixe + zoneName + zoneSaveFileExtension;
        string path = Path.Combine(savePath, gameSaveFileName + saveFileExtension);

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(path, FileMode.Open);

            GameSave gameSave = formatter.Deserialize(stream) as GameSave;
            stream.Close();

            //Debug.Log("PLayer loaded from " + path);

            return gameSave;
        }
        else
        {
            Debug.LogWarning("Save file not found in " + path);
            return null;
        }
    }

    public static ProgressionSave LoadProgressionSave()
    {
        //string path = savePath + progressionDataSaveFileName + saveFileExtension;
        string path = Path.Combine(savePath, progressionSaveFileName + saveFileExtension);
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(path, FileMode.Open);

            ProgressionSave progressionSave = formatter.Deserialize(stream) as ProgressionSave;
            stream.Close();

            //Debug.Log("PLayer loaded from " + path);

            return progressionSave;
        }
        else
        {
            Debug.LogWarning("Save file not found in " + path);
            return null;
        }
    }

    public static void DeleteGameSaveFile()
    {
        //string path = savePath + zoneDataFileNamePrefixe + zoneName + saveFileExtension;
        string path = Path.Combine(savePath, gameSaveFileName + saveFileExtension);
        
        if(File.Exists(path))
        {
            Debug.Log("Game save deleted at : " + path);
            File.Delete(path);
        }
    }
}
