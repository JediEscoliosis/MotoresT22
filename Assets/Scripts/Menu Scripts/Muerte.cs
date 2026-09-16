using UnityEngine;
using UnityEngine.SceneManagement;

public class Muerte : MonoBehaviour
{
    public GameObject menuMuerte;
    public bool juegoPausado = false;

    private void Awake()
    {
        Debug.Log("Muerte.cs está funcionando");
    }

    public void Reiniciar()
    {
        Debug.Log("REINICIANDO...");

        Time.timeScale = 1f;

        Scene escenaActual = SceneManager.GetActiveScene();

        Debug.Log("Escena: " + escenaActual.name);
        Debug.Log("Build Index: " + escenaActual.buildIndex);

        SceneManager.LoadScene(escenaActual.buildIndex);
    }

    public void Salir()
    {
        Debug.Log("Cerrando Juego...");
        Application.Quit();
    }
}