using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EncodingService
{
    public class Base64
    {
        private const int Base64OfUnit = 4;
        private const int OriginalBytesOfUnit = 3;

        private static readonly char[] EncodingTable = new char[64]{
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
            '0','1','2','3','4','5','6','7','8','9',
            '+','/'};

        public string Decode(string input)
        {
            var lenOfInput = input.Length;
            var data = input.ToCharArray();
            var utf8CodeCollection = new List<int>();

            for (var i = 0; i < lenOfInput;)
            {
                long triple = 0;
                for (var j = Base64OfUnit - 1; j >= 0; j--, i++)
                {
                    triple += Array.IndexOf(EncodingTable, data[i]) << j * 6;
                }
                for (var j = 0; j < OriginalBytesOfUnit; j++)
                {
                    var utf8Code = triple >> (16 - 8 * (j % 3));
                    triple -= utf8Code << (16 - 8 * (j % 3));
                    if (utf8Code != 0) utf8CodeCollection.Add((int)utf8Code);
                }
            }

            var decode = "";
            for (var i = 0; i < utf8CodeCollection.Count; i++)
            {
                var code = utf8CodeCollection[i];
                var encoding = new UTF8Encoding(true, true);
                if (code <= 0xEF && code >= 0xE0)
                {
                    var codeTmp = 0;
                    for (var j = 0; j < OriginalBytesOfUnit; j++, i++)
                    {
                        codeTmp += utf8CodeCollection[i] << (16 - 8 * (j % 3));
                    }
                    decode += encoding.GetString(HexToByte(codeTmp.ToString("X3")));
                    i--;
                }
                else
                {
                    decode += encoding.GetString(HexToByte(code.ToString("X1")));
                }
            }
            return decode;
        }

        public string Encode(string plaintext)
        {
            var encoding = new UTF8Encoding(true, true);
            var lenOfInput = plaintext.Length;
            var data = plaintext.ToCharArray();
            var encode = "";
            var utf8CodeCollection = new List<int>();
            for (var i = 0; i < lenOfInput; i++)
            {
                var utf8CodeUnit = encoding.GetBytes(data[i].ToString());
                utf8CodeCollection.AddRange(utf8CodeUnit.Select(code => (int)code));
            }
            var triple = 0;
            for (var i = 0; i < utf8CodeCollection.Count; i++)
            {
                triple += utf8CodeCollection[i] << (16 - 8 * (i % 3));
                if (i % 3 == 2 || i == utf8CodeCollection.Count - 1)
                {
                    for (var j = Base64OfUnit - 1; j >= 0; j--)
                    {
                        encode += EncodingTable[(triple >> j * 6) & 0x3F];
                    }
                    triple = 0;
                }
            }
            return encode;
        }

        public byte[] HexToByte(string hexString)
        {
            //運算後的位元組長度:16進位數字字串長/2
            var byteOut = new byte[hexString.Length / 2];
            for (var i = 0; i < hexString.Length; i = i + 2)
            {
                //每2位16進位數字轉換為一個10進位整數
                byteOut[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }
            return byteOut;
        }
    }
}