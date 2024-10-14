using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerHelper
{
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45f, 0));
    public static Vector3 ConvertToIsoCoords(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);
}
 