using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [Header("Panel de muerte")]
    [SerializeField] private GameManager gameManager; // arrastrar el objeto que tiene el GameManager

    public void Die()
    {
        gameManager.ShowDeathScreen();
    }
}