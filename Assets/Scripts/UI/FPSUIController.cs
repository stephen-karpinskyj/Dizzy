using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FPSUIController : MonoBehaviour
{
    [SerializeField]
    private Text text;
    
    [SerializeField]
    private float frequency = 0.5F; // The update frequency of the fps
	
    [SerializeField]
    private int numDecimal = 1; // How many decimal do you want to display
 
    [SerializeField]
    private float yellowFps = 50f;
    
    [SerializeField]
    private float greenFps = 60f;
 
	private float accum = 0f; // FPS accumulated over the interval
	private int frames = 0; // Frames drawn over the interval
 
	private void Awake()
	{
	    this.StartCoroutine(this.UpdateCoroutine());
	}
 
	private void Update()
	{
	    this.accum += Time.timeScale/Time.deltaTime;
        this.frames++;
	}
 
	private IEnumerator UpdateCoroutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(this.frequency);
            
		    var fps = this.accum / this.frames;

		    this.text.text = fps.ToString("f" + Mathf.Clamp(numDecimal, 0, 10));
 			this.text.color = (fps >= this.greenFps) ? Color.green : (fps >= this.yellowFps) ? Color.yellow : Color.red;
 
	        this.accum = 0f;
	        this.frames = 0;
		}
	}
}
