using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    public enum Inicio { Menu, Game }

    [Header("Mixer y música")]
    [SerializeField] AudioMixer mixer;
    [SerializeField] AudioSource music;
    [SerializeField] Inicio estadoInicial = Inicio.Game;

    [Header("Snapshots")]
    [SerializeField] AudioMixerSnapshot menu, game, pause, death, victory;

    [Header("Filtro del escudo")]
    [SerializeField] float cutoffAbierto = 22000f;
    [SerializeField] float cutoffEscudo = 800f;
    [SerializeField] float velocidad = 40000f;

    float objetivo, actual;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        objetivo = actual = cutoffAbierto;
    }

    void Start()
    {
        if (estadoInicial == Inicio.Menu) EnterMenu();
        else EnterGame(0f);
        music.Play();
    }

    // ---- Bloque B ----
    public void EnterMenu() => menu.TransitionTo(1f);
    public void EnterGame(float t = 0.3f) => game.TransitionTo(t);
    public void EnterPause() => pause.TransitionTo(0.2f);
    public void EnterDeath() => death.TransitionTo(1.5f);
    public void EnterVictory() => victory.TransitionTo(1.5f);

    // ---- Bloque F: lo llama Shield ----
    public void SetShieldAudioFilter(bool activo, float ratio)
    {
        objetivo = activo ? Mathf.Lerp(cutoffAbierto, cutoffEscudo, ratio) : cutoffAbierto;
    }

    void Update()
    {
        actual = Mathf.MoveTowards(actual, objetivo, velocidad * Time.unscaledDeltaTime);
        mixer.SetFloat("AmbienteCutoff", actual);
    }
}