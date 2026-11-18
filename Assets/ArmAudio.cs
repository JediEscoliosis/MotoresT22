using System.Reflection;
using UnityEngine;


public class ArmAudio : MonoBehaviour
{
    public enum ArmKind { Universal, Atractor, Repulsor }

    [Tooltip("Qué brazo escucha esta copia.")]
    [SerializeField] private ArmKind kind = ArmKind.Universal;

    [Tooltip("Objeto donde está el script del brazo. Si queda vacío, usa este mismo objeto.")]
    [SerializeField] private GameObject armObject;

    [SerializeField] private AudioSource audioSource;
    [Tooltip("Si está activado, el sonido se corta apenas el brazo suelta el objeto. " +
             "Desactivalo para que el clip suene completo (ideal para el repulsor).")]
    [SerializeField] private bool stopOnRelease = true;

    [SerializeField] private bool debugLog = true;

    private MonoBehaviour arm;
    private FieldInfo heldField;
    private bool wasHolding;

    private void Awake()
    {
        GameObject target = armObject != null ? armObject : gameObject;
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;

        switch (kind)
        {
            case ArmKind.Universal:
                arm = target.GetComponent<UniversalArm>();
                heldField = typeof(UniversalArm).GetField("heldObject", flags);
                break;
            case ArmKind.Atractor:
                arm = target.GetComponent<AttractorArm>();
                heldField = typeof(ArmBehaviour).GetField("selectedObject", flags);
                break;
            case ArmKind.Repulsor:
                arm = target.GetComponent<RepulsorArm>();
                heldField = typeof(ArmBehaviour).GetField("selectedObject", flags);
                break;
        }

        if (arm == null || heldField == null)
            Debug.LogWarning("ArmAudio: no encontré el brazo " + kind + " en " + target.name, this);
    }

    private void Update()
    {
        if (arm == null || heldField == null || audioSource == null) return;

        Rigidbody held = heldField.GetValue(arm) as Rigidbody;
        bool holding = held != null;

        if (holding && !wasHolding)
        {
            audioSource.Play();
            if (debugLog) Debug.Log("ArmAudio [" + kind + "]: agarró " + held.name, this);
        }
        else if (!holding && wasHolding)
        {
            if (stopOnRelease) audioSource.Stop();
            if (debugLog) Debug.Log("ArmAudio [" + kind + "]: soltó", this);
        }

        wasHolding = holding;
    }

    private void OnDisable()
    {
        if (audioSource != null) audioSource.Stop();
        wasHolding = false;
    }
}