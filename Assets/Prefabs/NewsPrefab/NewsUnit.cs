using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NewsUnit", menuName = "News Unit")]
public class NewsUnit : ScriptableObject
{
    public string title;
    [TextArea]
    public string text;
}
