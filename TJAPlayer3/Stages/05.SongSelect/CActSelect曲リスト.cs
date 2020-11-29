using CSharpTest.Net.Collections;
using FDK;
using SlimDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;

namespace TJAPlayer3
{
	internal class CActSelect曲リスト : CActivity
	{
		// プロパティ

		public bool bIsEnumeratingSongs
		{
			get;
			set;
		}
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
		public int n現在のアンカ難易度レベル 
		{
			get;
			private set;
		}
		public int n現在選択中の曲の現在の難易度レベル
		{
			get
			{
				return this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す( this.r現在選択中の曲 );
			}
		}
		public Cスコア r現在選択中のスコア
		{
			get
			{
				if( this.r現在選択中の曲 != null )
				{
					return this.r現在選択中の曲.arスコア[ this.n現在選択中の曲の現在の難易度レベル ];
				}
				return null;
			}
		}
		public C曲リストノード r現在選択中の曲 
		{
			get;
			private set;
		}

		public int nスクロールバー相対y座標
		{
			get;
			private set;
		}

		// t選択曲が変更された()内で使う、直前の選曲の保持
		// (前と同じ曲なら選択曲変更に掛かる再計算を省略して高速化するため)
		private C曲リストノード song_last = null;

		
		// コンストラクタ

		public CActSelect曲リスト()
        {
			#region[ レベル数字 ]
			STレベル数字[] stレベル数字Ar = new STレベル数字[10];
			STレベル数字 st数字0 = new STレベル数字();
			STレベル数字 st数字1 = new STレベル数字();
			STレベル数字 st数字2 = new STレベル数字();
			STレベル数字 st数字3 = new STレベル数字();
			STレベル数字 st数字4 = new STレベル数字();
			STレベル数字 st数字5 = new STレベル数字();
			STレベル数字 st数字6 = new STレベル数字();
			STレベル数字 st数字7 = new STレベル数字();
			STレベル数字 st数字8 = new STレベル数字();
			STレベル数字 st数字9 = new STレベル数字();

			st数字0.ch = '0';
			st数字1.ch = '1';
			st数字2.ch = '2';
			st数字3.ch = '3';
			st数字4.ch = '4';
			st数字5.ch = '5';
			st数字6.ch = '6';
			st数字7.ch = '7';
			st数字8.ch = '8';
			st数字9.ch = '9';
			st数字0.ptX = 0;
			st数字1.ptX = 21;
			st数字2.ptX = 42;
			st数字3.ptX = 63;
			st数字4.ptX = 84;
			st数字5.ptX = 105;
			st数字6.ptX = 126;
			st数字7.ptX = 147;
			st数字8.ptX = 168;
			st数字9.ptX = 189;

			stレベル数字Ar[0] = st数字0;
			stレベル数字Ar[1] = st数字1;
			stレベル数字Ar[2] = st数字2;
			stレベル数字Ar[3] = st数字3;
			stレベル数字Ar[4] = st数字4;
			stレベル数字Ar[5] = st数字5;
			stレベル数字Ar[6] = st数字6;
			stレベル数字Ar[7] = st数字7;
			stレベル数字Ar[8] = st数字8;
			stレベル数字Ar[9] = st数字9;
			this.st小文字位置 = stレベル数字Ar;
			#endregion


			this.r現在選択中の曲 = null;
            this.n現在のアンカ難易度レベル = TJAPlayer3.ConfigIni.nDefaultCourse;
			base.b活性化してない = true;
			this.bIsEnumeratingSongs = false;
		}


		// メソッド

		public int n現在のアンカ難易度レベルに最も近い難易度レベルを返す( C曲リストノード song )
		{
			// 事前チェック。

			if( song == null )
				return this.n現在のアンカ難易度レベル;	// 曲がまったくないよ

			if( song.arスコア[ this.n現在のアンカ難易度レベル ] != null )
				return this.n現在のアンカ難易度レベル;	// 難易度ぴったりの曲があったよ

			if( ( song.eノード種別 == C曲リストノード.Eノード種別.BOX ) || ( song.eノード種別 == C曲リストノード.Eノード種別.BACKBOX ) )
				return 0;								// BOX と BACKBOX は関係無いよ


			// 現在のアンカレベルから、難易度上向きに検索開始。

			int n最も近いレベル = this.n現在のアンカ難易度レベル;

			for( int i = 0; i < (int)Difficulty.Total; i++ )
			{
				if( song.arスコア[ n最も近いレベル ] != null )
					break;	// 曲があった。

				n最も近いレベル = ( n最も近いレベル + 1 ) % (int)Difficulty.Total;	// 曲がなかったので次の難易度レベルへGo。（5以上になったら0に戻る。）
			}


			// 見つかった曲がアンカより下のレベルだった場合……
			// アンカから下向きに検索すれば、もっとアンカに近い曲があるんじゃね？

			if( n最も近いレベル < this.n現在のアンカ難易度レベル )
			{
				// 現在のアンカレベルから、難易度下向きに検索開始。

				n最も近いレベル = this.n現在のアンカ難易度レベル;

				for( int i = 0; i < (int)Difficulty.Total; i++ )
				{
					if( song.arスコア[ n最も近いレベル ] != null )
						break;	// 曲があった。

					n最も近いレベル = ( ( n最も近いレベル - 1 ) + (int)Difficulty.Total) % (int)Difficulty.Total;	// 曲がなかったので次の難易度レベルへGo。（0未満になったら4に戻る。）
				}
			}

			return n最も近いレベル;
		}
		public C曲リストノード r指定された曲が存在するリストの先頭の曲( C曲リストノード song )
		{
			List<C曲リストノード> songList = GetSongListWithinMe( song );
			return ( songList == null ) ? null : songList[ 0 ];
		}
		public C曲リストノード r指定された曲が存在するリストの末尾の曲( C曲リストノード song )
		{
			List<C曲リストノード> songList = GetSongListWithinMe( song );
			return ( songList == null ) ? null : songList[ songList.Count - 1 ];
		}

		private List<C曲リストノード> GetSongListWithinMe( C曲リストノード song )
		{
			if ( song.r親ノード == null )					// root階層のノートだったら
			{
				return TJAPlayer3.Songs管理.list曲ルート;	// rootのリストを返す
			}
			else
			{
				if ( ( song.r親ノード.list子リスト != null ) && ( song.r親ノード.list子リスト.Count > 0 ) )
				{
					return song.r親ノード.list子リスト;
				}
				else
				{
					return null;
				}
			}
		}


		public delegate void DGSortFunc( List<C曲リストノード> songList, E楽器パート eInst, int order, params object[] p);
		/// <summary>
		/// 主にCSong管理.cs内にあるソート機能を、delegateで呼び出す。
		/// </summary>
		/// <param name="sf">ソート用に呼び出すメソッド</param>
		/// <param name="eInst">ソート基準とする楽器</param>
		/// <param name="order">-1=降順, 1=昇順</param>
		public void t曲リストのソート( DGSortFunc sf, E楽器パート eInst, int order, params object[] p )
		{
			List<C曲リストノード> songList = GetSongListWithinMe( this.r現在選択中の曲 );
			if ( songList == null )
			{
				// 何もしない;
			}
			else
			{
//				CDTXMania.Songs管理.t曲リストのソート3_演奏回数の多い順( songList, eInst, order );
				sf( songList, eInst, order, p );
//				this.r現在選択中の曲 = CDTXMania
				this.t現在選択中の曲を元に曲バーを再構成する();
			}
		}

		public bool tBOXに入る()
		{
			//Trace.TraceInformation( "box enter" );
			//Trace.TraceInformation( "Skin現在Current : " + CDTXMania.Skin.GetCurrentSkinSubfolderFullName(false) );
			//Trace.TraceInformation( "Skin現在System  : " + CSkin.strSystemSkinSubfolderFullName );
			//Trace.TraceInformation( "Skin現在BoxDef  : " + CSkin.strBoxDefSkinSubfolderFullName );
			//Trace.TraceInformation( "Skin現在: " + CSkin.GetSkinName( CDTXMania.Skin.GetCurrentSkinSubfolderFullName(false) ) );
			//Trace.TraceInformation( "Skin現pt: " + CDTXMania.Skin.GetCurrentSkinSubfolderFullName(false) );
			//Trace.TraceInformation( "Skin指定: " + CSkin.GetSkinName( this.r現在選択中の曲.strSkinPath ) );
			//Trace.TraceInformation( "Skinpath: " + this.r現在選択中の曲.strSkinPath );
			bool ret = false;
			if (CSkin.GetSkinName(TJAPlayer3.Skin.GetCurrentSkinSubfolderFullName(false)) != CSkin.GetSkinName(this.r現在選択中の曲.strSkinPath)
				&& CSkin.bUseBoxDefSkin)
			{
				ret = true;
				// BOXに入るときは、スキン変更発生時のみboxdefスキン設定の更新を行う
				TJAPlayer3.Skin.SetCurrentSkinSubfolderFullName(
					TJAPlayer3.Skin.GetSkinSubfolderFullNameFromSkinName(CSkin.GetSkinName(this.r現在選択中の曲.strSkinPath)), false);
			}

			//Trace.TraceInformation( "Skin変更: " + CSkin.GetSkinName( CDTXMania.Skin.GetCurrentSkinSubfolderFullName(false) ) );
			//Trace.TraceInformation( "Skin変更Current : "+  CDTXMania.Skin.GetCurrentSkinSubfolderFullName(false) );
			//Trace.TraceInformation( "Skin変更System  : "+  CSkin.strSystemSkinSubfolderFullName );
			//Trace.TraceInformation( "Skin変更BoxDef  : "+  CSkin.strBoxDefSkinSubfolderFullName );

			if(( this.r現在選択中の曲.list子リスト != null ))
			{
				List<C曲リストノード> list = TJAPlayer3.Songs管理.list曲ルート;
				list.InsertRange(list.IndexOf(this.r現在選択中の曲) + 1, this.r現在選択中の曲.list子リスト);
				int n回数 = this.r現在選択中の曲.Openindex;
				for (int index = 0; index <= n回数; index++)
					this.r現在選択中の曲 = this.r次の曲(this.r現在選択中の曲);
				if(list.IndexOf(this.r現在選択中の曲.r親ノード) != -1)
					list.RemoveAt(list.IndexOf(this.r現在選択中の曲.r親ノード));
				this.t現在選択中の曲を元に曲バーを再構成する();
				this.t選択曲が変更された(false);
				TJAPlayer3.stage選曲.t選択曲変更通知();                          // #27648 項目数変更を反映させる
				this.b選択曲が変更された = true;
			}
			return ret;
		}
		public bool tBOXを出る()
		{
			bool ret = false;
			if (CSkin.GetSkinName(TJAPlayer3.Skin.GetCurrentSkinSubfolderFullName(false)) != CSkin.GetSkinName(this.r現在選択中の曲.strSkinPath)
				&& CSkin.bUseBoxDefSkin)
			{
				ret = true;
			}
			// スキン変更が発生しなくても、boxdef圏外に出る場合は、boxdefスキン設定の更新が必要
			// (ユーザーがboxdefスキンをConfig指定している場合への対応のために必要)
			// tBoxに入る()とは処理が微妙に異なるので注意
			TJAPlayer3.Skin.SetCurrentSkinSubfolderFullName(
				(this.r現在選択中の曲.strSkinPath == "") ? "" : TJAPlayer3.Skin.GetSkinSubfolderFullNameFromSkinName(CSkin.GetSkinName(this.r現在選択中の曲.strSkinPath)), false);

			if ( this.r現在選択中の曲.r親ノード != null )
			{
				List<C曲リストノード> list = TJAPlayer3.Songs管理.list曲ルート;
				this.r現在選択中の曲.r親ノード.Openindex = r現在選択中の曲.r親ノード.list子リスト.IndexOf(this.r現在選択中の曲);
				list.Insert(list.IndexOf(this.r現在選択中の曲) + 1, this.r現在選択中の曲.r親ノード);
				this.r現在選択中の曲 = this.r次の曲(r現在選択中の曲);
				for (int index = 0; index < list.Count; index++)
				{
					if (this.r現在選択中の曲.list子リスト.Contains(list[index]))
					{
						list.RemoveAt(index);
						index--;
					}
				}
				this.t現在選択中の曲を元に曲バーを再構成する();
				this.t選択曲が変更された(false);
				TJAPlayer3.stage選曲.t選択曲変更通知();                                 // #27648 項目数変更を反映させる
				this.b選択曲が変更された = true;
			}
			return ret;
		}
		public void t現在選択中の曲を元に曲バーを再構成する()
		{
			this.tバーの初期化();
		}
		public void t次に移動()
		{
			if( this.r現在選択中の曲 != null )
			{
				this.n目標のスクロールカウンタ += 100;
			}
			this.b選択曲が変更された = true;
		}
		public void t前に移動()
		{
			if( this.r現在選択中の曲 != null )
			{
				this.n目標のスクロールカウンタ -= 100;
			}
			this.b選択曲が変更された = true;
		}

		/// <summary>
		/// 曲リストをリセットする
		/// </summary>
		/// <param name="cs"></param>
		public void Refresh(CSongs管理 cs, bool bRemakeSongTitleBar )		// #26070 2012.2.28 yyagi
		{
//			this.On非活性化();

			if ( cs != null && cs.list曲ルート.Count > 0 )	// 新しい曲リストを検索して、1曲以上あった
			{
				TJAPlayer3.Songs管理 = cs;

				if ( this.r現在選択中の曲 != null )			// r現在選択中の曲==null とは、「最初songlist.dbが無かった or 検索したが1曲もない」
				{
					this.r現在選択中の曲 = searchCurrentBreadcrumbsPosition( TJAPlayer3.Songs管理.list曲ルート, this.r現在選択中の曲.strBreadcrumbs );
					if ( bRemakeSongTitleBar )					// 選曲画面以外に居るときには再構成しない (非活性化しているときに実行すると例外となる)
					{
						this.t現在選択中の曲を元に曲バーを再構成する();
					}
#if false			// list子リストの中まではmatchしてくれないので、検索ロジックは手書きで実装 (searchCurrentBreadcrumbs())
					string bc = this.r現在選択中の曲.strBreadcrumbs;
					Predicate<C曲リストノード> match = delegate( C曲リストノード c )
					{
						return ( c.strBreadcrumbs.Equals( bc ) );
					};
					int nMatched = CDTXMania.Songs管理.list曲ルート.FindIndex( match );

					this.r現在選択中の曲 = ( nMatched == -1 ) ? null : CDTXMania.Songs管理.list曲ルート[ nMatched ];
					this.t現在選択中の曲を元に曲バーを再構成する();
#endif
					return;
				}
			}
			this.On非活性化();
			this.r現在選択中の曲 = null;
			this.On活性化();
		}


