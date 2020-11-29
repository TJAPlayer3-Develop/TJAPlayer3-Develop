using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing;
using SlimDX.DirectInput;
using FDK;
using System.Reflection;
using Un4seen.Bass.AddOn.Cd;

namespace TJAPlayer3
{
	internal class CStageタイトル : CStage
	{
		// コンストラクタ

		public CStageタイトル()
		{
			base.eステージID = CStage.Eステージ.タイトル;
			base.b活性化してない = true;
			base.list子Activities.Add( this.actFIfromSetup = new CActFIFOBlack() );
			base.list子Activities.Add( this.actFI = new CActFIFOBlack() );
			base.list子Activities.Add( this.actFO = new CActFIFOBlack() );
		}


		// CStage 実装

		public override void On活性化()
		{
			Trace.TraceInformation( "タイトルステージを活性化します。" );
			Trace.Indent();
			try
			{
				for ( int i = 0; i < 4; i++ )
				{
					this.ctキー反復用[ i ] = new CCounter( 0, 0, 0, TJAPlayer3.Timer );
				}
				this.ct上移動用 = new CCounter();
				this.ct下移動用 = new CCounter();
				this.ctカーソルフラッシュ用 = new CCounter();
				TJAPlayer3.NamePlate.tNamePlateInit();
				base.On活性化();
			}
			finally
			{
				Trace.TraceInformation( "タイトルステージの活性化を完了しました。" );
				Trace.Unindent();
			}
		}
		public override void On非活性化()
		{
			Trace.TraceInformation( "タイトルステージを非活性化します。" );
			Trace.Indent();
			try
			{
				for( int i = 0; i < 4; i++ )
				{
					this.ctキー反復用[ i ] = null;
				}
				this.ct上移動用 = null;
				this.ct下移動用 = null;
				this.ctカーソルフラッシュ用 = null;

				bTitleStartPlayed = false;
				bInit = false;
			}
			finally
			{
				Trace.TraceInformation( "タイトルステージの非活性化を完了しました。" );
				Trace.Unindent();
			}
			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			//if( !base.b活性化してない )
			//{
			//	this.tx背景 = CDTXMania.tテクスチャの生成( CSkin.Path(@"Graphics\2_background.png"));
			//	this.txメニュー = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\2_menu.png" ));
			//	base.OnManagedリソースの作成();
			//}
		}
		public override void OnManagedリソースの解放()
		{
			//if( !base.b活性化してない )
			//{
			//	CDTXMania.tテクスチャの解放( ref this.tx背景 );
			//	CDTXMania.tテクスチャの解放( ref this.txメニュー );
			//	base.OnManagedリソースの解放();
			//}
		}
		public override int On進行描画()
		{
			if( !base.b活性化してない )
			{
				#region [ 初めての進行描画 ]
				//---------------------
				if( base.b初めての進行描画 )
				{
					if( TJAPlayer3.r直前のステージ == TJAPlayer3.stage起動 )
					{
						this.actFIfromSetup.tフェードイン開始();
						base.eフェーズID = CStage.Eフェーズ.タイトル_起動画面からのフェードイン;
					}
					else
					{
						this.actFI.tフェードイン開始();
						base.eフェーズID = CStage.Eフェーズ.共通_フェードイン;
					}
					this.ctカーソルフラッシュ用.t開始( 0, 700, 5, TJAPlayer3.Timer );
					this.ctカーソルフラッシュ用.n現在の値 = 100;
					base.b初めての進行描画 = false;
                }
				//---------------------
				#endregion

				if(TJAPlayer3.ServiceCount >= TJAPlayer3.ConfigIni.nGameCost)
                {
					CreditStatus = 0;
                }
				else if (TJAPlayer3.CoinCount + TJAPlayer3.ServiceCount >= TJAPlayer3.ConfigIni.nGameCost)
                {
					CreditStatus = 1;
                }
				else if(TJAPlayer3.CoinCount >= TJAPlayer3.ConfigIni.nGameCost)
                {
					CreditStatus = 2;
                }
                else
                {
					CreditStatus = 3;
                }

				//--Homeキーでタイトル画面の切り替え : Press Home Key to toggle title screen--
				if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)Key.Home))
				{
					bEnableNewTitle = !bEnableNewTitle;
					bInit = false;

					TJAPlayer3.Skin.soundTitle_R1_BGM.t停止する();
				}
				//----------------

