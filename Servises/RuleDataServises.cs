using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows;
using Li.Krkr.krkrfgformatWPF.Models;
using Newtonsoft.Json;

namespace Li.Krkr.krkrfgformatWPF.Servises
{
    public class RuleDataServises
    {
        public static RuleDataModel CreatFromFile(string file)
        {
            if (string.IsNullOrEmpty(file)) return null;
            
            if (Path.GetExtension(file).ToLower()==".json")
            {
                return CreatFromJsonFile(file);
            }
            FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader br = new BinaryReader(fs);
            var header = br.ReadBytes(5);
            int index = 0;
            uint signature = (uint)(header[index] | (header[index + 1] << 8) | (header[index + 2] << 16) | (header[index + 3] << 24));
            if ((signature & 0xFF00FFFFu) == 0xFF00FEFEu && header[2] < 3 && 0xFE == header[4])
            {
                if (2 == header[2])
                    return CreatFromeZlibFile(fs, br);
                else if (1 == header[2])
                {
                    //br.Close();
                    return CreatFromeCryptFile(fs);
                }
            }
            Encoding encoding = (header[1] == 0) ? Encoding.Unicode : Encoding.UTF8;
            
            return CreatFromTextFile(fs, encoding);
        }
        /// <summary>
        /// 解析被简单算法加密的txt文档
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        private static RuleDataModel CreatFromeCryptFile(FileStream fs)
        {
            var reader = new BinaryReader(fs, Encoding.Unicode, true);
            var outsr = new MemoryStream((int)fs.Length + 2);
            var writer = new BinaryWriter(outsr, Encoding.Unicode, true);
            writer.Write('\xFEFF');
            int c;
            while ((c = reader.Read()) != -1)
            {
                c = (c & 0xAAAA) >> 1 | (c & 0x5555) << 1;
                writer.Write((char)c);
            }
            reader.Close();
            writer.Close();
            fs.Close();
            return CreatFromTextFile(outsr, Encoding.Unicode);
        }

        /// <summary>
        /// 解析普通文本文档
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        private static RuleDataModel CreatFromTextFile(Stream fs, Encoding encoding)
        {
            List<string> lines = new List<string>();
            fs.Position = 0L;
            using (StreamReader streamReader = new StreamReader(fs, encoding))
            {
                while (streamReader.Peek()>=0)
                {
                    string str = streamReader.ReadLine();
                    if(!string.IsNullOrEmpty(str))
                    {
                        lines.Add(str);
                    }
                }
            }


            RuleDataModel data = new RuleDataModel()
            {
                OriginalFilePath = "",
                FileHander = lines[0],
                FgLarge = LineDataModel.CreatFromLineString(lines[1])
            };
            for (int i = 2; i < lines.Count; i++)
            {
                data.TextData.Add(LineDataModel.CreatFromLineString(lines[i]));
            }
            fs.Close();
            return data;
        }
        /// <summary>
        /// 解析被ZLIB压缩的文本内容
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="br"></param>
        /// <returns></returns>
        private static RuleDataModel CreatFromeZlibFile(FileStream fs, BinaryReader br)
        {
            List<string> lines = new List<string>();
            var dataLength = br.ReadInt32();
            fs.Position = fs.Length - dataLength;
            int b1 = br.ReadByte();
            int b2 = br.ReadByte();
            if ((0x78 != b1 && 0x58 != b1) || 0 != (b1 << 8 | b2) % 31)
            { throw new InvalidDataException("Data not recoginzed as zlib-compressed stream"); }
            using (var deStream = new DeflateStream(new MemoryStream(br.ReadBytes(dataLength - 2)), CompressionMode.Decompress, true))
            {
                using (var sr = new StreamReader(deStream, Encoding.Unicode))
                {
                    while (sr.Peek() >= 0)
                    {
                        string str = sr.ReadLine();
                        if (!string.IsNullOrEmpty(str))
                        {
                            lines.Add(str);
                        }
                    }
                }
            }

            RuleDataModel data = new RuleDataModel()
            {
                OriginalFilePath = fs.Name,
                FileHander = lines[0],
                FgLarge = LineDataModel.CreatFromLineString(lines[1])
            };
            for (int i = 2; i < lines.Count; i++)
            {
                data.TextData.Add(LineDataModel.CreatFromLineString(lines[i]));
            }
            br.Close();
            fs.Close();
            return data;
        }
        /// <summary>
        /// 解析JSON文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static RuleDataModel CreatFromJsonFile(string file)
        {
            LineDataModel[] array = JsonConvert.DeserializeObject<LineDataModel[]>(File.ReadAllText(file).Replace("\t", ""));
            RuleDataModel data = new RuleDataModel();
            data.OriginalFilePath = file;
            data.FileHander = string.Empty;
            data.FgLarge = array[0];
            for (int i = 1; i < array.Length; i++)
            {
                data.TextData.Add(array[i]);
            }
            return data;
        }
        
    }
}
