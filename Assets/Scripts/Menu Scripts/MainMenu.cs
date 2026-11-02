using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Configuración de Audio")]
    [SerializeField] private AudioSource sonidoBoton;
    [SerializeField] private float tiempoDeEspera = 1.0f;

    public void IniciarJuego()
    {
        StartCoroutine(IniciarJuegoConRetraso());
    }

    private IEnumerator IniciarJuegoConRetraso()
    {
        if (sonidoBoton != null)
        {
            sonidoBoton.Play();
        }

        yield return new WaitForSeconds(tiempoDeEspera);

        SceneManager.LoadScene("SampleScene");
    }

    public void SalirJuego()
    {
        StartCoroutine(SalirJuegoConRetraso());
    }

    private IEnumerator SalirJuegoConRetraso()
    {
        if (sonidoBoton != null)
        {
            sonidoBoton.Play();
        }

        yield return new WaitForSeconds(tiempoDeEspera);

        Debug.Log("Saliendo del juego...");

        Application.Quit();
    }
}