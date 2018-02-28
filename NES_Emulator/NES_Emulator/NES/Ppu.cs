﻿using System;
namespace NES_Emulator.NES
{
    public class Ppu
    {
        /*PPUメモリマップ
         * アドレス        サイズ
         * 0x0000～0x0FFF 0x1000 パターンテーブル0
         * 0x1000～0x1FFF 0x1000 パターンテーブル1
         * 0x2000～0x23BF 0x03C0 ネームテーブル0
         * 0x23C0～0x23FF 0x0040 属性テーブル0
         * 0x2400～0x27BF 0x03C0 ネームテーブル1
         * 0x27C0～0x27FF 0x0040 属性テーブル1
         * 0x2800～0x2BBF 0x03C0 ネームテーブル2
         * 0x2BC0～0x2BFF 0x0040 属性テーブル2
         * 0x2C00～0x2FBF 0x03C0 ネームテーブル3
         * 0x2FC0～0x2FFF 0x0040 属性テーブル3
         * 0x3000～0x3EFF        0x2000-0x2EFFのミラー
         * 0x3F00～0x3F0F 0x0010 バックグラウンドパレット
         * 0x3F10～0x3F1F 0x0010 スプライトパレット
         * => 0x3F00 背景色
         *    0x3F01〜0x3F03 バックグラウンドパレット0
         *    0x3F04 空き領域
         *    0x3F05〜0x3F07 バックグラウンドパレット1
         *    0x3F08 空き領域
         *    0x3F09〜0x3F0B バックグラウンドパレット2
         *    0x3F0C 空き領域
         *    0x3F0D〜0x3F0F バックグラウンドパレット3
         *    0x3F10 0x3F00のミラー
         *    0x3F11〜0x3F13 スプライトパレット0
         *    0x3F14 0x3F04のミラー
         *    0x3F15〜0x3F17 スプライトパレット1
         *    0x3F18 0x3F08のミラー
         *    0x3F19〜0x3F1B スプライトパレット2
         *    0x3F1C 0x3F0Cのミラー
         *    0x3F1D〜0x3F1F スプライトパレット3
         * 0x3F20～0x3FFF        0x3F00-0x3F1Fのミラー
         */
        byte[] ppuAddress;
        byte[,,] sprite; //Sprite保存用
        Rom rom;

        //パレットカラー
        byte[][] paletteColors;


