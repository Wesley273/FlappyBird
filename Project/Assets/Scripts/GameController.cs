using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Process testProcess;
    public static string result = "";
    public static GameController instance = null;

    // 管道预制件
    public GameObject pipesPrefabs;

    public Text countText;
    public GameObject gameOverTips;

    // 管道产生的频率，每几秒产生一个
    public float createPipesRate = 3f;

    // 管道中心位置的y最小值
    public float minPipPosY = -1f;
    // 管道中心位置的y最大值
    public float maxPipPosY = 4f;
    // 初始化管道的位置，x最好为负数不可见位置
    public Vector2 startPipPos = new Vector2(-12f, 0f);

    // 统计已经成功过了几个管道
    private int count = 0;
    // 小鸟是否已经死了
    [HideInInspector] public bool isGameOver;

    // 上一次创建出管道的时间
    private float lastCreatePipTime = float.NegativeInfinity;

    // 缓存管道的链表，用来复用管道
    private List<GameObject> pipes = new List<GameObject>();
    // 管道缓存的个数
    private const int PIPESTOTAL = 8;
    // 当前管道下标，用来更新管道
    private int currPipesIndex = 0;

    private void Awake()
    {
        Application.targetFrameRate = 5;
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        isGameOver = false;
        gameOverTips.SetActive(false);
        // 开始时，创建出管道缓存
        InitPipesPool();
        Thread myThread = new Thread(RunPythonScript);//通过ThreadStart委托告诉子线程执行什么方法
        myThread.Start();//启动新线程
    }

    private void Update()
    {
        // 当可以创建出管道时，拿出缓存中一个管道来更新位置
        if (!isGameOver && lastCreatePipTime + createPipesRate < Time.time)
        {

            lastCreatePipTime = Time.time;
            UpdatePipesPosition();
            currPipesIndex = (currPipesIndex + 1) % PIPESTOTAL;
        }

        if (isGameOver && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)))
        {
            GameRestart();
        }
    }

    public void PassOnePip()
    {
        count++;
        countText.text = "Count: " + count.ToString();
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        gameOverTips.SetActive(true);

        SoundManager.instance.PlayDie();
    }

    private void GameRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // 初始化管道缓存池
    private void InitPipesPool()
    {
        for (int i = 0; i < PIPESTOTAL; ++i)
        {
            GameObject obj = Instantiate(pipesPrefabs, startPipPos, Quaternion.identity);
            pipes.Add(obj);
        }
    }

    // 更新当前管道的位置
    private void UpdatePipesPosition()
    {
        float randomPosY = UnityEngine.Random.Range(minPipPosY, maxPipPosY);
        Vector2 position = new Vector2(10f, randomPosY);
        pipes[currPipesIndex].transform.position = position;
    }
    public void RunPythonScript()
    {
        string sArgName = @"JudgeEmotion.py";
        Process myProcess = new Process();
        myProcess.StartInfo.WorkingDirectory = System.Environment.CurrentDirectory + @"\PythonScripts";
        myProcess.StartInfo.Arguments = System.Environment.CurrentDirectory + @"\PythonScripts\" + sArgName;
        myProcess.StartInfo.FileName = "python.exe";
        myProcess.StartInfo.CreateNoWindow = true;
        myProcess.StartInfo.UseShellExecute = false;
        myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;//这里设置DOS窗口不显示，经实践可行
        myProcess.StartInfo.RedirectStandardOutput = true;
        myProcess.StartInfo.RedirectStandardInput = true;
        myProcess.Start();
        myProcess.BeginOutputReadLine();
        myProcess.OutputDataReceived += new DataReceivedEventHandler(outPutData);
        Console.ReadLine();
        myProcess.WaitForExit();
    }
    static void outPutData(object sender, DataReceivedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.Data))
        {
            result = e.Data;
        }
    }
}
