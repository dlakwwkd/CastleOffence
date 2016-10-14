using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    //-----------------------------------------------------------------------------------
    // property
    public static AudioManager instance { get; private set; }

    //-----------------------------------------------------------------------------------
    // inspector field
    public AudioClip music          = null;
    public AudioClip reward         = null;
    public AudioClip build          = null;
    public AudioClip purchase       = null;
    public AudioClip unitPurchase   = null;
    public AudioClip purchaseFail   = null;
    public AudioClip incomeUp       = null;
    public AudioClip speedUp        = null;
    public AudioClip coinUp         = null;

    //-----------------------------------------------------------------------------------
    // handler functions
    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (music)
        {
            audioSource.clip = music;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    //-----------------------------------------------------------------------------------
    // public functions
    public void PlaySfx(AudioClip clip)
    {
        if (clip)
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
        {
            StartCoroutine(RatePlay(clip, volume, delay));
        }
    }

    public void PlayReward() { PlaySfx(reward, 1.0f, 0.2f); }
    public void PlayBuild() { PlaySfx(build, 3.0f); }
    public void PlayPurchaseItem() { audioSource.PlayOneShot(purchase, 0.5f); }
    public void PlayPurchaseFail() { audioSource.PlayOneShot(purchaseFail, 0.5f); }
    public void PlayPurchaseUnit() { audioSource.PlayOneShot(unitPurchase); }
    public void PlayIncomeUp() { audioSource.PlayOneShot(incomeUp, 0.5f); }
    public void PlaySpeedUp() { audioSource.PlayOneShot(speedUp, 0.5f); }
    public void PlayCoinUp() { audioSource.PlayOneShot(coinUp, 0.5f); }

    //-----------------------------------------------------------------------------------
    // coroutine functions
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

    //-----------------------------------------------------------------------------------
    // private field
    AudioSource audioSource = null;
}