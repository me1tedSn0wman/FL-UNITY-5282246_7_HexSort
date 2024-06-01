using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.PoolControl;

public enum HexCellState { 
    Free,
    Selected,
    Locked,
}

public class HexCell : MonoBehaviour
{
    [SerializeField] private HexCellState _hexCellState;

    public HexCellState hexCellState
    {
        get { return _hexCellState; }
        set {
            _hexCellState = value;
            switch (value) {
                case HexCellState.Free:
                    SetColor();
                    break;

                case HexCellState.Selected:
                    SetColor(colorSelected);
                    break;

                case HexCellState.Locked:
                    SetColor();
                    break;
            }
        
        }
    }

    [SerializeField] private Color colorSelected;

    private const float SQRT3 = 1.732f;

    [SerializeField] private Vector2Int _gridPos;
    [SerializeField] private float cellSize;



    public float pieceHeight = 0.2f;

    public Vector2Int gridPos {
        get { return _gridPos; }
        set { 
            _gridPos = value;
            float yHexOffset = gridPos.x % 2 == 0 ? 1f : 0f;
            transform.localPosition = new Vector3(
                cellSize + value.x * 1.5f * cellSize,
                0,
                SQRT3 *0.5f *(1+yHexOffset) * cellSize + value.y * SQRT3 * cellSize
                );
            }
    }

    public Transform cellContainer;

    public List<HexPiece> listOfHexPieces;
    public List<HexPieceType> listPrevHexPieces;

    public Vector3 pos {
        get { return transform.position; }
        set { transform.position = value; }
    }

    [SerializeField] private GameObject hexPiecePrefabGO;

    private Material[] material;
    private Color[] origColors;

    public void Awake()
    {
        listOfHexPieces = new List<HexPiece>();

        GetAllMaterials();
        GetOriginalColors();

        hexCellState = HexCellState.Free;
    }

    public void SetCell(Vector2Int gridPos, float cellSize) {
        this.cellSize = cellSize;
        this.gridPos = gridPos;

        gameObject.name = "Cell_" + gridPos.x.ToString("D2") + "_x_" + gridPos.y.ToString("D2");
    }

    public void AddPiece(HexPiece newPiece) {
        listOfHexPieces.Add(newPiece);
        hexCellState = HexCellState.Locked;
    }

    public int CountOfPieces() {
        return listOfHexPieces.Count;
    }

    public HexPieceType GetUpHexPieceType() {
        if (listOfHexPieces.Count == 0) return HexPieceType.None; 
        HexPiece upPiece = listOfHexPieces[listOfHexPieces.Count-1];
        return upPiece.hexPieceType;
    }

    public HexPiece PopLastPiece()
    {
        HexPiece crntHexPiece = listOfHexPieces[CountOfPieces() - 1];
        listOfHexPieces.RemoveAt(CountOfPieces() - 1);
        if (listOfHexPieces.Count == 0) hexCellState = HexCellState.Free;
        return crntHexPiece;
    }

    public bool CheckAllNeighborColor() {
        bool somethingMoved = false;

        int leftUpYIndOffset = gridPos.x % 2 == 0 ? 1 : 0;

        List<Vector2Int> listOfNeighb = new List<Vector2Int>();

        listOfNeighb.Add(new Vector2Int(gridPos.x - 1, gridPos.y + leftUpYIndOffset)); //leftUp
        listOfNeighb.Add(new Vector2Int(gridPos.x - 1, gridPos.y -1 + leftUpYIndOffset)); //leftDown

        listOfNeighb.Add(new Vector2Int(gridPos.x, gridPos.y + 1)); // midUp
        listOfNeighb.Add(new Vector2Int(gridPos.x, gridPos.y - 1)); // midDown

        listOfNeighb.Add(new Vector2Int(gridPos.x + 1, gridPos.y + leftUpYIndOffset)); //rightUp
        listOfNeighb.Add(new Vector2Int(gridPos.x + 1, gridPos.y - 1 + leftUpYIndOffset)); //rightDown

        for (int i = 0; i < listOfNeighb.Count; i++) {
            somethingMoved = CheckNeighborColor(listOfNeighb[i]);
            if (somethingMoved) break;
        }
        return somethingMoved;

    }

    public bool CheckNeighborColor(Vector2Int neighbGridPos)
    {
        HexCell neighbCell = HexCellGrid.TRY_GET_CELL(neighbGridPos);
        if (neighbCell == null) return false;
        if (neighbCell.CountOfPieces() == 0) return false;
        if (GetUpHexPieceType() != neighbCell.GetUpHexPieceType()) return false;


        HexPieceType hexPieceType = GetUpHexPieceType();
        int count = 0;
        while (CountOfPieces() > 0 && hexPieceType == GetUpHexPieceType()) { 
            HexPiece crntHexPiece = PopLastPiece();
            crntHexPiece.MoveToCell(neighbCell, true, count);
            count++;
        }
        return true;
    }

