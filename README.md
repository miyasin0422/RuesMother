# RuesMother（ルーマザー）

**少女ルーが、相棒レイとともに母親を探す2D横スクロールアクションゲーム。**

UnityとC#を使用し、プログラマ2人・デザイナ1人の3人チームで開発しています。ダッシュ回避を交えた戦闘と、敵から得た遺伝子を組み合わせる武器カスタマイズが特徴です。

**2026年10月31日、東京ゲームダンジョン14に出展。ボス戦まで遊べる体験版を制作しました。**

## ゲームをプレイ

### ▶ [ブラウザで体験版をプレイする（unityroom）](https://unityroom.com/games/ruesmother-demo)

インストール不要で遊べます。操作方法はゲーム内の案内をご覧ください。

## ゲーム概要

| 項目 | 内容 |
| --- | --- |
| ジャンル | 2D横スクロールアクション |
| プレイ人数 | 1人 |
| 開発体制 | プログラマ2人・デザイナ1人 |
| 使用技術 | Unity / C# |
| 公開形式 | ブラウザ向け体験版（unityroom） |
| 開発状況 | 体験版公開後も改善を継続中 |

## ゲームの特徴

- **スピード感のあるアクション**：移動・ジャンプ・ダッシュ回避を使い分けてステージを攻略します。
- **多彩な攻撃方法**：剣の近接攻撃、弓や投擲ナイフによる遠距離攻撃を使い分けられます。
- **遺伝子による武器カスタマイズ**：素材の組み合わせで武器の種類や性能、酸などの属性効果が変化します。
- **行動の異なる敵やボス**：地上・空中の敵や、ジャンプ・舌・衝撃波を使うカエル型ボスと戦います。

## こだわったシステム① 敵キャラクター制作（みやしん担当）

### 共通処理と敵固有の挙動を分ける設計

地上を移動する敵、空中を飛ぶ敵、カエル型ボスを制作しました。行動判断・移動・攻撃・HP管理を分離し、敵ごとの違いを組み合わせで表現しています。

- **ステートパターンによる行動の分離**：`EnemyState`を基底クラスとして巡回・追跡・攻撃を実装。`EnemyController`が`Enter`・`UpdateState`・`FixedUpdateState`・`Exit`を呼び出し、`BasicEnemyAI`が各状態と遷移先を組み立てます。状態の切り替えと、各状態で行う処理を分けています。
- **移動方法の差し替え**：抽象クラス`EnemyMovement`に巡回・追跡・停止・向き変更の共通操作を定義。`GroundEnemyMovement`と`SkyEnemyMovement`がそれぞれ実装するため、通常敵の状態処理を共有したまま移動方法を変えられます。
- **攻撃方法の差し替え**：`EnemyAttackController`が予備動作・攻撃生成・クールタイムを管理し、生成した`EnemyAttack`へ攻撃力と目標座標を渡します。近接攻撃や飛び道具を個別の実装に分けています。
- **HP・被ダメージ処理の共通化**：`EnemyHealth`にHP、ダメージ、死亡、アイテムドロップ、酸状態の継続ダメージを集約。HP変更・死亡のイベントも公開し、HP表示などから参照できる構成にしています。

この構成により、行動の流れを保ちながら移動や攻撃を変更でき、複数の敵で共通処理を再利用できます。

参照コード：[EnemyController](Assets/Scripts/Enemy/EnemyController.cs) / [EnemyStates](Assets/Scripts/Enemy/EnemyStates.cs) / [BasicEnemyAI](Assets/Scripts/Enemy/BasicEnemyAI.cs) / [EnemyMovement](Assets/Scripts/Enemy/EnemyMovement.cs) / [EnemyAttackController](Assets/Scripts/Enemy/EnemyAttackController.cs) / [EnemyHealth](Assets/Scripts/Enemy/EnemyHealth.cs)

### カエル型ボスと、逃げ道を残すための工夫

![カエル型ボスとの戦闘](docs/images/boss.gif)

