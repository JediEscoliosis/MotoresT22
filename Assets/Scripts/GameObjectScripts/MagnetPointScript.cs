using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class LayerMoveTrigger : MonoBehaviour
{
    [Header("Deteccion")]
    [SerializeField] private LayerMask activationLayers;
    [SerializeField] private bool activateOnlyOnce = true;

    [Header("Eventos")]
    [SerializeField] private UnityEvent onActivated = new UnityEvent();

    [Header("Gizmo")]
    [SerializeField]
    private Color zoneColor = new Color(0f, 1f, 1f, 0.25f);

    private bool activated;

    private void Reset()
    {
        // Configura el collider como una zona que no bloquea objetos.
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActiveAndEnabled)
            return;

        // Impide nuevas activaciones si la zona funciona una sola vez.
        if (activateOnlyOnce && activated)
            return;

        // Comprueba si la layer del collider puede activar esta zona.
        if ((activationLayers.value & (1 << other.gameObject.layer)) == 0)
            return;

        activated = true;

        // Ejecuta las acciones conectadas desde el Inspector.
        onActivated.Invoke();
    }

    public void ResetTrigger()
    {
        // Permite volver a activar una zona que ya fue utilizada.
        activated = false;
    }

    private void OnDrawGizmos()
    {
        BoxCollider zone = GetComponent<BoxCollider>();

        if (zone == null)
            return;

        Matrix4x4 previousMatrix = Gizmos.matrix;
        Color previousColor = Gizmos.color;

        // Dibuja el volumen de deteccion, incluso sin seleccionarlo.
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = zoneColor;
        Gizmos.DrawCube(zone.center, zone.size);

        Gizmos.color = new Color(zoneColor.r,zoneColor.g,zoneColor.b,1f);

        Gizmos.DrawWireCube(zone.center, zone.size);

        Gizmos.matrix = previousMatrix;
        Gizmos.color = previousColor;
    }
}