using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Ссылка на модель игрока
    [Header("Player Mesh")]
    [SerializeField] private Transform _PlayerMesh;

    // Скорость вращения и движения
    [Header("Rotation Speed")]
    [SerializeField] private float RotateSpeed = 1f;

    [Header("Movement Speed")]
    [SerializeField] private float speed = 1f;

    // Угол поворота и направление
    [Header("Rotation Direction")]
    [SerializeField] private Direction Povorot;

    [Header("Target Angle")]
    [SerializeField] private float angle = 0;

    // Флаг остановки
    public bool stop = true;

    void Update()
    {
        Move(); // Вызываем метод перемещения
    }

    private void Move()
    {
        // Если игрок остановлен, выходим из метода
        if (stop)
            return;

        // Обновляем вращение игрока
        if (transform.rotation.eulerAngles.y != angle)
            transform.rotation = Quaternion.Euler(0, angle, 0);

        // Перемещаем игрока вперед
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerStay(Collider other)
    {
        // Обрабатываем столкновение с различными объектами
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
            // Получаем направление поворота
            Povorot = other.GetComponent<Napravlayshiy>().Napravlenie;

            // Перемещаем игрока на позицию объекта
            transform.position = other.transform.position;

            // Уничтожаем объект
            Destroy(other.gameObject);

            // Запускаем корутин для вращения
            StartCoroutine(Rotate(Povorot));
        }
    }

    private void HandleMoneyPickup(Collider other)
    {
        var playerMesh = _PlayerMesh.GetComponent<PlayerMesh>();
        playerMesh.audioSource.PlayOneShot(playerMesh.clips[2]);
        playerMesh._UIManager.AddGold(1);
        Destroy(other.gameObject); // Уничтожаем объект "деньги"
    }

    private void HandleBottlePickup(Collider other)
    {
        var playerMesh = _PlayerMesh.GetComponent<PlayerMesh>();
        playerMesh.audioSource.PlayOneShot(playerMesh.clips[1]);
        playerMesh._UIManager.AddGold(-5);
        Destroy(other.gameObject); // Уничтожаем объект "бутылка"
    }

    private void HandleFlagActivation(Collider other)
    {
        var playerMesh = _PlayerMesh.GetComponent<PlayerMesh>();
        playerMesh.audioSource.PlayOneShot(playerMesh.clips[3]);
        other.GetComponent<FlagController>().Active();
        Destroy(other); // Уничтожаем объект "флаг"
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
            stop = true; // Останавливаем игрока
            playerMesh._UIManager.TriggerGameWin();
        }

        Destroy(other); // Уничтожаем объект "дверь"
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
