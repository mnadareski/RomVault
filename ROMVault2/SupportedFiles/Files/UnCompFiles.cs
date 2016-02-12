/******************************************************
 *     ROMVault2 is written by Gordon J.              *
 *     Contact gordon@romvault.com                    *
 *     Copyright 2014                                 *
 ******************************************************/

using System.IO;
using System.Security.Cryptography;
using System.Threading;
using ROMVault2.SupportedFiles.Zip.ZLib;

namespace ROMVault2.SupportedFiles.Files
{
    public static class UnCompFiles
    {
        private const int Buffersize = 4096 * 1024;
        private static readonly byte[] Buffer0;
        private static readonly byte[] Buffer1;

        static UnCompFiles()
        {
            Buffer0 = new byte[Buffersize];
            Buffer1 = new byte[Buffersize];
        }

        public static int CheckSumRead(string filename, bool testDeep, out byte[] crc, out byte[] bMD5, out byte[] bSHA1)
        {
            bMD5 = null;
            bSHA1 = null;
            crc = null;

            Stream ds = null;
            CRC32Hash crc32 = new CRC32Hash();

            MD5 md5 = null;
            if (testDeep) md5 = MD5.Create();
            SHA1 sha1 = null;
            if (testDeep) sha1 = SHA1.Create();

            try
            {
                int errorCode = IO.FileStream.OpenFileRead(filename, out ds);
                if (errorCode != 0)
                    return errorCode;

                long sizetogo = ds.Length;

                // Pre load the first buffer0
                int sizeNext = sizetogo > Buffersize ? Buffersize : (int)sizetogo;
                ds.Read(Buffer0, 0, sizeNext);
                int sizebuffer = sizeNext;
                sizetogo -= sizeNext;
                bool whichBuffer = true;

                while (sizebuffer > 0)
                {
                    sizeNext = sizetogo > Buffersize ? Buffersize : (int)sizetogo;

                    Thread t0 = null;
                    if (sizeNext > 0)
                    {
                        t0 = new Thread(() => { ds.Read(whichBuffer ? Buffer1 : Buffer0, 0, sizeNext); });
                        t0.Start();
                    }

                    byte[] buffer = whichBuffer ? Buffer0 : Buffer1;
                    Thread t1 = new Thread(() => { crc32.TransformBlock(buffer, 0, sizebuffer, null, 0); });
                    t1.Start();
                    if (testDeep)
                    {
                        Thread t2 = new Thread(() => { md5.TransformBlock(buffer, 0, sizebuffer, null, 0); });
                        Thread t3 = new Thread(() => { sha1.TransformBlock(buffer, 0, sizebuffer, null, 0); });
                        t2.Start();
                        t3.Start();
                        t2.Join();
                        t3.Join();
                    }
                    if (t0 != null)
                        t0.Join();
                    t1.Join();

                    sizebuffer = sizeNext;
                    sizetogo -= sizeNext;
                    whichBuffer = !whichBuffer;
                }

                crc32.TransformFinalBlock(Buffer0, 0, 0);
                if (testDeep) md5.TransformFinalBlock(Buffer0, 0, 0);
                if (testDeep) sha1.TransformFinalBlock(Buffer0, 0, 0);

                ds.Close();
            }
            catch
            {
                if (ds != null)
                    ds.Close();

                return 0x17;
            }

            crc = crc32.Hash;
            if (testDeep) bMD5 = md5.Hash;
            if (testDeep) bSHA1 = sha1.Hash;

            return 0;
        }
    }
}
