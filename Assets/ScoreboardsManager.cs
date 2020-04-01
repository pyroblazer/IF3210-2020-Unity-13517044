using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Networking;
using System.Text;
using System.Collections;
using System.Linq;

public static class JsonHelper
{
    public static T[] getJsonArray<T>(string json)
    {
        string newJson = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}

public class ScoreboardsManager : MonoBehaviour
{
    [SerializeField]
    string hoverOverSound = "ButtonHover";

    [SerializeField]
    string pressButtonSound = "ButtonPress";

    AudioManager audioManager;

    readonly string URI = "http://134.209.97.218:5051";
    public string NIM = "13517044";
    public JsonScoreboards.Scoreboard[] Scoreboards;
    public int scoreboardOrder = 0;

    [Serializable]
    public struct JsonScoreboards
    {
        [Serializable]
        public struct Scoreboard
        {
            public string _id;
            public string nim;
            public string username;
            public int score;
        }

        public Scoreboard[] scoreboards;
    }

    void Start()
    {
        audioManager = AudioManager.instance;
        if (audioManager == null)
        {
            Debug.LogError("No audiomanager found!");
        }
        string URL = URI + "/scoreboards/" + NIM;
        UnityWebRequest unityWebRequest = new UnityWebRequest(URL);
        StartCoroutine(SimpleGetRequest(URL));
    }

    IEnumerator SimpleGetRequest(string url)
    {
        var request = new UnityWebRequest(url, "GET");
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        Debug.Log("Status Code: " + request.responseCode);
        if (request.responseCode == 200)
        {
            Debug.Log("Download Handler Data : " + request.downloadHandler.data);
            Debug.Log("Type of request data : " + request.downloadHandler.data.GetType());
            string response = Encoding.UTF8.GetString(request.downloadHandler.data);
            Debug.Log("response = " + response);

            //string wrappedResponse = "{\"responseItems\": " + response + "}";
            //Debug.Log(wrappedResponse);
            //JsonScoreboards jsonScoreboards = JsonUtility.FromJson<JsonScoreboards>(wrappedResponse);
            //Debug.Log("jsonScoreboards : " + jsonScoreboards);
           
            Scoreboards = JsonHelper.getJsonArray<JsonScoreboards.Scoreboard>(response);
            Debug.Log(Scoreboards.GetType());
            Scoreboards = Scoreboards.OrderByDescending(w => w.score).ToArray();

        }
        else
        {
            //Display Nothing
        }
    }

    void OnGUI()
    {
        int y = 60;
        int height = 20;
        int scoreboardEntries = 0;
        int tempScoreboardOrder = scoreboardOrder;
        if (Scoreboards != null && Scoreboards.Length > 0)
        {
            while (scoreboardEntries < 20)
            {
                Debug.Log(tempScoreboardOrder);
                GUI.TextField(new Rect(200, y, 300, height), "No. " + tempScoreboardOrder + " Username : " + Scoreboards[tempScoreboardOrder].username + " Score : " + Scoreboards[tempScoreboardOrder].score);
                y += height;
                tempScoreboardOrder++;
                scoreboardEntries += 1;
            }
        }
    }

    public void OnPressNext()
    {
        audioManager.PlaySound(pressButtonSound);
        if (scoreboardOrder < Scoreboards.Length-20)
        {
            scoreboardOrder += 1;
        }
    }

    public void OnPressPrevious()
    {
        audioManager.PlaySound(pressButtonSound);
        if (scoreboardOrder != 0)
        {
            scoreboardOrder -= 1;
        }
    }

    public void BackToMainMenu()
    {
        audioManager.PlaySound(pressButtonSound);

        SceneManager.LoadScene("MainMenu");
    }

    public void OnMouseOver()
    {
        audioManager.PlaySound(hoverOverSound);
    }
}
