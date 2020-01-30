using System;
using UnityEditor;
using UnityEngine;

namespace Gamesture.CodeDocs
{
    public class CodeDocsInfoPopup : EditorWindow
    {
        private Vector2 _scroll;
        private string _text = string.Empty;
        private bool _updateScrollPos;
        
        internal static void Show(out Action<string> addTextAction, out Action close)
        {
            CodeDocsInfoPopup window =
                (CodeDocsInfoPopup) GetWindow(typeof(CodeDocsInfoPopup), true, "Generating docs...");
            window.minSize = new Vector2(600, 400);
            addTextAction = window.AddText;
            close = window.Close;
            window.Show();
        }

        private void AddText(string text)
        {
            _text += $"{text}\n";
            _updateScrollPos = true;
        }

        private void OnGUI()
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            _text = EditorGUILayout.TextArea(_text, GUILayout.Width(position.width - 30));
            EditorGUILayout.EndScrollView();

            if (_updateScrollPos)
            {
                _scroll.y = float.MaxValue;
                _updateScrollPos = false;
            }
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}