using UnityEngine;
using System.Collections;

public class PauseGame : MonoBehaviour
{

    private bool m_paused;

    void Start()
    {
        m_paused = false;
    }

    void Update()
    {
        // Check for when the game comes back into focus
        // Audio sometimes resume by itself,
        // this will reset the audioListeners paused state,
        // until we are ready to unpause the game.
        if(m_paused && !AudioListener.pause)
        {
            AudioListener.pause = true;
        }
    }

    // Pause The Game
    public void Pause()
    {
        m_paused = true;
        Time.timeScale = 0;
        AudioListener.pause = true;
        AudioListener.volume = 0;
    }

    // Resume The Game
    public void Resume()
    {
        m_paused = false;
        Time.timeScale = 1;
        AudioListener.pause = false;
        AudioListener.volume = 1f;
    }

    // Return Paused status
    public bool IsPaused()
    {
        return m_paused;
    }
}
