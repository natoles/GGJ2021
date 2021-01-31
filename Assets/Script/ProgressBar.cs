using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    // PUBLIC
    // Unity UI References
    public Slider slider;
    
    // Create a property to handle the slider's value
    private float currentValue = 0f;
    public float CurrentValue {
        get {
            return currentValue;
        }
        set {
            currentValue = value;
            slider.value = currentValue;
        }
    }

    // Use this for initialization
    void Start () {
        CurrentValue = 0f;
        if (slider == null)
            slider = gameObject.GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update () {
        
    }
}
