using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NewsServicePrivateStory", menuName = "News History")]
public class NewsServicePrivateStory : ScriptableObject
{
    public List<NewsUnit> history;
    public List<int> historyTempo;

    public NewsServicePrivateStory(NewsServicePrivateStory copy)
    {
        history = new List<NewsUnit>(copy.history);
        historyTempo = new List<int>(copy.historyTempo);
    }
}
