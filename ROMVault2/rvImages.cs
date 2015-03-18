/******************************************************
 *     ROMVault2 is written by Gordon J.              *
 *     Contact gordon@romvault.com                    *
 *     Copyright 2014                                 *
 ******************************************************/

using System.Collections.Generic;
using System.Drawing;

namespace ROMVault2
{
    public static class rvImages
    {
        public static Bitmap GetBitmap(string bitmapName)
        {
            object bmObj = rvImages1.ResourceManager.GetObject(bitmapName);

            Bitmap bm = null;
            if (bmObj != null)
                bm = (Bitmap)bmObj;

            return bm;
        }
    }
}
