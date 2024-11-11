using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMesh : MonoBehaviour
{
    [Header("Аудиоклипы")]
    public List<AudioClip> clips;                          // Список аудиоклипов для проигрывания
    [Header("Аудиоисточник")]
    public AudioSource audioSource;                         // Аудиоисточник для воспроизведения звуков
    [Header("Аниматор")]
    public Animator anim;                                   // Аниматор для управления анимациями персонажа
    [SerializeField] private List<PlayerRich> playerMeshRich; // Список доступных моделей персонажа с их стоимостью
    [Header("Индекс текущей модели")]
    public int meshid = 0;                                  // Индекс текущей модели персонажа
    private SkinnedMeshRenderer meshPlayer;                 // Компонент для отображения сетки персонажа
    [Header("Стартовая игра")]
    public bool start = true;                               // Флаг, указывающий, началась ли игра
    public UIManager _UIManager;                             // Менеджер пользовательского интерфейса
    [SerializeField] private GameObject particlePrefab;     // Префаб частиц, который можно использовать в игре

    // Метод для поиска и установки подходящей модели на основе золота
    public IEnumerator FindMesh(float gold)
    {
        Debug.Log("Золото: " + gold);
        int i = 0;
        Mesh mesh = null;

        // Ищем первую подходящую модель с учетом золота
        while (i < playerMeshRich.Count)
        {
            if (playerMeshRich[i].GoldValue < gold)
            {
                mesh = playerMeshRich[i].mesh;
            }
            else
            {
                break; // Прекращаем поиск, еслиmodel дальнейшие модели слишком дорогие
            }
            yield return new WaitForEndOfFrame();
            i++;
        }

        // Устанавливаем найденную модель
        meshPlayer.sharedMesh = mesh;
        meshid = i;

        // Если золота недостаточно, устанавливаем самую дешевую модель
        if (gold < playerMeshRich[0].GoldValue)
        {
            meshPlayer.sharedMesh = playerMeshRich[0].mesh;
            meshid = 0;
        }

        // Проигрываем первичный аудиоклип
        audioSource.PlayOneShot(clips[0]);
        yield return null;
    }

    // Метод для перемещения персонажа по горизонтали
    public void MoveHorizontal(float value)
    {
        transform.localPosition = new Vector3(value, 0, 0);

        // Запускаем игру при первом движении
        if (start)
        {
            _UIManager.StartGame();
            start = false;
        }
    }

    // Метод для вращения персонажа и смены модели
    public IEnumerator NextRich()
    {
        Debug.Log("Начинается вращение...");
        bool hasRotated = false;

        // Вращаем персонажа до 360 градусов
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

        // Обновляем.rotation после завершения вращения
        transform.rotation = Quaternion.Euler(0, 360f, 0);
        yield return null;
    }

    // Метод, вызываемый при старте игры
    void Start()
    {
        meshPlayer = GetComponent<SkinnedMeshRenderer>();
        anim = GetComponent<Animator>();
        meshPlayer.sharedMesh = playerMeshRich[meshid].mesh;
        anim.SetInteger("RichVolume", meshid);
    }

    // Метод для изменения модели в зависимости от текущего количества золота
    public void NextMesh(int value)
    {
        Debug.Log("Изменение модели...");
        StartCoroutine(FindMesh(_UIManager.money));
        anim.SetInteger("RichVolume", meshid);
    }
}

// Класс для хранения информации о богатстве персонажа
[System.Serializable]
public class PlayerRich
{
    [Header("Цена золота")]
    public int GoldValue; // Значение золота, необходимое для модели
    [Header("Сетка")]
    public Mesh mesh;     // Соответствующая сетка модели персонажа
}
