using UnityEngine;
using UnityEngine.SocialPlatforms;

[RequireComponent(typeof(Camera))]
public class CameraColission : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private ThirdPersonCamera cameraController;
    [SerializeField] private Transform normalPosition;
    [SerializeField] private Transform aimPosition;

    [Header("Colisiones")]
    [SerializeField] private LayerMask obstacleLayers;
    [SerializeField, Min(0.01f)] private float cameraRadius = 0.25f;
    [SerializeField, Min(0f)] private float collisionMargin = 0.05f;

    [Header("Transiciones")]
    [SerializeField, Min(0.01f)] private float transitionSpeed = 10f;
    [SerializeField, Min(0.01f)] private float returnSpeed = 5f;

    private Camera cameraComponent;
    private Vector3 currentOffset;
    private float currentDistance;

    private void Awake()
    {
        cameraComponent = GetComponent<Camera>();

        if (cameraPivot == null || cameraController == null || normalPosition == null || aimPosition == null)
        {
            //Chequea que que camerapivot este asignado para evitar errores.
            Debug.LogError("cameraColission: falta una referencia", this);
            enabled = false;
            return;
        }

        //Esta linea de codigo guarda la posion inicial respecto del pivot.
        currentOffset = cameraPivot.InverseTransformPoint(normalPosition.position);
        currentDistance = Vector3.Distance(cameraPivot.position, normalPosition.position);

    }

    private void LateUpdate()
    {
        // Elige el objeto que marca la posicion deseada.
        Transform target = cameraController.IsAiming? aimPosition : normalPosition;
        Vector3 targetOffset = cameraPivot.InverseTransformPoint(target.position);



        // Suaviza el cambio entre exploracion y apuntado.
        float blend =1f - Mathf.Exp(-transitionSpeed * Time.deltaTime);
        currentOffset = Vector3.Lerp(currentOffset, targetOffset, blend);



        Vector3 origin = cameraPivot.position;
        Vector3 desiredPosition =cameraPivot.TransformPoint(currentOffset);
        Vector3 offset = desiredPosition - origin;


        float desiredDistance = offset.magnitude;

        if (desiredDistance < 0.001f)
            return;

        Vector3 direction = offset / desiredDistance;
        float targetDistance = desiredDistance;

        // Protege tambien las esquinas del plano cercano.
        float near = cameraComponent.nearClipPlane;

        float halfHeight = near * Mathf.Tan(cameraComponent.fieldOfView * 0.5f * Mathf.Deg2Rad);

        float halfWidth = halfHeight * cameraComponent.aspect;

        float radius = Mathf.Max(cameraRadius,new Vector3(halfWidth, halfHeight, near).magnitude);



        // Comprueba el recorrido desde el pivote hacia la camara.
        if (Physics.SphereCast(origin,radius,direction, out RaycastHit hit,desiredDistance,obstacleLayers,QueryTriggerInteraction.Ignore))
        {
            targetDistance = Mathf.Max(0f, hit.distance - collisionMargin);
        }

        if (targetDistance < currentDistance)
        {
            // Se acerca inmediatamente ante un obstaculo.
            currentDistance = targetDistance;
        }
        else
        {
            // Recupera la distancia suavemente.
            currentDistance = Mathf.MoveTowards(currentDistance,targetDistance,returnSpeed * Time.deltaTime);
        }

        transform.position = origin + direction * currentDistance;

        // La orientacion sigue controlada por el pivote.
        transform.rotation = cameraPivot.rotation;
    }
}
