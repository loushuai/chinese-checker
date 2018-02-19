using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI {
    protected FullBoard board;
    protected int player;

    public AI(FullBoard bd, int pl) {
        board = bd;
        player = pl;
    }

    public virtual void NextMove() {
        Debug.Log("AI::NextMove");
    }

    public virtual bool IsWin() {
        return false;
    }
}
