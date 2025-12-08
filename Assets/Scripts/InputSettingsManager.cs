using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InputSettingsManager : MonoBehaviour
{
    public static InputSettingsManager Instance;
    public InputSettings settings;
    private const string SaveKey = "InputSettings";

    [SerializeField] private KeySprites keySprites;
    private HashSet<KeyCode> allowedKeys;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            allowedKeys = keySprites != null
                ? new HashSet<KeyCode>(keySprites.entries.Select(e => e.key))
                : new HashSet<KeyCode>();

            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool TrySetKey(string action, KeyCode newKey)
    {
        if (GetKeyForAction(action) == newKey)
        {
            SetKeyUnsafe(action, newKey);
            return true;
        }

        if (!allowedKeys.Contains(newKey))
            return false;

        if (IsKeyUsedByAnotherAction(action, newKey))
            return false;

        SetKeyUnsafe(action, newKey);
        return true;
    }

    private void SetKeyUnsafe(string action, KeyCode key)
    {
        switch (action)
        {
            case "MoveLeft": settings.moveLeft = key; break;
            case "MoveRight": settings.moveRight = key; break;
            case "Jump": settings.jump = key; break;
            case "Dash": settings.dash = key; break;
        }
        SaveSettings();
    }

    public KeyCode GetKeyForAction(string action)
    {
        return action switch
        {
            "MoveLeft" => settings.moveLeft,
            "MoveRight" => settings.moveRight,
            "Jump" => settings.jump,
            "Dash" => settings.dash,
            _ => KeyCode.None
        };
    }

    private bool IsKeyUsedByAnotherAction(string currentAction, KeyCode key)
    {
        foreach (var action in new[] { "MoveLeft", "MoveRight", "Jump", "Dash" })
        {
            if (action == currentAction) continue;
            if (GetKeyForAction(action) == key) return true;
        }
        return false;
    }

    public void LoadSettings()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
        {
            SetDefaultSettings();
            SaveSettings();
            return;
        }

        string json = PlayerPrefs.GetString(SaveKey);
        settings = JsonUtility.FromJson<InputSettings>(json);
        ValidateAllKeys();
    }

    private void ValidateAllKeys()
    {
        bool changed = false;

        if (!allowedKeys.Contains(settings.moveLeft)) { settings.moveLeft = KeyCode.A; changed = true; }
        if (!allowedKeys.Contains(settings.moveRight)) { settings.moveRight = KeyCode.D; changed = true; }
        if (!allowedKeys.Contains(settings.jump)) { settings.jump = KeyCode.Space; changed = true; }
        if (!allowedKeys.Contains(settings.dash)) { settings.dash = KeyCode.LeftShift; changed = true; }

        if (changed) SaveSettings();
    }

    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(settings);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    public void SetDefaultSettings()
    {
        settings = new InputSettings
        {
            moveLeft = KeyCode.A,
            moveRight = KeyCode.D,
            jump = KeyCode.Space,
            dash = KeyCode.LeftShift
        };
        SaveSettings();
    }
}
