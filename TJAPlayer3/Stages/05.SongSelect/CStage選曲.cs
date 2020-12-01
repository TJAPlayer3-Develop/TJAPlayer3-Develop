using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Threading;
using FDK;

namespace TJAPlayer3
{
    internal class CStage選曲 : CStage
    {
        // プロパティ
        public int nスクロールバー相対y座標
        {
            get
            {
                if (act曲リスト != null)
                {
                    return act曲リスト.nスクロールバー相対y座標;
                }
                else
                {
                    return 0;
                }
            }
        }
        public bool bIsEnumeratingSongs
        {
            get
            {
                return act曲リスト.bIsEnumeratingSongs;
            }
            set
            {
                act曲リスト.bIsEnumeratingSongs = value;
            }
        }
        public bool bスクロール中
        {
            get
            {
                return this.act曲リスト.bスクロール中;
            }
        }
        public int n確定された曲の難易度
        {
            get;
            private set;
        }
        public string str確定された曲のジャンル
        {
            get;
            private set;                
        }
		public Cスコア r確定されたスコア
		{
			get;
			private set;
		}
		public C曲リストノード r確定された曲 
		{
			get;
			private set;
		}
		public int n現在選択中の曲の難易度
		{
			get
			{
				return this.act曲リスト.n現在選択中の曲の現在の難易度レベル;
			}
		}
		public Cスコア r現在選択中のスコア
		{
			get
			{
				return this.act曲リスト.r現在選択中のスコア;
			}
		}
		public C曲リストノード r現在選択中の曲
		{
			get
			{
				return this.act曲リスト.r現在選択中の曲;
			}
		}

		// コンストラクタ
		public CStage選曲()
		{
			base.eステージID = CStage.Eステージ.選曲;
			base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
			base.b活性化してない = true;
			base.list子Activities.Add( this.actオプションパネル = new CActオプションパネル() );
            base.list子Activities.Add( this.actFIFO = new CActFIFOBlack() );
			base.list子Activities.Add( this.actFIfrom結果画面 = new CActFIFOBlack() );
			//base.list子Activities.Add( this.actFOtoNowLoading = new CActFIFOBlack() );
            base.list子Activities.Add( this.actFOtoNowLoading = new CActFIFOStart() );
			base.list子Activities.Add( this.act曲リスト = new CActSelect曲リスト() );
			base.list子Activities.Add( this.actPresound = new CActSelectPresound() );
			base.list子Activities.Add( this.actSortSongs = new CActSortSongs() );
			base.list子Activities.Add( this.actQuickConfig = new CActSelectQuickConfig() );
			base.list子Activities.Add( this.act難易度選択画面 = new CActSelect難易度選択画面() );

			this.CommandHistory = new CCommandHistory();		// #24063 2011.1.16 yyagi
		}
		
		
		// メソッド

		public void t選択曲変更通知()
		{
			if (this.act難易度選択画面.bIsDifficltSelect) this.act難易度選択画面.t難易度選択画面を閉じる();
			this.actPresound.t選択曲が変更された();

			#region [ プラグインにも通知する（BOX, RANDOM, BACK なら通知しない）]
			//---------------------
			if( TJAPlayer3.app != null )
			{
				var c曲リストノード = TJAPlayer3.stage選曲.r現在選択中の曲;
				var cスコア = TJAPlayer3.stage選曲.r現在選択中のスコア;

				if( c曲リストノード != null && cスコア != null && c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.SCORE )
				{
					string str選択曲ファイル名 = cスコア.ファイル情報.ファイルの絶対パス;
				    int n曲番号inブロック = TJAPlayer3.stage選曲.act曲リスト.n現在のアンカ難易度レベルに最も近い難易度レベルを返す( c曲リストノード );

					foreach( TJAPlayer3.STPlugin stPlugin in TJAPlayer3.app.listプラグイン )
					{
						Directory.SetCurrentDirectory( stPlugin.strプラグインフォルダ );
						stPlugin.plugin.On選択曲変更( str選択曲ファイル名, n曲番号inブロック );
						Directory.SetCurrentDirectory( TJAPlayer3.strEXEのあるフォルダ );
					}
				}
			}
			//---------------------
			#endregion
		}

        private void enterConfigStage()
        {
            TJAPlayer3.Skin.sound取消音.t再生する();
            this.actPresound.tサウンドの停止MT();
            this.eフェードアウト完了時の戻り値 = E戻り値.コンフィグ呼び出し;
            this.actFIFO.tフェードアウト開始();
            base.eフェーズID = CStage.Eフェーズ.共通_フェードアウト;
        }

        public void showQuickConfig()
        {
            TJAPlayer3.Skin.sound変更音.t再生する();
            this.actQuickConfig.tActivatePopupMenu( E楽器パート.DRUMS );
        }

		public void 制限時間音声のリセット(bool LessThan30Sec = false)
		{
			if (!LessThan30Sec) this.soundあと30秒.t再生を停止する();
			this.soundあと10秒.t再生を停止する();
			this.soundあと5秒.t再生を停止する();

			if (!LessThan30Sec) this.soundあと30秒.t再生位置を先頭に戻す();
			this.soundあと10秒.t再生位置を先頭に戻す();
			this.soundあと5秒.t再生位置を先頭に戻す();

			if (LessThan30Sec) this.suppress30sec = true;
			else this.suppress30sec = false;

			for (int i = 0; i < 10; i++) this.IsPlayed_pi[i] = false;
		}

		private void Timer_Plus()
        {
			if (ct制限時間.n現在の値 > 0 && ct制限時間.n現在の値 <= 100)
			{
				制限時間音声のリセット();
				ct制限時間.n現在の値--;
			}
		}

		private void Timer_Minus()
        {
			if (ct制限時間.n現在の値 >= 0 && ct制限時間.n現在の値 < 100)
			{
				制限時間音声のリセット();
				ct制限時間.n現在の値++;
			}
		}

		// CStage 実装

		/// <summary>
		/// 曲リストをリセットする
		/// </summary>
		/// <param name="cs"></param>
		public void Refresh( CSongs管理 cs, bool bRemakeSongTitleBar)
		{
			this.act曲リスト.Refresh( cs, bRemakeSongTitleBar );
		}