				if (bEnableNewTitle)
                {
					//--NEW TITLE SCREEN ROUTINE GOES HERE--

					//Stop old title screen sounds if playing
                    if (this.b曲再生)
                    {
						TJAPlayer3.Skin.soundタイトル音.t停止する();
						this.b曲再生 = false;
					}
					if (TJAPlayer3.Skin.soundタイトルスタート音.b再生中) TJAPlayer3.Skin.soundタイトルスタート音.t停止する();
					if (TJAPlayer3.Skin.soundEntry.b再生中) TJAPlayer3.Skin.soundEntry.t停止する();
					if (bTitleStartPlayed) bTitleStartPlayed = false;

					titleScreenRoutine = 1;

                    if (titleScreenRoutine == 0)
                    {
						if (!bInit)
						{
							bInit = true;
						}
					}
					else if (titleScreenRoutine == 1)
                    {
						if(!bInit)
                        {
							TJAPlayer3.Skin.soundTitle_R1_BGM.t再生する();

							bInit = true;
                        }

						TJAPlayer3.Tx.Title_R1_Background.t2D描画(TJAPlayer3.app.Device, 0, 0);
						TJAPlayer3.Tx.Title_R1_Logo.t2D描画(TJAPlayer3.app.Device, 308 + rnd.Next(1, 25), 58 + rnd.Next(1, 25));

						if (TJAPlayer3.Input管理.Keyboard.list入力イベント.Count >= 1)
                        {
							if (!TJAPlayer3.Input管理.Keyboard.bキーが押されている((int)Key.Home))
                            {
								TJAPlayer3.Skin.soundTitle_R1_BGM.t停止する();
								return (int)E戻り値.GAMESTART;
							}
                        }
					}
				}
				else
                {
					// 進行

					#region [ カーソル上移動 ]
					//---------------------
					if (this.ct上移動用.b進行中)
					{
						this.ct上移動用.t進行();
						if (this.ct上移動用.b終了値に達した)
						{
							this.ct上移動用.t停止();
						}
					}
					//---------------------
					#endregion
					#region [ カーソル下移動 ]
					//---------------------
					if (this.ct下移動用.b進行中)
					{
						this.ct下移動用.t進行();
						if (this.ct下移動用.b終了値に達した)
						{
							this.ct下移動用.t停止();
						}
					}
					//---------------------
					#endregion
					#region [ カーソルフラッシュ ]
					//---------------------
					this.ctカーソルフラッシュ用.t進行Loop();
					//---------------------
					#endregion

					// キー入力

					if (base.eフェーズID == CStage.Eフェーズ.共通_通常状態        // 通常状態、かつ
						&& TJAPlayer3.act現在入力を占有中のプラグイン == null)    // プラグインの入力占有がない
					{
						if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)Key.Escape))
							return (int)E戻り値.EXIT;

						this.ctキー反復用.Up.tキー反復(TJAPlayer3.Input管理.Keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.UpArrow), new CCounter.DGキー処理(this.tカーソルを上へ移動する));
						this.ctキー反復用.R.tキー反復(TJAPlayer3.Pad.b押されているGB(Eパッド.HH), new CCounter.DGキー処理(this.tカーソルを上へ移動する));
						if (TJAPlayer3.Pad.b押された(E楽器パート.DRUMS, Eパッド.SD))
							this.tカーソルを上へ移動する();

						this.ctキー反復用.Down.tキー反復(TJAPlayer3.Input管理.Keyboard.bキーが押されている((int)SlimDX.DirectInput.Key.DownArrow), new CCounter.DGキー処理(this.tカーソルを下へ移動する));
						this.ctキー反復用.B.tキー反復(TJAPlayer3.Pad.b押されているGB(Eパッド.BD), new CCounter.DGキー処理(this.tカーソルを下へ移動する));
						if (TJAPlayer3.Pad.b押された(E楽器パート.DRUMS, Eパッド.LT))
							this.tカーソルを下へ移動する();

						if (TJAPlayer3.Pad.b押されたDGB(Eパッド.CY) || TJAPlayer3.Pad.b押された(E楽器パート.DRUMS, Eパッド.RD) || TJAPlayer3.Pad.b押された(E楽器パート.DRUMS, Eパッド.LC) || TJAPlayer3.Pad.b押されたDGB(Eパッド.Decide) || TJAPlayer3.Pad.b押されたDGB(Eパッド.LRed) || TJAPlayer3.Pad.b押されたDGB(Eパッド.RRed) ||
								(TJAPlayer3.ConfigIni.bEnterがキー割り当てのどこにも使用されていない && TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.Return)))
						{
							if ((this.n現在のカーソル行 == (int)E戻り値.GAMESTART - 1))
							{
								if (CreditStatus != 3)
								{
                                    switch (CreditStatus)
                                    {
										case 0:
											TJAPlayer3.ServiceCount -= TJAPlayer3.ConfigIni.nGameCost;
											break;
										case 1:
											int sub_num = TJAPlayer3.ConfigIni.nGameCost - TJAPlayer3.ServiceCount;
											TJAPlayer3.CoinCount -= sub_num;
											TJAPlayer3.ServiceCount = 0;
											break;
										case 2:
											TJAPlayer3.CoinCount -= TJAPlayer3.ConfigIni.nGameCost;
											break;
									}
									TJAPlayer3.Skin.soundタイトル音.t停止する();
									TJAPlayer3.Skin.soundタイトルスタート音.t停止する();
									TJAPlayer3.Skin.soundEntry.t停止する();
									TJAPlayer3.Skin.sound決定音.t再生する();
									TJAPlayer3.Skin.soundゲーム開始音.t再生する();

									this.actFO.tフェードアウト開始();
									base.eフェーズID = CStage.Eフェーズ.共通_フェードアウト;
								}
                                else
                                {
									if (TJAPlayer3.Skin.CoinPrompt.b再生中)
										TJAPlayer3.Skin.CoinPrompt.t停止する();
									TJAPlayer3.Skin.CoinPrompt.t再生する();
								}
							}
							else
							{
								TJAPlayer3.Skin.sound決定音.t再生する();

								this.actFO.tフェードアウト開始();
								base.eフェーズID = CStage.Eフェーズ.共通_フェードアウト;
							}
							if (this.n現在のカーソル行 == (int)E戻り値.EXIT - 1)
							{
								return (int)E戻り値.EXIT;
							}
						}
						//					if ( CDTXMania.Input管理.Keyboard.bキーが押された( (int) Key.Space ) )
						//						Trace.TraceInformation( "DTXMania Title: SPACE key registered. " + CDTXMania.ct.nシステム時刻 );
					}

					// 描画

					if (!TJAPlayer3.Skin.soundタイトルスタート音.b再生中 && bTitleStartPlayed)
					{
						if (!b曲再生)
						{
							TJAPlayer3.Skin.soundタイトル音.t再生する();
							b曲再生 = true;
						}
					}
					else
					{
						TJAPlayer3.Skin.soundタイトル音.n位置_現在のサウンド = 0;
					}

					if (!TJAPlayer3.Skin.soundタイトルスタート音.b再生中 && !b曲再生 && !bTitleStartPlayed)
					{
						TJAPlayer3.Skin.soundタイトルスタート音.t再生する();
						bTitleStartPlayed = true;
					}
					//if (!TJAPlayer3.Skin.soundEntry.b再生中) TJAPlayer3.Skin.soundEntry.t再生する();

					if (TJAPlayer3.Tx.Title_Background != null)
						TJAPlayer3.Tx.Title_Background.t2D描画(TJAPlayer3.app.Device, 0, 0);

					#region[ バージョン表示 ]
					//string strVersion = "KTT:J:A:I:2017072200";
					string strCreator = "https://github.com/TJAPlayer3-Develop/TJAPlayer3-Develop";
					AssemblyName asmApp = Assembly.GetExecutingAssembly().GetName();
