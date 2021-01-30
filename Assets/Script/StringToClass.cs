using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringToClass : MonoBehaviour
{
    /*
    private List<string> availableClasses = new List<string> {
        "Bull",
        "Cat",
        "Cow"
        };
    */

    // Returns the type of an animal given its name as a string
    public System.Type TypeFromString(string type)
    {
        switch(type)
        {
            case "Bull":
                return typeof(Bull);
            case "Cat":
                return typeof(Cat);
            case "Cow":
                return typeof(Cat);
        }

        return null;
    }
}
