using System;
using System.IO;
using System.Diagnostics;
using FDK;

namespace TJAPlayer3
{
	internal class CStage結果 : CStage
	{
		// プロパティ

		public STDGBVALUE<bool> b新記録スキル;
		public STDGBVALUE<bool> b新記録スコア;
        public STDGBVALUE<bool> b新記録ランク;
		public STDGBVALUE<float> fPerfect率;
		public STDGBVALUE<float> fGreat率;
		public STDGBVALUE<float> fGood率;
		public STDGBVALUE<float> fPoor率;
        public STDGBVALUE<float> fMiss率;
        public STDGBVALUE<bool> bオート;        // #23596 10.11.16 add ikanick
                                                //        10.11.17 change (int to bool) ikanick
		public STDGBVALUE<int> nランク値;
		public STDGBVALUE<int> n演奏回数;
		public STDGBVALUE<int> nScoreRank;
		public int n総合ランク値;
		public CDTX.CChip[] r空うちドラムチップ;
		public STDGBVALUE<CScoreIni.C演奏記録> st演奏記録;


		// コンストラクタ

		public CStage結果()
		{
			this.st演奏記録.Drums = new CScoreIni.C演奏記録();
			this.st演奏記録.Guitar = new CScoreIni.C演奏記録();
			this.st演奏記録.Bass = new CScoreIni.C演奏記録();
			this.st演奏記録.Taiko = new CScoreIni.C演奏記録();
			this.r空うちドラムチップ = new CDTX.CChip[ 10 ];
			this.n総合ランク値 = -1;
			this.nチャンネル0Atoレーン07 = new int[] { 1, 2, 3, 4, 5, 7, 6, 1, 7, 0 };
			base.eステージID = CStage.Eステージ.結果;
			base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
			base.b活性化してない = true;
			base.list子Activities.Add( this.actResultImage = new CActResultImage() );
			base.list子Activities.Add( this.actParameterPanel = new CActResultParameterPanel() );
			base.list子Activities.Add( this.actRank = new CActResultRank() );
			base.list子Activities.Add( this.actSongBar = new CActResultSongBar() );
			base.list子Activities.Add( this.actOption = new CActオプションパネル() );
			base.list子Activities.Add( this.actFI = new CActFIFOResult() );
			base.list子Activities.Add( this.actFO = new CActFIFOBlack() );
		}

		
		// CStage 実装

