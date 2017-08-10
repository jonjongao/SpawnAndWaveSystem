# Spawn and Wave System #

# Overall Pipeline #
- **EnemyWaveManager**啟用Auto Start和Auto Proceed
	- *![](http://i.imgur.com/2kTE0nc.png)
- **OnWaveStart**加入事件關閉下一波等待時間UI
	- *![](http://i.imgur.com/jW9CytP.png)
- **OnWaveClear**加入事件開啟下一波等待時間UI
	- *![](http://i.imgur.com/mmUvMMX.png)
- **OnWaveProceed**加入事件將倒數int或string傳給UI
	- *![](http://i.imgur.com/DX8N2HT.png)
- **OnAllWaveClear**加入事件將下一個房間的Manager觸發
- **OnAllWaveClear**加入另一個事件給該Manager的**Portal**指定玩家物件以觸發玩家傳送到下一個房間的Function
	- *![](http://i.imgur.com/CIfbtt4.png)
- **Portal**設定好傳送目的地, 等待時間和過場時間
- **OnTravelBegin**加入事件觸發預製好的過場效果:**Shutter.ShutterEffect.BeginEffect()**百葉窗效果
- **OnTrabelEnd**加入事件結束過場效果**EndEffect()**
- **IntEvent**加入事件將倒數int傳給UI:**CountDown.FlipEffect.BeginEffect()**
	- *![](http://i.imgur.com/Nii1sHp.png)

# Effect Setup #
- **FlipEffect**設定要顯示的數字範圍和動畫時間
- **ShutterEffect**設定動畫時間, 動畫時間數值為單次播放時間, e.g. 設定0.5, 淡入時間加上淡出時間共需1秒

# Enemy Wave Manager #
## Value ##
### State ###
- int **currentWaveIndex** *get;*
	- 當前序列索引
- enum **currentWaveState** *get;*
	- 當前序列狀態
### Setting ###
- bool **autoStart** *get; set;*
	- *true* 自動從第一波開始
- bool **autoProceed** *get; set;*
	- *true* 當一波結束後會自動接續下一波
- int **waveDelay** *get; set;*
	- 每波序列的等待時間, 此參數會直接影響WaveProcessingCallback
- enum **separatedPattern** *get; set;*
	- 敵人數量分配在SpawnPoint內的邏輯
### Wave Event ###
- UnityEvent **onWaveStart** *get; set;*
	- 該序列開始生成時發送
- UnityEvent **onWaveFinish** *get; set;*
	- 該序列生成結束後發送
- UnityEvent **onWaveClear** *get; set;*
	- 該序列敵人被清光後發送
### Wave Processing Event ###
- UnityEvent &lt;int> **onWaveProceedInt** *get; set;*
- UnityEvent &lt;string> **onWaveProceedString** *get; set;*
	- 以上事件只在該序列接續時的等待時間發送, 事件分為傳送int和string兩種, 每秒發送一次, 作為在UI上的彈性應用
### Sequence Event ###
- UnityEvent **onAllWaveClear** *get; set;*
	- 在所有序列被清光後發送

## Function ##
- void **TimeNextWave** (int **time**)
	- 等待一段時間後接續到下一波序列
- void **StartWave** ()
	- 開始生成目前序列
- void **NowNextWave** ()
	- 立刻接續到下一波序列
- void **GoToAndStartWave** (int **index**)
	- 跳到指定序列並開始生成
- void **OnSpawnPointFinish** (SpawnPoint **point**)
	- SpawnPoint回報給EnemyWaveManager的端口