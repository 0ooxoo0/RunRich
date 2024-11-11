using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMesh : MonoBehaviour
{
    [Header("����������")]
    public List<AudioClip> clips;                          // ������ ����������� ��� ������������
    [Header("�������������")]
    public AudioSource audioSource;                         // ������������� ��� ��������������� ������
    [Header("��������")]
    public Animator anim;                                   // �������� ��� ���������� ���������� ���������
    [SerializeField] private List<PlayerRich> playerMeshRich; // ������ ��������� ������� ��������� � �� ����������
    [Header("������ ������� ������")]
    public int meshid = 0;                                  // ������ ������� ������ ���������
    private SkinnedMeshRenderer meshPlayer;                 // ��������� ��� ����������� ����� ���������
    [Header("��������� ����")]
    public bool start = true;                               // ����, �����������, �������� �� ����
    public UIManager _UIManager;                             // �������� ����������������� ����������
    [SerializeField] private GameObject particlePrefab;     // ������ ������, ������� ����� ������������ � ����

    // ����� ��� ������ � ��������� ���������� ������ �� ������ ������
    public IEnumerator FindMesh(float gold)
    {
        Debug.Log("������: " + gold);
        int i = 0;
        Mesh mesh = null;

        // ���� ������ ���������� ������ � ������ ������
        while (i < playerMeshRich.Count)
        {
            if (playerMeshRich[i].GoldValue < gold)
            {
                mesh = playerMeshRich[i].mesh;
            }
            else
            {
                break; // ���������� �����, ����model ���������� ������ ������� �������
            }
            yield return new WaitForEndOfFrame();
            i++;
        }

        // ������������� ��������� ������
        meshPlayer.sharedMesh = mesh;
        meshid = i;

        // ���� ������ ������������, ������������� ����� ������� ������
        if (gold < playerMeshRich[0].GoldValue)
        {
            meshPlayer.sharedMesh = playerMeshRich[0].mesh;
            meshid = 0;
        }

        // ����������� ��������� ���������
        audioSource.PlayOneShot(clips[0]);
        yield return null;
    }

    // ����� ��� ����������� ��������� �� �����������
    public void MoveHorizontal(float value)
    {
        transform.localPosition = new Vector3(value, 0, 0);

        // ��������� ���� ��� ������ ��������
        if (start)
        {
            _UIManager.StartGame();
            start = false;
        }
    }

    // ����� ��� �������� ��������� � ����� ������
    public IEnumerator NextRich()
    {
        Debug.Log("���������� ��������...");
        bool hasRotated = false;

        // ������� ��������� �� 360 ��������
        while (transform.rotation.eulerAngles.y < 360)
        {
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + (Time.deltaTime * 100), 0);
            if (transform.rotation.eulerAngles.y > 180 && !hasRotated)
            {
                NextMesh(1);
                hasRotated = true;
            }
            yield return new WaitForSeconds(0.005f);
        }

        // ���������.rotation ����� ���������� ��������
        transform.rotation = Quaternion.Euler(0, 360f, 0);
        yield return null;
    }

    // �����, ���������� ��� ������ ����
    void Start()
    {
        meshPlayer = GetComponent<SkinnedMeshRenderer>();
        anim = GetComponent<Animator>();
        meshPlayer.sharedMesh = playerMeshRich[meshid].mesh;
        anim.SetInteger("RichVolume", meshid);
    }

    // ����� ��� ��������� ������ � ����������� �� �������� ���������� ������
    public void NextMesh(int value)
    {
        Debug.Log("��������� ������...");
        StartCoroutine(FindMesh(_UIManager.money));
        anim.SetInteger("RichVolume", meshid);
    }
}

// ����� ��� �������� ���������� � ��������� ���������
[System.Serializable]
public class PlayerRich
{
    [Header("���� ������")]
    public int GoldValue; // �������� ������, ����������� ��� ������
    [Header("�����")]
    public Mesh mesh;     // ��������������� ����� ������ ���������
}