		public override void On活性化()
		{
			TJAPlayer3.Skin.bgmリザルトイン音.t再生する();
			Trace.TraceInformation( "結果ステージを活性化します。" );
			Trace.Indent();
			try
			{
				{
					#region [ 初期化 ]
					//---------------------
					this.eフェードアウト完了時の戻り値 = E戻り値.継続;
					this.bアニメが完了 = false;
					this.bIsCheckedWhetherResultScreenShouldSaveOrNot = false;              // #24609 2011.3.14 yyagi
					this.n最後に再生したHHのWAV番号 = -1;
					this.n最後に再生したHHのチャンネル番号 = 0;
					for (int i = 0; i < 3; i++)
					{
						this.b新記録スキル[i] = false;
						this.b新記録スコア[i] = false;
						this.b新記録ランク[i] = false;
					}
					//---------------------
					#endregion

					#region [ 結果の計算 ]
					//---------------------
					for (int i = 0; i < 3; i++)
					{
						this.nランク値[i] = -1;
						this.fPerfect率[i] = this.fGreat率[i] = this.fGood率[i] = this.fPoor率[i] = this.fMiss率[i] = 0.0f;  // #28500 2011.5.24 yyagi
						if ((((i != 0) || (TJAPlayer3.DTX.bチップがある.Drums))))
						{
							CScoreIni.C演奏記録 part = this.st演奏記録[i];
							bool bIsAutoPlay = true;
							switch (i)
							{
								case 0:
									bIsAutoPlay = TJAPlayer3.ConfigIni.b太鼓パートAutoPlay;
									break;

								case 1:
									bIsAutoPlay = TJAPlayer3.ConfigIni.b太鼓パートAutoPlay;
									break;

								case 2:
									bIsAutoPlay = TJAPlayer3.ConfigIni.b太鼓パートAutoPlay;
									break;
							}
							this.fPerfect率[i] = bIsAutoPlay ? 0f : ((100f * part.nPerfect数) / ((float)part.n全チップ数));
							this.fGreat率[i] = bIsAutoPlay ? 0f : ((100f * part.nGreat数) / ((float)part.n全チップ数));
							this.fGood率[i] = bIsAutoPlay ? 0f : ((100f * part.nGood数) / ((float)part.n全チップ数));
							this.fPoor率[i] = bIsAutoPlay ? 0f : ((100f * part.nPoor数) / ((float)part.n全チップ数));
							this.fMiss率[i] = bIsAutoPlay ? 0f : ((100f * part.nMiss数) / ((float)part.n全チップ数));
							this.bオート[i] = bIsAutoPlay; // #23596 10.11.16 add ikanick そのパートがオートなら1
														//        10.11.17 change (int to bool) ikanick
							this.nランク値[i] = CScoreIni.tランク値を計算して返す(part);
						}
					}
					this.n総合ランク値 = CScoreIni.t総合ランク値を計算して返す(this.st演奏記録.Drums, this.st演奏記録.Guitar, this.st演奏記録.Bass);
					//---------------------
					#endregion

					#region [ .score.ini の作成と出力 ]
					//---------------------
					string str = TJAPlayer3.DTX.strファイル名の絶対パス + ".score.ini";
					CScoreIni ini = new CScoreIni(str);

					bool[] b今までにフルコンボしたことがある = new bool[] { false, false, false };

					// フルコンボチェックならびに新記録ランクチェックは、ini.Record[] が、スコアチェックや演奏型スキルチェックの IF 内で書き直されてしまうよりも前に行う。(2010.9.10)

					b今までにフルコンボしたことがある[0] = ini.stセクション[0].bフルコンボである | ini.stセクション[0].bフルコンボである;

					// #24459 上記の条件だと[HiSkill.***]でのランクしかチェックしていないので、BestRankと比較するよう変更。
					if (this.nランク値[0] >= 0 && ini.stファイル.BestRank[0] > this.nランク値[0])       // #24459 2011.3.1 yyagi update BestRank
					{
						this.b新記録ランク[0] = true;
						ini.stファイル.BestRank[0] = this.nランク値[0];
					}

					// 新記録スコアチェック
					if ((this.st演奏記録[0].nスコア > ini.stセクション[0].nスコア) && !TJAPlayer3.ConfigIni.b太鼓パートAutoPlay)
					{
						this.b新記録スコア[0] = true;
						ini.stセクション[0] = this.st演奏記録[0];
					}

					if (TJAPlayer3.stage選曲.n確定された曲の難易度 != (int)Difficulty.Dan && TJAPlayer3.stage選曲.n確定された曲の難易度 != (int)Difficulty.Tower)
						if (this.st演奏記録[0].nスコア > ini.stセクション[0].nスコア)
							this.st演奏記録[0].nハイスコア[TJAPlayer3.stage選曲.n確定された曲の難易度] = (int)st演奏記録[0].nスコア;

					// 新記録スキルチェック
					if (this.st演奏記録[0].db演奏型スキル値 > ini.stセクション[0].db演奏型スキル値)
					{
						this.b新記録スキル[0] = true;
						ini.stセクション[0] = this.st演奏記録[0];
					}

					if (TJAPlayer3.stage選曲.n確定された曲の難易度 != (int)Difficulty.Dan && TJAPlayer3.stage選曲.n確定された曲の難易度 != (int)Difficulty.Tower && !TJAPlayer3.ConfigIni.b太鼓パートAutoPlay)
					{ 	
						for(int i = 0; i < 7; i++)
						{
							if (st演奏記録[0].nスコア >= TJAPlayer3.stage演奏ドラム画面.actScoreRank.ScoreRank[i])
								if(st演奏記録[0].nスコア >= 500000)
									this.st演奏記録[0].nScoreRank[TJAPlayer3.stage選曲.n確定された曲の難易度] = i;
						}
						if (st演奏記録[0].fゲージ >= 80.0f)
						{
							if (st演奏記録[0].nMiss数 == 0 && st演奏記録[0].nPoor数 == 0 && st演奏記録[0].nGreat数 == 0 && st演奏記録[0].fゲージ >= 100.0f)
							{
								this.st演奏記録[0].bIsDondaFullCombo[TJAPlayer3.stage選曲.n確定された曲の難易度] = true;
								this.st演奏記録[0].bIsFullCombo[TJAPlayer3.stage選曲.n確定された曲の難易度] = false;
								this.st演奏記録[0].bIsClear[TJAPlayer3.stage選曲.n確定された曲の難易度] = false;
							}
							else if (st演奏記録[0].nMiss数 == 0 && st演奏記録[0].nPoor数 == 0 && st演奏記録[0].fゲージ >= 100.0f)
							{
								this.st演奏記録[0].bIsDondaFullCombo[TJAPlayer3.stage選曲.n確定された曲の難易度] = false;
								this.st演奏記録[0].bIsFullCombo[TJAPlayer3.stage選曲.n確定された曲の難易度] = true;
								this.st演奏記録[0].bIsClear[TJAPlayer3.stage選曲.n確定された曲の難易度] = false;
							}
							else
							{
								this.st演奏記録[0].bIsDondaFullCombo[TJAPlayer3.stage選曲.n確定された曲の難易度] = false;
								this.st演奏記録[0].bIsFullCombo[TJAPlayer3.stage選曲.n確定された曲の難易度] = false;
								this.st演奏記録[0].bIsClear[TJAPlayer3.stage選曲.n確定された曲の難易度] = true;
							}
							ini.stセクション[0] = this.st演奏記録[0];
						}
					}

					// ラストプレイ #23595 2011.1.9 ikanick
					// オートじゃなければプレイ結果を書き込む
					if (TJAPlayer3.ConfigIni.b太鼓パートAutoPlay == false)
					{
						ini.stセクション[0] = this.st演奏記録[0];
					}

					// #23596 10.11.16 add ikanick オートじゃないならクリア回数を1増やす
					//        11.02.05 bオート to t更新条件を取得する use      ikanick
					bool[] b更新が必要か否か = new bool[3];
					CScoreIni.t更新条件を取得する(out b更新が必要か否か[0], out b更新が必要か否か[1], out b更新が必要か否か[2]);

					if (b更新が必要か否か[0])
					{
						ini.stファイル.ClearCountDrums++;
					}
					//---------------------------------------------------------------------/
					if (TJAPlayer3.ConfigIni.bScoreIniを出力する)
						ini.t書き出し(str);

					//---------------------
					#endregion

					#region [ リザルト画面への演奏回数の更新 #24281 2011.1.30 yyagi]
					if (TJAPlayer3.ConfigIni.bScoreIniを出力する)
					{
						this.n演奏回数.Drums = ini.stファイル.PlayCountDrums;
						this.n演奏回数.Guitar = ini.stファイル.PlayCountGuitar;
						this.n演奏回数.Bass = ini.stファイル.PlayCountBass;
					}
					#endregion
				}

				#region [ 選曲画面の譜面情報の更新 ]
				//---------------------
				if (!TJAPlayer3.bコンパクトモード)
				{
					Cスコア cスコア = TJAPlayer3.stage選曲.r確定されたスコア;
					if(st演奏記録[0].bIsClear[TJAPlayer3.stage選曲.n確定された曲の難易度] == true)
						cスコア.譜面情報.クリア[TJAPlayer3.stage選曲.n確定された曲の難易度] = st演奏記録[0].bIsClear[TJAPlayer3.stage選曲.n確定された曲の難易度];

					if (st演奏記録[0].bIsFullCombo[TJAPlayer3.stage選曲.n確定された曲の難易度] == true)
						cスコア.譜面情報.フルコンボ[TJAPlayer3.stage選曲.n確定された曲の難易度] = st演奏記録[0].bIsFullCombo[TJAPlayer3.stage選曲.n確定された曲の難易度];

					if (st演奏記録[0].bIsDondaFullCombo[TJAPlayer3.stage選曲.n確定された曲の難易度] == true)
						cスコア.譜面情報.ドンダフルコンボ[TJAPlayer3.stage選曲.n確定された曲の難易度] = st演奏記録[0].bIsDondaFullCombo[TJAPlayer3.stage選曲.n確定された曲の難易度];

					if (cスコア.譜面情報.nスコアランク[TJAPlayer3.stage選曲.n確定された曲の難易度] <= st演奏記録[0].nScoreRank[TJAPlayer3.stage選曲.n確定された曲の難易度] + 1)
						cスコア.譜面情報.nスコアランク[TJAPlayer3.stage選曲.n確定された曲の難易度] = st演奏記録[0].nScoreRank[TJAPlayer3.stage選曲.n確定された曲の難易度] + 1;
				} 
                //---------------------
                #endregion

                // Discord Presenseの更新
                Discord.UpdatePresence(TJAPlayer3.DTX.TITLE + ".tja", Properties.Discord.Stage_Result + (TJAPlayer3.ConfigIni.b太鼓パートAutoPlay == true ? " (" + Properties.Discord.Info_IsAuto + ")" : ""), TJAPlayer3.StartupTime);

                base.On活性化();
			}
			finally
			{
				Trace.TraceInformation( "結果ステージの活性化を完了しました。" );
				Trace.Unindent();
			}
		}
		public override void On非活性化()
		{
			if( this.rResultSound != null )
			{
				TJAPlayer3.Sound管理.tサウンドを破棄する( this.rResultSound );
				this.rResultSound = null;
			}
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				b音声再生 = false;
				this.EndAnime = false;
				//this.tx背景 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\8_background.png" ) );
				//this.tx上部パネル = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\8_header.png" ) );
				//this.tx下部パネル = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\8_footer panel.png" ), true );
				//this.txオプションパネル = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\Screen option panels.png" ) );
				base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
				if( this.ct登場用 != null )
				{
					this.ct登場用 = null;
				}
				//CDTXMania.tテクスチャの解放( ref this.tx背景 );
				//CDTXMania.tテクスチャの解放( ref this.tx上部パネル );
				//CDTXMania.tテクスチャの解放( ref this.tx下部パネル );
				//CDTXMania.tテクスチャの解放( ref this.txオプションパネル );
				base.OnManagedリソースの解放();
			}
		}

