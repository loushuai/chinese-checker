using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    string[] buttonNames = {
        "Player1Button",
        "Player2Button",
        "Player3Button",
        "Player4Button",
        "Player5Button",
        "Player6Button",};
    string[] buttonText = {
        "关  闭",
        "玩  家",
        "电  脑",
    };
    int[] buttonState = { 0, 0, 0, 0, 0, 0 };
    FullBoard board;

    void Awake() {
        GameObject gm = GameObject.Find("GameManager");
        if (!gm) {
            Debug.LogFormat("Can't find game manager");
            return;
        }

        board = gm.GetComponent<GameManager>().fullBoard.GetComponent<FullBoard>();
    }

    Button GetButtonByName(string btnName) {
        Component[] comps = GetComponentsInChildren<Button>();
        foreach (Component btn in comps) {
            if (btnName == ((Button)btn).name) {
                return (Button)btn;
            }
        }

        return null;
    }

    public void EnterLobbyClient() {
        SceneManager.LoadScene(2);
    }

    void PlayerButtonClick(int id) {
        Button btn = GetButtonByName(buttonNames[id]);
        if (!btn) {
            Debug.Log("null button");
            return;
        }
        buttonState[id] = (buttonState[id] + 1) % 3;
        int state = buttonState[id];
        btn.GetComponentInChildren<Text>().text = buttonText[buttonState[id]];

        board.LooksLikeInitPlayer(id, buttonState[id] == 0);
    }

    public void ClickPlayer1Btn() {
        PlayerButtonClick(0);
    }
    public void ClickPlayer2Btn() {
        PlayerButtonClick(1);
    }
    public void ClickPlayer3Btn() {
        PlayerButtonClick(2);
    }
    public void ClickPlayer4Btn() {
        PlayerButtonClick(3);
    }
    public void ClickPlayer5Btn() {
        PlayerButtonClick(4);
    }
    public void ClickPlayer6Btn() {
        PlayerButtonClick(5);
    }

    public void StartButtonClick() {
        for (int i = 0; i < buttonState.Length; ++i) {
            if (buttonState[i] > 0) {
                bool isAI = (buttonState[i] == 2);
                GameManager.AddPlayer(i, isAI);
            }
        }
        GameManager.gameOver = false;
        SceneManager.LoadScene(1);
    }
}
