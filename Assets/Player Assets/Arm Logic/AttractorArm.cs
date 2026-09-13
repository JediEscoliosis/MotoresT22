using UnityEngine;

public class AttractorArm : ArmBehaviour
{
    protected override Vector3 DefineDirection(Vector3 objPosition)
    {
        return (transform.position - objPosition).normalized;
    }

    protected override Color GizmoColor() => Color.green;
}
