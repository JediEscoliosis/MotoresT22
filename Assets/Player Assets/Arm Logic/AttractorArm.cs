using UnityEngine;
using UnityEngine.InputSystem;

public class AttractorArm : ArmBehaviour
{
    protected override bool IsActivated()
    {
        return Keyboard.current != null&& Keyboard.current.qKey.isPressed&& !Keyboard.current.eKey.isPressed;
    }

    protected override float DistanceDirection() => -1f;

    protected override Color GizmoColor() => Color.red;
}
