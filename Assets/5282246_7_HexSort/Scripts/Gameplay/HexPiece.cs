using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Utils;
using Utils.PoolControl;

public enum HexPieceState { 
    AtCell,
    MovingToPos,
    MovingToLocPos,
    Despawn,
}

public class HexPiece : MonoBehaviour {
    [Header("Set in Inspector")]

    public float timeMoving = -1f;
    public float timeMoveGapDuration = 0.1f;
    public float timeMoveDurationInit = 1f;
    public float timeMoveDurationFinal;
    public bool isFlipping = false;

    public float timeRemoving = -1f;
    public float timeRemovingDuration = 0.5f;
    public float rotationSpeedRemoving = 10f;

    public float animationSpeed = 1f;


    [SerializeField] private HexPieceState _hexPieceState;

    public HexPieceState hexPieceState {
        get { return _hexPieceState; }
        set { _hexPieceState = value; }
    }

    [Header("Color Palette")]
    [SerializeField] private Color redColor;
    [SerializeField] private Color yellowColor;
    [SerializeField] private Color greenColor;
    [SerializeField] private Color blueColor;
    [SerializeField] private Color snowColor;
    [SerializeField] private Color greyColor;
    [SerializeField] private Color violetColor;
    [SerializeField] private Color orangeColor;


    [SerializeField] private HexPieceType _hexPieceType;

    public HexPieceType hexPieceType {
        get { return _hexPieceType; }
        set { _hexPieceType = value;
            switch (value) {
                case HexPieceType.Red:
                    color = redColor;
                    break;
                case HexPieceType.Yellow:
                    color = yellowColor;
                    break;
                case HexPieceType.Green:
                    color = greenColor;
                    break;
                case HexPieceType.Blue:
                    color = blueColor;
                    break;
                case HexPieceType.Snow:
                    color = snowColor;
                    break;
                case HexPieceType.Grey:
                    color = greyColor;
                    break;
                case HexPieceType.Violet:
                    color = violetColor;
                    break;
                case HexPieceType.Orange:
                    color = orangeColor;
                    break;
            }
        }
    }

    public Color color {
        set { material.color = value; }
    }

    [SerializeField] private HexPieceTower _parentHexPieceTower;
    public HexPieceTower parentHexPieceTower {
        get { return _parentHexPieceTower; }
        private set { _parentHexPieceTower = value; }
    }
    [SerializeField] private HexCell _parentHexCell;
    public HexCell parentHexCell {
        get { return _parentHexCell; }
        private set { _parentHexCell = value; }
    }

    private List<Vector3> pts = null;

    private Vector3 locPos {
        set {
            Vector3 midPoint = new Vector3(
                0.5f * (transform.localPosition.x + value.x),
                2.5f * (transform.localPosition.y + value.y),
                0.5f * (transform.localPosition.z + value.z)
                );
            pts = new List<Vector3>() { transform.localPosition, midPoint, value };
        }
    }

    private List<Quaternion> ptsRot = null;

    public Quaternion locRot {
        get { return transform.localRotation; }
        set {
            ptsRot = new List<Quaternion> { transform.localRotation, value };
        }
    }

    public Vector3 locPosImmediate {
        set {
            transform.localPosition = value;
        }
    }

    public Vector3 locScale
    {
        get { return transform.localScale; }
        set { transform.localScale = value; }
    }



    public Renderer rend;
    public Material material;

    public static HexPieceType GetHexPieceType(int i) {
        switch (i) {
            case 0:
                return HexPieceType.Red;
            case 1:
                return HexPieceType.Yellow;
            case 2:
                return HexPieceType.Green;
            case 3:
                return HexPieceType.Blue;
            case 4:
                return HexPieceType.Snow;
            case 5:
                return HexPieceType.Grey;
            case 6:
                return HexPieceType.Violet;
            case 7:
                return HexPieceType.Orange;
        }
        return HexPieceType.Red;
    }

    public void SetParentTower(HexPieceTower hexPieceTower) {
        parentHexPieceTower = hexPieceTower;
        SetParent(hexPieceTower.transform);
    }

    public void SetParent(Transform newParent) { 
        transform.SetParent(newParent);
    }

    public void Awake() {
        rend = GetComponent<Renderer>();
        material = rend.material;
        hexPieceState = HexPieceState.AtCell;
    }

