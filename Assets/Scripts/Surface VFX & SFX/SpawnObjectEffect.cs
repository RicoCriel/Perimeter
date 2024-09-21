using UnityEngine;

[CreateAssetMenu(menuName = "Impact System/Spawn Object Effect", fileName = "SpawnObjectEffect")]
public class SpawnObjectEffect : ScriptableObject
{
    public GameObject Prefab;
    public float Probability = 1;
    public float Lifetime;
    public int MaxAmount;
    public bool RandomizeRotation;
    [Tooltip("Zero values will lock the rotation on that axis. Values up to 360 are sensible for each X,Y,Z")]
    public Vector3 RandomizedRotationMultiplier = Vector3.zero;
}
