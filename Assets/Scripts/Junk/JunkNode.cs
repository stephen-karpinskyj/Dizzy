using UnityEngine;

/// <summary>
/// Initial settings to instantiate a <c>JunkController</c>.
/// </summary>
public class JunkNode : LevelObjectNode
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(this.transform.position, 0.12f);
    }
}
