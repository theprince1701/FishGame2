using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject helpPanel;
    [SerializeField] GameObject exitButton;
  
    public void openHelp()
    {
        helpPanel.SetActive(true);
        exitButton.SetActive(true);
    }

    public void closeHelp()
    {
        helpPanel.SetActive(false);
        exitButton.SetActive(false);
    }
}
