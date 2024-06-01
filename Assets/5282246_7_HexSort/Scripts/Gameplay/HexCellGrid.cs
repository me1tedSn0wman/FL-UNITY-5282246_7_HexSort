using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCellGrid : MonoBehaviour
{
    public static Transform CELL_ANCHOR;
    [SerializeField] private Transform cellAnchor;

    public static int W;
    public static int H;
    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;

    public float cellSize = 1f;

    public static HexCell[,] HEX_CELL_MAP;

    [SerializeField] private HexCell hexCellPrefab;

    private const float SQRT3 = 1.732f;

    public void Awake()
    {
    }

    public void Init() {
        CELL_ANCHOR = cellAnchor;
    }

    public void Init(int mapWidth, int mapHeight) {
        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;
        CELL_ANCHOR = cellAnchor;
    }

    public void CreateGrid() {
        W = mapWidth;
        H = mapHeight;
        HEX_CELL_MAP = new HexCell[W, H];

        for (int j = 0; j < mapHeight; j++) {
            for (int i = 0; i < mapWidth; i++) {
                SpawnCellGrid(new Vector2Int(i, j));
            }
        }
    }

    public void SpawnCellGrid(Vector2Int gridPos) {
        HexCell newCell = Instantiate<HexCell>(hexCellPrefab);

        newCell.transform.SetParent(CELL_ANCHOR);
        newCell.SetCell(gridPos, cellSize);

        HEX_CELL_MAP[gridPos.x, gridPos.y] = newCell;
    }

    static public HexCell TRY_GET_CELL(Vector2Int gridPos) {
        if (gridPos.x < 0 || gridPos.x >= W || gridPos.y < 0 || gridPos.y >= H) {
            return null;
        }
        return HEX_CELL_MAP[gridPos.x, gridPos.y];
    }

    public Vector3 CenterOfGridPos() {
        float widthSize = mapWidth % 2 ==0 ? (1.5f * mapWidth + SQRT3 * 0.25f) *0.5f* cellSize  : ((0.75f * mapWidth +0.25f)*cellSize);
        float heightSize = cellSize * (SQRT3*0.5f*(mapHeight-1) + SQRT3 *0.75f);
        return new Vector3(widthSize, 0 ,heightSize);
    }
}
