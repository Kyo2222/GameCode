import subprocess
import os
import sys

# ==============================
# 請修改這兩個路徑
# ==============================
UNITY_PATH = r"D:\Unity\2022.3.25f1\Editor\Unity.exe"
PROJECT_PATH = r"D:\Hsu_Unity\Test"
RESULT_PATH = r"C:\Users\drthx\Desktop\Tests\result.xml"
# ==============================

def run_tests():
    # 建立輸出資料夾
    os.makedirs(os.path.dirname(RESULT_PATH), exist_ok=True)

    print("▶ Unity Test Runner 啟動中...")
    print(f"  專案：{PROJECT_PATH}")
    print(f"  結果輸出：{RESULT_PATH}")
    print()

    cmd = [
        UNITY_PATH,
        "-runTests",
        "-batchmode",
        "-projectPath", PROJECT_PATH,
        "-testResults", RESULT_PATH,
        "-testPlatform", "EditMode",
    ]

    # 執行 Unity（會等到跑完才繼續）
    result = subprocess.run(cmd, capture_output=True, text=True)

    print(f"Unity 結束，Return Code: {result.returncode}")
    return result.returncode

if __name__ == "__main__":
    code = run_tests()
    sys.exit(code)