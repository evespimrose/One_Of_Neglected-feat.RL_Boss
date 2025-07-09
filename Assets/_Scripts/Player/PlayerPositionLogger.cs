using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using System.Diagnostics;
using System.Text;
using Cysharp.Threading.Tasks;
using Debug = UnityEngine.Debug;

public class PlayerPositionLogger : MonoBehaviour
{
    private List<float[]> logRows = new List<float[]>();
    private string logFilePath;
    private string outputFilePath;
    private DateTime lastLogWriteTime;
    private DateTime lastOutputWriteTime;
    private static readonly string LabelLine = "userx,usery,monster1x,monster1y,monster2x,monster2y,monster3x,monster3y,monster4x,monster4y,monster5x,monster5y";

    private readonly string pythonExePath = @"C:\Users\Jang\AppData\Local\Programs\Python\Python310\python.exe";
    // private readonly string pythonExePath = @"C:\Users\ice31\AppData\Local\Programs\Python\Python310\python.exe";

    private readonly string pythonScriptPath = @"D:\Unity\One_Of_Neglected-feat.AI-\Python\DL2.py";
    // private readonly string pythonScriptPath = @"C:\Users\ice31\Downloads\One_Of_Neglected-feat.AI-\Python\DL2.py";

    private Process pythonProcess;

    private void Awake()
    {
        logFilePath = Path.Combine(Application.persistentDataPath, "PlayerPositionLog.txt");
        outputFilePath = Path.Combine(Application.persistentDataPath, "auto_trap_placement.txt");

        File.WriteAllText(logFilePath, LabelLine + Environment.NewLine);
        File.WriteAllText(outputFilePath, string.Empty);

        lastLogWriteTime = File.Exists(logFilePath)
            ? File.GetLastWriteTime(logFilePath)
            : DateTime.MinValue;

        lastOutputWriteTime = File.Exists(outputFilePath)
            ? File.GetLastWriteTime(outputFilePath)
            : DateTime.MinValue;

        CheckOrCreateLogFile();

        StartLoggingLoop().Forget();
        StartSavingLoop().Forget();
        
        // 게임 시작 시 한 번만 파이썬 스크립트 실행
        RunPythonScript();
    }

    private void Update()
    {
        CheckOutputFileChanged();
    }

    private async UniTaskVoid StartLoggingLoop()
    {
        var token = this.GetCancellationTokenOnDestroy();
        while (true)
        {
            await UniTask.DelayFrame(3, PlayerLoopTiming.Update, token);
            LogUserAndMonsterPositions();
        }
    }

