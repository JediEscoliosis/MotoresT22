using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Panel de muerte")]
    [SerializeField] private GameManager gameManager; // arrastrar el objeto que tiene el GameManager

    void Die()
    {
        gameManager.ShowDeathScreen();
    }
}