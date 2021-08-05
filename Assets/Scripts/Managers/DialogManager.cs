using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public float baseSpeakingSpeed;
    public float baseSpeakPauseTime;
    public float minTimeToPass;
    [Header("References")]
    public Text dialogText;
    public Image seikiFaceImage;
    public Sprite neutralFace;
    public Sprite happyFace;
    public Sprite sadFace;
    public Sprite angryFace;

    public Dialog testDialog;

    public enum SeikiEmote { Neutral, Happy, Sad, Angry }

    private bool isInDialogue;
    private Dialog currentDialog;
    private float timeElapsedOnSentence;
    private int currentDialogSentenceIndex;
    private int currentCharIndex;
    private float speakPauseTimeRemaining;
    private string sentenceProgression;
    private float charProgression;
    private bool isWaitingNext;

    public void StartDialogue(Dialog newDialog)
    {
        currentDialog = newDialog;
        currentDialogSentenceIndex = 0;
        timeElapsedOnSentence = 0;
        isInDialogue = true;
        sentenceProgression = string.Empty;
        charProgression = 0;
        speakPauseTimeRemaining = 0;
    }

    public void Update()
    {
        if (Input.GetButtonDown("XButton"))
        {
            StartDialogue(testDialog);
        }

        if (isInDialogue)
        {
            if(isWaitingNext)
            {
                dialogText.text = currentDialog.sentences[currentDialogSentenceIndex].sentence.Replace("_", string.Empty);
                //petite flêche qui vloup vloup
                if (Input.GetButtonDown("AButton"))
                {
                    isWaitingNext = false;
                    currentDialogSentenceIndex++;
                    sentenceProgression = string.Empty;
                    if (currentDialogSentenceIndex >= currentDialog.sentences.Count)
                    {
                        isInDialogue = false;
                        dialogText.text = string.Empty;
                    }
                }
            }
            else
            {
                if(speakPauseTimeRemaining > 0)
                {
                    speakPauseTimeRemaining -= Time.deltaTime;
                }
                else
                {
                    if (Input.GetButtonDown("AButton"))
                    {
                        isWaitingNext = true;
                        currentCharIndex = 0;
                    }
                    charProgression += currentDialog.sentences[currentDialogSentenceIndex].speakingSpeed * baseSpeakingSpeed * Time.deltaTime;

                    while (charProgression >= 1 && currentCharIndex < currentDialog.sentences[currentDialogSentenceIndex].sentence.Length && speakPauseTimeRemaining <= 0)
                    {
                        if (currentDialog.sentences[currentDialogSentenceIndex].sentence[currentCharIndex] == '_')
                        {
                            speakPauseTimeRemaining = baseSpeakPauseTime * currentDialog.sentences[currentDialogSentenceIndex].speakingSpeed;
                        }
                        else
                        {
                            sentenceProgression += currentDialog.sentences[currentDialogSentenceIndex].sentence[currentCharIndex];
                        }
                        currentCharIndex++;
                        charProgression--;
                    }

                    if (currentCharIndex >= currentDialog.sentences[currentDialogSentenceIndex].sentence.Length)
                    {
                        currentCharIndex = 0;
                        isWaitingNext = true;
                    }

                    dialogText.text = sentenceProgression;
                }
            }

        }
    }
}
