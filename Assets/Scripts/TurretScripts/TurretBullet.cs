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
        PlayerState playerState = other.GetComponentInParent<PlayerState>();

        if (playerState != null)
        {
            playerState.Die();
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject);
    }
}
