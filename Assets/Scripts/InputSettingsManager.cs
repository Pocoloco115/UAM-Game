using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InputSettingsManager : MonoBehaviour
{
    public static InputSettingsManager Instance { get; private set; }

    public InputSettings settings = new InputSettings();
    private const string SaveKey = "InputSettings";

    private HashSet<KeyCode> allowedKeys = new HashSet<KeyCode>();

    public static InputSettingsManager GetOrCreate()
    {
        if (Instance != null) return Instance;

        Instance = Object.FindAnyObjectByType<InputSettingsManager>();
        if (Instance != null)
        {
            Instance.Initialize();
            return Instance;
        }

        GameObject go = new GameObject("[InputSettingsManager]");
        go.tag = "EditorOnly"; 
        Instance = go.AddComponent<InputSettingsManager>();
        DontDestroyOnLoad(go);
        Instance.Initialize();
        return Instance;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Initialize();
    }

    private void Initialize()
    {
        KeySprites ks = Resources.Load<KeySprites>("KeySprites");
        if (ks != null && ks.entries != null)
        {
            allowedKeys = new HashSet<KeyCode>(ks.entries.Select(e => e.key));
        }
        else
        {
            allowedKeys = new HashSet<KeyCode> { KeyCode.A, KeyCode.D, KeyCode.Space, KeyCode.LeftShift, KeyCode.W, KeyCode.UpArrow, KeyCode.Mouse0 };
        }

        LoadSettings();
    }

    public bool TrySetKey(string action, KeyCode newKey)
    {
        if (GetKeyForAction(action) == newKey)
        {
            return true;
        }

        if (!allowedKeys.Contains(newKey))
        {
            return false;

        }

        if (IsKeyUsedByAnotherAction(action, newKey))
        {
            return false;

        }

        switch (action)
        {
            case "MoveLeft": settings.moveLeft = newKey; break;
            case "MoveRight": settings.moveRight = newKey; break;
            case "Jump": settings.jump = newKey; break;
            case "Dash": settings.dash = newKey; break;
        }
        SaveSettings();
        return true;
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
        foreach (var a in new[] { "MoveLeft", "MoveRight", "Jump", "Dash" })
        {
            if (a == currentAction) continue;
            if (GetKeyForAction(a) == key) return true;
        }
        return false;
    }

    public void LoadSettings()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
        {
            SetDefaultSettings();
            return;
        }

        string json = PlayerPrefs.GetString(SaveKey);
        JsonUtility.FromJsonOverwrite(json, settings);

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
        settings.moveLeft = KeyCode.A;
        settings.moveRight = KeyCode.D;
        settings.jump = KeyCode.Space;
        settings.dash = KeyCode.LeftShift;
        SaveSettings();
    }
}
