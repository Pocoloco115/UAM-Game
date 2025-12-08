using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class KeyBindItem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image keyIcon;
    [SerializeField] private Image arrow;

    [Header("Default Sprite")]
    [SerializeField] private Sprite defaultKeySprite;

    [SerializeField] private KeySprites keySprites;

    [Header("Action Name")]
    [SerializeField] private string actionName;

    private bool waitingForKey = false;

    public void Initialize()
    {
        UpdateKeyDisplay();               
        if (arrow != null)
        {
            arrow.gameObject.SetActive(false);
        }
    }

    public void UpdateKeyDisplay()
    {
        KeyCode currentKey = GetCurrentKey();
        Sprite sprite = keySprites.GetSprite(currentKey);
        if (keyIcon != null)
        {
            keyIcon.sprite = sprite != null ? sprite : defaultKeySprite;
            keyIcon.enabled = true;
        }
    }

    private KeyCode GetCurrentKey()
    {
        if (InputSettingsManager.Instance == null) return KeyCode.None;

        return actionName switch
        {
            "MoveLeft" => InputSettingsManager.Instance.settings.moveLeft,
            "MoveRight" => InputSettingsManager.Instance.settings.moveRight,
            "Jump" => InputSettingsManager.Instance.settings.jump,
            "Dash" => InputSettingsManager.Instance.settings.dash,
            _ => KeyCode.None
        };
    }

    public bool IsWaitingForKey() => waitingForKey;

    public void SetArrowActive(bool active)
    {
        if (arrow != null)
            arrow.gameObject.SetActive(active);
    }

    public void StartRebind()
    {
        if (!waitingForKey)
            StartCoroutine(RebindCoroutine());
    }

    private IEnumerator RebindCoroutine()
    {
        waitingForKey = true;
        StartCoroutine(BlinkSprite());

        yield return null; 

        while (waitingForKey)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    bool success = InputSettingsManager.Instance.TrySetKey(actionName, key);

                    if (success)
                    {
                        foreach (var item in Object.FindAnyObjectByType<SettingsMenu>().menuItems)
                        {
                            item.UpdateKeyDisplay();
                        }
                    }
                    waitingForKey = false;
                    keyIcon.enabled = true;
                    yield break;
                }
            }
            yield return null;
        }
    }
    private IEnumerator BlinkSprite()
    {
        bool visible = true;
        while (waitingForKey)
        {
            keyIcon.enabled = visible;
            visible = !visible;
            yield return new WaitForSeconds(0.35f);
        }
        keyIcon.enabled = true;
    }
}