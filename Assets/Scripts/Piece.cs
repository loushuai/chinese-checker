using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : ClickableMonoBehaviour {
    static public Color[] selectedColor = {
        new Color(191f/255, 7f/255, 7f/255, 1f),
        new Color(7f/255, 7f/255, 191f/255, 1f),
        new Color(34f/255, 102f/255, 34f/255, 1f),
        new Color(191f/255, 176f/255, 29f/255, 1f),
        new Color(106f/255, 102f/255, 127f/255, 1f),
        new Color(0, 141f/255, 159f/255, 1f),
    };

    static public Color[] color = {
        new Color(245f/255, 10f/255, 10f/255, 1f),
        new Color(10f/255, 10f/255, 245f/255, 1f),
        new Color(45f/255, 136f/255, 45f/255, 1f),
        new Color(255f/255, 235f/255, 39f/255, 1f),
        new Color(142f/255, 136f/255, 170f/255, 1f),
        new Color(0, 188f/255, 212f/255, 1f),
    };

    static public Color[] lightColor = {
        //new Color(245f/255, 30f/255, 30f/255, 1f),
        //new Color(30f/255, 30f/255, 245f/255, 1f),
        //new Color(65f/255, 156f/255, 65f/255, 1f),
        //new Color(255f/255, 255f/255, 59f/255, 1f),
        //new Color(162f/255, 156f/255, 190f/255, 1f),
        //new Color(20f/255, 200f/255, 242f/255, 1f),
        new Color(150f/255, 150f/255, 150f/255, 1f),
        new Color(150f/255, 150f/255, 150f/255, 1f),
        new Color(150f/255, 150f/255, 150f/255, 1f),
        new Color(150f/255, 150f/255, 150f/255, 1f),
        new Color(150f/255, 150f/255, 150f/255, 1f),
        new Color(150f/255, 150f/255, 150f/255, 1f),
    };

    public int player;
    public Vector3 position;
    public HashSet<Vector3> avaliableMoves = new HashSet<Vector3> ();
    public Hashtable pathes = new Hashtable();
    public Vector3 target;

    FullBoard board;
    SpriteRenderer spriteRenderer;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void UpdatePosition() {
        position = transform.position;
    }

    public void UpdatePosition(Vector3 pos) {
        position = pos;
    }

    public void Init(FullBoard bd, int pl) {
        board = bd;
        player = pl;
        ChangeColor(color[player]);
        UpdatePosition();
    }

    public Vector3 Pos() {
        return position;
    }

    public float Distance2() {
        Vector3 pos = Pos();
        return (pos.x - target.x) * (pos.x - target.x) + (pos.y - target.y) * (pos.y - target.y);
    }

    public void ChangeColor(Color color) {
        spriteRenderer.color = color;
    }

    public void UpdateAvaliableMove() {
        GetBlock().AvaliableMove(avaliableMoves, pathes);
    }

    public void Select() {
        if (board.selectedPiece != null) {
            board.selectedPiece.UnSelect();
        }
        board.selectedPiece = this;

        ChangeColor(selectedColor[player]);

        //board.selectedPiece.GetBlock().AvaliableMove(avaliableMoves);
        UpdateAvaliableMove();
        //board.SetColor(avaliableMoves, new Color(239f / 255, 154f / 255, 154f / 255, 1f));
        board.SetColor(avaliableMoves, Piece.lightColor[player]);
    }

    public void UnSelect() {
        board.selectedPiece = null;

        ChangeColor(color[player]);

        board.SetColor(avaliableMoves, new Color(1f, 1f, 1f, 1f));
    }

    public Block GetBlock() {
        return board.GetBlock(position);
    }

    public Vector3 MoveTo(Vector3 pos, bool move=true) {
        Vector3 org = new Vector3(position.x, position.y, position.z);

        board.GetBlock(position).isOccupied = false;
        board.UpdatePlayerValue(player, -Distance2());
        if (move) {
            transform.position = pos;
        }
        UpdatePosition(pos);
        board.GetBlock(pos).isOccupied = true;
        board.UpdatePlayerValue(player, Distance2());

        return org;
    }

    public float ValueDiff(Vector3 pos) {
        float result;
        Vector3 org = new Vector3(position.x, position.y, position.z);
        UpdatePosition(pos);
        result = Distance2();
        UpdatePosition(org);
        result -= Distance2();
        return result;
    }

    List<Vector3> GetPath(Vector3 dst, List<Vector3> path) {
        if (!pathes.Contains(dst)) {
            return null;
        }

        if (((Vector3)pathes[dst]).x == position.x 
            && ((Vector3)pathes[dst]).y == position.y) {
            return path;
        }

        path.Insert(0, (Vector3)pathes[dst]);
        return GetPath((Vector3)pathes[dst], path);
    }

    IEnumerator waiter(List<Vector3> path) {
        foreach (Vector3 p in path) {
            MoveTo(p);
            yield return new WaitForSeconds(0.3f);
        }
        GameManager.pause = false;
    }

    public int JumpTo(Vector3 pos) {
        if (avaliableMoves.Contains(pos) == false) {
            return 0;
        }

        List<Vector3> path = new List<Vector3>();
        path.Add(pos);
        path = GetPath(pos, path);

        GameManager.pause = true;
        StartCoroutine(waiter(path));

        return 1;
    }

    public override int OnClick() {
        Debug.LogFormat("Piece on click");
        if (board.selectedPiece != null) {
            board.selectedPiece.UnSelect();
        }

        Select();

        // debug
        Debug.LogFormat("value {0}", board.Evaluate(player));

        return 0;
    }
}
