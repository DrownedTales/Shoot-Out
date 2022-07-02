using UnityEngine;

public class MouseRayCast : MonoBehaviour
{
    private GameObject hovered = null;
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  
        RaycastHit hit;
        bool hitted = Physics.Raycast(ray, out hit);
        if (hitted) {
            MouseInteractor interactor =  hit.transform.gameObject.GetComponent<MouseInteractor>();
            

            if (interactor != null) {
                if (hovered != hit.transform.gameObject) {
                    interactor.callOnMouseHoverEnter();
                }
                
                if (Input.GetMouseButtonDown(0)) {
                    interactor.callOnMouseClick();
                }
            }

            if (hovered != hit.transform.gameObject) {
                if (hovered != null) {
                    MouseInteractor interactor2 = hovered.GetComponent<MouseInteractor>();
                    if (interactor2 != null) {
                        interactor2.callOnMouseHoverExit();
                    }
                }
                hovered = hit.transform.gameObject;
            }

        } else {
            if (hovered != null) {
                MouseInteractor interactor = hovered.GetComponent<MouseInteractor>();

                if (interactor != null) {
                    interactor.callOnMouseHoverExit();
                }
            }
        }
    }
}
