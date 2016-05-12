/******************************************************
 *     ROMVault2 is written by Gordon J.              *
 *     Contact gordon@romvault.com                    *
 *     Copyright 2014                                 *
 ******************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using ROMVault2.RvDB;

namespace ROMVault2
{

    public enum eScanLevel
    {
        Level1,
        Level2,
        Level3
    }


    public enum eFixLevel
    {
        TrrntZipLevel1,
        TrrntZipLevel2,
        TrrntZipLevel3,
        Level1,
        Level2,
        Level3
    }

    public class Settings
    {

        public string DatRoot;
        public string CacheFile;
        public eScanLevel ScanLevel;
        public eFixLevel FixLevel;

        public List<DirMap> DirPathMap;

        public List<string> IgnoreFiles;

        public bool DoubleCheckDelete = true;
        public bool DebugLogsEnabled;
        public bool CacheSaveTimerEnabled = true;
        public int CacheSaveTimePeriod = 10;

        public bool IsUnix
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return ((p == 4) || (p == 6) || (p == 128));
            }
        }

        public bool IsMono { get { return (Type.GetType("Mono.Runtime") != null); } }

        public void SetDefaults()
        {
            CacheFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RomVault2_" + DBVersion.Version + ".Cache");

            DatRoot = "DatRoot";

            ScanLevel = eScanLevel.Level2;
            FixLevel = eFixLevel.TrrntZipLevel2;

            IgnoreFiles = new List<string> { "_ReadMe_.txt" };

            ResetDirectories();

            ReadConfig();

            DirPathMap.Sort();
        }

        public void ResetDirectories()
        {
            DirPathMap = new List<DirMap>
                             {
                                 new DirMap("RomVault", "RomRoot"),
                                 new DirMap("ToSort", "ToSort")
                             };
        }

        public string ToSort()
        {
            foreach (DirMap t in DirPathMap)
            {
                if (t.DirKey == "ToSort")
                    return t.DirPath;
            }
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ToSort");
        }

        public void WriteConfig()
        {
            //if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RomVault2.cfg")))
            //    File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RomVault2.cfg"));
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RomVault2cfg.xml")))
                File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RomVault2cfg.xml"));

            using (StreamWriter sw = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RomVault2cfg.xml")))
            {
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(Program.rvSettings.GetType());
                x.Serialize(sw, Program.rvSettings);
                sw.Flush();
            }
        }

        private void ReadConfig()
        {
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RomVault2cfg.xml")))
            {
                using (StreamReader sr = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RomVault2cfg.xml")))
                {
                    System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(Program.rvSettings.GetType());
                    Program.rvSettings = (Settings)x.Deserialize(sr);
                }
                return;
            }

            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RomVault2.cfg")))
            {

                FileStream fs = new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RomVault2.cfg"), FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);

                int ver = br.ReadInt32();
                if (ver == 1)
                {
                    DatRoot = br.ReadString();
                    ScanLevel = eScanLevel.Level1;
                    FixLevel = (eFixLevel)br.ReadInt32();

                    IgnoreFiles = new List<string>();
                    int c = br.ReadInt32();
                    for (int i = 0; i < c; i++)
                        IgnoreFiles.Add(br.ReadString());

                    DirPathMap = new List<DirMap>();
                    c = br.ReadInt32();
                    for (int i = 0; i < c; i++)
                        DirPathMap.Add(new DirMap(br.ReadString(), br.ReadString()));
                }
                if (ver == 2)
                {
                    DatRoot = br.ReadString();
                    ScanLevel = (eScanLevel)br.ReadInt32();
                    FixLevel = (eFixLevel)br.ReadInt32();

                    IgnoreFiles = new List<string>();
                    int c = br.ReadInt32();
                    for (int i = 0; i < c; i++)
                        IgnoreFiles.Add(br.ReadString());

                    DirPathMap = new List<DirMap>();
                    c = br.ReadInt32();
                    for (int i = 0; i < c; i++)
                        DirPathMap.Add(new DirMap(br.ReadString(), br.ReadString()));
                }
                if (ver == 3)
                {
                    DatRoot = br.ReadString();
                    ScanLevel = (eScanLevel)br.ReadInt32();
                    FixLevel = (eFixLevel)br.ReadInt32();
                    DebugLogsEnabled = br.ReadBoolean();

                    IgnoreFiles = new List<string>();
                    int c = br.ReadInt32();
                    for (int i = 0; i < c; i++)
                        IgnoreFiles.Add(br.ReadString());

                    DirPathMap = new List<DirMap>();
                    c = br.ReadInt32();
                    for (int i = 0; i < c; i++)
                        DirPathMap.Add(new DirMap(br.ReadString(), br.ReadString()));
                }

                if (ver == 4)
                {
                    DatRoot = br.ReadString();
                    ScanLevel = (eScanLevel)br.ReadInt32();
                    FixLevel = (eFixLevel)br.ReadInt32();
                    DebugLogsEnabled = br.ReadBoolean();

                    IgnoreFiles = new List<string>();
                    int c = br.ReadInt32();
                    for (int i = 0; i < c; i++)
                        IgnoreFiles.Add(br.ReadString());

                    DirPathMap = new List<DirMap>();
                    c = br.ReadInt32();
                    for (int i = 0; i < c; i++)
                        DirPathMap.Add(new DirMap(br.ReadString(), br.ReadString()));

                    CacheSaveTimerEnabled = br.ReadBoolean();
                    CacheSaveTimePeriod = br.ReadInt32();
                }

                if (ver == 5)
                {
                    DatRoot = br.ReadString();
                    ScanLevel = (eScanLevel)br.ReadInt32();
                    FixLevel = (eFixLevel)br.ReadInt32();
                    DebugLogsEnabled = br.ReadBoolean();

                    IgnoreFiles = new List<string>();
                    int c = br.ReadInt32();
                    for (int i = 0; i < c; i++)
                        IgnoreFiles.Add(br.ReadString());

                    DirPathMap = new List<DirMap>();
                    c = br.ReadInt32();
                    for (int i = 0; i < c; i++)
                        DirPathMap.Add(new DirMap(br.ReadString(), br.ReadString()));

                    CacheSaveTimerEnabled = br.ReadBoolean();
                    CacheSaveTimePeriod = br.ReadInt32();

                    DoubleCheckDelete = br.ReadBoolean();
                }

                if (ver == 6)
                {
                    DatRoot = br.ReadString();
                    ScanLevel = (eScanLevel)br.ReadInt32();
                    FixLevel = (eFixLevel)br.ReadInt32();
                    DebugLogsEnabled = br.ReadBoolean();
                    bool UserLongFilenames = br.ReadBoolean();

                    IgnoreFiles = new List<string>();
                    int c = br.ReadInt32();
                    for (int i = 0; i < c; i++)
                        IgnoreFiles.Add(br.ReadString());

                    DirPathMap = new List<DirMap>();
                    c = br.ReadInt32();
                    for (int i = 0; i < c; i++)
                        DirPathMap.Add(new DirMap(br.ReadString(), br.ReadString()));

                    CacheSaveTimerEnabled = br.ReadBoolean();
                    CacheSaveTimePeriod = br.ReadInt32();

                    DoubleCheckDelete = br.ReadBoolean();
                }


                br.Close();
                fs.Close();
            }
        }
    }

    public class DirMap : IComparable<DirMap>
    {
        public string DirKey;
        public string DirPath;

        public DirMap()
        { }

        public DirMap(string key, string path)
        {
            DirKey = key;
            DirPath = path;
        }

        public int CompareTo(DirMap obj)
        {
            return Math.Sign(String.Compare(DirKey, obj.DirKey, StringComparison.Ordinal));
        }
    }
}


