using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Buyer", menuName = "Buyer")]
public class BuyerSettings : ScriptableObject
{
    public string name;
    public Sprite sprite;

    public PublicServicesStats publicService1;
    [TextArea]
    public List<string> sentencesPublicService1;
    [Range(0.5f, 2f)]
    public List<float> percentagesSencences1;

    public PublicServicesStats publicService2;
    [TextArea]
    public List<string> sentencesPublicService2;
    [Range(0.5f, 2f)]
    public List<float> percentagesSencences2;
}
