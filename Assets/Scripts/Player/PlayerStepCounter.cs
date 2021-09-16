using System.Collections;
using System.Collections.Generic;
using FG;
using UnityEngine;

public class PlayerStepCounter : MonoBehaviour {
    [SerializeField]
    [Tooltip("When the player has reached new max distance, the player get points")]
    private int scoreToAdd;

    private float _movedDistance = 0;

    /// <summary>
    /// If the new position is higher than saved position, and is divided by 10, the player gets score.
    /// Also the level generator speeds up the track spawning
    /// </summary>
    /// <param name="zPosition"></param>
    public void UpdatePosition(float zPosition) {
        if (zPosition > _movedDistance) {
            _movedDistance = zPosition;

            if (_movedDistance % 10 == 0) {
                LevelGenerator.Instance.SpeedUpTrackSpawn();
                ScoreManager.Instance.AddScore(scoreToAdd);    
            }
        }
    }
}
