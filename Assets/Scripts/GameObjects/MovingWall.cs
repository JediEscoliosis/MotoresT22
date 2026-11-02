using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovingWall : MonoBehaviour
{
    [Header("Movimiento")]
    [Tooltip("Desplazamiento respecto de la orientacion inicial de la pared.")]
    [SerializeField]
    private Vector3 movementOffset = new Vector3(0f, 3f, 0f);

    [SerializeField, Min(0.01f)]
    private float moveSpeed = 2f;

    private Rigidbody wallBody;

    private Vector3 targetPosition;
    private Vector3 initialPosition;
    private Vector3 currentTarget;

    private bool initialized;
    private bool isMoving;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (initialized)
            return;

        wallBody = GetComponent<Rigidbody>();
        
        // Guardo la referencia a la posición inicial para uso posterior.
        initialPosition = wallBody.position;
        
        // Calcula el destino desde la posicion y orientacion iniciales.
        targetPosition = initialPosition + wallBody.rotation * movementOffset;

        currentTarget = initialPosition;
        initialized = true;
    }

    public void MoveWall()
    {
        if (!isActiveAndEnabled)
            return;

        // Asegura la inicializacion antes de recibir una orden externa.
        Initialize();

        // Inicia el movimiento sin repetirlo si llegan mas eventos.
        currentTarget = targetPosition;
        isMoving = true;
    }

    public void ReturnWall()
    {
        if (!isActiveAndEnabled)
            return;
        Initialize();
        currentTarget = initialPosition;
        isMoving = true;
    }

    private void FixedUpdate()
    {
        if (!isMoving || wallBody == null)
            return;

        // Avanza hacia el destino a una velocidad constante.
        Vector3 nextPosition = Vector3.MoveTowards(wallBody.position,currentTarget,moveSpeed * Time.fixedDeltaTime);

        // Mueve la pared mediante su Rigidbody cinematico.
        wallBody.MovePosition(nextPosition);

        // Termina cuando alcanza el destino.
        if ((nextPosition - targetPosition).sqrMagnitude < 0.000001f)
            isMoving = false;
    }

    private void OnDrawGizmosSelected()
    {
        // Muestra el destino al seleccionar la pared.
        Vector3 origin = Application.isPlaying && initialized ? initialPosition : transform.position;
        Vector3 destination = Application.isPlaying && initialized ? targetPosition : transform.position + transform.rotation * movementOffset;

        Color previousColor = Gizmos.color;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, destination);
        Gizmos.DrawWireSphere(destination, 0.2f); //Cubo a la 2da posición
        Gizmos.DrawWireCube(origin, Vector3.one * 0.2f); // Cubo en la posición inicial

        Gizmos.color = previousColor;
    }
}