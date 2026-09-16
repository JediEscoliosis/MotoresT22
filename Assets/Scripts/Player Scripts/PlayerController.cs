using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private int vidaMaxima = 100;

    [Header("Panel de muerte")]
    [SerializeField] private GameObject deathPanel;

    public bool EstaMuerto { get; private set; }

    private int vidaActual;

    private void Awake()
    {
        vidaActual = vidaMaxima;
        EstaMuerto = false;

        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }
    }

    public void RecieveDamage(Vector2 direccion, int cantidadDanio)
    {
        vidaActual -= cantidadDanio;

        Debug.Log("Daño recibido: " + cantidadDanio);
        Debug.Log("Vida restante: " + vidaActual);

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        EstaMuerto = true;

        Debug.Log("Jugador muerto");

        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0f;
    }
}