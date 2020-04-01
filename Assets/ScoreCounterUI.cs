using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCounterUI : MonoBehaviour
{

    Text score;

    // Start is called before the first frame update
    void Awake()
    {
        score = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        score.text = "SCORE: " + GameMaster.currentScore.ToString();
    }
}
