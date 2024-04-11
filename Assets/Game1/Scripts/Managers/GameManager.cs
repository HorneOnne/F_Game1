using UnityEngine;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<CharacterDataSO> Characters;
    private int _currentCharacterIndex = 0;
    public CharacterDataSO CurrentCharacter;
    public int NumOfPlayers;

    public List<Map> Levels;
    public Map CurrentMap { get; private set; }
    public int CurrentLevel = 1;

    public CharacterDataSO NextUnlockCharacter;

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



        // Get next unlock character.
        NextUnlockCharacter = Characters[Characters.Count - 1];
        GetNextUnlockCharacter();
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

    public void LoadLevel(int level)
    {
        CurrentLevel = level;
        CurrentMap = Instantiate(Levels[level - 1], Vector2.zero, Quaternion.identity);  
    }

    public void UnlockNextCharacter()
    {
        NextUnlockCharacter.Unlock = true;
        Debug.Log("unlock");
        for(int i = 0; i < Characters.Count; i++)
        {
            if (Characters[i].ID == NextUnlockCharacter.ID)
            {
                Characters[i].Unlock = true;
            }
        }
    }

    public CharacterDataSO GetNextUnlockCharacter()
    {
        for (int i = 0; i < Characters.Count; i++)
        {
            if (Characters[i].Unlock == false)
            {
                NextUnlockCharacter = Characters[i];
                break;
            }
        }

        return NextUnlockCharacter;
    }
}

