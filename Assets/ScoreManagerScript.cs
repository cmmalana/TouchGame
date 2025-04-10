using TMPro;
using UnityEngine;

public class ScoreManagerScript : MonoBehaviour
{
    public TMP_Text TopScore;
    public TMP_Text CongratsScore;
    int Score = 0;

    public void onScoreAdd(){
        Score++;
        ShowTopScore();
    }

    public void ShowTopScore(){
        TopScore.text = Score.ToString();
    }

    public void ShowCongratsScore(){
        CongratsScore.text = Score.ToString();
    }

    public void ScoreReset(){
        Score = 0;
        TopScore.text = Score.ToString();
    }
}