ボスはプレイヤーに向かってジャンプし、設定回数のジャンプ後に、設定した確率で次の2種類の攻撃を選びます。

- **舌攻撃**：プレイヤーの位置を狙う遠距離攻撃。
- **踏みつけ攻撃**：プレイヤーを狙って跳び、着地時に攻撃判定と左右へ進む衝撃波を生成。

移動・攻撃選択・舌攻撃・踏みつけをそれぞれ状態として管理し、攻撃の予備動作や硬直は`FrogBossAttackController`で処理しています。

開発中には、ボスがルーを壁際に追い詰めたり、ルーの上に乗って停止したりして、身動きが取れなくなる問題がありました。対策として、**回避中だけ敵との衝突を除外する処理**と、**ボスがルーの上で停止した際に部屋の中央方向へ退避する処理**を実装しました。

退避処理では、接触点がボスの足元にあること、ルーが下側にいること、縦方向の速度が小さい状態が一定時間続いたことを確認します。退避開始後は着地まで横移動を続け、縦速度を保って自然に落下させます。中央付近ではルーから離れる側を選びます。

単に停止を解消するだけでなく、プレイヤーが回避で切り抜けられる余地を残し、理不尽に行動を封じられない戦闘を目指しました。

参照コード：[FrogBossAI](Assets/Scripts/Enemy/FrogBossAI.cs) / [FrogBossStates](Assets/Scripts/Enemy/FrogBossStates.cs) / [FrogBossAttackController](Assets/Scripts/Enemy/FrogBossAttackController.cs) / [FrogBossUnstuck](Assets/Scripts/Enemy/FrogBossUnstuck.cs) / [PlayerControll](Assets/Scripts/Player/PlayerControll.cs)

## こだわったシステム② 武器カスタマイズ機能（なす担当）

![遺伝子を組み合わせる武器カスタマイズ画面](docs/images/weapon-custom.gif)

敵から得た遺伝子をスロットへ配置し、武器を作成・装備する、本作の中心となるシステムです。例えば、剣と弓の遺伝子を組み合わせると投擲ナイフになり、酸の遺伝子を加えると酸状態の付与確率が増加します。

### データ定義・計算・装備・UIの役割分担

- **ScriptableObjectで素材とレシピを定義**：`GeneSO`に属性・攻撃力・射程などの補正値、UI用Prefab、説明文を保持。`WeaponRecipeSO`には必要なベース遺伝子と対応する武器Prefabを定義し、素材データと計算処理を分けています。
- **UIと共通の計算処理でプレビューを作成**：`CraftingCalculator.ProcessCrafting`がレシピ判定、性能計算、名前・説明文の生成を担当。プレビューと作成確定の両方から同じ処理を呼び、計算方法を共通化しています。
- **組み合わせの種類と個数を性能へ反映**：遺伝子の補正値を加算し、同種のベース遺伝子3個以上では特化ボーナスを適用。現在の素材設定では剣が攻撃力、弓が射程を増加させます。酸の付与確率は1個につき25%加算し、上限を100%に制限しています。
- **武器の挙動と個別性能を分離**：`EquippedWeaponData`に既存の武器Prefab参照、計算済み性能、名前・説明文、使用遺伝子をまとめ、`Gene.PlayerLoadout`が左右の装備を保持します。既存Prefabと個別の性能データを組み合わせる方式です。

### 組み直し時の素材管理と操作UI

`CraftingManager`は、変更前後の遺伝子を種類ごとに数えて差分を比較し、追加分を消費、取り外した分を返還します。装備切り替え時には、保存した使用遺伝子からスロットの構成を復元します。

`CreateUI`では所持数・スロット配置中の数・反対側の装備で使用中の数を考慮し、利用可能な素材数を表示します。ドラッグ＆ドロップ、右クリックでの取り外し、武器名と説明文のプレビューにより、組み合わせを確認しながらカスタマイズできます。

カスタマイズ結果を実際の攻撃へ反映する部分は、プレイヤー・攻撃側の処理と連携しています。生成した武器の`AttackBehavior`へ性能を渡し、当たり判定や弾にも引き継ぐ構成です。

