/******************************************************
 *     ROMVault2 is written by Gordon J.              *
 *     Contact gordon@romvault.com                    *
 *     Copyright 2014                                 *
 ******************************************************/

using System;
using ROMVault2.RvDB;

namespace ROMVault2.Utils
{
    public class DBTypeGet
    {
        public static FileType DirFromFile(FileType ft)
        {
            switch (ft)
            {
                case FileType.File:
                    return FileType.Dir;
                case FileType.ZipFile:
                    return FileType.Zip;
                case FileType.SevenZipFile:
                    return FileType.SevenZip;
            }
            return FileType.Zip;
        }

        public static FileType FileFromDir(FileType ft)
        {
            switch (ft)
            {
                case FileType.Dir:
                    return FileType.File;
                case FileType.Zip:
                    return FileType.ZipFile;
                case FileType.SevenZip:
                    return FileType.SevenZipFile;
            }
            return FileType.Zip;
        }

        public static bool isCompressedDir(FileType fileType)
        {
            return (fileType == FileType.Zip || fileType==FileType.SevenZip);
        }

        public static RvBase GetRvType(FileType fileType)
        {
            switch (fileType)
            {
                case FileType.Dir: 
                case FileType.Zip: 
                case FileType.SevenZip:
                    return new RvDir(fileType);
                case FileType.File:
                case FileType.ZipFile:
                case FileType.SevenZipFile:
                    return new RvFile(fileType);
                default:
                    throw new Exception("Unknown file type");
            }
        }
    }
}
