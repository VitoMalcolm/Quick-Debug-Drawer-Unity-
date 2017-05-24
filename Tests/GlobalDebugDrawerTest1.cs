using UnityEngine;
using DebugUtils;

public class GlobalDebugDrawerTest1 : MonoBehaviour
{
    public bool someCondition = false;

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
            "@@Panel #1",
            	"##My values #1",

                dbg.ShowIf(someCondition,
                    "**Optional labels",
                    "##Conditional label #1",
                    "##Conditional label #2"
                )
        );
    }
}
