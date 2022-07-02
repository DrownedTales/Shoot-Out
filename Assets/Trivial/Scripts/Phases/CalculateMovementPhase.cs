using System;
using System.Collections.Generic;
using UnityEngine;

public class CalculateMovementPhase : MonoBehaviour
{
    [SerializeField] TrivialManager.PhaseType nextPhase;
    public event Action<TrivialManager.PhaseType> onPhaseEnded;

    public void handlePhase(TrivialManager.TurnInfo turn, List<GameObject> pawnsSquares) {
        turn.availableSquares = getMoveSquares(turn.diceResult, 
            pawnsSquares[turn.playerIndex],
            new List<GameObject>(), 
            new List<GameObject>());
        onPhaseEnded(nextPhase);
    }

    private List<GameObject> getMoveSquares(int i, GameObject point, List<GameObject> visited, List<GameObject> res) {
        List<GameObject> adjacents = point.GetComponent<MovementPoint>().getAdjacents();
        visited.Add(point);
        for (int g = 0; g < adjacents.Count; g++) {
            if (visited.Contains(adjacents[g])) continue;
            if (i > 1) {
                getMoveSquares(i - 1, adjacents[g], visited, res);
            } else {
                res.Add(adjacents[g]);
                adjacents[g].GetComponent<MovementPoint>().select(); //DEBUG
            }
        }
        return res;
    }

}
