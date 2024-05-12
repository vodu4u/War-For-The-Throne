using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace WarForTheThrone.UI
{
    public class UIMenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject _rulesGO;
        [SerializeField] private GameObject[] _rulePanels;

        public void StartGame()
        {
            SceneManager.LoadScene("Game");
        }
        public void SetRules(bool state)
        {
            _rulesGO.SetActive(state);
        }

        public void SetRulesPanel(int index)
        {
            for (int i = 0; i < _rulePanels.Length; i++)
            {
                _rulePanels[i].SetActive(index == i);
            }
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}