    public void Start()
    {
        SetAnimationSpeed(GameManager.animationSpeed);
        GameManager.OnAnimationSpeedChanged += SetAnimationSpeed;
    }

    public void Update()
    {
        switch (hexPieceState) {
            case HexPieceState.MovingToLocPos:
                MoveToLocPos();
                break;
            case HexPieceState.Despawn:
                Removing();
                break;
        }
        
    }

    public void StartMovingToLocPos(Vector3 locPos, bool withFlippping, int moveGapCounts) {
        hexPieceState = HexPieceState.MovingToLocPos;
        this.locPos = locPos;

        timeMoveDurationFinal = timeMoveDurationInit + moveGapCounts * timeMoveGapDuration;


        // I don't have any ideas why this is work as supposed
        if (withFlippping) {
            isFlipping = true;
            Vector3 dir = pts[1] - pts[0];
            dir.y = 0;
            Vector3 posStart = pts[0];
            posStart.y = 0;
//            Debug.Log("Pos start: " + posStart + " direction " +dir);

            ptsRot = new List<Quaternion>()
            {
                transform.rotation,
                Quaternion.FromToRotation(Vector3.up, dir),
                Quaternion.FromToRotation(Vector3.up, Vector3.down)
            };
        }
        timeMoving = 0;


        switch (GameManager.Instance.gameState) {
            case GameState.MainMenu:
                MainMenuLogo.Instance.AddToWaitingQueue(this);
                break;
            case GameState.GamePlay:
                GameplayManager.Instance.AddToWaitingQueue(this);
                break;
        }
    }

    public void MoveToLocPos() {
        if (timeMoving == -1) return;
        timeMoving += Time.deltaTime * animationSpeed;
        float rTimeMove = timeMoving / timeMoveDurationFinal;
        rTimeMove = Mathf.Clamp01(rTimeMove);

        Vector3 tPos = Util.Bezier(rTimeMove, pts);
        transform.localPosition = tPos;

        if (isFlipping) {
            Quaternion tRot = Util.Bezier(rTimeMove, ptsRot);
            transform.localRotation = tRot;
        }

        if (rTimeMove == 1) {
            transform.localRotation = Quaternion.Euler(Vector3.up);
            EndMovingToLocPos();
        }
    }

    public void EndMovingToLocPos()
    {
        isFlipping = false;
        timeMoving = -1;

        switch (GameManager.Instance.gameState)
        {
            case GameState.MainMenu:
                MainMenuLogo.Instance.RemoveFromWaitingQueue(this);
                break;
            case GameState.GamePlay:
                GameplayManager.Instance.RemoveFromWaitingQueue(this);
                break;
        }

        hexPieceState = HexPieceState.AtCell;
    }

    public void MoveToCell(HexCell newParentHexCell, bool withFlip, int moveGapCounts) { 
        SetParent(newParentHexCell.transform);
        newParentHexCell.AddPiece(this);

        Vector3 newLocPos = new Vector3(
            0,
            newParentHexCell.CountOfPieces() * newParentHexCell.pieceHeight,
            0);
        StartMovingToLocPos(newLocPos, withFlip, moveGapCounts);
    }

    public void StartRemove() {
        hexPieceState = HexPieceState.Despawn;
        timeRemoving = 0;
        GameplayManager.Instance.AddToWaitingQueue(this);
    }

    public void Removing() {
        if (timeRemoving == -1) return;
        timeRemoving += Time.deltaTime * animationSpeed;

        float deltaTime = Mathf.Clamp01(timeRemoving / timeRemovingDuration);
        locScale = new Vector3(1 - deltaTime, 1 - deltaTime, 1 - deltaTime);
        transform.Rotate(new Vector3(0, rotationSpeedRemoving*Time.deltaTime, 0));

        if (timeRemoving > timeRemovingDuration)
        {
            timeRemoving = -1f;
            EndRemoving();
        }
    }

    public void EndRemoving() {
        GameplayManager.Instance.RemoveFromWaitingQueue(this);
        ReturnToPool();
    }

    public void ReturnToPool() {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;

        Poolable.TryPool(gameObject);
    }

    public void RemoveImmediately() {
        ReturnToPool();
    }

    public void SetAnimationSpeed(float value) {
        animationSpeed = value;
    }

    public void OnDestroy()
    {
        GameManager.OnAnimationSpeedChanged -= SetAnimationSpeed;
    }
}