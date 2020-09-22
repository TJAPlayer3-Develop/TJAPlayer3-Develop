using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using FDK;

namespace TJAPlayer3
{
	internal class CActFIFOStart : CActivity
	{
		// メソッド

		public void tフェードアウト開始()
		{
			this.mode = EFIFOモード.フェードアウト;

            this.counter = new CCounter( 0, 640, 1, TJAPlayer3.Timer );
		}
		public void tフェードイン開始()
		{
			this.mode = EFIFOモード.フェードイン;
			this.counter = new CCounter( 0, 940, 1, TJAPlayer3.Timer );
		}
		public void tフェードイン完了()		// #25406 2011.6.9 yyagi
		{
			this.counter.n現在の値 = this.counter.n終了値;
		}

		// CActivity 実装

		public override void On非活性化()
		{
			if( !base.b活性化してない )
			{
                //CDTXMania.tテクスチャの解放( ref this.tx幕 );
				base.On非活性化();
			}
		}
		public override void OnManagedリソースの作成()
		{
			if( !base.b活性化してない )
			{
				//this.tx幕 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\6_FO.png" ) );
 			//	this.tx幕2 = CDTXMania.tテクスチャの生成( CSkin.Path( @"Graphics\6_FI.png" ) );
				base.OnManagedリソースの作成();
			}
		}
		public override int On進行描画()
		{
			if( base.b活性化してない || ( this.counter == null ) )
			{
				return 0;
			}
			this.counter.t進行();

			// Size clientSize = CDTXMania.app.Window.ClientSize;	// #23510 2010.10.31 yyagi: delete as of no one use this any longer.

            if( this.mode == EFIFOモード.フェードアウト )
            {
                if( TJAPlayer3.Tx.SongLoading_FadeOut != null )
			    {
                    int x = this.counter.n現在の値 >= 640 ? 640 : this.counter.n現在の値;
					TJAPlayer3.Tx.SongLoading_FadeOut.vc拡大縮小倍率.X = 0.8f + (this.counter.n現在の値 / 3200.0f);
					TJAPlayer3.Tx.SongLoading_FadeOut.t2D拡大率考慮右描画( TJAPlayer3.app.Device, 0 + x, 0, new Rectangle(0, 0, 640, 720) );
                    TJAPlayer3.Tx.SongLoading_FadeOut.t2D描画( TJAPlayer3.app.Device, 1280 - x, 0, new Rectangle(640, 0, 640, 720));
                }

			}
            else
            {
				if(this.counter.n現在の値 >= 300)
				{
					if (TJAPlayer3.Tx.SongLoading_FadeIn != null)
					{
						int x = (this.counter.n現在の値 - 300);
						TJAPlayer3.Tx.SongLoading_FadeIn.vc拡大縮小倍率.X = 1.0f - (this.counter.n現在の値 - 300) / 3200.0f;
						TJAPlayer3.Tx.SongLoading_FadeIn.t2D拡大率考慮右描画(TJAPlayer3.app.Device, 640 - x, 0, new Rectangle(0, 0, 640, 720));
						TJAPlayer3.Tx.SongLoading_FadeIn.t2D描画(TJAPlayer3.app.Device, 640 + x, 0, new Rectangle(640, 0, 640, 720));
					}
				}
                else
                {
					TJAPlayer3.Tx.SongLoading_Background.t2D描画(TJAPlayer3.app.Device, 0, 0);
					TJAPlayer3.Tx.SongLoading_Chara.Opacity = 255 - counter.n現在の値;
					TJAPlayer3.Tx.SongLoading_Chara.t2D描画(TJAPlayer3.app.Device, 0, counter.n現在の値 <= 50 ? 0 + counter.n現在の値 : -50 - counter.n現在の値);
				}
            }

            if( this.mode == EFIFOモード.フェードアウト )
            {
			    if( this.counter.n現在の値 != 640)
			    {
				    return 0;
			    }
            }
            else if( this.mode == EFIFOモード.フェードイン )
            {
			    if( this.counter.n現在の値 != 940)
			    {
				    return 0;
			    }
            }
			return 1;
		}


		// その他

		#region [ private ]
		//-----------------
		private CCounter counter;
        private CCounter ct待機;
		private EFIFOモード mode;
        //private CTexture tx幕;
        //private CTexture tx幕2;
		//-----------------
		#endregion
	}
}
