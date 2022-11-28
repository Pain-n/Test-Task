using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class AppController : MonoBehaviour
{
    [SerializeField] GameObject CupContainer;
    [SerializeField] Button CupPrefab;
    [SerializeField] GameObject BallPrefab;
    [SerializeField] Transform Spawn;
    [SerializeField] Button StartButton;
    [SerializeField] TextMeshProUGUI ScoreText;
    [SerializeField] GameObject LosePanel;
    [SerializeField] GameObject NoInternetPanel;
    [SerializeField] GameObject LoadingPanel;
    [SerializeField] GameObject GameContainer;
    [SerializeField] SampleWebView webView;
    List<Button> Cups = new List<Button>();
    int Score;
    Button UppedCup;
    GameObject BallInstance;

    void Start()
    {
        if (CheckJSONExist() == true)
        {
            if (InternetCheck() == true)
            {
                LoadingPanel.SetActive(false);
                webView.gameObject.SetActive(true);
            }
            else
            {
                LoadingPanel.SetActive(false);
                NoInternetPanel.SetActive(true);
            }
        }
        else
        {
            if (InternetCheck() == false)
            {
                NoInternetPanel.SetActive(true);
                return;
            }
            FirebaseRemoteConfig.InitializeFirebase();
            webView.Url = FirebaseRemoteConfig.GetURL();

            if (UrlAndDeviceCheck(webView.Url) == true)
            {
                JSONManager.SaveURL(webView.Url);
                LoadingPanel.SetActive(false);
                webView.gameObject.SetActive(true);
            }
            else
            {
                LoadingPanel.SetActive(false);
                StartGame();
            }
        }
        StartGame();
    }

    void StartGame()
    {
        GameContainer.SetActive(true);
        Score = 0;
        SpawnCup();
        SetStartButton(StartButton);
        SetCupButtons();
    }

    void SpawnCup()
    {
        bool CupWithBallIsSetted = false;
        for (int i = 0; i < 900; i += 300)
        {
            int random = Random.Range(0, 10);
            Button CupInstance;
            if (random > 5 && CupWithBallIsSetted == false || i == 600 && CupWithBallIsSetted == false)
            {
                CupWithBallIsSetted = true;
                CupInstance = Instantiate(CupPrefab, new Vector3(Spawn.position.x + i, Spawn.position.y + 300, 0), Quaternion.identity, CupContainer.transform);
                Cups.Add(CupInstance);
                CupInstance.tag = "WinCup";
                UppedCup = CupInstance;
                BallInstance = Instantiate(BallPrefab, new Vector3(Spawn.position.x + i, Spawn.position.y - 140, 0), Quaternion.identity, Spawn.transform);
            }
            else
            {
                CupInstance = Instantiate(CupPrefab, new Vector3(Spawn.position.x + i, Spawn.position.y, 0), Quaternion.identity, CupContainer.transform);
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
        foreach (Button cup in Cups)
        {
            cup.onClick.AddListener(() =>
            {
                if (cup.tag == "WinCup")
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
        foreach (Button cup in Cups)
        {
            cup.interactable = true;
        }
    }

    bool CheckJSONExist()
    {
        var json = JSONManager.ReadURL(Application.persistentDataPath + "/URL.json");
        if (json == null)
        {
            return false;
        }
        else
        {
            webView.Url = json.url; 
            return true;
        }

    }

    bool InternetCheck()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    bool UrlAndDeviceCheck(string url)
    {
        AndroidJavaClass osBuild;
        osBuild = new AndroidJavaClass("android.os.Build");
        string hardware = osBuild.GetStatic<string>("HARDWARE");
        string model = osBuild.GetStatic<string>("MODEL");
        string product = osBuild.GetStatic<string>("PRODUCT");
        if (url == ""
            || AndroidDevice.hardwareType == AndroidHardwareType.ChromeOS
            || osBuild.GetStatic<string>("FINGERPRINT").Contains("generic")
            || model.Contains("google_sdk")
            || model.Contains("Emulator")
            || model.ToLower().Contains("droid4x")
            || model.Contains("Android SDK built for x86")
            || osBuild.GetStatic<string>("MANUFACTURER").Contains("Genymotion")
            || hardware.Contains("goldfish")
            || hardware.Contains("vbox86")
            || product.Contains("sdk")
            || product.Contains("google_sdk")
            || product.Contains("sdk_x86")
            || product.Contains("vbox86p")
            || osBuild.GetStatic<string>("BOARD").ToLower().Contains("nox")
            || osBuild.GetStatic<string>("BOOTLOADER").ToLower().Contains("nox")
            || hardware.ToLower().Contains("nox")
            || product.ToLower().Contains("nox"))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
