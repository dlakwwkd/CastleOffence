using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour
{
    //-----------------------------------------------------------------------------------
    // inspector field
    public bool     IsLocked    = false;
    public float    EaseValue   = 3.0f;

    public float    MinSize     = 3.0f;
    public float    MaxSize     = 8.0f;

    public float    LeftSide    = -25.0f;
    public float    RightSide   = +25.0f;
    public float    TopSide     = +14.0f;
    public float    BottomSide  = -2.0f;

    //-----------------------------------------------------------------------------------
    // handler functions
    void Start()
    {
        camera = GetComponent<Camera>();
    }

    void Update()
    {
        if (IsLocked)
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

    //-----------------------------------------------------------------------------------
    // public functions
    public void Lock()
    {
        IsLocked = true;
        deltaPos = Vector3.zero;
    }

    public void UnLock()
    {
        IsLocked = false;
    }

    public void Shake(float shakeTime, float shakeSense)
    {
        StartCoroutine(CameraShakeProcess(shakeTime, shakeSense));
    }

    //-----------------------------------------------------------------------------------
    // private functions
#if UNITY_EDITOR
    void MoveInEditor()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            deltaPos = Vector3.zero;
            prevPos = Input.mousePosition;
        }
        if (Input.GetButton("Fire1"))
        {
            deltaPos = (Input.mousePosition - new Vector3(prevPos.x, prevPos.y)) * (camera.orthographicSize * 2 / camera.pixelHeight);
            transform.position -= deltaPos;
            prevPos = Input.mousePosition;
            MoveBoundaryCheck();
        }
        if (Input.GetButtonUp("Fire1"))
        {
            StartCoroutine("SmoothMove", deltaPos);
            deltaPos = Vector3.zero;
        }
    }

    void ZoomInEditor()
    {
        float value = 20.0f;
        if (Input.GetAxis("Mouse ScrollWheel") < 0.0f)
        {
            camera.orthographicSize += Time.deltaTime * value;
            ZoomBoundaryCheck();
            MoveBoundaryCheck();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0.0f)
        {
            camera.orthographicSize -= Time.deltaTime * value;
            ZoomBoundaryCheck();
            MoveBoundaryCheck();
        }
    }
#endif

    void Move()
    {
        Touch touch = Input.GetTouch(0);
        switch (touch.phase)
        {
            case TouchPhase.Began:
                deltaPos = Vector3.zero;
                prevPos = touch.position;
                break;
            case TouchPhase.Moved:
                deltaPos = (touch.position - prevPos) * (camera.orthographicSize * 2 / camera.pixelHeight);
                transform.position -= deltaPos;
                prevPos = touch.position;
                break;
            case TouchPhase.Ended:
                StartCoroutine("SmoothMove", deltaPos);
                deltaPos = Vector3.zero;
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
            camera.orthographicSize -= dist;
            ZoomBoundaryCheck();
        }
        if (touch1.phase == TouchPhase.Ended)
            prevPos = touch2.position;
        else if (touch2.phase == TouchPhase.Ended)
            prevPos = touch1.position;
    }

    void MoveBoundaryCheck()
    {
        float left = transform.position.x - (camera.orthographicSize * camera.aspect);
        float right = transform.position.x + (camera.orthographicSize * camera.aspect);
        float top = transform.position.y + (camera.orthographicSize);
        float bottom = transform.position.y - (camera.orthographicSize);

        if (left < LeftSide)
            transform.position += new Vector3(LeftSide - left, 0.0f, 0.0f);
        else if (right > RightSide)
            transform.position += new Vector3(RightSide - right, 0.0f, 0.0f);
        if (top > TopSide)
            transform.position += new Vector3(0.0f, TopSide - top, 0.0f);
        else if (bottom < BottomSide)
            transform.position += new Vector3(0.0f, BottomSide - bottom, 0.0f);
    }

    void ZoomBoundaryCheck()
    {
        if (camera.orthographicSize < MinSize)
            camera.orthographicSize = MinSize;
        else if (camera.orthographicSize > MaxSize)
            camera.orthographicSize = MaxSize;
    }

    //-----------------------------------------------------------------------------------
    // coroutine functions
    IEnumerator SmoothMove(Vector3 force)
    {
        Vector3 deltaPos = force * (EaseValue * 10);

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
            basePos -= deltaPos;

            transform.localPosition = basePos;
            var pos = Vector3.zero;
            pos.x = Random.Range(-shakeSense, shakeSense);
            pos.y = Random.Range(-shakeSense, shakeSense);
            transform.localPosition += new Vector3(pos.x * shakeTime, pos.y * shakeTime);
            MoveBoundaryCheck();

            yield return new WaitForEndOfFrame();
        }
    }

    //-----------------------------------------------------------------------------------
    // private field
    Vector3     deltaPos    = new Vector3();
    Vector2     prevPos     = new Vector2();
    new Camera  camera      = null;
}
