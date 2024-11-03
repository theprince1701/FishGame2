using System;
using UnityEngine;
using UnityEngine.UI;

public class MiniGame : MonoBehaviour
{
    public GameObject miniGameUI;
    public GameObject gameOverUI;
    public Scrollbar progressBar;
    public GameObject[] liveImages;
    public Image fishIcon;
    public Image boxIcon;

    public float scrollSpeed = 1f;
    private float speedIncrease = 1.5f;
    public float maxY = 130.5f;
    public float minY = -130.5f;
    public float scoreIncrease = .25f;
    
    private bool _isPlaying;
    private bool _isGoingUp;
    private float mult;
    private float _currentSpeedIncrease;

    private float _currentScore;
    private int _currentLife;
    
    private void Start()
    {
        StartMiniGame();
    }

    public void StartMiniGame()
    {
        miniGameUI.SetActive(true);
        gameOverUI.SetActive(false);
        _currentLife = 5;
        _isGoingUp = true;
        progressBar.size = 0f;
        boxIcon.transform.localPosition = new Vector3(0, minY + 10f, 0);
        _currentSpeedIncrease = 1f;
        mult = 1f;
        _isPlaying = true;
        foreach (GameObject img in liveImages)
        {
            img.SetActive(true);
        }
    }

    public void StopMiniGame()
    {
        _isPlaying = false;
        boxIcon.transform.localPosition = new Vector3(0, minY + 10f, 0);
        gameOverUI.SetActive(true);
        
        foreach (GameObject img in liveImages)
        {
            img.SetActive(false);
        }
    }
    
    private void Update()
    {
        if (_isPlaying)
        {
            float moveSpeed = scrollSpeed * Time.deltaTime;

            if (_isGoingUp && boxIcon.transform.localPosition.y >= maxY)
            {
                mult = -1f;
                _isGoingUp = false;
            }

            if (!_isGoingUp && boxIcon.transform.localPosition.y <= minY)
            {
                mult = 1f;
                _isGoingUp = true;
            }

            moveSpeed *= mult * _currentSpeedIncrease;
            boxIcon.transform.localPosition += new Vector3(0, moveSpeed, 0);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (CheckInBounds())
                {
                    _currentScore += scoreIncrease;
                    _currentSpeedIncrease *= speedIncrease;
                    _currentScore = Mathf.Clamp(_currentScore, 0f, 1f);
                    progressBar.size = _currentScore;

                    if (_currentScore >= 1f)
                    {
                        StopMiniGame();
                    }
                }
                else
                {
                    _currentLife--;
                    liveImages[_currentLife].SetActive(false);
                }
            }
        }
    }

    bool CheckInBounds()
    {
        RectTransform boxRect = boxIcon.GetComponent<RectTransform>();
        RectTransform fishRect = fishIcon.GetComponent<RectTransform>();

        // Check each corner of the boxIcon to see if it's within the fishIcon
        Vector3[] boxCorners = new Vector3[4];
        boxRect.GetWorldCorners(boxCorners);

        foreach (Vector3 corner in boxCorners)
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(fishRect, corner, null))
            {
                return false;
            }
        }

        return true;
    }
    
}
