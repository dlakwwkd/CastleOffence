using UnityEngine;

public class DragDropItem : UIDragDropItem
{
	public GameObject   prefab  = null;
    public float        xSize   = 1.0f;
    public float        ySize   = 1.0f;
    public int          amount  = 1;

    GameObject  _obj    = null;
    UILabel     _label  = null;

    protected override void Start()
    {
        base.Start();
        _label = GetComponentInChildren<UILabel>();
        _label.text = amount.ToString();
    }

    protected override void OnDragDropStart()
    {
        if (amount < 1) return;

        if (mDragScrollView != null)
            mDragScrollView.enabled = false;

        mRoot = NGUITools.FindInParents<UIRoot>(mTrans.parent);

        _obj = ObjectManager.instance.Assign(prefab.name);
        _obj.transform.localScale = new Vector3(xSize, ySize, 1.0f);
        _obj.GetComponent<Rigidbody2D>().simulated = false;
    }

    protected override void OnDragDropMove(Vector2 delta)
    {
        if (amount < 1) return;

        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0.0f;
        _obj.transform.position = pos;
    }

    protected override void OnDragDropRelease(GameObject surface)
	{
        if (amount < 1) return;
        --amount;

        if (mDragScrollView != null)
            StartCoroutine(EnableDragScrollView());

        _obj.GetComponent<Rigidbody2D>().simulated = true;
        _obj = null;
        _label.text = amount.ToString();
    }
}
