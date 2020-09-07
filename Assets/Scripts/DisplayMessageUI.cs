using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayMessageUI : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private GameObject DisplayMessageGO;
    [SerializeField] private TextMeshProUGUI textMessage;
#pragma warning restore 0649

    public static DisplayMessageUI Instance = null;

    private Queue<Tuple<string, float>> messages = new Queue<Tuple<string, float>>();

    private bool IsCurrentlyDisplaying { get; set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        DisplayMessageGO.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsCurrentlyDisplaying && messages.Count > 0)
        {
            Tuple<string, float> nextMessage = messages.Dequeue();
            StartCoroutine(ShowMessage(nextMessage.Item1, nextMessage.Item2));
        }
    }

    public void DisplayMessage(string message, float time = 2.25f)
    {
        if (messages.Count > 0 && messages.Peek().Item1.Equals(message))
        {
            float timeLeft = messages.Dequeue().Item2;
            Tuple<string, float> nextMessage = new Tuple<string, float>(message, timeLeft += 0.5f);
            messages.Enqueue(nextMessage);
        }
        else
        {
            messages.Enqueue(new Tuple<string, float>(message, time));
        }
        
    }

    private IEnumerator ShowMessage(string message, float time)
    {
        IsCurrentlyDisplaying = true;

        textMessage.text = message;
        DisplayMessageGO.SetActive(true);

        for(float t = 0; t < time; t+= Time.deltaTime)
        {
            yield return null;
        }

        DisplayMessageGO.SetActive(false);
        yield return new WaitForSeconds(0.33f);
        IsCurrentlyDisplaying = false;

    }
}
