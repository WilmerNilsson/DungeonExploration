using UnityEngine;

[System.Serializable]
public class BookText
{
    public string Name;
    [Multiline]
    public string Text;
    public bool IsInJournal;
    //public enum[] areasWhereItCanBeFound;
}
