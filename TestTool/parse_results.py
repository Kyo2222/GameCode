import xml.etree.ElementTree as ET
import sys

RESULT_PATH = r"C:\Users\drthx\Desktop\Tests\result.xml"

def parse_results(xml_path):
    tree = ET.parse(xml_path)
    root = tree.getroot()

    # 整體結果
    total    = root.attrib.get("total", "0")
    passed   = root.attrib.get("passed", "0")
    failed   = root.attrib.get("failed", "0")
    skipped  = root.attrib.get("skipped", "0")
    duration = root.attrib.get("duration", "0")

    print("=" * 40)
    print("      Unity Test Runner 結果報告")
    print("=" * 40)
    print(f"  總計：   {total} 個測試")
    print(f"  ✅ 通過：{passed} 個")
    print(f"  ❌ 失敗：{failed} 個")
    print(f"  ⏭️  跳過：{skipped} 個")
    print(f"  ⏱️  時間：{float(duration):.2f} 秒")
    print("=" * 40)

    # 列出失敗的測試細節
    failed_cases = []
    for case in root.iter("test-case"):
        if case.attrib.get("result") == "Failed":
            failed_cases.append(case)

    if failed_cases:
        print("\n❌ 失敗的測試：")
        for case in failed_cases:
            name = case.attrib.get("fullname", "Unknown")
            message = case.find(".//message")
            trace = case.find(".//stack-trace")

            print(f"\n  【{name}】")
            if message is not None:
                print(f"  原因：{message.text.strip()}")
            if trace is not None:
                # 只顯示第一行
                first_line = trace.text.strip().split("\n")[0]
                print(f"  位置：{first_line}")
    else:
        print("\n🎉 全部測試通過！")

    print()
    return int(failed) == 0

if __name__ == "__main__":
    success = parse_results(RESULT_PATH)
    sys.exit(0 if success else 1)