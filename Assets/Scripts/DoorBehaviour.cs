using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
    public Animator doorAnimation;

    private void OntriggerEnter(Collider other)
    {
        doorAnimation.Play("DoorOpen");
    }

    private void OntriggerExit(Collider other)
    {
        doorAnimation.Play("DoorClose");
    }
}
