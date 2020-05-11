using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buyer
{
    public string name;
    public Sprite sprite;

    public PublicServicesStats publicService1;
    public List<string> sentencesPublicService1;
    public List<float> percentagesSencences1;

    public PublicServicesStats publicService2;
    public List<string> sentencesPublicService2;
    public List<float> percentagesSencences2;

    public Buyer(string pName, Sprite pSprite, PublicServicesStats pPublicService1, List<string> pSentencesPublicService1, List<float> pPercentagesSencences1,
        PublicServicesStats pPublicService2, List<string> pSentencesPublicService2,
        List<float> pPercentagesSencences2)
    {
        name = pName;
        sprite = pSprite;
        publicService1 = pPublicService1;
        sentencesPublicService1 = pSentencesPublicService1;
        percentagesSencences1 = pPercentagesSencences1;
        publicService2 = pPublicService2;
        sentencesPublicService2 = pSentencesPublicService2;
        percentagesSencences2 = pPercentagesSencences2;
    }


}
