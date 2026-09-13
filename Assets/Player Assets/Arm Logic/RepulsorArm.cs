using UnityEngine;

public class RepulsorArm : ArmBehaviour
{
    protected override Vector3 DefineDirection(Vector3 objPosition)
    {
        return (objPosition - transform.position).normalized;
    }

    protected override Color GizmoColor() => Color.green;
}
