using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    [SerializeField] float _time;
    [SerializeField] private TMP_Text _timerText;

    [Space(10)]
    [SerializeField] private UnityEvent _onPlay;
    [SerializeField] private UnityEvent _onPause;
    [SerializeField] private UnityEvent _onResume;
    [SerializeField] private UnityEvent _onGameWin;
    [SerializeField] private UnityEvent _onGameLose;
    
    private PlayerInputActions _inputActions;

    private void Start()
    {
        Time.timeScale = 0f;
        _inputActions = new PlayerInputActions();
        _inputActions.Player.Enable();
        _inputActions.Player.Escape.performed += Pause;
    }

    private void OnEnable()
    {
        if (_inputActions != null)
        {
            _inputActions.Player.Enable();
        }
    }

    private void OnDisable()
    {
        if (_inputActions != null)
        {
            _inputActions.Player.Disable();
        }
    }

    void Update()
    {
        _time -= Time.deltaTime;
        if (_time > 0)
        {
            _timerText.text = string.Format("{0:00}:{1:00}", (int)_time / 60, (int)_time % 60);
        }
        else
        {
            GameOver(false);
        }
    }

    void Pause(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        _onPause.Invoke();
        Time.timeScale = 0f;
    }

    public void GameOver(bool isWin)
    {
        Time.timeScale = 0f;
        if (isWin)
        {
            _onGameWin?.Invoke();
        }
        else
        {
            _onGameLose?.Invoke();
        }
    }

    public void OnClickPlay()
    {
        _onPlay.Invoke();
        Time.timeScale = 1f;
    }

    public void OnClickResume()
    {
        _onResume.Invoke();
        Time.timeScale = 1f;
    }

    public void OnClickExit()
    {
        Application.Quit();
    }

    public void OnClickReload()
    {
        SceneManager.LoadScene(0);
    }
    
}
