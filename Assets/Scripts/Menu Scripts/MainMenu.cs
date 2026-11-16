using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void IniciarJuego()
    {
        StartCoroutine(CargarConRetraso("Nivel 1"));
    }

    IEnumerator CargarConRetraso(string escena)
    {
        yield return new WaitForSecondsRealtime(0.25f);
        SceneManager.LoadScene(escena);
    }

    public void SalirJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}