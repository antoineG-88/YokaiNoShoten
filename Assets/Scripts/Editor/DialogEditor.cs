using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

[CustomEditor(typeof(Dialog))]
public class DialogEditor : Editor
{
    private Dialog dialog;

    public override void OnInspectorGUI()
    {
        dialog = (Dialog)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Load dialog from tsv"))
        {
            SetDialogFromTSV();
        }
    }

    private void SetDialogFromTSV()
    {
        StreamReader sr = new StreamReader(dialog.tsvDialogPath);
        if(File.Exists(dialog.tsvDialogPath))
        {
            List<string[]> lines = new List<string[]>();
            while (!sr.EndOfStream)
            {
                string[] Line = sr.ReadLine().Split('\t');
                lines.Add(Line);
            }

            string[][] dialogInfo = lines.ToArray();

            dialog.sentences = new List<Dialog.Sentence>();
            for (int i = 0; i < dialogInfo.Length; i++)
            {
                Dialog.Sentence newSentence = new Dialog.Sentence();
                newSentence.characterName = dialogInfo[i][0];
                newSentence.sentence = dialogInfo[i][1];
                newSentence.seikiReaction = (DialogManager.SeikiEmote)Enum.Parse(typeof(DialogManager.SeikiEmote), dialogInfo[i][2]);
                newSentence.speakingSpeed = float.Parse(dialogInfo[i][3]);
                dialog.sentences.Add(newSentence);
            }
        }
        else
        {
            Debug.LogWarning("Invalid file path to load tsv file");
        }
    }
}
