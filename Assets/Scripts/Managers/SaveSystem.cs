using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public static class SaveSystem
{
    public static string zoneDataFileNamePrefixe;
    public static string progressionDataSaveFileName;
    public static string zoneSaveFileExtension;
    public static string progressionSaveFileExtension;
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

    public static void SaveProgression(string zoneName)
    {
        if (savePath != "" && savePath != null)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            //string path = savePath + zoneDataFileNamePrefixe + zoneName + zoneSaveFileExtension;
            string path = Path.Combine(savePath, zoneDataFileNamePrefixe + zoneName + zoneSaveFileExtension);

            FileStream stream = new FileStream(path, FileMode.Create);

            ZoneData zoneData = new ZoneData();
            formatter.Serialize(stream, zoneData);
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
            //string path = savePath + progressionDataSaveFileName + progressionSaveFileExtension;
            string path = Path.Combine(savePath, progressionDataSaveFileName + progressionSaveFileExtension);

            FileStream stream = new FileStream(path, FileMode.Create);

            ProgressionData progressionData = new ProgressionData(GameManager.currentStoryStep);
            formatter.Serialize(stream, progressionData);
            stream.Close();

            //Debug.Log("Player saved in " + path);
        }
        else
        {
            Debug.LogError("The savePath has not been set");
        }
    }

    public static ZoneData LoadZone(string zoneName)
    {
        //string path = savePath + zoneDataFileNamePrefixe + zoneName + zoneSaveFileExtension;
        string path = Path.Combine(savePath, zoneDataFileNamePrefixe + zoneName + zoneSaveFileExtension);

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(path, FileMode.Open);

            ZoneData zoneData = formatter.Deserialize(stream) as ZoneData;
            stream.Close();

            //Debug.Log("PLayer loaded from " + path);

            return zoneData;
        }
        else
        {
            Debug.LogWarning("Save file not found in " + path);
            return null;
        }
    }

    public static ProgressionData LoadProgression()
    {
        //string path = savePath + progressionDataSaveFileName + progressionSaveFileExtension;
        string path = Path.Combine(savePath, progressionDataSaveFileName + progressionSaveFileExtension);

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(path, FileMode.Open);

            ProgressionData progressionData = formatter.Deserialize(stream) as ProgressionData;
            stream.Close();

            //Debug.Log("PLayer loaded from " + path);

            return progressionData;
        }
        else
        {
            Debug.LogWarning("Save file not found in " + path);
            return null;
        }
    }

    public static void DeleteSaveFile(string zoneName)
    {
        //string path = savePath + zoneDataFileNamePrefixe + zoneName + zoneSaveFileExtension;
        string path = Path.Combine(savePath, zoneDataFileNamePrefixe + zoneName + zoneSaveFileExtension);
        
        if(File.Exists(path))
        {
            Debug.Log("deleted at : " + path);
            File.Delete(path);
        }

        path = Path.Combine(savePath, progressionDataSaveFileName + progressionSaveFileExtension);
        if (File.Exists(path))
        {
            Debug.Log("deleted at : " + path);
            File.Delete(path);
        }
    }
}
