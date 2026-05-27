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