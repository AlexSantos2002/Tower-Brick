using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public AudioSource audioSource;

    public AudioClip menuMusic;
    public AudioClip[] gameplayPlaylist;

    private List<AudioClip> shuffledPlaylist = new List<AudioClip>();
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
            shuffledPlaylist.Count > 0
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
        shuffledPlaylist = new List<AudioClip>(gameplayPlaylist);
        Shuffle(shuffledPlaylist);
        currentTrackIndex = Random.Range(0, shuffledPlaylist.Count);
        PlayTrack(currentTrackIndex);
    }

    private void PlayTrack(int index)
    {
        if (shuffledPlaylist.Count == 0) return;

        audioSource.clip = shuffledPlaylist[index];
        audioSource.loop = false;
        audioSource.Play();
    }

    private void PlayNextTrack()
    {
        currentTrackIndex = (currentTrackIndex + 1) % shuffledPlaylist.Count;
        PlayTrack(currentTrackIndex);
    }

    private void Shuffle(List<AudioClip> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randIndex = Random.Range(i, list.Count);
            AudioClip temp = list[i];
            list[i] = list[randIndex];
            list[randIndex] = temp;
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}