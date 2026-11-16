using UnityEngine;

public class Collectible : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectibleManager.Instance.AddCollectible();

            Destroy(gameObject);
        }
    }
}
