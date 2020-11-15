using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using FDK;
using Newtonsoft.Json;


namespace TJAPlayer3
{
	internal class CStage起動 : CStage
	{
		// コンストラクタ

		public CStage起動()
		{
			base.eステージID = CStage.Eステージ.起動;
			base.b活性化してない = true;
		}

		public List<string> list進行文字列;

		// CStage 実装

		public override void On活性化()
		{
			Trace.TraceInformation("起動ステージを活性化します。");
			Trace.Indent();
			try
			{
				this.list進行文字列 = new List<string>();
				base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
				base.On活性化();
				Trace.TraceInformation("起動ステージの活性化を完了しました。");
			}
			finally
			{
				Trace.Unindent();
			}
		}
		public override void On非活性化()
		{
			Trace.TraceInformation("起動ステージを非活性化します。");
			Trace.Indent();
			try
			{
				this.list進行文字列 = null;
				if (es != null)
				{
					if ((es.thDTXFileEnumerate != null) && es.thDTXFileEnumerate.IsAlive)
					{
						Trace.TraceWarning("リスト構築スレッドを強制停止します。");
						es.thDTXFileEnumerate.Abort();
						es.thDTXFileEnumerate.Join();
					}
				}
				base.On非活性化();
				Trace.TraceInformation("起動ステージの非活性化を完了しました。");
			}
			finally
			{
				Trace.Unindent();
			}
		}
		public override void OnManagedリソースの作成()
		{
			if (!base.b活性化してない)
			{
				this.tx背景 = TJAPlayer3.tテクスチャの生成(CSkin.Path(@"Graphics\1_Title\Background.png"), false);
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if (!base.b活性化してない)
			{
				TJAPlayer3.t安全にDisposeする(ref this.tx背景);
				base.OnManagedリソースの解放();
			}
		}

		private sealed class WebClientWithTimeout : WebClient
		{
			private readonly TimeSpan _timeout;

			public WebClientWithTimeout(TimeSpan timeout)
			{
				_timeout = timeout;
			}

			protected override WebRequest GetWebRequest(Uri address)
			{
				var webRequest = base.GetWebRequest(address);
				webRequest.Timeout = (int)_timeout.TotalMilliseconds;
				return webRequest;
			}
		}
		public sealed class GitHubRelease
		{
			public string Name { get; set; }

			[JsonProperty("name")]
			public string ReleaseName { get; set; }
		}

		private static string GetLatestReleaseJson()
		{
			var client = new WebClientWithTimeout(TimeSpan.FromSeconds(2));
			client.Headers.Add("User-Agent", "TJAPlayer3-Develop/TJAPlayer3-Develop");

			return client.DownloadString(
				"https://api.github.com/repos/TJAPlayer3-Develop/TJAPlayer3-Develop/releases/latest");
		}

		public static GitHubRelease Deserialize(string releaseJson)
		{
			return JsonConvert.DeserializeObject<GitHubRelease>(releaseJson);
		}

		public override int On進行描画()
		{
			if (!base.b活性化してない)
			{
				if (base.b初めての進行描画)
				{
					this.list進行文字列.Add(TJAPlayer3.AppDisplayName);
					this.list進行文字列.Add("An open source Taiko no Tatsujin simulator");
					this.list進行文字列.Add("https://github.com/TJAPlayer3-Develop/TJAPlayer3-Develop");
					this.list進行文字列.Add("");
					this.list進行文字列.Add("OFFICIAL DISCORD SERVER:");
					this.list進行文字列.Add("https://discord.gg/jN7tUk7");
					this.list進行文字列.Add("");
					this.list進行文字列.Add("");
					this.list進行文字列.Add("Checking for Updates...");
					try
					{
						var release = Deserialize(GetLatestReleaseJson());

						var version = Assembly.GetExecutingAssembly().GetName().Version;
						var buildDateTime = new DateTime(2000, 1, 1).Add(new TimeSpan(
						TimeSpan.TicksPerDay * version.Build + // days since 1 January 2000
						TimeSpan.TicksPerSecond * 2 * version.Revision)); // seconds since midnight, (multiply by 2 to get original)

						var latestDateTime = DateTime.ParseExact(release.ReleaseName.Replace("Release on ", ""), "MM/dd/yyyy HH:mm:ss", null).AddMinutes(-1.5);

						if (latestDateTime.CompareTo(buildDateTime) > 0)
						{
							this.list進行文字列.Add("UPDATE FOUND!");
							this.list進行文字列.Add("Download it here: http://bit.ly/tjap3dev-update");
						} else this.list進行文字列.Add("You are using the latest version! :)");
					}
					catch
					{
						this.list進行文字列.Add("Error while checking for updates :(");
					}
					this.list進行文字列.Add("");
					this.list進行文字列.Add("");
					this.list進行文字列.Add("Now initializing:");
					es = new CEnumSongs();
					es.StartEnumFromCache();                                        // 曲リスト取得(別スレッドで実行される)
					base.b初めての進行描画 = false;
					return 0;
				}

				// CSongs管理 s管理 = CDTXMania.Songs管理;

				//if( this.tx背景 != null )
				//	this.tx背景.t2D描画( CDTXMania.app.Device, 0, 0 );

				#region [ this.str現在進行中 の決定 ]
				//-----------------
				switch (base.eフェーズID)
				{
					case CStage.Eフェーズ.起動0_システムサウンドを構築:
						this.str現在進行中 = "SYSTEM SOUND...";
						break;

					case CStage.Eフェーズ.起動00_songlistから曲リストを作成する:
						this.str現在進行中 = "SONG LIST...";
						break;

					case CStage.Eフェーズ.起動1_SongsDBからスコアキャッシュを構築:
						this.str現在進行中 = "SONG DATABASE...";
						break;

					case CStage.Eフェーズ.起動2_曲を検索してリストを作成する:
						this.str現在進行中 = string.Format("{0} ... {1}", "Enumerating songs", es.Songs管理.n検索されたスコア数);
						break;

					case CStage.Eフェーズ.起動3_スコアキャッシュをリストに反映する:
						this.str現在進行中 = string.Format("{0} ... {1}/{2}", "Loading score properties from songs.db", es.Songs管理.nスコアキャッシュから反映できたスコア数, es.Songs管理.n検索されたスコア数);
						break;

					case CStage.Eフェーズ.起動4_スコアキャッシュになかった曲をファイルから読み込んで反映する:
						this.str現在進行中 = string.Format("{0} ... {1}/{2}", "Loading score properties from files", es.Songs管理.nファイルから反映できたスコア数, es.Songs管理.n検索されたスコア数 - es.Songs管理.nスコアキャッシュから反映できたスコア数);
						break;

					case CStage.Eフェーズ.起動5_曲リストへ後処理を適用する:
						this.str現在進行中 = string.Format("{0} ... ", "Building songlists");
						break;

					case CStage.Eフェーズ.起動6_スコアキャッシュをSongsDBに出力する:
						this.str現在進行中 = string.Format("{0} ... ", "Saving songs.db");
						break;

					case CStage.Eフェーズ.起動7_完了:
						TJAPlayer3.Tx.LoadTexture();
						this.list進行文字列.Add("LOADING TEXTURES...OK");
						this.list進行文字列.Add("");
						this.str現在進行中 = "Now entering the game...";
						break;
				}
				//-----------------
				#endregion
				#region [ this.list進行文字列＋this.現在進行中 の表示 ]
				//-----------------
				lock (this.list進行文字列)
				{
					int x = 640;
					int y = 60;
					foreach (string str in this.list進行文字列)
					{
						TJAPlayer3.act文字コンソール.tPrintCenter(x, y, C文字コンソール.Eフォント種別.白, str);
						y += 28;
					}
					TJAPlayer3.act文字コンソール.tPrintCenter(x, y, C文字コンソール.Eフォント種別.白, this.str現在進行中);
				}
				//-----------------
#endregion

				if (es != null && es.IsSongListEnumCompletelyDone)                          // 曲リスト作成が終わったら
				{
					TJAPlayer3.Songs管理 = (es != null) ? es.Songs管理 : null;      // 最後に、曲リストを拾い上げる
					return 1;
				}
			}
			return 0;
		}


		// その他

#region [ private ]
		//-----------------
		private string str現在進行中 = "";
		private CTexture tx背景;
		private CEnumSongs es;
#endregion
	}
}