#if DEBUG
					TJAPlayer3.act文字コンソール.tPrint(4, 44, C文字コンソール.Eフォント種別.白, "DEBUG BUILD");
#endif
					#endregion

					TJAPlayer3.NamePlate.tNamePlateDraw(0, 0);

					if (TJAPlayer3.Tx.Title_Menu != null)
					{
						int x = MENU_X;
						int y = MENU_Y + (this.n現在のカーソル行 * MENU_H);
						if (this.ct上移動用.b進行中)
						{
							y += (int)((double)MENU_H / 2 * (Math.Cos(Math.PI * (((double)this.ct上移動用.n現在の値) / 100.0)) + 1.0));
						}
						else if (this.ct下移動用.b進行中)
						{
							y -= (int)((double)MENU_H / 2 * (Math.Cos(Math.PI * (((double)this.ct下移動用.n現在の値) / 100.0)) + 1.0));
						}
						if (this.ctカーソルフラッシュ用.n現在の値 <= 100)
						{
							float nMag = (float)(1.0 + ((((double)this.ctカーソルフラッシュ用.n現在の値) / 100.0) * 0.5));
							TJAPlayer3.Tx.Title_Menu.vc拡大縮小倍率.X = nMag;
							TJAPlayer3.Tx.Title_Menu.vc拡大縮小倍率.Y = nMag;
							TJAPlayer3.Tx.Title_Menu.Opacity = (int)(255.0 * (1.0 - (((double)this.ctカーソルフラッシュ用.n現在の値) / 100.0)));
							int x_magnified = x + ((int)((MENU_W * (1.0 - nMag)) / 2.0));
							int y_magnified = y + ((int)((MENU_H * (1.0 - nMag)) / 2.0));
							TJAPlayer3.Tx.Title_Menu.t2D描画(TJAPlayer3.app.Device, x_magnified, y_magnified, new Rectangle(0, MENU_H * 5, MENU_W, MENU_H));
						}
						TJAPlayer3.Tx.Title_Menu.vc拡大縮小倍率.X = 1f;
						TJAPlayer3.Tx.Title_Menu.vc拡大縮小倍率.Y = 1f;
						TJAPlayer3.Tx.Title_Menu.Opacity = 0xff;
						TJAPlayer3.Tx.Title_Menu.t2D描画(TJAPlayer3.app.Device, x, y, new Rectangle(0, MENU_H * 4, MENU_W, MENU_H));
					}
					if (TJAPlayer3.Tx.Title_Menu != null)
					{
						//this.txメニュー.t2D描画( CDTXMania.app.Device, 0xce, 0xcb, new Rectangle( 0, 0, MENU_W, MWNU_H ) );
						// #24525 2011.3.16 yyagi: "OPTION"を省いて描画。従来スキンとの互換性確保のため。
						TJAPlayer3.Tx.Title_Menu.t2D描画(TJAPlayer3.app.Device, MENU_X, MENU_Y, new Rectangle(0, 0, MENU_W, MENU_H));
						TJAPlayer3.Tx.Title_Menu.t2D描画(TJAPlayer3.app.Device, MENU_X, MENU_Y + MENU_H, new Rectangle(0, MENU_H * 2, MENU_W, MENU_H * 2));
					}

					// URLの座標が押されたらブラウザで開いてやる 兼 マウスクリックのテスト
					// クライアント領域内のカーソル座標を取得する。
					// point.X、point.Yは負の値になることもある。
					var point = TJAPlayer3.app.Window.PointToClient(System.Windows.Forms.Cursor.Position);
					// クライアント領域の横幅を取得して、1280で割る。もちろんdouble型。
					var scaling = 1.000 * TJAPlayer3.app.Window.ClientSize.Width / 1280;
					if (TJAPlayer3.Input管理.Mouse.bキーが押された((int)MouseObject.Button1))
					{
						if (point.X >= 0 * scaling && point.X <= 190 * scaling && point.Y >= 700 && point.Y <= 720 * scaling)
							System.Diagnostics.Process.Start(strCreator);
					}

					//CDTXMania.act文字コンソール.tPrint(0, 80, C文字コンソール.Eフォント種別.白, point.X.ToString());
					//CDTXMania.act文字コンソール.tPrint(0, 100, C文字コンソール.Eフォント種別.白, point.Y.ToString());
					//CDTXMania.act文字コンソール.tPrint(0, 120, C文字コンソール.Eフォント種別.白, scaling.ToString());


					CStage.Eフェーズ eフェーズid = base.eフェーズID;
					switch (eフェーズid)
					{
						case CStage.Eフェーズ.共通_フェードイン:
							if (this.actFI.On進行描画() != 0)
							{
								b曲再生 = false;
								base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
							}
							break;

						case CStage.Eフェーズ.共通_フェードアウト:
							if (this.actFO.On進行描画() == 0)
							{
								break;
							}
							base.eフェーズID = CStage.Eフェーズ.共通_終了状態;
							switch (this.n現在のカーソル行)
							{
								case (int)E戻り値.GAMESTART - 1:
									return (int)E戻り値.GAMESTART;

								case (int)E戻り値.CONFIG - 1:
									return (int)E戻り値.CONFIG;

								case (int)E戻り値.EXIT - 1:
									return (int)E戻り値.EXIT;
									//return ( this.n現在のカーソル行 + 1 );
							}
							break;

						case CStage.Eフェーズ.タイトル_起動画面からのフェードイン:
							if (this.actFIfromSetup.On進行描画() != 0)
							{
								base.eフェーズID = CStage.Eフェーズ.共通_通常状態;
							}
							break;
					}
				}
			}
			return 0;
		}
		public enum E戻り値
		{
			継続 = 0,
			GAMESTART,
//			OPTION,
			CONFIG,
			EXIT
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
					}
					throw new IndexOutOfRangeException();
				}
			}
		}

		private bool b曲再生;

		private CActFIFOBlack actFI;
		private CActFIFOBlack actFIfromSetup;
		private CActFIFOBlack actFO;
		private CCounter ctカーソルフラッシュ用;
		private STキー反復用カウンタ ctキー反復用;
		private CCounter ct下移動用;
		private CCounter ct上移動用;
		private const int MENU_H = 39;
		private const int MENU_W = 227;
		private const int MENU_X = 506;
		private const int MENU_Y = 513;
		private int n現在のカーソル行;
		private bool bTitleStartPlayed;
		private int CreditStatus;

		private void tカーソルを下へ移動する()
		{
			if ( this.n現在のカーソル行 != (int) E戻り値.EXIT - 1 )
			{
				TJAPlayer3.Skin.soundカーソル移動音.t再生する();
				this.n現在のカーソル行++;
				this.ct下移動用.t開始( 0, 100, 1, TJAPlayer3.Timer );
				if( this.ct上移動用.b進行中 )
				{
					this.ct下移動用.n現在の値 = 100 - this.ct上移動用.n現在の値;
					this.ct上移動用.t停止();
				}
			}
		}
		private void tカーソルを上へ移動する()
		{
			if ( this.n現在のカーソル行 != (int) E戻り値.GAMESTART - 1 )
			{
				TJAPlayer3.Skin.soundカーソル移動音.t再生する();
				this.n現在のカーソル行--;
				this.ct上移動用.t開始( 0, 100, 1, TJAPlayer3.Timer );
				if( this.ct下移動用.b進行中 )
				{
					this.ct上移動用.n現在の値 = 100 - this.ct下移動用.n現在の値;
					this.ct下移動用.t停止();
				}
			}
		}

		//新タイトル画面用の変数 - Variables for new title screen
		private bool bEnableNewTitle;
		private bool bInit;

		private int titleScreenRoutine;

		private Random rnd = new Random();

		//-----------------
		#endregion
	}
}
