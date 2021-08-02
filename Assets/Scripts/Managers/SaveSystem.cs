using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public static class SaveSystem
{
    public static string progressionDataSaveFileNamePrefixe;
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
            savePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/My Games";
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            savePath += defaultGameDirectoryName;
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            savePath += defaultSaveDirectoryName;
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
        }
    }

    public static void SaveProgression()
    {
        if (savePath != "" && savePath != null)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = savePath + progressionDataSaveFileNamePrefixe + GameManager.currentZoneName + saveFileExtension;

            FileStream stream = new FileStream(path, FileMode.Create);

            ProgressionData progressionData = new ProgressionData();
            formatter.Serialize(stream, progressionData);
            stream.Close();

            //Debug.Log("Player saved in " + path);
        }
        else
        {
            Debug.LogError("The savePath has not been set");
        }
    }

    public static ProgressionData LoadProgression()
    {
        string path = savePath + progressionDataSaveFileNamePrefixe + GameManager.currentZoneName + saveFileExtension;

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
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    public static void DeleteSaveFile(string zoneName)
    {
        string path = savePath + progressionDataSaveFileNamePrefixe + zoneName + saveFileExtension;
        Debug.Log("deleted at : " + path);
        File.Delete(path);
    }
}
