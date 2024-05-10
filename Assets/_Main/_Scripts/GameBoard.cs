using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
[RequireComponent(typeof(GameBoardAnimation))]
public class GameBoard : MonoBehaviour
{
    //Board hold a list of BoardAction that get ticked on its Update. Useful for Bonus to add timed effects and the like.
    public interface IBoardAction
    {
        //Return true if should continue, false if the action done 
        bool Tick();
    }
    public Tilemap tilemap;

    [SerializeField] private List<Vector3Int> initialPositions = new List<Vector3Int>();
    [SerializeField] GameObject[] slotPieces;

    Dictionary<Vector3Int, BoardCell> board = new Dictionary<Vector3Int, BoardCell>();
    private GameObject randomPiece => slotPieces[Random.Range(0, slotPieces.Length)];
    private GameBoardAnimation gameBoardAnimation;

    private List<IBoardAction> boardActions = new List<IBoardAction>();
    private void Awake()
    {
        gameBoardAnimation = GetComponent<GameBoardAnimation>();
    }

    void Start()
    {
        CreateBoard();
        Init();
    }

    private void Update()
    {
        if (gameBoardAnimation.isAnimating) return;
    }

    private void AddBoardAction(IBoardAction action) => boardActions.Add(action);


    [ContextMenu("Init")]
    private void Init()
    {
        if (transform.childCount > 0)
        {
            foreach (BoardCell cell in board.Values)
            {
                Destroy(cell.slotPiece.gameObject);
            }
        }

        foreach (Vector3Int pos in board.Keys)
        {
            Vector3Int positionToSpanw = initialPositions.Find(a => a.x == pos.x);
            GameObject piece = Instantiate(randomPiece, positionToSpanw, Quaternion.identity, transform);
            board[pos].slotPiece = piece.GetComponent<SlotPiece>();
        }
        gameBoardAnimation.Animate(board);
        StartCoroutine(CheckConnectedPieces());
    }
    private void CreateBoard()
    {
        // Obtener el tamaño de la grid en celdas
        BoundsInt bounds = tilemap.cellBounds;

        // Iterar sobre todas las celdas de la grid
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                // Obtener el tile en la posición (x, y)
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(tilePos);


                // Hacer algo con el tile obtenido (por ejemplo, imprimir su nombre)
                if (tile != null)
                {
                    if (tile.name == "initial_piece") initialPositions.Add(tilePos);
                    if (tile.name == "space_pieces")
                    {
                        BoardCell boardCell = new BoardCell();
                        board.Add(tilePos, boardCell);
                    }
                }
            }
        }
    }

    IEnumerator CheckConnectedPieces()
    {
        if (gameBoardAnimation.isAnimating)
            yield return new WaitUntil(() => !gameBoardAnimation.isAnimating);

        Debug.Log("Looking for connected pieces");
        foreach (var cell in board)
        {
            Vector3Int cellPosition = Vector3Int.RoundToInt(cell.Key);
            Vector3Int[] directions = { Vector3Int.down, Vector3Int.up, Vector3Int.left, Vector3Int.right };
            foreach (Vector3Int direction in directions)
            {
                Vector3Int adjacentPos = cellPosition + direction;

                // Verificar si la posición adyacente está dentro del tablero y si hay una pieza en esa posición
                if (board.ContainsKey(adjacentPos) && board[adjacentPos].slotPiece != null)
                {
                    Debug.Log("EXIST!");
                    if (board[adjacentPos].slotPiece.id == cell.Value.slotPiece.id)
                    {
                        Debug.Log("THEY ARE THE SAME");
                        board[adjacentPos].slotPiece.Grow(true);
                        cell.Value.slotPiece.Grow(true);
                    }
                }
            }
        }
    }

}
