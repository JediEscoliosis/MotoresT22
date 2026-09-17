using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
    [SerializeField] private Animator doorAnimation;

    //private static bool isOpen = false;

    public void OpenDoor()
    {
        doorAnimation.SetBool("isOpen", true);
        //doorAnimation.SetBool("isClosed", false);
    }

    public void CloseDoor()
    {
        doorAnimation.SetBool("isOpen", false);
        //doorAnimation.SetBool("isClosed", true);
    }
}