		/// <summary>
		/// 現在選曲している位置を検索する
		/// (曲一覧クラスを新しいものに入れ替える際に用いる)
		/// </summary>
		/// <param name="ln">検索対象のList</param>
		/// <param name="bc">検索するパンくずリスト(文字列)</param>
		/// <returns></returns>
		private C曲リストノード searchCurrentBreadcrumbsPosition( List<C曲リストノード> ln, string bc )
		{
			foreach (C曲リストノード n in ln)
			{
				if ( n.strBreadcrumbs == bc )
				{
					return n;
				}
				else if ( n.list子リスト != null && n.list子リスト.Count > 0 )	// 子リストが存在するなら、再帰で探す
				{
					C曲リストノード r = searchCurrentBreadcrumbsPosition( n.list子リスト, bc );
					if ( r != null ) return r;
				}
			}
			return null;
		}

		/// <summary>
		/// BOXのアイテム数と、今何番目を選択しているかをセットする
		/// </summary>
		public void t選択曲が変更された( bool bForce )	// #27648
		{
			C曲リストノード song = TJAPlayer3.stage選曲.r現在選択中の曲;
			if ( song == null )
				return;
			if ( song == song_last && bForce == false )
				return;
				
			song_last = song;
			List<C曲リストノード> list = TJAPlayer3.Songs管理.list曲ルート;
			int index = list.IndexOf( song ) + 1;
			if ( index <= 0 )
			{
				nCurrentPosition = nNumOfItems = 0;
			}
			else
			{
				nCurrentPosition = index;
				nNumOfItems = list.Count;
			}
		}

		// CActivity 実装

