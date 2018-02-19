using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : ClickableMonoBehaviour {
    private SpriteRenderer spriteRenderer;
    public float width;
    public float height;
    public Block[] neighbors = { null, null, null, null, null, null };
    public bool isOccupied = false;
    Board board;

    public static float Key(Vector3 pos) {
        return Mathf.Round(pos.x * 1000) + Mathf.Round(pos.y);
    }

    public Vector3 GetPos() {
        return transform.parent.transform.parent.TransformPoint(transform.position);
    }

    public float GetKey() {
        Vector3 pos = GetPos();
        return Key(pos);
    }

    public void SetBoard(Board bd) {
        board = bd;
    }

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        width = spriteRenderer.size.x;
        height = spriteRenderer.size.y;
    }

    public void SetNeighbor(int direction, Block neighbor) {
        neighbors[direction] = neighbor;
    }

    public void SetBoardLayer() {
        spriteRenderer.sortingLayerName = "Board";
        GetComponent<Transform>().Translate(new Vector3(0, 0, -50f));
    }

    public void ChangeColor(Color color) {
        spriteRenderer.color = color;
    }

    void AddNextMove(HashSet<Vector3> currentSet, Hashtable path, Vector3 currentPos, Vector3 nextPos) {
        if (path.Contains(nextPos)) {
            return;
        }

        currentSet.Add(nextPos);
        path.Add(nextPos, currentPos);
    }

    void AvaliableJump(int direction, HashSet<Vector3> currentSet, Hashtable path) {
        if (neighbors[direction] == null || neighbors[direction].isOccupied == false) {
            return;
        }

        Block next = neighbors[direction].neighbors[direction];
        if (next == null || next.isOccupied == true) {
            return;
        }

        if (currentSet.Contains(next.GetPos())) {
            return;
        }

        //currentSet.Add(next.GetPos());
        AddNextMove(currentSet, path, GetPos(), next.GetPos());

        for (int dir = 0; dir < 6; ++dir) {
            next.AvaliableJump(dir, currentSet, path);
        }
    }

    public void AvaliableMove(HashSet<Vector3> avaliableMoves, Hashtable pathes) {
        avaliableMoves.Clear();
        pathes.Clear();

        for (int dir = 0; dir < 6; ++dir) {
            Block neighbor = neighbors[dir];
            if (neighbor == null) {
                continue;
            } else if (neighbor.isOccupied == false) {
                //avaliableMoves.Add(neighbor.GetPos());
                AddNextMove(avaliableMoves, pathes, GetPos(), neighbor.GetPos());
            } else {
                AvaliableJump(dir, avaliableMoves, pathes);
            }
        }
    }

    public override int OnClick() {
        if (board.selectedPiece == null) {
            return 0;
        }

        // move piece
        int moved = board.selectedPiece.JumpTo(GetPos());


        board.selectedPiece.UnSelect();
        board.selectedPiece = null;

        return moved;
    }
}
