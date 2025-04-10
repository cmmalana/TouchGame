using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameScript : MonoBehaviour
{
    [Header("Game Configurations")]
    public SpawnerScript spawnerScript;
    public GameObject Wall;
    public TMP_Text GameTimerText;
    public Canvas TryAgainCanvas;
    public Canvas Congrats;
    public RawImage VidPlayerPlaceHolder;

    Animator WallAnimator;
    TextMeshProUGUI[] CongratsTexts;
    ScoreManagerScript scoreManagerScript;
    int GameTime = 15; // Ilang seconds yung game bago mag end

    void Start()
    {
        WallAnimator = Wall.GetComponent<Animator>();
        scoreManagerScript = FindFirstObjectByType<ScoreManagerScript>();
        Congrats.gameObject.SetActive(true);

        CongratsTexts = Congrats.GetComponentsInChildren<TextMeshProUGUI>();

        GameTimerText.text = GameTime.ToString();

        hideCongrats();        
    }

    public void hideCongrats(){
        foreach (TextMeshProUGUI tmp in CongratsTexts){
            tmp.gameObject.SetActive(false);
        }

        // ilagay sa baba yung clap vid para nagpplay na siya and nakaready na agad
        VidPlayerPlaceHolder.rectTransform.anchoredPosition = new Vector2(VidPlayerPlaceHolder.rectTransform.anchoredPosition.x, -1000f);
    }

    public void onStartButton(){
        spawnerScript.onStartGame();
        StartCoroutine(GameTimer());
        GameTimerText.text = GameTime.ToString();
        // Reset Score 
        scoreManagerScript.ScoreReset();
    }

    IEnumerator GameTimer(){
        // Add delay para makapwesto or mag ready ang user
        yield return new WaitForSeconds(2f);

        // Placeholder ng GameTime para makapagdecrease without modifying yung GameTime mismo and mag continue ang timer
        int timer = GameTime;

        for (int i = 0; i <= GameTime; i++){
            GameTimerText.text = timer.ToString();
            yield return new WaitForSeconds(1f);
            timer--;
        }

        // Game over after ng loop
        GameOver();

        // Try Again
        StartCoroutine(onGameOver());
    }

    public void GameOver(){
        // Gawing false yung isStart bool para di na magspawn or generate ng objects kapag game over
        spawnerScript.onGameEnd();

        // Lagyan ng tag na "Target" and "Bomb" yung mga prefabs para madetect and madisable yung collider 2D during game over or pag 0 na timer
        GameObject [] Targets = GameObject.FindGameObjectsWithTag("Target");
        GameObject [] Bombs = GameObject.FindGameObjectsWithTag("Bomb");

        foreach (GameObject target in Targets){
            BoxCollider2D collider = target.GetComponent<BoxCollider2D>();
            collider.enabled = false;
        }

        foreach (GameObject bomb in Bombs){
            BoxCollider2D collider = bomb.GetComponent<BoxCollider2D>();
            collider.enabled = false;
        }
    }

    IEnumerator onGameOver(){
        yield return new WaitForSeconds(1.3f);

        // show yung congrats
        foreach (TextMeshProUGUI tmp in CongratsTexts){
            tmp.gameObject.SetActive(true);
        }

        scoreManagerScript.ShowCongratsScore();

        // ilagay sa tamang pwesto yung clap vid
        VidPlayerPlaceHolder.rectTransform.anchoredPosition = new Vector2(VidPlayerPlaceHolder.rectTransform.anchoredPosition.x, 251.65f);

        // wait another sec para ipakita yung try again
        yield return new WaitForSeconds(1f);
        WallAnimator.SetTrigger("TryAgain");
    }

    public void TryAgainButton(){
        StartCoroutine(onTryAgain());
        hideCongrats();
        onStartButton();
    }

    IEnumerator onTryAgain(){
        WallAnimator.SetTrigger("Restart");
        yield return new WaitForSeconds(2f);
    }
    
}
