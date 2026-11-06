using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer y Snapshots (Bloques A y B)")]
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private AudioMixerSnapshot snapshotMenu;
    [SerializeField] private AudioMixerSnapshot snapshotGameplay;
    [SerializeField] private AudioMixerSnapshot snapshotPausa;
    [SerializeField] private AudioMixerSnapshot snapshotMuerte;

    [Header("Parámetros Expuestos del Mixer (Bloque F)")]
    [SerializeField] private string ambienceCutoffParam = "AmbienceCutoff";
    [SerializeField] private string sfxPitchParam = "ManipulatorPitch";

    [Header("Fuentes 2D Ruteadas (Bloques B, C, D y E)")]
    [SerializeField] private AudioSource musicSource;     // Output -> Música
    [SerializeField] private AudioSource ambienceSource;  // Output -> Ambiente
    [SerializeField] private AudioSource sfxSource;       // Output -> SFX
    [SerializeField] private AudioSource voiceSource;     // Output -> Voz

    private Coroutine voiceCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMenuMusic(null);
    }

    // =========================================================================
    // BLOQUE F: PROGRAMACIÓN DINÁMICA CON LÓGICA CONDICIONAL Y 2 PARÁMETROS
    // =========================================================================

    /// <summary>
    /// Parámetro 1 (AmbienceCutoff): Filtro paso bajo reactivo en Ambiente según el escudo.
    /// </summary>
    public void SetShieldAudioFilter(bool isShieldActive, float remainingRatio)
    {
        if (isShieldActive && remainingRatio > 0f)
        {
            // Lógica condicional: si la energía cae por debajo del 20%, amortiguación extrema
            float cutoffValue;
            if (remainingRatio < 0.2f)
                cutoffValue = Mathf.Lerp(300f, 600f, remainingRatio * 5f);
            else
                cutoffValue = Mathf.Lerp(600f, 1500f, remainingRatio);

            masterMixer.SetFloat(ambienceCutoffParam, cutoffValue);
        }
        else
        {
            // Sin escudo: espectro audible completo
            masterMixer.SetFloat(ambienceCutoffParam, 22000f);
        }
    }
    /*
    public void PlayPuzzleOrTransitionMusic(AudioClip cancionVictoriaOTransicion, float fadeDuration = 1.5f)
    {
        if (cancionVictoriaOTransicion != null && musicSource != null)
        {
            musicSource.clip = cancionVictoriaOTransicion;
            musicSource.Play();
        }

        // Transición de 1.5s elegida a propósito para el clímax del evento
        snapshotPuzzle?.TransitionTo(fadeDuration);
    }*/

    /// <summary>
    /// Parámetro 2 (ManipulatorPitch): Modulación dinámica de pitch en SFX según la interacción física.
    /// </summary>
    public void SetManipulatorPitch(bool isRepelling, bool isAttracting, float objectVelocity)
    {
        float targetPitch = 1.0f;

        if (isRepelling)
        {
            targetPitch = 1.35f; // Tono agudo de expulsión
        }
        else if (isAttracting)
        {
            // Modula de forma continua según la velocidad física real del objeto acercándose
            float pitchOffset = Mathf.Clamp(objectVelocity * 0.05f, 0f, 0.4f);
            targetPitch = 0.75f + pitchOffset; // Rango dinámico: 0.75 a 1.15
        }

        masterMixer.SetFloat(sfxPitchParam, targetPitch);
    }

    /// <summary>
    /// Lógica Bloque F extra: TransitionTo con tiempo calculado matemáticamente.
    /// </summary>
    public void TransitionToGameplayDynamic(float playerSpeed, float maxSpeed)
    {
        // El tiempo de fade no es fijo: varía entre 0.3s y 1.2s según la velocidad del jugador
        float normalizedSpeed = Mathf.Clamp01(playerSpeed / Mathf.Max(0.1f, maxSpeed));
        float calculatedFadeTime = Mathf.Lerp(1.2f, 0.3f, normalizedSpeed);

        snapshotGameplay?.TransitionTo(calculatedFadeTime);
    }

    // =========================================================================
    // BLOQUE B: TRANSICIONES Y ESTADOS DE MÚSICA
    // =========================================================================

    public void PlayMenuMusic(AudioClip clip)
    {
        if (clip != null) { musicSource.clip = clip; musicSource.Play(); }
        snapshotMenu?.TransitionTo(0.5f);
    }

    public void SetPauseState(bool isPaused)
    {
        if (isPaused)
            snapshotPausa?.TransitionTo(0.15f);
        else
            snapshotGameplay?.TransitionTo(0.2f);
    }

    public void TriggerDeathMusic(AudioClip deathClip = null)
    {
        if (deathClip != null) { musicSource.clip = deathClip; musicSource.Play(); }
        snapshotMuerte?.TransitionTo(0.8f);
    }
    /*
    public void TriggerPuzzleCompleted()
    {
        snapshotPuzzle?.TransitionTo(2.0f);
    }*/

    // =========================================================================
    // BLOQUES C, D y E: SFX, VOCES Y AMBIENTE
    // =========================================================================

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null) sfxSource.PlayOneShot(clip);
    }

    public void PlayVoiceLine(AudioClip clip)
    {
        if (clip != null && voiceSource != null) voiceSource.PlayOneShot(clip);
    }

    public void PlaySFXThenVoice(AudioClip sfx, AudioClip voice)
    {
        if (voiceCoroutine != null) StopCoroutine(voiceCoroutine);
        voiceCoroutine = StartCoroutine(SequenceRoutine(sfx, voice));
    }

    private IEnumerator SequenceRoutine(AudioClip sfx, AudioClip voice)
    {
        if (sfx != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(sfx);
            yield return new WaitForSeconds(sfx.length);
        }
        if (voice != null && voiceSource != null)
        {
            voiceSource.PlayOneShot(voice);
        }
    }
}