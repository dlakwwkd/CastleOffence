using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    //-----------------------------------------------------------------------------------
    // property
    public static AudioManager instance { get; private set; }

    //-----------------------------------------------------------------------------------
    // inspector field
    public AudioClip Music          = null;
    public AudioClip Reward         = null;
    public AudioClip Build          = null;
    public AudioClip Purchase       = null;
    public AudioClip UnitPurchase   = null;
    public AudioClip PurchaseFail   = null;
    public AudioClip IncomeUp       = null;
    public AudioClip SpeedUp        = null;
    public AudioClip CoinUp         = null;

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
        if (Music)
        {
            audioSource.clip = Music;
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

    public void PlayReward() { PlaySfx(Reward, 1.0f, 0.2f); }
    public void PlayBuild() { PlaySfx(Build, 3.0f); }
    public void PlayPurchaseItem() { audioSource.PlayOneShot(Purchase, 0.5f); }
    public void PlayPurchaseFail() { audioSource.PlayOneShot(PurchaseFail, 0.5f); }
    public void PlayPurchaseUnit() { audioSource.PlayOneShot(UnitPurchase); }
    public void PlayIncomeUp() { audioSource.PlayOneShot(IncomeUp, 0.5f); }
    public void PlaySpeedUp() { audioSource.PlayOneShot(SpeedUp, 0.5f); }
    public void PlayCoinUp() { audioSource.PlayOneShot(CoinUp, 0.5f); }

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