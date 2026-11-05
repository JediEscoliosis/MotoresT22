using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class TwoZoneDoorTrigger : MonoBehaviour
{
    [Header("Zonas de activacion")]
    [SerializeField] private BlockActivationZone firstZone;
    [SerializeField] private BlockActivationZone secondZone;

    [Header("Movimiento de la puerta")]
    [Tooltip("Arrastra el Transform de la parte movil de la puerta.")]
    [SerializeField] private Transform door;

    [Tooltip("Desplazamiento desde la posicion cerrada, en los ejes de su padre. Por ejemplo: (3, 0, 0).")]
    [SerializeField] private Vector3 openOffset = new Vector3(3f, 0f, 0f);

    [Tooltip("Velocidad en unidades locales por segundo.")]
    [SerializeField, Min(0.01f)] private float moveSpeed = 2f;

    [Header("Comportamiento")]
    [Tooltip("Activado: abre una vez y queda abierta. Desactivado: cierra si una zona queda vacia.")]
    [SerializeField] private bool activateOnlyOnce = true;

    [Header("Eventos opcionales")]
    [Tooltip("Se ejecuta al ordenar la apertura. Puede usarse para luces o sonidos.")]
    [SerializeField] private UnityEvent onActivated = new UnityEvent();

    [Tooltip("Se ejecuta al ordenar el cierre, con Activate Only Once desmarcado.")]
    [SerializeField] private UnityEvent onDeactivated = new UnityEvent();

    private Vector3 closedLocalPosition;
    private bool openRequested;
    private bool completed;
    private bool bothWereOccupied;

    private void Start()
    {
        if (door == null)
        {
            Debug.LogError("Asigna la parte movil de la puerta en el campo Door.", this);
            enabled = false;
            return;
        }

        // La posicion inicial de la puerta se considera su posicion cerrada.
        closedLocalPosition = door.localPosition;

        if (firstZone == null || secondZone == null || firstZone == secondZone)
        {
            Debug.LogWarning("Asigna dos zonas diferentes en First Zone y Second Zone.", this);
        }
        else if (firstZone.RequiredBlock == null || secondZone.RequiredBlock == null)
        {
            Debug.LogWarning("Cada zona necesita un bloque asignado en Required Block.", this);
        }
        else if (firstZone.RequiredBlock == secondZone.RequiredBlock)
        {
            Debug.LogWarning("Asigna un bloque distinto a cada zona.", this);
        }
    }

    private void Update()
    {
        CheckZones();

        // Sigue moviendo la puerta aunque la activacion ya se haya completado.
        MoveDoor();
    }

    private void CheckZones()
    {
        if (activateOnlyOnce && completed)
            return;

        // Las dos zonas deben estar ocupadas al mismo tiempo por bloques distintos.
        bool bothOccupied =
            firstZone != null &&
            secondZone != null &&
            firstZone != secondZone &&
            firstZone.RequiredBlock != null &&
            secondZone.RequiredBlock != null &&
            firstZone.RequiredBlock != secondZone.RequiredBlock &&
            firstZone.IsOccupied &&
            secondZone.IsOccupied;

        if (bothOccupied == bothWereOccupied)
            return;

        bothWereOccupied = bothOccupied;
        openRequested = bothOccupied;

        if (bothOccupied)
        {
            completed = true;
            onActivated.Invoke();
        }
        else
        {
            onDeactivated.Invoke();
        }
    }

    private void MoveDoor()
    {
        if (door == null)
            return;

        Vector3 targetPosition = openRequested
            ? closedLocalPosition + openOffset
            : closedLocalPosition;

        // Cambia directamente el Transform y se detiene al alcanzar el destino.
        door.localPosition = Vector3.MoveTowards(
            door.localPosition,
            targetPosition,
            moveSpeed * Time.deltaTime);
    }

    public void ResetTrigger()
    {
        // Reinicia la deteccion; no ordena cerrar la puerta.
        // Si ambos bloques siguen dentro, volvera a activarse en el siguiente Update.
        completed = false;
        bothWereOccupied = false;
    }
}

