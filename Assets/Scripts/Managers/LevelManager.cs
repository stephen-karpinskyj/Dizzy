using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelManager : BehaviourSingleton<LevelManager>
{
    #region Constants


    private const float NoviceMedalTime = 20f;
    private const float ProMedalTime = 8f;


    #endregion

    
    #region Fields


    private List<BombController> junk;
    private List<BombController> uncollectedJunk;

    private RocketController rocket;

    private bool hasInitialised = false;
    private bool isRunning = false;
    private bool canRun = false;

    private float startTime;
    private int earntJunk;


    #endregion


    #region Properties


    public RocketController Rocket
    {
        get { return this.rocket; }
    }


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
            this.StartRunning();
        }
    }


    #endregion


    #region Public


    public void Initialise()
    {
        Debug.Assert(!this.hasInitialised);

        this.hasInitialised = true;
        
        this.junk = new List<BombController>();
        this.uncollectedJunk = new List<BombController>();

        var prefab = Resources.Load<BombController>("Bomb");
        Debug.Assert(prefab);

        foreach (var dot in Object.FindObjectsOfType<Dot>())
        {
            this.junk.Add(Object.Instantiate(prefab, dot.transform.position, dot.transform.rotation) as BombController);
        }

        this.rocket = Object.FindObjectOfType<RocketController>();
        Debug.Assert(this.rocket);

        this.StopRunning(false);
    }

    public void HandlePickup(BombController j)
    {
        Debug.Assert(this.isRunning);

        this.earntJunk += j.ChosenElement.JunkValue;

        this.uncollectedJunk.Remove(j);

        if (this.uncollectedJunk.Count <= 0)
        {
            this.StopRunning(true);
        }
    }

    public void HandleOutOfBounds()
    {
        this.StopRunning(false);
    }


    #endregion


    #region Private


    private void StopRunning(bool onWin)
    {
        this.isRunning = false;

        foreach (var j in this.junk)
        {
            j.StopRunning();
        }

        this.rocket.StopRunning();

        Broadcast.SendMessage("LevelStop");
        this.StartCoroutine(this.BlockRunStart());

        if (onWin)
        {
            var runTime = Time.time - this.startTime;
            var newBestTime = StateManager.Instance.HandleRunComplete(runTime, NoviceMedalTime, ProMedalTime);

            Broadcast.SendMessage("LevelWin", newBestTime);
        }
        else
        {
            this.canRun = true;
        }

        StateManager.Instance.HandleNewJunk(this.earntJunk);

        CameraController.Shake(0.05f, 0.05f);
    }

    private void StartRunning()
    {
        Debug.Assert(!this.isRunning);

        this.isRunning = true;

        this.Rocket.StartRunning();

        this.uncollectedJunk.Clear();

        foreach (var j in this.junk)
        {
            j.StartRunning();

            this.uncollectedJunk.Add(j);
        }

        this.startTime = Time.time;
        this.earntJunk = 0;

        Broadcast.SendMessage("LevelStart");
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
