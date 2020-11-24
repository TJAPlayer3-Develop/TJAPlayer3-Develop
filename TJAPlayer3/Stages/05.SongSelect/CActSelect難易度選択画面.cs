using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Drawing.Text;

using SlimDX;
using FDK;

namespace TJAPlayer3
{
    /// <summary>
    /// 難易度選択画面。
    /// この難易度選択画面はAC7～AC14のような方式であり、WiiまたはAC15移行の方式とは異なる。
    /// </summary>
	internal class CActSelect難易度選択画面 : CActivity
	{
		// プロパティ

		public bool bスクロール中
		{
			get
			{
				if( this.n目標のスクロールカウンタ == 0 )
				{
					return ( this.n現在のスクロールカウンタ != 0 );
				}
				return true;
			}
		}

        public bool bIsDifficltSelect;

		// コンストラクタ

        public CActSelect難易度選択画面()
        {
			base.b活性化してない = true;
		}


		// メソッド
        public int t指定した方向に近い難易度番号を返す( int nDIRECTION, int pos )
        {
            if( nDIRECTION == 0)
            {
                for( int i = pos; i < 5; i++ )
                {
                    if( i == pos ) continue;
                    if( TJAPlayer3.stage選曲.r現在選択中の曲.arスコア[ i ] != null ) return i;
                    if( i == 4 ) return this.t指定した方向に近い難易度番号を返す( 0, 0 );
                }
            }
            else
            {
                for( int i = pos; i > -1; i-- )
                {
                    if( pos == i ) continue;
                    if( TJAPlayer3.stage選曲.r現在選択中の曲.arスコア[ i ] != null ) return i;
                    if( i == 0 ) return this.t指定した方向に近い難易度番号を返す( 1, 4 );
                }
            }
            return pos;
        }

        public void t次に移動()
        {
            TJAPlayer3.Skin.sound変更音.t再生する();
            if (this.n現在の選択行 + 1 < 4)
            {
                if (this.n現在の選択行 == 2 && b裏譜面)
                {
                    this.n現在の選択行 += 2;
                }
                else
                {
                    this.n現在の選択行 += 1;
                }
                ctBarAnime.t開始(0, 180, 1, TJAPlayer3.Timer);
            }
            else
            {
                縁カウント++;
                if(縁カウント >= 10)
                {
                    if (this.n現在の選択行 == 3 && TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.nレベル[4] >= 0)
                    {
                        TJAPlayer3.Skin.sound裏切り替え音.t再生する();
                        b裏譜面 = true;
                        this.n現在の選択行 = 4;
                        縁カウント = 0;
                    }
                    else if (this.n現在の選択行 == 4)
                    {
                        TJAPlayer3.Skin.sound裏切り替え音.t再生する();
                        b裏譜面 = false;
                        this.n現在の選択行 = 3;
                        縁カウント = 0;
                    }
                }
            }
        }

        public void t前に移動()
        {
            TJAPlayer3.Skin.sound変更音.t再生する();
            if (this.n現在の選択行 - 1 >= -2)
            {
                ctBarAnime.t開始(0, 180, 1, TJAPlayer3.Timer);
                if (this.n現在の選択行 == 4)
                {
                    n現在の選択行 -= 2;
                }
                else
                {
                    this.n現在の選択行 -= 1;
                }
            }
            縁カウント = 0;
        }

        public void t難易度選択画面を閉じる()
        {
            this.b曲選択 = false;
            this.bIsDifficltSelect = false;
            this.b初めての進行描画 = true;
            縁カウント = 0;
            TJAPlayer3.Skin.sound取消音.t再生する();
            if (TJAPlayer3.Skin.soundSelectAnnounce.b再生中) TJAPlayer3.Skin.soundSelectAnnounce.t停止する();
            TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 = 0;
            TJAPlayer3.stage選曲.ctDiffSelect移動待ち.t停止();
            TJAPlayer3.stage選曲.act曲リスト.ctBarOpen.t開始(0, 161, 2, TJAPlayer3.Timer);
        }

		// CActivity 実装

