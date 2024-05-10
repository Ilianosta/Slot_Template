using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameBoardAnimation : MonoBehaviour
{
    [SerializeField] private AnimationType _animationType;
    [SerializeField] private float _duration;
    [SerializeField] private float _delayBetween;
    [SerializeField] private LeanTweenType _easing;
    Dictionary<Vector3Int, BoardCell> _board;

    public bool isAnimating = false;

    [System.Serializable]
    public enum AnimationType
    {
        TopDown,
        LeftRight
    }
    public void Animate(Dictionary<Vector3Int, BoardCell> board, AnimationType animationType)
    {
        _board = board;
        System.Action animation = animationType switch
        {
            AnimationType.TopDown => AnimateTopDown,
            AnimationType.LeftRight => AnimateLeftRight,
            _ => throw new System.Exception("Unhandled animation type")
        };
        animation.Invoke();
    }
    public void Animate(Dictionary<Vector3Int, BoardCell> board) => Animate(board, _animationType);

    private void AnimateTopDown()
    {
        Debug.Log("Animating top down");
        List<Vector3Int> positions = new List<Vector3Int>();

        foreach (Vector3Int pos in _board.Keys)
        {
            positions.Add(pos);
        }
        SetAnimations(positions);
    }
    private void AnimateLeftRight()
    {
        Debug.Log("Animating left right");
        List<Vector3Int> positions = new List<Vector3Int>();
        positions.AddRange(_board.Keys);

        // Ordenar la lista de posiciones de izquierda a derecha
        positions = positions.OrderBy(pos => pos.y).ThenBy(pos => pos.x).ThenBy(pos => pos.z).ToList();
        SetAnimations(positions);
    }

    private void SetAnimations(List<Vector3Int> positions)
    {
        isAnimating = true;
        float counter = 0;
        foreach (Vector3Int pos in positions)
        {
            LeanTween.delayedCall(counter, () =>
            {
                GameObject piece = _board[pos].slotPiece.gameObject;
                float distance = Vector3.Distance(transform.position, piece.transform.position);
                LeanTween.move(piece, pos, _duration * distance).setEase(_easing);
            });
            counter += _delayBetween;
        }
        LeanTween.delayedCall(counter, () => isAnimating = false);
    }
}
