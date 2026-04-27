using UnityEngine;
using System.Linq;
public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }
    public int coinsCollected { get; set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void AddCoins(int amount)
    {
        coinsCollected += amount;
        DataManager.Instance.data.currentCoins += amount;
        DataManager.Instance.SaveData();
        Debug.Log($"[GameData] Saved coins = {DataManager.Instance.data.currentCoins}");
    }
    public void RemoveCoins(int amount)
    {
        coinsCollected -= amount;
        DataManager.Instance.data.currentCoins -= amount;
        DataManager.Instance.SaveData();
    }
    public bool PurchaseSkin(string id, int price)
    {
        var skin = DataManager.Instance.data.skins.FirstOrDefault(x => x.id == id);
        if (skin == null)
        {
            skin = new Skin { id = id, isEarned = false };
            DataManager.Instance.data.skins.Add(skin);
        }

        if (skin.isEarned) return true;

        if (DataManager.Instance.data.currentCoins < price) return false;

        DataManager.Instance.data.currentCoins -= price;
        skin.isEarned = true;
        DataManager.Instance.SaveData();
        return true;
    }
    public void CompleteLevel(string name)
    {
        var level = DataManager.Instance.data.levelsCompleted.Find(x => x.name == name);
        if (level == null)
        {
            level = new Level { name = name, isCompleted = true };
            DataManager.Instance.data.levelsCompleted.Add(level);
            return;
        }
        level.isCompleted = true;
        DataManager.Instance.SaveData();
    }
}
