


using System;
using FDK;

namespace TJAPlayer3
{
    internal class CAct演奏Drums背景 : CActivity
    {
        // 本家っぽい背景を表示させるメソッド。
        //
        // 拡張性とかないんで。はい、ヨロシクゥ!
        //
        public CAct演奏Drums背景()
        {
            base.b活性化してない = true;
        }

        public void tFadeIn(int player)
        {
            this.ct上背景クリアインタイマー[player] = new CCounter(0, 100, 2, TJAPlayer3.Timer);
            this.eFadeMode = EFIFOモード.フェードイン;
        }

        //public void tFadeOut(int player)
        //{
        //    this.ct上背景フェードタイマー[player] = new CCounter( 0, 100, 6, CDTXMania.Timer );
        //    this.eFadeMode = EFIFOモード.フェードアウト;
        //}

        public void ClearIn(int player)
        {
            this.ct上背景クリアインタイマー[player] = new CCounter(0, 100, 2, TJAPlayer3.Timer);
            this.ct上背景クリアインタイマー[player].n現在の値 = 0;
            this.ct上背景FIFOタイマー = new CCounter(0, 100, 2, TJAPlayer3.Timer);
            this.ct上背景FIFOタイマー.n現在の値 = 0;
        }

        public override void On活性化()
        {
            base.On活性化();
        }

        public override void On非活性化()
        {
            TJAPlayer3.t安全にDisposeする(ref this.ct上背景FIFOタイマー);
            for (int i = 0; i < TJAPlayer3.ConfigIni.nPlayerCount; i++)
            {
                ct上背景スクロール用タイマー1st[i] = null;
                ct上背景スクロール用タイマー2nd[i] = null;
                ct上背景スクロール用タイマー3rd[i] = null;
            }
            TJAPlayer3.t安全にDisposeする(ref this.ct下背景スクロール用タイマー1);
            TJAPlayer3.t安全にDisposeする(ref this.ct桜X移動用タイマー1);
            TJAPlayer3.t安全にDisposeする(ref this.ct桜Y移動用タイマー1);
            TJAPlayer3.t安全にDisposeする(ref this.ct桜X移動用タイマー2);
            TJAPlayer3.t安全にDisposeする(ref this.ct桜Y移動用タイマー2);
            TJAPlayer3.t安全にDisposeする(ref this.ct桜X移動用タイマー3);
            TJAPlayer3.t安全にDisposeする(ref this.ct桜Y移動用タイマー3);
            base.On非活性化();
        }

        public override void OnManagedリソースの作成()
        {
            this.ct上背景スクロール用タイマー1st = new CCounter[2];
            this.ct上背景スクロール用タイマー2nd = new CCounter[2];
            this.ct上背景スクロール用タイマー3rd = new CCounter[2];
            this.ct上背景クリアインタイマー = new CCounter[2];

            for (int i = 0; i < TJAPlayer3.ConfigIni.nPlayerCount; i++)
            {
                if (TJAPlayer3.Tx.Background_Up_3rd[i] != null)
                {
                    this.ct上背景スクロール用タイマー1st[i] = new CCounter(1, TJAPlayer3.Tx.Background_Up_1st[i].szテクスチャサイズ.Width, 16, TJAPlayer3.Timer);
                    this.ct上背景スクロール用タイマー2nd[i] = new CCounter(1, TJAPlayer3.Tx.Background_Up_2nd[i].szテクスチャサイズ.Height, 70, TJAPlayer3.Timer);
                    this.ct上背景スクロール用タイマー3rd[i] = new CCounter(1, 600, 3, TJAPlayer3.Timer);
                    this.ct上背景クリアインタイマー[i] = new CCounter();
                }
            }

            if (TJAPlayer3.Tx.Background_Down_Scroll != null)
                this.ct下背景スクロール用タイマー1 = new CCounter(1, TJAPlayer3.Tx.Background_Down_Scroll.szテクスチャサイズ.Width, 4, TJAPlayer3.Timer);

            if (TJAPlayer3.Tx.Background_Down_Sakura != null)
            {
                this.ct桜X移動用タイマー1 = new CCounter(0, 166, 15, TJAPlayer3.Timer);
                this.ct桜Y移動用タイマー1 = new CCounter(0, 500, 5, TJAPlayer3.Timer);
                this.ct桜X移動用タイマー2 = new CCounter(0, 250, 10, TJAPlayer3.Timer);
                this.ct桜Y移動用タイマー2 = new CCounter(0, 500, 5, TJAPlayer3.Timer);
                this.ct桜X移動用タイマー3 = new CCounter(0, 333, 15, TJAPlayer3.Timer);
                this.ct桜Y移動用タイマー3 = new CCounter(0, 500, 10, TJAPlayer3.Timer);
            }

            this.Background_Down_Index = TJAPlayer3.Random.Next(TJAPlayer3.Skin.Game_Background_Down_Ptn);

            this.ct上背景FIFOタイマー = new CCounter();
            base.OnManagedリソースの作成();
        }

