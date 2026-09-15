using UnityEngine;
using UnityEngine.InputSystem;

public class RepulsorArm : ArmBehaviour
{
    protected override bool IsActivated()
    {return Keyboard.current != null&& Keyboard.current.eKey.isPressed&& !Keyboard.current.qKey.isPressed;
    }

    protected override float DistanceDirection() => 1f;

    protected override Color GizmoColor() => Color.blue;

    protected override void ApplyObjectForce(Rigidbody rb, Ray ray)
    {
        // Fuerza continua en la direccion actual de la mira.
        // Sin freno ni punto objetivo que limite la velocidad.
        rb.AddForce(ray.direction * force, ForceMode.Force);
    }
}