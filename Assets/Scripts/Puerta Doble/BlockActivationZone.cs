using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider))]
public class BlockActivationZone : MonoBehaviour
{
    [Header("Bloque de esta zona")]
    [Tooltip("Arrastra el Rigidbody del bloque que debe entrar en esta zona. Su tag no importa.")]
    [SerializeField] private Rigidbody requiredBlock;

    private BoxCollider zoneCollider;

    // Guardamos colliders separados por si un bloque tiene varios.
    private readonly HashSet<Collider> contacts = new HashSet<Collider>();

    public Rigidbody RequiredBlock => requiredBlock;

    public bool IsOccupied
    {
        get
        {
            if (!isActiveAndEnabled ||zoneCollider == null ||!zoneCollider.enabled ||
                !zoneCollider.isTrigger || requiredBlock == null ||!requiredBlock.gameObject.activeInHierarchy ||
                !requiredBlock.detectCollisions)
            {
                return false;
            }

            // Al destruir o desactivar un collider, Unity puede no enviar OnTriggerExit.
            contacts.RemoveWhere(IsInvalidContact);
            return contacts.Count > 0;
        }
    }

    private void Reset()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void Awake()
    {
        zoneCollider = GetComponent<BoxCollider>();
        zoneCollider.isTrigger = true;

    }

    private void OnTriggerEnter(Collider other)
    {
        RegisterContact(other);
    }

    private void OnTriggerStay(Collider other)
    {
        // Tambien permite registrar un bloque que ya esta dentro.
        RegisterContact(other);
    }

    private void RegisterContact(Collider other)
    {
        if (!isActiveAndEnabled || requiredBlock == null)
            return;

        if (!IsInvalidContact(other))
            contacts.Add(other);
    }

    private bool IsInvalidContact(Collider other)
    {
        // attachedRigidbody tambien reconoce colliders en hijos del bloque.
        return other == null ||!other.enabled || !other.gameObject.activeInHierarchy ||other.isTrigger ||
               other.attachedRigidbody != requiredBlock;
    }

    private void OnTriggerExit(Collider other)
    {
        // Si queda otro collider del mismo bloque dentro, la zona sigue ocupada.
        contacts.Remove(other);
    }

    private void OnDisable()
    {
        contacts.Clear();
    }

    private void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();

        if (box == null)
            return;

        Color previousColor = Gizmos.color;
        Matrix4x4 previousMatrix = Gizmos.matrix;

        bool occupied = Application.isPlaying && IsOccupied;
        Color zoneColor = occupied ? Color.green : Color.cyan;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = new Color(zoneColor.r, zoneColor.g, zoneColor.b, 0.2f);
        Gizmos.DrawCube(box.center, box.size);
        Gizmos.color = zoneColor;
        Gizmos.DrawWireCube(box.center, box.size);

        Gizmos.matrix = previousMatrix;
        Gizmos.color = previousColor;
    }
}

