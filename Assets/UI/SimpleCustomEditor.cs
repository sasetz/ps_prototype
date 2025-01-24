using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SimpleCustomEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Window/UI Toolkit/SimpleCustomEditor")]
    public static void ShowExample()
    {
        SimpleCustomEditor wnd = GetWindow<SimpleCustomEditor>();
        wnd.titleContent = new GUIContent("SimpleCustomEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);
    }
}
