using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;

public class GameManager : MonoBehaviour
{
    public Text displayDate;
    public DateTime date;
    public int speedTime = 4;

    public int treasury;
    public List<PublicService> publicServices;

    // Start is called before the first frame update
    void Start()
    {
        date = new DateTime(2008,5,1);
        StartCoroutine(DateUpdate());

        treasury = 0;

        publicServices = new List<PublicService>();
        publicServices.Add(new PublicService("SNCF", -10000, 1000, 100, 0.4f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DateUpdate()
    {
        yield return new WaitForSecondsRealtime(speedTime);

        date = date.AddMonths(1);
        displayDate.text = date.ToString("y",new CultureInfo("fr-FR"));

        StartCoroutine(DateUpdate());
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
