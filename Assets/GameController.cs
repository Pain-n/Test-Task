using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] Canvas Canvas;
    [SerializeField] Button CupPrefab;
    [SerializeField] GameObject BallPrefab;
    [SerializeField] Transform Spawn;
    [SerializeField] Button StartButton;
    [SerializeField] TextMeshProUGUI ScoreText;
    [SerializeField] GameObject LosePanel;
    List<Button> Cups = new List<Button>();
    int Score;
    Button UppedCup;
    GameObject BallInstance;

    void Start()
    {
        Score = 0;
        SpawnCup();
        SetStartButton(StartButton);
        SetCupButtons();
    }

    void SpawnCup()
    {
        bool CupWithBallIsSetted = false;
        for(int i = 0; i<900; i += 300)
        {
            int random = Random.Range(0, 10);
            Button CupInstance;
            if (random > 5 && CupWithBallIsSetted == false || i == 600 && CupWithBallIsSetted == false)
            {
                CupWithBallIsSetted = true;
                CupInstance = Instantiate(CupPrefab, new Vector3(Spawn.position.x + i, Spawn.position.y + 300, 0), Quaternion.identity, Canvas.transform);
                Cups.Add(CupInstance);
                CupInstance.tag = "WinCup";
                UppedCup = CupInstance;
                BallInstance = Instantiate(BallPrefab, new Vector3(Spawn.position.x + i, Spawn.position.y - 140, 0), Quaternion.identity, Spawn.transform);
            }
            else
            {
                CupInstance = Instantiate(CupPrefab, new Vector3(Spawn.position.x + i, Spawn.position.y, 0), Quaternion.identity, Canvas.transform);
                Cups.Add(CupInstance);
            }
        }
    }
    void SetStartButton(Button button)
    {
        button.onClick.AddListener(() =>
        {
            StartCoroutine(StartButtonLogic(button));
        });
    }

    void SetCupButtons()
    {
        foreach(Button cup in Cups)
        {
            cup.onClick.AddListener(() =>
            {
                if(cup.tag == "WinCup")
                {
                    BallInstance = Instantiate(BallPrefab, new Vector3(cup.transform.position.x, cup.transform.position.y - 140, 0), Quaternion.identity, Spawn.transform);
                    UppedCup = cup;
                    iTween.MoveTo(cup.gameObject, new Vector3(cup.transform.position.x, cup.transform.position.y + 300, 0), 2f);
                    Score += 1;
                    ScoreText.text = Score.ToString();
                    StartButton.interactable = true;
                    foreach (Button cup in Cups)
                    {
                        cup.interactable = false;
                    }
                }
                else
                {
                    LosePanel.SetActive(true);
                }
            });
        }
    }

    IEnumerator StartButtonLogic(Button button)
    {
        iTween.MoveTo(UppedCup.gameObject, new Vector3(UppedCup.transform.position.x, UppedCup.transform.position.y - 300, 0), 2f);
        button.interactable = false;
        yield return new WaitForSeconds(2);
        Destroy(BallInstance.gameObject);
        for (int i = 0; i < 6; i++)
        {
            int firstCupID = Random.Range(0, Cups.Count);
            int secondCupID = Random.Range(0, Cups.Count);
            while (secondCupID == firstCupID)
            {
                secondCupID = Random.Range(0, 2);
            }

            Vector3 fistCupPosition = Cups[firstCupID].transform.position;
            Vector3 secondCupPosition = Cups[secondCupID].transform.position;

            iTween.MoveTo(Cups[firstCupID].gameObject, secondCupPosition, 2f);
            iTween.MoveTo(Cups[secondCupID].gameObject, fistCupPosition, 2f);
            yield return new WaitForSeconds(2);
        }
        foreach(Button cup in Cups)
        {
            cup.interactable = true;
        }
    }
}
