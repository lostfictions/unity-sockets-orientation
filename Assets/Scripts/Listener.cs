using UnityEngine;
using UnityEngine.Assertions;

using UnityEngine.UI;

public class Listener : MonoBehaviour
{
    public Text text;

    WebSocketPointer pointer;

    void Start()
    {
        pointer = GetComponent<WebSocketPointer>();
        Assert.IsNotNull(pointer);
    }
    
    void Update()
    {
        text.text = pointer.Orientation.ToString();
    }
}
