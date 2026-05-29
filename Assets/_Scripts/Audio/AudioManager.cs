using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music")]
    [SerializeField] private AudioClip[] tracks;
    [SerializeField] private bool shuffle = true;
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.5f;

    [Header("SFX")]
    [SerializeField] private AudioClip buttonClickClip;
    [SerializeField] private AudioClip flyClip;
    [SerializeField] private AudioClip hurtClip;
    [Tooltip("Walking enemies pick one of these at random when they spawn.")]
    [SerializeField] private AudioClip[] enemyNoises;
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;
    private AudioSource sfxSource;

    private AudioSource source;
    private readonly List<int> order = new List<int>();
    private int orderIndex;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);     // singleton guard (no duplicate when returning to menu)
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        // Dedicated source for one-shot SFX (separate from music so it never interrupts the track)
    sfxSource = gameObject.AddComponent<AudioSource>();
    sfxSource.playOnAwake = false;

        source = GetComponent<AudioSource>();
        source.loop = false;          // we advance the playlist manually
        source.playOnAwake = false;
        source.volume = musicVolume;
    }

    void Start()
    {
        if (tracks.Length == 0) return;
        BuildOrder();
        StartCoroutine(PlayLoop());
    }

    void Update()
    {
        source.volume = musicVolume;  // live volume tuning in play mode
    }

    IEnumerator PlayLoop()
    {
        while (true)
        {
            AudioClip clip = tracks[order[orderIndex]];
            source.clip = clip;
            source.Play();
            yield return new WaitForSecondsRealtime(clip.length);
            Advance();
        }
    }

    public void PlaySFX(AudioClip clip, float volumeScale = 1f)
{
    if (clip == null || sfxSource == null) return;
    sfxSource.PlayOneShot(clip, sfxVolume * volumeScale);
}

public void PlayButtonClick()
{
    PlaySFX(buttonClickClip);
}

public void PlayFly()  => PlaySFX(flyClip);
public void PlayHurt() => PlaySFX(hurtClip);

public void PlayEnemyNoise()
{
    if (enemyNoises == null || enemyNoises.Length == 0) return;
    PlaySFX(enemyNoises[Random.Range(0, enemyNoises.Length)]);
}
    void Advance()
    {
        orderIndex++;
        if (orderIndex >= order.Count)
            BuildOrder();             // reshuffle and restart the playlist
    }

    void BuildOrder()
    {
        order.Clear();
        for (int i = 0; i < tracks.Length; i++) order.Add(i);

        if (shuffle)
        {
            for (int i = order.Count - 1; i > 0; i--)   // Fisher–Yates shuffle
            {
                int j = Random.Range(0, i + 1);
                (order[i], order[j]) = (order[j], order[i]);
            }
        }
        orderIndex = 0;
    }
}