using UnityEngine;

public class DragDropItem : UIDragDropItem
{
	public GameObject   prefab  = null;
    public float        xSize   = 1.0f;
    public float        ySize   = 1.0f;
    public int          amount  = 1;

    GameObject  _obj    = null;
    UILabel     _label  = null;

    public void             Purchase()
    {
        _label.text = (++amount).ToString();
    }

    protected override void Start()
    {
        base.Start();
        mRoot = NGUITools.FindInParents<UIRoot>(mTrans.parent);
		mGrid = NGUITools.FindInParents<UIGrid>(mTrans.parent);
		if (mGrid != null) mGrid.repositionNow = true;

        _label = GetComponentInChildren<UILabel>();
        _label.text = amount.ToString();
        _label.depth = 2;
    }

    protected override void OnDragDropStart()
    {
        if (amount < 1) return;

        if (mDragScrollView != null)
            mDragScrollView.enabled = false;

        _obj = ObjectManager.instance.Assign(prefab.name);
        _obj.transform.localScale = new Vector3(xSize, ySize, 1.0f);
        _obj.GetComponent<Rigidbody2D>().simulated = false;
    }
    protected override void OnDragDropMove(Vector2 delta)
    {
        if (amount < 1) return;

        Vector3 pos;
#if UNITY_EDITOR
        pos = Input.mousePosition;
#elif UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8
        pos = Input.GetTouch(0).position;
#endif
        pos = Camera.main.ScreenToWorldPoint(pos);
        pos.z = 0.0f;
        _obj.transform.position = pos;
    }
    protected override void OnDragDropRelease(GameObject surface)
	{
        if (amount < 1) return;

        if (mDragScrollView != null)
            StartCoroutine(EnableDragScrollView());

        if (NGUITools.FindInParents<UIDragDropContainer>(surface))
            ObjectManager.instance.Free(_obj);
        else
        {
            _label.text = (--amount).ToString();
            _obj.GetComponent<Rigidbody2D>().simulated = true;
        }
        _obj = null;
    }
}
