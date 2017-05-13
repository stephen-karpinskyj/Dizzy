using System;
using UnityEngine;

using Object = UnityEngine.Object;

public abstract class UIPanel : MonoBehaviour
{
    public event Action OnClose = delegate { };

    public virtual void Close()
    {
        this.OnClose();

        Object.Destroy(this.gameObject);
    }

    public void OnCloseButtonPress()
    {
        this.Close();
    }
}
