using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private AudioClip colect1;
    [SerializeField] private AudioClip voiceLine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectibleManager.Instance.AddCollectible();

            SoundController.instance.SoundExecute(colect1, voiceLine);

            Destroy(gameObject);
        }
    }
}