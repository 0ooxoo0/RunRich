using ButchersGames;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("������� ����������")]
    [SerializeField] private GameObject clickButton;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameWinPanel;

    [Header("������� ����������")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerMesh playerMesh;
    [SerializeField] private Level level;
    [SerializeField] private LevelManager levelManager;

    [Header("������� ������")]
    public float money = 10;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TextMeshProUGUI moneyText;

    private void Start()
    {
        // ������������� ��������� ��������
        moneyText.text = money.ToString();
        progressSlider.value = 0;
    }

    public void AddGold(int value)
    {
        money += value;

        // ���������� ��������� � ����� ���� ������ ���� �������� ��������� ��������
        if (progressSlider.value + value > progressSlider.maxValue)
        {
            progressSlider.value = (progressSlider.value + value) - progressSlider.maxValue;
            playerMesh.NextMesh(1);
        }
        else if (progressSlider.value + value < 0)
        {
            progressSlider.value = progressSlider.maxValue + (progressSlider.value + value);
            playerMesh.NextMesh(-1);
        }
        else
        {
            progressSlider.value += value;
        }

        // �������� ���������� ������������� �������� ���������
        if (progressSlider.value >= progressSlider.maxValue)
        {
            progressSlider.value = 0;
            playerMesh.NextMesh(1);
        }

        // ���������� ����������� ���������
        progressSlider.value = money - (int)(progressSlider.maxValue * (playerMesh.meshid - 1));

        // ��������, �������� �� ������
        if (money <= 0)
        {
            moneyText.text = "0";
            TriggerGameOver();
            return;
        }

        // ���������� ������ � ����������� �����
        moneyText.text = money.ToString();
    }

    public void StartGame()
    {
        // ������ ����: ������������� ������ � ������� ������
        playerMesh = playerController.transform.GetChild(0).GetComponent<PlayerMesh>();
        playerMesh.anim.SetTrigger("DefaultWalk");

        level = levelManager.transform.GetChild(0).GetComponent<Level>();
        levelManager.StartLevel();

        playerController.stop = false;
        playerController.transform.position = level.playerSpawnPoint.position;
        playerMesh.transform.localPosition = Vector3.zero;
        clickButton.SetActive(false);
    }

    public void NextLevel()
    {
        // ������� �� ��������� �������
        playerMesh.anim.SetTrigger("DefauldState");
        gameWinPanel.SetActive(false);
        levelManager.NextLevel();

        playerController.stop = true;
        playerController.transform.position = level.playerSpawnPoint.position;
        playerController.transform.GetChild(0).localPosition = Vector3.zero;

        clickButton.SetActive(true);
        playerMesh.start = true;
        money = 10;
    }

    public void RestartLevel()
    {
        // ���������� �������� ������
        playerMesh.anim.SetTrigger("DefauldState");
        gameOverPanel.SetActive(false);
        levelManager.RestartLevel();

        playerController.stop = true;
        playerController.transform.position = level.playerSpawnPoint.position;
        playerController.transform.GetChild(0).localPosition = Vector3.zero;

        clickButton.SetActive(true);
        playerMesh.start = true;
        money = 10;
    }

    private void TriggerGameOver()
    {
        // ��������� ��������� ����
        playerMesh.GetComponent<PlayerMesh>().audioSource.PlayOneShot(playerMesh.clips[4]);
        playerMesh.anim.SetTrigger("Lose");
        gameOverPanel.SetActive(true);
        playerController.stop = true;
    }

    public void TriggerGameWin()
    {
        // ��������� ��������
        playerMesh.GetComponent<PlayerMesh>().audioSource.PlayOneShot(playerMesh.clips[5]);
        playerMesh.anim.SetTrigger("Win");
        gameWinPanel.SetActive(true);
        playerController.stop = true;
    }
}
