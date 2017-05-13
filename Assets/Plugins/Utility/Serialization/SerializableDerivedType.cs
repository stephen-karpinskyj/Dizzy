using System;

[Serializable]
/// <remarks>Needed because JsonUtility can't automatically figure out derived types.</remarks>
public struct SerializableDerivedType
{
    public string Type;
    public string Json;
}