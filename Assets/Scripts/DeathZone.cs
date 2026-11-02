using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Has Muerto");

            PlayerState player = other.GetComponent<PlayerState>();

            if (player != null)
            {
                player.Die();
            }
        }
    }
}