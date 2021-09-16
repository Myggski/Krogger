using System;
using FG;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerGameOverChecker : MonoBehaviour
{
    [Scene]
    [SerializeField]
    private string gameOverScene = string.Empty;

    private bool _gameOver;

    /// <summary>
    /// If player is under 0 in y, the player is falling and lose the game
    /// </summary>
    private void CheckGameOver() {
        if (transform.position.y < 0 && !_gameOver) {
            _gameOver = true;
            SceneManager.LoadScene(gameOverScene, LoadSceneMode.Additive);
        }
    }
    
    void FixedUpdate()
    {
        CheckGameOver();
    }
}
