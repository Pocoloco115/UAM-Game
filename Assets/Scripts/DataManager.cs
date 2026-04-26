using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class Data
{
    public int currentCoins;
    public List<Level> levelsCompleted = new List<Level>();
    public string selectedSkinId = "default";
    public List<Skin> skins = new List<Skin>();

}
public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    private string _dataFilePath;
    public Data data { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _dataFilePath = Path.Combine(Application.persistentDataPath, "data.json");

        Debug.Log($"[DataManager] persistentDataPath = {Application.persistentDataPath}");
        Debug.Log($"[DataManager] data path = {_dataFilePath}");
        LoadData();
    }

    private void LoadData()
    {
        if (File.Exists(_dataFilePath))
        {
            string json = File.ReadAllText(_dataFilePath);
            try
            {
                data = JsonUtility.FromJson<Data>(json);
            }
            catch
            {
                data = new Data();
            }
        }
        else
        {
            data = new Data();
            SaveData();
        }
    }

    public void SaveData()
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(_dataFilePath, json);
        Debug.Log($"[DataManager] Saved to {_dataFilePath}\n{json}");
    }
}
[Serializable]
public class Skin
{
    public string id;
    public bool isEarned;
}

[Serializable]
public class Level
{
    public string name;
    public bool isCompleted;

    public Level() { }

    public Level(string name, bool isCompleted)
    {
        this.name = name;
        this.isCompleted = isCompleted;
    }
}