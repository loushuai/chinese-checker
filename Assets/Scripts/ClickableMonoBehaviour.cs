using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class ClickableMonoBehaviour : MonoBehaviour {


    public int id;
    public int type; // 0 block, 1 piece
    abstract public int OnClick();
    public void SetId(int id) {
        this.id = id;
    }
}
