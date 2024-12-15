using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPadWithAnimation : Interactable
{
    [SerializeField] private GameObject door;
    [SerializeField] private Animator anim;
    private bool doorOpen;
    protected override void Interact()
    {
        Debug.Log("Interacted with" + gameObject.name);
        anim.GetComponent<Animator>().SetBool("Animating", true);

        doorOpen = !doorOpen;
        door.GetComponent<Animator>().SetBool("IsOpen", doorOpen);
    }
}
