using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public GameObject fullBoardPrefab;
    public GameObject fullBoard;
    FullBoard board;

    GameObject popMenu = null;
    int whoseTurn = 0;

    static public List<Player> players = new List<Player>();
    static public bool pause = true;
    static public bool begin = false;
    static public bool gameOver = true;

    static public void AddPlayer(int id, bool isAI) {
        players.Add(new Player(id, isAI));
    }

    void Awake() {
        pause = gameOver;
        fullBoard = Instantiate(fullBoardPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
        board = fullBoard.GetComponent<FullBoard>();

        board.Init(players);

        foreach (Player plr in players) {
            if (plr.isAI) {
                plr.brain = new GreedyAI(board, plr.id, (int)(Random.value * 2) + 1);
            }
        }
    }

    void DetectClick() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                Debug.Log("Hit by mouse\t");
                ClickableMonoBehaviour clickable = hit.transform.GetComponent<ClickableMonoBehaviour>();

                clickable.OnClick();
            }
        }
    }

    void SetPopupMenuEnable(bool enable) {
        if (popMenu) {
            popMenu.SetActive(enable);
            return;
        }

        GameObject menu = GameObject.Find("/PopupMenu");
        if (menu == null) {
            Debug.Log("Can't find menu object");
            return;
        }
        this.popMenu = menu;
        menu.SetActive(enable);
    }

    void GameOver() {
        SetPopupMenuEnable(true);
        gameOver = true;
        players.Clear();
    }

    void Start() {
        SetPopupMenuEnable(false);
    }

    // Update is called once per frame
    void Update() {
        if (pause) {
            return;
        }

        if (gameOver) {
            return;
        }

        if (players.Count > 0) {
            whoseTurn = players[whoseTurn].Go(whoseTurn);
            whoseTurn = whoseTurn % players.Count;
        }

        foreach (Player player in players) {
            if (player.IsWin()) {
                GameOver();
                break;
            }
        }
    }

    public void ReturnToMainMenue() {
        SceneManager.LoadScene(0);
    }
}
