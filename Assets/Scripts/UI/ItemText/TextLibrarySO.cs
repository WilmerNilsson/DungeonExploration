using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "TextLibrarySO", menuName = "Scriptable Objects/TextLibrarySO")]
public class TextLibrarySO : ScriptableObject
{
#if UNITY_EDITOR
    [SerializeField] private bool ImportTextButton;
#endif

    [SerializeField] private List<BookText> texts = new();

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(texts == null) texts = new List<BookText>();

        if(ImportTextButton)
        {
            ImportTextButton = false;
            if (!Directory.Exists(Application.dataPath + "/ScriptableObjects/TextImportFolder"))
            {
                Directory.CreateDirectory(Application.dataPath + "/ScriptableObjects/TextImportFolder");
            }

            string[] textFilePaths = Directory.GetFiles(Application.dataPath + "/ScriptableObjects/TextImportFolder/");

            foreach (string path in textFilePaths)
            {
                if (path.EndsWith(".meta")) continue;
                if (!path.EndsWith(".txt")) Debug.LogWarning("import text file is not .txt, if you want more formats talk to Jonatan", this);

                int nameStartIndex = path.LastIndexOf('/')+1;
                int nameLength = path.LastIndexOf(".txt") - nameStartIndex;
                string name = path.Substring(nameStartIndex, nameLength);
                
                if(texts.Find(x => x.Name == name) != null)
                {
                    Debug.LogWarning($"text with name \"{name}\" already exist, delete or rename entry for it to import", this);
                    continue;
                }

                string text = File.ReadAllText(path);

                BookText bookText = new BookText();
                bookText.Name = name;
                bookText.Text = text;

                texts.Add(bookText);
            }
        }
    }
#endif

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    public bool TryGetTextByName(string name, [NotNullWhen(true)] out BookText? text)
    {
        text = texts.Find(x => x.Name == name);
        return text != null;
    }

    public bool TryGetRandomUnfoundText([NotNullWhen(true)] out BookText? text)
    {
        List<BookText> found = texts.FindAll(x => x.IsInJournal == false);

        if(found.Count == 0)
        {
            text = null;
            return false;
        }

        int randomIndex = Random.Range(0, found.Count);
        text = found[randomIndex];

        return true;
    }
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
}
