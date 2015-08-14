using UnityEngine;

public class DragDropItem : UIDragDropItem
{
	public GameObject prefab;

    GameObject _obj = null;

    protected override void OnDragDropStart()
    {
        if (!draggedItems.Contains(this))
            draggedItems.Add(this);

        // Automatically disable the scroll view
        if (mDragScrollView != null) mDragScrollView.enabled = false;

        // Disable the collider so that it doesn't intercept events
        if (mButton != null) mButton.isEnabled = false;
        else if (mCollider != null) mCollider.enabled = false;
        else if (mCollider2D != null) mCollider2D.enabled = false;

        mParent = mTrans.parent;
        mRoot = NGUITools.FindInParents<UIRoot>(mParent);
        mGrid = NGUITools.FindInParents<UIGrid>(mParent);
        mTable = NGUITools.FindInParents<UITable>(mParent);

        // Re-parent the item
        if (UIDragDropRoot.root != null)
            mTrans.parent = UIDragDropRoot.root;

        Vector3 pos = mTrans.localPosition;
        pos.z = 0f;
        mTrans.localPosition = pos;

        TweenPosition tp = GetComponent<TweenPosition>();
        if (tp != null) tp.enabled = false;

        SpringPosition sp = GetComponent<SpringPosition>();
        if (sp != null) sp.enabled = false;

        // Notify the widgets that the parent has changed
        NGUITools.MarkParentAsChanged(gameObject);

        if (mTable != null) mTable.repositionNow = true;
        if (mGrid != null) mGrid.repositionNow = true;




        _obj = ObjectManager.instance.Assign(prefab.name);
        _obj.SetActive(false);
    }

    protected override void OnDragDropMove(Vector2 delta)
    {
        mTrans.localPosition += (Vector3)delta;



        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0.0f;
        _obj.transform.position = pos;
    }



    protected override void OnDragDropRelease (GameObject surface)
	{
		if (surface != null)
		{
			DragDropSurface dds = surface.GetComponent<DragDropSurface>();

			if (dds != null)
			{
				GameObject child = NGUITools.AddChild(dds.gameObject, prefab);
				child.transform.localScale = dds.transform.localScale;

				Transform trans = child.transform;
				trans.position = UICamera.lastWorldPosition;
				
				// Destroy this icon as it's no longer needed
				NGUITools.Destroy(gameObject);
				return;
			}
		}
		base.OnDragDropRelease(surface);



        _obj.SetActive(true);
	}
}
