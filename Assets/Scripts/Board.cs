using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour{
    public GameObject PiecePrefab;
    public GameObject blockPrefab;
    public Piece selectedPiece = null;

    virtual public void Init(List<int> players) 
    {
        
    }
	
    virtual public Block GetBlock(int row, int col, int subid) 
    {
        return null;
    }
}
