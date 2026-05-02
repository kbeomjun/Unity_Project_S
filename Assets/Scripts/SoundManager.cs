using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource uiSource;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private AudioClip buttonClickClip;

    [SerializeField] private AudioClip coinClip;

    public static SoundManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlayButtonClickSound()
    {
        uiSource.PlayOneShot(buttonClickClip);
    }

    public void PlayCoinSound()
    {
        sfxSource.PlayOneShot(coinClip);
    }

}
