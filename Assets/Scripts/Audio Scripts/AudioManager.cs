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

    [Header("Música por estado")]
    [SerializeField] AudioResource menuMusic;
    [SerializeField] AudioResource pauseMusic;
    [SerializeField] AudioResource deathMusic;
    [SerializeField] AudioResource victoryMusic;

    [Header("Snapshots")]
    [SerializeField] AudioMixerSnapshot menu, game, pause, death, victory;

    [Header("Filtro del escudo")]
    [SerializeField] string ambienceParam = "AmbienceCutoff";
    [SerializeField] string sfxParam = "SFXCutoff";
    [SerializeField] float cutoffAbierto = 5000f;
    [SerializeField] float cutoffAmbiente = 500f;
    [SerializeField] float cutoffSfx = 4000f;
    [SerializeField] float velocidad = 0.2f;
    float objetivo, actual;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        mixer.updateMode = AudioMixerUpdateMode.UnscaledTime;
        objetivo = actual = 0f;
    }

    void Start()
    {
        if (estadoInicial == Inicio.Menu)
        {
            playResource(menuMusic);
            EnterMenu();
        }
        else
        {
            EnterGame(0f);
        }
    }

    void playResource (AudioResource resource)
    {
        if (resource == null) return;
        music.Stop();
        music.resource = resource;
        music.Play();
    }

    // ---- Bloque B ----
    public void EnterMenu() => menu.TransitionTo(1f);

    public void EnterGame(float t = 0.3f) => game.TransitionTo(t);
    
    public void EnterPause()
    {
        playResource(pauseMusic);
        pause.TransitionTo(0.2f);
    }
    
    public void EnterDeath()
    {
        playResource(deathMusic);
        death.TransitionTo(1.5f);
    }
    
    public void EnterVictory() 
    {
        playResource(victoryMusic);
        victory.TransitionTo(1.5f);
    }

    // ---- Bloque F: lo llama Shield ----
    public void SetShieldAudioFilter(bool activo)
    {
        objetivo = activo ? 1f : 0f;
    }

    void Update()
    {
        // Avanza suavemente entre 0 (apagado) y 1 (prendido)
        actual = Mathf.MoveTowards(actual, objetivo, velocidad * Time.unscaledDeltaTime);

        // Pasa directamente de cutoffAbierto (ej. 8000 Hz) a cutoffAmbiente/Sfx (ej. 600 Hz)
        float hzAmbiente = Mathf.Lerp(cutoffAbierto, cutoffAmbiente, actual);
        float hzSfx = Mathf.Lerp(cutoffAbierto, cutoffSfx, actual);

        mixer.SetFloat(ambienceParam, hzAmbiente);
        mixer.SetFloat(sfxParam, hzSfx);
    }
}