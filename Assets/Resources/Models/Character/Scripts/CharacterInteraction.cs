using UnityEngine;
using TMPro;
using System.Collections;

public class CharacterInteraction : MonoBehaviour
{
    public Animator animator;
    public TextMeshPro dialogueText; 
    public string dialogueMessage = "Hello!";
    public float displayDuration = 3f;

    private Coroutine _hideCoroutine;

    public void Interact()
    {
        animator.SetTrigger("Talk");
        Debug.Log("Hello");

        ShowText();
    }

    private void ShowText()
    {
        // If already showing, cancel the previous hide timer
        if (_hideCoroutine != null)
            StopCoroutine(_hideCoroutine);

        dialogueText.text = dialogueMessage;
        dialogueText.gameObject.SetActive(true);

        _hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        dialogueText.gameObject.SetActive(false);
    }
}