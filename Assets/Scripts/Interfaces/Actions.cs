using System;
using UnityEngine;

public class Actions : MonoBehaviour
{
    public static Action<RaycastHit> UnitMove;
    public static Action DeSelect;
    public static Action<Vector3> CameraLoadedPosition;
}