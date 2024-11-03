using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    private int _fishCaught;

    public void OnFishCaught()
    {
        _fishCaught++;
        text.text = "MONSTERS CAUGHT: " + _fishCaught;
    }
}
