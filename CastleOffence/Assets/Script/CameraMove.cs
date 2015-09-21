using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour
{
    public bool     isLocked    = false;
    public float    easeValue   = 3.0f;

    public float    minSize     = 3.0f;
    public float    maxSize     = 8.0f;

    public float    leftSide    = -25.0f;
    public float    rightSide   = +25.0f;
    public float    topSide     = +14.0f;
    public float    bottomSide  = -2.0f;

    Vector3 _deltaPos   = new Vector3();
    Vector2 _prevPos    = new Vector2();
    Camera  _camera     = null;


    void Start()
    {
        _camera = GetComponent<Camera>();
    }
    void Update()
    {
        if (isLocked)
            return;
#if UNITY_EDITOR
        MoveInEditor();
        ZoomInEditor();
#elif UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8
        if (Input.touchCount == 0)
            return;
        else if(Input.touchCount == 1)
            Move();
        else if(Input.touchCount == 2)
            Zoom();
        MoveBoundaryCheck();
#endif
	}


    public void Lock() { isLocked = true; _deltaPos = Vector3.zero; }
    public void UnLock() { isLocked = false; }
    public void Shake(float shakeTime, float shakeSense)
    {
        StartCoroutine(CameraShakeProcess(shakeTime, shakeSense));
    }


    void MoveInEditor()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            _deltaPos = Vector3.zero;
            _prevPos = Input.mousePosition;
        }
        if (Input.GetButton("Fire1"))
        {
            _deltaPos = (Input.mousePosition - new Vector3(_prevPos.x, _prevPos.y)) * (_camera.orthographicSize * 2 / _camera.pixelHeight);
            transform.position -= _deltaPos;
            _prevPos = Input.mousePosition;
            MoveBoundaryCheck();
        }
        if (Input.GetButtonUp("Fire1"))
        {
            StartCoroutine("SmoothMove", _deltaPos);
            _deltaPos = Vector3.zero;
        }
    }
    void ZoomInEditor()
    {
        float value = 20.0f;
        if (Input.GetAxis("Mouse ScrollWheel") < 0.0f)
        {
            _camera.orthographicSize += Time.deltaTime * value;
            ZoomBoundaryCheck();
            MoveBoundaryCheck();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0.0f)
        {
            _camera.orthographicSize -= Time.deltaTime * value;
            ZoomBoundaryCheck();
            MoveBoundaryCheck();
        }
    }
    void Move()
    {
        Touch touch = Input.GetTouch(0);
        switch (touch.phase)
        {
            case TouchPhase.Began:
                _deltaPos = Vector3.zero;
                _prevPos = touch.position;
                break;
            case TouchPhase.Moved:
                _deltaPos = (touch.position - _prevPos) * (_camera.orthographicSize * 2 / _camera.pixelHeight);
                transform.position -= _deltaPos;
                _prevPos = touch.position;
                break;
            case TouchPhase.Ended:
                StartCoroutine("SmoothMove", _deltaPos);
                _deltaPos = Vector3.zero;
                break;
        }
    }
    void Zoom()
    {
        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);

        if (touch1.phase == TouchPhase.Moved ||
           touch2.phase == TouchPhase.Moved)
        {
            //Vector2 prePos = (touch2.position - touch2.deltaPosition) - (touch1.position - touch1.deltaPosition);
            Vector2 nowPos = touch2.position - touch1.position;
            Vector2 prePos = nowPos - (touch2.deltaPosition - touch1.deltaPosition);

            float dist = (nowPos.magnitude - prePos.magnitude) * Time.deltaTime;
            _camera.orthographicSize -= dist;
            ZoomBoundaryCheck();
        }
        if (touch1.phase == TouchPhase.Ended)
            _prevPos = touch2.position;
        else if (touch2.phase == TouchPhase.Ended)
            _prevPos = touch1.position;
    }
    void MoveBoundaryCheck()
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
    void ZoomBoundaryCheck()
    {
        if (_camera.orthographicSize < minSize)
            _camera.orthographicSize = minSize;
        else if (_camera.orthographicSize > maxSize)
            _camera.orthographicSize = maxSize;
    }


    IEnumerator SmoothMove(Vector3 force)
    {
        Vector3 deltaPos = force * (easeValue * 10);

        while(deltaPos.magnitude > 0.001f)
        {
            transform.position -= deltaPos * Time.deltaTime;
            MoveBoundaryCheck();

            deltaPos *= 0.8f;
            yield return new WaitForEndOfFrame();
        }
    }
    IEnumerator CameraShakeProcess(float shakeTime, float shakeSense)
    {
        var basePos = transform.localPosition;
        while (shakeTime > 0.0f)
        {
            shakeTime -= Time.deltaTime;
            basePos -= _deltaPos;

            transform.localPosition = basePos;
            var pos = Vector3.zero;
            pos.x = Random.Range(-shakeSense, shakeSense);
            pos.y = Random.Range(-shakeSense, shakeSense);
            transform.localPosition += new Vector3(pos.x * shakeTime, pos.y * shakeTime);
            MoveBoundaryCheck();

            yield return new WaitForEndOfFrame();
        }
    }
}
