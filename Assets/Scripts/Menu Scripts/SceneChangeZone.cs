using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
[RequireComponent(typeof(BoxCollider))]
public class SceneChangeZone : MonoBehaviour
{
    [Header("Jugador")]
    [Tooltip("Arrastra el objeto de la escena que tiene el Rigidbody del Player.")]
    [SerializeField] private Rigidbody player;

    [Header("Escena de destino")]
    [Tooltip("Nombre de la escena que quieres cargar, sin la extension .unity.")]
    [SerializeField] private string sceneName = "Nivel2";

    private bool isLoading;

    private void Reset()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void Awake()
    {
        // La zona detecta al jugador sin bloquearle el paso.
        GetComponent<BoxCollider>().isTrigger = true;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActiveAndEnabled || isLoading || player == null)
            return;

        // Solo acepta colliders solidos del Rigidbody del jugador asignado.
        if (other.isTrigger || other.attachedRigidbody != player)
            return;

        // Comprueba el nombre y que la escena este incluida para cargarla.
        if (string.IsNullOrWhiteSpace(sceneName) ||
            !Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError(
                $"No se puede cargar '{sceneName}'. Revisa Scene Name y agrega " +
                "la escena a la Scene List de Build Profiles, con su casilla marcada.",
                this);
            return;
        }

        // Evita solicitar varias cargas si el jugador tiene varios colliders.
        isLoading = true;

        // El siguiente nivel comienza con el tiempo normal.
        Time.timeScale = 1f;

        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
    }
}