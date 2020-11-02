using System;
using System.Collections.Generic;
using System.Reflection;
using System.Net;
using System.Diagnostics;
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
			Trace.TraceInformation( "起動ステージを活性化します。" );
			Trace.Indent();
			try
			{
				this.list進行文字列 = new List<string>();
				base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
				base.On活性化();
				Trace.TraceInformation( "起動ステージの活性化を完了しました。" );
			}
			finally
			{
				Trace.Unindent();
			}
		}
		public override void On非活性化()
		{
			Trace.TraceInformation( "起動ステージを非活性化します。" );
			Trace.Indent();
			try
			{
				this.list進行文字列 = null;
				if ( es != null )
				{
					if ( ( es.thDTXFileEnumerate != null ) && es.thDTXFileEnumerate.IsAlive )
					{
						Trace.TraceWarning( "リスト構築スレッドを強制停止します。" );
						es.thDTXFileEnumerate.Abort();
						es.thDTXFileEnumerate.Join();
					}
				}
				base.On非活性化();
				Trace.TraceInformation( "起動ステージの非活性化を完了しました。" );
			}
			finally
			{
				Trace.Unindent();
			}
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				this.tx背景 = TJAPlayer3.tテクスチャの生成( CSkin.Path( @"Graphics\1_Title\Background.png" ), false );
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				TJAPlayer3.t安全にDisposeする( ref this.tx背景 );
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
			client.Headers.Add("User-Agent", "TJAPlayer3-Develop");

			return client.DownloadString(
				"https://api.github.com/repos/TJAPlayer3-Develop/TJAPlayer3-Develop/releases/latest");
		}

		public static GitHubRelease Deserialize(string releaseJson)
		{
			return JsonConvert.DeserializeObject<GitHubRelease>(releaseJson);
		}

		public void t読込開始()
		{
			this.list進行文字列.Add("TEST");

			es = new CEnumSongs();
			es.StartEnumFromCache(); // 曲リスト取得(別スレッドで実行される)
		}

		public override int On進行描画()
		{
			if( !base.b活性化してない )
			{
				if (startupState == 2)
                {
					TJAPlayer3.act文字コンソール.tPrint(1120, 688, C文字コンソール.Eフォント種別.白, "LOADING...");

					this.t読込開始();
                }

				TJAPlayer3.act文字コンソール.tPrintCenter(640, 70, C文字コンソール.Eフォント種別.白, "TJAPlayer3-Develop");
				TJAPlayer3.act文字コンソール.tPrintCenter(640, 95, C文字コンソール.Eフォント種別.白, "An open source Taiko no Tatsujin simulator");

				TJAPlayer3.act文字コンソール.tPrintCenter(640, 145, C文字コンソール.Eフォント種別.白, "https://github.com/TJAPlayer3-Develop/TJAPlayer3-Develop");

				TJAPlayer3.act文字コンソール.tPrintCenter(640, 550, C文字コンソール.Eフォント種別.白, "OFFICIAL DEVELOPMENT SERVER");
				TJAPlayer3.act文字コンソール.tPrintCenter(640, 575, C文字コンソール.Eフォント種別.白, "https://discord.gg/jN7tUk7");

				if (startupState < 1)
                {
					TJAPlayer3.act文字コンソール.tPrintCenter(640, 285, C文字コンソール.Eフォント種別.白, "Checking for Update...");
				}

				if (startupState == 0 && !base.b初めての進行描画)
				{
					startupState = 1;

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
							updateState = 1;
                        }
					}
					catch (Exception e)
					{
						updateState = -1;
					}
				}

				if ( base.b初めての進行描画 )
				{
					base.b初めての進行描画 = false;
				}

                if (startupState > 0)
                {
					if (updateState == 0)
                    {
						TJAPlayer3.act文字コンソール.tPrintCenter(640, 285, C文字コンソール.Eフォント種別.白, "You are using the latest version! :)");
					}
					else if (updateState == 1)
                    {
						TJAPlayer3.act文字コンソール.tPrintCenter(640, 285, C文字コンソール.Eフォント種別.白, "UPDATE FOUND!");
						TJAPlayer3.act文字コンソール.tPrintCenter(640, 335, C文字コンソール.Eフォント種別.白, "Download it at here:");
						TJAPlayer3.act文字コンソール.tPrintCenter(640, 360, C文字コンソール.Eフォント種別.白, "http://bit.ly/tjap3dev-update");
					}
					else
                    {
						TJAPlayer3.act文字コンソール.tPrintCenter(640, 285, C文字コンソール.Eフォント種別.白, "Checking for Update...");
						TJAPlayer3.act文字コンソール.tPrintCenter(640, 335, C文字コンソール.Eフォント種別.白, "Error while checking for updates :(");
					}
					
					if (startupState == 1) startupState = 2;
                }

				#region [ this.str現在進行中 の決定 ]
				//-----------------
				switch ( base.eフェーズID )
				{
					case CStage.Eフェーズ.起動7_完了:
                        TJAPlayer3.Tx.LoadTexture();
                        break;
				}
				//-----------------
				#endregion

				if( es != null && es.IsSongListEnumCompletelyDone )							// 曲リスト作成が終わったら
				{
					TJAPlayer3.Songs管理 = ( es != null ) ? es.Songs管理 : null;		// 最後に、曲リストを拾い上げる
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

		private int updateState = 0;

		private int startupState = 0;

#if false
		private void t曲リストの構築()
		{
			// ！注意！
			// 本メソッドは別スレッドで動作するが、プラグイン側でカレントディレクトリを変更しても大丈夫なように、
			// すべてのファイルアクセスは「絶対パス」で行うこと。(2010.9.16)

			DateTime now = DateTime.Now;
			string strPathSongsDB = CDTXMania.strEXEのあるフォルダ + "songs.db";
			string strPathSongList = CDTXMania.strEXEのあるフォルダ + "songlist.db";

			try
			{
		#region [ 0) システムサウンドの構築  ]
				//-----------------------------
				base.eフェーズID = CStage.Eフェーズ.起動0_システムサウンドを構築;

				Trace.TraceInformation( "0) システムサウンドを構築します。" );
				Trace.Indent();

				try
				{
					for( int i = 0; i < CDTXMania.Skin.nシステムサウンド数; i++ )
					{
						CSkin.Cシステムサウンド cシステムサウンド = CDTXMania.Skin[ i ];
						if( !CDTXMania.bコンパクトモード || cシステムサウンド.bCompact対象 )
						{
							try
							{
								cシステムサウンド.t読み込み();
								Trace.TraceInformation( "システムサウンドを読み込みました。({0})", new object[] { cシステムサウンド.strファイル名 } );
								if( ( cシステムサウンド == CDTXMania.Skin.bgm起動画面 ) && cシステムサウンド.b読み込み成功 )
								{
									cシステムサウンド.t再生する();
								}
							}
							catch( FileNotFoundException )
							{
								Trace.TraceWarning( "システムサウンドが存在しません。({0})", new object[] { cシステムサウンド.strファイル名 } );
							}
							catch( Exception exception )
							{
								Trace.TraceError( exception.Message );
								Trace.TraceWarning( "システムサウンドの読み込みに失敗しました。({0})", new object[] { cシステムサウンド.strファイル名 } );
							}
						}
					}
					lock( this.list進行文字列 )
					{
						this.list進行文字列.Add( "Loading system sounds ... OK " );
					}
				}
				finally
				{
					Trace.Unindent();
				}
				//-----------------------------
		#endregion

				if( CDTXMania.bコンパクトモード )
				{
					Trace.TraceInformation( "コンパクトモードなので残りの起動処理は省略します。" );
					return;
				}

		#region [ 00) songlist.dbの読み込みによる曲リストの構築  ]
				//-----------------------------
				base.eフェーズID = CStage.Eフェーズ.起動00_songlistから曲リストを作成する;

				Trace.TraceInformation( "1) songlist.dbを読み込みます。" );
				Trace.Indent();

				try
				{
					if ( !CDTXMania.ConfigIni.bConfigIniがないかDTXManiaのバージョンが異なる )
					{
						try
						{
							CDTXMania.Songs管理.tSongListDBを読み込む( strPathSongList );
						}
						catch
						{
							Trace.TraceError( "songlist.db の読み込みに失敗しました。" );
						}

						int scores = ( CDTXMania.Songs管理 == null ) ? 0 : CDTXMania.Songs管理.n検索されたスコア数;		// 読み込み途中でアプリ終了した場合など、CDTXMania.Songs管理 がnullの場合があるので注意
						Trace.TraceInformation( "songlist.db の読み込みを完了しました。[{0}スコア]", scores );
						lock ( this.list進行文字列 )
						{
							this.list進行文字列.Add( "Loading songlist.db ... OK" );
						}
					}
					else
					{
						Trace.TraceInformation( "初回の起動であるかまたはDTXManiaのバージョンが上がったため、songlist.db の読み込みをスキップします。" );
						lock ( this.list進行文字列 )
						{
							this.list進行文字列.Add( "Loading songlist.db ... Skip" );
						}
					}
				}
				finally
				{
					Trace.Unindent();
				}

		#endregion

		#region [ 1) songs.db の読み込み ]
				//-----------------------------
				base.eフェーズID = CStage.Eフェーズ.起動1_SongsDBからスコアキャッシュを構築;

				Trace.TraceInformation( "2) songs.db を読み込みます。" );
				Trace.Indent();

				try
				{
					if ( !CDTXMania.ConfigIni.bConfigIniがないかDTXManiaのバージョンが異なる )
					{
						try
						{
							CDTXMania.Songs管理.tSongsDBを読み込む( strPathSongsDB );
						}
						catch
						{
							Trace.TraceError( "songs.db の読み込みに失敗しました。" );
						}

						int scores = ( CDTXMania.Songs管理 == null ) ? 0 : CDTXMania.Songs管理.nSongsDBから取得できたスコア数;	// 読み込み途中でアプリ終了した場合など、CDTXMania.Songs管理 がnullの場合があるので注意
						Trace.TraceInformation( "songs.db の読み込みを完了しました。[{0}スコア]", scores );
						lock ( this.list進行文字列 )
						{
							this.list進行文字列.Add( "Loading songs.db ... OK" );
						}
					}
					else
					{
						Trace.TraceInformation( "初回の起動であるかまたはDTXManiaのバージョンが上がったため、songs.db の読み込みをスキップします。" );
						lock ( this.list進行文字列 )
						{
							this.list進行文字列.Add( "Loading songs.db ... Skip" );
						}
					}
				}
				finally
				{
					Trace.Unindent();
				}
				//-----------------------------
		#endregion

			}
			finally
			{
				base.eフェーズID = CStage.Eフェーズ.起動7_完了;
				TimeSpan span = (TimeSpan) ( DateTime.Now - now );
				Trace.TraceInformation( "起動所要時間: {0}", new object[] { span.ToString() } );
			}
		}
#endif
		#endregion

		#region Gets the build date and time (by reading the COFF header)

		// http://msdn.microsoft.com/en-us/library/ms680313

		struct _IMAGE_FILE_HEADER
		{
			public ushort Machine;
			public ushort NumberOfSections;
			public uint TimeDateStamp;
			public uint PointerToSymbolTable;
			public uint NumberOfSymbols;
			public ushort SizeOfOptionalHeader;
			public ushort Characteristics;
		};

		#endregion
	}
}
