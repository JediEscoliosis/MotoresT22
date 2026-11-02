using UnityEngine;

[RequireComponent(typeof(Light))]
public class CambiarColorLuz : MonoBehaviour
{
    [SerializeField] private Color colorActivado = Color.green;

    public void Activar()
    {
        GetComponent<Light>().color = colorActivado;
    }
}
