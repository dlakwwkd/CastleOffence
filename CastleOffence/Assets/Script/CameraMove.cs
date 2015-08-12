using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour
{
    Vector2 _prevPos    = new Vector2();
    Camera  _camera     = null;

    public float minSize = 2.0f;
    public float maxSize = 8.0f;

    public float leftSide   = -25.0f;
    public float rightSide  = 25.0f;
    public float topSide    = 14.0f;
    public float bottomSide = -2.0f;

    void Start ()
    {
        _camera = GetComponent<Camera>();
    }

	void Update ()
    {
        if (Input.touchCount == 0)
            return;
        else if(Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            switch(touch.phase)
            {
                case TouchPhase.Began:
                    _prevPos = Vector2.zero;
                    break;
                case TouchPhase.Moved:
                    _prevPos = touch.deltaPosition;
                    Vector3 deltaPos = _prevPos * Time.deltaTime;
                    transform.position -= deltaPos * (_camera.orthographicSize / 3);
                    break;
                case TouchPhase.Ended:
                    StartCoroutine("SmoothMove", _prevPos);
                    break;
            }
        }
        else if(Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            if(touch1.phase == TouchPhase.Moved ||
               touch2.phase == TouchPhase.Moved )
            {
                //Vector2 prePos = (touch2.position - touch2.deltaPosition) - (touch1.position - touch1.deltaPosition);
                Vector2 nowPos = touch2.position - touch1.position;
                Vector2 prePos = nowPos - (touch2.deltaPosition - touch1.deltaPosition);

                float dist = (nowPos.magnitude - prePos.magnitude) * Time.deltaTime;
                _camera.orthographicSize -= dist;
                ZoomBoundary();
            }
        }
        MoveBoundary();
	}

    void ZoomBoundary()
    {
        if (_camera.orthographicSize < minSize)
            _camera.orthographicSize = minSize;
        else if (_camera.orthographicSize > maxSize)
            _camera.orthographicSize = maxSize;
    }

    void MoveBoundary()
    {
        float left = transform.position.x - (_camera.orthographicSize * _camera.aspect);
        float right = transform.position.x + (_camera.orthographicSize * _camera.aspect);
        float top = transform.position.y + (_camera.orthographicSize);
        float bottom = transform.position.y - (_camera.orthographicSize);

        if (left < leftSide)
            transform.position += new Vector3(leftSide - left, 0.0f, 0.0f);
        else if (right > rightSide)
            transform.position += new Vector3(rightSide - right, 0.0f, 0.0f);
        if (top > topSide)
            transform.position += new Vector3(0.0f, topSide - top, 0.0f);
        else if (bottom < bottomSide)
            transform.position += new Vector3(0.0f, bottomSide - bottom, 0.0f);
    }

    IEnumerator SmoothMove(Vector2 pos)
    {
        Vector3 deltaPos = pos * (_camera.orthographicSize / 3);

        while(deltaPos.magnitude > 0.001f)
        {
            transform.position -= deltaPos * Time.deltaTime;
            MoveBoundary();

            deltaPos *= 0.8f;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}
