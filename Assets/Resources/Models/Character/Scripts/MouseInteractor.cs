using UnityEngine;

public class MouseInteractor : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
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