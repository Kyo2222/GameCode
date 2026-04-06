## SideScrollingCode
 
ポイント：**スキルシステム**、**敵 AI**、**オブジェクトプール管理**
 
### スキルシステム
 
すべてのスキルは `Skill` 基底クラスを継承し、`SkillManager`（Singleton）を通じて一元アクセスします。各スキルは段階的なアンロックに対応しています。
 
```csharp
SkillManager.Instance.Clone.CreateClone(transform, offset);
SkillManager.Instance.CrystalSkill.CanUseSkill();
```
 
実装済みスキル：Dash、Clone（分身）、Crystal（クリスタル）、Blackhole（ブラックホール）、ThrowSword（投げ剣・4種類）、Parry（パリィ）、Dodge（回避）。
 
### 敵 AI
 
すべての敵は `Enemy` を継承し、**ステートマシン**で行動を管理します。敵の種類ごとに独立したステートセットを持ちます。
 
| 敵 | 特徴的な行動 |
|------|----------|
| Skeleton | 標準的な近接ステートマシン |
| Archer | ジャンプ回避、地面／壁の検知 |
| Slime | 死亡時に子スライムへ分裂（Big → Medium → Small） |
| DeathBringer（ボス） | ランダムテレポート、呪文詠唱、動的な再配置 |
 
### オブジェクトプール管理
 
`ObjPoolManager` がすべての動的生成オブジェクトを一元管理し、頻繁な Instantiate/Destroy による GC 負荷を回避します。
 
```csharp
// 取得
ObjPoolManager.Instance.GetClone<SkillPoolType>((int)SkillPoolType.Clone, position);
// 返却
ObjPoolManager.Instance.ReleaseClone<SkillPoolType>((int)SkillPoolType.Clone, obj);
```
 
Skill、Element、Item、EquipmentEffect、FX など複数のプール種別を Enum をキーとして管理しています。
 
---
 
## FPSGameCode
 
ポイント：**IDamagable インターフェース**、**リコイルシステム**、**スコープシステム**
 
### IDamagable インターフェース
 
ゾンビ（`ZombieBaseControllor`）とプレイヤーの両方が `IDamagable` を実装しており、射撃ロジックや AI の攻撃ロジックがターゲットの具体的な型を知らずにダメージを与えられます。
 
```csharp
var target = hit.collider.GetComponent<IDamagable>();
if (target != null)
    target.TakeDamage(damagePerShot);
```
 
### Recoil リコイルシステム
 
発砲のたびに `targetRecoil` へランダムな X/Y/Z 回転を加算し、`Vector3.Lerp` で元の位置へ滑らかに戻します。`maxRecoil` で上限を制限して過度な偏転を防ぎ、発砲ボタンを離した瞬間に即リセットすることでリアルな射撃感を再現しています。
 
### ScopeSystem スコープシステム
 
`mainCamera` と `scopeCamera` のデュアルカメラ構成で切り替えます。右クリックでスコープに入ると、メインカメラと HUD を非表示にして Scope UI を表示し、マウスホイールで FOV を動的に調整できます（10°〜80°）。
 
---
 
## 2.5D PRGCode
 
ポイント：**クエストシステム**
 
クエストシステムは **ScriptableObject + 実行時状態の分離** を採用しています。クエスト定義（目標・報酬）は `Quest` アセットに保存し、プレイヤーの進捗は `QuestStatus` がメモリ上で管理し、`ISaveable` インターフェースを通じてセーブデータへ永続化します。
 
`QuestList` はプレイヤーオブジェクトにアタッチし、**条件付き自動完了**をサポートします。目標に `Condition` を設定すると毎フレーム自動評価されるため、手動呼び出しが不要です。
 
```csharp
questList.AddQuest(quest);                         // クエスト受注
questList.CompleteObjective(quest, "objectiveRef"); // 手動で目標完了
// Condition が設定された目標は Update 内で自動完了
```
 
UI 層（`QuestListUI`）は `onUpdate` イベントを購読してリフレッシュし、クエスト詳細はツールチップで各目標の完了状態と報酬を表示します。
 
---
 
## MVC｜拡張性を重視した MVC アーキテクチャ
 
ポイント：**高い拡張性**
 
厳格な MVC 分層を採用しており、Model はイベントでのみ外部へ通知し、View はイベントを購読するだけ、Controller だけが Model のメソッドを呼び出せます。`GameContext`（Singleton）がグローバルな Model レジストリとして機能し、どのシステムからもオブジェクト参照なしに Model へアクセスできます。
 
```csharp
// どこからでもアクセス可能、Inspector での参照ドラッグ不要
GameContext.Instance.Character.CurrentHp
```
 
**新しいシステムの追加は3ステップで完了：**
1. `XxxModel` クラスを作成（純粋な C#、Unity 非依存）
2. `GameContext` にプロパティと `Register` メソッドを追加
3. 対応する Controller の `Awake()` で生成して登録
 
```csharp
// GameContext.cs への追加例
public InventoryModel Inventory { get; private set; }
public void RegisterInventory(InventoryModel model) { Inventory = model; }
```
 
これにより `HealerController` や `EnemyHudView` などのシステムが独立して動作でき、`GameContext` から必要な Model を取得してイベントを購読するだけで、お互いが完全に疎結合になります。

---
 
# Unity テスト自動化ツール
 
PythonでUnityのTest Runnerを自動実行し、結果をレポート出力するデモです。
 
## ファイル構成
 
| ファイル | 説明 |
|---|---|
| `run_tests.py` | テストを自動実行する |
| `parse_results.py` | 結果を解析してレポートを出力する |
| `FirstTest.cs` | EditModeテストのサンプル |
 
## 使い方
 
> ※ 実行前にUnity Editorを閉じてください。
 
```bash
python run_tests.py
python parse_results.py
```
 
## 動作環境
 
- Unity 2022.3.x
- Python 3.x
