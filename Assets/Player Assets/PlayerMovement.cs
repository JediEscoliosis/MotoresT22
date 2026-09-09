using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Rotacion")]
    [SerializeField] private ThirdPersonCamera cameraController;
    [SerializeField, Min(0.1f)] private float rotacionSmoothness = 12f;

    [Header("Camara")]
    [SerializeField] private Transform cameraTransform;

    [Header("Salto")]
    [SerializeField, Min(0f)] private float jumpHeight = 1.5f;
    [SerializeField, Min(1f)] private float fallHeight = 2.5f;

    [Header("Deteccion del Suelo")]
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private Transform groundCheck;


    private Rigidbody rb;

    private Vector2 moveInput;
    private Vector3 moveDirection;

    private bool jumpRequested;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //Estas funciones van aca para que cada frame calcule los inputs y el transform de la capasula.
        ReadInput();
        CalculateMovement();
    }

    private void FixedUpdate()
    {
        //Estas funciones van aca porque el FixedUptade calcula cada frame utilizando gravedad y vectores.
        Move();
        CheckGround();
        Jump();
        RotatePlayer();
        FallGravity();
    }

    private void ReadInput()
    {
        moveInput = Vector2.zero;

        if (Keyboard.current == null)
            return;

        if (Keyboard.current.wKey.isPressed)
            moveInput.y += 1f;

        if (Keyboard.current.sKey.isPressed)
            moveInput.y -= 1f;

        if (Keyboard.current.dKey.isPressed)
            moveInput.x += 1f;

        if (Keyboard.current.aKey.isPressed)
            moveInput.x -= 1f;

        moveInput = moveInput.normalized;


        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            jumpRequested = true;
    }

    private void CalculateMovement()
    {
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        moveDirection = Vector3.ClampMagnitude (cameraForward * moveInput.y + cameraRight * moveInput.x, 1f);
    }

    private Vector3 GetGroundCheckSize()
    {
        //Detecta el area del GroundCheck del player
        Vector3 size = groundCheck.lossyScale;

        return new Vector3(Mathf.Abs(size.x),Mathf.Abs(size.y),Mathf.Abs(size.z));
    }


    private void CheckGround ()
    {
        Vector3 halfExtents = GetGroundCheckSize() * 0.5f;
        bool groundDetected = Physics.CheckBox(groundCheck.position,halfExtents,groundCheck.rotation,groundLayers,QueryTriggerInteraction.Ignore);

        //Evita el salto infinito
        isGrounded = groundDetected && rb.linearVelocity.y <= 0.1f;
    }

    private void Jump()
    {
        if (jumpRequested && isGrounded)
        {
            Vector3 velocity = rb.linearVelocity;

            velocity.y = Mathf.Sqrt(2f * Mathf.Abs(Physics.gravity.y) * jumpHeight);

            rb.linearVelocity = velocity;
            isGrounded = false;
        }

        jumpRequested = false;
    }


    private void Move()
    {
        Vector3 targetPosition = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(targetPosition);
    }


    private void OnDrawGizmosSelected()
    {
        //Toda esta funcion sirve para que cuando la deteccion del suelo se cumple cambia de color el GroundCheck
        if (groundCheck == null) 
           return;

        Color previousColor = Gizmos.color;
        Matrix4x4 previousMatrix = Gizmos.matrix;

        Gizmos.color = isGrounded ? Color.green : Color.yellow;

        Gizmos.matrix = Matrix4x4.TRS(groundCheck.position, groundCheck.rotation, GetGroundCheckSize());
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

        Gizmos.matrix = previousMatrix;
        Gizmos.color = previousColor;
    }

    private void RotatePlayer()
    {
        //cuando apunta, se llama al script thirdPersonCamera
        if (cameraController == null || cameraController.IsAiming)
            return;

        //para que sin movimiento conserve la rotacion
        if (moveDirection.sqrMagnitude < 0.01f)
            return;

        quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);

        float blend = 1f - Mathf.Exp (-rotacionSmoothness * Time.deltaTime);

        rb.MoveRotation (Quaternion.Slerp (rb.rotation,targetRotation, blend));

    }

    private void FallGravity()
    {
        if (rb.linearVelocity.y < 0f)
        {
            rb.AddForce(Physics.gravity * (fallHeight - 1f), ForceMode.Acceleration);
        }
    }
}
