using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public AudioSource audioSource;

    public AudioClip menuMusic;
    public AudioClip[] gameplayPlaylist;

    private int currentTrackIndex = 0;
    private string gameplaySceneName = "GameScene";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayMenuMusic();
    }

    void Update()
    {
        if (
            SceneManager.GetActiveScene().name == gameplaySceneName &&
            !audioSource.isPlaying &&
            audioSource.clip != null &&
            gameplayPlaylist.Length > 0
        )
        {
            PlayNextTrack();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == gameplaySceneName)
        {
            StartGameplayMusic();
        }
        else
        {
            PlayMenuMusic();
        }
    }

    public void PlayMenuMusic()
    {
        if (audioSource.clip != menuMusic)
        {
            audioSource.clip = menuMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private void StartGameplayMusic()
    {
        currentTrackIndex = 0;
        PlayTrack(currentTrackIndex);
    }

    private void PlayTrack(int index)
    {
        if (gameplayPlaylist.Length == 0) return;

        audioSource.clip = gameplayPlaylist[index];
        audioSource.loop = false;
        audioSource.Play();
    }

    private void PlayNextTrack()
    {
        currentTrackIndex = (currentTrackIndex + 1) % gameplayPlaylist.Length;
        PlayTrack(currentTrackIndex);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
