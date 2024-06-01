using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Utils.PoolControl;

public enum GameplayState { 
    PreGame,
    SpawnCellGrid,
    SpawnTower,
    WaitingInput,
    WaitingAnimation,
    MoveTower,
    MovePiecesBetweenCells,
    FinishedMovePiecesBetweenCells,
    EliminateFullfilledPieces,
    FinishedEliminateFullfilledPieces,
    GameOver,
}

public class GameplayManager : Singleton<GameplayManager>
{
    [Header("Set in Inspector")]
    
    [SerializeField] public ScoreManager scoreManager;
    [SerializeField] public GameplayUIManager gameplayUIManager;
    [SerializeField] private PlayerControlManager playerControlManager;
    [SerializeField] public HexPieceSpawner hexPieceTowerSpawner;
    [SerializeField] private PoolManager poolManager;
    [SerializeField] private HexCellGrid hexCellGrid;
    [SerializeField] private CameraControl cameraControl;

    [Header("Set dynamically")]

    [SerializeField] private GameplayState _gameplayState;
    [SerializeField] private GameplayState _nextGameplayState;

    public GameplayState gameplayState {
        get { return _gameplayState; }
        set { _gameplayState = value; }
    }

    [SerializeField] private HexPieceTower selectedHexPieceTower;
    [SerializeField] private HexCell selectedHexCell;

    [SerializeField] private bool repeatCheckProcess;

    [SerializeField] private List<HexPiece> movingPieces;



    public override void Awake()
    {
        base.Awake();

        gameplayState = GameplayState.PreGame;



        gameplayState = GameplayState.SpawnCellGrid;

        hexCellGrid.Init(5, 5);
        hexCellGrid.CreateGrid();
        cameraControl.MoveFocalPointToPos(hexCellGrid.transform.position + hexCellGrid.CenterOfGridPos());

        gameplayState = GameplayState.SpawnTower;

        hexPieceTowerSpawner.SpawnAllTowers();

        gameplayState = GameplayState.WaitingInput;
    }

    public void Update()
    {
        switch (gameplayState) {
            case GameplayState.WaitingInput:
                break;
            case GameplayState.WaitingAnimation:
                WaitingAnimation();
                break;
            case GameplayState.MoveTower:
                UpdateTowerPosition();
                break;
            }
       
    }

    public void SetSelectedTower(HexPieceTower newTower) {
        if (gameplayState != GameplayState.WaitingInput) return;
        gameplayState = GameplayState.MoveTower;
        selectedHexPieceTower = newTower;
    }

    public void SetSelectedCell(HexCell newCell) {
        if (selectedHexCell != null) {
            if (selectedHexCell.hexCellState == HexCellState.Selected) {
                selectedHexCell.hexCellState = HexCellState.Free;
            }
            selectedHexCell = null;
        }
        if (newCell.hexCellState == HexCellState.Locked) return;
        selectedHexCell = newCell;
        selectedHexCell.hexCellState = HexCellState.Selected;
    }

    public void ReleaseTower() {
        if (selectedHexPieceTower == null) return;
        if (gameplayState != GameplayState.MoveTower || selectedHexCell == null || selectedHexCell.hexCellState == HexCellState.Locked)
        {
            Debug.Log("BackToHand");
            TryMoveBackToHand(selectedHexPieceTower);
            return;
        }
        Debug.Log(selectedHexCell.hexCellState);
        TryMoveTowerToCell(selectedHexPieceTower, selectedHexCell);
    }

    public void MovePiecesBetweenCell() 
    {
        gameplayState = GameplayState.MovePiecesBetweenCells;

        bool somethingWasFound = false;
        for (int j = 0; j < HexCellGrid.H; j++) {
            for (int i = 0; i < HexCellGrid.W; i++) {
                HexCell crntCell = HexCellGrid.TRY_GET_CELL(new Vector2Int(i, j));
                if (crntCell == null) continue;
                somethingWasFound = crntCell.CheckAllNeighborColor();
                if (somethingWasFound) break;
            }
            if (somethingWasFound) break;
        }

        if (somethingWasFound) 
            StartWaitingAnimiation(GameplayState.MovePiecesBetweenCells);
        else 
            EliminateFullfilledTowers();
    }

