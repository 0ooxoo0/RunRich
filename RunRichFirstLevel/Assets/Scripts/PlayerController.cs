using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // ������ �� ������ ������
    [Header("Player Mesh")]
    [SerializeField] private Transform _PlayerMesh;

    // �������� �������� � ��������
    [Header("Rotation Speed")]
    [SerializeField] private float RotateSpeed = 1f;

    [Header("Movement Speed")]
    [SerializeField] private float speed = 1f;

    // ���� �������� � �����������
    [Header("Rotation Direction")]
    [SerializeField] private Direction Povorot;

    [Header("Target Angle")]
    [SerializeField] private float angle = 0;

    // ���� ���������
    public bool stop = true;

    void Update()
    {
        Move(); // �������� ����� �����������
    }

    private void Move()
    {
        // ���� ����� ����������, ������� �� ������
        if (stop)
            return;

        // ��������� �������� ������
        if (transform.rotation.eulerAngles.y != angle)
            transform.rotation = Quaternion.Euler(0, angle, 0);

        // ���������� ������ ������
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerStay(Collider other)
    {
        // ������������ ������������ � ���������� ���������
        switch (other.tag)
        {
            case "Rotate":
                HandleRotationCollider(other);
                break;

            case "Money":
                HandleMoneyPickup(other);
                break;

            case "Bottle":
                HandleBottlePickup(other);
                break;

            case "Flag":
                HandleFlagActivation(other);
                break;

            case "Door":
                HandleDoorInteraction(other);
                break;
        }
    }

    private void HandleRotationCollider(Collider other)
    {
        if (Vector3.Distance(other.transform.position, transform.position) < 1)
        {
            // �������� ����������� ��������
            Povorot = other.GetComponent<Napravlayshiy>().Napravlenie;

            // ���������� ������ �� ������� �������
            transform.position = other.transform.position;

            // ���������� ������
            Destroy(other.gameObject);

            // ��������� ������� ��� ��������
            StartCoroutine(Rotate(Povorot));
        }
    }

    private void HandleMoneyPickup(Collider other)
    {
        var playerMesh = _PlayerMesh.GetComponent<PlayerMesh>();
        playerMesh.audioSource.PlayOneShot(playerMesh.clips[2]);
        playerMesh._UIManager.AddGold(1);
        Destroy(other.gameObject); // ���������� ������ "������"
    }

    private void HandleBottlePickup(Collider other)
    {
        var playerMesh = _PlayerMesh.GetComponent<PlayerMesh>();
        playerMesh.audioSource.PlayOneShot(playerMesh.clips[1]);
        playerMesh._UIManager.AddGold(-5);
        Destroy(other.gameObject); // ���������� ������ "�������"
    }

    private void HandleFlagActivation(Collider other)
    {
        var playerMesh = _PlayerMesh.GetComponent<PlayerMesh>();
        playerMesh.audioSource.PlayOneShot(playerMesh.clips[3]);
        other.GetComponent<FlagController>().Active();
        Destroy(other); // ���������� ������ "����"
    }

    private void HandleDoorInteraction(Collider other)
    {
        var playerMesh = _PlayerMesh.GetComponent<PlayerMesh>();
        var door = other.GetComponent<Dveri>();

        if (playerMesh._UIManager.money >= door.PorogVhoda)
        {
            playerMesh.audioSource.PlayOneShot(playerMesh.clips[3]);
            door.Active();
        }
        else
        {
            stop = true; // ������������� ������
            playerMesh._UIManager.TriggerGameWin();
        }

        Destroy(other); // ���������� ������ "�����"
    }

    IEnumerator Rotate(Direction _Povorot)
    {
        int schot = 0;
        stop = true;

        if (_Povorot == Direction.Left)
            angle -= 90;
        if (_Povorot == Direction.Right)
        {
            angle += 90;
        }
        float steps = 0;
        while (transform.rotation.eulerAngles.y != angle)
        {
            schot++;

            if (transform.rotation.eulerAngles.y - angle > 0)
            {
                steps = math.abs(steps) - 1;
            }
            else if (transform.rotation.eulerAngles.y - angle < 0)
            {
                steps = math.abs(steps) + 1;
            }


            transform.rotation = Quaternion.Euler(0, steps, 0);

            yield return new WaitForEndOfFrame();
        }
        stop = false;
        yield return null;
    }
}

public enum Direction
{
    Left,
    Right
}
