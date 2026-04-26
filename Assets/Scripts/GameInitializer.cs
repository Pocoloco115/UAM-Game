using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private PlayerSO[] skins;
    [SerializeField] private PlayerSO defaultSkin;
    [SerializeField] private Transform playerSpawnPoint;

    void Start()
    {
        string selectedId = DataManager.Instance.data.selectedSkinId;

        PlayerSO skinToSpawn = System.Array.Find(skins, s => s.id == selectedId);

        if (skinToSpawn == null || !IsSkinOwned(selectedId))
        {
            Debug.LogWarning($"[GameInitializer] Skin {selectedId} not owned or not found. Spawning default.");
            skinToSpawn = defaultSkin;

            DataManager.Instance.data.selectedSkinId = defaultSkin.id;
            DataManager.Instance.SaveData();
        }

        if (skinToSpawn != null && skinToSpawn.playerPrefab != null)
        {
            Instantiate(skinToSpawn.playerPrefab, playerSpawnPoint.position, Quaternion.identity);
        }
    }

    private bool IsSkinOwned(string id)
    {
        var entry = DataManager.Instance.data.skins.Find(x => x.id == id);
        return entry != null && entry.isEarned;
    }
}
