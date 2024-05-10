using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotPiece : MonoBehaviour
{
    public int id;
    private Vector3 initialScale;
    private void Awake()
    {
        initialScale = transform.localScale;
    }
    public void Grow(bool haveTo)
    {
        if (haveTo) LeanTween.scale(gameObject, Vector3.one * 2, 0.2f);
        else LeanTween.scale(gameObject, initialScale, 0.2f);
    }

}
