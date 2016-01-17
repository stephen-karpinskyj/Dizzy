using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : BehaviourSingleton<GameManager>
{
    #region Constants


    private const float NoviceMedalTime = 20f;
    private const float ProMedalTime = 7f;


    #endregion

    
    #region Fields


    private LevelManager levelManager;
    private CanvasUIController canvas;
    
    private LaunchLightsController launchLights;

    private bool hasInitialised = false;
    private bool isRunning = false;
    private bool canRun = false;


    #endregion


    #region Properties


    public ShipController Ship { get; private set; }

    public MultiplierController Multiplier { get; private set; }
    

    #endregion


    #region Unity


    private void Update()
    {   
        if (this.isRunning || !this.canRun)
        {
            return;
        }

        var shouldRun = false;

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            shouldRun = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began &&
                (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId));
        }
        else
        {
            shouldRun = Input.anyKeyDown && (!EventSystem.current || !EventSystem.current.IsPointerOverGameObject());
        }
        
        if (shouldRun)
        {
            this.OnLevelStart();
        }
    }


    #endregion


    #region Public


    public void Initialise()
    {
        Debug.Assert(!this.hasInitialised);

        this.hasInitialised = true;

        this.levelManager = this.gameObject.AddComponent<LevelManager>();
        Debug.Assert(this.levelManager);
        
        this.canvas = Object.FindObjectOfType<CanvasUIController>();
        Debug.Assert(this.canvas);
        
        this.Ship = Object.FindObjectOfType<ShipController>();
        Debug.Assert(this.Ship);
        
        this.Multiplier = this.gameObject.AddComponent<MultiplierController>();
        
        this.launchLights = Object.FindObjectOfType<LaunchLightsController>();
        Debug.Assert(this.launchLights);
        
        this.OnLevelLoad(Data.Instance.GetDefaultLevel());

        this.OnLevelStop(false);
    }

    public void HandleOutOfBounds()
    {
        this.OnLevelStop(false);
    }
    
    public void ResetProgress()
    {
        StateManager.Instance.ResetProgress();
        this.canvas.ForceUpdateAll(this.levelManager.CurrentLevelState, StateManager.Instance.JunkCount);
    }


    #endregion


    #region Private


    private void OnLevelStop(bool won)
    {
        Debug.LogFormat("[{0}] Stopping level, won={1}", this.GetType().Name, won);
        
        this.isRunning = false;

        this.Ship.OnLevelStop();
        this.Multiplier.OnLevelStop();
        this.launchLights.OnLevelStop();

        this.canvas.Show(true);
        this.levelManager.OnLevelStop();
        
        if (won)
        {
            this.levelManager.OnLevelWin();
            this.StartCoroutine(this.BlockRunStart());
        }
        else
        {
            this.canRun = true;
        }
        
        CameraController.Shake(0.05f, 0.05f);
    }

    private void OnLevelStart()
    {
        Debug.LogFormat("[{0}] Starting level", this.GetType().Name);
        
        Debug.Assert(!this.isRunning);

        this.isRunning = true;

        this.Ship.OnLevelStart();
        this.launchLights.OnLevelStart();

        this.levelManager.OnLevelStart();
        this.canvas.OnLevelStart(this.levelManager.CurrentLevelState, StateManager.Instance.JunkCount);
    }
    
    private void OnLevelLoad(LevelData level)
    {
        this.levelManager.OnLevelLoad(this.canvas, level, () => this.OnLevelStop(true));
        this.canvas.OnLevelLoad(this.levelManager.CurrentLevelState, StateManager.Instance.JunkCount);
    }


    #endregion


    #region Coroutines


    private IEnumerator BlockRunStart()
    {
        this.canRun = false;
        yield return new WaitForSeconds(0.5f);
        this.canRun = true;
    }


    #endregion
}
