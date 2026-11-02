using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider))]
public class SignInteraction : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Rigidbody playerBody;
    [SerializeField] private GameObject textPanel;
    [SerializeField] private GameObject readPrompt;

    // Registra los colliders del jugador que estan dentro de la zona.
    private readonly HashSet<Collider> playerColliders =
        new HashSet<Collider>();

    private bool isOpen;

    private void Awake()
    {
        if (textPanel != null)
            textPanel.SetActive(false);

        // Oculta el aviso al comenzar.
        if (readPrompt != null)
            readPrompt.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActiveAndEnabled)
            return;

        // Solo acepta colliders asociados al Rigidbody del jugador.
        if (playerBody == null || other.attachedRigidbody != playerBody)
            return;

        playerColliders.Add(other);

        // Muestra el aviso si el jugador entra y el panel esta cerrado.
        if (readPrompt != null)
            readPrompt.SetActive(!isOpen);
    }

    private void OnTriggerExit(Collider other)
    {
        playerColliders.Remove(other);

        // Cierra cuando todos los colliders del jugador salieron.
        if (playerColliders.Count == 0)
            SetPanel(false);
    }

    private void Update()
    {
        // Limpia referencias si un collider se destruye o desactiva.
        playerColliders.RemoveWhere(c =>
            c == null || !c.enabled || !c.gameObject.activeInHierarchy
        );

        if (playerColliders.Count == 0)
        {
            // Oculta el panel y el aviso cuando no hay jugador en la zona.
            SetPanel(false);
            return;
        }

        // Cada pulsacion de E alterna el estado del panel.
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            SetPanel(!isOpen);
        }
    }

    private void SetPanel(bool open)
    {
        isOpen = open;

        if (textPanel != null)
            textPanel.SetActive(open);

        // El aviso solo aparece dentro de la zona y con el panel cerrado.
        if (readPrompt != null) readPrompt.SetActive(playerColliders.Count > 0 && !isOpen);
    }

    private void OnDisable()
    {
        playerColliders.Clear();
        SetPanel(false);
    }

    private void OnDrawGizmosSelected()
    {
        BoxCollider zone = GetComponent<BoxCollider>();

        if (zone == null)
            return;

        // Dibuja la zona respetando su posicion, rotacion y escala.
        Matrix4x4 previousMatrix = Gizmos.matrix;
        Color previousColor = Gizmos.color;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(zone.center, zone.size);

        Gizmos.matrix = previousMatrix;
        Gizmos.color = previousColor;
    }
}