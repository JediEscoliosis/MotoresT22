using UnityEngine;
using UnityEngine.SceneManagement;

public class Menú : MonoBehaviour
{
    public GameObject mainMenu;

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Saliendo Del Juego...");
    }    

    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }    
}