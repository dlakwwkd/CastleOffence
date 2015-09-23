using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    static AudioManager _instance = null;
    public static AudioManager instance { get { return _instance; } }

    public AudioClip music          = null;
    public AudioClip reward         = null;
    public AudioClip build          = null;
    public AudioClip purchase       = null;
    public AudioClip unitPurchase   = null;
    public AudioClip purchaseFail   = null;
    public AudioClip incomeUp       = null;
    public AudioClip speedUp        = null;
    public AudioClip coinUp         = null;

    AudioSource _audio = null;


    void Awake()
    {
        if (_instance == null)
            _instance = this;
    }
    void Start()
    {
        _audio = GetComponent<AudioSource>();
        if (music != null)
        {
            _audio.clip = music;
            _audio.loop = true;
            _audio.Play();
        }
    }


    public void PlaySfx(AudioClip clip)
    {
        if(clip)
        {
            var obj = ObjectManager.instance.Assign("AudioSource");
            var source = obj.GetComponent<AudioSource>();

            source.clip = clip;
            source.Play();

            StartCoroutine(EndPlay(source, clip.length));
        }
    }
    public void PlaySfx(AudioClip clip, float volume)
    {
        if (clip)
        {
            var obj = ObjectManager.instance.Assign("AudioSource");
            var source = obj.GetComponent<AudioSource>();

            source.clip = clip;
            source.volume = volume * 0.2f;
            source.Play();

            StartCoroutine(EndPlay(source, clip.length));
        }
    }
    public void PlaySfx(AudioClip clip, float volume, float delay)
    {
        if (clip)
            StartCoroutine(RatePlay(clip, volume, delay));
    }

    public void PlayReward()
    {
        PlaySfx(reward, 1.0f, 0.2f);
    }
    public void PlayBuild()
    {
        _audio.PlayOneShot(build);
    }
    public void PlayPurchaseItem()
    {
        _audio.PlayOneShot(purchase, 0.5f);
    }
    public void PlayPurchaseFail()
    {
        _audio.PlayOneShot(purchaseFail, 0.5f);
    }
    public void PlayPurchaseUnit()
    {
        _audio.PlayOneShot(unitPurchase);
    }
    public void PlayIncomeUp()
    {
        _audio.PlayOneShot(incomeUp, 0.5f);
    }
    public void PlaySpeedUp()
    {
        _audio.PlayOneShot(speedUp, 0.5f);
    }
    public void PlayCoinUp()
    {
        _audio.PlayOneShot(coinUp, 0.5f);
    }


    IEnumerator EndPlay(AudioSource source, float time)
    {
        yield return new WaitForSeconds(time);
        source.Stop();
        source.clip = null;
        source.volume = 0.1f;
        ObjectManager.instance.Free(source.gameObject);
    }
    IEnumerator RatePlay(AudioClip clip, float volume, float time)
    {
        yield return new WaitForSeconds(time);
        PlaySfx(clip, volume);
    }
}