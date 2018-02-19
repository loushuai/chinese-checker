using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerClient : NetworkBehaviour {

	// Use this for initialization
	void Start () {
        Debug.Log("PlayerClient Start");
        if (isServer) {
            Debug.Log("Running as client");
        } else if (isClient) {
            Debug.Log("Running as server");
        } else if (isLocalPlayer) {
            Debug.Log("Running as local player");
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Player1ButtonClick() {
        Debug.Log("Player Client button1 clicked");
        GameObject btnObj = GameObject.Find("/Canvas/GameMenu/Player3Button");
        Button btn = btnObj.GetComponentInChildren<Button>();
        btn.GetComponentInChildren<Text>().text = "haha";
    }
}
