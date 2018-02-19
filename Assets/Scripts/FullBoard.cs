using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Directions: 0: →, 1: ↘, 2: ↙, 3: ←, 4: ↖, 5: ↗
/// </summary>

public class FullBoard : Board {
    public GameObject subBoardPrefab;
    public List<Piece>[] pieces = new List<Piece>[6];
    private SubBoard[] subBoard = new SubBoard[6];
    private Hashtable blocks = new Hashtable();
    float[] playerValues = { 0f, 0f, 0f, 0f, 0f, 0f };
    public static Hashtable clickables = new Hashtable();
    static int nextId = 0;

    static void AddClickable(ClickableMonoBehaviour clickable, int type) {
        clickable.type = type;
        clickable.SetId(nextId++);
        clickables.Add(clickable.id, clickable);
    }

    public override Block GetBlock(int row, int col, int subid) {
        return subBoard[subid].GetBlock(row, col);
    }

    private void UpdateNeighbors(Block dst, Block src) {
        for (int i = 0; i < 6; ++i) {
            if (dst.neighbors[i] == null) {
                dst.neighbors[i] = src.neighbors[i];
            }
        }
    }

    float SquareDistance(Block blk1, Block blk2) {
        Vector3 pos1 = blk1.GetPos();
        Vector3 pos2 = blk2.GetPos();
        return (pos1.x - pos2.x) * (pos1.x - pos2.x) + (pos1.y - pos2.y) * (pos1.y - pos2.y);
    }

    Block ContainsBlock(Block blk) {
        const float err = 1.0f;

        if (blk == null) {
            return null;
        }

        float key = blk.GetKey();
        foreach (DictionaryEntry de in blocks) {
            Block exist = (Block)de.Value;
            if (SquareDistance(blk, exist) < err) {
                return exist;
            }
        }

        return null;
    }

    public Block GetBlock(Vector3 pos) {
        float key = Block.Key(pos);
        return GetBlock(key);
    }

    public Block GetBlock(float key) {
        if (blocks.Contains(key)) {
            return (Block)blocks[key];
        }
        return null;
    }

    void AddBlock(Block blk) {
        float key = blk.GetKey();
        blocks.Add(key, blk);
        blk.SetBoardLayer();
        blk.SetBoard(this);
        AddClickable(blk, 0);
    }

    void UnifySubBoards() {
        for (int i = 0; i < 6; ++i) {
            for (int row = 0; row < SubBoard.SIZE; ++row) {
                for (int col = 0; col < SubBoard.SIZE; ++col) {
                    Block blk = subBoard[i].GetBlock(row, col);
                    Block exist = ContainsBlock(blk);
                    if (exist == null) {
                        AddBlock(blk);
                    } else {
                        UpdateNeighbors(exist, blk);
                    }
                }
            }
        }

        foreach (DictionaryEntry de in blocks) {
            Block blk = (Block)de.Value;
            for (int i = 0; i < 6; ++i) {
                if (blk.neighbors[i] != null) {
                    blk.neighbors[i] = ContainsBlock(blk.neighbors[i]);
                }
            }
        }
    }

    public void Init(List <Player> players) {
        for (int i = 0; i < 6; ++i) {
            GameObject obj = Instantiate(subBoardPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 60 * i), transform) as GameObject;
            subBoard[i] = obj.GetComponent<SubBoard>();
            subBoard[i].InitNeighbors(6 - i);
        }

        // init block hash table
        UnifySubBoards();

        //Debug.LogFormat("block num {0}", blocks.Count);

        // init players
        foreach (Player plr in players) {
            if (plr.id > 5) {
                continue;
            }
            InitPlayer(plr.id);
        }
    }

    void InitPlayer(int player) {
        Vector3 target = subBoard[(player + 3) % 6].GetBlock(SubBoard.SIZE - 1, SubBoard.SIZE - 1, player).GetPos();
        pieces[player] = new List<Piece>();
        for (int len = SubBoard.SIZE - 1; len > 0; --len) {
            for (int r = len; r > 0; --r) {
                Block blk = GetBlock(len, SubBoard.SIZE - r, player);
                Vector3 pos = blk.GetPos();
                GameObject obj = Instantiate(PiecePrefab, new Vector3(pos.x, pos.y, 1f), Quaternion.identity) as GameObject;
                Piece pie = obj.GetComponent<Piece>();
                pie.Init(this, player);
                pie.target = target;
                pieces[player].Add(pie);
                blk.isOccupied = true;
                AddClickable(pie, 1);
            }
        }

        // init target
        CalcPlayerValue(player);
    }

    public void LooksLikeInitPlayer(int player, bool clear) {
        for (int len = SubBoard.SIZE - 1; len > 0; --len) {
            for (int r = len; r > 0; --r) {
                Block blk = GetBlock(len, SubBoard.SIZE - r, player);
                if (!clear) {
                    blk.ChangeColor(Piece.color[player]);    
                } else {
                    blk.ChangeColor(new Color(1f, 1f, 1f, 1f));
                }
            }
        }
    }

    void Awake() {
    }

    public void SetColor(HashSet<Vector3> avaliableMoves, Color color) {
        foreach (Vector3 pos in avaliableMoves) {
            float key = Block.Key(pos);
            Block blk = GetBlock(key);
            blk.ChangeColor(color);
        }
    }

    public void CalcPlayerValue(int player) {
        float value = 0f;
        foreach (Piece piece in pieces[player]) {
            value += piece.Distance2();
        }
        playerValues[player] = value;
    }

    public void UpdatePlayerValue(int player, float delta) {
        playerValues[player] += delta;
    }

    public float Evaluate(int player) {
        return playerValues[player];
    }

    void DetectClick() {
        if (Input.GetMouseButtonDown(0)) {
            Debug.Log("Get mouse button down");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                Debug.Log("Hit by mouse\t");
                ClickableMonoBehaviour clickable = hit.transform.GetComponent<ClickableMonoBehaviour>();
                clickable.OnClick();
            }
        }
    }

    void Update() {
        //DetectClick();
    }
}
