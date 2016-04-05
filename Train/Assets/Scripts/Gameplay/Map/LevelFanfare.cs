using UnityEngine;
using System.Collections;
using Assets.Scripts.Gameplay;
using UnityEngine.UI;

public class LevelFanfare : MonoBehaviour
{
    private GameObject mainGame;
    private GameManager gameManager;
    private GameObject fanfareCanvas;
    private GameObject screenTransition;
    private GameObject levelNameObject;
    private GameObject levelDescriptionObject;

    private bool started;
    public bool HasFinished { get; private set; }

    // Use this for initialization
    void Start()
    {
        mainGame = GameObject.Find(Constants.GameObjects.Game);
        fanfareCanvas = GameObject.Find(Constants.GameObjects.FanfareCanvas);
        screenTransition = fanfareCanvas.transform.Find(Constants.GameObjects.ScreenTransition).gameObject;
        gameManager = mainGame.GetComponent<GameManager>();

        levelNameObject = screenTransition.transform.Find("LevelName").gameObject;
        levelDescriptionObject = screenTransition.transform.Find("LevelDescription").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.CurrentGameState == GameStates.LevelOpening && !started)
        {
            StartCoroutine(ShowFanfare());
        }
    }

    public IEnumerator ShowFanfare()
    {
        yield return new WaitForSeconds(0.5f);
        //Destroy(screenTransition);
        //yield return true;
        started = true;
        Image img = screenTransition.GetComponent<Image>();
        img.CrossFadeAlpha(0.0f, 2.0f, false);
        levelNameObject.GetComponent<TextComponent>().enabled = false;
        levelDescriptionObject.GetComponent<TextComponent>().enabled = false;
        this.HasFinished = true;

        started = false;
        yield return true;
    }
}
