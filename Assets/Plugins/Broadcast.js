#pragma strict

static function SendMessage (messageName: String) {
    for (var go : GameObject in GameObject.FindObjectsOfType(GameObject)) {
        go.SendMessage(messageName, SendMessageOptions.DontRequireReceiver);
    }
}

static function SendMessage (messageName: String, messageParameter: Object) {
    for (var go : GameObject in GameObject.FindObjectsOfType(GameObject)) {
        go.SendMessage(messageName, messageParameter, SendMessageOptions.DontRequireReceiver);
    }
}
