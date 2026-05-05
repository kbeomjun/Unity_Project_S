using UnityEngine;

public enum BGMType
{
    Start,
    Battle,
    Elite,
    Town,
    Rest,
    Event,
    Boss
}

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioSource _uiSource;

    [SerializeField] private AudioClip _startNodeClip;
    [SerializeField] private AudioClip _townNodeClip;

    [SerializeField] private AudioClip[] _attackClips;
    [SerializeField] private AudioClip _defenseClip;
    [SerializeField] private AudioClip _hitClip;
    [SerializeField] private AudioClip _blockClip;

    [SerializeField] private AudioClip _buttonClickClip;
    [SerializeField] private AudioClip _mapButtonClickClip;
    [SerializeField] private AudioClip _coinClip;

    public static SoundManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlayBGM(BGMType type)
    {
        AudioClip clip = null;

        switch (type)
        {
            case BGMType.Start:
                clip = _startNodeClip;
                break;
            case BGMType.Town:
                clip = _townNodeClip;
                break;
        }

        if (_bgmSource.clip == clip && _bgmSource.isPlaying) return;

        _bgmSource.clip = clip;
        _bgmSource.loop = true;
        _bgmSource.Play();
    }

    public void StopBGM()
    {
        _bgmSource.Stop();
    }

    public void PlayAttackSound(UnitType type)
    {
        _sfxSource.PlayOneShot(_attackClips[(int)type]);
    }

    public void PlayDefenseSound()
    {
        _sfxSource.PlayOneShot(_defenseClip);
    }

    public void PlayHitSound()
    {
        _sfxSource.PlayOneShot(_hitClip);
    }

    public void PlayBlockSound()
    {
        _sfxSource.PlayOneShot(_blockClip);
    }

    public void PlayButtonClickSound()
    {
        _uiSource.PlayOneShot(_buttonClickClip);
    }

    public void PlayMapButtonClickSound()
    {
        _uiSource.PlayOneShot(_mapButtonClickClip);
    }

    public void PlayCoinSound()
    {
        _uiSource.PlayOneShot(_coinClip);
    }

}