		public override void On活性化()
		{
			Trace.TraceInformation( "選曲ステージを活性化します。" );
			Trace.Indent();
			try
			{
				TJAPlayer3.Skin.bgm選曲画面In.t再生する();
				TJAPlayer3.Skin.soundSongSelectChara.t再生する();
				this.eフェードアウト完了時の戻り値 = E戻り値.継続;
				this.bBGM再生済み = false;
				this.ftフォント = new Font("MS UI Gothic", 26f, GraphicsUnit.Pixel );
				for( int i = 0; i < 6; i++ )
					this.ctキー反復用[ i ] = new CCounter( 0, 0, 0, TJAPlayer3.Timer );

				this.ctDonchanNormal = new CCounter(0, TJAPlayer3.Tx.SongSelect_Donchan_Normal.Length - 1, 1000 / 45, TJAPlayer3.Timer);
				this.ctDonchanSelect = new CCounter();
				this.ctDonchanStart = new CCounter();
				ctDiffSelect移動待ち = new CCounter();
				//this.act難易度選択画面.bIsDifficltSelect = true;
				this.ct制限時間 = new CCounter(0, 100, 1000, TJAPlayer3.Timer);
				this.suppress30sec = false;
				this.IsPlayed_pi = new bool[10];
				for (int i = 0; i < 10; i++) this.IsPlayed_pi[i] = false;
				this.ctOneCoin = new CCounter(0, 510, 4, TJAPlayer3.Timer);
				if (TJAPlayer3.ConfigIni.bStopTimerByDefault)
					ct制限時間.t停止();
				base.On活性化();

                // Discord Presenceの更新
                Discord.UpdatePresence("", Properties.Discord.Stage_SongSelect, TJAPlayer3.StartupTime);
            }
			finally
			{
                TJAPlayer3.ConfigIni.eScrollMode = EScrollMode.Normal;
                TJAPlayer3.ConfigIni.bスクロールモードを上書き = false;
                Trace.TraceInformation( "選曲ステージの活性化を完了しました。" );
				Trace.Unindent();
			}
		}
		public override void On非活性化()
		{
			Trace.TraceInformation( "選曲ステージを非活性化します。" );
			Trace.Indent();
			try
			{
				ct制限時間.n現在の値 = 0;
				if ( this.ftフォント != null )
				{
					this.ftフォント.Dispose();
					this.ftフォント = null;
				}
                for (int i = 0; i < 6; i++)
                {
                    this.ctキー反復用[i] = null;
                }
				base.On非活性化();
			}
			finally
			{
				Trace.TraceInformation( "選曲ステージの非活性化を完了しました。" );
				Trace.Unindent();
			}
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				//this.tx背景 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_background.jpg" ), false );
				//this.tx上部パネル = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_header_panel.png" ), false );
				//this.tx下部パネル = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_footer panel.png" ) );

				//this.txFLIP = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_skill number on gauge etc.png" ), false );
				//this.tx難易度名 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_difficulty name.png" ) );
				//this.txジャンル別背景[0] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_background_Anime.png" ) );
				//this.txジャンル別背景[1] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_background_JPOP.png" ) );
				//this.txジャンル別背景[2] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_background_Game.png" ) );
				//this.txジャンル別背景[3] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_background_Namco.png" ) );
				//this.txジャンル別背景[4] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_background_Classic.png" ) );
				//this.txジャンル別背景[5] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_background_Child.png" ) );
				//this.txジャンル別背景[6] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_background_Variety.png" ) );
				//this.txジャンル別背景[7] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_background_VOCALID.png" ) );
				//this.txジャンル別背景[8] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_background_Other.png" ) );

				//this.tx難易度別背景[0] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_background_Easy.png" ) );
				//this.tx難易度別背景[1] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_background_Normal.png" ) );
				//this.tx難易度別背景[2] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_background_Hard.png" ) );
				//this.tx難易度別背景[3] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_background_Master.png" ) );
				//this.tx難易度別背景[4] = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_background_Edit.png" ) );
				//this.tx下部テキスト = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\5_footer text.png" ) );
				this.soundあと30秒 = TJAPlayer3.Sound管理.tサウンドを生成する(CSkin.Path(@"Sounds\30Seconds.ogg"), ESoundGroup.Voice);
				this.soundあと10秒 = TJAPlayer3.Sound管理.tサウンドを生成する(CSkin.Path(@"Sounds\10Seconds.ogg"), ESoundGroup.Voice);
				this.soundあと5秒 = TJAPlayer3.Sound管理.tサウンドを生成する(CSkin.Path(@"Sounds\5Seconds.ogg"), ESoundGroup.Voice);
				this.soundピッ = TJAPlayer3.Sound管理.tサウンドを生成する(CSkin.Path(@"Sounds\pi.ogg"), ESoundGroup.SoundEffect);
				this.ct背景スクロール用タイマー = new CCounter(0, TJAPlayer3.Tx.SongSelect_Background.szテクスチャサイズ.Width, 30, TJAPlayer3.Timer);
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				//CDTXMania.tテクスチャの解放( ref this.tx背景 );
				//CDTXMania.tテクスチャの解放( ref this.tx上部パネル );
				//CDTXMania.tテクスチャの解放( ref this.tx下部パネル );
				//CDTXMania.tテクスチャの解放( ref this.txFLIP );
                //CDTXMania.tテクスチャの解放( ref this.tx難易度名 );
                //CDTXMania.tテクスチャの解放( ref this.tx下部テキスト );
                //for( int j = 0; j < 9; j++ )
                //{
                //    CDTXMania.tテクスチャの解放( ref this.txジャンル別背景[ j ] );
                //}
                //for( int j = 0; j < 5; j++ )
                //{
                //    CDTXMania.tテクスチャの解放( ref this.tx難易度別背景[ j ] );
                //}
				base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if( !base.b活性化してない )
			{
			this.ct背景スクロール用タイマー.t進行Loop();
				#region [ 初めての進行描画 ]
				//---------------------
				if( base.b初めての進行描画 )
				{
					this.act難易度選択画面.n現在の選択行 = 3;
					this.ct登場時アニメ用共通 = new CCounter( 0, 100, 3, TJAPlayer3.Timer );
                    if (TJAPlayer3.r直前のステージ == TJAPlayer3.stage結果)
                    {
                        this.actFIfrom結果画面.tフェードイン開始();
                        base.eフェーズID = CStage.Eフェーズ.選曲_結果画面からのフェードイン;
                    }
                    else
                    {
                        this.actFIFO.tフェードイン開始();
                        base.eフェーズID = CStage.Eフェーズ.共通_フェードイン;
                    }
					this.t選択曲変更通知();
					base.b初めての進行描画 = false;
				}
				//---------------------
				#endregion

				ctDonchanNormal.t進行Loop();
				ctDonchanSelect.t進行();
				ctDonchanStart.t進行();
				ct制限時間.t進行();
				ctOneCoin.t進行Loop();

				this.ct登場時アニメ用共通.t進行();

				if (TJAPlayer3.Tx.SongSelect_Background != null)
				{
					for (int i = 0; i < (1280 / TJAPlayer3.Tx.SongSelect_Background.szテクスチャサイズ.Width) + 2; i++)
						TJAPlayer3.Tx.SongSelect_Background.t2D描画(TJAPlayer3.app.Device, -ct背景スクロール用タイマー.n現在の値 + TJAPlayer3.Tx.SongSelect_Background.szテクスチャサイズ.Width * i, 0);
				}

				if ( this.r現在選択中の曲 != null )
                {
                    if(this.nStrジャンルtoNum(this.r現在選択中の曲.strジャンル) != 0 || r現在選択中の曲.eノード種別 == C曲リストノード.Eノード種別.BOX || r現在選択中の曲.eノード種別 == C曲リストノード.Eノード種別.SCORE)
                    {
                        nGenreBack = this.nStrジャンルtoNum(this.r現在選択中の曲.strジャンル);
                    }
                    if (TJAPlayer3.Tx.SongSelect_GenreBack[nGenreBack] != null )
                    {
                        for( int i = 0 ; i <(1280 / TJAPlayer3.Tx.SongSelect_Background.szテクスチャサイズ.Width) + 2; i++ )
                            TJAPlayer3.Tx.SongSelect_GenreBack[nGenreBack].t2D描画(TJAPlayer3.app.Device, -ct背景スクロール用タイマー.n現在の値 + TJAPlayer3.Tx.SongSelect_Background.szテクスチャサイズ.Width * i , 0);
                    }
                }

				this.act曲リスト.On進行描画();
				int y = 0;

				if (TJAPlayer3.Tx.SongSelect_Header != null)
					TJAPlayer3.Tx.SongSelect_Header.t2D描画(TJAPlayer3.app.Device, 0, 0);

				#region[ 下部テキスト ]
				if (TJAPlayer3.Tx.SongSelect_Auto != null)
                {
                    if (TJAPlayer3.ConfigIni.b太鼓パートAutoPlay)
                    {
                        TJAPlayer3.Tx.SongSelect_Auto.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.SongSelect_Auto_X[0], TJAPlayer3.Skin.SongSelect_Auto_Y[0]);
                    }
                    if (TJAPlayer3.ConfigIni.nPlayerCount > 1 && TJAPlayer3.ConfigIni.b太鼓パートAutoPlay2P)
                    {
                        TJAPlayer3.Tx.SongSelect_Auto.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.SongSelect_Auto_X[1], TJAPlayer3.Skin.SongSelect_Auto_Y[1]);
                    }
                }
                if (TJAPlayer3.ConfigIni.eGameMode == EGame.完走叩ききりまショー)
                    TJAPlayer3.act文字コンソール.tPrint(0, 0, C文字コンソール.Eフォント種別.白, "GAME: SURVIVAL");
                if (TJAPlayer3.ConfigIni.eGameMode == EGame.完走叩ききりまショー激辛)
                    TJAPlayer3.act文字コンソール.tPrint(0, 0, C文字コンソール.Eフォント種別.白, "GAME: SURVIVAL HARD");
                if (TJAPlayer3.ConfigIni.bSuperHard)
                    TJAPlayer3.act文字コンソール.tPrint(0, 16, C文字コンソール.Eフォント種別.赤, "SUPER HARD MODE : ON");
                if (TJAPlayer3.ConfigIni.eScrollMode == EScrollMode.BMSCROLL)
                    TJAPlayer3.act文字コンソール.tPrint(0, 32, C文字コンソール.Eフォント種別.赤, "BMSCROLL : ON");
                else if (TJAPlayer3.ConfigIni.eScrollMode == EScrollMode.HBSCROLL)
                    TJAPlayer3.act文字コンソール.tPrint(0, 32, C文字コンソール.Eフォント種別.赤, "HBSCROLL : ON");
                #endregion

