using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.PoolControl;

public enum HexPieceStatus { 
    InTower,
    Selected,
    InCell,
    Moving,
}

[RequireComponent(typeof(BoxCollider))]
public class HexPieceTower : MonoBehaviour
{
    [SerializeField] private int minHeight = 3;
    [SerializeField] private int maxHeight = 8;
    [SerializeField] private float pieceHeight = 0.2f;

    [SerializeField] private int towerHeight;
    
    [SerializeField] private GameObject hexPiecePrefabGO;
    public List<HexPiece> listOfHexPieces;

    [SerializeField] private BoxCollider boxCollider;

    [SerializeField] public Transform spawnAnchor;

    public Vector3 locPos
    {
        get { return transform.localPosition; }
        set { transform.localPosition = value; }
    }

    public Vector3 immediatePos {
        get { return transform.position; }
        set { transform.position = value; }
    }

    public void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    public void SetParent(Transform newParent) {
        transform.SetParent(newParent);
        spawnAnchor = newParent;
    }

    public void GenerateTower() {

        towerHeight = Random.Range(minHeight, maxHeight+1);

        for (int i = 0; i < towerHeight; i++) {
//            Debug.Log(i);
            HexPiece newHexPiece = Poolable.TryGetPoolable<HexPiece>(hexPiecePrefabGO);

            int endIndex=8;

            switch (GameManager.Instance.gameDifficulty) {
                case GameDifficulty.Easy:
                    endIndex = 4;
                    break;
                case GameDifficulty.Normal:
                    endIndex = 6;
                    break;
                case GameDifficulty.Hard:
                    endIndex = 8;
                    break;
            }

            int indHexType = Random.Range(0, endIndex);
            HexPieceType newHexPieceType = HexPiece.GetHexPieceType(indHexType);

            newHexPiece.SetParentTower(this);
            newHexPiece.locPosImmediate = new Vector3(0, pieceHeight * i, 0);
            newHexPiece.locRot = Quaternion.Euler(Vector3.zero);
            newHexPiece.hexPieceType = newHexPieceType;

            listOfHexPieces.Add(newHexPiece);
        }
        ResizeCollider();
    }

    public void GenerateTower(List<HexPieceType> listOfHexPieceType) {
        for (int i = 0; i < listOfHexPieceType.Count; i++)
        {
            HexPiece newHexPiece = Poolable.TryGetPoolable<HexPiece>(hexPiecePrefabGO);

            newHexPiece.SetParentTower(this);
            newHexPiece.locPosImmediate = new Vector3(0, pieceHeight * i, 0);
            newHexPiece.locRot = Quaternion.Euler(Vector3.zero);
            newHexPiece.hexPieceType = listOfHexPieceType[i];

            listOfHexPieces.Add(newHexPiece);
        }
        ResizeCollider();
    }

    public void ResizeCollider() {
        boxCollider.center = new Vector3(
            0,
            0.5f * towerHeight * pieceHeight,
            0);
        boxCollider.size = new Vector3(
            1.5f,
            towerHeight * pieceHeight,
            1.5f
            );
    }

    public void TryMoveToCell(HexCell hexCell) {
        GameplayManager.Instance.hexPieceTowerSpawner.RemoveTowerFromUpperLine(this);
        for (int i = 0; i < listOfHexPieces.Count; i++) {
            listOfHexPieces[i].MoveToCell(hexCell,false,0);
        }
        listOfHexPieces.Clear();
        Destroy(gameObject);
    }

    public void TryMoveBackToHand() {
        transform.SetParent(spawnAnchor);
        locPos = Vector3.zero;
    }

    public List<HexPieceType> GetListHexPieceType() {
        List<HexPieceType> crntList = new List<HexPieceType>();
        for (int i = 0; i < listOfHexPieces.Count; i++) {
            crntList.Add(listOfHexPieces[i].hexPieceType);
        }
        return crntList;
    }

    public void DestroyTower() {
        for (int i = 0; i < listOfHexPieces.Count; i++)
        {
            listOfHexPieces[i].RemoveImmediately();
        }
        listOfHexPieces.Clear();
        Destroy(gameObject);
    }
}
