using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [Header("Efeitos Sonoros")]
    public AudioClip gameOverSound;
    public AudioClip blockLandingSound;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayGameOverSound()
    {
        Debug.Log("[SFXManager] TOCANDO SOM DE GAME OVER!");
        if (gameOverSound != null)
        {
            audioSource.PlayOneShot(gameOverSound, 1.5f);
        }
        else
        {
            Debug.LogWarning("GameOverSound não está atribuído no SFXManager.");
        }
    }

    public void PlayBlockLandingSound()
{
    if (blockLandingSound != null)
    {
        audioSource.PlayOneShot(blockLandingSound);
    }
}
}