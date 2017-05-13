using System.Collections.Generic;
using UnityEngine;

public class UIPanelManager : BehaviourSingleton<UIPanelManager>
{
    [SerializeField]
    private List<UIPanel> panelPrefabs;

    private List<Transform> originalChildren = new List<Transform>();
    private RectTransform rectTransform;

    private void Awake()
    {
        this.GetComponentsInChildren<Transform>(this.originalChildren);
        this.rectTransform = this.GetComponent<RectTransform>();
    }

    private T GetPanelPrefab<T>() where T : UIPanel
    {
        var prefab = this.panelPrefabs.Find(p => p is T) as T;
        Debug.Assert(prefab != null, this);

        return prefab;
    }

    public T OpenPanel<T>(bool asModal = false) where T : UIPanel
    {
        if (asModal)
        {
            foreach (Transform t in this.transform)
            {
                if (!this.originalChildren.Contains(t))
                {
                    var p = t.GetComponent<UIPanel>();

                    if (p)
                    {
                        p.Close();
                    }
                }
            }
        }

        return GameObjectUtility.InstantiatePrefab(this.GetPanelPrefab<T>(), this.transform, false);
    }

    public Vector2 GlobalPositionToCanvasPosition(Camera camera, Vector3 globalPosition)
    {
        var viewportPosition = camera.WorldToViewportPoint(globalPosition);
        return new Vector2(
            (viewportPosition.x * this.rectTransform.sizeDelta.x),
            (viewportPosition.y * this.rectTransform.sizeDelta.y));
    }
}
