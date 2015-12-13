#pragma strict

// Source: http://unitytipsandtricks.blogspot.co.nz/2013/05/camera-shake.html
static function Shake (duration: float, magnitude: float) {
    var elapsed = 0.0f;
    
    var originalCamPos = Camera.main.transform.position;
    
    while (elapsed < duration) {
        elapsed += Time.deltaTime;          
        
        var percentComplete = elapsed / duration;         
        var damper = 1 - Mathf.Clamp(4 * percentComplete - 3, 0, 1);
        
        // map value to [-1, 1]
        var x = Random.value * 2.0f - 1.0f;
        var y = Random.value * 2.0f - 1.0f;
        x *= magnitude * damper;
        y *= magnitude * damper;
        
        Camera.main.transform.position = Vector3(x, y, originalCamPos.z);
            
        yield null;
    }
    
    Camera.main.transform.position = originalCamPos;
}
