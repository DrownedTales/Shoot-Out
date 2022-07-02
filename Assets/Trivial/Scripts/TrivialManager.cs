using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class TrivialManager : MonoBehaviour
{
    public enum PhaseType {
        ThrowDice,
        CalculateMovement,
        MovePawn,
        End
        }
    public class TurnInfo {
        public TurnInfo(int playerIndex, GameObject pawn) {
            this.playerIndex = playerIndex;
            this.pawn = pawn;
        }
        public List<GameObject> availableSquares;
        public GameObject pawn;
        public int playerIndex;
        public int diceResult;
    }
    [SerializeField] private List<GameObject> pawnsSquares;
    [SerializeField] private List<GameObject> pawns;
    private List<GameObject> squares = new List<GameObject>();
    private TurnInfo currentTurn;

    [SerializeField] private DicePhase dicePhase;
    [SerializeField] private CalculateMovementPhase calculatePhase;
    [SerializeField] private MovePawnPhase movePhase;
    [SerializeField] private PhaseType firstPhase;
    private bool endedTurn = true; //DEBUG

    
    [ContextMenu("Build Board")]
    private void buildBoard() {
        GameObject squares = transform.Find("Squares").gameObject;
        for (int i = 0; i < squares.transform.childCount; i++) {
            squares.transform.GetChild(i).GetComponent<MovementPoint>().resetAdjacent();
        }
        for (int i = 0; i < squares.transform.childCount; i++) {
            squares.transform.GetChild(i).GetComponent<MovementPoint>().setAdjacents();
        }
    }

    private void Start() {
        dicePhase.onPhaseEnded += continueTurn;
        calculatePhase.onPhaseEnded += continueTurn;
        movePhase.onPhaseEnded += continueTurn;

        resetRound();
    }

    private void resetRound() {
        currentTurn = new TurnInfo(-1, null);
        endedTurn = true;
    }

    [ContextMenu("Start Turn")]
    private void startTurn() {
        if (!endedTurn) return;
        endedTurn = false;

        currentTurn = new TurnInfo(currentTurn.playerIndex + 1, pawns[currentTurn.playerIndex + 1]);
        continueTurn(firstPhase);
    }

    private void continueTurn(PhaseType nextPhase) {
        switch (nextPhase) {
            case PhaseType.ThrowDice:
                dicePhase.handlePhase(currentTurn);
                break;
            case PhaseType.CalculateMovement:
                calculatePhase.handlePhase(currentTurn, pawnsSquares);
                break;
            case PhaseType.MovePawn:
                movePhase.handlePhase(currentTurn, pawnsSquares, transform.Find("Squares").gameObject);
                break;
            case PhaseType.End:
                if (currentTurn.playerIndex == pawns.Count-1) {
                    resetRound();
                }
                startTurn();
                break;
        }
    }
}
