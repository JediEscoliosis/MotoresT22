using UnityEngine;
using System.Collections.Generic;

public abstract class ArmBehaviour : MonoBehaviour
{
    //Esta es la clase padre de ambos brazos del robot.
    
    [SerializeField] protected float activationRadius = 5f;

    [SerializeField] protected float force = 10f;
    
    [SerializeField] protected LayerMask metalMask;

    protected List<Rigidbody> onRange = new List<Rigidbody>();
    
    //Variables para el cono de efectividad de los imanes
    [SerializeField, Range(1f, 180f)] protected float coneAngle = 45f;
    [SerializeField] protected Transform originDirection;
    
    //Funciones que utilizan las clases 
    protected abstract Vector3 DefineDirection(Vector3 objPosition);
    protected abstract Color GizmoColor();


    protected void Awake()
    {
        if (originDirection == null)
            originDirection = transform;
    }

    protected void FixedUpdate()
    {
        metalDetection();
        applyForce();
    }

    protected void metalDetection()
    {
        onRange.Clear();
        Collider[] hits = Physics.OverlapSphere(transform.position, activationRadius, metalMask);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Rigidbody>(out var rb))
                onRange.Add(rb);
        }
    }

    protected bool insideEffectiveArea(Vector3 objPosition)
    {
        if (objPosition == null)
            return false;

        Vector3 toObjDirection = (objPosition - originDirection.position).normalized;
        float angleInBetween = Vector3.Angle(originDirection.forward, toObjDirection);

        return angleInBetween <= coneAngle * 0.5f;
    }

    protected void applyForce()
    {
        foreach(var rb in onRange)
        {
            Vector3 direction = DefineDirection(rb.position);
            rb.AddForce(direction * force);
        }
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = GizmoColor();

        Vector3 origin = originDirection != null ? originDirection.position : transform.position;
        Vector3 forward = originDirection != null ? originDirection.forward : transform.forward;

        // Dibuja los bordes del cono
        Quaternion rotLeft = Quaternion.AngleAxis(-coneAngle * 0.5f, Vector3.up);
        Quaternion rotRight = Quaternion.AngleAxis(coneAngle * 0.5f, Vector3.up);

        Gizmos.DrawRay(origin, rotLeft * forward * activationRadius);
        Gizmos.DrawRay(origin, rotRight * forward * activationRadius);
        Gizmos.DrawWireSphere(origin, activationRadius);
    }
}
