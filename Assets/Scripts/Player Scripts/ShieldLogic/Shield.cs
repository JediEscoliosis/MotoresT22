using UnityEngine;

public class Shield : MonoBehaviour
{
    enum State { Ready, Active, Cooldown }

    [Header("Shield")]
    [SerializeField] KeyCode activationKey = KeyCode.LeftControl;
    [SerializeField] float duration = 2f;
    [SerializeField] float cooldown = 8f;
    [SerializeField] string projectileTag = "Projectile";
    [SerializeField] GameObject visual;

    [Header("References")]
    [SerializeField] Timer timer;

    [Header("Audio")]
    [SerializeField] AudioSource activateSource;   // AudioSource con el SFX, conectado al mixer

    State state = State.Ready;
    float remaining;

    public bool IsActive => state == State.Active;

    void Awake()
    {
        visual.SetActive(false);
        timer.OnFinished += EndCooldown;
    }

    void OnDestroy()
    {
        if (timer != null) timer.OnFinished -= EndCooldown;
    }

    void Update()
    {
        if (state == State.Ready && Input.GetKeyDown(activationKey))
        {
            Activate();
        }
        else if (state == State.Active)
        {
            remaining -= Time.deltaTime;

            // Envía el progreso (1 al iniciar -> 0 al terminar) en cada frame
            float progreso = Mathf.Clamp01(remaining / duration);
            AudioManager.Instance?.SetShieldAudioFilter(true);

            if (remaining <= 0f)
                EndShield();
        }
    }

    void Activate()
    {
        state = State.Active;
        remaining = duration;
        visual.SetActive(true);

        if (activateSource != null) activateSource.Play();

        AudioManager.Instance?.SetShieldAudioFilter(true);
    }

    void EndShield()
    {
        visual.SetActive(false);
        state = State.Cooldown;
        timer.StartTimer(cooldown);

        AudioManager.Instance?.SetShieldAudioFilter(false);
    }

    void EndCooldown()
    {
        state = State.Ready;
    }

    void OnTriggerEnter(Collider other)
    {
        if (IsActive && other.CompareTag(projectileTag))
            Destroy(other.gameObject);
    }
}