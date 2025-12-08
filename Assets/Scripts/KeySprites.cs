using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "KeySprites", menuName = "Input/KeySprites")]
public class KeySprites : ScriptableObject
{
    public List<KeySpriteEntry> entries;

    private Dictionary<KeyCode, Sprite> keySpriteDict;

    public void Initialize()
    {
        keySpriteDict = new Dictionary<KeyCode, Sprite>();
        foreach (var entry in entries)
        {
            keySpriteDict[entry.key] = entry.sprite;
        }
    }

    public Sprite GetSprite(KeyCode key)
    {
        if (keySpriteDict == null) Initialize();
        if (keySpriteDict.ContainsKey(key))
        {
            return keySpriteDict[key];
        }
        return null;
    }
}

[System.Serializable]
public class KeySpriteEntry
{
    public KeyCode key;
    public Sprite sprite;
}