		private void ExitResultScreen()
        {
			TJAPlayer3.Skin.bgmリザルト音.t停止する();
			actFI.tフェードアウト開始();
			base.eフェーズID = CStage.Eフェーズ.共通_フェードアウト;
			this.eフェードアウト完了時の戻り値 = E戻り値.完了;
		}

		public override int On進行描画()
		{
			if( !base.b活性化してない )
			{
				int num;
				if( base.b初めての進行描画 )
				{
					this.ct登場用 = new CCounter( 0, 100, 5, TJAPlayer3.Timer );
					this.ctAutoReturn = new CCounter(0, 15, 1000, TJAPlayer3.Timer);
					this.ctAutoReturn.t停止();
					this.actFI.tフェードイン開始();
					base.eフェーズID = CStage.Eフェーズ.共通_フェードイン;
					if( this.rResultSound != null )
					{
						this.rResultSound.t再生を開始する();
					}
					base.b初めての進行描画 = false;
				}
				this.bアニメが完了 = true;
				if( this.ct登場用.b進行中 )
				{
					this.ct登場用.t進行();
					if( this.ct登場用.b終了値に達した )
					{
						this.ct登場用.t停止();
					}
					else
					{
						this.bアニメが完了 = false;
					}
				}

				if(this.actParameterPanel.ct全体アニメ.n現在の値 > 2000 + (this.actParameterPanel.ctゲージアニメーション.n終了値 * 66) + 8360 - 85)
                {
					if (!this.ctAutoReturn.b進行中)
						this.ctAutoReturn.t開始(0, 15, 1000, TJAPlayer3.Timer);
					this.ctAutoReturn.t進行();
                }

				// 描画

				if (TJAPlayer3.Tx.Result_Background != null)
				{
					if (this.actParameterPanel.ct全体アニメ.n現在の値 >= 2000 + (this.actParameterPanel.ctゲージアニメーション.n終了値 * 66) + 8360 - 85)
					{
						if(this.st演奏記録.Drums.fゲージ >= 80.0)
                        {
							TJAPlayer3.Tx.Result_Background[1].Opacity = (this.actParameterPanel.ct全体アニメ.n現在の値 - (10275 + (this.actParameterPanel.ctゲージアニメーション.n終了値 * 66))) * 3;
							TJAPlayer3.Tx.Result_Mountain[1].Opacity = (this.actParameterPanel.ct全体アニメ.n現在の値 - (10275 + (this.actParameterPanel.ctゲージアニメーション.n終了値 * 66))) * 3;
							TJAPlayer3.Tx.Result_Mountain[0].Opacity = 255 - (this.actParameterPanel.ct全体アニメ.n現在の値 - (10275 + (this.actParameterPanel.ctゲージアニメーション.n終了値 * 66))) * 3;

							if (this.actParameterPanel.ctMountain_ClearIn.n現在の値 <= 90)
                            {
								TJAPlayer3.Tx.Result_Mountain[1].vc拡大縮小倍率.Y = 1.0f - (float)Math.Sin((float)this.actParameterPanel.ctMountain_ClearIn.n現在の値 * (Math.PI / 180)) * 0.18f; 
							}
							else if (this.actParameterPanel.ctMountain_ClearIn.n現在の値 <= 225)
							{
								TJAPlayer3.Tx.Result_Mountain[1].vc拡大縮小倍率.Y = 0.82f + (float)Math.Sin((float)(this.actParameterPanel.ctMountain_ClearIn.n現在の値 - 90) / 1.5f * (Math.PI / 180)) * 0.58f;
							}
							else if (this.actParameterPanel.ctMountain_ClearIn.n現在の値 <= 245)
							{
								TJAPlayer3.Tx.Result_Mountain[1].vc拡大縮小倍率.Y = 1.4f;
							}
							else if (this.actParameterPanel.ctMountain_ClearIn.n現在の値 <= 335)
							{
								TJAPlayer3.Tx.Result_Mountain[1].vc拡大縮小倍率.Y = 0.9f + (float)Math.Sin((float)(this.actParameterPanel.ctMountain_ClearIn.n現在の値 - 155) * (Math.PI / 180)) * 0.5f;
							}
							else if (this.actParameterPanel.ctMountain_ClearIn.n現在の値 <= 515)
							{
								TJAPlayer3.Tx.Result_Mountain[1].vc拡大縮小倍率.Y = 0.9f + (float)Math.Sin((float)(this.actParameterPanel.ctMountain_ClearIn.n現在の値 - 335) * (Math.PI / 180)) * 0.4f;
							}
						}
					}
                    else
					{
						
						TJAPlayer3.Tx.Result_Background[1].Opacity = 0;
						TJAPlayer3.Tx.Result_Mountain[0].Opacity = 255;
						TJAPlayer3.Tx.Result_Mountain[1].Opacity = 0;
					}

					TJAPlayer3.Tx.Result_Background[0].t2D描画(TJAPlayer3.app.Device, 0, 0);
					TJAPlayer3.Tx.Result_Background[1].t2D描画(TJAPlayer3.app.Device, 0, 0);
					TJAPlayer3.Tx.Result_Mountain[0].t2D描画(TJAPlayer3.app.Device, 0, 0);
					TJAPlayer3.Tx.Result_Mountain[1].t2D拡大率考慮下基準描画(TJAPlayer3.app.Device, 0, 720);
				}
				

				if( this.ct登場用.b進行中 && ( TJAPlayer3.Tx.Result_Header != null ) )
				{
					double num2 = ( (double) this.ct登場用.n現在の値 ) / 100.0;
					double num3 = Math.Sin( Math.PI / 2 * num2 );
					num = ( (int) ( TJAPlayer3.Tx.Result_Header.sz画像サイズ.Height * num3 ) ) - TJAPlayer3.Tx.Result_Header.sz画像サイズ.Height;
				}
				else
				{
					num = 0;
				}
				if(!b音声再生 && !TJAPlayer3.Skin.bgmリザルトイン音.b再生中)
                {
					TJAPlayer3.Skin.bgmリザルト音.t再生する();
					b音声再生 = true;
				}

				if (TJAPlayer3.Tx.Result_Header != null )
				{
                    TJAPlayer3.Tx.Result_Header.t2D描画( TJAPlayer3.app.Device, 0, 0 );
				}
                if ( this.actResultImage.On進行描画() == 0 )
				{
					this.bアニメが完了 = false;
				}
				if ( this.actParameterPanel.On進行描画() == 0 )
				{
					this.bアニメが完了 = false;
				}

				if ( this.actSongBar.On進行描画() == 0 )
				{
					this.bアニメが完了 = false;
				}

                #region ネームプレート
                for (int i = 0; i < TJAPlayer3.ConfigIni.nPlayerCount; i++)
                {
                    if (TJAPlayer3.Tx.NamePlate[i] != null)
                    {
                        TJAPlayer3.Tx.NamePlate[i].t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Result_NamePlate_X[i], TJAPlayer3.Skin.Result_NamePlate_Y[i]);
                    }
                }
                #endregion

                if ( base.eフェーズID == CStage.Eフェーズ.共通_フェードイン )
				{
					if( this.actFI.On進行描画() != 0 )
					{
						base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
					}
				}
				else if( ( base.eフェーズID == CStage.Eフェーズ.共通_フェードアウト ) )			//&& ( this.actFO.On進行描画() != 0 ) )
				{
					return (int) this.eフェードアウト完了時の戻り値;
				}
				#region [ #24609 2011.3.14 yyagi ランク更新or演奏型スキル更新時、リザルト画像をpngで保存する ]
				if ( this.bアニメが完了 == true && this.bIsCheckedWhetherResultScreenShouldSaveOrNot == false	// #24609 2011.3.14 yyagi; to save result screen in case BestRank or HiSkill.
					&& TJAPlayer3.ConfigIni.bScoreIniを出力する
					&& TJAPlayer3.ConfigIni.bIsAutoResultCapture)												// #25399 2011.6.9 yyagi
				{
					CheckAndSaveResultScreen(true);
					this.bIsCheckedWhetherResultScreenShouldSaveOrNot = true;
				}
				#endregion

				if(this.ctAutoReturn.b終了値に達した)
					this.ExitResultScreen(); ;

				// キー入力

				if ( TJAPlayer3.act現在入力を占有中のプラグイン == null )
				{
					#region [ #24609 2011.4.7 yyagi リザルト画面で[F12]を押下すると、リザルト画像をpngで保存する機能は、CDTXManiaに移管。 ]
//					if ( CDTXMania.Input管理.Keyboard.bキーが押された( (int) SlimDX.DirectInput.Key.F12 ) &&
//						CDTXMania.ConfigIni.bScoreIniを出力する )
//					{
//						CheckAndSaveResultScreen(false);
//						this.bIsCheckedWhetherResultScreenShouldSaveOrNot = true;
//					}
					#endregion
					if ( base.eフェーズID == CStage.Eフェーズ.共通_通常状態 )
					{
						if ( TJAPlayer3.Input管理.Keyboard.bキーが押された( (int)SlimDX.DirectInput.Key.Escape ) )
						{
							this.ExitResultScreen();
						}
                        if (TJAPlayer3.Pad.b押されたDGB(Eパッド.CY) || TJAPlayer3.Pad.b押された(E楽器パート.DRUMS, Eパッド.RD) || TJAPlayer3.Pad.b押された(E楽器パート.DRUMS, Eパッド.LC) || TJAPlayer3.Pad.b押されたDGB(Eパッド.LRed) || TJAPlayer3.Pad.b押されたDGB(Eパッド.RRed) || TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.Return))
						{
                            if (this.actParameterPanel.ct全体アニメ.n現在の値 <= 2000 + (this.actParameterPanel.ctゲージアニメーション.n終了値 * 66) + 8360 - 85)
                            {
								this.actParameterPanel.ct全体アニメ.n現在の値 = 2000 + (this.actParameterPanel.ctゲージアニメーション.n終了値 * 66) + 8360 - 85;
								this.actParameterPanel.ctゲージアニメーション.n現在の値 = actParameterPanel.ctゲージアニメーション.n終了値;
								TJAPlayer3.Skin.sound決定音.t再生する();
								EndAnime = true;
								this.actParameterPanel.ctEndAnime.t開始(0, 360, 1, TJAPlayer3.Timer);
								this.actParameterPanel.ctMountain_ClearIn.t開始(0, 515, 3, TJAPlayer3.Timer);
							}
                            else
							{
								this.ExitResultScreen();
							}
						}
					}
				}
			}
			return 0;
		}

		public enum E戻り値 : int
		{
			継続,
			完了
		}


		// その他

		#region [ private ]
		//-----------------
		public bool b音声再生;
		public bool EndAnime;
		private CCounter ct登場用;
		private E戻り値 eフェードアウト完了時の戻り値;
		private CActFIFOResult actFI;
		private CActFIFOBlack actFO;
		private CActオプションパネル actOption;
		private CActResultParameterPanel actParameterPanel;
		private CActResultRank actRank;
		private CActResultImage actResultImage;
		private CActResultSongBar actSongBar;
		private bool bアニメが完了;
		private bool bIsCheckedWhetherResultScreenShouldSaveOrNot;				// #24509 2011.3.14 yyagi
		private readonly int[] nチャンネル0Atoレーン07;
		private int n最後に再生したHHのWAV番号;
		private int n最後に再生したHHのチャンネル番号;
		private CSound rResultSound;
		private CCounter ctAutoReturn;
		//private CTexture txオプションパネル;
		//private CTexture tx下部パネル;
		//private CTexture tx上部パネル;
		//private CTexture tx背景;

		#region [ #24609 リザルト画像をpngで保存する ]		// #24609 2011.3.14 yyagi; to save result screen in case BestRank or HiSkill.
		/// <summary>
		/// リザルト画像のキャプチャと保存。
		/// 自動保存モード時は、ランク更新or演奏型スキル更新時に自動保存。
		/// 手動保存モード時は、ランクに依らず保存。
		/// </summary>
		/// <param name="bIsAutoSave">true=自動保存モード, false=手動保存モード</param>
		private void CheckAndSaveResultScreen(bool bIsAutoSave)
		{
			string path = Path.GetDirectoryName( TJAPlayer3.DTX.strファイル名の絶対パス );
			string datetime = DateTime.Now.ToString( "yyyyMMddHHmmss" );
			if ( bIsAutoSave )
			{
				// リザルト画像を自動保存するときは、dtxファイル名.yyMMddHHmmss_DRUMS_SS.png という形式で保存。
				for ( int i = 0; i < 3; i++ )
				{
					if ( this.b新記録ランク[ i ] == true || this.b新記録スキル[ i ] == true )
					{
						string strPart = ( (E楽器パート) ( i ) ).ToString();
						string strRank = ( (CScoreIni.ERANK) ( this.nランク値[ i ] ) ).ToString();
						string strFullPath = TJAPlayer3.DTX.strファイル名の絶対パス + "." + datetime + "_" + strPart + "_" + strRank + ".png";
						//Surface.ToFile( pSurface, strFullPath, ImageFileFormat.Png );
						TJAPlayer3.app.SaveResultScreen( strFullPath );
					}
				}
			}
			#region [ #24609 2011.4.11 yyagi; リザルトの手動保存ロジックは、CDTXManiaに移管した。]
//			else
//			{
//				// リザルト画像を手動保存するときは、dtxファイル名.yyMMddHHmmss_SS.png という形式で保存。(楽器名無し)
//				string strRank = ( (CScoreIni.ERANK) ( CDTXMania.stage結果.n総合ランク値 ) ).ToString();
//				string strSavePath = CDTXMania.strEXEのあるフォルダ + "\\" + "Capture_img";
//				if ( !Directory.Exists( strSavePath ) )
//				{
//					try
//					{
//						Directory.CreateDirectory( strSavePath );
//					}
//					catch
//					{
//					}
//				}
//				string strFullPath = strSavePath + "\\" + CDTXMania.DTX.TITLE +
//					"." + datetime + "_" + strRank + ".png";
//				// Surface.ToFile( pSurface, strFullPath, ImageFileFormat.Png );
//				CDTXMania.app.SaveResultScreen( strFullPath );
//			}
			#endregion
		}
		#endregion
		//-----------------
		#endregion
	}
}
