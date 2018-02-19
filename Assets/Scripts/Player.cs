using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {
    public bool isAI;
    public int id;
    public AI brain = null;

    public Player(int playerID, bool ai) {
        this.id = playerID;
        this.isAI = ai;
    }

    public bool IsWin() {
        if (brain != null) {
            return brain.IsWin();
        }

        return false;
    }

    bool DetectClick() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                Debug.Log("Hit by mouse\t");
                ClickableMonoBehaviour clickable = hit.transform.GetComponent<ClickableMonoBehaviour>();

                Debug.LogFormat("type {0}, id {1}", clickable.type, clickable.id);
                if (clickable.type == 1) {
                    Piece piece = (Piece)clickable;
                    if (piece.player != this.id) {
                        return false;
                    }
                }

                int moved = clickable.OnClick();
                return moved != 0;
            }
        }

        return false;
    }

    public int Go(int turn) {
        if (this.isAI) {
            brain.NextMove();
            return turn + 1;
        }

        if (DetectClick()) {
            return turn + 1;
        }

        return turn;
    }
}
