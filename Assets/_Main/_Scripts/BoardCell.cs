using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCell
{
    public SlotPiece slotPiece;
    private Vector3Int[] directions = {
        Vector3Int.up,
        Vector3Int.down,
        Vector3Int.left,
        Vector3Int.right
    };
    public (bool, int) IsConnected(Vector3Int myPosition, ref List<BoardCell> cellsChecked, ref int connectionCounter)
    {
        bool haveEnoughConnections = false;
        cellsChecked.Add(this);
        connectionCounter++;

        foreach (Vector3Int direction in directions)
        {
            Vector3Int neighbourPos = myPosition + direction;
            if (GameBoard.instance.Board.ContainsKey(neighbourPos))
            {
                BoardCell neighbourCell = GameBoard.instance.Board[neighbourPos];

                if (cellsChecked.Contains(neighbourCell)) continue;

                if (neighbourCell.slotPiece.id == slotPiece.id)
                {
                    var connection = neighbourCell.IsConnected(neighbourPos, ref cellsChecked, ref connectionCounter);
                    connectionCounter = connection.Item2;
                    haveEnoughConnections = connectionCounter >= GameBoard.instance.MinConnections;
                }
            }
        }
        return (haveEnoughConnections, connectionCounter);
    }
}
