using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public static class StringToClass
{
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
            case "Dog":
                return typeof(Dog);
            case "Pig":
                return typeof(Pig);
            case "Sheep":
                return typeof(Sheep);
            case "Wolf":
                return typeof(Wolf);
            default:
                Debug.LogError("WRONG TYPE TO CONVERT: "+type);
                break;
        };

        return null;
    }
}