    public void MoveAllToGridPos(Vector2Int gridPos) {
        int count = 0;
        HexCell cell = HexCellGrid.TRY_GET_CELL(gridPos);
        while (CountOfPieces() > 0)
        {
            HexPiece crntHexPiece = PopLastPiece();
            crntHexPiece.MoveToCell(cell, true, count);
            count++;
        }
    }



    public bool CheckPiecesForEliminate() {
        if (CountOfPieces() == 0) return false;
        bool somethingWasEliminated = false;

        HexPieceType crntHexPieceType = HexPieceType.None;

        Stack<HexPiece> stackForRemove = new Stack<HexPiece>();

        for (int i = CountOfPieces()-1; i >= 0; i--) {
            HexPiece crntHexPiece = listOfHexPieces[i];
            if (crntHexPieceType == listOfHexPieces[i].hexPieceType)
            {
                stackForRemove.Push(crntHexPiece);
            }
            else if (stackForRemove.Count > 10)
            {
                break;
            }
            else {
                crntHexPieceType = crntHexPiece.hexPieceType;
                
                stackForRemove = new Stack<HexPiece>();
                stackForRemove.Push(crntHexPiece);
            }
        }




        if (stackForRemove.Count > 10)
        {


            int count = stackForRemove.Count;
            while (stackForRemove.Count > 0)
            {
                HexPiece crntHexPiece = stackForRemove.Pop();
                listOfHexPieces.Remove(crntHexPiece);
                crntHexPiece.StartRemove();
            }
            somethingWasEliminated = true;
            if (listOfHexPieces.Count == 0) hexCellState = HexCellState.Free;
            IncreaseScore(count);
        }
        return somethingWasEliminated;
    }

    public void IncreaseScore(int count) {
        
        /*
         FIND A BETTER PLACE FOR THAT
         */
        int scoreMult = 1;
        switch (GameManager.Instance.gameDifficulty)
        {
            case GameDifficulty.Easy:
                scoreMult = 1;
                break;
            case GameDifficulty.Normal:
                scoreMult = 2;
                break;
            case GameDifficulty.Hard:
                scoreMult = 3;
                break;
        }

        int score = (count + (count - 10) * (count / 8))* scoreMult;
        GameplayManager.Instance.scoreManager.AddScore(score);
    }

    public void SpawnListOfPieces(List<HexPieceType> hexPiecesList)
    {
        for (int i = 0; i < hexPiecesList.Count; i++) {
            SpawnPiece(hexPiecesList[i]);
        }
    }

    public void SpawnPiece(HexPieceType hexPiece)
    {
        HexPiece newHexPiece = Poolable.TryGetPoolable<HexPiece>(hexPiecePrefabGO);
        listOfHexPieces.Add(newHexPiece);

        newHexPiece.SetParent(transform);
        newHexPiece.hexPieceType = hexPiece;
        newHexPiece.locPosImmediate = new Vector3(0, pieceHeight *CountOfPieces(), 0);
        newHexPiece.locRot = Quaternion.Euler(Vector3.zero);

        
    }

    public void GetAllMaterials()
    {
        Renderer[] rends = GetComponentsInChildren<Renderer>();
        List<Material> mats = new List<Material>();
        foreach (Renderer rend in rends) {
            mats.Add(rend.material);
        }
        material = mats.ToArray();
    }

    public void GetOriginalColors() {
        origColors = new Color[material.Length];
        for (int i = 0; i < material.Length; i++) {
            origColors[i]= material[i].color;
        }
    }

    public void SetColor() {
        for (int i = 0; i < material.Length; i++) {
            material[i].color = origColors[i];
        }
    }
    public void SetColor(Color color)
    {
        for (int i = 0; i < material.Length; i++)
        {
            material[i].color = color;
        }
    }

    public void SaveListOfPieces() {
        listPrevHexPieces.Clear();
        for (int i = 0; i < listOfHexPieces.Count; i++) {
            listPrevHexPieces.Add(listOfHexPieces[i].hexPieceType);
        }
    }

    public void LoadListOfPieces() {
        SpawnListOfPieces(listPrevHexPieces);
        if(CountOfPieces() != 0 ) hexCellState = HexCellState.Locked;
    }

    public void RemoveAllPiecesImmediately() {
        while (CountOfPieces() > 0) {
            HexPiece crntHexPiece = PopLastPiece();
            crntHexPiece.RemoveImmediately();
        }
        listOfHexPieces.Clear();
        hexCellState = HexCellState.Free;
    }

}