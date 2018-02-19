using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAI : AI {
    const float MINVALUE = 136120f;

    public SimpleAI(FullBoard bd, int pl) : base(bd, pl) {
        
    }

    public override void NextMove() {
        Piece bestPiece = null;
        Vector3 bestMove = new Vector3();
        float maxGain = 0f;

        foreach (Piece piece in board.pieces[player]) {
            piece.UpdateAvaliableMove();
            foreach (Vector3 move in piece.avaliableMoves) {
                float diff = piece.ValueDiff(move);
                if (-diff >= maxGain) {
                    bestMove = move;
                    bestPiece = piece;
                    maxGain = -diff;
                }
            }
        }

        if (bestPiece != null) {
            bestPiece.MoveTo(bestMove);
        }
    }
}
