using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public float baseSpeakingSpeed;
    public float baseSpeakPauseTime;
    public float minTimeToPass;
    public bool manualSeikiReaction;
    public Sound nextSound;
    [Header("References")]
    public Text dialogText;
    public Image seikiFaceImage;
    public Image dialogBox;
    public GameObject dialogPanel;
    public Text nameText;
    public Image characterFace;
    public Image nextImage;
    public Animator dialogBoxAnimator;
    public Animator seikiFaceAnimator;
    public Animator characterFaceAnimator;
    [Space]
    public Sprite neutralFace;
    public Sprite happyFace;
    public Sprite sadFace;
    public Sprite angryFace;
    public Sprite okFace;
    public Sprite questionFace;
    public Sprite shockedFace;
    public Sprite cryFace;
    public List<CharacterFace> characterFaces;
    public Sprite transparentFace;

    public Dialog testDialog;

    public enum SeikiEmote { Neutral, Happy, Sad, Angry, Ok, Question, Shocked, Cry}

    private bool isInDialogue;
    private Dialog currentDialog;
    private float timeElapsedOnSentence;
    private int currentDialogSentenceIndex;
    private int currentCharIndex;
    private float speakPauseTimeRemaining;
    private string sentenceProgression;
    private float charProgression;
    private bool isWaitingNext;
    private bool seikiReacting;
    private EndDialCallback endDialCallback;

    private void Start()
    {
        if(dialogPanel != null)
        {
            dialogText.text = string.Empty;
            dialogPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Les élements d'UI de dialogue ne sont pas référencé dans le DialogManager, ils se trouve dans le canvas");
        }
    }

    public delegate void EndDialCallback();

    public void StartDialogue(Dialog newDialog, EndDialCallback endDial)
    {
        if(dialogPanel != null)
        {
            if (!isInDialogue)
            {
                currentDialog = newDialog;
                currentDialogSentenceIndex = 0;
                timeElapsedOnSentence = 0;
                characterFace.sprite = transparentFace;

                sentenceProgression = string.Empty;
                charProgression = 0;
                speakPauseTimeRemaining = 0;
                dialogPanel.SetActive(true);
                dialogBoxAnimator.SetBool("IsActive", true);
                seikiFaceAnimator.SetBool("IsActive", true);
                characterFaceAnimator.SetBool("IsActive", true);
                seikiFaceImage.sprite = neutralFace;
                nameText.text = string.Empty;
                endDialCallback = endDial;
                nextImage.gameObject.SetActive(false);
                StartCoroutine(StartDialogDelay());
            }
        }
        else
        {
            Debug.LogWarning("Ne peux pas lancé le dialogue car les éléments d'ui de dialogue ne sont pas référencés. Ils se trouvent dans le canvas");
        }
    }


    private IEnumerator StartDialogDelay()
    {
        yield return new WaitForSeconds(0.4f);
        isInDialogue = true;
    }

    public void CloseDialogue()
    {
        currentDialog = null;
        isInDialogue = false;
        dialogBoxAnimator.SetBool("IsActive", false);
        seikiFaceAnimator.SetBool("IsActive", false);
        characterFaceAnimator.SetBool("IsActive", false);
        StartCoroutine(CloseDialogDelay());
    }

    private IEnumerator CloseDialogDelay()
    {
        yield return new WaitForSeconds(0.4f);
        dialogText.text = string.Empty;
        dialogPanel.SetActive(false);
        endDialCallback();
        endDialCallback = null;
    }

    public void Update()
    {
        if (isInDialogue)
        {
            if(isWaitingNext)
            {
                timeElapsedOnSentence = 0;
                if(seikiReacting)
                {
                    nextImage.gameObject.SetActive(true);
                    if (GameManager.isUsingController ? Input.GetButtonDown("AButton") : (Input.GetButtonDown("Dash") || Input.GetMouseButtonDown(0)))
                    {
                        GameData.playerSource.PlayOneShot(nextSound.clip, nextSound.volumeScale);
                        currentDialogSentenceIndex++;
                        isWaitingNext = false;
                        sentenceProgression = string.Empty;
                        seikiReacting = false;
                        if (currentDialogSentenceIndex >= currentDialog.sentences.Count)
                        {
                            CloseDialogue();
                        }
                    }
                }
                else
                {
                    dialogText.text = currentDialog.sentences[currentDialogSentenceIndex].sentence.Replace("_", string.Empty);
                    //petite flêche qui vloup vloup
                    if ((GameManager.isUsingController ? Input.GetButtonDown("AButton") : Input.GetButtonDown("Dash") || Input.GetMouseButtonDown(0)) || !manualSeikiReaction)
                    {
                        isWaitingNext = false;
                        seikiReacting = true;
                    }
                }
            }
            else
            {
                nextImage.gameObject.SetActive(false);
                if (seikiReacting)
                {
                    if (seikiFaceImage.sprite != GetFaceFromReaction(currentDialog.sentences[currentDialogSentenceIndex].seikiReaction))
                    {
                        if(GetFaceFromReaction(currentDialog.sentences[currentDialogSentenceIndex].seikiReaction) == shockedFace)
                        {
                            seikiFaceAnimator.SetTrigger("Shocked");
                        }
                        else
                        {
                            seikiFaceAnimator.SetTrigger("Fade");
                        }
                    }

                    seikiFaceImage.sprite = GetFaceFromReaction(currentDialog.sentences[currentDialogSentenceIndex].seikiReaction);

                    timeElapsedOnSentence += Time.deltaTime;
                    if (timeElapsedOnSentence > minTimeToPass)
                    {
                        isWaitingNext = true;
                        currentCharIndex = 0;
                    }
                }
                else
                {
                    timeElapsedOnSentence += Time.deltaTime;
                    nameText.text = currentDialog.sentences[currentDialogSentenceIndex].characterName;
                    if(characterFace.sprite != GetCharacterFaceFromName(currentDialog.sentences[currentDialogSentenceIndex].characterName))
                    {
                        characterFaceAnimator.SetTrigger("ChangeCharacter");
                    }
                    characterFace.sprite = GetCharacterFaceFromName(currentDialog.sentences[currentDialogSentenceIndex].characterName);

                    if ((GameManager.isUsingController ? Input.GetButtonDown("AButton") : (Input.GetButtonDown("Dash") || Input.GetMouseButtonDown(0))) && timeElapsedOnSentence > minTimeToPass)
                    {
                        isWaitingNext = true;
                        currentCharIndex = 0;
                    }

                    if (speakPauseTimeRemaining > 0)
                    {
                        speakPauseTimeRemaining -= Time.deltaTime;
                    }
                    else
                    {
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

    private Sprite GetFaceFromReaction(SeikiEmote seikiEmote)
    {
        Sprite faceSprite = null;
        switch(seikiEmote)
        {
            case SeikiEmote.Neutral:
                faceSprite = neutralFace;
                break;

            case SeikiEmote.Angry:
                faceSprite = angryFace;
                break;

            case SeikiEmote.Happy:
                faceSprite = happyFace;
                break;

            case SeikiEmote.Sad:
                faceSprite = sadFace;
                break;

            case SeikiEmote.Ok:
                faceSprite = okFace;
                break;

            case SeikiEmote.Shocked:
                faceSprite = shockedFace;
                break;

            case SeikiEmote.Question:
                faceSprite = questionFace;
                break;

            case SeikiEmote.Cry:
                faceSprite = cryFace;
                break;

            default:
                break;
        }

        return faceSprite;
    }

    [System.Serializable]
    public class CharacterFace
    {
        public string characterName;
        public Sprite face;
    }

    private Sprite GetCharacterFaceFromName(string characeterName)
    {
        Sprite face = null;
        for (int i = 0; i < characterFaces.Count; i++)
        {
            if(characterFaces[i].characterName == characeterName)
            {
                face = characterFaces[i].face;
            }
        }
        return face;
    }
}
