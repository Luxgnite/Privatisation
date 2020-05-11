using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueMessage
{
    public string displayName;
    public string text;
    public Sprite sprite;

    public DialogueMessage(string pName, string pText, Sprite pSprite)
    {
        displayName = pName;
        text = pText;
        sprite = pSprite;
    }
}
