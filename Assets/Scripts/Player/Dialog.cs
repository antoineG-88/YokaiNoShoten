using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialog", menuName = "Yokai/new dialog", order = 0)]
public class Dialog : ScriptableObject
{
    [Header("Optionnal")]
    public string tsvDialogPath;
    [Space]
    [Header("Dialog sentences")]
    public List<Sentence> sentences;

    [System.Serializable]
    public class Sentence
    {
        [TextArea] public string sentence;
        public string characterName;
        public DialogManager.SeikiEmote seikiReaction;
        public float speakingSpeed;
    }
}
