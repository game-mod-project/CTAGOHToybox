// 게임 텍스트 파일을 읽고 같은 인코딩으로 되쓴다.
// BOM이 없으면 Latin-1로 읽는다: 바이트 하나가 글자 하나라 고치지 않은 부분이 그대로 보존된다.
using System;
using System.Text;

namespace CtaToybox.Core
{
    public enum TextKind { Latin1, Utf8Bom, Utf16LeBom }

    public static class TextCodec
    {
        static readonly Encoding Latin1 = Encoding.Latin1;
        static readonly Encoding Utf8 = new UTF8Encoding(false);

        public static string Decode(byte[] bytes, out TextKind kind)
        {
            if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
            {
                kind = TextKind.Utf16LeBom;
                return Encoding.Unicode.GetString(bytes, 2, bytes.Length - 2);
            }
            if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            {
                kind = TextKind.Utf8Bom;
                return Utf8.GetString(bytes, 3, bytes.Length - 3);
            }
            kind = TextKind.Latin1;
            return Latin1.GetString(bytes);
        }

        public static byte[] Encode(string text, TextKind kind)
        {
            switch (kind)
            {
                case TextKind.Utf16LeBom: return Join(new byte[] { 0xFF, 0xFE }, Encoding.Unicode.GetBytes(text));
                case TextKind.Utf8Bom: return Join(new byte[] { 0xEF, 0xBB, 0xBF }, Utf8.GetBytes(text));
                default: return Latin1.GetBytes(text);
            }
        }

        // 그 인코딩으로 썼다가 읽으면 같은 글자가 돌아오는가. Latin-1 파일에는 U+00FF까지만 쓸 수 있다
        public static bool CanEncode(string text, TextKind kind)
        {
            byte[] bytes = Encode(text, kind);
            TextKind ignored;
            return kind == TextKind.Latin1 ? Latin1.GetString(bytes) == text : Decode(bytes, out ignored) == text;
        }

        static byte[] Join(byte[] a, byte[] b)
        {
            var r = new byte[a.Length + b.Length];
            Buffer.BlockCopy(a, 0, r, 0, a.Length);
            Buffer.BlockCopy(b, 0, r, a.Length, b.Length);
            return r;
        }
    }
}
