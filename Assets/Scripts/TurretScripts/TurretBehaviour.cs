using UnityEngine;

public class TurretBehaviour : MonoBehaviour
{
    [Header("Caracteristicas de la Torreta")]
    [SerializeField] private float damage = 1f;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float detectionRadius = 10f;

    [Header("Referencia para rotación de la Torreta")]
    [SerializeField] private Transform turretPivot; //Desde donde se gira la torreta
    [SerializeField] private Transform firePoint; //Desde donde sale la bala

    [Header("Proyectil")]
    [SerializeField] private GameObject bulletPrefab;

    [Header("Objetivo")]
    [SerializeField] private LayerMask targetLayer;

    private Transform currentTarget;
    private float nextFireTime;

    // Update is called once per frame
    void Update()
    {
        FindTarget();
        if(currentTarget != null)
        {
            RotateToTarget();
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + (1f / fireRate);
            }
        }
    }

    private bool CheckPlayerInRadius()
    {
        //Obtengo la distancia entre jugador y torreta
        float distanceTarget = Vector3.Distance(this.transform.position, currentTarget.position);
        if (distanceTarget > detectionRadius) //Jugador dentro de area de detección?
        {
            return true;
        }
        return false; 
    }

    private void FindTarget()
    {
        if(currentTarget != null)
        {
            if(CheckPlayerInRadius())//Jugador dentro de área de detección?
            {
                currentTarget = null; //jugador no esta dentro del área
            }
            return; //Si el jugador sigue dentro del área, el "target" queda intacto.
        }
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, targetLayer);
        if (colliders.Length > 0) 
        {
            currentTarget = colliders[0].transform;
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Pasar daño al proyectil si tiene el componente
        TurretBullet bullet = bulletObj.GetComponent<TurretBullet>();
        if (bullet != null)
        {
            bullet.SetDamage(damage);
        }
    }

    private void RotateToTarget()
    {
        // Dirección hacia el objetivo omitiendo la altura para que gire sobre el suelo
        Vector3 targetDirection = currentTarget.position - turretPivot.position;
        targetDirection.y = 0f;

        if (targetDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            turretPivot.rotation = Quaternion.Slerp(turretPivot.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
