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
            if (this.n現在の選択行 + 1 < 5)
            {
                this.n現在の選択行 += 1;
                TJAPlayer3.Skin.sound変更音.t再生する();
            }
        }
		public void t前に移動()
		{
			if( TJAPlayer3.stage選曲.r現在選択中の曲 != null )
			{
		        if( this.n現在の選択行 - 1 >= 0 )
                {
                    this.n現在の選択行 -= 1;
                    TJAPlayer3.Skin.sound変更音.t再生する();
                }
			}
		}
		public void t選択画面初期化()
		{
			//かんたんから一番近いところにカーソルを移動させる。
            for( int i = 0; i < (int)Difficulty.Total; i++ )
            {
                if( TJAPlayer3.stage選曲.r現在選択中の曲.arスコア[ i ] != null )
                {
                    this.n現在の選択行 = i;
                    break;
                }
            }

            int n譜面数 = 0;
            for( int i = 0; i < (int)Difficulty.Total; i++ )
			{
                if( TJAPlayer3.stage選曲.r現在選択中の曲.arスコア[ i ] != null ) n譜面数++;
            }
            for( int i = 0; i < (int)Difficulty.Total; i++ )
			{
                //描画順と座標を決める。
                switch( n譜面数 )
                {
                    case 1:
                    case 2:
                        this.n描画順 = new[] { 0, 1, 2, 3, 4 };
                        this.n踏み台座標 = new[] { 12, 252, 492, 732, 972 };
                        break;
                    case 3:
                        this.n描画順 = new[] { 0, 2, 1, 3, 4 };
                        this.n踏み台座標 = new[] { 12, 492, 252, 732, 972 };
                        break;
                    case 4:
                        this.n描画順 = new[] { 0, 2, 1, 3, 4 };
                        this.n踏み台座標 = new[] { 12, 492, 252, 732, 972 };
                        break;
                    case 5:
                        this.n描画順 = new[] { 0, 3, 1, 4, 2 };
                        this.n踏み台座標 = new[] { 12, 492, 972, 252, 732 };
                        break;
                }

            }
            this.b初めての進行描画 = true;
		}

		// CActivity 実装

		public override void On活性化()
		{
			if( this.b活性化してる )
				return;

			this.b登場アニメ全部完了 = false;
			this.n目標のスクロールカウンタ = 0;
			this.n現在のスクロールカウンタ = 0;
			this.nスクロールタイマ = -1;

			// フォント作成。
			// 曲リスト文字は２倍（面積４倍）でテクスチャに描画してから縮小表示するので、フォントサイズは２倍とする。
            this.ct三角矢印アニメ = new CCounter();
            this.ct移動 = new CCounter();

			base.On活性化();
		}
		public override void On非活性化()
		{
			if( this.b活性化してない )
				return;

			for( int i = 0; i < 13; i++ )
				this.ct登場アニメ用[ i ] = null;

            this.ct移動 = null;
            this.ct三角矢印アニメ = null;

			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( this.b活性化してない )
				return;

            this.soundSelectAnnounce = TJAPlayer3.Sound管理.tサウンドを生成する( CSkin.Path( @"Sounds\DiffSelect.ogg" ), ESoundGroup.SoundEffect );

			base.OnManagedリソースの作成();
		}
		public override void OnManagedリソースの解放()
		{
			if( this.b活性化してない )
				return;

            TJAPlayer3.t安全にDisposeする( ref this.soundSelectAnnounce );

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
				for( int i = 0; i < 13; i++ )
					this.ct登場アニメ用[ i ] = new CCounter( -i * 10, 100, 3, TJAPlayer3.Timer );
				this.nスクロールタイマ = CSound管理.rc演奏用タイマ.n現在時刻;
				TJAPlayer3.stage選曲.t選択曲変更通知();

                this.n矢印スクロール用タイマ値 = CSound管理.rc演奏用タイマ.n現在時刻;
				this.ct三角矢印アニメ.t開始( 0, 19, 40, TJAPlayer3.Timer );
				
                //this.soundSelectAnnounce.tサウンドを再生する();
				base.b初めての進行描画 = false;
			}
			//-----------------
			#endregion

			// 本ステージは、(1)登場アニメフェーズ → (2)通常フェーズ　と二段階にわけて進む。
			// ２つしかフェーズがないので CStage.eフェーズID を使ってないところがまた本末転倒。

			
			// 進行。
            //this.ct三角矢印アニメ.t進行Loop();

			{
				#region [ (2) 通常フェーズの進行。]
				//-----------------

                //キー操作
                if (TJAPlayer3.Pad.b押されたDGB(Eパッド.RBlue))
                {
                    this.t次に移動();
                }
                else if( TJAPlayer3.Pad.b押されたDGB(Eパッド.LBlue) )
                {
                    this.t前に移動();
                }
                else if ( ( TJAPlayer3.Pad.b押されたDGB( Eパッド.Decide ) ||
						( ( TJAPlayer3.ConfigIni.bEnterがキー割り当てのどこにも使用されていない && TJAPlayer3.Input管理.Keyboard.bキーが押された( (int) SlimDX.DirectInput.Key.Return ) ) ) ) )
                {
                    TJAPlayer3.stage選曲.actPresound.tサウンドの停止MT();
                    switch( TJAPlayer3.stage選曲.r現在選択中の曲.eノード種別 )
                    {
                        case C曲リストノード.Eノード種別.SCORE:
                            {
                                TJAPlayer3.Skin.sound決定音.t再生する();
                                TJAPlayer3.stage選曲.t曲を選択する( this.n現在の選択行 );
                            }
                            break;
                    }
                }
                else if( TJAPlayer3.Input管理.Keyboard.bキーが押された( (int) SlimDX.DirectInput.Key.Escape ) )
                {
                    this.bIsDifficltSelect = false;
                }

                TJAPlayer3.Tx.Diffculty_Back[TJAPlayer3.stage選曲.act曲リスト.nStrジャンルtoNum(TJAPlayer3.stage選曲.r現在選択中の曲.strジャンル)].Opacity = TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 - 1235;
                TJAPlayer3.Tx.Difficulty_Bar.Opacity = TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 - 1235;
                TJAPlayer3.Tx.Diffculty_Back[TJAPlayer3.stage選曲.act曲リスト.nStrジャンルtoNum(TJAPlayer3.stage選曲.r現在選択中の曲.strジャンル)].t2D中心基準描画(TJAPlayer3.app.Device, 640, 280);
                TJAPlayer3.Tx.Difficulty_Bar.t2D中心基準描画(TJAPlayer3.app.Device, 640, 380);

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

		private bool b登場アニメ全部完了;
		private CCounter[] ct登場アニメ用 = new CCounter[ 13 ];
        private CCounter ct三角矢印アニメ;
        private CCounter ct移動;
		private long nスクロールタイマ;
		private int n現在のスクロールカウンタ;
		private int n現在の選択行;
		private int n目標のスクロールカウンタ;

        private CSound soundSelectAnnounce;


        private long n矢印スクロール用タイマ値;

        private int[] n描画順;
        private int[] n踏み台座標;
		//-----------------
		#endregion
	}
}
