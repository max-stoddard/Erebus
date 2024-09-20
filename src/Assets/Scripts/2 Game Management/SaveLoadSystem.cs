using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveLoadSystem
{
    private static readonly BinaryFormatter formatter = new BinaryFormatter();
    private static readonly string pathExtension = "data.EREBUSDATA";


    public static void SaveData(int Score, float Volume, int Width, int Height, bool FullScreen)
    {
        if (Score < 0)
        {
            Debug.LogError($"Score, {Score}, out of range");
            return;
        }

        if (Volume < 0f || Volume > 1f)
        {
            Debug.LogError($"Volume, {Volume}, out of range");
        }
        if (Width < 0f || Height < 0f)
        {
            Debug.LogError($"Width, {Width}, or Height, {Height}, out of range");
        }

        string path = Application.persistentDataPath + Path.DirectorySeparatorChar + pathExtension;
        


        if (File.Exists(path))
        {
            PlayerData dataTemp = LoadData();
            if (dataTemp.Score > Score)
            {
                Score = dataTemp.Score;
            }
        }
        
        FileStream fileStream = new FileStream(path, FileMode.Create);
        PlayerData data = new PlayerData(Score, Volume, Width, Height, FullScreen);

        formatter.Serialize(fileStream, data);
        fileStream.Close();
    }

    public static void SaveData(int Score)
    {
        PlayerData data = LoadData();
        SaveData(Score, data.Volume, data.Width, data.Height, data.FullScreen);
    }

    public static void SaveData(int Width, int Height)
    {
        PlayerData data = LoadData();
        SaveData(data.Score, data.Volume, Width, Height, data.FullScreen);
    }

    public static void SaveData(float Volume, int Width, int Height, bool FullScreen)
    {
        PlayerData data = LoadData();
        SaveData(data.Score, Volume, Width, Height, FullScreen);
    }

    private static void SaveDataDefault()
    {
        SaveData(0, 0.5f, 1280, 720, false);
    }

    public static PlayerData LoadData()
    {
        string path = Application.persistentDataPath + Path.DirectorySeparatorChar + pathExtension;

        if (!File.Exists(path))
        {
            SaveDataDefault();
        }
        FileStream fileStream = new FileStream(path, FileMode.Open);
        PlayerData data = (PlayerData)formatter.Deserialize(fileStream);
        fileStream.Close();
        return data;
    }
}
