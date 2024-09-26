using UnityEngine;

namespace Game.RTS
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private GameObject menuPanel;
        void Start()
        {
            menuPanel.SetActive(true);
        }

        public void StartGame()
        {
            menuPanel.SetActive(false);
        }
    }
}
