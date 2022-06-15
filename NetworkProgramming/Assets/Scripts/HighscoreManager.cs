using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

namespace NetworkProgramming
{
    public class HighscoreManager : MonoBehaviour
    {
        public static HighscoreManager Instance;

        public string Username = "";
        public int highscore = 0;
        bool isWorking = false;
        string errorMessage = "";

        string rootURL = "https://studenthome.hku.nl/~reinier.maartense/"; //Path where php files are located

        [SerializeField] private Transform highscorePanel;
        [SerializeField] private GameObject highscorePrefab;


        private void Awake()
        {
            Instance = this;
            //StartCoroutine(HighscoreEnumerator());

        }

        public void UpdateHighscore()
        {
            highscore = (int)GameManager.Instance.matchLength;
            StartCoroutine(HighscoreEnumerator());
            StartCoroutine(GetHighscoreEnumerator());
        }

        IEnumerator HighscoreEnumerator()
        {
            isWorking = true;
            errorMessage = "";

            WWWForm form = new WWWForm();
            form.AddField("username", Username);
            form.AddField("highscore", highscore);


            using (UnityWebRequest www = UnityWebRequest.Post(rootURL + "submitHighscore.php", form))
            {
                yield return www.SendWebRequest();
                
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                    Debug.Log("result failed");
                    errorMessage = www.error;
                }
                else
                {
                    string responseText = www.downloadHandler.text;
                    Debug.Log("result success");
                    Debug.Log(responseText);


                    if (responseText.StartsWith("Success"))
                    {
                        Debug.Log("Highscore updated");
                    }
                    else
                    {
                        errorMessage = responseText;
                    }
                }
            }

            isWorking = false;
        }

        IEnumerator GetHighscoreEnumerator()
        {
            isWorking = true;
            errorMessage = "";

            WWWForm form = new WWWForm();

            using (UnityWebRequest www = UnityWebRequest.Get(rootURL + "getHighscore.php"))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                    Debug.Log("result failed");
                    errorMessage = www.error;
                }
                else
                {
                    string responseText = www.downloadHandler.text;
                   
                    Debug.Log("result success");
                    Debug.Log(responseText);

                    HighscoreJSONWrapper highscoreWrapper = JsonUtility.FromJson<HighscoreJSONWrapper>("{\"highscores\":"+ responseText + "}");

                    Debug.Log(highscoreWrapper.highscores.Length);
                    foreach(Highscore hs in highscoreWrapper.highscores)
                    {
                        GameObject hsPrefab = Instantiate(highscorePrefab, highscorePanel);
                        hsPrefab.GetComponent<TextMeshProUGUI>().text = hs.username+ " " + hs.highscore;
                    }
                    highscorePanel.transform.parent.gameObject.SetActive(true);
                    if (responseText.StartsWith("Success"))
                    {
                        Debug.Log("Highscore updated");
                    }
                    else
                    {
                        errorMessage = responseText;
                    }
                }
            }

            isWorking = false;
        }
    }

    [System.Serializable]
    public class Highscore
    {
        public string username;
        public int highscore;
    }

    [System.Serializable]
    public class HighscoreJSONWrapper
    {
        public Highscore[] highscores;
    }
}
