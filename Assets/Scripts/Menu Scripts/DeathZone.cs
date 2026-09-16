using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Has Muerto");

            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                player.RecieveDamage(Vector2.zero, 999);
            }
        }
    }
}