参照コード：[GeneSO](Assets/Nasu/GeneSO/GeneSO.cs) / [WeaponRecipeSO](Assets/Nasu/WeaponRecipeSO/WeaponRecipeSO.cs) / [CraftingCalculator](Assets/Nasu/CraftingCalculator.cs) / [CraftingManager](Assets/Nasu/CraftingManager.cs) / [EquippedWeaponData](Assets/Nasu/EquipedWeaponData.cs) / [PlayerLoadout](Assets/Nasu/PlayerLoadout.cs) / [CreateUI](Assets/Nasu/CreateUI.cs) / [GeneDropSlot](Assets/Nasu/GeneDropSlot.cs)

攻撃側の連携：[AttackBehavior](Assets/Scripts/Player/Attack/AttackBehavior.cs) / [AttackHitbox](Assets/Scripts/Player/Attack/AttackHitbox.cs)

## 東京ゲームダンジョン14への出展

![東京ゲームダンジョン14の出展ブース](docs/images/game-dungeon14.JPG)

**2026年10月31日開催の「東京ゲームダンジョン14」に出展しました。** 出展を目標にボス戦までプレイ可能な体験版を制作し、当日は約30人の方にプレイしていただきました。

遺伝子を組み合わせる武器カスタマイズ、ダッシュ回避などの操作感、世界観や演出に好評をいただきました。一方で、チュートリアルや操作説明の分かりにくさ、UIの操作性、敵の攻撃の視認性が改善点として挙がりました。

試遊で得られたフィードバックをもとに、初めて遊ぶ方にも操作や戦闘の意図が伝わるよう、現在も改善を進めています。

## 開発メンバー

プログラマ2人とデザイナ1人で役割を分担し、各機能を連携させながらチームで制作しています。以下は主な担当範囲です。

| メンバー | 主な担当 | リンク |
| --- | --- | --- |
| みやしん | プレイヤー・敵キャラクター制御、物理挙動、ゲームデザイン、イベント応募・企画 | [GitHub](https://github.com/miyasin0422) |
| なす | 武器カスタマイズ、遺伝子システム、データ管理 | [GitHub](https://github.com/nasu-cell) |
| おちょくりぷす | キャラクター・背景などのデザイン、アニメーション、世界観 | [X](https://x.com/koutasama07) |

## 開発環境・起動方法

| 項目 | 使用環境 |
| --- | --- |
| ゲームエンジン | Unity 6000.3.18f1 |
| 言語 | C# |
| 描画 | Universal Render Pipeline（URP） |
| 入力 | Input System |
| UI | uGUI / TextMeshPro |
| バージョン管理 | Git / GitHub（画像・音声などにGit LFSを使用） |

### Unity Editorから起動

1. Git LFSを導入した環境で、このリポジトリをクローンします。画像・音声が取得されていない場合は`git lfs pull`を実行してください。
2. Unity Hubの「追加」から、`Assets`・`Packages`・`ProjectSettings`があるプロジェクトのルートフォルダを選びます。
3. Unity **6000.3.18f1**で開き、パッケージの解決とインポートが完了するまで待ちます。
4. [`Assets/Scenes/Title.unity`](Assets/Scenes/Title.unity)を開き、Unity Editorの「Play」を押します。
5. タイトル画面の「プレイ」からゲームを開始します。

ブラウザ向けにビルドする場合は、Unity HubでWeb Build Supportを追加してください。開発中のため、仕様や操作方法は変更される場合があります。

## 今後の開発

- チュートリアルと操作説明の改善
- 武器カスタマイズを含むUIの操作性向上
- 敵の攻撃を把握しやすくする表示・演出の改善
- 試遊で得た意見を踏まえた、操作感と戦闘バランスの継続的な調整

お問い合わせ：[sota150422@gmail.com](mailto:sota150422@gmail.com)

© RuesMother。本リポジトリに含まれるゲームデータ・画像・プログラム等の権利は、各権利者に帰属します。
