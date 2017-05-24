using UnityEngine;

namespace DebugUtils
{
    /*
    	A singleton-wrapper for QuickDebugDrawer, can be used as shared service to other instances
    */

	public class GlobalDebugDrawer : MonoBehaviour
	{
        static GlobalDebugDrawer me;

        public Vector2 screenPosition = new Vector2(50f,50f);
        public delegate void DebugGuiEvent(IDebugDrawer dbg);
        public static event DebugGuiEvent onDebugGUI;

        QuickDebugDrawer dbg = new QuickDebugDrawer()
		{
			sx = 50,
			sy = 50
		};

		void Awake()
		{
            //singleton
            if (me == null) me = this;
            else {Destroy(this); return;}

            SetDrawPosition(screenPosition);
            dbg.Prepare();
		}

		void OnGUI()
		{
			dbg.Prepare();

            if (onDebugGUI != null)
                onDebugGUI(dbg);
		}

        public static void SetDrawPosition(Vector2 pos)
        {
            me.dbg.sx = pos.x;
            me.dbg.sy = pos.y;
        }

        /*
		public static void StartPanel(string name, float width = 200f)
		{
			me.dbg.StartPanel(name, width);
		}

		public static void Header(string header)
		{
			me.dbg.Header(header);
		}

		public static void Pair(string field, string value)
		{
			me.dbg.Pair(field, value);
		}

		public static void Label(string label)
		{
			me.dbg.Label(label);
		}

		public static void Separator()
		{
			me.dbg.Separator();
		}

		public static void ShowInfo(params object[] pack)
		{
			me.dbg.ShowInfo(pack);
		}

		public static bool isCollapsed(string header)
		{
			return me.dbg.isCollapsed(header);
		}*/
	}
}