        public Ppu(Rom rom)
        {
            ppuAddress = new byte[0x4000];
            this.rom = rom;
            sprite = new byte[rom.CharacterRom.Length / 16, 8, 8];
            paletteColors = new byte[][]
            {
                new byte[]{ 0x75, 0x75, 0x75 },
                new byte[]{ 0x27, 0x1B, 0x8F }, new byte[]{ 0x00, 0x00, 0xAB },
                new byte[]{ 0x47, 0x00, 0x9F }, new byte[]{ 0x8F, 0x00, 0x77 },
                new byte[]{ 0xAB, 0x00, 0x13 }, new byte[]{ 0xA7, 0x00, 0x00 },
                new byte[]{ 0x7F, 0x0B, 0x00 }, new byte[]{ 0x43, 0x2F, 0x00 },
                new byte[]{ 0x00, 0x47, 0x00 }, new byte[]{ 0x00, 0x51, 0x00 },
                new byte[]{ 0x00, 0x3F, 0x17 }, new byte[]{ 0x1B, 0x3F, 0x5F },
                new byte[]{ 0x00, 0x00, 0x00 }, new byte[]{ 0x05, 0x05, 0x05 },
                new byte[]{ 0x05, 0x05, 0x05 },

                new byte[]{ 0xBC, 0xBC, 0xBC }, new byte[]{ 0x00, 0x73, 0xEF },
                new byte[]{ 0x23, 0x3B, 0xEF }, new byte[]{ 0x83, 0x00, 0xF3 },
                new byte[]{ 0xBF, 0x00, 0xBF }, new byte[]{ 0xE7, 0x00, 0x5B },
                new byte[]{ 0xDB, 0x2B, 0x00 }, new byte[]{ 0xCB, 0x4F, 0x0F },
                new byte[]{ 0x8B, 0x73, 0x00 }, new byte[]{ 0x00, 0x97, 0x00 },
                new byte[]{ 0x00, 0xAB, 0x00 }, new byte[]{ 0x00, 0x93, 0x3B },
                new byte[]{ 0x00, 0x83, 0x8B }, new byte[]{ 0x11, 0x11, 0x11 },
                new byte[]{ 0x09, 0x09, 0x09 }, new byte[]{ 0x09, 0x09, 0x09 },

                new byte[]{ 0xFF, 0xFF, 0xFF }, new byte[]{ 0x3F, 0xBF, 0xFF },
                new byte[]{ 0x5F, 0x97, 0xFF }, new byte[]{ 0xA7, 0x8B, 0xFD },
                new byte[]{ 0xF7, 0x7B, 0xFF }, new byte[]{ 0xFF, 0x77, 0xB7 },
                new byte[]{ 0xFF, 0x77, 0x63 }, new byte[]{ 0xFF, 0x9B, 0x3B },
                new byte[]{ 0xF3, 0xBF, 0x3F }, new byte[]{ 0x83, 0xD3, 0x13 },
                new byte[]{ 0x4F, 0xDF, 0x4B }, new byte[]{ 0x58, 0xF8, 0x98 },
                new byte[]{ 0x00, 0xEB, 0xDB }, new byte[]{ 0x66, 0x66, 0x66 },
                new byte[]{ 0x0D, 0x0D, 0x0D }, new byte[]{ 0x0D, 0x0D, 0x0D },

                new byte[]{ 0xFF, 0xFF, 0xFF }, new byte[]{ 0xAB, 0xE7, 0xFF },
                new byte[]{ 0xC7, 0xD7, 0xFF }, new byte[]{ 0xD7, 0xCB, 0xFF },
                new byte[]{ 0xFF, 0xC7, 0xFF }, new byte[]{ 0xFF, 0xC7, 0xDB },
                new byte[]{ 0xFF, 0xBF, 0xB3 }, new byte[]{ 0xFF, 0xDB, 0xAB },
                new byte[]{ 0xFF, 0xE7, 0xA3 }, new byte[]{ 0xE3, 0xFF, 0xA3 },
                new byte[]{ 0xAB, 0xF3, 0xBF }, new byte[]{ 0xB3, 0xFF, 0xCF },
                new byte[]{ 0x9F, 0xFF, 0xF3 }, new byte[]{ 0xDD, 0xDD, 0xDD },
                new byte[]{ 0x11, 0x11, 0x11 }, new byte[]{ 0x11, 0x11, 0x11 }
            };

        }

        public void WriteMemory(ushort address, byte value)
        {
            ppuAddress[address] = value;
        }

        /// <summary>
        /// スプライトを読み込み保存
        /// </summary>
        public void LoadSprite()
        {
            int count = 0;
            for (int i = 0;i < sprite.GetLength(0);i++)
            {
                for (int j = i * 16; j < i * 16 + 8; j++)
                {
                    string highOrder = BinaryNumberConversion(rom.CharacterRom[j + 8]);
                    string lowOrder = BinaryNumberConversion(rom.CharacterRom[j]);
                    for (int l = 0;l < 8; l++)
                    {
                        sprite[i, count, l] = (byte)(int.Parse(highOrder[l].ToString()) * 2 + int.Parse(lowOrder[l].ToString()));
                    }
                    count++;
                }
                count = 0;
            }
        }

        /// <summary>
        /// 8bit変換
        /// </summary>
        /// <returns>8bit</returns>
        /// <param name="originNumber">元値</param>
        string BinaryNumberConversion(byte originNumber)
        {
            string convertNumber = Convert.ToString(originNumber, 2);
            if (convertNumber.Length < 8)
            {
                for (int i = 0;i < 8 - convertNumber.Length;i++)
                {
                    convertNumber = convertNumber.Insert(0, "0");
                }
            }
            return convertNumber;
        }
    }
}
