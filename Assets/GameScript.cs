using System.Collections;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class settingsConfig{
    public string GameTimevalue;
    public int GameTimeDropDownvalue;
    public float scalevalue;
    public float particlescalevalue;
}

public class GameScript : MonoBehaviour
{
    [Header("Game Configurations")]
    public SpawnerScript spawnerScript;
    public GameObject Wall;
    public TMP_Text GameTimerText;
    public Canvas TryAgainCanvas;
    public Canvas Congrats;
    public RawImage VidPlayerPlaceHolder;
    public TMP_Dropdown GameTimeDropDown;
    public Image TargetTemplate;

    Animator WallAnimator;
    TextMeshProUGUI[] CongratsTexts;
    ScoreManagerScript scoreManagerScript;
    string CurrentDirect;
    string configfolderpath;
    string settingsconfig;
    settingsConfig settingsConfig;
    bool cursorVisible = true;
    float scale = 1f;
    int GameTime = 60; // Ilang seconds yung game bago mag end

    void Awake()
    {
        settingsConfig = new settingsConfig();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            cursorVisible = !cursorVisible; // Toggle the state

            Cursor.visible = cursorVisible;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void Start()
    {
        CurrentDirect = Directory.GetCurrentDirectory();
        WallAnimator = Wall.GetComponent<Animator>();
        scoreManagerScript = FindFirstObjectByType<ScoreManagerScript>();
        Congrats.gameObject.SetActive(true);

        CongratsTexts = Congrats.GetComponentsInChildren<TextMeshProUGUI>();

        LoadConfig();    

        GameTimerText.text = GameTime.ToString();

        hideCongrats();    
    }

    public void LoadConfig(){
        configfolderpath = Path.Combine(CurrentDirect, "Configs");

        // Gagawa ng Folder for Configs kapag wala pa
        if (!Directory.Exists(configfolderpath)){
            Directory.CreateDirectory(configfolderpath);
        }

        settingsconfig = Path.Combine(configfolderpath, "SettingsConfig.json");

        // If nag exist yung json file, iload lang yung laman
        if (File.Exists(settingsconfig)){
            string loadsettings = File.ReadAllText(settingsconfig);
            settingsConfig = JsonUtility.FromJson<settingsConfig>(loadsettings);

            GameTime = int.Parse(settingsConfig.GameTimevalue);
            GameTimeDropDown.value = settingsConfig.GameTimeDropDownvalue;
            scale = settingsConfig.scalevalue;
            spawnerScript.scale = scale;
            spawnerScript.particleScale = settingsConfig.particlescalevalue;

            TargetTemplate.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, scale);

            Debug.Log("SettingsConfig.json configs loaded. Configs: " + loadsettings);
        }
        else{   // Mag save ng default config if wala pa yung json file
            InitialSaveConfig();
        }
        
    }

    public void InitialSaveConfig(){
        settingsConfig.GameTimevalue = GameTime.ToString();
        settingsConfig.GameTimeDropDownvalue = GameTimeDropDown.value;
        settingsConfig.scalevalue = scale;
        settingsConfig.particlescalevalue = spawnerScript.particleScale;
        SaveConfig();
    }

    public void SaveConfig(){
        string savesettings = JsonUtility.ToJson(settingsConfig);
        File.WriteAllText(settingsconfig, savesettings);
        Debug.Log("Configs saved! Configs: " + savesettings);
    }

    public void hideCongrats(){
        foreach (TextMeshProUGUI tmp in CongratsTexts){
            tmp.gameObject.SetActive(false);
        }

        // ilagay sa baba yung clap vid para nagpplay na siya and nakaready na agad
        VidPlayerPlaceHolder.rectTransform.anchoredPosition = new Vector2(VidPlayerPlaceHolder.rectTransform.anchoredPosition.x, -1000f);
    }

    public void onStartButton(){

        if (spawnerScript.isStart) return;

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
        if (spawnerScript.isStart) return;

        StartCoroutine(onTryAgain());
        hideCongrats();
        onStartButton();
    }

    IEnumerator onTryAgain(){
        WallAnimator.SetTrigger("Restart");
        yield return new WaitForSeconds(2f);
    }

    public void onWrongTarget(){
        WallAnimator.SetTrigger("WrongTarget");
    }

    public void onGameTimeDropDown(){
        // 30 seconds
        if (GameTimeDropDown.value == 1){
            GameTime = 30;
        }
        else if (GameTimeDropDown.value == 2){ // 15 seconds
            GameTime = 15;
        }
        else{ // 60 seconds
            GameTime = 60;
        }

        settingsConfig.GameTimeDropDownvalue = GameTimeDropDown.value;
        settingsConfig.GameTimevalue = GameTime.ToString();
        
        SaveConfig();
    }

    // Decrease ng Target Size
    public void onMinusButton(){
        if (scale <= 0.5f) return;

        spawnerScript.TargetSizeDecrease();
        scale -= 0.5f;
        TargetTemplate.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, scale);

        settingsConfig.scalevalue = scale;
        settingsConfig.particlescalevalue = spawnerScript.particleScale;
        SaveConfig();
    }

    // Increase ng Target Size
    public void onAddButton(){
        if (scale >= 4f) return;

        spawnerScript.TargetSizeIncrease();
        scale += 0.5f;
        TargetTemplate.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, scale);

        settingsConfig.scalevalue = scale;
        settingsConfig.particlescalevalue = spawnerScript.particleScale;
        SaveConfig();
    }
    
}
