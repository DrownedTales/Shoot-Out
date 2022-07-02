using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DicePhase : MonoBehaviour
{
    [SerializeField] TrivialManager.PhaseType nextPhase;
    public event Action<TrivialManager.PhaseType> onPhaseEnded;

    public void handlePhase(TrivialManager.TurnInfo turn) {
        throwDice(turn);
    }

    private void throwDice(TrivialManager.TurnInfo turn) {
        int result = UnityEngine.Random.Range(1, 7);
        Debug.Log("dice: " + result); //DEBUG
        turn.diceResult = result;
        if (onPhaseEnded != null) onPhaseEnded(nextPhase);
    }
}
