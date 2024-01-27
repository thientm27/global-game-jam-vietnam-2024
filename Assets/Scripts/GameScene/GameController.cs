using UnityEngine;

namespace GameScene
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameView gameView;
        [SerializeField] private GameModel gameModel;
        [SerializeField] private FirstPersonController firstPersonController;

        void Start()
        {
            firstPersonController.LockCusor();
        }

        void Update()
        {
            firstPersonController.Controlling();
        }
    }
}