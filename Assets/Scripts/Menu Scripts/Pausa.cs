using UnityEngine;
using UnityEngine.SceneManagement;

public class Pausa : MonoBehaviour
{
    public GameObject menuPausa;
    public bool juegoPausado = false;
    [SerializeField] private PlayerController playerController;

    private void Update()
    {
        if (playerController != null && playerController.EstaMuerto)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (juegoPausado)
            {
                Reanudar();
            }
            else
            {
                Pausar();
            }
        }
    }

    public void Pausar()
    {
        menuPausa.SetActive(true);
        Time.timeScale = 0;
        juegoPausado = true;
    }

    public void Reanudar()
    {
        Debug.Log("REANUDAR EJECUTADO");

        Time.timeScale = 1;
        juegoPausado = false;

        menuPausa.SetActive(false);

        Debug.Log("TimeScale: " + Time.timeScale);
        Debug.Log("Panel activo: " + menuPausa.activeSelf);
    }

    public void Reiniciar()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Salir()
    {
        Debug.Log("Cerrando Juego...");
        Application.Quit();
    }
}