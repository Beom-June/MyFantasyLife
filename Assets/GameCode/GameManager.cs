using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Menu,
    Loading,
    Playing,
    GameOver
}
public class GameManager : MonoBehaviour
{
    public static GameManager gameManager;
    public GameState currentGameState = GameState.Menu;

    public static int Stage;                        // static으로 전역에서 할 수 있게                                    // 게임 스테이지

    public GameObject Monsters;
    private void Awake()
    {
        gameManager = this;

        Stage = 1;
    }
    void Start()
    {
        StartGame();
        StartMonster();
    }

    void Update()
    {

    }
    public void StartGame()
    {
        // 게임이 시작되면 플레이 상태로
        SetGameState(GameState.Playing);
    }
    public void GameOver()
    {

    }

    void SetGameState(GameState newGameState)
    {
        if (newGameState == GameState.Menu)
        {

        }
        else if (newGameState == GameState.Loading)
        {

        }
        else if (newGameState == GameState.Playing)
        {

        }
        else if (newGameState == GameState.GameOver)
        {

        }
        currentGameState = newGameState;
    }

    void StartMonster()
    {
        for (int i = 1; i < Monsters.transform.childCount + 1; i++)
        {
            if (Stage == i)
            {
                Monsters.transform.GetChild(i - 1).gameObject.SetActive(true);
            }
            break;
        }
    }
}
