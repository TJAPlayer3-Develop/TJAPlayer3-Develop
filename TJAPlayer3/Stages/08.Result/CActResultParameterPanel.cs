using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;
using SlimDX;
using FDK;

namespace TJAPlayer3
{
	internal class CActResultParameterPanel : CActivity
	{
		// コンストラクタ

		public CActResultParameterPanel()
		{
			ST文字位置[] st文字位置Array = new ST文字位置[ 11 ];
			ST文字位置 st文字位置 = new ST文字位置();
			st文字位置.ch = '0';
			st文字位置.pt = new Point( 0, 0 );
			st文字位置Array[ 0 ] = st文字位置;
			ST文字位置 st文字位置2 = new ST文字位置();
			st文字位置2.ch = '1';
			st文字位置2.pt = new Point( 32, 0 );
			st文字位置Array[ 1 ] = st文字位置2;
			ST文字位置 st文字位置3 = new ST文字位置();
			st文字位置3.ch = '2';
			st文字位置3.pt = new Point( 64, 0 );
			st文字位置Array[ 2 ] = st文字位置3;
			ST文字位置 st文字位置4 = new ST文字位置();
			st文字位置4.ch = '3';
			st文字位置4.pt = new Point( 96, 0 );
			st文字位置Array[ 3 ] = st文字位置4;
			ST文字位置 st文字位置5 = new ST文字位置();
			st文字位置5.ch = '4';
			st文字位置5.pt = new Point( 128, 0 );
			st文字位置Array[ 4 ] = st文字位置5;
			ST文字位置 st文字位置6 = new ST文字位置();
			st文字位置6.ch = '5';
			st文字位置6.pt = new Point( 160, 0 );
			st文字位置Array[ 5 ] = st文字位置6;
			ST文字位置 st文字位置7 = new ST文字位置();
			st文字位置7.ch = '6';
			st文字位置7.pt = new Point( 192, 0 );
			st文字位置Array[ 6 ] = st文字位置7;
			ST文字位置 st文字位置8 = new ST文字位置();
			st文字位置8.ch = '7';
			st文字位置8.pt = new Point( 224, 0 );
			st文字位置Array[ 7 ] = st文字位置8;
			ST文字位置 st文字位置9 = new ST文字位置();
			st文字位置9.ch = '8';
			st文字位置9.pt = new Point( 256, 0 );
			st文字位置Array[ 8 ] = st文字位置9;
			ST文字位置 st文字位置10 = new ST文字位置();
			st文字位置10.ch = '9';
			st文字位置10.pt = new Point( 288, 0 );
			st文字位置Array[ 9 ] = st文字位置10;
			ST文字位置 st文字位置11 = new ST文字位置();
			st文字位置11.ch = ' ';
			st文字位置11.pt = new Point( 0, 0 );
			st文字位置Array[ 10 ] = st文字位置11;
			this.st小文字位置 = st文字位置Array;

			ST文字位置[] st文字位置Array2 = new ST文字位置[ 11 ];
			ST文字位置 st文字位置12 = new ST文字位置();
			st文字位置12.ch = '0';
			st文字位置12.pt = new Point( 0, 0 );
			st文字位置Array2[ 0 ] = st文字位置12;
			ST文字位置 st文字位置13 = new ST文字位置();
			st文字位置13.ch = '1';
			st文字位置13.pt = new Point( 32, 0 );
			st文字位置Array2[ 1 ] = st文字位置13;
			ST文字位置 st文字位置14 = new ST文字位置();
			st文字位置14.ch = '2';
			st文字位置14.pt = new Point( 64, 0 );
			st文字位置Array2[ 2 ] = st文字位置14;
			ST文字位置 st文字位置15 = new ST文字位置();
			st文字位置15.ch = '3';
			st文字位置15.pt = new Point( 96, 0 );
			st文字位置Array2[ 3 ] = st文字位置15;
			ST文字位置 st文字位置16 = new ST文字位置();
			st文字位置16.ch = '4';
			st文字位置16.pt = new Point( 128, 0 );
			st文字位置Array2[ 4 ] = st文字位置16;
			ST文字位置 st文字位置17 = new ST文字位置();
			st文字位置17.ch = '5';
			st文字位置17.pt = new Point( 160, 0 );
			st文字位置Array2[ 5 ] = st文字位置17;
			ST文字位置 st文字位置18 = new ST文字位置();
			st文字位置18.ch = '6';
			st文字位置18.pt = new Point( 192, 0 );
			st文字位置Array2[ 6 ] = st文字位置18;
			ST文字位置 st文字位置19 = new ST文字位置();
			st文字位置19.ch = '7';
			st文字位置19.pt = new Point( 224, 0 );
			st文字位置Array2[ 7 ] = st文字位置19;
			ST文字位置 st文字位置20 = new ST文字位置();
			st文字位置20.ch = '8';
			st文字位置20.pt = new Point( 256, 0 );
			st文字位置Array2[ 8 ] = st文字位置20;
			ST文字位置 st文字位置21 = new ST文字位置();
			st文字位置21.ch = '9';
			st文字位置21.pt = new Point( 288, 0 );
			st文字位置Array2[ 9 ] = st文字位置21;
			ST文字位置 st文字位置22 = new ST文字位置();
			st文字位置22.ch = '%';
			st文字位置22.pt = new Point( 0x37, 0 );
			st文字位置Array2[ 10 ] = st文字位置22;
			this.st大文字位置 = st文字位置Array2;

            ST文字位置[] stScore文字位置Array = new ST文字位置[10];
            ST文字位置 stScore文字位置 = new ST文字位置();
            stScore文字位置.ch = '0';
            stScore文字位置.pt = new Point(0, 0);
            stScore文字位置Array[0] = stScore文字位置;
            ST文字位置 stScore文字位置2 = new ST文字位置();
            stScore文字位置2.ch = '1';
            stScore文字位置2.pt = new Point(51, 0);
            stScore文字位置Array[1] = stScore文字位置2;
            ST文字位置 stScore文字位置3 = new ST文字位置();
            stScore文字位置3.ch = '2';
            stScore文字位置3.pt = new Point(102, 0);
            stScore文字位置Array[2] = stScore文字位置3;
            ST文字位置 stScore文字位置4 = new ST文字位置();
            stScore文字位置4.ch = '3';
            stScore文字位置4.pt = new Point(153, 0);
            stScore文字位置Array[3] = stScore文字位置4;
            ST文字位置 stScore文字位置5 = new ST文字位置();
            stScore文字位置5.ch = '4';
            stScore文字位置5.pt = new Point(204, 0);
            stScore文字位置Array[4] = stScore文字位置5;
            ST文字位置 stScore文字位置6 = new ST文字位置();
            stScore文字位置6.ch = '5';
            stScore文字位置6.pt = new Point(255, 0);
            stScore文字位置Array[5] = stScore文字位置6;
            ST文字位置 stScore文字位置7 = new ST文字位置();
            stScore文字位置7.ch = '6';
            stScore文字位置7.pt = new Point(306, 0);
            stScore文字位置Array[6] = stScore文字位置7;
            ST文字位置 stScore文字位置8 = new ST文字位置();
            stScore文字位置8.ch = '7';
            stScore文字位置8.pt = new Point(357, 0);
            stScore文字位置Array[7] = stScore文字位置8;
            ST文字位置 stScore文字位置9 = new ST文字位置();
            stScore文字位置9.ch = '8';
            stScore文字位置9.pt = new Point(408, 0);
            stScore文字位置Array[8] = stScore文字位置9;
            ST文字位置 stScore文字位置10 = new ST文字位置();
            stScore文字位置10.ch = '9';
            stScore文字位置10.pt = new Point(459, 0);
            stScore文字位置Array[9] = stScore文字位置10;
            this.stScoreFont = stScore文字位置Array;



			this.ptFullCombo位置 = new Point[] { new Point( 0x80, 0xed ), new Point( 0xdf, 0xed ), new Point( 0x141, 0xed ) };
			base.b活性化してない = true;
		}


