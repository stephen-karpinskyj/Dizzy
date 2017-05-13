using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(MultiImageButton))]
public class MultiImageButtonEditor : UnityEditor.UI.ButtonEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var b = (MultiImageButton)this.target;
        if (b.transition == Selectable.Transition.ColorTint)
        {
            EditorGUI.BeginChangeCheck();

            var p = this.serializedObject.FindProperty("additionalTargetGraphics");
            Debug.Assert(p != null, this);
            EditorGUILayout.PropertyField(p, true);

            if (EditorGUI.EndChangeCheck())
            {
                this.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}