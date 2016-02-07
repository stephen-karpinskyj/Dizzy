using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : BehaviourSingleton<GameManager>
{
    #region Fields


    private LevelManager level;
    private WorldManager world;
    private CanvasUIController canvas;
    private StarfieldController starfield;
    
    private LaunchLightsController launchLights;

    private bool hasInitialised = false;
    private bool isRunning = false;
    private bool canRun = false;
    
    private Vector2 offset;
    private Vector3 prevAcceleration;
    
    private bool isExploration;


    #endregion


    #region Properties


    public bool IsRunning { get { return this.isRunning; } }

    public ShipController Ship { get; private set; }

    public MultiplierController Multiplier { get; private set; }
    

    #endregion


    #region Unity


    private void Update()
    {   
        this.UpdateStarfield();
        
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
            shouldRun = Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) ||
                (Input.GetMouseButtonDown(0) && (!EventSystem.current || !EventSystem.current.IsPointerOverGameObject()));
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

        this.level = this.gameObject.AddComponent<LevelManager>();
        Debug.Assert(this.level);
        
        this.world = Object.FindObjectOfType<WorldManager>();
        Debug.Assert(this.world);
        
        this.canvas = Object.FindObjectOfType<CanvasUIController>();
        Debug.Assert(this.canvas);
        
        this.starfield = Object.FindObjectOfType<StarfieldController>();
        Debug.Assert(this.starfield);
        
        this.Ship = Object.FindObjectOfType<ShipController>();
        Debug.Assert(this.Ship);
        
        this.Multiplier = this.gameObject.AddComponent<MultiplierController>();
        
        this.launchLights = Object.FindObjectOfType<LaunchLightsController>();
        Debug.Assert(this.launchLights);
        
        this.OnGameStart();
        this.OnLevelLoad(Data.Instance.GetDefaultLevel());
    }

    public void HandleOutOfBounds()
    {
        this.OnLevelStop(false);
    }
    
    public void ResetProgress()
    {
        StateManager.Instance.ResetProgress(this.level.CurrentLevel as TrialLevelData);
        this.level.ReinitialiseJunkValues();
        this.canvas.ForceUpdateAll(this.level.CurrentLevel, this.level.CurrentLevelState, StateManager.Instance.JunkCount, StateManager.Instance.JunkMultiplier);
    }
    
    public void LoadNextLevel(bool forward)
    {
        var currLevel = this.level.CurrentLevel;
        var nextLevel = Data.Instance.GetNextLevel(currLevel, forward);
        
        this.OnLevelLoad(nextLevel);
    }


    #endregion


    #region Private


    private void OnGameStart()
    {
        this.level.OnGameStart(this.canvas);
    }

    private void OnLevelLoad(LevelData level)
    {
        Debug.LogFormat("[{0}] Loading level={1}", this.GetType().Name, level.Id);
        
        this.isExploration = level is ExplorationLevelData;
        
        if (this.isExploration)
        {
            CameraController.AddChild(this.starfield.transform);
        }
        else
        {
            this.starfield.transform.parent = null;
            this.starfield.ResetPosition();
        }
        
        CameraController.ChangeMode(this.isExploration);
        
        this.level.OnLeveUnload();
        this.level.OnLevelLoad(level, () => this.OnLevelStop(true));
        this.OnLevelStop(false);
        
        this.canvas.OnLevelLoad(this.level.CurrentLevel, this.level.CurrentLevelState, StateManager.Instance.JunkCount, StateManager.Instance.JunkMultiplier);
    }
    
    private void OnLevelStop(bool won)
    {
        Debug.LogFormat("[{0}] Stopping level, won={1}", this.GetType().Name, won);
        
        this.isRunning = false;

        this.Ship.OnLevelStop();
        this.Multiplier.OnLevelStop();
        
        this.launchLights.OnLevelStop(this.level.CurrentLevel is TrialLevelData);
        
        this.canvas.OnLevelStop(this.level.CurrentLevel);
        this.level.OnLevelStop();
        
        if (won)
        {
            this.level.OnLevelWin();
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

        this.level.OnLevelStart();
        this.canvas.OnLevelStart(this.level.CurrentLevel, this.level.CurrentLevelState, StateManager.Instance.JunkCount, StateManager.Instance.JunkMultiplier);
        
        if (this.isExploration)
        {
            // SK: TODO: Fuel mechanic
            this.StartCoroutine(this.DelayedLevelStop(15f));
        }
    }
    
    private void UpdateStarfield()
    {
        if (this.isExploration)
        {
            this.offset = (Vector2)GameManager.Instance.Ship.Trans.position;
        }
        else
        {
            this.offset = this.GetAcceleration() * 0.5f;
        }
        
        this.starfield.UpdateOffset(this.offset);
    }

    private Vector3 GetAcceleration()
    {
        // Source: http://stackoverflow.com/questions/24501290/unity3d-android-accelerometer-and-gyroscope-controls
        const float AccelerometerUpdateInterval = 1f / 100f;
        const float LowPassKernelWidthInSeconds = 0.1f;
        const float LowPassFilterFactor = AccelerometerUpdateInterval / LowPassKernelWidthInSeconds;
        
        this.prevAcceleration = Vector3.Lerp(this.prevAcceleration, Input.acceleration, LowPassFilterFactor);
        
        return this.prevAcceleration;
    }


    #endregion


    #region Coroutines


    private IEnumerator BlockRunStart()
    {
        this.canRun = false;
        yield return new WaitForSeconds(0.5f);
        this.canRun = true;
    }
    
    private IEnumerator DelayedLevelStop(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        this.HandleOutOfBounds();
    }


    #endregion
}
