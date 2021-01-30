using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringToClass
{
    /*
    private List<string> availableClasses = new List<string> {
        "Bull",
        "Cat",
        "Cow"
        };
    */

    // Returns the type of an animal given its name as a string
    public static System.Type TypeFromString(string type)
    {
        switch(type)
        {
            case "Bull":
                return typeof(Bull);
            case "Cat":
                return typeof(Cat);
            case "Cow":
                return typeof(Cow);
            case "Mouse":
                return typeof(Mouse);
        }

        return null;
    }
}
