using UnityEngine;
using DebugUtils;

public class GlobalDebugDrawerTest2 : MonoBehaviour
{
	void OnEnable()
	{
        GlobalDebugDrawer.onDebugGUI += ShowDebug;
	}

	void OnDisable()
	{
        GlobalDebugDrawer.onDebugGUI -= ShowDebug;
	}

    void ShowDebug(IDebugDrawer dbg)
    {
        dbg.ShowInfo(
            "@@Panel #2",
            	"##My values #2"
        );
    }
}
