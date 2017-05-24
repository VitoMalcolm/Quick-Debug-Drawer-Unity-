using System.Collections.Generic;
using UnityEngine;



namespace DebugUtils
{
    /*
	  This is just a quick formatter and drawer for your debug info. Uses IMGUI, so all calls
	  must be performed during OnGUI event. It also allows you to hide/show panels and headers
	  by clicking on them.

	  Usage info: http://vitomalcolm.com/unity-quickdebugdrawer-snippet
	  Author: Vito Malcolm
	  License: MIT
	*/

    /* PackedInfo usage snippets:

        @@ - StartPanel
        ** - StartHeader
        ## - Label
        "---" - Separator
    */

    public interface IDebugDrawer
    {
        //must be called in OnGUI before any other calls
        void Prepare();

        //start a panel and set it as current
        void StartPanel(string name, float width = 200f);

        //start a header inside current panel and set it as current group
        void Header(string name);

        //draw a text-value pair in current panel/header group,
        //ex: dbg.Pair("CurrentPos", transform.position.ToString());
        void Pair(string field, string value);

        //just a single label in current panel/header group
        void Label(string label);

        void Separator();

        //returns current toggle state for particular header
        bool isCollapsed(string header);

        //pretty handy, let you send a block of instructions at once
        void ShowInfo(params object[] pack);

        object[] ShowIf(bool condition, params object[] pack);
    }

    public class QuickDebugDrawer : IDebugDrawer
    {
        //screen pos pivot x,y
        public float sx = 10f, sy = 10f;
        public float labelWidth = 200f;
        public float labelHeight = 15f;
        public float labelSep = 15f;
        public float panelWidth = 200f;
        public float headerSep = 5f;

        public int buttonfontSize = 13;
        public bool showPanelsBG = true;

        public Color normalTextColor = new Color(0.87f, 0.83f, 0.81f);
        public Color errorTextColor = new Color(1.0f, 0.31f, 0.3f);
        public Color warningTextColor = new Color(0.96f, 0.80f, 0.121f);
        public Color buttonTextColor = Color.white;
        public Color panelBGColor = new Color(0.22745098f, 0.22745098f, 0.22745098f, 1f);

        //button style
        GUIStyle _headerStyle;
        GUIStyle _nrmStyle;
        GUIStyle _errStyle;
        GUIStyle _wrnStyle;
        GUIStyle _curStyle;
        GUIStyle _btnStyle;
        GUIStyle _panelStyle;
        Texture2D _panelTexture;
        Texture2D _btnTexture;

        private HashSet<string> _collapsedHeaders;

        const float adn = 15f;
        const float ady = 2f;
        float x, y;
        bool firstPanel = true;
        bool hidePanel = false;
        bool hideHeader = false;
        bool groupHeader, groupPanel;

        public QuickDebugDrawer()
        {
            _collapsedHeaders = new HashSet<string>();
        }

        public void Prepare()
        {
            firstPanel = true;
            x = sx; y = sy;

            if (_nrmStyle == null) InitBtnStyle();
            if (showPanelsBG && _panelTexture == null)
            {
                GUI.backgroundColor = panelBGColor;
            }
        }

        public void StartPanel(string name, float width = 200f)
        {
            y = sy;
            if (!firstPanel)
            {
                x += panelWidth;
            }
            else
            {
                firstPanel = false;
            }
            panelWidth = width;

            if (name == "")
            {
                hidePanel = false;
                y -= headerSep;
                return;
            }

            hidePanel = isCollapsed(name);
            hideHeader = false;

            var rect = new Rect(x, y, labelWidth, labelHeight + ady);
            bool click = GUI.Button(rect, name, _headerStyle);
            y += labelHeight + headerSep;

            if (click) ToggleGroup(name);
        }

        public void Header(string header)
        {
            if (hidePanel) return;
            hideHeader = isCollapsed(header);

            y += headerSep;
            var rect = new Rect(x, y, labelWidth - 2, labelHeight + ady);

            bool click = GUI.Button(rect, header, _headerStyle);
            y += labelHeight + headerSep;

            if (click && !hidePanel) ToggleGroup(header);
        }

