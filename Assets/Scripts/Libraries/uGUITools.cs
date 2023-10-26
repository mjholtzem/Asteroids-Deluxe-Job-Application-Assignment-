using UnityEditor;
using UnityEngine;

//https://www.youtube.com/redirect?event=video_description&redir_token=QUFFLUhqa3lMQVlZSVNvM2hIUy05WDhkZjBFS3lyTmprUXxBQ3Jtc0ttczF3VnRTUlJrQWJYMTktNXR6UURRN1JnTmZDRDFKRDY3T3h5VFF5LUkySFh1aXhjM2dtckUwdV9qNHgtME8yXzR6cm5RQ2FJU3NTdGVFSnBUSThKUjJlcXp3eVlxX3RKMnlVeHdWSU5DUUxnRzBlSQ&q=https%3A%2F%2Fforum.unity.com%2Fthreads%2Fscripts-useful-4-6-scripts-collection.264161%2F&v=xRDy4WhKDxk
public class uGUITools : MonoBehaviour {
	[MenuItem("uGUI/Anchors to Corners %[")]
	static void AnchorsToCorners(){
		RectTransform t = Selection.activeTransform as RectTransform;
		RectTransform pt = Selection.activeTransform.parent as RectTransform;

		if(t == null || pt == null) return;
		
		Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
		                                    t.anchorMin.y + t.offsetMin.y / pt.rect.height);
		Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
		                                    t.anchorMax.y + t.offsetMax.y / pt.rect.height);

		t.anchorMin = newAnchorsMin;
		t.anchorMax = newAnchorsMax;
		t.offsetMin = t.offsetMax = new Vector2(0, 0);
	}

	[MenuItem("uGUI/Corners to Anchors %]")]
	static void CornersToAnchors(){
		RectTransform t = Selection.activeTransform as RectTransform;

		if(t == null) return;

		t.offsetMin = t.offsetMax = new Vector2(0, 0);
	}
}