    public void EliminateFullfilledTowers() 
    {
        gameplayState = GameplayState.EliminateFullfilledPieces;

        bool somethingWasEliminate = false;
        for (int j = 0; j < HexCellGrid.H; j++) {
            for (int i = 0; i < HexCellGrid.W; i++) {
                HexCell crntCell = HexCellGrid.TRY_GET_CELL(new Vector2Int(i, j));
                if (crntCell == null) continue;
                somethingWasEliminate = somethingWasEliminate || crntCell.CheckPiecesForEliminate();
            }
        }

        if (somethingWasEliminate)
            StartWaitingAnimiation(GameplayState.MovePiecesBetweenCells);
        else
            CheckGameOver();
    }

    public void CheckGameOver() {
        bool endGame = true;
        for (int j = 0; j < HexCellGrid.H; j++) {
            for (int i = 0; i < HexCellGrid.W; i++) {
                HexCell crntCell = HexCellGrid.TRY_GET_CELL(new Vector2Int(i, j));
                if (crntCell!=null && crntCell.hexCellState != HexCellState.Locked)
                {
                    endGame = false;
                    break;
                }

            }
            if (!endGame) break;
        }

        if (endGame)
        {
            StartGameOver();
        }
        else {
            StartNewRound();
        }
    }

    public void SaveGameplayState() {
        scoreManager.SaveStateScore();
        SaveCellStateList();
        gameplayUIManager.UnlockCancelTurn();
    }

    public void SaveCellStateList() {
        for (int j = 0; j < HexCellGrid.H; j++) {
            for (int i = 0; i < HexCellGrid.W; i++) { 
                HexCell crntCell = HexCellGrid.TRY_GET_CELL(new Vector2Int(i, j));
                if (crntCell != null) {
                    crntCell.SaveListOfPieces();
                }
            }
        }
    }

    public void LoadGameplayState() {
        scoreManager.LoadStateScore();
        LoadCellStateList();
        hexPieceTowerSpawner.LoadPrevTower();
        movingPieces.Clear();
    }

    public void LoadCellStateList() {
        for (int j = 0; j < HexCellGrid.H; j++)
        {
            for (int i = 0; i < HexCellGrid.W; i++)
            {
                HexCell crntCell = HexCellGrid.TRY_GET_CELL(new Vector2Int(i, j));
                if (crntCell != null)
                {
                    crntCell.RemoveAllPiecesImmediately();
                    crntCell.LoadListOfPieces();
                }
            }
        }
    }


    public void StartNewRound() 
    {
        gameplayState = GameplayState.SpawnTower;
        hexPieceTowerSpawner.CheckUpperLine();

        gameplayState = GameplayState.WaitingInput;
    }

    public void StartGameOver() {
        gameplayState = GameplayState.GameOver;

        gameplayUIManager.GameOver(
            scoreManager.score
            ,scoreManager.isNewHighScore
            );
    }

    public void UpdateTowerPosition() {
        Vector3 newPos = playerControlManager.GetPointerWordPositionOnSurface();
        selectedHexPieceTower.immediatePos = newPos;
    }

    public void TryMoveTowerToCell(HexPieceTower hexPieceTower, HexCell hexCell) {
        SaveGameplayState();

        hexPieceTower.TryMoveToCell(hexCell);
        selectedHexPieceTower = null;
        StartWaitingAnimiation(GameplayState.MovePiecesBetweenCells);
    }

    public void TryMoveBackToHand(HexPieceTower hexPieceTower)
    {
        hexPieceTower.TryMoveBackToHand();

        selectedHexPieceTower = null;
        StartWaitingAnimiation(GameplayState.MovePiecesBetweenCells);
    }

    public static void RESTART() {
        GameManager.RELOAD_LEVEL();
    }

    public static void CANCEL_TURN() {
        Instance.LoadGameplayState();
    }

    public static void RESTORE_CAMERA_POSITION() {
        Instance.cameraControl.ResetCameraRotation();
    }

    public void AddToWaitingQueue(HexPiece hexPiece) {
        movingPieces.Add(hexPiece);
    }

    public void RemoveFromWaitingQueue(HexPiece hexPiece) { 
        movingPieces.Remove(hexPiece);
    }

    public void WaitingAnimation() {
        if (movingPieces.Count != 0) return;
        EndWaitingAnimation();
    }

    public void StartWaitingAnimiation(GameplayState nextGameplayState) {
        gameplayState = GameplayState.WaitingAnimation;
        _nextGameplayState = nextGameplayState;
    }

    public void EndWaitingAnimation() {
        gameplayState = _nextGameplayState;
        switch (_nextGameplayState) {
            case GameplayState.MovePiecesBetweenCells:
                MovePiecesBetweenCell();
                break;
        }
    }



}
