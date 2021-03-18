using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardParser : MonoBehaviour
{
    void Awake()
    {
        TextAsset cardsCSV = Resources.Load<TextAsset>("Cards");
        List<string> lines = new List<string>(cardsCSV.text.Split('\n'));
        foreach(string line in lines)
        {
            Debug.Log("===========================");
            foreach(string word in ParseLine(line))
            {
                Debug.Log(word);
            }
        }
    }

    private List<string> ParseLine(string line)
    {
        List<string> returnMe = new List<string>();

        returnMe.Add("");
        int currIndex = 0;
        bool betweenQuotes = false;
        foreach(char currChar in line)
        {
            if(currChar == '"')
            {
                betweenQuotes = !betweenQuotes;
            }
            else if(currChar == ',')
            {
                if(!betweenQuotes)
                {
                    returnMe.Add("");
                    currIndex++;
                }
                else
                {
                    returnMe[currIndex] += currChar;
                }
            }
            else
            {
                returnMe[currIndex] += currChar;
            }
        }

        return returnMe;
    }

    private Card CreateCardFromLine(List<string> lineFields)
    {
        //take that list of strings and turn it into a Card struct
        return new Card();
    }
}
