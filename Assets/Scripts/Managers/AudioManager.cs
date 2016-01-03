using UnityEngine;

public class AudioManager : BehaviourSingleton<AudioManager>
{
    public void Play(AudioSource source)
    {
        Debug.Assert(source);

        if (!StateManager.Instance.SoundEnabled)
        {
            return;
        }

        source.Play();
    }

    public void Stop(AudioSource source)
    {
        Debug.Assert(source);

        if (!StateManager.Instance.SoundEnabled)
        {
            return;
        }

        source.Stop();
    }
}
