using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PublicService", menuName = "Public Service")]
public class PublicServicesStats : ScriptableObject
{
    public string name;
    public int cost;
    public int income;
    public int privMalus;
    public int budgetAllowed;
    public int debt;
    public int treasury;
    public float profitTendency;
    public NewsServicePrivateStory newsStory;
}
