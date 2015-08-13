using UnityEngine;

public class DragDropItem : UIDragDropItem
{
	public GameObject prefab;

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
	}
}
