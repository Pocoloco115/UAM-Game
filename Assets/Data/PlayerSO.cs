using UnityEngine;

public enum UnlockType
{
    StartUnlocked,   // desbloqueada desde inicio
    Purchase,        // se compra con monedas
    CompleteLevel    // se desbloquea al completar un nivel
}

[CreateAssetMenu(fileName = "PlayerSO", menuName = "Scriptable Objects/PlayerSO")]
public class PlayerSO : ScriptableObject
{
    public string id;
    public string playerName;
    public GameObject playerPrefab;
    public Sprite menuSprite;

    public UnlockType unlockType;

    public int price;                 // si unlockType == Purchase
    public string requiredLevelName;  // si unlockType == CompleteLevel
}
