using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Collectible : MonoBehaviour
{
    [Header("Al recogerlo")]
    [SerializeField] private AudioClip pickupSfx;
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;

    [SerializeField] private AudioClip voiceLine;
    [SerializeField, Range(0f, 1f)] private float voiceVolume = 1f;
    [SerializeField] private float voiceDelay = 0.1f;    // pausa entre el SFX y la voz

    [Header("Animación")]
    [SerializeField] private float rotationSpeed = 90f;   // 360° cada 4 segundos
    [SerializeField] private float floatHeight = 0.15f;   // sube de 0 a 0.15
    [SerializeField] private float floatPeriod = 2f;      // un sube y baja completo cada 2 segundos
    [SerializeField] private bool randomizeStart = false; // true = cada uno arranca en un momento distinto

    [Header("Mixer")]
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioMixerGroup voiceGroup;
    // Compartidas por TODOS los coleccionables.
    private static readonly List<AudioSource> activeSources = new List<AudioSource>();
    private static GameObject currentAudio;

    private bool collected;
    private Vector3 startPosition;
    private float timer;

    private void Start()
    {
        // Cada instancia guarda SU posición, donde la colocaste en el nivel.
        startPosition = transform.position;

        if (randomizeStart)
            timer = Random.value * floatPeriod;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // Giro continuo en Y: 90°/s = 360° en 4 s
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.World);

        // Flote suave: 0 -> floatHeight -> 0 en cada período
        float t = timer / floatPeriod * Mathf.PI * 2f;
        float offset = floatHeight * 0.5f * (1f - Mathf.Cos(t));
        transform.position = startPosition + Vector3.up * offset;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected || !other.CompareTag("Player")) return;
        collected = true;

        CollectibleManager.Instance.AddCollectible();

        PlayPickupSounds();

        Destroy(gameObject);
    }

    // Corta cualquier audio de coleccionables anteriores.
    private static void StopAllCollectibleAudio()
    {
        foreach (AudioSource src in activeSources)
        {
            if (src != null)
                src.Stop();          // corta lo que suena y cancela lo programado
        }
        activeSources.Clear();

        if (currentAudio != null)
        {
            Destroy(currentAudio);
            currentAudio = null;
        }
    }

    // Los sonidos salen de un objeto temporal, así no se cortan cuando se destruye el coleccionable.
    private void PlayPickupSounds()
    {
        // Primero cortamos lo que haya sonando de un coleccionable anterior.
        StopAllCollectibleAudio();

        float sfxLength = pickupSfx != null ? pickupSfx.length : 0f;
        float voiceLength = voiceLine != null ? voiceLine.length : 0f;

        GameObject temp = new GameObject("CollectibleAudio");
        temp.transform.position = transform.position;
        currentAudio = temp;

        double start = AudioSettings.dspTime + 0.05;

        if (pickupSfx != null)
        {
            AudioSource sfx = temp.AddComponent<AudioSource>();
            sfx.clip = pickupSfx;
            sfx.volume = sfxVolume;
            sfx.spatialBlend = 0f;                            // 2D, se oye igual en todos lados
            sfx.outputAudioMixerGroup = sfxGroup;             //Asignación a su respectivo mixer
            sfx.PlayScheduled(start);
            activeSources.Add(sfx);
        }

        if (voiceLine != null)
        {
            AudioSource voice = temp.AddComponent<AudioSource>();
            voice.clip = voiceLine;
            voice.volume = voiceVolume;
            voice.spatialBlend = 0f;
            voice.outputAudioMixerGroup = voiceGroup;           //Asignación a su respectivo mixer
            voice.PlayScheduled(start + sfxLength + voiceDelay);
            activeSources.Add(voice);
        }

        Destroy(temp, 0.05f + sfxLength + voiceDelay + voiceLength + 0.5f);
    }
}