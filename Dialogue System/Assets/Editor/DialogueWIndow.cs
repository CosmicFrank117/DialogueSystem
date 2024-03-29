using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DialogueWIndow : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    private DialogueLineData m_activeItem = null;
    private InspectorElement m_detailInspecteor = null;
    private GroupBox m_listBox = null;
    private GroupBox m_detailBox = null;
    private ListView m_listView = null;
    private Button m_addButton = null;
    private Button m_deleteButton = null;
    private DialogueDatabase m_currentDatabase = null;

    [MenuItem("Window/MyTools/DialogueWIndow")]
    public static void ShowExample()
    {
        DialogueWIndow wnd = GetWindow<DialogueWIndow>();
        wnd.titleContent = new GUIContent("DialogueWIndow");
    }

    private void BindFunc(VisualElement e, int i)
    {
        Label item = e as Label;
        item.text = m_currentDatabase.dataList[i].name;
    }

    public void PopulateDialogueList()
    {
        m_listBox.Clear();

        if (m_currentDatabase != null)
        {
            Func<VisualElement> makeItem = () => new Label();

            Action<VisualElement, int> bindItem = BindFunc;

            m_listView = new ListView(m_currentDatabase.dataList, EditorGUIUtility.singleLineHeight, makeItem, bindItem);

            m_listView.itemsSource = m_currentDatabase.dataList;

            m_listView.selectionType = SelectionType.Single;

            m_listBox.Add(m_listView);

            m_listView.onSelectionChange += DialogueListSelectionChanged;
        }
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        var objectField = new ObjectField();
        objectField.label = "Dialogue Database";
        objectField.objectType = typeof(DialogueDatabase);

        objectField.RegisterCallback<ChangeEvent<UnityEngine.Object>>( e =>
        {
            if (objectField.value != null)
            {
                m_currentDatabase = objectField.value as DialogueDatabase;
            }
            else
            {
                m_currentDatabase = null;
            }

            PopulateDialogueList();

        });

        root.Add(objectField);

        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/DialogueWindow.uxml");

        VisualElement uxmlData = visualTree.Instantiate();
        root.Add(uxmlData);

        m_listBox = root.Query<GroupBox>("MainContent").First();
        m_detailBox = root.Query<GroupBox>("DetailBox").First();

        m_addButton = root.Query<Button>("AddButton").First();
        m_addButton.clicked += () => CreateNewDialogueLine();

        m_deleteButton = root.Query<Button>("DeleteButton").First();
        m_deleteButton.clicked += () => DeleteSelectedDialogueLine();
    }

    private void CreateNewDialogueLine()

    {
        string path = EditorUtility.SaveFilePanelInProject("Create Dialogue File", "DefaultDialogueLine", "asset", "Please select a name for your dialogue file");

        if (path.Length != 0)
        {
            DialogueLineData line = ScriptableObject.CreateInstance<DialogueLineData>();
            AssetDatabase.CreateAsset(line, path);
            AssetDatabase.Refresh();


            m_currentDatabase.dataList.Add(line);
            EditorUtility.SetDirty(m_currentDatabase);

            PopulateDialogueList();

            m_listView.SetSelection(m_currentDatabase.dataList.Count - 1);
        }
    }

    private void DeleteSelectedDialogueLine()
    {
        if (m_activeItem != null)
        {
            m_currentDatabase.dataList.Remove(m_activeItem);

            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(m_activeItem));
            EditorUtility.SetDirty(m_currentDatabase);

            m_activeItem = null;

            PopulateDialogueList();

            if (m_currentDatabase.dataList.Count > 0)
            {
                m_listView.SetSelection(0);
            }
        }


    }

    private void DialogueListSelectionChanged(IEnumerable<object> selectedItems)
    {
        foreach(DialogueLineData line in selectedItems)
        {
            m_activeItem = line;
        }

        if (m_detailInspecteor == null)
        {
            m_detailInspecteor = new InspectorElement();
            m_detailInspecteor.style.flexGrow = 1.0f;
            m_detailBox.Add(m_detailInspecteor);
        }

        if (selectedItems.Count() > 0)
        {
             m_detailInspecteor.Bind(new SerializedObject(m_activeItem));
        }

       
    }
}
