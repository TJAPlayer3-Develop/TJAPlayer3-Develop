using FDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TJAPlayer3
{
    class CNamePlate
    {
        public void tNamePlateInit()
        {
            if (TJAPlayer3.ConfigIni.FontName != null)
            {
                pfName = new CPrivateFastFont(new FontFamily(TJAPlayer3.ConfigIni.FontName), 14);
                pfTitle = new CPrivateFastFont(new FontFamily(TJAPlayer3.ConfigIni.FontName), 8);
            }
            else
            {
                pfName = new CPrivateFastFont(new FontFamily("MS Gothic UI"), 14);
                pfTitle = new CPrivateFastFont(new FontFamily("MS Gothic UI"), 8);
            }

            using (var NameTexture = pfName.DrawPrivateFont(TJAPlayer3.ConfigIni.strNamePlateName, Color.White, Color.Black))
            {
                txName = TJAPlayer3.tテクスチャの生成(NameTexture);
            }

            using (var TitleTexture = pfTitle.DrawPrivateFont(TJAPlayer3.ConfigIni.strNamePlateTitle, Color.Black, Color.Empty))
            {
                txTitle = TJAPlayer3.tテクスチャの生成(TitleTexture);
            }
        }

        public void tNamePlateDraw(int x, int y)
        {
            txTitle.t2D中心基準描画(TJAPlayer3.app.Device, x + 100, y + 17);
            txName.t2D中心基準描画(TJAPlayer3.app.Device, x + 100, y + 40);
        }

        private CPrivateFastFont pfName;
        private CPrivateFastFont pfTitle;
        private CTexture txName;
        private CTexture txTitle;
    }
}
