using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Utils.PoolControl;


public enum LogoState
{
    SpawnCellGrid,
    WaitingAnimation,
    MovePiecesBetweenCells,
    FinishedMovePiecesBetweenCells,
}

public class MainMenuLogo : Singleton<MainMenuLogo>
{
    [SerializeField] private LogoState _logoState;
    [SerializeField] private LogoState _nextLogoState;

    public LogoState logoState
    {
        get { return _logoState; }
        set { _logoState = value; }
    }

    [SerializeField] public PoolManager poolManager;
    [SerializeField] public CameraControl cameraControl;
    [SerializeField] public float cameraRotationSpeed;

    [SerializeField] private HexCellGrid hexCellGrid;
    [SerializeField] private List<HexPiece> movingPieces;

    [SerializeField] private List<HexPieceType> listTower1;
    [SerializeField] private List<HexPieceType> listTower2;
    [SerializeField] private List<HexPieceType> listTower3;

    [SerializeField] private List<Vector2Int> rotationQueue;
    [SerializeField] public int crntStep = 0;

    public void Awake()
    {
        logoState = LogoState.SpawnCellGrid;
        hexCellGrid.Init(3, 3);
        hexCellGrid.CreateGrid();
        HexCellGrid.HEX_CELL_MAP[0, 0].SpawnListOfPieces(listTower1);
        HexCellGrid.HEX_CELL_MAP[0, 2].SpawnListOfPieces(listTower2);
        HexCellGrid.HEX_CELL_MAP[2, 0].SpawnListOfPieces(listTower3);
    }

    public void Start()
    {
        MovePiecesBetweenCell();
        
    }

    public void Update()
    {

        switch (logoState) {
            case LogoState.WaitingAnimation:
                WaitingAnimation();
                break;
        }
        cameraControl.RotateCamera(cameraRotationSpeed);
    }

    public void AddToWaitingQueue(HexPiece hexPiece)
    {
        movingPieces.Add(hexPiece);
    }

    public void RemoveFromWaitingQueue(HexPiece hexPiece)
    {
        movingPieces.Remove(hexPiece);
    }

    public void WaitingAnimation()
    {
        if (movingPieces.Count != 0) return;
        EndWaitingAnimation();
    }

    public void StartWaitingAnimiation(LogoState nextLogoState)
    {
        logoState = LogoState.WaitingAnimation;
        _nextLogoState = nextLogoState;
    }

    public void EndWaitingAnimation()
    {
        logoState = _nextLogoState;
        switch (_nextLogoState)
        {
            case LogoState.MovePiecesBetweenCells:
                MovePiecesBetweenCell();
                break;
        }
    }

    public void MovePiecesBetweenCell()
    {
        logoState = LogoState.MovePiecesBetweenCells;
        Vector2Int crntPos = rotationQueue[crntStep];
        Vector2Int moveToPos = rotationQueue[crntStep+1];

        HexCellGrid.HEX_CELL_MAP[crntPos.x, crntPos.y].MoveAllToGridPos(moveToPos);

        crntStep = (crntStep+2)% rotationQueue.Count;
        StartWaitingAnimiation(LogoState.MovePiecesBetweenCells);
    }
}
