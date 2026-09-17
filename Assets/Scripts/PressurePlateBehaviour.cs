using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class PressurePlate : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private string requiredTag = ""; //Lo dejo por si no funciona y poder cambiar el comportamiento mas rapido. 
    [SerializeField] private float pressedYOffset = -0.1f; // cuánto baja visualmente el botón

    [Header("Eventos")]
    public UnityEvent OnPressed;   // se dispara al activarse
    public UnityEvent OnReleased;  // se dispara al desactivarse

    private HashSet<Collider> objectsOnPlate = new HashSet<Collider>();
    private Vector3 originalPosition;
    private bool isPressed = false;

    void Start()
    {
        originalPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag))
            return;

        objectsOnPlate.Add(other);
        UpdateState();
    }

    private void OnTriggerExit(Collider other)
    {
        objectsOnPlate.Remove(other);
        UpdateState();
    }

    private void UpdateState()
    {
        bool shouldBePressed = objectsOnPlate.Count > 0;

        if (shouldBePressed && !isPressed)
        {
            isPressed = true;
            transform.position = originalPosition + Vector3.up * pressedYOffset;
            OnPressed.Invoke();
        }
        else if (!shouldBePressed && isPressed)
        {
            isPressed = false;
            transform.position = originalPosition;
            OnReleased.Invoke();
        }
    }
}