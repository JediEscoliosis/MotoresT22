using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance;

    private int collectibleCount;

    private void Awake()
    {
        Instance = this;
    }

    public void AddCollectible()
    {
        collectibleCount++;

        Debug.Log("Coleccionales: " + collectibleCount);
    }

    public int GetCollectibleCount()
    {
        return collectibleCount;
    }    
}