		// メソッド

		public void tアニメを完了させる()
		{
			this.ct表示用.n現在の値 = this.ct表示用.n終了値;
		}


		// CActivity 実装

		public override void On活性化()
		{

			this.sdDTXで指定されたフルコンボ音 = null;
			this.bフルコンボ音再生済み = false;
			this.b音声再生 = new bool[7];
			for (int i = 0; i < 7; i++)
				this.b音声再生[i] = false;
			base.On活性化();
		}
		public override void On非活性化()
		{
			if( this.ct表示用 != null )
			{
				this.ct表示用 = null;
			}
			if( this.sdDTXで指定されたフルコンボ音 != null )
			{
				TJAPlayer3.Sound管理.tサウンドを破棄する( this.sdDTXで指定されたフルコンボ音 );
				this.sdDTXで指定されたフルコンボ音 = null;
			}
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
                ctゲージアニメーション = new CCounter();
                ct全体アニメ = new CCounter(0, 13660, 1, TJAPlayer3.Timer);
				ctSoulFlash = new CCounter(0, 1, 33, TJAPlayer3.Timer);
				ctSoul = new CCounter(0, 8, 33, TJAPlayer3.Timer);
				ctRainbowGauge = new CCounter(0, 40, 1000 / 60, TJAPlayer3.Timer);
				ctEndAnime = new CCounter();
				ctBackgroundAnime = new CCounter(0, 1000, 1, TJAPlayer3.Timer);
				ctBackgroundAnime_Clear = new CCounter(0, 1000, 1, TJAPlayer3.Timer);
				ctMountain_ClearIn = new CCounter();

				ctDonchan_Normal = new CCounter(0, TJAPlayer3.Tx.Result_Donchan_Normal.Length - 1, 1000 / 60, TJAPlayer3.Timer);
				ctDonchan_Clear = new CCounter();

				Dan_Plate = TJAPlayer3.tテクスチャの生成(Path.GetDirectoryName(TJAPlayer3.DTX.strファイル名の絶対パス) + @"\Dan_Plate.png");
                base.OnManagedリソースの作成();
			}
		}
		public override void OnManagedリソースの解放()
		{
			if( !base.b活性化してない )
			{
                Dan_Plate?.Dispose();
                base.OnManagedリソースの解放();
			}
		}
		public override int On進行描画()
		{
			if( base.b活性化してない )
			{
				return 0;
			}
			if( base.b初めての進行描画 )
			{
				this.ct表示用 = new CCounter( 0, 0x3e7, 2, TJAPlayer3.Timer );
				base.b初めての進行描画 = false;
			}
			this.ct表示用.t進行();
            ct全体アニメ.t進行();
            ctゲージアニメーション.t進行();
			ctEndAnime.t進行();
			ctBackgroundAnime.t進行Loop();
			ctMountain_ClearIn.t進行();
			ctDonchan_Clear.t進行Loop();
			ctDonchan_Normal.t進行Loop();
			if (TJAPlayer3.Tx.Result_Panel != null)
			{
				int[] y補正 = { 0, 1, 0, 11, 12};
				TJAPlayer3.Tx.Result_Panel.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.nResultPanelP1X, TJAPlayer3.Skin.nResultPanelP1Y);
				TJAPlayer3.Tx.Result_Diff_Bar.t2D描画(TJAPlayer3.app.Device, 27, 120 - y補正[TJAPlayer3.stage選曲.n確定された曲の難易度], new Rectangle(0, TJAPlayer3.stage選曲.n確定された曲の難易度 * 50, 153, 50));
			}
			if (TJAPlayer3.Tx.Result_Gauge_Base != null && TJAPlayer3.Tx.Result_Gauge != null)
			{
				//int nRectX = (int)( CDTXMania.stage結果.st演奏記録.Drums.fゲージ / 2) * 12;
				//nRectX = CDTXMania.stage結果.st演奏記録.Drums.fゲージ >= 80.0f ? 80 : nRectX;
				TJAPlayer3.Tx.Result_Gauge_Base.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.nResultGaugeBaseP1X, TJAPlayer3.Skin.nResultGaugeBaseP1Y);
				if (ctゲージアニメーション.n現在の値 <= 49)
				{
					TJAPlayer3.Tx.Result_Gauge.t2D描画(TJAPlayer3.app.Device, 57, 141, new RectangleF(3, 0, 9.78f * ctゲージアニメーション.n現在の値, 37));

					if(ctゲージアニメーション.n現在の値 < 40)
						TJAPlayer3.Tx.Result_Gauge.t2D描画(TJAPlayer3.app.Device, 441, 145, new RectangleF(45, 38, 40, 17));
					else
						TJAPlayer3.Tx.Result_Gauge.t2D描画(TJAPlayer3.app.Device, 441, 145, new RectangleF(2, 38, 37, 17));
				}
                else
                {
					ctRainbowGauge.t進行Loop();
					TJAPlayer3.Tx.Result_Rainbow[ctRainbowGauge.n現在の値].t2D描画(TJAPlayer3.app.Device, 57, 145);
				}
			}
			if (ct全体アニメ.n現在の値 >= 2000)
				if (!ctゲージアニメーション.b進行中)
				{
					ctゲージアニメーション.t開始(0, (int)Math.Floor(TJAPlayer3.stage結果.st演奏記録.Drums.fゲージ) / 2, 66, TJAPlayer3.Timer);
					if (!this.b音声再生[6])
					{
						TJAPlayer3.Skin.soundResultGauge.t再生する();
						this.b音声再生[6] = true;
					}
				}

			if (ctゲージアニメーション.n現在の値 <= 49 || TJAPlayer3.stage結果.EndAnime)
			{
				if (ctゲージアニメーション.b終了値に達した)
				{
					if (TJAPlayer3.Skin.soundResultGauge.b再生中)
						TJAPlayer3.Skin.soundResultGauge.t停止する();
				}
			}

			if (TJAPlayer3.stage結果.EndAnime)
			{
				{
					TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.X = this.ctEndAnime.n現在の値 <= 270 ? 1.3f - (float)Math.Sin((ctEndAnime.n現在の値) / 3 * (Math.PI / 180)) * 0.3f : 1.0f;
					TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.Y = this.ctEndAnime.n現在の値 <= 270 ? 1.3f - (float)Math.Sin((ctEndAnime.n現在の値) / 3 * (Math.PI / 180)) * 0.3f : 1.0f;
					this.t小文字表示(TJAPlayer3.Skin.nResultGreatP1X, TJAPlayer3.Skin.nResultGreatP1Y, string.Format("{0,4:###0}", TJAPlayer3.stage結果.st演奏記録.Drums.nPerfect数.ToString()));
					TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.X = 1;
					TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.Y = 1;
				}
				{
					TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.X = this.ctEndAnime.n現在の値 <= 270 ? 1.3f - (float)Math.Sin((ctEndAnime.n現在の値) / 3 * (Math.PI / 180)) * 0.3f : 1.0f;
					TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.Y = this.ctEndAnime.n現在の値 <= 270 ? 1.3f - (float)Math.Sin((ctEndAnime.n現在の値) / 3 * (Math.PI / 180)) * 0.3f : 1.0f;
					this.t小文字表示(TJAPlayer3.Skin.nResultGoodP1X, TJAPlayer3.Skin.nResultGoodP1Y, string.Format("{0,4:###0}", TJAPlayer3.stage結果.st演奏記録.Drums.nGreat数.ToString()));
					TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.X = 1;
					TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.Y = 1;
				}
				{
					TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.X = this.ctEndAnime.n現在の値 <= 270 ? 1.3f - (float)Math.Sin((ctEndAnime.n現在の値) / 3 * (Math.PI / 180)) * 0.3f : 1.0f;
					TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.Y = this.ctEndAnime.n現在の値 <= 270 ? 1.3f - (float)Math.Sin((ctEndAnime.n現在の値) / 3 * (Math.PI / 180)) * 0.3f : 1.0f;
					this.t小文字表示(TJAPlayer3.Skin.nResultBadP1X, TJAPlayer3.Skin.nResultBadP1Y, string.Format("{0,4:###0}", TJAPlayer3.stage結果.st演奏記録.Drums.nMiss数.ToString()));
					TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.X = 1;
					TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.Y = 1;
				}
				{
					TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.X = this.ctEndAnime.n現在の値 <= 270 ? 1.3f - (float)Math.Sin((ctEndAnime.n現在の値) / 3 * (Math.PI / 180)) * 0.3f : 1.0f;
					TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.Y = this.ctEndAnime.n現在の値 <= 270 ? 1.3f - (float)Math.Sin((ctEndAnime.n現在の値) / 3 * (Math.PI / 180)) * 0.3f : 1.0f;
					this.t小文字表示(TJAPlayer3.Skin.nResultComboP1X, TJAPlayer3.Skin.nResultComboP1Y, string.Format("{0,4:###0}", TJAPlayer3.stage結果.st演奏記録.Drums.n最大コンボ数.ToString()));
					TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.X = 1;
					TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.Y = 1;
				}
				{
					TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.X = this.ctEndAnime.n現在の値 <= 270 ? 1.3f - (float)Math.Sin((ctEndAnime.n現在の値) / 3 * (Math.PI / 180)) * 0.3f : 1.0f;
					TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.Y = this.ctEndAnime.n現在の値 <= 270 ? 1.3f - (float)Math.Sin((ctEndAnime.n現在の値) / 3 * (Math.PI / 180)) * 0.3f : 1.0f;
					this.t小文字表示(TJAPlayer3.Skin.nResultRollP1X, TJAPlayer3.Skin.nResultRollP1Y, string.Format("{0,4:###0}", TJAPlayer3.stage結果.st演奏記録.Drums.n連打数.ToString()));
					TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.X = 1;
					TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.Y = 1;
				}

				{
					TJAPlayer3.Tx.Result_Score_Number.vc拡大縮小倍率.X = this.ctEndAnime.n現在の値 <= 270 ? 1.0f + (float)Math.Sin((ctEndAnime.n現在の値 / 1.5f * (Math.PI / 180))) * 0.5f : 1.0f - (float)Math.Sin((ctEndAnime.n現在の値 - 270) * 2 * (Math.PI / 180)) * 0.05f;
					TJAPlayer3.Tx.Result_Score_Number.vc拡大縮小倍率.Y = this.ctEndAnime.n現在の値 <= 270 ? 1.0f + (float)Math.Sin((ctEndAnime.n現在の値 / 1.5f * (Math.PI / 180))) * 0.5f : 1.0f - (float)Math.Sin((ctEndAnime.n現在の値 - 270) * 2 * (Math.PI / 180)) * 0.05f;
					this.tスコア文字表示(TJAPlayer3.Skin.nResultScoreP1X, TJAPlayer3.Skin.nResultScoreP1Y, string.Format("{0,7:######0}", TJAPlayer3.stage結果.st演奏記録.Drums.nスコア));
				}

				TJAPlayer3.Tx.ScoreRank.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 135, 339, new Rectangle(0, (TJAPlayer3.stage結果.st演奏記録[0].nScoreRank[TJAPlayer3.stage選曲.n確定された曲の難易度]) * 120, 140, 120));

				if (TJAPlayer3.ConfigIni.b太鼓パートAutoPlay)
				{
					TJAPlayer3.Tx.Result_Crown[2].t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 262, 336);
				}
                else
				{
					if (TJAPlayer3.stage結果.st演奏記録[0].bIsClear[TJAPlayer3.stage選曲.n確定された曲の難易度])
					{
						TJAPlayer3.Tx.Result_Crown[0].t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 262, 336);
					}
					else if (TJAPlayer3.stage結果.st演奏記録[0].bIsFullCombo[TJAPlayer3.stage選曲.n確定された曲の難易度])
					{
						TJAPlayer3.Tx.Result_Crown[1].t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 262, 336);
					}
					else if (TJAPlayer3.stage結果.st演奏記録[0].bIsDondaFullCombo[TJAPlayer3.stage選曲.n確定された曲の難易度])
					{
						TJAPlayer3.Tx.Result_Crown[2].t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 262, 336);
					}
				}

				if (TJAPlayer3.stage結果.st演奏記録[0].fゲージ >= 80.0f)
					if (!this.ctDonchan_Clear.b進行中)
						this.ctDonchan_Clear.t開始(0, TJAPlayer3.Tx.Result_Donchan_Clear.Length - 1, 1000 / 60, TJAPlayer3.Timer);
			}
			else
			{
				if (ct全体アニメ.n現在の値 >= 2000)
				{
					if (ct全体アニメ.n現在の値 >= 2000 + (ctゲージアニメーション.n終了値 * 66) + 1000)
					{
						if (!b音声再生[0])
						{
							TJAPlayer3.Skin.soundResultPom.t再生する();
							b音声再生[0] = true;
						}
						TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.X = ct全体アニメ.n現在の値 <= 2000 + (ctゲージアニメーション.n終了値 * 66) + 1180 ? 1.3f - (float)Math.Sin((ct全体アニメ.n現在の値 - (2000 + (ctゲージアニメーション.n終了値 * 66) + 1000)) / 2 * (Math.PI / 180)) * 0.3f : 1.0f;
						TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.Y = ct全体アニメ.n現在の値 <= 2000 + (ctゲージアニメーション.n終了値 * 66) + 1180 ? 1.3f - (float)Math.Sin((ct全体アニメ.n現在の値 - (2000 + (ctゲージアニメーション.n終了値 * 66) + 1000)) / 2 * (Math.PI / 180)) * 0.3f : 1.0f;
						this.t小文字表示(TJAPlayer3.Skin.nResultGreatP1X, TJAPlayer3.Skin.nResultGreatP1Y, string.Format("{0,4:###0}", TJAPlayer3.stage結果.st演奏記録.Drums.nPerfect数.ToString()));
						TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.X = 1;
						TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.Y = 1;
					}
					if (ct全体アニメ.n現在の値 >= 2000 + (ctゲージアニメーション.n終了値 * 66) + 1500)
					{
						if (!b音声再生[1])
						{
							TJAPlayer3.Skin.soundResultPom.t再生する();
							b音声再生[1] = true;
						}
						TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.X = ct全体アニメ.n現在の値 <= 2000 + (ctゲージアニメーション.n終了値 * 66) + 1680 ? 1.3f - (float)Math.Sin((ct全体アニメ.n現在の値 - (2000 + (ctゲージアニメーション.n終了値 * 66) + 1500)) / 2 * (Math.PI / 180)) * 0.3f : 1.0f;
						TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.Y = ct全体アニメ.n現在の値 <= 2000 + (ctゲージアニメーション.n終了値 * 66) + 1680 ? 1.3f - (float)Math.Sin((ct全体アニメ.n現在の値 - (2000 + (ctゲージアニメーション.n終了値 * 66) + 1500)) / 2 * (Math.PI / 180)) * 0.3f : 1.0f;
						this.t小文字表示(TJAPlayer3.Skin.nResultGoodP1X, TJAPlayer3.Skin.nResultGoodP1Y, string.Format("{0,4:###0}", TJAPlayer3.stage結果.st演奏記録.Drums.nGreat数.ToString()));
						TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.X = 1;
						TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.Y = 1;
					}
					if (ct全体アニメ.n現在の値 >= 2000 + (ctゲージアニメーション.n終了値 * 66) + 2000)
					{
						if (!b音声再生[2])
						{
							TJAPlayer3.Skin.soundResultPom.t再生する();
							b音声再生[2] = true;
						}
						TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.X = ct全体アニメ.n現在の値 <= 2000 + (ctゲージアニメーション.n終了値 * 66) + 2180 ? 1.3f - (float)Math.Sin((ct全体アニメ.n現在の値 - (2000 + (ctゲージアニメーション.n終了値 * 66) + 2000)) / 2 * (Math.PI / 180)) * 0.3f : 1.0f;
						TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.Y = ct全体アニメ.n現在の値 <= 2000 + (ctゲージアニメーション.n終了値 * 66) + 2180 ? 1.3f - (float)Math.Sin((ct全体アニメ.n現在の値 - (2000 + (ctゲージアニメーション.n終了値 * 66) + 2000)) / 2 * (Math.PI / 180)) * 0.3f : 1.0f;
						this.t小文字表示(TJAPlayer3.Skin.nResultBadP1X, TJAPlayer3.Skin.nResultBadP1Y, string.Format("{0,4:###0}", TJAPlayer3.stage結果.st演奏記録.Drums.nMiss数.ToString()));
						TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.X = 1;
						TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.Y = 1;
					}
					if (ct全体アニメ.n現在の値 >= 2000 + (ctゲージアニメーション.n終了値 * 66) + 2500)
					{
						if (!b音声再生[3])
						{
							TJAPlayer3.Skin.soundResultPom.t再生する();
							b音声再生[3] = true;
						}
						TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.X = ct全体アニメ.n現在の値 <= 2000 + (ctゲージアニメーション.n終了値 * 66) + 2680 ? 1.3f - (float)Math.Sin((ct全体アニメ.n現在の値 - (2000 + (ctゲージアニメーション.n終了値 * 66) + 2500)) / 2 * (Math.PI / 180)) * 0.3f : 1.0f;
						TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.Y = ct全体アニメ.n現在の値 <= 2000 + (ctゲージアニメーション.n終了値 * 66) + 2680 ? 1.3f - (float)Math.Sin((ct全体アニメ.n現在の値 - (2000 + (ctゲージアニメーション.n終了値 * 66) + 2500)) / 2 * (Math.PI / 180)) * 0.3f : 1.0f;
						this.t小文字表示(TJAPlayer3.Skin.nResultRollP1X, TJAPlayer3.Skin.nResultRollP1Y, string.Format("{0,4:###0}", TJAPlayer3.stage結果.st演奏記録.Drums.n連打数.ToString()));
						TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.X = 1;
						TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.Y = 1;
					}
					if (ct全体アニメ.n現在の値 >= 2000 + (ctゲージアニメーション.n終了値 * 66) + 3000)
					{
						if (!b音声再生[4])
						{
							TJAPlayer3.Skin.soundResultPom.t再生する();
							b音声再生[4] = true;
						}
						TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.X = ct全体アニメ.n現在の値 <= 2000 + (ctゲージアニメーション.n終了値 * 66) + 3180 ? 1.3f - (float)Math.Sin((ct全体アニメ.n現在の値 - (2000 + (ctゲージアニメーション.n終了値 * 66) + 3000)) / 2 * (Math.PI / 180)) * 0.3f : 1.0f;
						TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.Y = ct全体アニメ.n現在の値 <= 2000 + (ctゲージアニメーション.n終了値 * 66) + 3180 ? 1.3f - (float)Math.Sin((ct全体アニメ.n現在の値 - (2000 + (ctゲージアニメーション.n終了値 * 66) + 3000)) / 2 * (Math.PI / 180)) * 0.3f : 1.0f;
						this.t小文字表示(TJAPlayer3.Skin.nResultComboP1X, TJAPlayer3.Skin.nResultComboP1Y, string.Format("{0,4:###0}", TJAPlayer3.stage結果.st演奏記録.Drums.n最大コンボ数.ToString()));
						TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.X = 1;
						TJAPlayer3.Tx.Result_Number.vc拡大縮小倍率.Y = 1;
					}

					if(ct全体アニメ.n現在の値 >= 2000 + (ctゲージアニメーション.n終了値 * 66) + 4000)
					{
						if (!b音声再生[5])
						{
							TJAPlayer3.Skin.sound決定音.t再生する();
							b音声再生[5] = true;
						}
						TJAPlayer3.Tx.Result_Score_Number.vc拡大縮小倍率.X = ct全体アニメ.n現在の値 <= 2000 + (ctゲージアニメーション.n終了値 * 66) + 4270 ? 1.0f + (float)Math.Sin((ct全体アニメ.n現在の値 - (2000 + (ctゲージアニメーション.n終了値 * 66) + 4000)) / 1.5f * (Math.PI / 180)) * 0.5f :
																		  ct全体アニメ.n現在の値 >= 2000 + (ctゲージアニメーション.n終了値 * 66) + 4270 && ct全体アニメ.n現在の値 <= 2000 + (ctゲージアニメーション.n終了値 * 66) + 4360 ? 1.0f - (float)Math.Sin((ct全体アニメ.n現在の値 - (2000 + (ctゲージアニメーション.n終了値 * 66) + 4270)) / 1.5f * (Math.PI / 180)) * 0.05f : 1.0f;

						TJAPlayer3.Tx.Result_Score_Number.vc拡大縮小倍率.Y = ct全体アニメ.n現在の値 <= 2000 + (ctゲージアニメーション.n終了値 * 66) + 4270 ? 1.0f + (float)Math.Sin((ct全体アニメ.n現在の値 - (2000 + (ctゲージアニメーション.n終了値 * 66) + 4000)) / 1.5f * (Math.PI / 180)) * 0.5f :
																		  ct全体アニメ.n現在の値 >= 2000 + (ctゲージアニメーション.n終了値 * 66) + 4270 && ct全体アニメ.n現在の値 <= 2000 + (ctゲージアニメーション.n終了値 * 66) + 4360 ? 1.0f - (float)Math.Sin((ct全体アニメ.n現在の値 - (2000 + (ctゲージアニメーション.n終了値 * 66) + 4270)) / 1.5f * (Math.PI / 180)) * 0.05f : 1.0f;


						this.tスコア文字表示(TJAPlayer3.Skin.nResultScoreP1X, TJAPlayer3.Skin.nResultScoreP1Y, string.Format("{0,7:######0}", TJAPlayer3.stage結果.st演奏記録.Drums.nスコア));

					}

					if (ct全体アニメ.n現在の値 >= 2000 + (ctゲージアニメーション.n終了値 * 66) + 5000)
					{
						if (ct全体アニメ.n現在の値 <= 2000 + (ctゲージアニメーション.n終了値 * 66) + 5180)
							for (int i = 0; i < 3; i++)
							{
								TJAPlayer3.Tx.ScoreRank.Opacity = (ct全体アニメ.n現在の値 - (2000 + (ctゲージアニメーション.n終了値 * 66) + 5000)) / 180.0f * 255.0f;
								TJAPlayer3.Tx.ScoreRank.vc拡大縮小倍率.X = 1.0f + (float)Math.Sin((float)(ct全体アニメ.n現在の値 - (2000 + (ctゲージアニメーション.n終了値 * 66) + 4910)) / 1.5f * (Math.PI / 180)) * 1.4f;
								TJAPlayer3.Tx.ScoreRank.vc拡大縮小倍率.Y = 1.0f + (float)Math.Sin((float)(ct全体アニメ.n現在の値 - (2000 + (ctゲージアニメーション.n終了値 * 66) + 4910)) / 1.5f * (Math.PI / 180)) * 1.4f;
							}
						else if (ct全体アニメ.n現在の値 <= 2000 + (ctゲージアニメーション.n終了値 * 66) + 5270)
							for (int i = 0; i < 3; i++)
							{
								TJAPlayer3.Tx.ScoreRank.vc拡大縮小倍率.X = 0.5f + (float)Math.Sin((float)(ct全体アニメ.n現在の値 - (2000 + (ctゲージアニメーション.n終了値 * 66) + 5180)) * (Math.PI / 180)) * 0.5f;
								TJAPlayer3.Tx.ScoreRank.vc拡大縮小倍率.Y = 0.5f + (float)Math.Sin((float)(ct全体アニメ.n現在の値 - (2000 + (ctゲージアニメーション.n終了値 * 66) + 5180)) * (Math.PI / 180)) * 0.5f;
							}

						TJAPlayer3.Tx.ScoreRank.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 135, 339, new Rectangle(0, (TJAPlayer3.stage結果.st演奏記録[0].nScoreRank[TJAPlayer3.stage選曲.n確定された曲の難易度]) * 120, 140, 120));
					}

					if (ct全体アニメ.n現在の値 >= 2000 + (ctゲージアニメーション.n終了値 * 66) + 6500)
					{
						if (ct全体アニメ.n現在の値 <= 2000 + (ctゲージアニメーション.n終了値 * 66) + 6680)
							for (int i = 0; i < 3; i++)
							{
								TJAPlayer3.Tx.Result_Crown[i].Opacity = (ct全体アニメ.n現在の値 - (2000 + (ctゲージアニメーション.n終了値 * 66) + 6500)) / 180.0f * 255.0f;
								TJAPlayer3.Tx.Result_Crown[i].vc拡大縮小倍率.X = 1.0f + (float)Math.Sin((float)(ct全体アニメ.n現在の値 - (2000 + (ctゲージアニメーション.n終了値 * 66) + 6410)) / 1.5f * (Math.PI / 180)) * 1.4f;
								TJAPlayer3.Tx.Result_Crown[i].vc拡大縮小倍率.Y = 1.0f + (float)Math.Sin((float)(ct全体アニメ.n現在の値 - (2000 + (ctゲージアニメーション.n終了値 * 66) + 6410)) / 1.5f * (Math.PI / 180)) * 1.4f;
							}
						else if (ct全体アニメ.n現在の値 <= 2000 + (ctゲージアニメーション.n終了値 * 66) + 6770)
							for (int i = 0; i < 3; i++)
							{
								TJAPlayer3.Tx.Result_Crown[i].vc拡大縮小倍率.X = 0.5f + (float)Math.Sin((float)(ct全体アニメ.n現在の値 - (2000 + (ctゲージアニメーション.n終了値 * 66) + 6680)) * (Math.PI / 180)) * 0.5f;
								TJAPlayer3.Tx.Result_Crown[i].vc拡大縮小倍率.Y = 0.5f + (float)Math.Sin((float)(ct全体アニメ.n現在の値 - (2000 + (ctゲージアニメーション.n終了値 * 66) + 6680)) * (Math.PI / 180)) * 0.5f;
							}

						if (TJAPlayer3.ConfigIni.b太鼓パートAutoPlay)
						{
							TJAPlayer3.Tx.Result_Crown[2].t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 262, 336);
						}
                        else
						{
							if (TJAPlayer3.stage結果.st演奏記録[0].bIsClear[TJAPlayer3.stage選曲.n確定された曲の難易度])
							{
								TJAPlayer3.Tx.Result_Crown[0].t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 262, 336);
							}
							else if (TJAPlayer3.stage結果.st演奏記録[0].bIsFullCombo[TJAPlayer3.stage選曲.n確定された曲の難易度])
							{
								TJAPlayer3.Tx.Result_Crown[1].t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 262, 336);
							}
							else if (TJAPlayer3.stage結果.st演奏記録[0].bIsDondaFullCombo[TJAPlayer3.stage選曲.n確定された曲の難易度])
							{
								TJAPlayer3.Tx.Result_Crown[2].t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 262, 336);
							}
						}
					}

					if (ct全体アニメ.n現在の値 >= 2000 + (this.ctゲージアニメーション.n終了値 * 66) + 8360 - 85)
					{
						if (!this.ctMountain_ClearIn.b進行中)
							this.ctMountain_ClearIn.t開始(0, 515, 3, TJAPlayer3.Timer);

						if (TJAPlayer3.stage結果.st演奏記録[0].fゲージ >= 80.0f)
							if (!this.ctDonchan_Clear.b進行中)
								this.ctDonchan_Clear.t開始(0, TJAPlayer3.Tx.Result_Donchan_Clear.Length - 1, 1000 / 60, TJAPlayer3.Timer);
					}

				}
			}

			if (!this.ctDonchan_Clear.b進行中)
			{
				TJAPlayer3.Tx.Result_Donchan_Normal[ctDonchan_Normal.n現在の値].t2D中心基準描画(TJAPlayer3.app.Device, 157, 527);
			}
			else
			{
				TJAPlayer3.Tx.Result_Donchan_Clear[ctDonchan_Clear.n現在の値].t2D中心基準描画(TJAPlayer3.app.Device, 157, 527);
			}

			if (TJAPlayer3.Tx.Result_Soul_Text != null)
			{
				if (ctゲージアニメーション.n現在の値 <= 30)
					TJAPlayer3.Tx.Result_Soul_Text.t2D中心基準描画(TJAPlayer3.app.Device, 575, 159, new Rectangle(37 * 0, 0, 37, 37));

				if (ctゲージアニメーション.n現在の値 > 30 && ctゲージアニメーション.n現在の値 <= 49)
					TJAPlayer3.Tx.Result_Soul_Text.t2D中心基準描画(TJAPlayer3.app.Device, 575, 159, new Rectangle(37 * 1, 0, 37, 37));

				if (ctゲージアニメーション.n現在の値 >= 50)
				{
					this.ctSoulFlash.t進行Loop();
					this.ctSoul.t進行Loop();

					TJAPlayer3.Tx.Result_Soul_Fire.t2D中心基準描画(TJAPlayer3.app.Device, 575, 159, new Rectangle(150 * ctSoul.n現在の値, 0, 150, 131));

					if (ctSoulFlash.n現在の値 >= 1)
						TJAPlayer3.Tx.Result_Soul_Text.t2D中心基準描画(TJAPlayer3.app.Device, 575, 159, new Rectangle(37 * 2, 0, 37, 37));
					else
						TJAPlayer3.Tx.Result_Soul_Text.t2D中心基準描画(TJAPlayer3.app.Device, 575, 159, new Rectangle(37 * 1, 0, 37, 37));

				}
			}

            #region 段位認定モード用
            if(TJAPlayer3.stage選曲.n確定された曲の難易度 == (int)Difficulty.Dan)
            {
                TJAPlayer3.stage演奏ドラム画面.actDan.DrawExam(TJAPlayer3.stage結果.st演奏記録.Drums.Dan_C);
                switch (TJAPlayer3.stage演奏ドラム画面.actDan.GetExamStatus(TJAPlayer3.stage結果.st演奏記録.Drums.Dan_C))
                {
                    case Exam.Status.Failure:
                        TJAPlayer3.Tx.Result_Dan?.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Result_Dan_XY[0], TJAPlayer3.Skin.Result_Dan_XY[1], new Rectangle(0, 0, TJAPlayer3.Skin.Result_Dan[0], TJAPlayer3.Skin.Result_Dan[1]));
                        break;
                    case Exam.Status.Success:
                        TJAPlayer3.Tx.Result_Dan?.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Result_Dan_XY[0], TJAPlayer3.Skin.Result_Dan_XY[1], new Rectangle(TJAPlayer3.Skin.Result_Dan[0], 0, TJAPlayer3.Skin.Result_Dan[0], TJAPlayer3.Skin.Result_Dan[1]));
                        break;
                    case Exam.Status.Better_Success:
                        TJAPlayer3.Tx.Result_Dan?.t2D描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Result_Dan_XY[0], TJAPlayer3.Skin.Result_Dan_XY[1], new Rectangle(TJAPlayer3.Skin.Result_Dan[0] * 2, 0, TJAPlayer3.Skin.Result_Dan[0], TJAPlayer3.Skin.Result_Dan[1]));
                        break;
                    default:
                        break;
                }
                // Dan_Plate
                Dan_Plate?.t2D中心基準描画(TJAPlayer3.app.Device, TJAPlayer3.Skin.Result_Dan_Plate_XY[0], TJAPlayer3.Skin.Result_Dan_Plate_XY[1]);
            }
            #endregion

            #if DEBUG
            TJAPlayer3.act文字コンソール.tPrint(0, 0, C文字コンソール.Eフォント種別.白, ct全体アニメ.n現在の値.ToString());
            #endif

            if ( !this.ct表示用.b終了値に達した )
			{
				return 0;
			}
			return 1;
		}
		


		// その他

		#region [ private ]
		//-----------------
		[StructLayout( LayoutKind.Sequential )]
		private struct ST文字位置
		{
			public char ch;
			public Point pt;
		}

		public bool[] b音声再生;
        public CCounter ct全体アニメ;
        public CCounter ctゲージアニメーション;
		private CCounter ctSoulFlash;
		private CCounter ctSoul;
		private CCounter ctRainbowGauge;
		public CCounter ctEndAnime;
		public CCounter ctMountain_ClearIn;
		public CCounter ctBackgroundAnime;
		public CCounter ctBackgroundAnime_Clear;
		private CCounter ctDonchan_Normal;
		private CCounter ctDonchan_Clear;

		private bool bフルコンボ音再生済み;
		private CCounter ct表示用;
		private readonly Point[] ptFullCombo位置;
		private CSound sdDTXで指定されたフルコンボ音;
		private readonly ST文字位置[] st小文字位置;
		private readonly ST文字位置[] st大文字位置;
        private ST文字位置[] stScoreFont;

        private CTexture Dan_Plate;

		private void t小文字表示( int x, int y, string str )
		{
			foreach( char ch in str )
			{
				for( int i = 0; i < this.st小文字位置.Length; i++ )
				{
                    if( ch == ' ' )
                    {
                        break;
                    }

					if( this.st小文字位置[ i ].ch == ch )
					{
						Rectangle rectangle = new Rectangle( this.st小文字位置[ i ].pt.X, this.st小文字位置[ i ].pt.Y, 32, 38 );
						if(TJAPlayer3.Tx.Result_Number != null )
						{
                            TJAPlayer3.Tx.Result_Number.t2D拡大率考慮中央基準描画( TJAPlayer3.app.Device, x + 16, y + 19, rectangle );
						}
						break;
					}
				}
				x += 22;
			}
		}
		private void t大文字表示( int x, int y, string str )
		{
			this.t大文字表示( x, y, str, false );
		}
		private void t大文字表示( int x, int y, string str, bool b強調 )
		{
			foreach( char ch in str )
			{
				for( int i = 0; i < this.st大文字位置.Length; i++ )
				{
					if( this.st大文字位置[ i ].ch == ch )
					{
						Rectangle rectangle = new Rectangle( this.st大文字位置[ i ].pt.X, this.st大文字位置[ i ].pt.Y, 11, 0x10 );
						if( ch == '.' )
						{
							rectangle.Width -= 2;
							rectangle.Height -= 2;
						}
						if(TJAPlayer3.Tx.Result_Number != null )
						{
                            TJAPlayer3.Tx.Result_Number.t2D描画( TJAPlayer3.app.Device, x, y, rectangle );
						}
						break;
					}
				}
				x += 8;
			}
		}

        protected void tスコア文字表示(int x, int y, string str)
        {
            foreach (char ch in str)
            {
                for (int i = 0; i < this.stScoreFont.Length; i++)
                {
                    if (this.stScoreFont[i].ch == ch)
                    {
                        Rectangle rectangle = new Rectangle(this.stScoreFont[ i ].pt.X, 0, 51, 60);
                        if (TJAPlayer3.Tx.Result_Score_Number != null)
                        {
                            TJAPlayer3.Tx.Result_Score_Number.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, x - (str.Length * 33) + 25, y + 30, rectangle);
                        }
                        break;
                    }
                }
                x += 33;
            }
        }
		//-----------------
		#endregion
	}
}