		public override void On活性化()
		{
			if( this.b活性化してる )
				return;

            // Reset to not performing calibration each time we
            // enter or return to the song select screen.
            TJAPlayer3.IsPerformingCalibration = false;

            if (!string.IsNullOrEmpty(TJAPlayer3.ConfigIni.FontName))
            {
                this.pfMusicName = new CPrivateFastFont(new FontFamily(TJAPlayer3.ConfigIni.FontName), 23);
                this.pfSubtitle = new CPrivateFastFont(new FontFamily(TJAPlayer3.ConfigIni.FontName), 16);
            }
            else
            {
                this.pfMusicName = new CPrivateFastFont(new FontFamily("MS UI Gothic"), 28);
                this.pfSubtitle = new CPrivateFastFont(new FontFamily("MS UI Gothic"), 20);
            }

		    _titleTextures.ItemRemoved += OnTitleTexturesOnItemRemoved;
		    _titleTextures.ItemUpdated += OnTitleTexturesOnItemUpdated;

            this.e楽器パート = E楽器パート.DRUMS;
			this.b登場アニメ全部完了 = false;
			this.n目標のスクロールカウンタ = 0;
			this.n現在のスクロールカウンタ = 0;
			this.nスクロールタイマ = -1;

			// フォント作成。
			// 曲リスト文字は２倍（面積４倍）でテクスチャに描画してから縮小表示するので、フォントサイズは２倍とする。

			FontStyle regular = FontStyle.Regular;
			this.ft曲リスト用フォント = new Font( TJAPlayer3.ConfigIni.FontName, 40f, regular, GraphicsUnit.Pixel );


			// 現在選択中の曲がない（＝はじめての活性化）なら、現在選択中の曲をルートの先頭ノードに設定する。

			if( ( this.r現在選択中の曲 == null ) && ( TJAPlayer3.Songs管理.list曲ルート.Count > 0 ) )
				this.r現在選択中の曲 = TJAPlayer3.Songs管理.list曲ルート[ 0 ];




			// バー情報を初期化する。

			this.tバーの初期化();

            this.ct三角矢印アニメ = new CCounter();

			base.On活性化();

			this.t選択曲が変更された(true);		// #27648 2012.3.31 yyagi 選曲画面に入った直後の 現在位置/全アイテム数 の表示を正しく行うため
		}
		public override void On非活性化()
		{
			if( this.b活性化してない )
				return;

		    _titleTextures.ItemRemoved -= OnTitleTexturesOnItemRemoved;
		    _titleTextures.ItemUpdated -= OnTitleTexturesOnItemUpdated;

			TJAPlayer3.t安全にDisposeする( ref this.ft曲リスト用フォント );

            this.ct三角矢印アニメ = null;

			base.On非活性化();
		}
		public override void OnManagedリソースの作成()
		{
			if( this.b活性化してない )
				return;

			ctBarOpen = new CCounter();
			ctBoxExplanationOpacity = new CCounter();
			ctBoxOpen = new CCounter();
			ctBoxClose = new CCounter();

			if (!string.IsNullOrEmpty(TJAPlayer3.ConfigIni.BoxFontName))
				pfBoxExplanation = new CPrivateFont(new FontFamily(TJAPlayer3.ConfigIni.BoxFontName), 15);
			else
				pfBoxExplanation = new CPrivateFont(new FontFamily("MS UI Gothic"), 15);

			for (int i = 0; i < 3; i++)
			{
				if (TJAPlayer3.stage選曲.r現在選択中の曲 != null)
				{
					if (TJAPlayer3.stage選曲.r現在選択中の曲.strボックス説明[i] != null)
					{
						using (var pfBE = pfBoxExplanation.DrawPrivateFont(TJAPlayer3.stage選曲.r現在選択中の曲.strボックス説明[i],
							TJAPlayer3.stage選曲.r現在選択中の曲.ForeColor,
							TJAPlayer3.stage選曲.r現在選択中の曲.BackColor))
						{
							txBoxExplanation[i] = TJAPlayer3.tテクスチャの生成(pfBE);
							this.txBoxExplanation[i].vc拡大縮小倍率.X = this.txBoxExplanation[i].szテクスチャサイズ.Width >= 540f ? 540f / this.txBoxExplanation[i].szテクスチャサイズ.Width : 1.0f;
							OldBoxExplanetion = TJAPlayer3.stage選曲.r現在選択中の曲.strボックス説明[0] + "\n" + TJAPlayer3.stage選曲.r現在選択中の曲.strボックス説明[1] + "\n" + TJAPlayer3.stage選曲.r現在選択中の曲.strボックス説明[2];
						}
					}
				}
			}

			for ( int i = 0; i < 13; i++ )
            {
                //this.t曲名バーの生成(i, this.stバー情報[i].strタイトル文字列, this.stバー情報[i].ForeColor, this.stバー情報[i].BackColor);
                this.stバー情報[ i ].ttkタイトル = this.ttk曲名テクスチャを生成する( this.stバー情報[ i ].strタイトル文字列, this.stバー情報[i].ForeColor, this.stバー情報[i].BackColor);
            }

			int c = ( CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ja" ) ? 0 : 1;

			#region [ Songs were not found画像 ]
			try
			{
				using( Bitmap image = new Bitmap( 640, 128 ) )
				using( Graphics graphics = Graphics.FromImage( image ) )
				{
					string[] s1 = { "曲データが見つかりません。", "Songs were not found." };
					string[] s2 = { "曲データをDTXManiaGR.exe以下の", "You need to install songs first." };
					string[] s3 = { "フォルダにインストールして下さい。", "" };
					graphics.DrawString( s1[c], this.ft曲リスト用フォント, Brushes.DarkGray, (float) 2f, (float) 2f );
					graphics.DrawString( s1[c], this.ft曲リスト用フォント, Brushes.White, (float) 0f, (float) 0f );
					graphics.DrawString( s2[c], this.ft曲リスト用フォント, Brushes.DarkGray, (float) 2f, (float) 44f );
					graphics.DrawString( s2[c], this.ft曲リスト用フォント, Brushes.White, (float) 0f, (float) 42f );
					graphics.DrawString( s3[c], this.ft曲リスト用フォント, Brushes.DarkGray, (float) 2f, (float) 86f );
					graphics.DrawString( s3[c], this.ft曲リスト用フォント, Brushes.White, (float) 0f, (float) 84f );

					this.txSongNotFound = new CTexture( TJAPlayer3.app.Device, image, TJAPlayer3.TextureFormat );

					this.txSongNotFound.vc拡大縮小倍率 = new Vector3( 0.5f, 0.5f, 1f );	// 半分のサイズで表示する。
				}
			}
			catch( CTextureCreateFailedException e )
			{
				Trace.TraceError( e.ToString() );
				Trace.TraceError( "SoungNotFoundテクスチャの作成に失敗しました。" );
				this.txSongNotFound = null;
			}
			#endregion
			#region [ "曲データを検索しています"画像 ]
			try
			{
				using ( Bitmap image = new Bitmap( 640, 96 ) )
				using ( Graphics graphics = Graphics.FromImage( image ) )
				{
					string[] s1 = { "曲データを検索しています。", "Now enumerating songs." };
					string[] s2 = { "そのまましばらくお待ち下さい。", "Please wait..." };
					graphics.DrawString( s1[c], this.ft曲リスト用フォント, Brushes.DarkGray, (float) 2f, (float) 2f );
					graphics.DrawString( s1[c], this.ft曲リスト用フォント, Brushes.White, (float) 0f, (float) 0f );
					graphics.DrawString( s2[c], this.ft曲リスト用フォント, Brushes.DarkGray, (float) 2f, (float) 44f );
					graphics.DrawString( s2[c], this.ft曲リスト用フォント, Brushes.White, (float) 0f, (float) 42f );

					this.txEnumeratingSongs = new CTexture( TJAPlayer3.app.Device, image, TJAPlayer3.TextureFormat );

					this.txEnumeratingSongs.vc拡大縮小倍率 = new Vector3( 0.5f, 0.5f, 1f );	// 半分のサイズで表示する。
				}
			}
			catch ( CTextureCreateFailedException e )
			{
				Trace.TraceError( e.ToString() );
				Trace.TraceError( "txEnumeratingSongsテクスチャの作成に失敗しました。" );
				this.txEnumeratingSongs = null;
			}
			#endregion

			base.OnManagedリソースの作成();
		}

		public override void OnManagedリソースの解放()
		{
			if( this.b活性化してない )
				return;

			//CDTXMania.t安全にDisposeする( ref this.txアイテム数数字 );

			for( int i = 0; i < 13; i++ )
            {
                TJAPlayer3.t安全にDisposeする( ref this.stバー情報[ i ].txタイトル名 );
                this.stバー情報[ i ].ttkタイトル = null;
            }

		    ClearTitleTextureCache();

            TJAPlayer3.t安全にDisposeする( ref this.txEnumeratingSongs );
            TJAPlayer3.t安全にDisposeする( ref this.txSongNotFound );

			base.OnManagedリソースの解放();
		}
		public override int On進行描画()
		{
			if (this.b活性化してない)
				return 0;

			#region [ 初めての進行描画 ]
			//-----------------
			if (this.b初めての進行描画)
			{
				ctBoxClose.n現在の値 = 130;
				this.nスクロールタイマ = CSound管理.rc演奏用タイマ.n現在時刻;
				TJAPlayer3.stage選曲.t選択曲変更通知();

				this.n矢印スクロール用タイマ値 = CSound管理.rc演奏用タイマ.n現在時刻;
				this.ct三角矢印アニメ.t開始(0, 1000, 1, TJAPlayer3.Timer);
				ctBarOpen.t開始(0, 161, 2, TJAPlayer3.Timer);
				this.ctBoxExplanationOpacity.t開始(0, 210, 2, TJAPlayer3.Timer);
				TJAPlayer3.stage選曲.act難易度選択画面.bIsDifficltSelect = false;
				base.b初めての進行描画 = false;
			}
			//-----------------
			#endregion

			// まだ選択中の曲が決まってなければ、曲ツリールートの最初の曲にセットする。

			if ((this.r現在選択中の曲 == null) && (TJAPlayer3.Songs管理.list曲ルート.Count > 0))
				this.r現在選択中の曲 = TJAPlayer3.Songs管理.list曲ルート[0];


			// 本ステージは、(1)登場アニメフェーズ → (2)通常フェーズ　と二段階にわけて進む。

			ctBarOpen.t進行();
			ctBoxOpen.t進行();
			ctBoxClose.t進行();
			ctBoxExplanationOpacity.t進行();

			if (TJAPlayer3.stage選曲.r現在選択中の曲 != null)
			{
				if (TJAPlayer3.stage選曲.r現在選択中の曲.strボックス説明[0] != null && TJAPlayer3.stage選曲.r現在選択中の曲.strボックス説明[1] != null && TJAPlayer3.stage選曲.r現在選択中の曲.strボックス説明[2] != null)
				{
					if (TJAPlayer3.stage選曲.r現在選択中の曲.strボックス説明[0] + "\n" + TJAPlayer3.stage選曲.r現在選択中の曲.strボックス説明[1] + "\n" + TJAPlayer3.stage選曲.r現在選択中の曲.strボックス説明[2] != OldBoxExplanetion)
					{
						for (int i = 0; i < 3; i++)
						{
							using (var pfBE = pfBoxExplanation.DrawPrivateFont(TJAPlayer3.stage選曲.r現在選択中の曲.strボックス説明[i], TJAPlayer3.stage選曲.r現在選択中の曲.ForeColor, TJAPlayer3.stage選曲.r現在選択中の曲.BackColor))
							{
								OldBoxExplanetion = TJAPlayer3.stage選曲.r現在選択中の曲.strボックス説明[0] + "\n" + TJAPlayer3.stage選曲.r現在選択中の曲.strボックス説明[1] + "\n" + TJAPlayer3.stage選曲.r現在選択中の曲.strボックス説明[2];
								txBoxExplanation[i] = TJAPlayer3.tテクスチャの生成(pfBE);
								this.txBoxExplanation[i].vc拡大縮小倍率.X = this.txBoxExplanation[i].szテクスチャサイズ.Width >= 540f ? 540f / this.txBoxExplanation[i].szテクスチャサイズ.Width : 1.0f;
							}
						}
					}
				}
			} 

			// 進行。
			if (n現在のスクロールカウンタ == 0) ct三角矢印アニメ.t進行Loop();
            else ct三角矢印アニメ.n現在の値 = 0;

			{
				#region [ (2) 通常フェーズの進行。]
				//-----------------
				long n現在時刻 = CSound管理.rc演奏用タイマ.n現在時刻;
				
				if( n現在時刻 < this.nスクロールタイマ )	// 念のため
					this.nスクロールタイマ = n現在時刻;

				const int nアニメ間隔 = 2;
				while( ( n現在時刻 - this.nスクロールタイマ ) >= nアニメ間隔 )
				{
					float n加速度 = 1;
					int n残距離 = Math.Abs( (int) ( this.n目標のスクロールカウンタ - this.n現在のスクロールカウンタ ) );

                    #region [ 残距離が遠いほどスクロールを速くする（＝n加速度を多くする）。]
                    //-----------------
                    if (n残距離 <= 10)
                    {
                        n加速度 = 1.4f;
                    }
                    else if ( n残距離 <= 100 )
					{
						n加速度 = 1.4f;
					}
					else if( n残距離 <= 300 )
					{
						n加速度 = 3;
					}
					else if( n残距離 <= 500 )
					{
						n加速度 = 4;
					}
					else
					{
						n加速度 = 8;
					}
					//-----------------
					#endregion

					#region [ 加速度を加算し、現在のスクロールカウンタを目標のスクロールカウンタまで近づける。 ]
					//-----------------
					if( this.n現在のスクロールカウンタ < this.n目標のスクロールカウンタ )		// (A) 正の方向に未達の場合：
					{
						this.n現在のスクロールカウンタ += n加速度;								// カウンタを正方向に移動する。

						if( this.n現在のスクロールカウンタ > this.n目標のスクロールカウンタ )
							this.n現在のスクロールカウンタ = this.n目標のスクロールカウンタ;	// 到着！スクロール停止！
					}

					else if( this.n現在のスクロールカウンタ > this.n目標のスクロールカウンタ )	// (B) 負の方向に未達の場合：
					{
						this.n現在のスクロールカウンタ -= n加速度;								// カウンタを負方向に移動する。

						if( this.n現在のスクロールカウンタ < this.n目標のスクロールカウンタ )	// 到着！スクロール停止！
							this.n現在のスクロールカウンタ = this.n目標のスクロールカウンタ;
					}
					//-----------------
					#endregion

					if( this.n現在のスクロールカウンタ >= 100 )		// １行＝100カウント。
					{
						#region [ パネルを１行上にシフトする。]
						//-----------------

						// 選択曲と選択行を１つ下の行に移動。

						this.r現在選択中の曲 = this.r次の曲( this.r現在選択中の曲 );
						this.n現在の選択行 = ( this.n現在の選択行 + 1 ) % 13;


						// 選択曲から７つ下のパネル（＝新しく最下部に表示されるパネル。消えてしまう一番上のパネルを再利用する）に、新しい曲の情報を記載する。

						C曲リストノード song = this.r現在選択中の曲;
						for( int i = 0; i < 7; i++ )
							song = this.r次の曲( song );

						int index = ( this.n現在の選択行 + 7 ) % 13;	// 新しく最下部に表示されるパネルのインデックス（0～12）。
						this.stバー情報[ index ].strタイトル文字列 = song.strタイトル;
                        this.stバー情報[index].ForeColor = song.ForeColor;
                        this.stバー情報[index].BackColor = song.BackColor;
                        this.stバー情報[ index ].strジャンル = song.strジャンル;
                        this.stバー情報[ index ].strサブタイトル = song.strサブタイトル;
                        this.stバー情報[ index ].ar難易度 = song.nLevel;
                        for ( int f = 0; f < (int)Difficulty.Total; f++ )
                        {
                            if( song.arスコア[ f ] != null )
                                this.stバー情報[ index ].b分岐 = song.arスコア[ f ].譜面情報.b譜面分岐;
                        }


						for (int j = 0; j < 5; j++)
						{
							if (song.arスコア[j] != null)
							{
								this.stバー情報[index].bクリア[j] = song.arスコア[j].譜面情報.クリア[j];
								this.stバー情報[index].bフルコンボ[j] = song.arスコア[j].譜面情報.フルコンボ[j];
								this.stバー情報[index].bドンダフルコンボ[j] = song.arスコア[j].譜面情報.ドンダフルコンボ[j];
								this.stバー情報[index].nスコアランク = song.arスコア[j].譜面情報.nスコアランク;
							}
						}

						// stバー情報[] の内容を1行ずつずらす。

						C曲リストノード song2 = this.r現在選択中の曲;
						for( int i = 0; i < 5; i++ )
							song2 = this.r前の曲( song2 );

						for( int i = 0; i < 13; i++ )
						{
							int n = ( ( ( this.n現在の選択行 - 5 ) + i ) + 13 ) % 13;
							this.stバー情報[ n ].eバー種別 = this.e曲のバー種別を返す( song2 );
							song2 = this.r次の曲( song2 );
                            this.stバー情報[ i ].ttkタイトル = this.ttk曲名テクスチャを生成する( this.stバー情報[ i ].strタイトル文字列, this.stバー情報[i].ForeColor, this.stバー情報[i].BackColor);

						}

						
						// 新しく最下部に表示されるパネル用のスキル値を取得。

						for( int i = 0; i < 3; i++ )
							this.stバー情報[ index ].nスキル値[ i ] = (int) song.arスコア[ this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す( song ) ].譜面情報.最大スキル[ i ];


						// 1行(100カウント)移動完了。

						this.n現在のスクロールカウンタ -= 100;
						this.n目標のスクロールカウンタ -= 100;

						this.t選択曲が変更された(false);				// スクロールバー用に今何番目を選択しているかを更新



						if( this.n目標のスクロールカウンタ == 0)
						{
							TJAPlayer3.stage選曲.t選択曲変更通知();      // スクロール完了＝選択曲変更！
							ctBarOpen.t開始(0, 161, 2, TJAPlayer3.Timer);
							this.ctBoxExplanationOpacity.t開始(0, 211, 2, TJAPlayer3.Timer);
						}

						//-----------------
						#endregion
					}
					else if( this.n現在のスクロールカウンタ <= -100 )
					{
						#region [ パネルを１行下にシフトする。]
						//-----------------

						// 選択曲と選択行を１つ上の行に移動。

						this.r現在選択中の曲 = this.r前の曲( this.r現在選択中の曲 );
						this.n現在の選択行 = ( ( this.n現在の選択行 - 1 ) + 13 ) % 13;


						// 選択曲から５つ上のパネル（＝新しく最上部に表示されるパネル。消えてしまう一番下のパネルを再利用する）に、新しい曲の情報を記載する。

						C曲リストノード song = this.r現在選択中の曲;
						for( int i = 0; i < 5; i++ )
							song = this.r前の曲( song );

						int index = ( ( this.n現在の選択行 - 5 ) + 13 ) % 13;	// 新しく最上部に表示されるパネルのインデックス（0～12）。
						this.stバー情報[ index ].strタイトル文字列 = song.strタイトル;
                        this.stバー情報[index].ForeColor = song.ForeColor;
                        this.stバー情報[index].BackColor = song.BackColor;
                        this.stバー情報[ index ].strサブタイトル = song.strサブタイトル;
                        this.stバー情報[ index ].strジャンル = song.strジャンル;
                        this.stバー情報[ index ].ar難易度 = song.nLevel;

						for (int j = 0; j < 5; j++)
						{
							if (song.arスコア[j] != null)
							{
								this.stバー情報[index].bクリア[j] = song.arスコア[j].譜面情報.クリア[j];
								this.stバー情報[index].bフルコンボ[j] = song.arスコア[j].譜面情報.フルコンボ[j];
								this.stバー情報[index].bドンダフルコンボ[j] = song.arスコア[j].譜面情報.ドンダフルコンボ[j];
								this.stバー情報[index].nスコアランク = song.arスコア[j].譜面情報.nスコアランク;
							}
						}

						for ( int f = 0; f < (int)Difficulty.Total; f++ )
                        {
                            if( song.arスコア[ f ] != null )
                                this.stバー情報[ index ].b分岐 = song.arスコア[ f ].譜面情報.b譜面分岐;
                        }

						// stバー情報[] の内容を1行ずつずらす。
						
						C曲リストノード song2 = this.r現在選択中の曲;
						for( int i = 0; i < 5; i++ )
							song2 = this.r前の曲( song2 );

						for( int i = 0; i < 13; i++ )
						{
							int n = ( ( ( this.n現在の選択行 - 5 ) + i ) + 13 ) % 13;
							this.stバー情報[ n ].eバー種別 = this.e曲のバー種別を返す( song2 );
							song2 = this.r次の曲( song2 );
                            this.stバー情報[ i ].ttkタイトル = this.ttk曲名テクスチャを生成する( this.stバー情報[ i ].strタイトル文字列, this.stバー情報[i].ForeColor, this.stバー情報[i].BackColor);
						}

		
						// 新しく最上部に表示されるパネル用のスキル値を取得。
						
						for( int i = 0; i < 3; i++ )
							this.stバー情報[ index ].nスキル値[ i ] = (int) song.arスコア[ this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す( song ) ].譜面情報.最大スキル[ i ];


						// 1行(100カウント)移動完了。

						this.n現在のスクロールカウンタ += 100;
						this.n目標のスクロールカウンタ += 100;

						this.t選択曲が変更された(false);				// スクロールバー用に今何番目を選択しているかを更新

					    this.ttk選択している曲の曲名 = null;
					    this.ttk選択している曲のサブタイトル = null;

						if (this.n目標のスクロールカウンタ == 0)
						{
							TJAPlayer3.stage選曲.t選択曲変更通知();      // スクロール完了＝選択曲変更！
							ctBarOpen.t開始(0, 161, 2, TJAPlayer3.Timer);
							this.ctBoxExplanationOpacity.t開始(0, 211, 2, TJAPlayer3.Timer);
						}
						//-----------------
						#endregion
					}

                    if(this.b選択曲が変更された && n現在のスクロールカウンタ == 0　&& n目標のスクロールカウンタ == 0)
                    {
                        if (this.ttk選択している曲の曲名 != null)
                        {
                            this.ttk選択している曲の曲名 = null;
                            this.b選択曲が変更された = false;
                        }
                        if (this.ttk選択している曲のサブタイトル != null)
                        {
                            this.ttk選択している曲のサブタイトル = null;
                            this.b選択曲が変更された = false;
                        }
                    }
					this.nスクロールタイマ += nアニメ間隔;
				}
				//-----------------
				#endregion
			}

			// 描画。

			if( this.r現在選択中の曲 == null )
			{
				#region [ 曲が１つもないなら「Songs were not found.」を表示してここで帰れ。]
				//-----------------
				if ( bIsEnumeratingSongs )
				{
					if ( this.txEnumeratingSongs != null )
					{
						this.txEnumeratingSongs.t2D描画( TJAPlayer3.app.Device, 320, 160 );
					}
				}
				else
				{
					if ( this.txSongNotFound != null )
						this.txSongNotFound.t2D描画( TJAPlayer3.app.Device, 320, 160 );
				}
				//-----------------
				#endregion

				return 0;
			}
            else
            {
				int i選曲バーX座標 = 673; //選曲バーの座標用
				int i選択曲バーX座標 = 665; //選択曲バーの座標用

				{
					#region [ (2) 通常フェーズの描画。]
					//-----------------
					for (int i = 0; i < 13; i++)    // パネルは全13枚。
					{
						if ((i == 0 && this.n現在のスクロールカウンタ > 0) ||       // 最上行は、上に移動中なら表示しない。
							(i == 12 && this.n現在のスクロールカウンタ < 0))        // 最下行は、下に移動中なら表示しない。
							continue;

						int nパネル番号 = (((this.n現在の選択行 - 5) + i) + 13) % 13;
						int n見た目の行番号 = i;
						int n次のパネル番号 = (this.n現在のスクロールカウンタ <= 0) ? ((i + 1) % 13) : (((i - 1) + 13) % 13);
						int x = i選曲バーX座標;
						int xAnime = this.ptバーの座標[n見た目の行番号].X + ((int)((this.ptバーの座標[n次のパネル番号].X - this.ptバーの座標[n見た目の行番号].X) * (((double)Math.Abs(this.n現在のスクロールカウンタ)) / 100.0)));
						int y = this.ptバーの座標[n見た目の行番号].Y + ((int)((this.ptバーの座標[n次のパネル番号].Y - this.ptバーの座標[n見た目の行番号].Y) * (((double)Math.Abs(this.n現在のスクロールカウンタ)) / 100.0)));

						{
							// (B) スクロール中の選択曲バー、またはその他のバーの描画。

							#region [ BoxOpen,BoxCloseのバーアニメーション ]

							float BoxX = 0;
							float BoxY = 0;
							if (this.ctBoxOpen.b進行中)
							{
								{
									if (i < 5)
									{
										if (i <= 2)
										{
											if (this.ctBoxOpen.n現在の値 >= 400)
											{
												BoxX = -(-this.ctBoxOpen.n現在の値 + 400) * (-this.ctBoxOpen.n現在の値 + 400) / 1000.0f;
												BoxY = -(-this.ctBoxOpen.n現在の値 + 400) * (-this.ctBoxOpen.n現在の値 + 400) / 200.0f;
											}
										}
										else if (i == 3)
										{
											if (this.ctBoxOpen.n現在の値 >= 500)
											{
												BoxX = -(-this.ctBoxOpen.n現在の値 + 500) * (-this.ctBoxOpen.n現在の値 + 500) / 1000.0f;
												BoxY = -(-this.ctBoxOpen.n現在の値 + 500) * (-this.ctBoxOpen.n現在の値 + 500) / 200.0f;
											}
										}
										else if (i == 4)
										{
											if (this.ctBoxOpen.n現在の値 >= 600)
											{
												BoxX = -(-this.ctBoxOpen.n現在の値 + 600) * (-this.ctBoxOpen.n現在の値 + 600) / 1000.0f;
												BoxY = -(-this.ctBoxOpen.n現在の値 + 600) * (-this.ctBoxOpen.n現在の値 + 600) / 200.0f;
											}
										}
									}
									if (i > 5)
									{
										if (i == 6)
										{
											if (this.ctBoxOpen.n現在の値 >= 600)
											{
												BoxX = (this.ctBoxOpen.n現在の値 - 600) * (this.ctBoxOpen.n現在の値 - 600) / 1000.0f;
												BoxY = (this.ctBoxOpen.n現在の値 - 600) * (this.ctBoxOpen.n現在の値 - 600) / 200.0f;
											}
										}
										else if (i == 7)
										{
											if (this.ctBoxOpen.n現在の値 >= 500)
											{
												BoxX = (this.ctBoxOpen.n現在の値 - 500) * (this.ctBoxOpen.n現在の値 - 500) / 1000.0f;
												BoxY = (this.ctBoxOpen.n現在の値 - 500) * (this.ctBoxOpen.n現在の値 - 500) / 200.0f;
											}
										}
										else if (i >= 8)
										{
											if (this.ctBoxOpen.n現在の値 >= 400)
											{
												BoxX = (this.ctBoxOpen.n現在の値 - 400) * (this.ctBoxOpen.n現在の値 - 400) / 1000.0f;
												BoxY = (this.ctBoxOpen.n現在の値 - 400) * (this.ctBoxOpen.n現在の値 - 400) / 200.0f;
											}
										}
									}
								}
							}
							else if (this.ctBoxClose.b進行中)
							{
								if (i < 5)
								{
									if (i <= 2)
									{
										if (this.ctBoxClose.n現在の値 <= 130)
										{
											BoxX = -90 + (float)Math.Sin((this.ctBoxClose.n現在の値 - 40) * (Math.PI / 180)) * 90;
											BoxY = -600 + (float)Math.Sin((this.ctBoxClose.n現在の値 - 40) * (Math.PI / 180)) * 600;
										}
									}
									if (i == 3)
									{
										if (this.ctBoxClose.n現在の値 <= 110)
										{
											BoxX = -90 + (float)Math.Sin((this.ctBoxClose.n現在の値 - 20) * (Math.PI / 180)) * 90;
											BoxY = -600 + (float)Math.Sin((this.ctBoxClose.n現在の値 - 20) * (Math.PI / 180)) * 600;
										}
									}
									if (i == 4)
									{
										if (this.ctBoxClose.n現在の値 <= 90)
										{
											BoxX = -90 + (float)Math.Sin(this.ctBoxClose.n現在の値 * (Math.PI / 180)) * 90;
											BoxY = -600 + (float)Math.Sin(this.ctBoxClose.n現在の値 * (Math.PI / 180)) * 600;
										}
									}
								}
								if (i > 5)
								{
									if (i >= 8)
									{
										if (this.ctBoxClose.n現在の値 <= 130)
										{
											BoxX = 90 - (float)Math.Sin((this.ctBoxClose.n現在の値 - 40) * (Math.PI / 180)) * 90;
											BoxY = 600 - (float)Math.Sin((this.ctBoxClose.n現在の値 - 40) * (Math.PI / 180)) * 600;
										}
									}
									if (i == 7)
									{
										if (this.ctBoxClose.n現在の値 <= 110)
										{
											BoxX = 90 - (float)Math.Sin((this.ctBoxClose.n現在の値 - 20) * (Math.PI / 180)) * 90;
											BoxY = 600 - (float)Math.Sin((this.ctBoxClose.n現在の値 - 20) * (Math.PI / 180)) * 600;
										}
									}
									if (i == 6)
									{
										if (this.ctBoxClose.n現在の値 <= 90)
										{
											BoxX = 90 - (float)Math.Sin(this.ctBoxClose.n現在の値 * (Math.PI / 180)) * 90;
											BoxY = 600 - (float)Math.Sin(this.ctBoxClose.n現在の値 * (Math.PI / 180)) * 600;
										}
									}
								}
							}
							else
							{
								BoxY = 0;
							}

							#endregion

							#region [ バーテクスチャを描画。]
							//-----------------
							if (n現在のスクロールカウンタ != 0)
								this.tジャンル別選択されていない曲バーの描画(xAnime + (int)BoxX, y + (int)BoxY, this.stバー情報[nパネル番号].strジャンル, this.stバー情報[nパネル番号].eバー種別, stバー情報[nパネル番号].bクリア, stバー情報[nパネル番号].bフルコンボ, stバー情報[nパネル番号].bドンダフルコンボ, stバー情報[nパネル番号].nスコアランク);
							else if (n見た目の行番号 != 5)
								this.tジャンル別選択されていない曲バーの描画(xAnime + (int)BoxX, y + (int)BoxY, this.stバー情報[nパネル番号].strジャンル, this.stバー情報[nパネル番号].eバー種別, stバー情報[nパネル番号].bクリア, stバー情報[nパネル番号].bフルコンボ, stバー情報[nパネル番号].bドンダフルコンボ, stバー情報[nパネル番号].nスコアランク);
							//-----------------
							#endregion

							#region [ タイトル名テクスチャを描画。]

							if (this.stバー情報[nパネル番号].eバー種別 != Eバー種別.BackBox)
							{
								if (this.stバー情報[nパネル番号].eバー種別 == Eバー種別.Score)
								{
									ResolveTitleTexture(this.stバー情報[nパネル番号].ttkタイトル).vc拡大縮小倍率.Y = 0.9f;
								}
								else
								{
									ResolveTitleTexture(this.stバー情報[nパネル番号].ttkタイトル).vc拡大縮小倍率.Y = 1.1f;
								}
							}

							if (TJAPlayer3.stage選曲.act難易度選択画面.bIsDifficltSelect)
							{
								if (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 >= 725)
								{
									if (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 <= 1190)
									{
										ResolveTitleTexture(this.stバー情報[nパネル番号].ttkタイトル).Opacity = 255.0f - (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 - 725);
									}
								}
							}
							else
							{
								ResolveTitleTexture(this.stバー情報[nパネル番号].ttkタイトル).Opacity = 255;
							}

							if (ResolveTitleTexture(this.stバー情報[nパネル番号].ttkタイトル).szテクスチャサイズ.Width > stバー情報[nパネル番号].ttkタイトル.maxWidht)
							{
								if (this.stバー情報[nパネル番号].eバー種別 == Eバー種別.Score)
									ResolveTitleTexture(this.stバー情報[nパネル番号].ttkタイトル).vc拡大縮小倍率.X = (float)(((double)stバー情報[nパネル番号].ttkタイトル.maxWidht) / ResolveTitleTexture(this.stバー情報[nパネル番号].ttkタイトル).szテクスチャサイズ.Width) - 0.1f;
								else if (this.stバー情報[nパネル番号].eバー種別 != Eバー種別.BackBox)
									ResolveTitleTexture(this.stバー情報[nパネル番号].ttkタイトル).vc拡大縮小倍率.X = (float)(((double)stバー情報[nパネル番号].ttkタイトル.maxWidht) / ResolveTitleTexture(this.stバー情報[nパネル番号].ttkタイトル).szテクスチャサイズ.Width) + 0.1f;
							}
							else
							{
								if (this.stバー情報[nパネル番号].eバー種別 == Eバー種別.Score)
									ResolveTitleTexture(this.stバー情報[nパネル番号].ttkタイトル).vc拡大縮小倍率.X = 0.9f;
								else if (this.stバー情報[nパネル番号].eバー種別 != Eバー種別.BackBox)
									ResolveTitleTexture(this.stバー情報[nパネル番号].ttkタイトル).vc拡大縮小倍率.X = 1.1f;
							}

							if (n現在のスクロールカウンタ != 0)
								ResolveTitleTexture(this.stバー情報[nパネル番号].ttkタイトル).t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, xAnime + 318 + (int)BoxX + TJAPlayer3.Skin.SongSelect_Title_X, y + 54 + (int)BoxY + TJAPlayer3.Skin.SongSelect_Title_Y);
							else if (n見た目の行番号 != 5)
								ResolveTitleTexture(this.stバー情報[nパネル番号].ttkタイトル).t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, xAnime + 318 + (int)BoxX + TJAPlayer3.Skin.SongSelect_Title_X, y + 54 + (int)BoxY + TJAPlayer3.Skin.SongSelect_Title_Y);


							#endregion

							//-----------------						
						}
						#endregion
					}

					if (this.n現在のスクロールカウンタ == 0)
					{
						#region [ Bar_Centerの表示 ]
						TJAPlayer3.Tx.SongSelect_Bar_Select.vc拡大縮小倍率.X = 1.0f;
						TJAPlayer3.Tx.SongSelect_Bar_Select.vc拡大縮小倍率.Y = 1.0f;
						if (ctBarOpen.n現在の値 > 100)
						{
							if (ctBoxOpen.n現在の値 <= 600)
							{
								if (ctBoxClose.n現在の値 >= 90)
								{
									if (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 <= 600)
									{
										TJAPlayer3.Tx.SongSelect_Bar_Select.Opacity = 255.0f;
										TJAPlayer3.Tx.SongSelect_Bar_Select.t2D描画(TJAPlayer3.app.Device, 313, ctBarOpen.n現在の値 <= 100 ? 294 : 294 - (ctBarOpen.n現在の値 - 100), new Rectangle(0, 0, 663, 66));//上
										TJAPlayer3.Tx.SongSelect_Bar_Select.t2D描画(TJAPlayer3.app.Device, 313, ctBarOpen.n現在の値 <= 100 ? 355 : 355 + (ctBarOpen.n現在の値 - 100), new Rectangle(0, 250 - 66, 663, 66));//上

										TJAPlayer3.Tx.SongSelect_Bar_Select.vc拡大縮小倍率.Y = ctBarOpen.n現在の値 <= 100 ? 1f : 1f + ((ctBarOpen.n現在の値 - 100) / 43f);
										TJAPlayer3.Tx.SongSelect_Bar_Select.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 644, 359, new Rectangle(0, 100, 663, 250 - 200));    //中心

										if (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.b進行中)
										{
											TJAPlayer3.Tx.SongSelect_Bar_Select.vc拡大縮小倍率.Y = 1.0f;

											if (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 <= 100)
												TJAPlayer3.Tx.SongSelect_Bar_Select.Opacity = TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 * 2.55f;
											else if (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 <= 200)
												TJAPlayer3.Tx.SongSelect_Bar_Select.Opacity = 255 - (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 - 100) * 2.55f;
											else if (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 <= 300)
												TJAPlayer3.Tx.SongSelect_Bar_Select.Opacity = (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 - 200) * 2.55f;
											else if (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 <= 400)
												TJAPlayer3.Tx.SongSelect_Bar_Select.Opacity = 255 - (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 - 300) * 2.55f;
											else if (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 <= 500)
												TJAPlayer3.Tx.SongSelect_Bar_Select.Opacity = (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 - 400) * 2.55f;
											else if (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 <= 600)
												TJAPlayer3.Tx.SongSelect_Bar_Select.Opacity = 255 - (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 - 500) * 2.55f;

											TJAPlayer3.Tx.SongSelect_Bar_Select.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 644, 359, new Rectangle(0, 251, 663, 251));//上
										}
										else if (ctBoxOpen.b進行中)
										{
											TJAPlayer3.Tx.SongSelect_Bar_Select.vc拡大縮小倍率.Y = 1.0f;

											if (ctBoxOpen.n現在の値 <= 100)
												TJAPlayer3.Tx.SongSelect_Bar_Select.Opacity = ctBoxOpen.n現在の値 * 2.55f;
											else if (ctBoxOpen.n現在の値 <= 200)
												TJAPlayer3.Tx.SongSelect_Bar_Select.Opacity = 255 - (ctBoxOpen.n現在の値 - 100) * 2.55f;
											else if (ctBoxOpen.n現在の値 <= 300)
												TJAPlayer3.Tx.SongSelect_Bar_Select.Opacity = (ctBoxOpen.n現在の値 - 200) * 2.55f;
											else if (ctBoxOpen.n現在の値 <= 400)
												TJAPlayer3.Tx.SongSelect_Bar_Select.Opacity = 255 - (ctBoxOpen.n現在の値 - 300) * 2.55f;
											else if (ctBoxOpen.n現在の値 <= 500)
												TJAPlayer3.Tx.SongSelect_Bar_Select.Opacity = (ctBoxOpen.n現在の値 - 400) * 2.55f;
											else if (ctBoxOpen.n現在の値 <= 600)
												TJAPlayer3.Tx.SongSelect_Bar_Select.Opacity = 255 - (ctBoxOpen.n現在の値 - 500) * 2.55f;

											TJAPlayer3.Tx.SongSelect_Bar_Select.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 644, 359, new Rectangle(0, 251, 663, 251));//上
										}
									}
								}
							}
						}

