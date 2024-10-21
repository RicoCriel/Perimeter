using UnityEngine;

public static class PlayerHelper
{
    //Extension methods
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45f, 0));
    public static Vector3 ConvertToIsoCoords(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);

    public static bool IsSprinting(float inputValue) => inputValue > 0.5f;

    public static Vector3 CalculateMovement(Vector2 input)
    {
        Vector3 movement = new Vector3(input.x, 0, input.y);
        return -movement.ConvertToIsoCoords();
    }


}
 