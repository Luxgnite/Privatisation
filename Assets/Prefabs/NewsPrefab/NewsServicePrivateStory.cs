using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New NewsServicePrivateStory", menuName = "News History")]
public class NewsServicePrivateStory : ScriptableObject
{
    public List<NewsUnit> history;
    public List<int> historyTempo;
}
