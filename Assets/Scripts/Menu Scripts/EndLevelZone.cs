using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider))]
public class EndLevelZone : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Rigidbody player;
    [SerializeField] private GameObject endPanel;

    [Header("Escena del menu")]
    [SerializeField] private string menuSceneName = "Menu";

    [Header("Scripts a desactivar al finalizar")]
    [Tooltip("Incluye GameManager y los controles del jugador. Deja activo el EventSystem.")]
    [SerializeField] private MonoBehaviour[] scriptsToDisable = new MonoBehaviour[0];

    private bool activated;
    private bool isLoadingMenu;

    private void Reset()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;


        // El panel permanece oculto hasta que entra el jugador.
        endPanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActiveAndEnabled || activated || player == null || endPanel == null)
            return;

        if (other.isTrigger || other.attachedRigidbody != player)
            return;

        activated = true;

        // Evita que los controles o el menu de pausa interfieran.
        if (scriptsToDisable != null)
        {
            foreach (MonoBehaviour script in scriptsToDisable)
            {
                if (script != null && script != this)
                    script.enabled = false;
            }
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        endPanel.SetActive(true);
    }

    // Conecta este metodo al On Click del boton "Volver al menu".
    public void VolverAlMenu()
    {
        if (!activated || isLoadingMenu)
            return;


        isLoadingMenu = true;

        // Restaura el tiempo para que el menu funcione normalmente.
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadSceneAsync(menuSceneName, LoadSceneMode.Single);
    }
}

