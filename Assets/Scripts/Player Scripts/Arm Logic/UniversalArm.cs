using UnityEngine;
using UnityEngine.InputSystem;

public class UniversalArm : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera aimCamera;

    [Tooltip("Centro de ambos radios. Si queda vacio, usa este objeto.")]
    [SerializeField] private Transform actionOrigin;

    [Header("Capas")]
    [SerializeField] private LayerMask universalMask;

    [Tooltip("Incluir objetos y paredes. Excluir la capa del Player.")]
    [SerializeField] private LayerMask raycastMask = ~0;

    [Header("Radios")]
    [SerializeField, Min(0.1f)]
    private float selectionRadius = 5f;

    [SerializeField, Min(0.1f)]
    private float manipulationRadius = 10f;

    [Header("Distancia respecto de la camara")]
    [SerializeField, Min(0.1f)]
    private float minimumDistance = 2f;

    [SerializeField, Min(0f)]
    private float distanceSpeed = 3f;

    [Header("Movimiento")]
    [SerializeField, Min(0f)]
    private float followStrength = 40f;

    [SerializeField, Min(0f)]
    private float damping = 12f;

    [SerializeField, Min(0f)]
    private float maxAcceleration = 100f;

    private Rigidbody heldObject;
    private bool originalUseGravity;
    private float targetDistance;



    private Vector3 OriginPosition =>actionOrigin != null? actionOrigin.position: transform.position;

    private void Awake()
    {
        if (aimCamera == null)
            aimCamera = Camera.main;

    }

    //evita que el radio de manipulacion sea menor que el de seleccion
    private void OnValidate()
    {
        manipulationRadius = Mathf.Max(manipulationRadius,selectionRadius);
    }

    private void Update()
    {
        Mouse mouse = Mouse.current;

        if (mouse == null || aimCamera == null)
        {
            ReleaseObject();
            return;
        }

        if (!mouse.leftButton.isPressed)
        {
            ReleaseObject();
            return;
        }

        // Selecciona solo al comenzar el clic.
        if (mouse.leftButton.wasPressedThisFrame)
            TrySelectObject();

        if (heldObject == null)
            return;

        // Q acerca y E aleja el objeto seleccionado

        float distanceInput = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.qKey.isPressed)
                distanceInput -= 1f;

            if (Keyboard.current.eKey.isPressed)
                distanceInput += 1f;
        }

        targetDistance += distanceInput * distanceSpeed * Time.deltaTime;

        Ray ray = GetAimRay();

        float maximumDistance = Mathf.Max(minimumDistance,Vector3.Distance(ray.origin, OriginPosition) + manipulationRadius);

        targetDistance = Mathf.Clamp(targetDistance,minimumDistance,maximumDistance);
    }

    //Crea un rayo desde el centro de la vista de la camara en la direccion a la que se apunta
    private Ray GetAimRay()
    {
        return aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    }


    //Selecciona los objetos que se pueden mover
    private void TrySelectObject()
    {
        ReleaseObject();

        Ray ray = GetAimRay();

        float rayLength =
            Vector3.Distance(ray.origin, OriginPosition) + selectionRadius;

        if (!Physics.Raycast(ray,out RaycastHit hit,rayLength,raycastMask,QueryTriggerInteraction.Ignore))
        {
            return;
        }

        // Solo permite seleccionar colliders de la capa universal.
        if ((universalMask.value & (1 << hit.collider.gameObject.layer)) == 0)
        {
            return;
        }

        Rigidbody candidate = hit.rigidbody;

        if (candidate == null || candidate.isKinematic)
            return;

        if (Vector3.Distance(OriginPosition,candidate.worldCenterOfMass) > selectionRadius)
        {
            return;
        }

        heldObject = candidate;

        // Guardamos el estado para restaurarlo al soltar.
        originalUseGravity = heldObject.useGravity;heldObject.useGravity = false;

        targetDistance = Mathf.Max (minimumDistance,Vector3.Dot(heldObject.worldCenterOfMass - ray.origin,ray.direction));

        heldObject.WakeUp();
    }

    private void FixedUpdate()
    {
        if (heldObject == null)
            return;

        if (aimCamera == null || Mouse.current == null || !Mouse.current.leftButton.isPressed|| heldObject.isKinematic)
        {
            ReleaseObject();
            return;
        }

        // Una vez seleccionado, usa el radio mayor.
        if (Vector3.Distance(OriginPosition,heldObject.worldCenterOfMass) > manipulationRadius)
        {
            ReleaseObject();
            return;
        }

        Ray ray = GetAimRay();
        Vector3 targetPosition = ray.GetPoint(targetDistance);

        // Limitamos el destino al area de manipulacion.
        // El margen reduce liberaciones por pequeñas oscilaciones.
        Vector3 offset = targetPosition - OriginPosition;

        targetPosition = OriginPosition + Vector3.ClampMagnitude(offset,manipulationRadius * 0.95f);

        Vector3 acceleration = (targetPosition - heldObject.worldCenterOfMass) * followStrength - heldObject.linearVelocity * damping;

        heldObject.AddForce(Vector3.ClampMagnitude(acceleration,maxAcceleration),ForceMode.Acceleration);
    }


    //Restaura la gravedad del objeto cuando se lo libera
    private void ReleaseObject()
    {
        if (heldObject != null)
            heldObject.useGravity = originalUseGravity;

        heldObject = null;
    }

    private void OnDisable()
    {
        ReleaseObject();
    }

    //cuando se lo activa quita la gravedad del objeto
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            ReleaseObject();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(OriginPosition,selectionRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(OriginPosition,manipulationRadius);
    }
}
