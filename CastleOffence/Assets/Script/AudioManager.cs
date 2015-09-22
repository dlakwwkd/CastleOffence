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
            _audio.PlayOneShot(clip);
    }
    public void PlaySfxRate(AudioClip clip, float rate)
    {
        if (clip)
            StartCoroutine(RatePlay(clip, rate));
    }
    public void PlayReward()
    {
        StartCoroutine(RatePlay(reward, 0.2f));
    }
    public void PlayBuild()
    {
        _audio.PlayOneShot(build);
    }
    public void PlayPurchaseItem()
    {
        _audio.PlayOneShot(purchase);
    }
    public void PlayPurchaseFail()
    {
        _audio.PlayOneShot(purchaseFail);
    }
    public void PlayPurchaseUnit()
    {
        _audio.PlayOneShot(unitPurchase);
    }
    public void PlayIncomeUp()
    {
        _audio.PlayOneShot(incomeUp);
    }
    public void PlaySpeedUp()
    {
        _audio.PlayOneShot(speedUp);
    }
    public void PlayCoinUp()
    {
        _audio.PlayOneShot(coinUp);
    }


    IEnumerator RatePlay(AudioClip clip, float time)
    {
        yield return new WaitForSeconds(time);
        _audio.PlayOneShot(clip);
    }
}