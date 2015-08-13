using UnityEngine;
using System.Collections;

public class ContainerOnOff : MonoBehaviour
{
    UIWidget    _widget = null;
    bool        _isOn   = false;

    void Start()
    {
        _widget = GetComponent<UIWidget>();
        _widget.alpha = 0.0f;
    }

    public void Click()
    {
        if(_isOn)
        {
            _widget.alpha = 0.0f;
            _isOn = false;
        }
        else
        {
            _widget.alpha = 1.0f;
            _isOn = true;
        }
    }
}
