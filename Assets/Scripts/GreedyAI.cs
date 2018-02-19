using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreedyAI : AI {
    const float POSINF = 99999999f;
    const float WINVALUE = -88888888f;
    const float MINVALUE = 140000f;

    int maxDepth = 1;
    Piece bestPiece = null;
    Vector3 bestMove;

    public GreedyAI(FullBoard bd, int pl, int depth=1) : base(bd, pl) {
        maxDepth = depth;
    }

    public override bool IsWin() {
        return board.Evaluate(player) < MINVALUE;
    }

    public override void NextMove() {
        Debug.Log("MinMaxAI::NextMove");
        AlphaBeta(maxDepth);
        if (bestPiece != null) {
            bestPiece.UpdateAvaliableMove();
            //bestPiece.MoveTo(bestMove);
            bestPiece.JumpTo(bestMove);
        }
    }

    float AlphaBeta(int depth) {
        float bestValue = POSINF;

        if (board.Evaluate(player) < MINVALUE) {
            return WINVALUE;
        }

        if (depth == 0) {
            return board.Evaluate(player);
        }

        foreach (Piece piece in board.pieces[player]) {
            piece.UpdateAvaliableMove();
            HashSet<Vector3> avaliableMoves = new HashSet<Vector3>(piece.avaliableMoves);

            foreach (Vector3 move in avaliableMoves) {
                Vector3 org = piece.MoveTo(move, false);

                float value = AlphaBeta(depth - 1);
                value += (maxDepth - depth)*10;

                piece.MoveTo(org, false);

                if (value < bestValue || ((value == bestValue) && (Random.Range(0f, 1f) > 0.5f))) {
                    bestValue = value;
                    if (depth == maxDepth) {
                        bestPiece = piece;
                        bestMove = move;
                    }
                }

                if (bestValue == WINVALUE) {
                    return WINVALUE;
                }
            }
        }

        return bestValue;
    }
}
