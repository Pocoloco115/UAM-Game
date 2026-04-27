using UnityEngine;

public enum UnlockType
{
    StartUnlocked,   
    Purchase,       
    CompleteLevel   
}

[CreateAssetMenu(fileName = "PlayerSO", menuName = "Scriptable Objects/PlayerSO")]
public class PlayerSO : ScriptableObject
{
    public string id;
    public string playerName;
    public GameObject playerPrefab;
    public Sprite menuSprite;

    public UnlockType unlockType;

    public int price;                 
    public string requiredLevelName;  
}
