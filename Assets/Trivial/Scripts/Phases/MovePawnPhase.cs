using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MovePawnPhase : MonoBehaviour
{
    [SerializeField] TrivialManager.PhaseType nextPhase;
    [SerializeField] private float pawnMoveTime;
    [SerializeField] AnimationCurve pawnMovementCurve;
    [SerializeField] AnimationCurve pawnJumpCurve;
    [SerializeField] float pawnJumpHeight;
    [SerializeField] float pawnRotateTime;

    public event Action<TrivialManager.PhaseType> onPhaseEnded;

    private TrivialManager.TurnInfo currentTurn;
    private List<GameObject> pawnsSquares;
    private GameObject squares;
    private bool setted = false;
    
    public void handlePhase(TrivialManager.TurnInfo turn, List<GameObject> pS, GameObject s) {
        currentTurn = turn;
        pawnsSquares = pS;
        squares = s;
        if (!setted) {
            for (int i = 0; i < squares.transform.childCount; i++) {
                squares.transform.GetChild(i).GetComponent<MovementPoint>().onClicked += handleSquareClick;
            }
            setted = true;
        }
    }

    private void handleSquareClick(GameObject point) {
        if (currentTurn.availableSquares.Contains(point)) {

            for (int i = 0; i < squares.transform.childCount; i++) {
                squares.transform.GetChild(i).GetComponent<MovementPoint>().deselect(); //DEBUG
            }

            List<GameObject> path = getPath(point, pawnsSquares[currentTurn.playerIndex]);
            movePawnAlongPath(currentTurn.pawn, path, 0);

            pawnsSquares[currentTurn.playerIndex] = point;
        }
    }

    private void movePawnAlongPath(GameObject pawn, List<GameObject> path, int i) {
        Action a = () => movePawnAlongPath(pawn, path, i + 1);
        Action callback = i < path.Count - 1 ? a : onPawnMoved;

        LeanTween.moveX(pawn,
            path[i].transform.position.x,
            pawnMoveTime)
            .setEase(pawnMovementCurve);
        LeanTween.moveZ(pawn,
            path[i].transform.position.z,
            pawnMoveTime)
            .setEase(pawnMovementCurve);
        LeanTween.moveY(pawn,
            pawnJumpHeight,
            pawnMoveTime)
            .setEase(pawnJumpCurve)
            .setOnComplete(callback);
        LeanTween.rotateY(pawn, 
            pawn.transform.rotation.eulerAngles.y 
            + Vector3.SignedAngle(pawn.transform.forward,
            path[i].transform.position - pawn.transform.position, 
            Vector3.up),
            pawnRotateTime);
    }

    private void onPawnMoved() {
        onPhaseEnded(nextPhase);
    }


    private List<GameObject> getPath(GameObject target, GameObject start) {
        List<GameObject> res = new List<GameObject>();
        getPathAux(target, start, new List<GameObject>(), new Queue<GameObject>(), res);
        res.RemoveAt(0);
        return res;
    }

    private GameObject getPathAux(GameObject target, GameObject point, List<GameObject> visited, 
        Queue<GameObject> queue, List<GameObject> res) {

        if (point == target) {
            res.Insert(0, point);
            return point;
        }
        visited.Add(point);
        List<GameObject> adjacents = point.GetComponent<MovementPoint>().getAdjacents();
        adjacents.Where(x => !visited.Contains(x))
            .ToList()
            .ForEach(x => queue.Enqueue(x));
        GameObject next = queue.Dequeue();
        GameObject value = getPathAux(target, next, visited, queue, res);
        if (value != null) {
            if (adjacents.Contains(value)) {
                res.Insert(0, point);
                return point;
            }
            return value;
        }
        return null;
    }


}
