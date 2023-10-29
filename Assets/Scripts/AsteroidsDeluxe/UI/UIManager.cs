using UnityEngine;

namespace AsteroidsDeluxe
{
    public class UIManager : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private MainMenu _mainMenu;
        [SerializeField] private GameHUD _gameHUD;

        private void Start()
        {
            Dispatch.Listen<GameStateMessage>(OnGameStateChanged);
        }

        private void OnDestroy()
        {
            Dispatch.Unlisten<GameStateMessage>(OnGameStateChanged);
        }

        private void OnGameStateChanged(GameStateMessage message)
        {
            switch(message.prevState)
            {
                case GameManager.GameState.MainMenu:
                    //Close Main Menu
                    _mainMenu.Close();
                    break;
                case GameManager.GameState.Gameplay:
                    _gameHUD.Close();
                    break;
                case GameManager.GameState.Leaderboard:
                    break;
                default:
                    break;
            }

            switch(message.state)
            {
                case GameManager.GameState.MainMenu:
                    _mainMenu.Open();
                    break;
                case GameManager.GameState.Gameplay:
                    _gameHUD.Open();
                    break;
                case GameManager.GameState.Leaderboard:
                    break;
            }
        }
    }
}