                this.actPresound.On進行描画();

                if( !this.bBGM再生済み && (base.eフェーズID == CStage.Eフェーズ.共通_通常状態) && !TJAPlayer3.Skin.bgm選曲画面In.b再生中)
                {
                    TJAPlayer3.Skin.bgm選曲画面.t再生する();
                    this.bBGM再生済み = true;
                }

                if( this.ctDiffSelect移動待ち != null )
                    this.ctDiffSelect移動待ち.t進行();

				if (act曲リスト.ctBoxOpen != null)
				{
					if (this.act曲リスト.ctBoxOpen.n現在の値 == 1000)
					{
						act曲リスト.ctBoxOpen.t停止();
						act曲リスト.ctBoxOpen.n現在の値 = 0;
						act曲リスト.ctBoxClose.t開始(0, 130, 4, TJAPlayer3.Timer);
						if (this.act曲リスト.bBoxOpen)
						{
							this.act曲リスト.tBOXに入る();
							this.act曲リスト.bBoxOpen = false;
						}
						if (this.act曲リスト.bBoxClose)
						{
							this.act曲リスト.tBOXを出る();
							this.act曲リスト.bBoxClose = false;
						}
					}
				}

				if (act曲リスト.ctBoxClose != null)
				{
					if (this.act曲リスト.ctBoxClose.n現在の値 == 130)
					{
						this.act曲リスト.bBoxOpenAnime = false;
					}
				}
				// キー入力
				if ( base.eフェーズID == CStage.Eフェーズ.共通_通常状態 
					&& TJAPlayer3.act現在入力を占有中のプラグイン == null )
				{
					#region [ 簡易CONFIGでMore、またはShift+F1: 詳細CONFIG呼び出し ]
					if (  actQuickConfig.bGotoDetailConfig )
					{	// 詳細CONFIG呼び出し
						actQuickConfig.tDeativatePopupMenu();
						this.enterConfigStage();
					}
					#endregion
					if ( !this.actSortSongs.bIsActivePopupMenu && !this.actQuickConfig.bIsActivePopupMenu && !this.act曲リスト.ctBoxOpen.b進行中 && !act曲リスト.bBoxOpenAnime && !this.act難易度選択画面.b曲選択)
					{
						this.ctキー反復用.Plus.tキー反復(TJAPlayer3.Input管理.Keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.NumberPadPlus), new CCounter.DGキー処理(this.Timer_Plus));
						this.ctキー反復用.Minus.tキー反復(TJAPlayer3.Input管理.Keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.NumberPadMinus), new CCounter.DGキー処理(this.Timer_Minus));
						#region [ ESC ]
						if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.Escape) && !this.act難易度選択画面.bIsDifficltSelect)
							if (this.act曲リスト.r現在選択中の曲 == null)
							{   // [ESC]
								TJAPlayer3.Skin.sound取消音.t再生する();
								this.eフェードアウト完了時の戻り値 = E戻り値.タイトルに戻る;
								this.actFIFO.tフェードアウト開始();
								base.eフェーズID = CStage.Eフェーズ.共通_フェードアウト;
								return 0;
							}
						    else if (this.act曲リスト.r現在選択中の曲.r親ノード == null)
                            {   // [ESC]
                                TJAPlayer3.Skin.sound取消音.t再生する();
                                this.eフェードアウト完了時の戻り値 = E戻り値.タイトルに戻る;
                                this.actFIFO.tフェードアウト開始();
                                base.eフェーズID = CStage.Eフェーズ.共通_フェードアウト;
                                return 0;
                            }
                            else
                            {
                                TJAPlayer3.Skin.sound取消音.t再生する();
                                ctDonchanSelect.t開始(0, TJAPlayer3.Tx.SongSelect_Donchan_Select.Length - 1, 1000 / 45, TJAPlayer3.Timer);
                                this.act曲リスト.bBoxOpenAnime = true;
                                this.act曲リスト.bBoxClose = true;
                                this.act曲リスト.bBoxOpen = false;
                                this.act曲リスト.ctBoxOpen.t開始(0, 1000, 1, TJAPlayer3.Timer);
                            }
                        #endregion
                        #region [ Shift-F1: CONFIG画面 ]
                        else if ( ( TJAPlayer3.Input管理.Keyboard.bキーが押されている( (int) SlimDX.DirectInput.Key.RightShift ) || TJAPlayer3.Input管理.Keyboard.bキーが押されている( (int) SlimDX.DirectInput.Key.LeftShift ) ) &&
							TJAPlayer3.Input管理.Keyboard.bキーが押された( (int) SlimDX.DirectInput.Key.F1 ) )
						{	// [SHIFT] + [F1] CONFIG
                                                    this.enterConfigStage();
						}
						#endregion
						#region [ F2 簡易オプション ]
						else if (TJAPlayer3.Input管理.Keyboard.bキーが押された( (int) SlimDX.DirectInput.Key.F2 ) )
						{
                                                    this.showQuickConfig();
						}
						#endregion
						#region [ F3 1PオートON/OFF ]
						else if (TJAPlayer3.Input管理.Keyboard.bキーが押された( (int) SlimDX.DirectInput.Key.F3 ) )
						{
							TJAPlayer3.Skin.sound変更音.t再生する();
                            C共通.bToggleBoolian( ref TJAPlayer3.ConfigIni.b太鼓パートAutoPlay );
						}
                        #endregion
                        #region [ F4 2PオートON/OFF ]
                        else if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.F4))
                        {
                            if (TJAPlayer3.ConfigIni.nPlayerCount > 1)
                            {
                                TJAPlayer3.Skin.sound変更音.t再生する();
                                C共通.bToggleBoolian(ref TJAPlayer3.ConfigIni.b太鼓パートAutoPlay2P);
                            }
                        }
                        #endregion
                        #region [ F5 曲データ一覧の再読み込み ]
                        else if (TJAPlayer3.Input管理.Keyboard.bキーが押された( (int) SlimDX.DirectInput.Key.F5 ) )
                        {
                            TJAPlayer3.Skin.sound決定音.t再生する();
                            if ( TJAPlayer3.EnumSongs.IsEnumerating )
                            {
                                TJAPlayer3.EnumSongs.Abort();
                                TJAPlayer3.actEnumSongs.On非活性化();
                            }
                            TJAPlayer3.EnumSongs.StartEnumFromDisk();
                            TJAPlayer3.EnumSongs.ChangeEnumeratePriority( ThreadPriority.Normal );
                            TJAPlayer3.actEnumSongs.bコマンドでの曲データ取得 = true;
                            TJAPlayer3.actEnumSongs.On活性化();
                        }
                        #endregion
                        #region [ F6 SCROLL ]
                        else if (TJAPlayer3.Input管理.Keyboard.bキーが押された( (int) SlimDX.DirectInput.Key.F6 ) )
                        {
                            TJAPlayer3.Skin.sound変更音.t再生する();
                            TJAPlayer3.ConfigIni.bスクロールモードを上書き = true;
                            switch( (int)TJAPlayer3.ConfigIni.eScrollMode )
                            {
                                case 0:
                                    TJAPlayer3.ConfigIni.eScrollMode = EScrollMode.BMSCROLL;
                                    break;
                                case 1:
                                    TJAPlayer3.ConfigIni.eScrollMode = EScrollMode.HBSCROLL;
                                    break;
                                case 2:
                                    TJAPlayer3.ConfigIni.eScrollMode = EScrollMode.Normal;
                                    TJAPlayer3.ConfigIni.bスクロールモードを上書き = false;
                                    break;
                            }
                        }
                        #endregion
                        #region [ F7 スーパーハード ]
                        else if (TJAPlayer3.Input管理.Keyboard.bキーが押された( (int) SlimDX.DirectInput.Key.F7 ) )
                        {
                            TJAPlayer3.Skin.sound変更音.t再生する();
                            C共通.bToggleBoolian( ref TJAPlayer3.ConfigIni.bSuperHard );
                        }
						#endregion
						#region [ F8 timer ]
						else if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.F8)) // Press "F8" key to pause or resume the timer.
						{
							TJAPlayer3.Skin.sound変更音.t再生する();
							if (ct制限時間.b進行中)
							{
								ct制限時間.t停止();
							}
							else
							{
								int int制限時間 = ct制限時間.n現在の値;
								ct制限時間.t開始(0, 100, 1000, TJAPlayer3.Timer);
								ct制限時間.n現在の値 = int制限時間;
							}
						}
						else if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.NumberPadPeriod))
						{
							制限時間音声のリセット();
							ct制限時間.n現在の値 = 0;
						}
						/*else if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.NumberPad1))
						{
							制限時間音声のリセット();
							ct制限時間.n現在の値 = 90;
						}
						else if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.NumberPad2))
						{
							制限時間音声のリセット();
							ct制限時間.n現在の値 = 80;
						}
						else if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.NumberPad3))
						{
							制限時間音声のリセット();
							ct制限時間.n現在の値 = 70;
						}
						else if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.NumberPad4))
						{
							制限時間音声のリセット();
							ct制限時間.n現在の値 = 60;
						}
						else if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.NumberPad5))
						{
							制限時間音声のリセット();
							ct制限時間.n現在の値 = 50;
						}
						else if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.NumberPad6))
						{
							制限時間音声のリセット();
							ct制限時間.n現在の値 = 40;
						}
						else if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.NumberPad7))
						{
							制限時間音声のリセット();
							ct制限時間.n現在の値 = 30;
						}
						else if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.NumberPad8))
						{
							制限時間音声のリセット();
							ct制限時間.n現在の値 = 20;
						}
						else if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.NumberPad9))
						{
							制限時間音声のリセット();
							ct制限時間.n現在の値 = 10;
						}*/
						else if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.NumberPad0))
						{
							//制限時間音声のリセット();
							ct制限時間.n現在の値 = 100;
						}
						#endregion
						#region [ F9 Randomly select a song ]
						else if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.F9))
						{
							if (TJAPlayer3.Skin.sound曲決定音.b読み込み成功)
								TJAPlayer3.Skin.sound曲決定音.t再生する();
							else
								TJAPlayer3.Skin.sound決定音.t再生する();
							this.t曲をランダム選択する();
						}
						#endregion
						else if (this.act曲リスト.r現在選択中の曲 != null && !this.act難易度選択画面.bIsDifficltSelect)
						{
							#region [ Decide ]
							if (ct制限時間.b終了値に達した || TJAPlayer3.Pad.b押されたDGB(Eパッド.Decide) || TJAPlayer3.Pad.b押されたDGB(Eパッド.LRed) || TJAPlayer3.Pad.b押されたDGB(Eパッド.RRed) ||
									(TJAPlayer3.ConfigIni.bEnterがキー割り当てのどこにも使用されていない && TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.Return)))
							{
								if (this.act曲リスト.r現在選択中の曲 != null)
								{
									if(this.act曲リスト.n目標のスクロールカウンタ == 0 && this.act曲リスト.n現在のスクロールカウンタ == 0)
									{
										switch (this.act曲リスト.r現在選択中の曲.eノード種別)
										{
											case C曲リストノード.Eノード種別.SCORE:
												this.act難易度選択画面.n現在の選択行 = 3;
												if (TJAPlayer3.stage選曲.n現在選択中の曲の難易度 != (int)Difficulty.Dan && TJAPlayer3.stage選曲.n現在選択中の曲の難易度 != (int)Difficulty.Tower)
												{
													this.act難易度選択画面.SongTitle = TJAPlayer3.stage選曲.act曲リスト.ttk曲名テクスチャを生成する(TJAPlayer3.stage選曲.r現在選択中の曲.strタイトル, Color.White, Color.Black);
													this.act難易度選択画面.SongSubTitle = TJAPlayer3.stage選曲.act曲リスト.ttkサブタイトルテクスチャを生成する(TJAPlayer3.stage選曲.r現在選択中の曲.strサブタイトル, Color.White, Color.Black);

													ctDonchanSelect.t開始(0, TJAPlayer3.Tx.SongSelect_Donchan_Select.Length - 1, 1000 / 45, TJAPlayer3.Timer);
													ctDiffSelect移動待ち.t開始(0, 1190, 1, TJAPlayer3.Timer);
													act難易度選択画面.bIsDifficltSelect = true;
													TJAPlayer3.Skin.sound決定音.t再生する();
												}
												else
												{
													ctDonchanStart.t開始(0, TJAPlayer3.Tx.SongSelect_Donchan_Start.Length - 1, 1000 / 45, TJAPlayer3.Timer);
													TJAPlayer3.Skin.sound決定音.t再生する();
													this.t曲を選択する();
												}
												break;
											case C曲リストノード.Eノード種別.BOX:
												{
													ctDonchanSelect.t開始(0, TJAPlayer3.Tx.SongSelect_Donchan_Select.Length - 1, 1000 / 45, TJAPlayer3.Timer);
													this.act曲リスト.bBoxOpenAnime = true;
													this.act曲リスト.bBoxOpen = true;
													this.act曲リスト.bBoxClose = false;
													TJAPlayer3.Skin.sound決定音.t再生する();
													this.act曲リスト.ctBoxOpen.t開始(0, 1000, 1, TJAPlayer3.Timer);
												}
												break;
											case C曲リストノード.Eノード種別.BACKBOX:
												if (!ct制限時間.b終了値に達した)
												{
													TJAPlayer3.Skin.sound取消音.t再生する();
													ctDonchanSelect.t開始(0, TJAPlayer3.Tx.SongSelect_Donchan_Select.Length - 1, 1000 / 45, TJAPlayer3.Timer);
													this.act曲リスト.bBoxOpenAnime = true;
													this.act曲リスト.bBoxClose = true;
													this.act曲リスト.bBoxOpen = false;
													this.act曲リスト.ctBoxOpen.t開始(0, 1000, 1, TJAPlayer3.Timer);
												}
												else this.tカーソルを下へ移動する();
												break;
											case C曲リストノード.Eノード種別.RANDOM:
												if (TJAPlayer3.Skin.sound曲決定音.b読み込み成功)
													TJAPlayer3.Skin.sound曲決定音.t再生する();
												else
													TJAPlayer3.Skin.sound決定音.t再生する();
												this.t曲をランダム選択する();
												break;
												//case C曲リストノード.Eノード種別.DANI:
												//    if (CDTXMania.Skin.sound段位移動.b読み込み成功)
												//        CDTXMania.Skin.sound段位移動.t再生する();
												//    else
												//        CDTXMania.Skin.sound段位移動.t再生する();
												//    this.X();
												//    break;
										}
									}
								}
							}
							#endregion
							if (!ct制限時間.b終了値に達した && !this.bスクロール中)
							{
								#region [ Up ]
								this.ctキー反復用.Up.tキー反復(TJAPlayer3.Input管理.Keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.LeftArrow), new CCounter.DGキー処理(this.tカーソルを上へ移動する));
								//this.ctキー反復用.Up.tキー反復( CDTXMania.Input管理.Keyboard.bキーが押されている( (int) SlimDX.DirectInput.Key.UpArrow ) || CDTXMania.Input管理.Keyboard.bキーが押されている( (int) SlimDX.DirectInput.Key.LeftArrow ), new CCounter.DGキー処理( this.tカーソルを上へ移動する ) );
								if (TJAPlayer3.Pad.b押された(E楽器パート.DRUMS, Eパッド.LBlue))
								{
									this.tカーソルを上へ移動する();
								}
								#endregion
								#region [ Down ]
								this.ctキー反復用.Down.tキー反復(TJAPlayer3.Input管理.Keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.RightArrow), new CCounter.DGキー処理(this.tカーソルを下へ移動する));
								//this.ctキー反復用.Down.tキー反復( CDTXMania.Input管理.Keyboard.bキーが押されている( (int) SlimDX.DirectInput.Key.DownArrow ) || CDTXMania.Input管理.Keyboard.bキーが押されている( (int) SlimDX.DirectInput.Key.RightArrow ), new CCounter.DGキー処理( this.tカーソルを下へ移動する ) );
								if (TJAPlayer3.Pad.b押された(E楽器パート.DRUMS, Eパッド.RBlue))
									this.tカーソルを下へ移動する();
								#endregion
							}
							if(!ct制限時間.b終了値に達した)
							{
								#region [ Skip_Up ]
								if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.LeftControl))
								{
									this.tカーソルを上へ移動する_Skip();
								}
								#endregion
								#region [ Skip_Down ]
								if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.RightControl))
								{
									this.tカーソルを下へ移動する_Skip();

								}
								#endregion
							}
							#region [ Upstairs ]
							if ( ( ( this.act曲リスト.r現在選択中の曲 != null ) && ( this.act曲リスト.r現在選択中の曲.r親ノード != null ) ) && ( TJAPlayer3.Pad.b押された( E楽器パート.DRUMS, Eパッド.FT ) || TJAPlayer3.Pad.b押されたGB( Eパッド.Cancel ) ) )
							{
								TJAPlayer3.Skin.sound取消音.t再生する();
								this.act曲リスト.bBoxClose = true;
							}
							#endregion
							#region [ BDx2: 簡易CONFIG ]
                            if( TJAPlayer3.Input管理.Keyboard.bキーが押された( (int) SlimDX.DirectInput.Key.Space ) )
                            {
							    TJAPlayer3.Skin.sound変更音.t再生する();
								this.actSortSongs.tActivatePopupMenu( E楽器パート.DRUMS, ref this.act曲リスト );
                            }
							#endregion
						}
					}

					#region [ Minus & Equals Sound Group Level ]
					KeyboardSoundGroupLevelControlHandler.Handle(
				        TJAPlayer3.Input管理.Keyboard, TJAPlayer3.SoundGroupLevelController, TJAPlayer3.Skin, true);
				    #endregion
				}

				if (this.act難易度選択画面.bIsDifficltSelect)
				{
					if (TJAPlayer3.stage選曲.n現在選択中の曲の難易度 != (int)Difficulty.Dan && TJAPlayer3.stage選曲.n現在選択中の曲の難易度 != (int)Difficulty.Tower)
					{
						if (this.ctDiffSelect移動待ち.n現在の値 >= 935)
						{
							this.act難易度選択画面.On進行描画();
						}
					}
				}

				this.actSortSongs.t進行描画();
				this.actQuickConfig.t進行描画();

				if (TJAPlayer3.ConfigIni.nPlayerCount == 1)
				{
					if (TJAPlayer3.Tx.SongSelect_One_Coin != null)
						TJAPlayer3.Tx.SongSelect_One_Coin.t2D描画(TJAPlayer3.app.Device, 0, 0);
					if (this.ctOneCoin.n現在の値 > 255) TJAPlayer3.Tx.SongSelect_One_Coin.Opacity = 511 - this.ctOneCoin.n現在の値;
					else TJAPlayer3.Tx.SongSelect_One_Coin.Opacity = this.ctOneCoin.n現在の値;
				}

				if (!ctDonchanSelect.b進行中 && !ctDonchanStart.b進行中)
				{
					TJAPlayer3.Tx.SongSelect_Donchan_Normal[ctDonchanNormal.n現在の値].t2D描画(TJAPlayer3.app.Device, 0, 330);
					if(TJAPlayer3.ConfigIni.nPlayerCount == 2)
						TJAPlayer3.Tx.SongSelect_Donchan_Normal2[ctDonchanNormal.n現在の値].t2D描画(TJAPlayer3.app.Device, 990, 330);
				}
				else
				{
					if (ctDonchanSelect.b進行中)
					{
						if (ctDonchanSelect.n現在の値 >= ctDonchanSelect.n終了値)
						{
							ctDonchanSelect.t停止();
						}
						TJAPlayer3.Tx.SongSelect_Donchan_Select[ctDonchanSelect.n現在の値].t2D描画(TJAPlayer3.app.Device, 0, 330);
						if (TJAPlayer3.ConfigIni.nPlayerCount == 2)
							TJAPlayer3.Tx.SongSelect_Donchan_Select2[ctDonchanSelect.n現在の値].t2D描画(TJAPlayer3.app.Device, 990, 330);
					}
					if (ctDonchanStart.b進行中)
					{
						TJAPlayer3.Tx.SongSelect_Donchan_Start[ctDonchanStart.n現在の値].t2D描画(TJAPlayer3.app.Device, 0, 330);
						if (TJAPlayer3.ConfigIni.nPlayerCount == 2)
							TJAPlayer3.Tx.SongSelect_Donchan_Start2[ctDonchanStart.n現在の値].t2D描画(TJAPlayer3.app.Device, 990, 330);
					}
				}


				#region ネームプレート
				for (int i = 0; i < TJAPlayer3.ConfigIni.nPlayerCount; i++)
				{
					if (TJAPlayer3.Tx.NamePlate[i] != null)
					{
						TJAPlayer3.Tx.NamePlate[i].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.SongSelect_NamePlate_X[i], TJAPlayer3.Skin.SongSelect_NamePlate_Y[i]);
					}
				}
				#endregion

				if (ct制限時間.n現在の値 > 90 && ct制限時間.n現在の値 <= 100)
				{
					if (TJAPlayer3.Tx.SongSelect_Timer_Red != null)
						TJAPlayer3.Tx.SongSelect_Timer_Red.t2D描画(TJAPlayer3.app.Device, 0, 0);
				}

				switch (ct制限時間.n現在の値)
				{
					case 0:
						if (TJAPlayer3.Tx.SongSelect_Timer100 != null)
							TJAPlayer3.Tx.SongSelect_Timer100.t2D描画(TJAPlayer3.app.Device, 0, 0);
						break;
					case 1:
					case 11:
					case 21:
					case 31:
					case 41:
					case 51:
					case 61:
					case 71:
					case 81:
						if (TJAPlayer3.Tx.SongSelect_Timer[9] != null)
							TJAPlayer3.Tx.SongSelect_Timer[9].t2D描画(TJAPlayer3.app.Device, 1191, 56);
						break;
					case 2:
					case 12:
					case 22:
					case 32:
					case 42:
					case 52:
					case 62:
					case 72:
					case 82:
						if (TJAPlayer3.Tx.SongSelect_Timer[8] != null)
							TJAPlayer3.Tx.SongSelect_Timer[8].t2D描画(TJAPlayer3.app.Device, 1191, 56);
						break;
					case 3:
					case 13:
					case 23:
					case 33:
					case 43:
					case 53:
					case 63:
					case 73:
					case 83:
						if (TJAPlayer3.Tx.SongSelect_Timer[7] != null)
							TJAPlayer3.Tx.SongSelect_Timer[7].t2D描画(TJAPlayer3.app.Device, 1191, 56);
						break;
					case 4:
					case 14:
					case 24:
					case 34:
					case 44:
					case 54:
					case 64:
					case 74:
					case 84:
						if (TJAPlayer3.Tx.SongSelect_Timer[6] != null)
							TJAPlayer3.Tx.SongSelect_Timer[6].t2D描画(TJAPlayer3.app.Device, 1191, 56);
						break;
					case 5:
					case 15:
					case 25:
					case 35:
					case 45:
					case 55:
					case 65:
					case 75:
					case 85:
						if (TJAPlayer3.Tx.SongSelect_Timer[5] != null)
							TJAPlayer3.Tx.SongSelect_Timer[5].t2D描画(TJAPlayer3.app.Device, 1191, 56);
						break;
					case 6:
					case 16:
					case 26:
					case 36:
					case 46:
					case 56:
					case 66:
					case 76:
					case 86:
						if (TJAPlayer3.Tx.SongSelect_Timer[4] != null)
							TJAPlayer3.Tx.SongSelect_Timer[4].t2D描画(TJAPlayer3.app.Device, 1191, 56);
						break;
					case 7:
					case 17:
					case 27:
					case 37:
					case 47:
					case 57:
					case 67:
					case 77:
					case 87:
						if (TJAPlayer3.Tx.SongSelect_Timer[3] != null)
							TJAPlayer3.Tx.SongSelect_Timer[3].t2D描画(TJAPlayer3.app.Device, 1191, 56);
						break;
					case 8:
					case 18:
					case 28:
					case 38:
					case 48:
					case 58:
					case 68:
					case 78:
					case 88:
						if (TJAPlayer3.Tx.SongSelect_Timer[2] != null)
							TJAPlayer3.Tx.SongSelect_Timer[2].t2D描画(TJAPlayer3.app.Device, 1191, 56);
						break;
					case 9:
					case 19:
					case 29:
					case 39:
					case 49:
					case 59:
					case 69:
					case 79:
					case 89:
						if (TJAPlayer3.Tx.SongSelect_Timer[1] != null)
							TJAPlayer3.Tx.SongSelect_Timer[1].t2D描画(TJAPlayer3.app.Device, 1191, 56);
						break;
					case 10:
					case 20:
					case 30:
					case 40:
					case 50:
					case 60:
					case 70:
					case 80:
					case 90:
						if (TJAPlayer3.Tx.SongSelect_Timer[0] != null)
							TJAPlayer3.Tx.SongSelect_Timer[0].t2D描画(TJAPlayer3.app.Device, 1191, 56);
						break;
					case 91:
						if (TJAPlayer3.Tx.SongSelect_Timerw[9] != null)
							TJAPlayer3.Tx.SongSelect_Timerw[9].t2D描画(TJAPlayer3.app.Device, 1169, 56);
						if (!this.IsPlayed_pi[1])
						{
							this.soundピッ.t再生を開始する();
							this.IsPlayed_pi[1] = true;
						}
						break;
					case 92:
						if (TJAPlayer3.Tx.SongSelect_Timerw[8] != null)
							TJAPlayer3.Tx.SongSelect_Timerw[8].t2D描画(TJAPlayer3.app.Device, 1169, 56);
						if (!this.IsPlayed_pi[2])
						{
							this.soundピッ.t再生を開始する();
							this.IsPlayed_pi[2] = true;
						}
						break;
					case 93:
						if (TJAPlayer3.Tx.SongSelect_Timerw[7] != null)
							TJAPlayer3.Tx.SongSelect_Timerw[7].t2D描画(TJAPlayer3.app.Device, 1169, 56);
						if (!this.IsPlayed_pi[3])
						{
							this.soundピッ.t再生を開始する();
							this.IsPlayed_pi[3] = true;
						}
						break;
					case 94:
						if (TJAPlayer3.Tx.SongSelect_Timerw[6] != null)
							TJAPlayer3.Tx.SongSelect_Timerw[6].t2D描画(TJAPlayer3.app.Device, 1169, 56);
						if (!this.IsPlayed_pi[4])
						{
							this.soundピッ.t再生を開始する();
							this.IsPlayed_pi[4] = true;
						}
						break;
					case 95:
						if (TJAPlayer3.Tx.SongSelect_Timerw[5] != null)
							TJAPlayer3.Tx.SongSelect_Timerw[5].t2D描画(TJAPlayer3.app.Device, 1169, 56);
						if (!this.IsPlayed_pi[5])
						{
							this.soundピッ.t再生を開始する();
							this.IsPlayed_pi[5] = true;
						}
						break;
					case 96:
						if (TJAPlayer3.Tx.SongSelect_Timerw[4] != null)
							TJAPlayer3.Tx.SongSelect_Timerw[4].t2D描画(TJAPlayer3.app.Device, 1169, 56);
						if (!this.IsPlayed_pi[6])
						{
							this.soundピッ.t再生を開始する();
							this.IsPlayed_pi[6] = true;
						}
						break;
					case 97:
						if (TJAPlayer3.Tx.SongSelect_Timerw[3] != null)
							TJAPlayer3.Tx.SongSelect_Timerw[3].t2D描画(TJAPlayer3.app.Device, 1169, 56);
						if(!this.IsPlayed_pi[7])
{
							this.soundピッ.t再生を開始する();
							this.IsPlayed_pi[7] = true;
						}
						break;
					case 98:
						if (TJAPlayer3.Tx.SongSelect_Timerw[2] != null)
							TJAPlayer3.Tx.SongSelect_Timerw[2].t2D描画(TJAPlayer3.app.Device, 1169, 56);
						if (!this.IsPlayed_pi[8])
						{
							this.soundピッ.t再生を開始する();
							this.IsPlayed_pi[8] = true;
						}
						break;
					case 99:
						if (TJAPlayer3.Tx.SongSelect_Timerw[1] != null)
							TJAPlayer3.Tx.SongSelect_Timerw[1].t2D描画(TJAPlayer3.app.Device, 1169, 56);
						if (!this.IsPlayed_pi[9])
						{
							this.soundピッ.t再生を開始する();
							this.IsPlayed_pi[9] = true;
						}
						break;
					case 100:
						if (TJAPlayer3.Tx.SongSelect_Timerw[0] != null)
							TJAPlayer3.Tx.SongSelect_Timerw[0].t2D描画(TJAPlayer3.app.Device, 1169, 56);
						if (!this.IsPlayed_pi[0])
						{
							this.soundピッ.t再生を開始する();
							this.IsPlayed_pi[0] = true;
						}
						break;

				}
				if (ct制限時間.n現在の値 > 0 & ct制限時間.n現在の値 < 11)
				{
					if (TJAPlayer3.Tx.SongSelect_Timer[9] != null)
						TJAPlayer3.Tx.SongSelect_Timer[9].t2D描画(TJAPlayer3.app.Device, 1147, 56);
				}
				else if (ct制限時間.n現在の値 > 10 & ct制限時間.n現在の値 < 21)
				{
					if (TJAPlayer3.Tx.SongSelect_Timer[8] != null)
						TJAPlayer3.Tx.SongSelect_Timer[8].t2D描画(TJAPlayer3.app.Device, 1147, 56);
				}
				else if (ct制限時間.n現在の値 > 20 & ct制限時間.n現在の値 < 31)
				{
					if (TJAPlayer3.Tx.SongSelect_Timer[7] != null)
						TJAPlayer3.Tx.SongSelect_Timer[7].t2D描画(TJAPlayer3.app.Device, 1147, 56);
				}
				else if (ct制限時間.n現在の値 > 30 & ct制限時間.n現在の値 < 41)
				{
					if (TJAPlayer3.Tx.SongSelect_Timer[6] != null)
						TJAPlayer3.Tx.SongSelect_Timer[6].t2D描画(TJAPlayer3.app.Device, 1147, 56);
				}
				else if (ct制限時間.n現在の値 > 40 & ct制限時間.n現在の値 < 51)
				{
					if (TJAPlayer3.Tx.SongSelect_Timer[5] != null)
						TJAPlayer3.Tx.SongSelect_Timer[5].t2D描画(TJAPlayer3.app.Device, 1147, 56);
				}
				else if (ct制限時間.n現在の値 > 50 & ct制限時間.n現在の値 < 61)
				{
					if (TJAPlayer3.Tx.SongSelect_Timer[4] != null)
						TJAPlayer3.Tx.SongSelect_Timer[4].t2D描画(TJAPlayer3.app.Device, 1147, 56);
				}
				else if (ct制限時間.n現在の値 > 60 & ct制限時間.n現在の値 < 71)
				{
					if (TJAPlayer3.Tx.SongSelect_Timer[3] != null)
						TJAPlayer3.Tx.SongSelect_Timer[3].t2D描画(TJAPlayer3.app.Device, 1147, 56);
				}
				else if (ct制限時間.n現在の値 > 70 & ct制限時間.n現在の値 < 81)
				{
					if (TJAPlayer3.Tx.SongSelect_Timer[2] != null)
						TJAPlayer3.Tx.SongSelect_Timer[2].t2D描画(TJAPlayer3.app.Device, 1147, 56);
				}
				else if (ct制限時間.n現在の値 > 80 & ct制限時間.n現在の値 < 91)
				{
					if (TJAPlayer3.Tx.SongSelect_Timer[1] != null)
						TJAPlayer3.Tx.SongSelect_Timer[1].t2D描画(TJAPlayer3.app.Device, 1147, 56);
				}

				if (ct制限時間.n現在の値 == 70 && !this.suppress30sec)
				{
					this.soundあと30秒.tサウンドを再生する();
				}
				else if (ct制限時間.n現在の値 == 90)
				{
					this.soundあと10秒.tサウンドを再生する();
				}
				else if (ct制限時間.n現在の値 == 95)
				{
					this.soundあと5秒.tサウンドを再生する();
				}
				/*else if (ct制限時間.b終了値に達した)
				{
					// Unused.
				}*/

				switch ( base.eフェーズID )
				{
					case CStage.Eフェーズ.共通_フェードイン:
						if( this.actFIFO.On進行描画() != 0 )
						{
							base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
						}
						break;

					case CStage.Eフェーズ.共通_フェードアウト:
						if( this.actFIFO.On進行描画() == 0 )
						{
							break;
						}
						return (int) this.eフェードアウト完了時の戻り値;

					case CStage.Eフェーズ.選曲_結果画面からのフェードイン:
						if( this.actFIfrom結果画面.On進行描画() != 0 )
						{
							base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
						}
						break;

					case CStage.Eフェーズ.選曲_NowLoading画面へのフェードアウト:
						if( this.actFOtoNowLoading.On進行描画() == 0 )
						{
							break;
						}
						return (int) this.eフェードアウト完了時の戻り値;
				}
			}
			return 0;
		}
		public enum E戻り値 : int
		{
			継続,
			タイトルに戻る,
			選曲した,
			オプション呼び出し,
			コンフィグ呼び出し,
			スキン変更
		}
		

		// その他

		#region [ private ]
		//-----------------
		[StructLayout( LayoutKind.Sequential )]
		private struct STキー反復用カウンタ
		{
			public CCounter Up;
			public CCounter Down;
			public CCounter R;
			public CCounter B;
			public CCounter Plus;
			public CCounter Minus;
			public CCounter this[ int index ]
			{
				get
				{
					switch( index )
					{
						case 0:
							return this.Up;

						case 1:
							return this.Down;

						case 2:
							return this.R;

						case 3:
							return this.B;

						case 4:
							return this.Plus;

						case 5:
							return this.Minus;
					}
					throw new IndexOutOfRangeException();
				}
				set
				{
					switch( index )
					{
						case 0:
							this.Up = value;
							return;

						case 1:
							this.Down = value;
							return;

						case 2:
							this.R = value;
							return;

						case 3:
							this.B = value;
							return;

						case 4:
							this.Plus = value;
							return;

						case 5:
							this.Minus = value;
							return;
					}
					throw new IndexOutOfRangeException();
				}
			}
		}
		private CCounter ctDonchanNormal;
		private CCounter ctDonchanSelect;
		public CCounter ctDonchanStart;
		public CCounter ct制限時間;
		private CCounter ctOneCoin;
		private CSound soundあと30秒;
		private CSound soundあと10秒;
		private CSound soundあと5秒;
		private CSound soundピッ;
		private bool suppress30sec;
		private bool[] IsPlayed_pi;
		private CActFIFOBlack actFIFO;
		private CActFIFOBlack actFIfrom結果画面;
		//private CActFIFOBlack actFOtoNowLoading;
        private CActFIFOStart actFOtoNowLoading;
		public CActSelectPresound actPresound;
		private CActオプションパネル actオプションパネル;
		public CActSelect曲リスト act曲リスト;
        public CActSelect難易度選択画面 act難易度選択画面;

		public CActSortSongs actSortSongs;
		public CActSelectQuickConfig actQuickConfig;

                private int nGenreBack;
		public bool bBGM再生済み;
		private STキー反復用カウンタ ctキー反復用;
		public CCounter ct登場時アニメ用共通;
		private CCounter ct背景スクロール用タイマー;
		private E戻り値 eフェードアウト完了時の戻り値;
		private Font ftフォント;

		public CCounter ctDiffSelect移動待ち;

		private struct STCommandTime		// #24063 2011.1.16 yyagi コマンド入力時刻の記録用
		{
			public E楽器パート eInst;		// 使用楽器
			public EパッドFlag ePad;		// 押されたコマンド(同時押しはOR演算で列挙する)
			public long time;				// コマンド入力時刻
		}
		private class CCommandHistory		// #24063 2011.1.16 yyagi コマンド入力履歴を保持_確認するクラス
		{
			readonly int buffersize = 16;
			private List<STCommandTime> stct;

			public CCommandHistory()		// コンストラクタ
			{
				stct = new List<STCommandTime>( buffersize );
			}

			/// <summary>
			/// コマンド入力履歴へのコマンド追加
			/// </summary>
			/// <param name="_eInst">楽器の種類</param>
			/// <param name="_ePad">入力コマンド(同時押しはOR演算で列挙すること)</param>
			public void Add( E楽器パート _eInst, EパッドFlag _ePad )
			{
				STCommandTime _stct = new STCommandTime {
					eInst = _eInst,
					ePad = _ePad,
					time = TJAPlayer3.Timer.n現在時刻
				};

				if ( stct.Count >= buffersize )
				{
					stct.RemoveAt( 0 );
				}
				stct.Add(_stct);
//Debug.WriteLine( "CMDHIS: 楽器=" + _stct.eInst + ", CMD=" + _stct.ePad + ", time=" + _stct.time );
			}
			public void RemoveAt( int index )
			{
				stct.RemoveAt( index );
			}

			/// <summary>
			/// コマンド入力に成功しているか調べる
			/// </summary>
			/// <param name="_ePad">入力が成功したか調べたいコマンド</param>
			/// <param name="_eInst">対象楽器</param>
			/// <returns>コマンド入力成功時true</returns>
			public bool CheckCommand( EパッドFlag[] _ePad, E楽器パート _eInst)
			{
				int targetCount = _ePad.Length;
				int stciCount = stct.Count;
				if ( stciCount < targetCount )
				{
//Debug.WriteLine("NOT start checking...stciCount=" + stciCount + ", targetCount=" + targetCount);
					return false;
				}

				long curTime = TJAPlayer3.Timer.n現在時刻;
//Debug.WriteLine("Start checking...targetCount=" + targetCount);
				for ( int i = targetCount - 1, j = stciCount - 1; i >= 0; i--, j-- )
				{
					if ( _ePad[ i ] != stct[ j ].ePad )
					{
//Debug.WriteLine( "CMD解析: false targetCount=" + targetCount + ", i=" + i + ", j=" + j + ": ePad[]=" + _ePad[i] + ", stci[j] = " + stct[j].ePad );
						return false;
					}
					if ( stct[ j ].eInst != _eInst )
					{
//Debug.WriteLine( "CMD解析: false " + i );
						return false;
					}
					if ( curTime - stct[ j ].time > 500 )
					{
//Debug.WriteLine( "CMD解析: false " + i + "; over 500ms" );
						return false;
					}
					curTime = stct[ j ].time;
				}

//Debug.Write( "CMD解析: 成功!(" + _ePad.Length + ") " );
//for ( int i = 0; i < _ePad.Length; i++ ) Debug.Write( _ePad[ i ] + ", " );
//Debug.WriteLine( "" );
				//stct.RemoveRange( 0, targetCount );			// #24396 2011.2.13 yyagi 
				stct.Clear();									// #24396 2011.2.13 yyagi Clear all command input history in case you succeeded inputting some command

				return true;
			}
		}
		private CCommandHistory CommandHistory;
		private void tカーソルを下へ移動する()
		{
			TJAPlayer3.Skin.soundカーソル移動音.t再生する();
			this.act曲リスト.t次に移動();
		}

		private void tカーソルを下へ移動する_Skip()
		{
			TJAPlayer3.Skin.soundスキップ音.t再生する();
			for (int i = 0; i < 7; i++)
				this.act曲リスト.t次に移動();
		}

		private void tカーソルを上へ移動する()
		{
			TJAPlayer3.Skin.soundカーソル移動音.t再生する();
			this.act曲リスト.t前に移動();
		}

		private void tカーソルを上へ移動する_Skip()
		{
			TJAPlayer3.Skin.soundスキップ音.t再生する();
			for (int i = 0; i < 7; i++)
				this.act曲リスト.t前に移動();
		}

		private void t曲をランダム選択する()
		{
			C曲リストノード song = this.act曲リスト.r現在選択中の曲;
			if( ( song.stackランダム演奏番号.Count == 0 ) || ( song.listランダム用ノードリスト == null ) )
			{
				if( song.listランダム用ノードリスト == null )
				{
					song.listランダム用ノードリスト = this.t指定された曲が存在する場所の曲を列挙する_子リスト含む( song );
				}
				int count = song.listランダム用ノードリスト.Count;
				if( count == 0 )
				{
					return;
				}
				int[] numArray = new int[ count ];
				for( int i = 0; i < count; i++ )
				{
					numArray[ i ] = i;
				}
				for( int j = 0; j < ( count * 1.5 ); j++ )
				{
					int index = TJAPlayer3.Random.Next( count );
					int num5 = TJAPlayer3.Random.Next( count );
					int num6 = numArray[ num5 ];
					numArray[ num5 ] = numArray[ index ];
					numArray[ index ] = num6;
				}
				for( int k = 0; k < count; k++ )
				{
					song.stackランダム演奏番号.Push( numArray[ k ] );
				}
				if( TJAPlayer3.ConfigIni.bLogDTX詳細ログ出力 )
				{
					StringBuilder builder = new StringBuilder( 0x400 );
					builder.Append( string.Format( "ランダムインデックスリストを作成しました: {0}曲: ", song.stackランダム演奏番号.Count ) );
					for( int m = 0; m < count; m++ )
					{
						builder.Append( string.Format( "{0} ", numArray[ m ] ) );
					}
					Trace.TraceInformation( builder.ToString() );
				}
			}
			this.r確定された曲 = song.listランダム用ノードリスト[ song.stackランダム演奏番号.Pop() ];
			this.n確定された曲の難易度 = this.act曲リスト.n現在のアンカ難易度レベルに最も近い難易度レベルを返す( this.r確定された曲 );
			this.r確定されたスコア = this.r確定された曲.arスコア[this.n確定された曲の難易度];
            this.str確定された曲のジャンル = this.r確定された曲.strジャンル;
			this.eフェードアウト完了時の戻り値 = E戻り値.選曲した;
			this.actFOtoNowLoading.tフェードアウト開始();					// #27787 2012.3.10 yyagi 曲決定時の画面フェードアウトの省略
			base.eフェーズID = CStage.Eフェーズ.選曲_NowLoading画面へのフェードアウト;
			if( TJAPlayer3.ConfigIni.bLogDTX詳細ログ出力 )
			{
				int[] numArray2 = song.stackランダム演奏番号.ToArray();
				StringBuilder builder2 = new StringBuilder( 0x400 );
				builder2.Append( "ランダムインデックスリスト残り: " );
				if( numArray2.Length > 0 )
				{
					for( int n = 0; n < numArray2.Length; n++ )
					{
						builder2.Append( string.Format( "{0} ", numArray2[ n ] ) );
					}
				}
				else
				{
					builder2.Append( "(なし)" );
				}
				Trace.TraceInformation( builder2.ToString() );
			}
			TJAPlayer3.Skin.bgm選曲画面.t停止する();
		}
		private void t曲を選択する()
		{
			t曲を選択する(this.act曲リスト.n現在選択中の曲の現在の難易度レベル);
		}
		public void t曲を選択する( int nCurrentLevel )
		{
			this.r確定された曲 = this.act曲リスト.r現在選択中の曲;
			this.r確定されたスコア = this.act曲リスト.r現在選択中のスコア;
			this.n確定された曲の難易度 = nCurrentLevel;
            this.str確定された曲のジャンル = this.r確定された曲.strジャンル;
            if ( ( this.r確定された曲 != null ) && ( this.r確定されたスコア != null ) )
			{
				this.eフェードアウト完了時の戻り値 = E戻り値.選曲した;
				this.actFOtoNowLoading.tフェードアウト開始();				// #27787 2012.3.10 yyagi 曲決定時の画面フェードアウトの省略
				base.eフェーズID = CStage.Eフェーズ.選曲_NowLoading画面へのフェードアウト;
			}
			TJAPlayer3.Skin.bgm選曲画面.t停止する();
		}
		private List<C曲リストノード> t指定された曲が存在する場所の曲を列挙する_子リスト含む( C曲リストノード song )
		{
			List<C曲リストノード> list = new List<C曲リストノード>();
			song = song.r親ノード;
			if( ( song == null ) && ( TJAPlayer3.Songs管理.list曲ルート.Count > 0 ) )
			{
				foreach( C曲リストノード c曲リストノード in TJAPlayer3.Songs管理.list曲ルート )
				{
					if( ( c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.SCORE ) || ( c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.SCORE_MIDI ) )
					{
						list.Add( c曲リストノード );
					}
					if( ( c曲リストノード.list子リスト != null ) && TJAPlayer3.ConfigIni.bランダムセレクトで子BOXを検索対象とする )
					{
						this.t指定された曲の子リストの曲を列挙する_孫リスト含む( c曲リストノード, ref list );
					}
				}
				return list;
			}
			this.t指定された曲の子リストの曲を列挙する_孫リスト含む( song, ref list );
			return list;
		}
		private void t指定された曲の子リストの曲を列挙する_孫リスト含む( C曲リストノード r親, ref List<C曲リストノード> list )
		{
			if( ( r親 != null ) && ( r親.list子リスト != null ) )
			{
				foreach( C曲リストノード c曲リストノード in r親.list子リスト )
				{
					if( ( c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.SCORE ) || ( c曲リストノード.eノード種別 == C曲リストノード.Eノード種別.SCORE_MIDI ) )
					{
						list.Add( c曲リストノード );
					}
					if( ( c曲リストノード.list子リスト != null ) && TJAPlayer3.ConfigIni.bランダムセレクトで子BOXを検索対象とする )
					{
						this.t指定された曲の子リストの曲を列挙する_孫リスト含む( c曲リストノード, ref list );
					}
				}
			}
		}

        private int nStrジャンルtoNum( string strジャンル )
        {
            int nGenre = 8;
            switch( strジャンル )
			{
				case "ポップス":
					nGenre = 1;
					break;
				case "アニメ":
                    nGenre = 2;
                    break;
                case "ゲームミュージック":
                    nGenre = 3;
                    break;
                case "ナムコオリジナル":
                    nGenre = 4;
                    break;
                case "クラシック":
                    nGenre = 5;
                    break;
                case "キッズ":
                    nGenre = 6;
                    break;
                case "ボーカロイド":
                case "VOCALOID":
                    nGenre = 7;
                    break;
                default:
                    nGenre = 0;
                    break;

            }

            return nGenre;
        }
		//-----------------
		#endregion
	}
}
