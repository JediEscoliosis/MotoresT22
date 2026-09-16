using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Sensibilidad del mouse")]
    [SerializeField] float normalSensivity = 0.15f;
    [SerializeField] float aimSensivity = 0.08f;

    [Header("Limite de Camara")]
    [SerializeField] private float minVerticalAngle = -30f;
    [SerializeField] private float maxVerticalAngle = 60f;

    [Header("Limites al apuntar")]
    [SerializeField] private float aimMinAngle = -70f;
    [SerializeField] private float aimMaxAngle = 60f;

    [Header("Transision de Camaras")]
    [SerializeField, Min(0.1f)] private float aligmentSpeed = 12f;
    [SerializeField, Min(1f)] private float playerTurnSpeed = 720f;

    [Header("Referencia")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Rigidbody playerBody;
    [SerializeField] private GameObject crosshair;

    public bool IsAiming { get; private set; }

    private float horizontalRotacion;
    private float targetHorizontalRotation;
    private float verticalRotation;


    private void Awake()
    {
        //Este bloque inicia la rotación de la cámara y oculta la mira al comenzar.
        horizontalRotacion = transform.eulerAngles.y;
        targetHorizontalRotation = horizontalRotacion;

        verticalRotation = Mathf.DeltaAngle(0f, cameraPivot.localEulerAngles.x);

        if (crosshair != null )
            crosshair.SetActive(false);
    }

    private void Start()
    {
        LockCursor();
    }

    private void Update()
    {
        if (Time.timeScale == 0)
            return;

        if (Mouse.current == null) 
           return;

        //De momento Esc libera el cursor
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        else if (Mouse.current.leftButton.wasPressedThisFrame &&
                Cursor.lockState != CursorLockMode.Locked)
        {
            LockCursor();
        }

        bool cursorLocked = Cursor.lockState == CursorLockMode.Locked;
        bool aimPressed = cursorLocked && Mouse.current.rightButton.isPressed;


        //Esta linea de codigo busca el frente del jugador
        if (aimPressed && !IsAiming)
        {
            targetHorizontalRotation = playerBody.rotation.eulerAngles.y;
        }
        //Esta otra permite concervar la rotacion a la que se apunto
        else if (!aimPressed && IsAiming)
        {
            targetHorizontalRotation = horizontalRotacion;
        }

        IsAiming = aimPressed;


        //muestra y oculta la mira
        if (crosshair != null && crosshair.activeSelf != IsAiming)
        {
            crosshair.SetActive(IsAiming);
        }

        if (!cursorLocked)
            return;


        RotateCamera();

    }


    private void RotateCamera()
    {
        //el delta representa el movimiento del mouse
        Vector2 mouseInput = Mouse.current.delta.ReadValue();

        float sensivity = IsAiming ? aimSensivity : normalSensivity;


        targetHorizontalRotation += mouseInput.x * sensivity;
        verticalRotation -= mouseInput.y * sensivity;

        //Marca los limites del campo de vision 
        float minAngle = IsAiming ? aimMinAngle : minVerticalAngle;
        float maxAngle = IsAiming ? aimMaxAngle : maxVerticalAngle;
        verticalRotation = Mathf.Clamp(verticalRotation, minAngle, maxAngle);


        //Al apuntar suaviza el giro
        if (IsAiming)
        {
            float blend = 1f - Mathf.Exp(-aligmentSpeed * Time.deltaTime);

            horizontalRotacion = Mathf.LerpAngle(horizontalRotacion, targetHorizontalRotation, blend);
        }

        else
        {
            horizontalRotacion = targetHorizontalRotation;
        }

    }

    private void FixedUpdate()
    {
        //si el Player no apunta, ejecuta lo de debajo.
        if (!IsAiming)
            return;

        //convierte los angulos en grados para hacer la rotacion efectiva
        Quaternion targetRotation = Quaternion.Euler (0f, horizontalRotacion, 0f);

        //aplica el giro al rigidbody
        playerBody.MoveRotation(Quaternion.RotateTowards(playerBody.rotation,targetRotation,playerTurnSpeed * Time.fixedDeltaTime));
    }

    private void LateUpdate()
    {
        if (Time.timeScale == 0)
            return;

        // Mantiene la orbita independiente del giro del personaje.
        transform.rotation = Quaternion.Euler(0f, horizontalRotacion, 0f);

        // Solo el pivote se inclina verticalmente.
        cameraPivot.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }


    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void OnDisable()
    {
        IsAiming = false;

        if (crosshair != null)
            crosshair.SetActive(false);


        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}