        public override void OnManagedリソースの解放()
        {
            //CDTXMania.tテクスチャの解放( ref this.tx上背景メイン );
            //CDTXMania.tテクスチャの解放( ref this.tx上背景クリアメイン );
            //CDTXMania.tテクスチャの解放( ref this.tx下背景メイン );
            //CDTXMania.tテクスチャの解放( ref this.tx下背景クリアメイン );
            //CDTXMania.tテクスチャの解放( ref this.tx下背景クリアサブ1 );
            //Trace.TraceInformation("CActDrums背景 リソースの開放");
            base.OnManagedリソースの解放();
        }

        public override int On進行描画()
        {
            this.ct上背景FIFOタイマー.t進行();

            for (int i = 0; i < TJAPlayer3.ConfigIni.nPlayerCount; i++)
            {
                if (this.ct上背景クリアインタイマー[i] != null)
                    this.ct上背景クリアインタイマー[i].t進行();
            }
            for (int i = 0; i < TJAPlayer3.ConfigIni.nPlayerCount; i++)
            {
                if (this.ct上背景スクロール用タイマー1st[i] != null)
                    this.ct上背景スクロール用タイマー1st[i].t進行Loop();
            }
            for (int i = 0; i < TJAPlayer3.ConfigIni.nPlayerCount; i++)
            {
                if (this.ct上背景スクロール用タイマー2nd[i] != null)
                    this.ct上背景スクロール用タイマー2nd[i].t進行Loop();
            }
            for (int i = 0; i < TJAPlayer3.ConfigIni.nPlayerCount; i++)
            {
                if (this.ct上背景スクロール用タイマー3rd[i] != null)
                    this.ct上背景スクロール用タイマー3rd[i].t進行Loop();
            }
            if (this.ct下背景スクロール用タイマー1 != null)
                this.ct下背景スクロール用タイマー1.t進行Loop();

            if (this.ct桜X移動用タイマー1 != null)
                this.ct桜X移動用タイマー1.t進行Loop();

            if (this.ct桜Y移動用タイマー1 != null)
                this.ct桜Y移動用タイマー1.t進行Loop();

            if (this.ct桜X移動用タイマー2 != null)
                this.ct桜X移動用タイマー2.t進行Loop();

            if (this.ct桜Y移動用タイマー2 != null)
                this.ct桜Y移動用タイマー2.t進行Loop();

            if (this.ct桜X移動用タイマー3 != null)
                this.ct桜X移動用タイマー3.t進行Loop();

            if (this.ct桜Y移動用タイマー3 != null)
                this.ct桜Y移動用タイマー3.t進行Loop();

            #region 1P-2P-上背景
            for (int i = 0; i < TJAPlayer3.ConfigIni.nPlayerCount; i++)
            {
                if (this.ct上背景スクロール用タイマー1st[i] != null)
                {
                    double TexSizeL = 1280 / TJAPlayer3.Tx.Background_Up_1st[i].szテクスチャサイズ.Width;
                    double TexSizeW = 308 / TJAPlayer3.Tx.Background_Up_2nd[i].szテクスチャサイズ.Height;
                    double TexSizeF = 1280 / TJAPlayer3.Tx.Background_Up_3rd[i].szテクスチャサイズ.Width;
                    // 1280をテクスチャサイズで割ったものを切り上げて、プラス+1足す。
                    int ForLoopL = (int)Math.Ceiling(TexSizeL) + 1;
                    int ForLoopW = (int)Math.Ceiling(TexSizeW) + 1;
                    int ForLoopF = (int)Math.Ceiling(TexSizeF) + 1;
                    //int nループ幅 = 328;

                    #region [ 上背景-Back ]

                    for (int W = 1; W < ForLoopW + 1; W++)
                    {
                        TJAPlayer3.Tx.Background_Up_1st[i].t2D描画(TJAPlayer3.app.Device, 0 - this.ct上背景スクロール用タイマー1st[i].n現在の値, (185 + i * 600) - (W * TJAPlayer3.Tx.Background_Up_1st[i].szテクスチャサイズ.Height) + ct上背景スクロール用タイマー2nd[i].n現在の値);
                    }
                    for (int l = 1; l < ForLoopL + 1; l++)
                    {
                        for (int W = 1; W < ForLoopW + 1; W++)
                        {
                            TJAPlayer3.Tx.Background_Up_1st[i].t2D描画(TJAPlayer3.app.Device, +(l * TJAPlayer3.Tx.Background_Up_1st[i].szテクスチャサイズ.Width) - this.ct上背景スクロール用タイマー1st[i].n現在の値, (185 + i * 600) - (W * TJAPlayer3.Tx.Background_Up_1st[i].szテクスチャサイズ.Height) + ct上背景スクロール用タイマー2nd[i].n現在の値);
                        }
                    }

                    for (int W = 1; W < ForLoopW + 1; W++)
                    {
                        TJAPlayer3.Tx.Background_Up_2nd[i].t2D描画(TJAPlayer3.app.Device, 0 - this.ct上背景スクロール用タイマー1st[i].n現在の値, (370 + i * 600) - (W * TJAPlayer3.Tx.Background_Up_2nd[i].szテクスチャサイズ.Height) - ct上背景スクロール用タイマー2nd[i].n現在の値);
                    }
                    for (int l = 1; l < ForLoopL + 1; l++)
                    {
                        for (int W = 1; W < ForLoopW + 1; W++)
                        {
                            TJAPlayer3.Tx.Background_Up_2nd[i].t2D描画(TJAPlayer3.app.Device, +(l * TJAPlayer3.Tx.Background_Up_2nd[i].szテクスチャサイズ.Width) - this.ct上背景スクロール用タイマー1st[i].n現在の値, (370 + i * 600) - (W * TJAPlayer3.Tx.Background_Up_2nd[i].szテクスチャサイズ.Height) - ct上背景スクロール用タイマー2nd[i].n現在の値);
                        }
                    }

                    #endregion

                    #region [ 上背景-Front ]

                    float thirdy = 0;
                    float thirdx = 0;

                    if(this.ct上背景スクロール用タイマー3rd[i].n現在の値 <= 270)
                    {
                        thirdx = this.ct上背景スクロール用タイマー3rd[i].n現在の値 * 0.9258f;
						thirdy = (float)Math.Sin((float)this.ct上背景スクロール用タイマー3rd[i].n現在の値 * (Math.PI / 270.0f)) * 40.0f;
					}
					else
					{
						thirdx = 250 + (ct上背景スクロール用タイマー3rd[i].n現在の値 - 270) * 0.24f;

						if (this.ct上背景スクロール用タイマー3rd[i].n現在の値 <= 490) thirdy = -(float)Math.Sin((float)(this.ct上背景スクロール用タイマー3rd[i].n現在の値 - 270) * (Math.PI / 170.0f)) * 15.0f;
						else thirdy = -((float)Math.Sin((float)220f * (Math.PI / 170.0f)) * 15.0f) + (float)(((this.ct上背景スクロール用タイマー3rd[i].n現在の値 - 490) / 110f) * ((float)Math.Sin((float)220f * (Math.PI / 170.0f)) * 15.0f));
					}

                    TJAPlayer3.Tx.Background_Up_3rd[i].t2D描画(TJAPlayer3.app.Device, 0 - thirdx, 0 + i * 540 - thirdy);

                    for (int l = 1; l < ForLoopF + 1; l++)
                    {
                        TJAPlayer3.Tx.Background_Up_3rd[i].t2D描画(TJAPlayer3.app.Device, +(l * TJAPlayer3.Tx.Background_Up_3rd[i].szテクスチャサイズ.Width) - thirdx, 0 + i * 540 - thirdy);
                    }

                    #endregion
                }

                if (this.ct上背景スクロール用タイマー1st[i] != null)
                {
                    if (TJAPlayer3.stage演奏ドラム画面.bIsAlreadyCleared[i])
                    {
                        TJAPlayer3.Tx.Background_Up_1st[2].Opacity = ((this.ct上背景クリアインタイマー[i].n現在の値 * 0xff) / 100);
                        TJAPlayer3.Tx.Background_Up_2nd[2].Opacity = ((this.ct上背景クリアインタイマー[i].n現在の値 * 0xff) / 100);
                        TJAPlayer3.Tx.Background_Up_3rd[2].Opacity = ((this.ct上背景クリアインタイマー[i].n現在の値 * 0xff) / 100);
                    }
                    else
                    {
                        TJAPlayer3.Tx.Background_Up_1st[2].Opacity = 0;
                        TJAPlayer3.Tx.Background_Up_2nd[2].Opacity = 0;
                        TJAPlayer3.Tx.Background_Up_3rd[2].Opacity = 0;
                    }

                    double TexSizeL = 1280 / TJAPlayer3.Tx.Background_Up_1st[2].szテクスチャサイズ.Width;
                    double TexSizeW = 308 / TJAPlayer3.Tx.Background_Up_2nd[2].szテクスチャサイズ.Height;
                    double TexSizeF = 1280 / TJAPlayer3.Tx.Background_Up_3rd[2].szテクスチャサイズ.Width;
                    // 1280をテクスチャサイズで割ったものを切り上げて、プラス+1足す。
                    int ForLoopL = (int)Math.Ceiling(TexSizeL) + 1;
                    int ForLoopW = (int)Math.Ceiling(TexSizeW) + 1;
                    int ForLoopF = (int)Math.Ceiling(TexSizeF) + 1;

                    #region [ 上背景-Back ]

                    for (int W = 1; W < ForLoopW + 1; W++)
                    {
                        TJAPlayer3.Tx.Background_Up_1st[2].t2D描画(TJAPlayer3.app.Device, 0 - this.ct上背景スクロール用タイマー1st[i].n現在の値, (185 + i * 600) - (W * TJAPlayer3.Tx.Background_Up_1st[2].szテクスチャサイズ.Height) + ct上背景スクロール用タイマー2nd[i].n現在の値);
                    }
                    for (int l = 1; l < ForLoopL + 1; l++)
                    {
                        for (int W = 1; W < ForLoopW + 1; W++)
                        {
                            TJAPlayer3.Tx.Background_Up_1st[2].t2D描画(TJAPlayer3.app.Device, +(l * TJAPlayer3.Tx.Background_Up_1st[2].szテクスチャサイズ.Width) - this.ct上背景スクロール用タイマー1st[i].n現在の値, (185 + i * 600) - (W * TJAPlayer3.Tx.Background_Up_1st[2].szテクスチャサイズ.Height) + ct上背景スクロール用タイマー2nd[i].n現在の値);
                        }
                    }

                    for (int W = 1; W < ForLoopW + 1; W++)
                    {
                        TJAPlayer3.Tx.Background_Up_2nd[2].t2D描画(TJAPlayer3.app.Device, 0 - this.ct上背景スクロール用タイマー1st[i].n現在の値, (370 + i * 600) - (W * TJAPlayer3.Tx.Background_Up_2nd[2].szテクスチャサイズ.Height) - ct上背景スクロール用タイマー2nd[i].n現在の値);
                    }
                    for (int l = 1; l < ForLoopL + 1; l++)
                    {
                        for (int W = 1; W < ForLoopW + 1; W++)
                        {
                            TJAPlayer3.Tx.Background_Up_2nd[2].t2D描画(TJAPlayer3.app.Device, +(l * TJAPlayer3.Tx.Background_Up_2nd[2].szテクスチャサイズ.Width) - this.ct上背景スクロール用タイマー1st[i].n現在の値, (370 + i * 600) - (W * TJAPlayer3.Tx.Background_Up_2nd[2].szテクスチャサイズ.Height) - ct上背景スクロール用タイマー2nd[i].n現在の値);
                        }
                    }

                    #endregion

                    #region [ 上背景-Front ]

                    float thirdy = 0;
                    float thirdx = 0;

                    if(this.ct上背景スクロール用タイマー3rd[i].n現在の値 <= 270)
                    {
                        thirdx = this.ct上背景スクロール用タイマー3rd[i].n現在の値 * 0.9258f;
						thirdy = (float)Math.Sin((float)this.ct上背景スクロール用タイマー3rd[i].n現在の値 * (Math.PI / 270.0f)) * 40.0f;
					}
					else
					{
						thirdx = 250 + (ct上背景スクロール用タイマー3rd[i].n現在の値 - 270) * 0.24f;

						if (this.ct上背景スクロール用タイマー3rd[i].n現在の値 <= 490) thirdy = -(float)Math.Sin((float)(this.ct上背景スクロール用タイマー3rd[i].n現在の値 - 270) * (Math.PI / 170.0f)) * 15.0f;
						else thirdy = -((float)Math.Sin((float)220f * (Math.PI / 170.0f)) * 15.0f) + (float)(((this.ct上背景スクロール用タイマー3rd[i].n現在の値 - 490) / 110f) * ((float)Math.Sin((float)220f * (Math.PI / 170.0f)) * 15.0f));
					}

                    TJAPlayer3.Tx.Background_Up_3rd[2].t2D描画(TJAPlayer3.app.Device, 0 - thirdx, 0 + i * 540 - thirdy);

                    for (int l = 1; l < ForLoopF + 1; l++)
                    {
                        TJAPlayer3.Tx.Background_Up_3rd[2].t2D描画(TJAPlayer3.app.Device, +(l * TJAPlayer3.Tx.Background_Up_3rd[2].szテクスチャサイズ.Width) - thirdx, 0 + i * 540 - thirdy);
                    }

                    #endregion
                }
            }
            #endregion
            #region 1P-下背景
            if (!TJAPlayer3.stage演奏ドラム画面.bDoublePlay)
            {
                if (TJAPlayer3.Skin.Game_Background_Down_Ptn != 0)
                {
                    TJAPlayer3.Tx.Background_Down[this.Background_Down_Index].t2D描画(TJAPlayer3.app.Device, 0, 360);

		    #region 桜モーション
                    if (TJAPlayer3.Tx.Background_Down_Sakura != null && this.Background_Down_Index == 0)
                    {
                        TJAPlayer3.Tx.Background_Down_Sakura.t2D描画(TJAPlayer3.app.Device, 900 - this.ct桜X移動用タイマー1.n現在の値, 400 + this.ct桜Y移動用タイマー1.n現在の値);
                        TJAPlayer3.Tx.Background_Down_Sakura.t2D描画(TJAPlayer3.app.Device, 100 + this.ct桜X移動用タイマー1.n現在の値, 350 + this.ct桜Y移動用タイマー1.n現在の値);
                        TJAPlayer3.Tx.Background_Down_Sakura.t2D描画(TJAPlayer3.app.Device, 1300 - this.ct桜X移動用タイマー1.n現在の値, 350 + this.ct桜Y移動用タイマー1.n現在の値);
                        TJAPlayer3.Tx.Background_Down_Sakura.t2D描画(TJAPlayer3.app.Device, 900 - this.ct桜X移動用タイマー2.n現在の値, 320 + this.ct桜Y移動用タイマー2.n現在の値);
                        TJAPlayer3.Tx.Background_Down_Sakura.t2D描画(TJAPlayer3.app.Device, 800 - this.ct桜X移動用タイマー3.n現在の値, 450 + this.ct桜Y移動用タイマー3.n現在の値);
                        TJAPlayer3.Tx.Background_Down_Sakura.t2D描画(TJAPlayer3.app.Device, 10 + this.ct桜X移動用タイマー3.n現在の値, 430 + this.ct桜Y移動用タイマー3.n現在の値);
                        TJAPlayer3.Tx.Background_Down_Sakura.t2D描画(TJAPlayer3.app.Device, 800 - this.ct桜X移動用タイマー1.n現在の値, 200 + this.ct桜Y移動用タイマー1.n現在の値);
                        TJAPlayer3.Tx.Background_Down_Sakura.t2D描画(TJAPlayer3.app.Device, 0 + this.ct桜X移動用タイマー1.n現在の値, 220 + this.ct桜Y移動用タイマー1.n現在の値);
                        TJAPlayer3.Tx.Background_Down_Sakura.t2D描画(TJAPlayer3.app.Device, 1100 - this.ct桜X移動用タイマー1.n現在の値, 250 + this.ct桜Y移動用タイマー1.n現在の値);
                    }
                    #endregion
                }
                if (TJAPlayer3.stage演奏ドラム画面.bIsAlreadyCleared[0])
                {
                    if (TJAPlayer3.Tx.Background_Down_Clear != null && TJAPlayer3.Tx.Background_Down_Scroll != null && ct下背景スクロール用タイマー1 != null)
                    {
                        TJAPlayer3.Tx.Background_Down_Clear.Opacity = ((this.ct上背景FIFOタイマー.n現在の値 * 0xff) / 100);
                        TJAPlayer3.Tx.Background_Down_Scroll.Opacity = ((this.ct上背景FIFOタイマー.n現在の値 * 0xff) / 100);
                        TJAPlayer3.Tx.Background_Down_Clear.t2D描画(TJAPlayer3.app.Device, 0, 360);

                        //int nループ幅 = 1257;
                        //CDTXMania.Tx.Background_Down_Scroll.t2D描画( CDTXMania.app.Device, 0 - this.ct下背景スクロール用タイマー1.n現在の値, 360 );
                        //CDTXMania.Tx.Background_Down_Scroll.t2D描画(CDTXMania.app.Device, (1 * nループ幅) - this.ct下背景スクロール用タイマー1.n現在の値, 360);
                        double TexSize = 1280 / TJAPlayer3.Tx.Background_Down_Scroll.szテクスチャサイズ.Width;
                        // 1280をテクスチャサイズで割ったものを切り上げて、プラス+1足す。
                        int ForLoop = (int)Math.Ceiling(TexSize) + 1;

                        //int nループ幅 = 328;
                        TJAPlayer3.Tx.Background_Down_Scroll.t2D描画(TJAPlayer3.app.Device, 0 - this.ct下背景スクロール用タイマー1.n現在の値, 360);
                        for (int l = 1; l < ForLoop + 1; l++)
                        {
                            TJAPlayer3.Tx.Background_Down_Scroll.t2D描画(TJAPlayer3.app.Device, +(l * TJAPlayer3.Tx.Background_Down_Scroll.szテクスチャサイズ.Width) - this.ct下背景スクロール用タイマー1.n現在の値, 360);
                        }

                    }
                }
            }
            #endregion
            return base.On進行描画();
        }

        #region[ private ]
        //-----------------
        private CCounter[] ct上背景スクロール用タイマー1st; //上背景のX方向スクロール用
        private CCounter[] ct上背景スクロール用タイマー2nd; //上背景のY方向スクロール用
        private CCounter[] ct上背景スクロール用タイマー3rd; //上背景のY方向スクロール用
        private CCounter ct下背景スクロール用タイマー1; //下背景パーツ1のX方向スクロール用
        private CCounter ct上背景FIFOタイマー;
        private CCounter[] ct上背景クリアインタイマー;
        private CCounter ct桜X移動用タイマー1;
        private CCounter ct桜Y移動用タイマー1;
        private CCounter ct桜X移動用タイマー2;
        private CCounter ct桜Y移動用タイマー2;
        private CCounter ct桜X移動用タイマー3;
        private CCounter ct桜Y移動用タイマー3;
        //private CTexture tx上背景メイン;
        //private CTexture tx上背景クリアメイン;
        //private CTexture tx下背景メイン;
        //private CTexture tx下背景クリアメイン;
        //private CTexture tx下背景クリアサブ1;
	private int Background_Down_Index;
        private EFIFOモード eFadeMode;
        //-----------------
        #endregion
    }
}
