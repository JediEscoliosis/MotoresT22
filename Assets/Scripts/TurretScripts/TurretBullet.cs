using UnityEngine;

public class TurretBullet : MonoBehaviour
{
    [SerializeField] private float speed = 25f;
    [SerializeField] private float lifeTime = 10f;

    private float damage;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; 
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb.linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifeTime);
    }

    public void SetDamage(float amount)
    {
        damage = amount;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 1) El escudo se revisa PRIMERO, antes que al jugador
        Shield shield = other.GetComponentInParent<Shield>();
        if (shield != null)
        {
            if (shield.IsActive)
                Destroy(gameObject);   // escudo activo: la bala se rompe, nadie muere

            return;                    // escudo inactivo: la bala lo ignora y sigue hacia el jugador
        }

        // 2) Jugador
        PlayerState playerState = other.GetComponentInParent<PlayerState>();
        if (playerState != null)
        {
            playerState.Die();
            Destroy(gameObject);
            return;
        }

        // 3) Cualquier otra cosa (paredes, etc.)
        Destroy(gameObject);
    }
}
