using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://deff-dev.tistory.com/124
public class SingletonObject<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                // �ش� ������Ʈ�� ������ �ִ� ���� ������Ʈ�� ã�Ƽ� ��ȯ�Ѵ�.
                instance = (T)FindAnyObjectByType(typeof(T));

                if (instance == null) // �ν��Ͻ��� ã�� ���� ���
                {
                    // ���ο� ���� ������Ʈ�� �����Ͽ� �ش� ������Ʈ�� �߰��Ѵ�.
                    GameObject obj = new GameObject(typeof(T).Name, typeof(T));
                    // ������ ���� ������Ʈ���� �ش� ������Ʈ�� instance�� �����Ѵ�.
                    instance = obj.GetComponent<T>();
                }
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (transform.parent != null && transform.root != null) // �ش� ������Ʈ�� �ڽ� ������Ʈ���
        {
            DontDestroyOnLoad(this.transform.root.gameObject); // �θ� ������Ʈ�� DontDestroyOnLoad ó��
        }
        else
        {
            DontDestroyOnLoad(this.gameObject); // �ش� ������Ʈ�� �� ���� ������Ʈ��� �ڽ��� DontDestroyOnLoad ó��
        }
    }
}
