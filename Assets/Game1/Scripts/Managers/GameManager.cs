using UnityEngine;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<CharacterDataSO> Characters;
    private int _currentCharacterIndex = 0;
    public CharacterDataSO CurrentCharacter;
    public int NumOfPlayers;



    private void Awake()
    {
        // Check if an instance already exists, and destroy the duplicate
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        CurrentCharacter = Characters[_currentCharacterIndex];
        // FPS
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        // Make the GameObject persist across scenes
        DontDestroyOnLoad(this.gameObject);
    }

    public void TurnLeft()
    {
        _currentCharacterIndex--;
        if (_currentCharacterIndex < 0)
        {
            _currentCharacterIndex = Characters.Count - 1;
        }
        CurrentCharacter = Characters[_currentCharacterIndex];
    }

    public void TurnRight()
    {  
        _currentCharacterIndex = (_currentCharacterIndex + 1) % Characters.Count;
        CurrentCharacter = Characters[_currentCharacterIndex];
    }
}

