using UnityEngine;
using UnityEngine.InputSystem;

public abstract class ArmBehaviour : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] protected Camera aimCamera;

    [Header("Deteccion")]
    [SerializeField, Min(0.1f)]
    protected float activationRadius = 5f;
    [SerializeField] protected LayerMask metalMask;

    // Incluir objetos y paredes. Excluir la capa del Player.
    [SerializeField] protected LayerMask raycastMask = ~0;

    [Header("Movimiento del objeto")]
    [SerializeField, Min(0f)] protected float force = 50f;

    // Velocidad con la que acercamos o alejamos el punto objetivo.
    [SerializeField, Min(0f)] private float distanceSpeed = 3f;

    [SerializeField, Min(0.1f)] private float minimumDistance = 1f;

    [SerializeField, Min(0f)] private float followStrength = 20f;
    [SerializeField, Min(0f)] private float damping = 8f;

    private Rigidbody selectedObject;
    private float targetDistance;
    private bool wasActivated;

    protected abstract bool IsActivated();

    // -1 para atraer y +1 para repeler.
    protected abstract float DistanceDirection();
    protected abstract Color GizmoColor();

    protected void Awake()
    {
        if (aimCamera == null)
            aimCamera = Camera.main;
    }

    protected void Update()
    {
        bool activated = IsActivated();

        // Seleccionamos una sola vez al comenzar a presionar.
        if (activated && !wasActivated)
            TrySelectObject();

        if (!activated)
            selectedObject = null;

        wasActivated = activated;
    }

    private void TrySelectObject()
    {
        selectedObject = null;

        Ray ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        // La camara puede estar detras del personaje.
        float rayLength =Vector3.Distance(ray.origin, transform.position) + activationRadius;

        if (!Physics.Raycast(ray,out RaycastHit hit,rayLength,raycastMask,QueryTriggerInteraction.Ignore))
        {
            return;
        }


        // El primer impacto debe ser un objeto metalico.
        // Una pared por delante impide seleccionarlo.
        if ((metalMask.value & (1 << hit.collider.gameObject.layer)) == 0)

            return;

        Rigidbody candidate = hit.rigidbody;

        if (candidate == null || candidate.isKinematic)
            return;

        if (Vector3.Distance(transform.position,candidate.worldCenterOfMass) > activationRadius)
        {
            return;
        }

        selectedObject = candidate;

        targetDistance = Mathf.Max(minimumDistance,Vector3.Dot(candidate.worldCenterOfMass - ray.origin,ray.direction));
    }

    protected void FixedUpdate()
    {
        if (selectedObject == null)
            return;

        if (!IsActivated() || selectedObject.isKinematic)
        {
            selectedObject = null;
            return;
        }

        // Al salir del alcance del brazo, se libera.
        if (Vector3.Distance(transform.position,selectedObject.worldCenterOfMass) > activationRadius)
        {
            selectedObject = null;
            return;
        }

        Ray ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        ApplyObjectForce(selectedObject, ray);
    }

    //Para desactivarlo
    protected void OnDisable()
    {
        selectedObject = null;
        wasActivated = false;
    }

    //Gizmo para verlo en el gamescene
    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = GizmoColor();
        Gizmos.DrawWireSphere(transform.position, activationRadius);
    }

    //Esta funcion duplica la fuerza de empuje de los objetos mientras esten en el radio de accion
    protected virtual void ApplyObjectForce(Rigidbody rb, Ray ray)
    {
        targetDistance +=
            DistanceDirection() * distanceSpeed * Time.fixedDeltaTime;

        targetDistance = Mathf.Clamp(targetDistance,minimumDistance,Vector3.Distance(ray.origin, transform.position) + activationRadius);

        Vector3 targetPosition = ray.GetPoint(targetDistance);

        Vector3 appliedForce =(targetPosition - rb.worldCenterOfMass) * followStrength- rb.linearVelocity * damping;

        rb.AddForce(Vector3.ClampMagnitude(appliedForce, force),ForceMode.Force);
    }
}
