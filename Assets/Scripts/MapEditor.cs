using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

//������ Custom�ҰŴ�?
[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
    //target�� ���� ����
    Map map;
    //map.objectList�� ����ִ� �ֵ��� �̸�
    string[] objectListName;

    //�ش� ���� ������Ʈ�� Ŭ���Ǿ����� ȣ��Ǵ� �Լ�
    private void OnEnable()
    {
        Debug.Log("���õ�");
        map = (Map)target;

        //map.objectList�� ����ִ� �ֵ��� �̸� ����
        objectListName = new string[map.objectList.Length];
        for (int i = 0; i < objectListName.Length; i++)
            objectListName[i] = map.objectList[i].name;
    }

    //Inspector �� ��ȭ�� ������ ȣ��Ǵ� �Լ�
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        //tileX, tileZ�� ǥ��
        map.tileX = EditorGUILayout.IntField("Ÿ�� ����", map.tileX);
        EditorGUILayout.Space();
        map.tileZ = EditorGUILayout.IntField("Ÿ�� ����", map.tileZ);
        //tileX, tileZ (1~100)
        map.tileX = Mathf.Clamp(map.tileX, 1, 100);
        map.tileZ = Mathf.Clamp(map.tileZ, 1, 100);

        //�ٴ� Prefab ����
        map.floorFactory = (GameObject)EditorGUILayout.ObjectField("�ٴ�", map.floorFactory, typeof(GameObject), false);
        //BlueCube Prefab ����
        map.blueCubeFactory = (GameObject)EditorGUILayout.ObjectField("�Ķ� ť��", map.blueCubeFactory, typeof(GameObject), false);

        //objectList ����
        EditorGUI.ChangeCheckScope check = new EditorGUI.ChangeCheckScope();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("objectList"));

        if(check.changed)
        {
            serializedObject.ApplyModifiedProperties();
        }

        //���� ������Ʈ ����
        map.selectObjIdx = EditorGUILayout.Popup("���� ������Ʈ", map.selectObjIdx, objectListName);

        //�ٴڻ��� ��ư
        if (GUILayout.Button("�ٴ� ����"))
        {
            CreateFloor();
        }

        //�ν����Ϳ� ��������� ����ٸ�
        if(GUI.changed)
        {
            //Hierarchy�� Scene�̸� ���� *ǥ�� ������ ����
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            //Sceneȭ���� �ٽ� �׸���
            SceneView.RepaintAll();
        }
    }

    //Scene ȭ���� �׸��� �Լ�
    private void OnSceneGUI()
    {
        //���� Map �Ǿ������� (Sceneȭ�鿡����)�ٸ� ���ӿ�����Ʈ�� �������� ���ϰ� ����
        int id = GUIUtility.GetControlID(FocusType.Passive);
        HandleUtility.AddDefaultControl(id);

        DrawGrid();

        CreateObject();

        DeleteObject();
    }

    void DeleteObject()
    {
        Event e = Event.current;
        //���࿡ ���� ���콺�� Ŭ���ߴٸ�
        //���࿡ ctrlŰ�� �����ִٸ�        
        if(e.type == EventType.MouseDown && e.button == 0 && e.control)
        {
            //Ray�� ���콺 ��ġ���� ����
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //���� ���ӿ�����Ʈ�� Layer�� Object��� 
                if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Object"))
                {
                    //������
                    DestroyImmediate(hit.transform.gameObject);
                }
            }            
        }
    }

    void CreateObject()
    {
        Event e = Event.current;
        //���࿡ ���� ���콺�� Ŭ���ߴٸ�
        if(e.type == EventType.MouseDown && e.button == 0 && e.control == false)
        {
            //���콺 ��ġ�� Ray�� �����.
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;
            //���� Ray�� �߻��ؼ� �¾Ҵٸ�
            if (Physics.Raycast(ray, out hit))
            {
                //���࿡ �ε��� ���� Layer�� Floor�� ��
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Floor"))
                {
                    //�ش� ��ġ�� BlueCube�� �����ؼ� ���´�.
                    GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(
                        map.objectList[map.selectObjIdx]);

                    int x = (int)hit.point.x;
                    int z = (int)hit.point.z;

                    obj.transform.position = new Vector3(x, hit.point.y, z);

                    //�θ� �ٴ����� ����
                    obj.transform.parent = GameObject.Find("Floor").transform;
                }
            }
        }        
    }

    void CreateFloor()
    {
        //Floor���� ������Ʈ�� ã��
        GameObject floor = GameObject.Find("Floor");
        //���࿡ ������ �۾��ϴ� floor�� �����Ѵٸ�
        if(floor != null)
        {            
            //�ı�����
            DestroyImmediate(floor);
        }

        //prefab �� ����������� ���ӿ�����Ʈ
        //Instantiate(map.floorFactory);
        //prefab �� ����� ���ӿ�����Ʈ
        floor = (GameObject)PrefabUtility.InstantiatePrefab(map.floorFactory);
        //tileX, tileZ�� ������ floor�� ũ�� ���� ����
        floor.transform.localScale = new Vector3(map.tileX, 1, map.tileZ);
    }

    void DrawGrid()
    {
        Vector3 start;
        Vector3 end;
        Handles.color = Color.red;
        //������(tileX)
        for (int i = 0; i <= map.tileX; i++)
        {
            start = new Vector3(i, 0, 0);
            end = new Vector3(i, 0, map.tileZ);
            Handles.DrawLine(start, end);
        }

        Handles.color = Color.blue;
        //������(tileZ)
        for (int i = 0; i <= map.tileZ; i++)
        {
            start = new Vector3(0, 0, i);
            end = new Vector3(map.tileX, 0, i);
            Handles.DrawLine(start, end);
        }
    }
}
