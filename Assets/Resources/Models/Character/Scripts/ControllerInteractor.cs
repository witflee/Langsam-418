using UnityEngine;

public class ControllerInteractor : MonoBehaviour
{
    public float interactDistance = 5f;

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            {
                CharacterInteraction character =
                    hit.collider.GetComponent<CharacterInteraction>();
                if (character != null)
                {
                    character.Interact();
                }
            }
        }
    }
}