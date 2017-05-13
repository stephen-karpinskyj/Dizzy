using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <remarks>Based on: http://answers.unity3d.com/questions/820311/ugui-multi-image-button-transition.html</remarks>
public class MultiImageButton : Button
{
    public List<Graphic> additionalTargetGraphics;

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);

        if (!this.gameObject.activeInHierarchy)
        {
            return;
        }

        if (this.transition == Transition.ColorTint)
        {
            Color color;
            switch (state)
            {
                case Selectable.SelectionState.Normal: color = this.colors.normalColor; break;
                case Selectable.SelectionState.Highlighted: color = this.colors.highlightedColor; break;
                case Selectable.SelectionState.Pressed: color = this.colors.pressedColor; break;
                case Selectable.SelectionState.Disabled: color = this.colors.disabledColor; break;
                default: color = Color.black; break;
            }

            switch (this.transition)
            {
                case Selectable.Transition.ColorTint: this.ColorTween(color * this.colors.colorMultiplier, instant); break;
                default: throw new NotSupportedException();
            }
        }
    }

    private void ColorTween(Color targetColor, bool instant)
    {
        if (this.additionalTargetGraphics == null)
        {
            this.additionalTargetGraphics = new List<Graphic>();
            foreach (Transform t in this.transform)
            {
                t.GetComponentsInChildren<Graphic>(this.additionalTargetGraphics);
            }
        }

        foreach (var g in this.additionalTargetGraphics)
        {
            if (!g)
            {
                continue;
            }
            
            g.CrossFadeColor(targetColor, !instant ? this.colors.fadeDuration : 0f, true, true);
        }
    }
}