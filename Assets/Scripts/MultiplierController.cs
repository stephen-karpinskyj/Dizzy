using UnityEngine;

public class MultiplierController : BehaviourSingleton<MultiplierController>
{
    const float SecondsUntilExpire = 1f;

    private int currentMultiplier = 0;
    private float lastExplosionTime = 0f;

    public int CurrentMultiplier
    {
        get { return this.currentMultiplier; }
    }

    private void Update()
    {
        if ((Time.time - lastExplosionTime) >= SecondsUntilExpire)
        {
            if (!Input.anyKey)
            {
                this.currentMultiplier = 0;
                this.lastExplosionTime = 0f;
            }
        }
    }

    public void Increment()
    {
        this.currentMultiplier++;
        this.lastExplosionTime = Time.time;
    }


    #region Broadcast


    private void LevelStop()
    {
        this.currentMultiplier = 0;
        this.lastExplosionTime = 0f;
    }


    #endregion
}