						if (TJAPlayer3.stage選曲.r現在選択中の曲.eノード種別 != C曲リストノード.Eノード種別.BACKBOX && TJAPlayer3.stage選曲.r現在選択中の曲.eノード種別 != C曲リストノード.Eノード種別.BOX)
						{

							TJAPlayer3.Tx.SongSelect_Bar_Genre[this.nStrジャンルtoNum(TJAPlayer3.stage選曲.r現在選択中の曲.strジャンル)].vc拡大縮小倍率.X = 1.0f;
							TJAPlayer3.Tx.SongSelect_Bar_Genre[this.nStrジャンルtoNum(TJAPlayer3.stage選曲.r現在選択中の曲.strジャンル)].vc拡大縮小倍率.Y = 1.0f;

							if (ctBarOpen.n現在の値 <= 100)
							{
								TJAPlayer3.Tx.SongSelect_Bar_Genre[this.nStrジャンルtoNum(TJAPlayer3.stage選曲.r現在選択中の曲.strジャンル)].t2D描画(TJAPlayer3.app.Device, 328, 315);

								TJAPlayer3.Tx.SongSelect_Crown.vc拡大縮小倍率.X = 0.8f;
								TJAPlayer3.Tx.SongSelect_Crown.vc拡大縮小倍率.Y = 0.8f;
								TJAPlayer3.Tx.SongSelect_ScoreRank.vc拡大縮小倍率.X = 0.8f;
								TJAPlayer3.Tx.SongSelect_ScoreRank.vc拡大縮小倍率.Y = 0.8f;
								if(TJAPlayer3.stage選曲.r現在選択中の曲.eノード種別 != C曲リストノード.Eノード種別.BACKBOX)
                                {
									for (int i = 0; i < 5; i++)
									{
										if (TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.クリア[i])
											TJAPlayer3.Tx.SongSelect_Crown.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 358, 342, new Rectangle((i * 3) * 43, 0, 43, 39));
										if (TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.フルコンボ[i])
											TJAPlayer3.Tx.SongSelect_Crown.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 358, 342, new Rectangle((i * 3 + 1) * 43, 0, 43, 39));
										if (TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.ドンダフルコンボ[i])
											TJAPlayer3.Tx.SongSelect_Crown.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 358, 342, new Rectangle((i * 3 + 2) * 43, 0, 43, 39));

										if (TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.nスコアランク[i] >= 1)
											TJAPlayer3.Tx.SongSelect_ScoreRank.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 358, 342 + 30, new RectangleF(0, 41f * (TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.nスコアランク[i] - 1), 48, 41f));
									}
								}
							}
							else
							{
								if (ctBoxOpen.b進行中)
								{
									if (ctBoxOpen.n現在の値 >= 575)
										if (ctBoxOpen.n現在の値 <= 800)
										{
											TJAPlayer3.Tx.SongSelect_Bar_Genre[this.nStrジャンルtoNum(TJAPlayer3.stage選曲.r現在選択中の曲.strジャンル)].vc拡大縮小倍率.X = (float)Math.Sin((((ctBoxOpen.n現在の値 - 575) / 2.5f + 90) * (Math.PI / 180))) * 1.0f;
										}
                                        else
                                        {
											TJAPlayer3.Tx.SongSelect_Bar_Genre[this.nStrジャンルtoNum(TJAPlayer3.stage選曲.r現在選択中の曲.strジャンル)].vc拡大縮小倍率.X = 0;
										}
								}
								else if (ctBoxClose.b進行中)
								{
									if (ctBoxClose.n現在の値 >= 0)
										if (ctBoxClose.n現在の値 <= 90)
											TJAPlayer3.Tx.SongSelect_Bar_Genre[this.nStrジャンルtoNum(TJAPlayer3.stage選曲.r現在選択中の曲.strジャンル)].vc拡大縮小倍率.X = (float)Math.Sin((ctBoxClose.n現在の値) * (Math.PI / 180)) * 1.0f;
								}
								else
									TJAPlayer3.Tx.SongSelect_Bar_Genre[this.nStrジャンルtoNum(TJAPlayer3.stage選曲.r現在選択中の曲.strジャンル)].vc拡大縮小倍率.X = 1.0f;

								TJAPlayer3.Tx.SongSelect_Bar_Genre[this.nStrジャンルtoNum(TJAPlayer3.stage選曲.r現在選択中の曲.strジャンル)].t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 643, ctBarOpen.n現在の値 <= 100 ? 322 : 322 - (ctBarOpen.n現在の値 - 100), new Rectangle(0, 0, 630, 26));   //上
								TJAPlayer3.Tx.SongSelect_Bar_Genre[this.nStrジャンルtoNum(TJAPlayer3.stage選曲.r現在選択中の曲.strジャンル)].t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 643, ctBarOpen.n現在の値 <= 100 ? 394 : 394 + (ctBarOpen.n現在の値 - 100), new Rectangle(0, 66, 630, 26));   //下

								TJAPlayer3.Tx.SongSelect_Bar_Genre[this.nStrジャンルtoNum(TJAPlayer3.stage選曲.r現在選択中の曲.strジャンル)].vc拡大縮小倍率.Y = ctBarOpen.n現在の値 <= 100 ? 1.41f : 1.41f + ((ctBarOpen.n現在の値 - 100) / 20f);
								TJAPlayer3.Tx.SongSelect_Bar_Genre[this.nStrジャンルtoNum(TJAPlayer3.stage選曲.r現在選択中の曲.strジャンル)].t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 643, 359, new Rectangle(0, 27, 630, 92 - 54));    //中心

								TJAPlayer3.Tx.SongSelect_Crown.vc拡大縮小倍率.X = 0.8f + ((ctBarOpen.n現在の値 - 100) / 222.0f);
								TJAPlayer3.Tx.SongSelect_Crown.vc拡大縮小倍率.Y = 0.8f + ((ctBarOpen.n現在の値 - 100) / 222.0f);
								TJAPlayer3.Tx.SongSelect_ScoreRank.vc拡大縮小倍率.X = 0.8f + ((ctBarOpen.n現在の値 - 100) / 222.0f);
								TJAPlayer3.Tx.SongSelect_ScoreRank.vc拡大縮小倍率.Y = 0.8f + ((ctBarOpen.n現在の値 - 100) / 222.0f);

								if (TJAPlayer3.stage選曲.r現在選択中の曲.eノード種別 != C曲リストノード.Eノード種別.BACKBOX)
								{
									for (int i = 0; i < 5; i++)
									{
										if (TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.クリア[i])
											TJAPlayer3.Tx.SongSelect_Crown.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 358, 342 - (ctBarOpen.n現在の値 - 100), new Rectangle((i * 3) * 43, 0, 43, 39));
										if (TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.フルコンボ[i])
											TJAPlayer3.Tx.SongSelect_Crown.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 358, 342 - (ctBarOpen.n現在の値 - 100), new Rectangle((i * 3 + 1) * 43, 0, 43, 39));
										if (TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.ドンダフルコンボ[i])
											TJAPlayer3.Tx.SongSelect_Crown.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 358, 342 - (ctBarOpen.n現在の値 - 100), new Rectangle((i * 3 + 2) * 43, 0, 43, 39));

										if (TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.nスコアランク[i] >= 1)
										{
											TJAPlayer3.Tx.SongSelect_ScoreRank.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 358, 372 - ((ctBarOpen.n現在の値 - 100) / 1.17f), new RectangleF(0, 41f * (TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.nスコアランク[i] - 1), 48, 41f));
										}
									}

								}

							}
						}

						if (TJAPlayer3.stage選曲.r現在選択中の曲.eノード種別 == C曲リストノード.Eノード種別.BOX)
						{
							TJAPlayer3.Tx.SongSelect_Bar_Box[this.nStrジャンルtoNum(TJAPlayer3.stage選曲.r現在選択中の曲.strジャンル)].vc拡大縮小倍率.X = 1.0f;
							TJAPlayer3.Tx.SongSelect_Bar_Box[this.nStrジャンルtoNum(TJAPlayer3.stage選曲.r現在選択中の曲.strジャンル)].vc拡大縮小倍率.Y = 1.0f;
							if (ctBarOpen.n現在の値 <= 100)
							{
								TJAPlayer3.Tx.SongSelect_Bar_Genre[this.nStrジャンルtoNum(TJAPlayer3.stage選曲.r現在選択中の曲.strジャンル)].t2D描画(TJAPlayer3.app.Device, 328, 315);
							}
							else
							{
								if (ctBoxOpen.n現在の値 >= 575)
								{
									if (ctBoxOpen.n現在の値 <= 800)
									{
										TJAPlayer3.Tx.SongSelect_Bar_Box[this.nStrジャンルtoNum(TJAPlayer3.stage選曲.r現在選択中の曲.strジャンル)].vc拡大縮小倍率.X = (float)Math.Sin((((ctBoxOpen.n現在の値 - 575) / 2.5f + 90) * (Math.PI / 180))) * 1.0f;
										TJAPlayer3.Tx.SongSelect_Bar_Box[8].vc拡大縮小倍率.X = (float)Math.Sin((((ctBoxOpen.n現在の値 - 575) / 2.5f + 90) * (Math.PI / 180))) * 1.0f;
									}
                                    else
									{
										TJAPlayer3.Tx.SongSelect_Bar_Box[this.nStrジャンルtoNum(TJAPlayer3.stage選曲.r現在選択中の曲.strジャンル)].vc拡大縮小倍率.X = 0;
										TJAPlayer3.Tx.SongSelect_Bar_Box[8].vc拡大縮小倍率.X = 0;
									}
								}
								if (ctBoxClose.b進行中)
								{
									if (ctBoxClose.n現在の値 >= 0)
									{
										if (ctBoxClose.n現在の値 <= 90)
										{
											TJAPlayer3.Tx.SongSelect_Bar_Box[this.nStrジャンルtoNum(TJAPlayer3.stage選曲.r現在選択中の曲.strジャンル)].vc拡大縮小倍率.X = (float)Math.Sin((ctBoxClose.n現在の値) * (Math.PI / 180)) * 1.0f;
											TJAPlayer3.Tx.SongSelect_Bar_Box[8].vc拡大縮小倍率.X = (float)Math.Sin((ctBoxClose.n現在の値) * (Math.PI / 180)) * 1.0f;
										}
                                        else
										{
											if (ctBoxOpen.n現在の値 == 0)
											{
												TJAPlayer3.Tx.SongSelect_Bar_Box[this.nStrジャンルtoNum(TJAPlayer3.stage選曲.r現在選択中の曲.strジャンル)].vc拡大縮小倍率.X = 1.0f;
												TJAPlayer3.Tx.SongSelect_Bar_Box[8].vc拡大縮小倍率.X = 1.0f;
											}
										}
									}
								}

								TJAPlayer3.Tx.SongSelect_Bar_Box[this.nStrジャンルtoNum(TJAPlayer3.stage選曲.r現在選択中の曲.strジャンル)].t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 643, ctBarOpen.n現在の値 <= 100 ? 322 : 322 - (ctBarOpen.n現在の値 - 100), new Rectangle(0, 0, 630, 26));   //上
								TJAPlayer3.Tx.SongSelect_Bar_Box[this.nStrジャンルtoNum(TJAPlayer3.stage選曲.r現在選択中の曲.strジャンル)].t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 643, ctBarOpen.n現在の値 <= 100 ? 394 : 394 + (ctBarOpen.n現在の値 - 100), new Rectangle(0, 66, 630, 26));   //下

								TJAPlayer3.Tx.SongSelect_Bar_Box[this.nStrジャンルtoNum(TJAPlayer3.stage選曲.r現在選択中の曲.strジャンル)].vc拡大縮小倍率.Y = ctBarOpen.n現在の値 <= 100 ? 1.41f : 1.41f + ((ctBarOpen.n現在の値 - 100) / 20f);
								TJAPlayer3.Tx.SongSelect_Bar_Box[this.nStrジャンルtoNum(TJAPlayer3.stage選曲.r現在選択中の曲.strジャンル)].t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 643, 359, new Rectangle(0, 27, 630, 92 - 54));    //中心
								TJAPlayer3.Tx.SongSelect_Bar_Box[8].vc拡大縮小倍率.Y = 1.0f;
								TJAPlayer3.Tx.SongSelect_Bar_Box[8].t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 643, 336, new Rectangle(0, 0, 630, 40));   //上
								TJAPlayer3.Tx.SongSelect_Bar_Box[8].t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 643, ctBarOpen.n現在の値 <= 100 ? 393 : 393 + (ctBarOpen.n現在の値 - 100), new Rectangle(0, 57, 630, 40));   //下

								TJAPlayer3.Tx.SongSelect_Bar_Box[8].vc拡大縮小倍率.Y = 1.0f + ((ctBarOpen.n現在の値 - 100f) / 17f);
								TJAPlayer3.Tx.SongSelect_Bar_Box[8].t2D拡大率考慮上中心基準描画(TJAPlayer3.app.Device, 643, 356, new Rectangle(0, 40, 630, 17));   //
							}
						}

						if (TJAPlayer3.stage選曲.r現在選択中の曲.eノード種別 == C曲リストノード.Eノード種別.BACKBOX)
						{
							if(TJAPlayer3.Tx.SongSelect_Bar_Genre_Back != null)
							{
								TJAPlayer3.Tx.SongSelect_Bar_Genre_Back.vc拡大縮小倍率.X = 1.0f;
								TJAPlayer3.Tx.SongSelect_Bar_Genre_Back.vc拡大縮小倍率.Y = 1.0f;
								if (ctBarOpen.n現在の値 <= 100)
								{
									TJAPlayer3.Tx.SongSelect_Bar_Genre_Back.t2D描画(TJAPlayer3.app.Device, 328, 317);
								}
								else
								{
									if (ctBoxOpen.b進行中)
									{
										if (ctBoxOpen.n現在の値 >= 575)
											if (ctBoxOpen.n現在の値 <= 800)
											{
												TJAPlayer3.Tx.SongSelect_Bar_Genre_Back.vc拡大縮小倍率.X = (float)Math.Sin((((ctBoxOpen.n現在の値 - 575) / 2.5f + 90) * (Math.PI / 180))) * 1.0f;
											}
                                            else
											{
												TJAPlayer3.Tx.SongSelect_Bar_Genre_Back.vc拡大縮小倍率.X = 0;
											}
									}
									else if (ctBoxClose.b進行中)
									{
										if (ctBoxClose.n現在の値 >= 0)
											if (ctBoxClose.n現在の値 <= 90)
												TJAPlayer3.Tx.SongSelect_Bar_Genre_Back.vc拡大縮小倍率.X = (float)Math.Sin((ctBoxClose.n現在の値) * (Math.PI / 180)) * 1.0f;
									}
									else
										TJAPlayer3.Tx.SongSelect_Bar_Genre_Back.vc拡大縮小倍率.X = 1.0f;

									TJAPlayer3.Tx.SongSelect_Bar_Genre_Back.Opacity = 255;
									TJAPlayer3.Tx.SongSelect_Bar_Genre_Back.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 643, ctBarOpen.n現在の値 <= 100 ? 322 : 322 - (ctBarOpen.n現在の値 - 100), new Rectangle(0, 0, 630, 26));   //上
									TJAPlayer3.Tx.SongSelect_Bar_Genre_Back.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 643, ctBarOpen.n現在の値 <= 100 ? 394 : 394 + (ctBarOpen.n現在の値 - 100), new Rectangle(0, 66, 630, 26));   //下
									TJAPlayer3.Tx.SongSelect_Bar_Genre_Back.vc拡大縮小倍率.Y = ctBarOpen.n現在の値 <= 100 ? 5f : 5f + ((ctBarOpen.n現在の値 - 100) / 3.68f);
									TJAPlayer3.Tx.SongSelect_Bar_Genre_Back.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 643, 359, new Rectangle(0, 62, 630, 8));    //中心

									if (ctBoxOpen.b進行中)
									{
										if (ctBoxOpen.n現在の値 >= 575)
											if (ctBoxOpen.n現在の値 <= 800)
											{
												TJAPlayer3.Tx.SongSelect_Bar_Genre_Back.Opacity = 270 - (ctBoxOpen.n現在の値 - 575);
											}
                                            else
											{
												TJAPlayer3.Tx.SongSelect_Bar_Genre_Back.Opacity = 0;
											}
									}
									else if (ctBoxClose.b進行中)
									{
										if (ctBoxClose.n現在の値 >= 0)
											if (ctBoxClose.n現在の値 <= 90)
												TJAPlayer3.Tx.SongSelect_Bar_Genre_Back.Opacity = ctBoxClose.n現在の値 * 2.8333f;
									}
									else
									{
										TJAPlayer3.Tx.SongSelect_Bar_Genre_Back.Opacity = 255.0f;
									}

									TJAPlayer3.Tx.SongSelect_Bar_Genre_Back.vc拡大縮小倍率.Y = 1.0f;
									TJAPlayer3.Tx.SongSelect_Bar_Genre_Back.vc拡大縮小倍率.X = 1.0f;
									TJAPlayer3.Tx.SongSelect_Bar_Genre_Back.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 643, 359, new Rectangle(271, 25, 87, 37));    //中心

								}
							}
						}

						#endregion

						switch (r現在選択中の曲.eノード種別)
						{
							case C曲リストノード.Eノード種別.SCORE:
								{
									if (TJAPlayer3.Tx.SongSelect_Frame_Score != null)
									{
										// 難易度がTower、Danではない
										if (TJAPlayer3.stage選曲.n現在選択中の曲の難易度 != (int)Difficulty.Tower && TJAPlayer3.stage選曲.n現在選択中の曲の難易度 != (int)Difficulty.Dan)
										{
											for (int i = 0; i < (int)Difficulty.Edit + 1; i++)
											{
												if (ctBarOpen.n現在の値 >= 100)
												{
													if (TJAPlayer3.stage選曲.act難易度選択画面.bIsDifficltSelect)
													{
														if (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 >= 725)
														{
															if (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 <= 1190)
															{
																TJAPlayer3.Tx.SongSelect_Frame_Score.Opacity = 255.0f - (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 - 725);
																if (TJAPlayer3.Tx.SongSelect_Level_Number != null)
																	TJAPlayer3.Tx.SongSelect_Level_Number.Opacity = 255.0f - (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 - 725);
																if (TJAPlayer3.Tx.SongSelect_Bar_Select != null)
																	TJAPlayer3.Tx.SongSelect_Bar_Select.Opacity = 255.0f - (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 - 725);
															}
														}
													}
													else
													{
														TJAPlayer3.Tx.SongSelect_Frame_Score.Opacity = (ctBarOpen.n現在の値 - 100) * 5.1f;
														if (TJAPlayer3.Tx.SongSelect_Level_Number != null)
															TJAPlayer3.Tx.SongSelect_Level_Number.Opacity = (ctBarOpen.n現在の値 - 100) * 5.1f;
														if (TJAPlayer3.Tx.SongSelect_Bar_Select != null)
															TJAPlayer3.Tx.SongSelect_Bar_Select.Opacity = 255;
													}
													if (TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.nレベル[i] >= 0)
													{
														// レベルが0以上
														TJAPlayer3.Tx.SongSelect_Frame_Score.color4 = new Color4(1f, 1f, 1f);
														if (i == 4 && TJAPlayer3.stage選曲.n現在選択中の曲の難易度 == 4)
														{
															// エディット
															TJAPlayer3.Tx.SongSelect_Frame_Score.t2D下中央基準描画(TJAPlayer3.app.Device, 494 + (3 * 122) - 31, TJAPlayer3.Skin.SongSelect_Overall_Y + 465, new Rectangle(122 * i, 0, 122, 360));
														}
														else if (i != 4)
														{
															TJAPlayer3.Tx.SongSelect_Frame_Score.t2D下中央基準描画(TJAPlayer3.app.Device, 494 + (i * 122) - 31, TJAPlayer3.Skin.SongSelect_Overall_Y + 465, new Rectangle(122 * i, 0, 122, 360));
														}
														if (i == 4 && TJAPlayer3.stage選曲.n現在選択中の曲の難易度 == 4)
														{
															t小文字表示(TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.nレベル[3] < 10 ? 497 + (3 * 122) - 5 : 492 + (3 * 122) - 5, TJAPlayer3.Skin.SongSelect_Overall_Y + 277, TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.nレベル[3].ToString());
														}
														else if (i != 4)
														{
															t小文字表示(TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.nレベル[i] < 10 ? 497 + (i * 122) - 5 : 492 + (i * 122) - 5, TJAPlayer3.Skin.SongSelect_Overall_Y + 277, TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.nレベル[i].ToString());
														}

													}
													else
													{
														// レベルが0未満 = 譜面がないとみなす
														TJAPlayer3.Tx.SongSelect_Frame_Score.color4 = new Color4(0.5f, 0.5f, 0.5f);
														if (i == 4 && TJAPlayer3.stage選曲.n現在選択中の曲の難易度 == 4)
														{
															// エディット
															TJAPlayer3.Tx.SongSelect_Frame_Score.t2D下中央基準描画(TJAPlayer3.app.Device, 494 + (3 * 122) - 31, TJAPlayer3.Skin.SongSelect_Overall_Y + 465, new Rectangle(122 * i, 0, 122, 360));
														}
														else if (i != 4)
														{
															TJAPlayer3.Tx.SongSelect_Frame_Score.t2D下中央基準描画(TJAPlayer3.app.Device, 494 + (i * 122) - 31, TJAPlayer3.Skin.SongSelect_Overall_Y + 465, new Rectangle(122 * i, 0, 122, 360));
														}
													}
												}
											}
										}
									}
								}
								break;

							case C曲リストノード.Eノード種別.BOX:
								if (TJAPlayer3.Tx.SongSelect_Frame_Box != null)
									TJAPlayer3.Tx.SongSelect_Frame_Box.t2D描画(TJAPlayer3.app.Device, 450, TJAPlayer3.Skin.SongSelect_Overall_Y);
								break;

							case C曲リストノード.Eノード種別.BACKBOX:
								if (TJAPlayer3.Tx.SongSelect_Frame_BackBox != null)
									TJAPlayer3.Tx.SongSelect_Frame_BackBox.t2D描画(TJAPlayer3.app.Device, 450, TJAPlayer3.Skin.SongSelect_Overall_Y);
								break;

							case C曲リストノード.Eノード種別.RANDOM:
								if (TJAPlayer3.Tx.SongSelect_Frame_Random != null)
									TJAPlayer3.Tx.SongSelect_Frame_Random.t2D描画(TJAPlayer3.app.Device, 450, TJAPlayer3.Skin.SongSelect_Overall_Y);
								break;
						}
						TJAPlayer3.Tx.SongSelect_Branch.Opacity = (ctBarOpen.n現在の値 - 100) * 5.1f;
						for (int i = 0; i < 4; i++) // Don't show the branch indicator for Ura, so the maximum value of "i" should be 4, instead of 5.
						{
						    if (TJAPlayer3.Tx.SongSelect_Branch != null && TJAPlayer3.stage選曲.r現在選択中のスコア.譜面情報.b譜面分岐[i])
						        TJAPlayer3.Tx.SongSelect_Branch.t2D中心基準描画(TJAPlayer3.app.Device, 447 + (i * 122) - 31, TJAPlayer3.Skin.SongSelect_Overall_Y + 273);
						}

					}

					for (int i = 0; i < 13; i++)    // パネルは全13枚。
					{
						if ((i == 0 && this.n現在のスクロールカウンタ > 0) ||       // 最上行は、上に移動中なら表示しない。
							(i == 12 && this.n現在のスクロールカウンタ < 0))        // 最下行は、下に移動中なら表示しない。
							continue;

						int nパネル番号 = (((this.n現在の選択行 - 5) + i) + 13) % 13;
						int n見た目の行番号 = i;
						int n次のパネル番号 = (this.n現在のスクロールカウンタ <= 0) ? ((i + 1) % 13) : (((i - 1) + 13) % 13);
						//int x = this.ptバーの座標[ n見た目の行番号 ].X + ( (int) ( ( this.ptバーの座標[ n次のパネル番号 ].X - this.ptバーの座標[ n見た目の行番号 ].X ) * ( ( (double) Math.Abs( this.n現在のスクロールカウンタ ) ) / 100.0 ) ) );
						int x = i選曲バーX座標;
						int xAnime = this.ptバーの座標[n見た目の行番号].X + ((int)((this.ptバーの座標[n次のパネル番号].X - this.ptバーの座標[n見た目の行番号].X) * (((double)Math.Abs(this.n現在のスクロールカウンタ)) / 100.0)));
						int y = this.ptバーの座標[n見た目の行番号].Y + ((int)((this.ptバーの座標[n次のパネル番号].Y - this.ptバーの座標[n見た目の行番号].Y) * (((double)Math.Abs(this.n現在のスクロールカウンタ)) / 100.0)));

						if ((i == 5) && (this.n現在のスクロールカウンタ == 0))
						{
							// (A) スクロールが停止しているときの選択曲バーの描画。

							#region [ タイトル名テクスチャを描画。]
							//-----------------
							if (this.stバー情報[nパネル番号].strタイトル文字列 != "" && this.ttk選択している曲の曲名 == null)
								this.ttk選択している曲の曲名 = this.ttk曲名テクスチャを生成する(this.stバー情報[nパネル番号].strタイトル文字列, TJAPlayer3.stage選曲.r現在選択中の曲.ForeColor, TJAPlayer3.stage選曲.r現在選択中の曲.BackColor);
							if (this.stバー情報[nパネル番号].strサブタイトル != "" && this.ttk選択している曲のサブタイトル == null)
								this.ttk選択している曲のサブタイトル = this.ttkサブタイトルテクスチャを生成する(this.stバー情報[nパネル番号].strサブタイトル, TJAPlayer3.stage選曲.r現在選択中の曲.ForeColor, TJAPlayer3.stage選曲.r現在選択中の曲.BackColor);

							//サブタイトルがあったら700

							if (this.ttk選択している曲の曲名 != null)
							{
								if (TJAPlayer3.stage選曲.r現在選択中の曲.eノード種別 != C曲リストノード.Eノード種別.BACKBOX)
								{
									if (TJAPlayer3.stage選曲.r現在選択中の曲.eノード種別 == C曲リストノード.Eノード種別.SCORE)
									{
										ResolveTitleTexture(this.ttk選択している曲の曲名).vc拡大縮小倍率.Y = 0.9f;
									}
									else
									{
										ResolveTitleTexture(this.ttk選択している曲の曲名).vc拡大縮小倍率.Y = 1.1f;
									}
								}
							}
							if (this.ttk選択している曲のサブタイトル != null)
							{
								var tx選択している曲のサブタイトル = ResolveTitleTexture(ttk選択している曲のサブタイトル);

								#region [ 透明度操作 ]
								if (this.ttk選択している曲の曲名 != null)
								{
									if (TJAPlayer3.stage選曲.r現在選択中の曲.eノード種別 != C曲リストノード.Eノード種別.BACKBOX)
									{
										if (ctBoxOpen.b進行中)
										{
											if (ctBoxOpen.n現在の値 >= 545)
											{
												if (ctBoxOpen.n現在の値 <= 630)
												{
													if (txBoxExplanation != null)
														for (int j = 0; j < 3; j++)
															if (txBoxExplanation[j] != null)
																this.txBoxExplanation[j].Opacity = 255 - ((ctBoxOpen.n現在の値 - 545) * 3);

													tx選択している曲のサブタイトル.Opacity = 255 - ((ctBoxOpen.n現在の値 - 645) * 3);
													ResolveTitleTexture(this.ttk選択している曲の曲名).Opacity = 255 - ((ctBoxOpen.n現在の値 - 545) * 3);
												}
											}
										}
										else if (ctBoxClose.b進行中)
										{
											if (ctBoxClose.n現在の値 >= 0)
											{
												if (ctBoxClose.n現在の値 <= 90)
												{
													if (txBoxExplanation != null)
														for (int j = 0; j < 3; j++)
															if (txBoxExplanation[j] != null)
																this.txBoxExplanation[j].Opacity = ctBoxClose.n現在の値 * 2.833333333f;
													tx選択している曲のサブタイトル.Opacity = ctBoxClose.n現在の値 * 2.833333333f;
													ResolveTitleTexture(this.ttk選択している曲の曲名).Opacity = ctBoxClose.n現在の値 * 2.8333f;
												}
											}
										}
										if (TJAPlayer3.stage選曲.act難易度選択画面.bIsDifficltSelect)
										{
											if (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 >= 625)
											{
												if (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 <= 1090)
												{
													tx選択している曲のサブタイトル.Opacity = 255.0f - (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 - 725);
													ResolveTitleTexture(this.ttk選択している曲の曲名).Opacity = 255.0f - (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 - 725);
												}
											}
										}
										else
										{
											tx選択している曲のサブタイトル.Opacity = (ctBarOpen.n現在の値 - 100) * 5.1f;
											ResolveTitleTexture(this.ttk選択している曲の曲名).Opacity = 255;
										}
									}
									#endregion

									tx選択している曲のサブタイトル.vc拡大縮小倍率.Y = 0.87f;

									if(ctBarOpen.n現在の値 >= 90)
										tx選択している曲のサブタイトル.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 647 + TJAPlayer3.Skin.SongSelect_Title_X, 416 - (ctBarOpen.n現在の値 <= 100 ? 0 : (ctBarOpen.n現在の値 - 100)) + TJAPlayer3.Skin.SongSelect_Title_Y - 8);

									if (ResolveTitleTexture(this.ttk選択している曲の曲名).szテクスチャサイズ.Width > ttk選択している曲の曲名.maxWidht)
									{
										if (TJAPlayer3.stage選曲.r現在選択中の曲.eノード種別 == C曲リストノード.Eノード種別.SCORE)
											ResolveTitleTexture(this.ttk選択している曲の曲名).vc拡大縮小倍率.X = (float)(((double)ttk選択している曲の曲名.maxWidht) / ResolveTitleTexture(this.ttk選択している曲の曲名).szテクスチャサイズ.Width) - 0.1f;
										else if (TJAPlayer3.stage選曲.r現在選択中の曲.eノード種別 != C曲リストノード.Eノード種別.BACKBOX)
											ResolveTitleTexture(this.ttk選択している曲の曲名).vc拡大縮小倍率.X = (float)(((double)ttk選択している曲の曲名.maxWidht) / ResolveTitleTexture(this.ttk選択している曲の曲名).szテクスチャサイズ.Width) + 0.1f;
									}
									else
									{
										if (TJAPlayer3.stage選曲.r現在選択中の曲.eノード種別 == C曲リストノード.Eノード種別.SCORE)
											ResolveTitleTexture(this.ttk選択している曲の曲名).vc拡大縮小倍率.X = 0.9f;
										else if (TJAPlayer3.stage選曲.r現在選択中の曲.eノード種別 != C曲リストノード.Eノード種別.BACKBOX)
											ResolveTitleTexture(this.ttk選択している曲の曲名).vc拡大縮小倍率.X = 1.1f;
									}
									if (ResolveTitleTexture(this.ttk選択している曲のサブタイトル).szテクスチャサイズ.Width > ttk選択している曲のサブタイトル.maxWidht)
									{
										if (TJAPlayer3.stage選曲.r現在選択中の曲.eノード種別 == C曲リストノード.Eノード種別.SCORE)
											ResolveTitleTexture(this.ttk選択している曲のサブタイトル).vc拡大縮小倍率.X = (float)(((double)ttk選択している曲のサブタイトル.maxWidht) / ResolveTitleTexture(this.ttk選択している曲のサブタイトル).szテクスチャサイズ.Width) - 0.1f;
										else if (TJAPlayer3.stage選曲.r現在選択中の曲.eノード種別 != C曲リストノード.Eノード種別.BACKBOX)
											ResolveTitleTexture(this.ttk選択している曲のサブタイトル).vc拡大縮小倍率.X = (float)(((double)ttk選択している曲のサブタイトル.maxWidht) / ResolveTitleTexture(this.ttk選択している曲のサブタイトル).szテクスチャサイズ.Width) + 0.1f;
									}
									else
									{
										if (TJAPlayer3.stage選曲.r現在選択中の曲.eノード種別 == C曲リストノード.Eノード種別.SCORE)
											ResolveTitleTexture(this.ttk選択している曲のサブタイトル).vc拡大縮小倍率.X = 0.9f;
										else if (TJAPlayer3.stage選曲.r現在選択中の曲.eノード種別 != C曲リストノード.Eノード種別.BACKBOX)
											ResolveTitleTexture(this.ttk選択している曲のサブタイトル).vc拡大縮小倍率.X = 1.1f;
									}
									ResolveTitleTexture(this.ttk選択している曲の曲名).t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 647 + TJAPlayer3.Skin.SongSelect_Title_X, 376 - (ctBarOpen.n現在の値 <= 97 ? 0 : ((ctBarOpen.n現在の値 - 97))) + TJAPlayer3.Skin.SongSelect_Title_Y - 8);
								}
							}
							else
							{
								if (this.ttk選択している曲の曲名 != null)
								{
									#region [ 透明度操作 ]
									if (TJAPlayer3.stage選曲.r現在選択中の曲.eノード種別 != C曲リストノード.Eノード種別.BACKBOX)
									{
										if (ctBoxOpen.b進行中)
										{
											if (ctBoxOpen.n現在の値 >= 545)
											{
												if (ctBoxOpen.n現在の値 <= 630)
												{
													if (txBoxExplanation != null)
														for (int j = 0; j < 3; j++)
															if (txBoxExplanation[j] != null)
																this.txBoxExplanation[j].Opacity = 255 - ((ctBoxOpen.n現在の値 - 545) * 3);

													ResolveTitleTexture(this.ttk選択している曲の曲名).Opacity = 255 - ((ctBoxOpen.n現在の値 - 545) * 3);
												}
                                                else
												{
													if (txBoxExplanation != null)
														for (int j = 0; j < 3; j++)
															if (txBoxExplanation[j] != null)
																this.txBoxExplanation[j].Opacity = 0;

													ResolveTitleTexture(this.ttk選択している曲の曲名).Opacity = 0;
												}
											}
										}
										else if (ctBoxClose.b進行中)
										{
											if (ctBoxClose.n現在の値 >= 0)
												if (ctBoxClose.n現在の値 <= 90)
												{
													if (txBoxExplanation != null)
														for (int j = 0; j < 3; j++)
															if (txBoxExplanation[j] != null)
																this.txBoxExplanation[j].Opacity = ctBoxClose.n現在の値 * 2.833333333f;
													ResolveTitleTexture(this.ttk選択している曲の曲名).Opacity = ctBoxClose.n現在の値 * 2.8333f;
												}
										}
										else
										{
											if (TJAPlayer3.stage選曲.act難易度選択画面.bIsDifficltSelect)
												if (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 >= 725)
													if (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 <= 1190)
														ResolveTitleTexture(this.ttk選択している曲の曲名).Opacity = 255.0f - (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 - 725);
										}
									}

									#endregion

									if (ResolveTitleTexture(this.ttk選択している曲の曲名).szテクスチャサイズ.Width > ttk選択している曲の曲名.maxWidht)
									{
										if (TJAPlayer3.stage選曲.r現在選択中の曲.eノード種別 == C曲リストノード.Eノード種別.SCORE)
											ResolveTitleTexture(this.ttk選択している曲の曲名).vc拡大縮小倍率.X = (float)(((double)ttk選択している曲の曲名.maxWidht) / ResolveTitleTexture(this.ttk選択している曲の曲名).szテクスチャサイズ.Width) - 0.1f;
										else if (TJAPlayer3.stage選曲.r現在選択中の曲.eノード種別 != C曲リストノード.Eノード種別.BACKBOX)
											ResolveTitleTexture(this.ttk選択している曲の曲名).vc拡大縮小倍率.X = (float)(((double)ttk選択している曲の曲名.maxWidht) / ResolveTitleTexture(this.ttk選択している曲の曲名).szテクスチャサイズ.Width) + 0.1f;
									}
									else
									{
										if (TJAPlayer3.stage選曲.r現在選択中の曲.eノード種別 == C曲リストノード.Eノード種別.SCORE)
											ResolveTitleTexture(this.ttk選択している曲の曲名).vc拡大縮小倍率.X = 0.9f;
										else if (TJAPlayer3.stage選曲.r現在選択中の曲.eノード種別 != C曲リストノード.Eノード種別.BACKBOX)
											ResolveTitleTexture(this.ttk選択している曲の曲名).vc拡大縮小倍率.X = 1.1f;
									}

									ResolveTitleTexture(this.ttk選択している曲の曲名).t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 647 + TJAPlayer3.Skin.SongSelect_Title_X, 376 - (ctBarOpen.n現在の値 <= 97 ? 0 : ((ctBarOpen.n現在の値 - 97))) + TJAPlayer3.Skin.SongSelect_Title_Y - 8);
								}
							}
							if (TJAPlayer3.stage選曲.r現在選択中の曲.eノード種別 == C曲リストノード.Eノード種別.BOX)
							{
								if (txBoxExplanation != null)
								{
									for (int j = 0; j < 3; j++)
									{
										if (txBoxExplanation[j] != null)
										{
											if (ctBoxExplanationOpacity.n現在の値 >= 161)
											{
												if (!ctBoxOpen.b進行中 && !ctBoxClose.b進行中)
													this.txBoxExplanation[j].Opacity = (ctBoxExplanationOpacity.n現在の値 - 161) * 5.1f;
												this.txBoxExplanation[j].t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, 643 + TJAPlayer3.Skin.SongSelect_BoxExplanation_X, 360 + j * 35 + TJAPlayer3.Skin.SongSelect_BoxExplanation_Y);
											}
											else
											{
												if (!ctBoxOpen.b進行中 && !ctBoxClose.b進行中)
													this.txBoxExplanation[j].Opacity = 0;
											}
										}
									}
								}
							}
							//-----------------
							#endregion
						}

					}
					//-----------------
				}
			}

			return 0;
		}
		

		// その他

		#region [ private ]
		//-----------------
		private enum Eバー種別 { Score, Box, Other, BackBox }

		private struct STバー
		{
			public CTexture Score;
			public CTexture Box;
			public CTexture Other;
			public CTexture this[ int index ]
			{
				get
				{
					switch( index )
					{
						case 0:
							return this.Score;

						case 1:
							return this.Box;

						case 2:
							return this.Other;
					}
					throw new IndexOutOfRangeException();
				}
				set
				{
					switch( index )
					{
						case 0:
							this.Score = value;
							return;

						case 1:
							this.Box = value;
							return;

						case 2:
							this.Other = value;
							return;
					}
					throw new IndexOutOfRangeException();
				}
			}
		}

		private struct STバー情報
		{
			public CActSelect曲リスト.Eバー種別 eバー種別;
			public string strタイトル文字列;
			public CTexture txタイトル名;
			public STDGBVALUE<int> nスキル値;
			public Color col文字色;
            public Color ForeColor;
            public Color BackColor;
            public int[] ar難易度;
            public bool[] b分岐;
            public string strジャンル;
            public string strサブタイトル;
            public TitleTextureKey ttkタイトル;
			public bool[] bドンダフルコンボ;
			public bool[] bフルコンボ;
			public bool[] bクリア;
			public int[] nスコアランク;
		}

		private struct ST選曲バー
		{
			public CTexture Score;
			public CTexture Box;            
			public CTexture Other;
            public CTexture this[ int index ]
			{
				get
				{
					switch( index )
					{
						case 0:
							return this.Score;

						case 1:
							return this.Box;

						case 2:
							return this.Other;
					}
					throw new IndexOutOfRangeException();
				}
				set
				{
					switch( index )
					{
						case 0:
							this.Score = value;
							return;

						case 1:
							this.Box = value;
							return;

						case 2:
							this.Other = value;
							return;
					}
					throw new IndexOutOfRangeException();
				}
			}
		}

		

		private string OldBoxExplanetion;

		private CPrivateFont pfBoxExplanation;
		private CTexture[] txBoxExplanation = new CTexture[3];

		public CCounter ctBoxOpen;
		public CCounter ctBoxClose;

		public bool bBoxOpen;
		public bool bBoxClose;
		public bool bBoxOpenAnime;

		public CCounter ctBarOpen;
		private CCounter ctBoxExplanationOpacity;

        public bool b選択曲が変更された = true;
		private bool b登場アニメ全部完了;
		private Color color文字影 = Color.FromArgb( 0x40, 10, 10, 10 );
        private CCounter ct三角矢印アニメ;
        private CCounter counter;
        private EFIFOモード mode;
        private CPrivateFastFont pfMusicName;
        private CPrivateFastFont pfSubtitle;

	    // 2018-09-17 twopointzero: I can scroll through 2300 songs consuming approx. 200MB of memory.
	    //                          I have set the title texture cache size to a nearby round number (2500.)
        //                          If we'd like title textures to take up no more than 100MB, for example,
        //                          then a cache size of 1000 would be roughly correct.
	    private readonly LurchTable<TitleTextureKey, CTexture> _titleTextures =
	        new LurchTable<TitleTextureKey, CTexture>(LurchTableOrder.Access, 2500);

		private E楽器パート e楽器パート;
		private Font ft曲リスト用フォント;
		private long nスクロールタイマ;
		public float n現在のスクロールカウンタ;
		public float n目標のスクロールカウンタ;
		private int n現在の選択行;
		private Point[] ptバーの座標 = new Point[]
        { new Point( 215, -123 ), new Point( 215, -123 ), new Point( 241, -36  ), new Point( 268, 53 ), new Point( 295, 143 ), 
			new Point( 328, 312 ),
			new Point( 369, 481 ), new Point( 392, 571 ), new Point( 415, 661  ), new Point( 428, 751 ), new Point( 428, 776 ), new Point( 428, 776 ), new Point( 428, 776 ) };

		private STバー情報[] stバー情報 = new STバー情報[ 13 ];
		private CTexture txSongNotFound, txEnumeratingSongs;

        private TitleTextureKey ttk選択している曲の曲名;
        private TitleTextureKey ttk選択している曲のサブタイトル;

        private CTexture[] tx曲バー_難易度 = new CTexture[ 5 ];

        private long n矢印スクロール用タイマ値;

		private int nCurrentPosition = 0;
		private int nNumOfItems = 0;

		private Eバー種別 e曲のバー種別を返す( C曲リストノード song )
		{
			if( song != null )
			{
				switch( song.eノード種別 )
				{
					case C曲リストノード.Eノード種別.SCORE:
					case C曲リストノード.Eノード種別.SCORE_MIDI:
						return Eバー種別.Score;

					case C曲リストノード.Eノード種別.BOX:
						return Eバー種別.Box;

					case C曲リストノード.Eノード種別.BACKBOX:
						return Eバー種別.BackBox;
				}
			}
			return Eバー種別.Other;
		}
		private C曲リストノード r次の曲( C曲リストノード song )
		{
			if( song == null )
				return null;

			List<C曲リストノード> list = TJAPlayer3.Songs管理.list曲ルート;

			int index = list.IndexOf( song );

			if( index < 0 )
				return null;

			if( index == ( list.Count - 1 ) )
				return list[ 0 ];

			return list[ index + 1 ];
		}
		private C曲リストノード r前の曲( C曲リストノード song )
		{
			if( song == null )
				return null;

			List<C曲リストノード> list = TJAPlayer3.Songs管理.list曲ルート;

			int index = list.IndexOf( song );
	
			if( index < 0 )
				return null;

			if( index == 0 )
				return list[ list.Count - 1 ];

			return list[ index - 1 ];
		}

		private void tバーの初期化()
		{
			C曲リストノード song = this.r現在選択中の曲;

			if (song == null)
				return;

			for ( int i = 0; i < 5; i++ )
				song = this.r前の曲( song );

			if (song == null)
			{
				if (TJAPlayer3.Songs管理.list曲ルート[0] != null)
				{
					this.r現在選択中の曲 = TJAPlayer3.Songs管理.list曲ルート[0];
					this.t現在選択中の曲を元に曲バーを再構成する();
					this.t選択曲が変更された(false);
					this.b選択曲が変更された = true;
					TJAPlayer3.stage選曲.t選択曲変更通知();
				}
				return;
			}

			for ( int i = 0; i < 13; i++ )
			{
				this.stバー情報[ i ].strタイトル文字列 = song.strタイトル;
                this.stバー情報[ i ].strジャンル = song.strジャンル;
				this.stバー情報[ i ].col文字色 = song.col文字色;
                this.stバー情報[i].ForeColor = song.ForeColor;
                this.stバー情報[i].BackColor = song.BackColor;
				this.stバー情報[ i ].eバー種別 = this.e曲のバー種別を返す( song );
                this.stバー情報[ i ].strサブタイトル = song.strサブタイトル;
                this.stバー情報[ i ].ar難易度 = song.nLevel;

				this.stバー情報[i].bクリア = new bool[] { false, false, false, false, false };
				this.stバー情報[i].bフルコンボ = new bool[] { false, false, false, false, false };
				this.stバー情報[i].bドンダフルコンボ = new bool[] { false, false, false, false, false };
				this.stバー情報[i].nスコアランク = new int[] { 0, 0, 0, 0, 0 };

				for (int j = 0; j < 5; j++)
				{
					if (song.arスコア[j] != null)
					{
						this.stバー情報[i].bクリア[j] = song.arスコア[j].譜面情報.クリア[j];
						this.stバー情報[i].bフルコンボ[j] = song.arスコア[j].譜面情報.フルコンボ[j];
						this.stバー情報[i].bドンダフルコンボ[j] = song.arスコア[j].譜面情報.ドンダフルコンボ[j];
						this.stバー情報[i].nスコアランク[j] = song.arスコア[j].譜面情報.nスコアランク[j];
					}
                }

			    for( int f = 0; f < (int)Difficulty.Total; f++ )
                {
                    if( song.arスコア[ f ] != null )
                        this.stバー情報[ i ].b分岐 = song.arスコア[ f ].譜面情報.b譜面分岐;
                }
				
				for( int j = 0; j < 3; j++ )
					this.stバー情報[ i ].nスキル値[ j ] = (int) song.arスコア[ this.n現在のアンカ難易度レベルに最も近い難易度レベルを返す( song ) ].譜面情報.最大スキル[ j ];

                this.stバー情報[ i ].ttkタイトル = this.ttk曲名テクスチャを生成する( this.stバー情報[ i ].strタイトル文字列, this.stバー情報[i].ForeColor, this.stバー情報[i].BackColor);

				song = this.r次の曲( song );
			}

			this.n現在の選択行 = 5;
		}
		private void tジャンル別選択されていない曲バーの描画( int x, int y, string strジャンル, Eバー種別 eバー種別, bool[] Clear, bool[] FullCombo, bool[] DondaFullCombo, int[] nScoreRank)
		{
			if( x >= SampleFramework.GameWindowSize.Width || y >= SampleFramework.GameWindowSize.Height )
				return;

			for (int i = 0; i < TJAPlayer3.Tx.SongSelect_Bar_Genre.Length - 1; i++)
			{
				TJAPlayer3.Tx.SongSelect_Bar_Genre[i].vc拡大縮小倍率.X = 1;
				TJAPlayer3.Tx.SongSelect_Bar_Genre[i].vc拡大縮小倍率.Y = 1;
			}

			TJAPlayer3.Tx.SongSelect_Bar_Genre_Back.vc拡大縮小倍率.X = 1.0f;
			TJAPlayer3.Tx.SongSelect_Bar_Genre_Back.vc拡大縮小倍率.Y = 1.0f;

			for(int i = 0; i < TJAPlayer3.Tx.SongSelect_Bar_Genre.Length - 1; i++)
			{
                if (TJAPlayer3.stage選曲.act難易度選択画面.bIsDifficltSelect)
                {
					if(TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 >= 725)
                    {
						if (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 <= 1190)
						{
							TJAPlayer3.Tx.SongSelect_Bar_Genre_Back.Opacity = 255.0f - (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 - 725) ;
							TJAPlayer3.Tx.SongSelect_Bar_Genre[i].Opacity = 255.0f - (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 - 725);
							TJAPlayer3.Tx.SongSelect_Crown.Opacity = 255.0f - (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 - 725);
							TJAPlayer3.Tx.SongSelect_ScoreRank.Opacity = 255.0f - (TJAPlayer3.stage選曲.ctDiffSelect移動待ち.n現在の値 - 725);
						}
					}
                }
                else
                {
					TJAPlayer3.Tx.SongSelect_Bar_Genre_Back.Opacity = 255.0f;
					TJAPlayer3.Tx.SongSelect_Bar_Genre[i].Opacity = 255.0f;
					TJAPlayer3.Tx.SongSelect_Crown.Opacity = 255.0f;
					TJAPlayer3.Tx.SongSelect_ScoreRank.Opacity = 255.0f;
				}
			}


			switch ( strジャンル )
            {
                case "ポップス":
				    #region [ J-POP ]
    				//-----------------
	    			if( TJAPlayer3.Tx.SongSelect_Bar_Genre[1] != null )
                        TJAPlayer3.Tx.SongSelect_Bar_Genre[1].t2D描画(TJAPlayer3.app.Device, x, y );
	    			//-----------------
		    		#endregion
                    break;
                case "アニメ":
				    #region [ アニメ ]
    				//-----------------
	    			if(TJAPlayer3.Tx.SongSelect_Bar_Genre[2] != null )
                        TJAPlayer3.Tx.SongSelect_Bar_Genre[2].t2D描画( TJAPlayer3.app.Device, x, y );
	    			//-----------------
		    		#endregion
                    break;
                case "ゲームミュージック":
				    #region [ ゲーム ]
    				//-----------------
	    			if(TJAPlayer3.Tx.SongSelect_Bar_Genre[3] != null )
                        TJAPlayer3.Tx.SongSelect_Bar_Genre[3].t2D描画( TJAPlayer3.app.Device, x, y );
	    			//-----------------
		    		#endregion
                    break;
                case "ナムコオリジナル":
				    #region [ ナムコオリジナル ]
    				//-----------------
	    			if(TJAPlayer3.Tx.SongSelect_Bar_Genre[4] != null )
                        TJAPlayer3.Tx.SongSelect_Bar_Genre[4].t2D描画( TJAPlayer3.app.Device, x, y );
	    			//-----------------
		    		#endregion
                    break;
                case "クラシック":
				    #region [ クラシック ]
    				//-----------------
	    			if(TJAPlayer3.Tx.SongSelect_Bar_Genre[5] != null )
                        TJAPlayer3.Tx.SongSelect_Bar_Genre[5].t2D描画( TJAPlayer3.app.Device, x, y );
	    			//-----------------
		    		#endregion
                    break;
				case "キッズ":
					#region [ クラシック ]
					//-----------------
					if (TJAPlayer3.Tx.SongSelect_Bar_Genre[6] != null)
						TJAPlayer3.Tx.SongSelect_Bar_Genre[6].t2D描画(TJAPlayer3.app.Device, x, y);
					//-----------------
					#endregion
					break;
				case "ボーカロイド":
                case "VOCALOID":
				    #region [ ボカロ ]
    				//-----------------
	    			if(TJAPlayer3.Tx.SongSelect_Bar_Genre[7] != null )
                        TJAPlayer3.Tx.SongSelect_Bar_Genre[7].t2D描画( TJAPlayer3.app.Device, x, y );
	    			//-----------------
		    		#endregion
                    break;
                case "難易度ソート":
				    #region [ 難易度ソート ]
    				//-----------------
	    			if( this.tx曲バー_難易度[ this.n現在選択中の曲の現在の難易度レベル ] != null )
		    			this.tx曲バー_難易度[ this.n現在選択中の曲の現在の難易度レベル ].t2D描画( TJAPlayer3.app.Device, x, y );
	    			//-----------------
		    		#endregion
                    break;
                default:
                    #region [ その他の場合 ]
                    //-----------------
                    switch (eバー種別)
                    {
						case Eバー種別.BackBox:
							{
								if (TJAPlayer3.Tx.SongSelect_Bar_Genre_Back != null)
									TJAPlayer3.Tx.SongSelect_Bar_Genre_Back.t2D描画(TJAPlayer3.app.Device, x, y);
							}
							break;

						default:
							{
								if (TJAPlayer3.Tx.SongSelect_Bar_Genre[0] != null)
									TJAPlayer3.Tx.SongSelect_Bar_Genre[0].t2D描画(TJAPlayer3.app.Device, x, y);
							}
							break;
                    }
	    			//-----------------
		    		#endregion
                    break;
			}
			//SongSelect_Crown
			TJAPlayer3.Tx.SongSelect_Crown.vc拡大縮小倍率.X = 0.8f;
			TJAPlayer3.Tx.SongSelect_Crown.vc拡大縮小倍率.Y = 0.8f;
			TJAPlayer3.Tx.SongSelect_ScoreRank.vc拡大縮小倍率.X = 0.8f;
			TJAPlayer3.Tx.SongSelect_ScoreRank.vc拡大縮小倍率.Y = 0.8f;

			if (eバー種別 != Eバー種別.BackBox && eバー種別 != Eバー種別.Box)
			{
				for (int i = 0; i < 5; i++)
				{
					if (Clear[i])
						TJAPlayer3.Tx.SongSelect_Crown.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, x + 30, y + 30, new Rectangle((i * 3) * 43, 0, 43, 39));
					if (FullCombo[i])
						TJAPlayer3.Tx.SongSelect_Crown.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, x + 30, y + 30, new Rectangle((i * 3 + 1) * 43, 0, 43, 39));
					if (DondaFullCombo[i])
						TJAPlayer3.Tx.SongSelect_Crown.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, x + 30, y + 30, new Rectangle((i * 3 + 2) * 43, 0, 43, 39));

					if (nScoreRank[i] >= 1)
						TJAPlayer3.Tx.SongSelect_ScoreRank.t2D拡大率考慮中央基準描画(TJAPlayer3.app.Device, x + 30, y + 60, new RectangleF(0, 41f * (nScoreRank[i] - 1), 48, 41f));

				}
			}
		}
        public int nStrジャンルtoNum( string strジャンル )
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

        public TitleTextureKey ttk曲名テクスチャを生成する( string str文字, Color forecolor, Color backcolor)
        {
            return new TitleTextureKey(str文字, pfMusicName, forecolor, backcolor, 700);
        }

		public TitleTextureKey ttkサブタイトルテクスチャを生成する( string str文字, Color forecolor, Color backcolor)
        {
            return new TitleTextureKey(str文字, pfSubtitle, forecolor, backcolor, 700);
        }

		public CTexture ResolveTitleTexture(TitleTextureKey titleTextureKey)
	    {
	        if (!_titleTextures.TryGetValue(titleTextureKey, out var texture))
	        {
	            texture = GenerateTitleTexture(titleTextureKey);
                _titleTextures.Add(titleTextureKey, texture);
	        }

	        return texture;
	    }

		public static CTexture GenerateTitleTexture(TitleTextureKey titleTextureKey)
	    {
	        using (var bmp = new Bitmap(titleTextureKey.cPrivateFastFont.DrawPrivateFont(
	            titleTextureKey.str文字, titleTextureKey.forecolor, titleTextureKey.backcolor)))
	        {
	            CTexture tx文字テクスチャ = TJAPlayer3.tテクスチャの生成(bmp, false);
	            return tx文字テクスチャ;
	        }
	    }

		public static void OnTitleTexturesOnItemUpdated(
	        KeyValuePair<TitleTextureKey, CTexture> previous, KeyValuePair<TitleTextureKey, CTexture> next)
	    {
            previous.Value.Dispose();
	    }

		public static void OnTitleTexturesOnItemRemoved(
	        KeyValuePair<TitleTextureKey, CTexture> kvp)
	    {
	        kvp.Value.Dispose();
	    }

		public void ClearTitleTextureCache()
	    {
	        foreach (var titleTexture in _titleTextures.Values)
	        {
	            titleTexture.Dispose();
	        }

            _titleTextures.Clear();
	    }

		public sealed class TitleTextureKey
	    {
	        public readonly string str文字;
	        public readonly CPrivateFont cPrivateFastFont;
	        public readonly Color forecolor;
	        public readonly Color backcolor;
	        public readonly int maxWidht;

	        public TitleTextureKey(string str文字, CPrivateFont cPrivateFastFont, Color forecolor, Color backcolor, int maxWidht)
	        {
	            this.str文字 = str文字;
	            this.cPrivateFastFont = cPrivateFastFont;
	            this.forecolor = forecolor;
	            this.backcolor = backcolor;
	            this.maxWidht = maxWidht;
	        }

	        private bool Equals(TitleTextureKey other)
	        {
	            return string.Equals(str文字, other.str文字) &&
	                   cPrivateFastFont.Equals(other.cPrivateFastFont) &&
	                   forecolor.Equals(other.forecolor) &&
	                   backcolor.Equals(other.backcolor) &&
	                   maxWidht == other.maxWidht;
	        }

	        public override bool Equals(object obj)
	        {
	            if (ReferenceEquals(null, obj)) return false;
	            if (ReferenceEquals(this, obj)) return true;
	            return obj is TitleTextureKey other && Equals(other);
	        }

	        public override int GetHashCode()
	        {
	            unchecked
	            {
	                var hashCode = str文字.GetHashCode();
	                hashCode = (hashCode * 397) ^ cPrivateFastFont.GetHashCode();
	                hashCode = (hashCode * 397) ^ forecolor.GetHashCode();
	                hashCode = (hashCode * 397) ^ backcolor.GetHashCode();
	                hashCode = (hashCode * 397) ^ maxWidht;
	                return hashCode;
	            }
	        }

	        public static bool operator ==(TitleTextureKey left, TitleTextureKey right)
	        {
	            return Equals(left, right);
	        }

	        public static bool operator !=(TitleTextureKey left, TitleTextureKey right)
	        {
	            return !Equals(left, right);
	        }
	    }

		private void tアイテム数の描画()
		{
			string s = nCurrentPosition.ToString() + "/" + nNumOfItems.ToString();
			int x = 639 - 8 - 12;
			int y = 362;

			for ( int p = s.Length - 1; p >= 0; p-- )
			{
				tアイテム数の描画_１桁描画( x, y, s[ p ] );
				x -= 8;
			}
		}
		private void tアイテム数の描画_１桁描画( int x, int y, char s数値 )
		{
			int dx, dy;
			if ( s数値 == '/' )
			{
				dx = 48;
				dy = 0;
			}
			else
			{
				int n = (int) s数値 - (int) '0';
				dx = ( n % 6 ) * 8;
				dy = ( n / 6 ) * 12;
			}
			//if ( this.txアイテム数数字 != null )
			//{
			//	this.txアイテム数数字.t2D描画( CDTXMania.app.Device, x, y, new Rectangle( dx, dy, 8, 12 ) );
			//}
		}


        //数字フォント
        private CTexture txレベル数字フォント;
        [StructLayout( LayoutKind.Sequential )]
        private struct STレベル数字
        {
            public char ch;
            public int ptX;
        }
        private STレベル数字[] st小文字位置 = new STレベル数字[ 10 ];
        private void t小文字表示(int x, int y, string str)
        {
            foreach (char ch in str)
            {
                for (int i = 0; i < this.st小文字位置.Length; i++)
                {
                    if( this.st小文字位置[i].ch == ch )
                    {
                        Rectangle rectangle = new Rectangle( this.st小文字位置[i].ptX, 0, 21, 25 );
                        if (TJAPlayer3.Tx.SongSelect_Level_Number != null)
                        {
							if (str.Length > 1) TJAPlayer3.Tx.SongSelect_Level_Number.vc拡大縮小倍率.X = 0.8f;
							else TJAPlayer3.Tx.SongSelect_Level_Number.vc拡大縮小倍率.X = 1.0f;
							TJAPlayer3.Tx.SongSelect_Level_Number.t2D描画(TJAPlayer3.app.Device, x, y, rectangle);
                        }
                        break;
                    }
                }
                x += 11;
            }
        }
		//-----------------
		#endregion
	}
}
