using UnityEngine;

[System.Serializable]
public class MinimapComponentData
{
    public string name;
    public Vector3 position;
    public Vector3 scale;
    public Vector3 rotation;

    public MinimapComponentData(string _name, Vector3 _position, Vector3 _scale,  Vector3 _rotation)
    {
        name = _name;
        position = _position;
        scale = _scale;
        rotation = _rotation;
    }
}