    private async UniTaskVoid StartSavingLoop()
    {
        var token = this.GetCancellationTokenOnDestroy();
        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(30), cancellationToken: token);
            if (logRows.Count > 0)
                SaveLogRowsToFile();
        }
    }

    private void LogUserAndMonsterPositions()
    {
        Vector2 userPos = transform.position;
        var monsters = UnitManager.Instance.activeMonsters
            .Where(m => m != null)
            .OrderBy(m => Vector2.Distance(userPos, m.transform.position))
            .Take(5)
            .ToList();

        while (monsters.Count < 5)
            monsters.Add(null);

        float[] row = new float[12];
        row[0] = userPos.x;
        row[1] = userPos.y;

        for (int i = 0; i < 5; i++)
        {
            if (monsters[i] != null)
            {
                Vector2 mpos = monsters[i].transform.position;
                row[2 + i * 2] = mpos.x;
                row[2 + i * 2 + 1] = mpos.y;
            }
            else
            {
                row[2 + i * 2] = float.NaN;
                row[2 + i * 2 + 1] = float.NaN;
            }
        }

        logRows.Add(row);
    }

    private void CheckOrCreateLogFile()
    {
        if (!File.Exists(logFilePath) || File.ReadAllLines(logFilePath).Length == 0)
        {
            File.WriteAllText(logFilePath, LabelLine + Environment.NewLine);
        }
        else
        {
            var lines = File.ReadAllLines(logFilePath);
            if (lines[0] != LabelLine)
            {
                File.WriteAllLines(logFilePath, new[] { LabelLine });
            }
        }
    }

    private void SaveLogRowsToFile()
    {
        File.WriteAllText(logFilePath, LabelLine + Environment.NewLine);

        var lines = File.Exists(logFilePath)
            ? File.ReadAllLines(logFilePath).ToList()
            : new List<string>();

        if (lines.Count == 0 || lines[0] != LabelLine)
        {
            lines.Insert(0, LabelLine);
        }

        foreach (var row in logRows)
        {
            string[] rowStrings = new string[12];
            for (int i = 0; i < 12; i++)
            {
                rowStrings[i] = float.IsNaN(row[i]) ? "" : row[i].ToString();
            }
            lines.Add(string.Join(",", rowStrings));
        }

        File.WriteAllLines(logFilePath, lines);
        logRows.Clear();
        Debug.Log("[1. 로그 저장] 플레이어 및 몬스터 위치 데이터 저장 완료");
    }

    private bool CheckLogFileChanged()
    {
        if (!File.Exists(logFilePath)) return false;

        DateTime writeTime = File.GetLastWriteTime(logFilePath);
        if (writeTime <= lastLogWriteTime) return false;

        lastLogWriteTime = writeTime;
        Debug.Log("[2. 파일 변경] 로그 파일 변경 감지됨");
        return true;
    }

    private void RunPythonScript()
    {
        try
        {
            if (pythonProcess != null && !pythonProcess.HasExited)
            {
                Debug.LogWarning("[3. 파이썬 실행] 이전 프로세스가 아직 종료되지 않음. 새 실행을 건너뜀.");
                return;
            }

            Debug.Log("[3. 파이썬 실행] AI 예측 스크립트 비동기 실행 시작");

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = pythonExePath,
                Arguments = $"\"{pythonScriptPath}\" \"{logFilePath}\" \"{outputFilePath}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                WorkingDirectory = Path.GetDirectoryName(pythonScriptPath) // 작업 디렉토리 설정
            };

            pythonProcess = new Process
            {
                StartInfo = psi,
                EnableRaisingEvents = true
            };

            pythonProcess.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Debug.Log("[PYTHON OUT] " + e.Data);
                }
            };

            pythonProcess.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Debug.LogError("[PYTHON ERR] " + e.Data);
                }
            };

            pythonProcess.Exited += (sender, e) =>
            {
                Debug.Log("[3. 파이썬 실행] 프로세스 종료 감지됨");

                try
                {
                    if (File.Exists(outputFilePath))
                    {
                        string contents = File.ReadAllText(outputFilePath);
                        Debug.Log("[4. 출력 감지] AI 예측 결과 읽음: " + contents);
                    }
                    else
                    {
                        Debug.LogWarning("[WARN] 결과 파일을 찾을 수 없음: " + outputFilePath);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("[ERROR] 결과 파일 읽기 실패: " + ex.Message);
                }
            };

            bool started = pythonProcess.Start();
            if (started)
            {
                Debug.Log("[3. 파이썬 실행] 프로세스 시작 성공");
                pythonProcess.BeginOutputReadLine();
                pythonProcess.BeginErrorReadLine();
            }
            else
            {
                Debug.LogError("[ERROR] 파이썬 프로세스 시작 실패");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("[ERROR] 파이썬 스크립트 실행 중 예외 발생: " + ex.Message);
        }
    }

    private void CheckOutputFileChanged()
    {
        if (!File.Exists(outputFilePath)) return;

        DateTime writeTime = File.GetLastWriteTime(outputFilePath);
        if (writeTime <= lastOutputWriteTime) return;

        lastOutputWriteTime = writeTime;
        Debug.Log("[4. 출력 감지] AI 예측 결과 파일 변경 감지됨");
        string[] lines = File.ReadAllLines(outputFilePath);

        if (lines.Length > 0)
        {
            string lastLine = lines[lines.Length - 1];
            var parts = lastLine.Split(',');

            if (parts.Length >= 2 &&
                float.TryParse(parts[0], out float x) &&
                float.TryParse(parts[1], out float y))
            {
                Vector3 newPos = new Vector3(x, y, 0f);
                GameManager.Instance.SetExternalPosition(newPos);
            }
        }
    }

    private void OnDestroy()
    {
        if (pythonProcess != null)
        {
            if (!pythonProcess.HasExited)
            {
                pythonProcess.Kill();
            }
            pythonProcess.Dispose();
            pythonProcess = null;
        }
    }
}



