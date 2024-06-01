using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexPieceSpawner : MonoBehaviour
{
    [Header("Set in Inspector")]
    [SerializeField] private HexPieceTower hexPieceTowerPrefab;

    [SerializeField] private List<Transform> upperAnchors;
    [SerializeField] private List<Transform> downAnchors;

    [Header("Set Dynamically")]
    [SerializeField] private List<Transform> allAnchors;

    [SerializeField] private List<HexPieceTower> downTowers;
    [SerializeField] private List<HexPieceTower> upperTowers;

    [SerializeField] private List<HexPieceType> prevTower;
    [SerializeField] private Transform prevSpawnAnchor; 

    public void Awake() {
        allAnchors = new List<Transform>();
        allAnchors.AddRange(upperAnchors);
        allAnchors.AddRange(downAnchors);

    }

    public void SpawnAllTowers() {
        SpawnUpperLineAnchors();
        SpawnDownLineAnchors();
    }

    public void CheckUpperLine() {
        if (upperTowers.Count != 0) return;
        MoveFromDownLineToUpperLine();
        SpawnDownLineAnchors();
    }

    public void SpawnUpperLineAnchors() {
        for (int i = 0; i < upperAnchors.Count; i++) {
            HexPieceTower newHexPieceTower = SpawnNewPieceTower(upperAnchors[i]);
            upperTowers.Add(newHexPieceTower);
        }
    }

    public void SpawnDownLineAnchors() {
        for (int i = 0; i < downAnchors.Count; i++)
        {
            HexPieceTower newHexPieceTower = SpawnNewPieceTower(downAnchors[i]);
            downTowers.Add(newHexPieceTower);
        }
    }

    public void MoveFromDownLineToUpperLine() {
        upperTowers.Clear();
        for (int i = 0; i < upperAnchors.Count && i < downTowers.Count; i++) {
            downTowers[i].SetParent(upperAnchors[i]);
            downTowers[i].locPos = Vector3.zero;
            upperTowers.Add(downTowers[i]);
        }
        downTowers.Clear();
    }


    public void MoveFromUpLineToDownLine()
    {
        for (int i = 0; i < downTowers.Count; i++) {
            downTowers[i].DestroyTower();
        }
        downTowers.Clear();

        for (int i = 0; i < upperTowers.Count && i < downAnchors.Count; i++)
        {
            upperTowers[i].SetParent(downAnchors[i]);
            upperTowers[i].locPos = Vector3.zero;
            downTowers.Add(upperTowers[i]);
        }
        upperTowers.Clear();
    }


    public HexPieceTower SpawnNewPieceTower(Transform pieceAnchor) {
        HexPieceTower newHexPieceTower = Instantiate(hexPieceTowerPrefab);

        newHexPieceTower.GenerateTower();
        newHexPieceTower.SetParent(pieceAnchor);
        newHexPieceTower.locPos = Vector3.zero;

        return newHexPieceTower;
    }

    public void RemoveTowerFromUpperLine(HexPieceTower hexPieceTower) {
        prevTower = new List<HexPieceType>(hexPieceTower.GetListHexPieceType());
        prevSpawnAnchor = hexPieceTower.spawnAnchor;
        upperTowers.Remove(hexPieceTower);
    }

    public void LoadPrevTower() {
        HexPieceTower newHexPieceTower = Instantiate(hexPieceTowerPrefab);

        if (upperAnchors.Count == upperTowers.Count) {
            MoveFromUpLineToDownLine();
        }

        newHexPieceTower.GenerateTower(prevTower);
        newHexPieceTower.SetParent(prevSpawnAnchor);
        newHexPieceTower.locPos = Vector3.zero;
        upperTowers.Add(newHexPieceTower);
    }
}
