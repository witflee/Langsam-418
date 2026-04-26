using UnityEngine;

public class CharacterInteraction : MonoBehaviour
{
    public Animator animator;
    //public GameObject textBubble;

    public void Interact()
    {
        animator.SetTrigger("Talk");
        //textBubble.SetActive(true);
        Debug.Log("Hello");
    }
}