        public void Pair(string field, string value)
        {
            if (hidePanel || hideHeader) return;

            value = SetStyle(value, ref _curStyle);
            x += labelSep;

            GUI.Label(
                    new Rect(x, y, labelWidth, labelHeight + adn),
                    field + ": " + value, _curStyle);

            y += labelHeight; x -= labelSep;
        }

        public void Label(string label)
        {
            if (hidePanel || hideHeader) return;

            label = SetStyle(label, ref _curStyle);
            x += labelSep;
            GUI.Label(
                    new Rect(x, y, labelWidth, labelHeight + adn),
                    label, _curStyle);

            y += labelHeight; x -= labelSep;
        }

        public void Separator()
        {
            y += headerSep;
        }

        public bool Button(string label)
        {
            if (hidePanel || hideHeader) return false;

            x += labelSep;
            bool click = GUI.Button(
                    new Rect(x, y, labelWidth, labelHeight + adn),
                    label, _btnStyle);

            y += labelHeight; x -= labelSep;
            return click;
        }

        public void ShowInfo(params object[] pack)
        {
            for (int i = 0; i < pack.Length; i++)
            {
                var item = pack[i];
                if (item is object[])
                {
                    ShowInfo(item as object[]);
                    continue;
                }

                var str = item.ToString();

                if (str.StartsWith("@@"))
                {
                    str = str.Remove(0, 2);
                    var args = str.Split('|');
                    if (args.Length >= 2)
                    {
                        int width;
                        if (System.Int32.TryParse(args[1], out width))
                        {
                            StartPanel(args[0], width);
                            continue;
                        }
                    }

                    StartPanel(str);
                    continue;
                }
                if (str.StartsWith("**"))
                {
                    Header(str.Remove(0, 2));
                    continue;
                }
                if (str.StartsWith("##"))
                {
                    Label(str.Remove(0, 2));
                    continue;
                }
                if (str == "Separator" || str == "---")
                {
                    Separator();
                    continue;
                }

                FormPair(str);
            }
        }

        public object[] ShowIf(bool condition, params object[] pack)
        {
            if (condition) return pack;
            else return new object[] {};
        }

        //--- PRIVATE HELPERS --------------------

        string formField = "";

        void FormPair(string str)
        {
            if (formField == "")
            {
                formField = str; return;
            }
            else
            {
                Pair(formField, str); formField = "";
            }
        }

        void ToggleGroup(string name)
        {
            if (_collapsedHeaders.Contains(name))
                _collapsedHeaders.Remove(name);
            else
                _collapsedHeaders.Add(name);
        }

        public bool isCollapsed(string name)
        {
            return _collapsedHeaders.Contains(name);
        }

        string SetStyle(string str, ref GUIStyle style)
        {
            if (str.StartsWith("!!"))
            {
                style = _errStyle;
                return str.Remove(0, 2);
            }
            else if (str.StartsWith("!"))
            {
                style = _wrnStyle;
                return str.Remove(0, 1);
            }
            style = _nrmStyle;
            return str;
        }

        void InitBtnStyle()
        {
            _nrmStyle = new GUIStyle();
            _nrmStyle.normal.textColor = normalTextColor;

            _errStyle = new GUIStyle(_nrmStyle);
            _errStyle.normal.textColor = errorTextColor;

            _wrnStyle = new GUIStyle(_nrmStyle);
            _wrnStyle.normal.textColor = warningTextColor;

            //clickable header style
            _headerStyle = new GUIStyle(_nrmStyle);
            _headerStyle.fontSize = buttonfontSize;
            _headerStyle.normal.textColor = buttonTextColor;
            _headerStyle.padding.left = 6;
            _headerStyle.alignment = TextAnchor.MiddleLeft;

            //clickable button style
            _btnTexture = new Texture2D(1, 1);
            _btnTexture.SetPixel(0, 0, Color.black);

            _btnStyle = new GUIStyle(_nrmStyle);
            _btnStyle.fontSize = buttonfontSize;
            _btnStyle.normal.textColor = buttonTextColor;
            _btnStyle.padding.left = 6;
            _btnStyle.alignment = TextAnchor.MiddleLeft;
            _btnStyle.normal.background = _btnTexture;
        }
    }
}