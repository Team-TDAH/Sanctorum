using UnityEngine;

public class GameAudio : MonoBehaviour
{
    public static GameAudio Instance { get; private set; }

    [Header("Menu")]
    [SerializeField] private AK.Wwise.Event playMenuMusicEvent;
    [SerializeField] private AK.Wwise.Event stopMenuMusicEvent;

    [Header("Ambiente Cueva")]
    [SerializeField] private AK.Wwise.Event playAmbientCaveEvent;
    [SerializeField] private AK.Wwise.Event stopAmbientCaveEvent;

    [Header("Pasos")]
    [SerializeField] private AK.Wwise.Event playFootstepEvent;

    [Header("Angevin")]
    [SerializeField] private AK.Wwise.Event playBossMusicEvent;
    [SerializeField] private AK.Wwise.Event stopBossMusicEvent;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayMenuMusic();
    }

    public void PlayMenuMusic()
    {
        if (playMenuMusicEvent != null)
            playMenuMusicEvent.Post(gameObject);
    }

    public void StopMenuMusic()
    {
        if (stopMenuMusicEvent != null)
            stopMenuMusicEvent.Post(gameObject);
    }

    public void PlayAmbientCave()
    {
        if (playAmbientCaveEvent != null)
            playAmbientCaveEvent.Post(gameObject);
    }

    public void StopAmbientCave()
    {
        if (stopAmbientCaveEvent != null)
            stopAmbientCaveEvent.Post(gameObject);
    }

    public void PlayBossMusic()
    {
        if (playBossMusicEvent != null)
            playBossMusicEvent.Post(gameObject);
    }

    public void StopBossMusic()
    {
        if (stopBossMusicEvent != null)
            stopBossMusicEvent.Post(gameObject);
    }
}