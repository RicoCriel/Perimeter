using UnityEngine;
public enum Type
{
    Environment,
    Enemy
}

[System.Serializable]
public class SurfaceType
{
    public Texture AlbedoTexture;
    public Surface Surface;
    public Type Type;
}
