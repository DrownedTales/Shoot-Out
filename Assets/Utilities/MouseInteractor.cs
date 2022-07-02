using UnityEngine;
using System;

// This is a class to allow INSERTNAMEOFSCRIPT to interact with 3d objects

public class MouseInteractor : MonoBehaviour
{
    public event Action onMouseClick;
    public event Action onMouseHoverEnter;
    public event Action onMouseHoverExit;

    public void callOnMouseClick() {
        if (onMouseClick != null) {
            onMouseClick();
        }
    }
    public void callOnMouseHoverEnter() {
        if (onMouseHoverEnter != null) {
            onMouseHoverEnter();
        }
    }
    public void callOnMouseHoverExit() {
        if (onMouseHoverExit != null) {
            onMouseHoverExit();
        }
    }
}