		public override void On活性化()
		{
            if( this.b活性化してる )
                return;

            this.n目標のスクロールカウンタ = 0;
            this.n現在のスクロールカウンタ = 0;
            ct決定待機 = new CCounter();

            // フォント作成。
            // 曲リスト文字は２倍（面積４倍）でテクスチャに描画してから縮小表示するので、フォントサイズは２倍とする。
            this.ctBarAnime = new CCounter();
            this.ct移動 = new CCounter();

            this.n現在の選択行 = 3;
            base.On活性化();
		}
		public override void On非活性化()
		{
			if( this.b活性化してない )
				return;

            this.ct移動 = null;
            this.ctBarAnime = null;

            TJAPlayer3.t安全にDisposeする(ref SongTitle);
            TJAPlayer3.t安全にDisposeする(ref SongSubTitle);

            base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( this.b活性化してない )
				return;

			base.OnManagedリソースの作成();
		}
		public override void OnManagedリソースの解放()
		{
			if( this.b活性化してない )
				return;

			base.OnManagedリソースの解放();
		}
		public override int On進行描画()
		{
			if( this.b活性化してない )
				return 0;

			#region [ 初めての進行描画 ]
			//-----------------
			if( this.b初めての進行描画 )
            {
                TJAPlayer3.Skin.soundSelectAnnounce.t再生する();
                this.b曲選択 = false;
                this.b裏譜面 = false;
                縁カウント = 0;

                if (TJAPlayer3.stage選曲.ct制限時間.n現在の値 > 70)
                {
                    TJAPlayer3.stage選曲.制限時間音声のリセット(true);
                    TJAPlayer3.stage選曲.ct制限時間.n現在の値 = 70;
                }

                base.b初めての進行描画 = false;
			}
            //-----------------
            #endregion

            ctBarAnime.t進行();
            ct決定待機.t進行();

            {
                #region [ (2) 通常フェーズの進行。]
                //-----------------

                //キー操作
                if (!TJAPlayer3.stage選曲.actSortSongs.bIsActivePopupMenu && !TJAPlayer3.stage選曲.actQuickConfig.bIsActivePopupMenu && !b曲選択)
                {
                    if (TJAPlayer3.Pad.b押されたDGB(Eパッド.RBlue) ||
                            TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.RightArrow))
                    {
                        this.t次に移動();
                    }
                    else if (TJAPlayer3.Pad.b押されたDGB(Eパッド.LBlue) ||
                            TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.LeftArrow))
                    {
                        this.t前に移動();
                    }
                    else if (TJAPlayer3.stage選曲.ct制限時間.b終了値に達した || (TJAPlayer3.Pad.b押されたDGB(Eパッド.Decide) ||
                            ((TJAPlayer3.ConfigIni.bEnterがキー割り当てのどこにも使用されていない && TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.Return)))) ||
                            TJAPlayer3.Pad.b押されたDGB(Eパッド.LRed) || TJAPlayer3.Pad.b押されたDGB(Eパッド.RRed))
                    {
                        if (n現在の選択行 >= 0 && TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.nレベル[n現在の選択行] >= 0)
                        {
                            TJAPlayer3.stage選曲.ctDonchanStart.t開始(0, TJAPlayer3.Tx.SongSelect_Donchan_Start.Length - 1, 1000 / 45, TJAPlayer3.Timer);
                            this.b曲選択 = true;
                            if (TJAPlayer3.Skin.soundSelectAnnounce.b再生中) TJAPlayer3.Skin.soundSelectAnnounce.t停止する();
                            TJAPlayer3.Skin.sound曲決定音.t再生する();
                            if (TJAPlayer3.stage選曲.ct制限時間.b進行中) TJAPlayer3.stage選曲.ct制限時間.t停止();
                            ct決定待機.t開始(0, 2000, 1, TJAPlayer3.Timer);
                        }
                        else if (n現在の選択行 == -1 && !TJAPlayer3.stage選曲.ct制限時間.b終了値に達した)
                        {
                            TJAPlayer3.Skin.soundSongOption.t再生する();
                            TJAPlayer3.stage選曲.showQuickConfig(); // Use QCM for the time being, since we haven't implemented this feature.
                            //TODO: Implement this feature.
                        }
                        else if (n現在の選択行 == -2 && !TJAPlayer3.stage選曲.ct制限時間.b終了値に達した)
                        {
                            this.t難易度選択画面を閉じる();
                        }
                        else this.t次に移動();
                    }
                    else if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.Tab) && TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.nレベル[4] >= 0)
                    {
                        TJAPlayer3.Skin.sound裏切り替え音.t再生する();
                        b裏譜面 = !b裏譜面;
                        縁カウント = 0;
                        if (this.n現在の選択行 == 4) this.n現在の選択行 = 3;
                        else if (this.n現在の選択行 == 3) this.n現在の選択行 = 4;
                    }
                    else if (TJAPlayer3.Input管理.Keyboard.bキーが押された((int)SlimDX.DirectInput.Key.Escape))
                    {
                        this.t難易度選択画面を閉じる();
                    }
                }

                TJAPlayer3.Tx.Diffculty_Back[TJAPlayer3.stage選曲.act曲リスト.nStrジャンルtoNum(TJAPlayer3.stage選曲.r現在選択中の曲.strジャンル)].Opacity = TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 - 935;
                TJAPlayer3.Tx.Diffculty_Back[TJAPlayer3.stage選曲.act曲リスト.nStrジャンルtoNum(TJAPlayer3.stage選曲.r現在選択中の曲.strジャンル)].t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 640, 280);

                TJAPlayer3.Tx.Difficulty_Bar.color4 = new Color4(1f, 1f, 1f);
                TJAPlayer3.Tx.Difficulty_Bar.Opacity = TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 - 935;
                TJAPlayer3.Tx.Difficulty_Bar.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 515 - 150, 380, new Rectangle(0, 0, 163, 228));


                if (b裏譜面)
                {
                    if (TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.nレベル[4] >= 0)
                    {
                        TJAPlayer3.Tx.Difficulty_Bar.color4 = new Color4(1f, 1f, 1f);
                        TJAPlayer3.Tx.Difficulty_Bar.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 515 + 3 * 136, 380, new Rectangle(165 + (4 * 136), 0, 136, 228));
                    }
                    else
                    {
                        TJAPlayer3.Tx.Difficulty_Bar.color4 = new Color4(0.5f, 0.5f, 0.5f);
                        TJAPlayer3.Tx.Difficulty_Bar.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 515 + 3 * 136, 380, new Rectangle(165 + (4 * 136), 0, 136, 228));
                    }
                }
                else
                {
                    if (TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.nレベル[3] >= 0)
                    {
                        TJAPlayer3.Tx.Difficulty_Bar.color4 = new Color4(1f, 1f, 1f);
                        TJAPlayer3.Tx.Difficulty_Bar.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 515 + 3 * 136, 380, new Rectangle(163 + (3 * 136), 0, 136, 228));
                    }
                    else
                    {
                        TJAPlayer3.Tx.Difficulty_Bar.color4 = new Color4(0.5f, 0.5f, 0.5f);
                        TJAPlayer3.Tx.Difficulty_Bar.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 515 + 3 * 136, 380, new Rectangle(163 + (3 * 136), 0, 136, 228));
                    }
                }

                for (int i = 0; i < 5; i++)
                {
                    if (i < 3)
                    {
                        if (TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.nレベル[i] >= 0)
                        {
                            TJAPlayer3.Tx.Difficulty_Bar.color4 = new Color4(1f, 1f, 1f);
                            TJAPlayer3.Tx.Difficulty_Bar.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 515 + i * 136, 380, new Rectangle(163 + (i * 136), 0, 136, 228));
                        }
                        else
                        {
                            TJAPlayer3.Tx.Difficulty_Bar.color4 = new Color4(0.5f, 0.5f, 0.5f);
                            TJAPlayer3.Tx.Difficulty_Bar.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 515 + i * 136, 380, new Rectangle(163 + (i * 136), 0, 136, 228));
                        }
                    }
                }

                TJAPlayer3.Tx.Difficulty_SelectBar.Opacity = TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 - 935;
                TJAPlayer3.Tx.Difficulty_SelectBar.t2D描画(TJAPlayer3.app.Device, ptバー座標[n現在の選択行 + 2].X, (float)(ptバー座標[n現在の選択行 + 2].Y) + (float)Math.Sin(this.ctBarAnime.n現在の値 * (Math.PI / 180)) * 11.0f, new Rectangle(0, 0, 260, 122));

                TJAPlayer3.stage選曲.act曲リスト.ResolveTitleTexture(SongTitle).vc拡大縮小倍率.X = 1.0f;
                TJAPlayer3.stage選曲.act曲リスト.ResolveTitleTexture(SongTitle).vc拡大縮小倍率.Y = 1.0f;
                TJAPlayer3.stage選曲.act曲リスト.ResolveTitleTexture(SongTitle).Opacity = TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 - 935;
                TJAPlayer3.stage選曲.act曲リスト.ResolveTitleTexture(SongTitle).t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 640 + TJAPlayer3.Skin.Difficulty_Title_X, 130 + TJAPlayer3.Skin.Difficulty_Title_Y);

                TJAPlayer3.stage選曲.act曲リスト.ResolveTitleTexture(SongSubTitle).vc拡大縮小倍率.X = 1.0f;
                TJAPlayer3.stage選曲.act曲リスト.ResolveTitleTexture(SongSubTitle).vc拡大縮小倍率.Y = 1.0f;
                TJAPlayer3.stage選曲.act曲リスト.ResolveTitleTexture(SongSubTitle).Opacity = TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 - 935;
                TJAPlayer3.stage選曲.act曲リスト.ResolveTitleTexture(SongSubTitle).t2D中心基準描画(TJAPlayer3.app.Device, 640 + TJAPlayer3.Skin.Difficulty_Title_X, 160 + TJAPlayer3.Skin.Difficulty_Title_Y);
                TJAPlayer3.Tx.Difficulty_Crown.Opacity = TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 - 935;

                for (int i = 0; i < 5; i++)
                {
                    if(TJAPlayer3.stage選曲.r現在選択中の曲.arスコア[i] != null)
                    {
                        if (i < 3)
                        {
                            if (TJAPlayer3.stage選曲.r現在選択中の曲.arスコア[i].譜面情報.クリア[i])
                                TJAPlayer3.Tx.Difficulty_Crown.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 480 + i * 138, 292, new Rectangle(0 * 24, 0, 24, 26));
                            if (TJAPlayer3.stage選曲.r現在選択中の曲.arスコア[i].譜面情報.フルコンボ[i])
                                TJAPlayer3.Tx.Difficulty_Crown.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 480 + i * 138, 292, new Rectangle(1 * 24, 0, 24, 26));
                            if (TJAPlayer3.stage選曲.r現在選択中の曲.arスコア[i].譜面情報.ドンダフルコンボ[i])
                                TJAPlayer3.Tx.Difficulty_Crown.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 480 + i * 138, 292, new Rectangle(2 * 24, 0, 24, 26));

                            if (TJAPlayer3.stage選曲.r現在選択中の曲.arスコア[i].譜面情報.nスコアランク[i] >= 1)
                            {
                                TJAPlayer3.Tx.SongSelect_ScoreRank.Opacity = 255;
                                TJAPlayer3.Tx.SongSelect_ScoreRank.vc拡大縮小倍率.X = 0.5f;
                                TJAPlayer3.Tx.SongSelect_ScoreRank.vc拡大縮小倍率.Y = 0.5f;
                                TJAPlayer3.Tx.SongSelect_ScoreRank.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 480 + i * 138 + 25, 292, new RectangleF(0, 41f * (TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.nスコアランク[i] - 1), 48, 41f));
                                TJAPlayer3.Tx.SongSelect_ScoreRank.vc拡大縮小倍率.X = 1.0f;
                                TJAPlayer3.Tx.SongSelect_ScoreRank.vc拡大縮小倍率.Y = 1.0f;
                                TJAPlayer3.Tx.SongSelect_ScoreRank.Opacity = 0;
                            }
                        }
                        if (i >= 3)
                        {
                            if (b裏譜面)
                            {
                                if (TJAPlayer3.stage選曲.r現在選択中の曲.arスコア[4].譜面情報.クリア[4])
                                    TJAPlayer3.Tx.Difficulty_Crown.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 480 + 3 * 138, 292, new Rectangle(0 * 24, 0, 24, 26));
                                if (TJAPlayer3.stage選曲.r現在選択中の曲.arスコア[4].譜面情報.フルコンボ[4])
                                    TJAPlayer3.Tx.Difficulty_Crown.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 480 + 3 * 138, 292, new Rectangle(1 * 24, 0, 24, 26));
                                if (TJAPlayer3.stage選曲.r現在選択中の曲.arスコア[4].譜面情報.ドンダフルコンボ[4])
                                    TJAPlayer3.Tx.Difficulty_Crown.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 480 + 3 * 138, 292, new Rectangle(2 * 24, 0, 24, 26));

                                if (TJAPlayer3.stage選曲.r現在選択中の曲.arスコア[4].譜面情報.nスコアランク[i] >= 1)
                                {
                                    TJAPlayer3.Tx.SongSelect_ScoreRank.Opacity = 255;
                                    TJAPlayer3.Tx.SongSelect_ScoreRank.vc拡大縮小倍率.X = 0.5f;
                                    TJAPlayer3.Tx.SongSelect_ScoreRank.vc拡大縮小倍率.Y = 0.5f;
                                    TJAPlayer3.Tx.SongSelect_ScoreRank.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 480 + 3 * 138 + 25, 292, new RectangleF(0, 41f * (TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.nスコアランク[i] - 1), 48, 41f));
                                    TJAPlayer3.Tx.SongSelect_ScoreRank.vc拡大縮小倍率.X = 1.0f;
                                    TJAPlayer3.Tx.SongSelect_ScoreRank.vc拡大縮小倍率.Y = 1.0f;
                                    TJAPlayer3.Tx.SongSelect_ScoreRank.Opacity = 0;
                                }
                            }
                            else
                            {
                                if (TJAPlayer3.stage選曲.r現在選択中の曲.arスコア[3].譜面情報.クリア[3])
                                    TJAPlayer3.Tx.Difficulty_Crown.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 480 + 3 * 138, 292, new Rectangle(0 * 24, 0, 24, 26));
                                if (TJAPlayer3.stage選曲.r現在選択中の曲.arスコア[3].譜面情報.フルコンボ[3])
                                    TJAPlayer3.Tx.Difficulty_Crown.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 480 + 3 * 138, 292, new Rectangle(1 * 24, 0, 24, 26));
                                if (TJAPlayer3.stage選曲.r現在選択中の曲.arスコア[3].譜面情報.ドンダフルコンボ[3])
                                    TJAPlayer3.Tx.Difficulty_Crown.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 480 + 3 * 138, 292, new Rectangle(2 * 24, 0, 24, 26));

                                if (TJAPlayer3.stage選曲.r現在選択中の曲.arスコア[3].譜面情報.nスコアランク[i] >= 1)
                                {
                                    TJAPlayer3.Tx.SongSelect_ScoreRank.Opacity = 255;
                                    TJAPlayer3.Tx.SongSelect_ScoreRank.vc拡大縮小倍率.X = 0.5f;
                                    TJAPlayer3.Tx.SongSelect_ScoreRank.vc拡大縮小倍率.Y = 0.5f;
                                    TJAPlayer3.Tx.SongSelect_ScoreRank.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 480 + 3 * 138 + 25, 292, new RectangleF(0, 41f * (TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.nスコアランク[i] - 1), 48, 41f));
                                    TJAPlayer3.Tx.SongSelect_ScoreRank.vc拡大縮小倍率.X = 1.0f;
                                    TJAPlayer3.Tx.SongSelect_ScoreRank.vc拡大縮小倍率.Y = 1.0f;
                                    TJAPlayer3.Tx.SongSelect_ScoreRank.Opacity = 0;
                                }
                            }
                        }
                    }
                }

                if (ct決定待機.n現在の値 == 2000)
                {
                    TJAPlayer3.stage選曲.actPresound.tサウンドの停止MT();
                    TJAPlayer3.stage選曲.t曲を選択する(this.n現在の選択行);
                    ct決定待機.t停止();
                    ct決定待機.n現在の値 = 0;
                }

				//-----------------
				#endregion
			}

			// 描画。

            int i選曲バーX座標 = 673; //選曲バーの座標用
            int i選択曲バーX座標 = 665; //選択曲バーの座標用

			return 0;
		}


        // その他

        #region [ private ]
        //-----------------

        private Point[] ptバー座標 = { new Point(194, 174), new Point(278, 174), new Point(389, 174), new Point(528, 174), new Point(666, 174), new Point(804, 174), new Point(804, 174) };

        public bool b曲選択;
        public bool b裏譜面;

        public CActSelect曲リスト.TitleTextureKey SongTitle;
        public CActSelect曲リスト.TitleTextureKey SongSubTitle;
        private CCounter ct決定待機;
        public int 縁カウント = 0;
        private CCounter ctBarAnime;
        private CCounter ct移動;
		private int n現在のスクロールカウンタ;
		public int n現在の選択行;
		private int n目標のスクロールカウンタ;
		//-----------------
		#endregion
	}
}
