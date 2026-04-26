using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SkinsManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerSO[] playerSO;

    [Header("UI")]
    [SerializeField] private Image skinImage;

    [SerializeField] private Button purchaseSelectButton;
    [SerializeField] private TMPro.TextMeshProUGUI purchaseSelectText;
    [SerializeField] private TMPro.TextMeshProUGUI priceText;
    [SerializeField] private TMPro.TextMeshProUGUI wallet;

    [Header("Locked Visuals")]
    [SerializeField] private CanvasGroup skinPreviewGroup;
    [SerializeField] private GameObject lockOverlay;
    [SerializeField, Range(0f, 1f)] private float lockedAlpha = 0.4f;

    private int actualIndex = 0;

    void Start()
    {
        EnsureSkinsExistInSave();
        wallet.text = DataManager.Instance.data.currentCoins.ToString();
        string currentId = DataManager.Instance.data.selectedSkinId;
        actualIndex = System.Array.FindIndex(playerSO, s => s.id == currentId);

        if (actualIndex == -1 || !IsUnlocked(playerSO[actualIndex]))
        {
            actualIndex = System.Array.FindIndex(playerSO, s => s.unlockType == UnlockType.StartUnlocked);
            if (actualIndex == -1) actualIndex = 0;

            DataManager.Instance.data.selectedSkinId = playerSO[actualIndex].id;
            DataManager.Instance.SaveData();
        }

        UpdateSkin();
    }

    public void LeftButton()
    {
        actualIndex--;
        if (actualIndex < 0) actualIndex = playerSO.Length - 1;
        UpdateSkin();
    }

    public void RightButton()
    {
        actualIndex++;
        if (actualIndex >= playerSO.Length) actualIndex = 0;
        UpdateSkin();
    }

    public void SaveSkin()
    {
        var so = playerSO[actualIndex];
        if (!IsUnlocked(so)) return;

        PlayerPrefs.SetInt("SkinIndex", actualIndex);
        PlayerPrefs.Save();
    }

    public void OnPurchaseOrSelect()
    {
        var so = playerSO[actualIndex];

        if (IsUnlocked(so))
        {
            DataManager.Instance.data.selectedSkinId = so.id;
            DataManager.Instance.SaveData();
            UpdateSkin();
            return;
        }

        if (so.unlockType != UnlockType.Purchase) return;

        bool purchased = GameData.Instance.PurchaseSkin(so.id, so.price);
        UpdateWallet();
        if (!purchased) return;

        PlayerPrefs.SetInt("SkinIndex", actualIndex);
        PlayerPrefs.Save();
        UpdateSkin();
    }

    private void UpdateSkin()
    {
        var so = playerSO[actualIndex];
        skinImage.sprite = so.menuSprite;

        UpdateButton();
        UpdateLockedVisual();
    }
    private void UpdateWallet()
    {
        wallet.text = DataManager.Instance.data.currentCoins.ToString();
    }
    private void UpdateButton()
    {
        var so = playerSO[actualIndex];

        if (IsUnlocked(so))
        {
            purchaseSelectText.text = "Select";
            if (priceText != null) priceText.text = "";
            purchaseSelectButton.interactable = true;
            return;
        }
        if (so.unlockType == UnlockType.Purchase)
        {
            purchaseSelectText.text = "Purchase";
            if (priceText != null) priceText.text = so.price.ToString();
            purchaseSelectButton.interactable = true;
        }
        else if (so.unlockType == UnlockType.CompleteLevel)
        {
            purchaseSelectText.text = $"Beat {so.requiredLevelName}";
            if (priceText != null) priceText.text = "";
            purchaseSelectButton.interactable = false;
        }
        else 
        {
            purchaseSelectText.text = "Select";
            if (priceText != null) priceText.text = "";
            purchaseSelectButton.interactable = true;
        }
    }

    private void UpdateLockedVisual()
    {
        var so = playerSO[actualIndex];
        bool locked = !IsUnlocked(so);

        if (lockOverlay != null)
            lockOverlay.SetActive(locked);

        if (skinPreviewGroup != null)
        {
            skinPreviewGroup.alpha = locked ? lockedAlpha : 1f;
        }
        else
        {
            skinImage.color = locked
                ? new Color(1f, 1f, 1f, lockedAlpha)
                : new Color(1f, 1f, 1f, 1f);
        }
    }

    private bool IsUnlocked(PlayerSO so)
    {
        var entry = DataManager.Instance.data.skins.FirstOrDefault(x => x.id == so.id);
        if (entry != null && entry.isEarned) return true;

        if (so.unlockType == UnlockType.StartUnlocked) return true;

        if (so.unlockType == UnlockType.CompleteLevel)
        {
            var lvl = DataManager.Instance.data.levelsCompleted
                .FirstOrDefault(x => x.name == so.requiredLevelName);

            if (lvl != null && lvl.isCompleted)
                return true;
        }

        return false;
    }

    private void EnsureSkinsExistInSave()
    {
        if (DataManager.Instance.data.skins == null)
            DataManager.Instance.data.skins = new System.Collections.Generic.List<Skin>();

        DataManager.Instance.data.skins.RemoveAll(s => string.IsNullOrWhiteSpace(s.id));
        foreach (var s in DataManager.Instance.data.skins) s.id = s.id.Trim();

        var validIds = playerSO
            .Where(so => so != null && !string.IsNullOrWhiteSpace(so.id))
            .Select(so => so.id.Trim())
            .ToHashSet();

        int beforeCount = DataManager.Instance.data.skins.Count;
        DataManager.Instance.data.skins.RemoveAll(s => !validIds.Contains(s.id));
        bool changed = (DataManager.Instance.data.skins.Count != beforeCount);

        foreach (var so in playerSO)
        {
            if (so == null || string.IsNullOrWhiteSpace(so.id)) continue;

            string id = so.id.Trim();
            var entry = DataManager.Instance.data.skins.FirstOrDefault(x => x.id == id);

            if (entry == null)
            {
                bool startUnlocked = (so.unlockType == UnlockType.StartUnlocked);
                DataManager.Instance.data.skins.Add(new Skin { id = id, isEarned = startUnlocked });
                changed = true;
            }
            else
            {
                if (so.unlockType == UnlockType.StartUnlocked && !entry.isEarned)
                {
                    entry.isEarned = true;
                    changed = true;
                }
            }
        }

        if (changed)
        {
            DataManager.Instance.SaveData();
        }
    }
}