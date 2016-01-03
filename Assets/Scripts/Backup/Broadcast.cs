using UnityEngine;

public static class Broadcast
{
    public static void SendMessage(string messageName)
    {
        foreach (GameObject go in Object.FindObjectsOfType<GameObject>())
        {
            go.SendMessage(messageName, SendMessageOptions.DontRequireReceiver);
        }
    }

    public static void SendMessage(string messageName, object parameter)
    {
        foreach (GameObject go in Object.FindObjectsOfType<GameObject>())
        {
            go.SendMessage(messageName, parameter, SendMessageOptions.DontRequireReceiver);
        }
    }
}
