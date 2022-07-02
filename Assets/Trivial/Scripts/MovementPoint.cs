using System.Collections.Generic;
using UnityEngine;
using System;

public class MovementPoint : MonoBehaviour
{
    [SerializeField] private List<GameObject> adjacents;
    [SerializeField] private float adjacencyDistance;
    public event Action<GameObject> onClicked;
    private MouseInteractor interactor;

    //DEBUG start
    private Color baseColor;
    private bool selected;
    private Renderer matRenderer;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color hoverColor;

    private void Awake() {
        matRenderer = transform.GetChild(0).GetComponent<Renderer>();
        Debug.Assert(matRenderer != null);
        baseColor = matRenderer.material.color;

        interactor = transform.GetChild(0).GetComponent<MouseInteractor>();
        Debug.Assert(interactor != null);
        interactor.onMouseClick += onMouseClick;
        interactor.onMouseHoverEnter += onHoverEnter;
        interactor.onMouseHoverExit += onHoverExit;
    }

    private void onHoverEnter() {
        if (selected) {
            matRenderer.material.color = hoverColor;
            matRenderer.material.EnableKeyword("_EMISSION");
        }
    }

    public void select() {
        selected = true;
        matRenderer.material.color = selectedColor;
    }
    public void deselect() {
        selected = false;
        matRenderer.material.color = baseColor;
    }
    
    private void onHoverExit() {
        matRenderer.material.DisableKeyword("_EMISSION");
        if (!selected) {
            matRenderer.material.color = baseColor;
        } else {
            matRenderer.material.color = selectedColor;
        }
    }

    //DEBUG end

    public void onMouseClick() {
        if (onClicked != null) {
            onClicked(gameObject);
        }
    }

    [ContextMenu("Reset Adjacents")]
    public void resetAdjacent() {
        adjacents = new List<GameObject>();
    }

    public List<GameObject> getAdjacents() {
        return adjacents;
    }

    public void addAdjacent(GameObject o) {
        adjacents.Add(o);
    }

    [ContextMenu("Set Adjacents")]
    public void setAdjacents() {
        for (int i = 0; i < transform.parent.childCount; i++) {
            GameObject other = transform.parent.GetChild(i).gameObject;
            if (other == gameObject || adjacents.Contains(other)) {
                continue;
            }
            float distance = Vector3.Distance(other.transform.position, transform.position);
            if ((distance / transform.localScale.x) <= adjacencyDistance) {
                adjacents.Add(other);
                MovementPoint otherScript = other.GetComponent<MovementPoint>();
                if (!otherScript.getAdjacents().Contains(gameObject)) {
                    otherScript.addAdjacent(gameObject);
                }
                Debug.DrawLine(other.transform.position, transform.position, Color.green, 10.0f);
            }
        }
    }
}
