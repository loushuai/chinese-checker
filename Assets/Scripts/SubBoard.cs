using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Directions: 0: →, 1: ↘, 2: ↙, 3: ←, 4: ↖, 5: ↗
/// </summary>

public class SubBoard : Board {
    public static int SIZE = 5;
    const float SQRT_3 = 1.732f;
    private float BLOCK_WIDTH = 55;

    Block[,] board = new Block[SIZE, SIZE];
    private Vector2 offset = new Vector2(0f, 0f);
    private Quaternion rotation = Quaternion.identity;
    private Transform trans;

    private void Awake() {
        trans = GetComponent<Transform>();
        Init();
    }

    public void InitNeighbors(int diroffset) {
        for (int i = 0; i < SIZE; ++i) {
            for (int j = 0; j < SIZE; ++j) {
                Block blk = GetBlock(i, j);
                if (j + 1 < SIZE) {
                    blk.SetNeighbor((1 + diroffset) % 6, board[i, j + 1]);
                }
                if (j - 1 >= 0) {
                    blk.SetNeighbor((4 + diroffset) % 6, board[i, j - 1]);
                }
                if (i + 1 < SIZE) {
                    blk.SetNeighbor((2 + diroffset) % 6, board[i + 1, j]);
                }
                if (i - 1 >= 0) {
                    blk.SetNeighbor((5 + diroffset) % 6, board[i - 1, j]);
                }
                if (i + 1 < SIZE && j - 1 >= 0) {
                    blk.SetNeighbor((3 + diroffset) % 6, board[i + 1, j - 1]);
                }
                if (i - 1 >= 0 && j + 1 < SIZE) {
                    blk.SetNeighbor((0 + diroffset) % 6, board[i - 1, j + 1]);
                }
            }
        }
    }

    public override void Init(List<int> players=null) 
    {
        for (int i = 0; i < SIZE; ++i) {
            for (int j = 0; j < SIZE; ++j) {
                GameObject obj = Instantiate(blockPrefab, trans, false);
                board[i, j] = obj.GetComponent<Block>();

                float firstX = -i * BLOCK_WIDTH / 2;
                float firstY = -i * BLOCK_WIDTH * SQRT_3 / 2;
                obj.GetComponent<Transform>().Translate(new Vector3(firstX + j * BLOCK_WIDTH / 2, firstY - j * BLOCK_WIDTH * SQRT_3 / 2, 100f));
            }
        }
    }

    public override Block GetBlock(int row, int col, int subid=0) 
    {
        return board[row, col];
    }
}
