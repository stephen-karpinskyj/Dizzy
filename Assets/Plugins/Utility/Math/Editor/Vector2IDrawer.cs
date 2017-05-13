using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Vector2I))]
public class Vector2IDrawer : PropertyDrawer
{
    private static readonly GUIContent[] content = { new GUIContent("X"), new GUIContent("Y") };

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position = EditorGUI.PrefixLabel(position, label);

        position.x -= 1;
        position.width *= 2/3f;

        if (property.Next(true))
        {
            EditorGUI.MultiPropertyField(position, content, property);
        }
    }
}
