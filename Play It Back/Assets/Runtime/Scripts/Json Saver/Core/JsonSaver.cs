using UnityEngine;
using System.IO;


public static class JsonSaver
{
    /// <summary>
    /// Call this method to write data into a json file
    /// </summary>
    /// <typeparam name="TType">Type of data</typeparam>
    /// <param name="data">Target data</param>
    /// <param name="filepath">Target filepath</param>
    public static void WriteData<TType>(TType data, string filepath, string filename)
    {
        string jsonString = JsonUtility.ToJson(data);

        if (string.IsNullOrEmpty(jsonString))
        {
            Debug.LogError("Data not supported!!");
            return;
        }

        File.WriteAllText($"{filepath}/{filename}.json", jsonString);
    }

    /// <summary>
    /// Call this method to write data into a json file
    /// </summary>
    /// <typeparam name="TType">Type of data</typeparam>
    /// <param name="data">Target data</param>
    /// <param name="fullFilepath">Target filepath</param>
    public static void WriteData<TType>(TType data, string fullFilepath)
    {
        string jsonString = JsonUtility.ToJson(data);

        if (string.IsNullOrEmpty(jsonString))
        {
            Debug.LogError("Data not supported!!");
            return;
        }

        File.WriteAllText($"{fullFilepath}.json", jsonString);
    }

    /// <summary>
    /// Call this method to read the data from a target file
    /// </summary>
    /// <typeparam name="TType">Type of data</typeparam>
    /// <param name="filepath">Target filepath</param>
    /// <param name="data">Out data</param>
    public static void ReadData<TType>(string filepath, string filename, out TType data)
    {
        string jsonString = File.ReadAllText($"{filepath}/{filename}.json");
        if (string.IsNullOrEmpty(jsonString))
        {
            Debug.LogError("File read error!!");
            data = default;
            return;
        }
        data = JsonUtility.FromJson<TType>(jsonString);
    }

    /// <summary>
    /// Call this method to read the data from a target file
    /// </summary>
    /// <typeparam name="TType">Type of data</typeparam>
    /// <param name="fullFilepath">Target filepath</param>
    /// <param name="data">Out data</param>
    public static void ReadData<TType>(string fullFilepath, out TType data)
    {
        string jsonString = File.ReadAllText($"{fullFilepath}.json");
        if (string.IsNullOrEmpty(jsonString))
        {
            Debug.LogError("File read error!!");
            data = default;
            return;
        }
        data = JsonUtility.FromJson<TType>(jsonString);
    }
}
