using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    [SerializeField] float _time;
    [SerializeField] private TMP_Text _timerText;

    [Space(10)]
    [SerializeField] private UnityEvent _onGameWin;
    [SerializeField] private UnityEvent _onGameLose;
    
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

    public void OnClickReload()
    {
        SceneManager.LoadScene(0);
    }
    
}
