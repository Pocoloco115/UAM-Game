using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("SFX")]
    [SerializeField] private AudioClip clickSFX;

    [Header("Music Playlist")]
    [SerializeField] private AudioClip[] musicClips;

    private AudioClip lastClipPlayed;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        // Solo reproducir música en Level1
        if (SceneManager.GetActiveScene().name == "Level1")
        {
            StopAllCoroutines();
            StartCoroutine(PlayMusicLoop());
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu" || scene.name == "SettingsMenu")
        {
            // Detener música y corrutinas en menús
            StopAllCoroutines();
            musicSource.Stop();
        }
        else if (scene.name == "Level1")
        {
            // En Level1 asegurarse que la música esté corriendo
            if (!musicSource.isPlaying)
            {
                StopAllCoroutines();
                StartCoroutine(PlayMusicLoop());
            }
        }
    }

    // ----------- SFX METHODS -------------

    public void PlayClickSFX()
    {
        if (clickSFX != null)
            sfxSource.PlayOneShot(clickSFX);
    }

    // ----------- MUSIC METHODS -------------

    private IEnumerator PlayMusicLoop()
    {
        while (true)
        {
            AudioClip nextClip = GetRandomMusicClip();
            if (nextClip == null)
                yield break;

            lastClipPlayed = nextClip;
            musicSource.clip = nextClip;
            musicSource.Play();

            // Espera hasta que termine la canción
            while (musicSource.isPlaying)
                yield return null;
        }
    }

    private AudioClip GetRandomMusicClip()
    {
        if (musicClips.Length == 0)
            return null;

        if (musicClips.Length == 1)
            return musicClips[0];

        AudioClip chosen = musicClips[Random.Range(0, musicClips.Length)];

        while (chosen == lastClipPlayed)
        {
            chosen = musicClips[Random.Range(0, musicClips.Length)];
        }

        return chosen;
    }
}

