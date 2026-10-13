using System.Text;
using CtaToybox.Core;

static class TextCodecTests
{
    public static void Run()
    {
        // Latin-1: 0x80 이상의 바이트가 그대로 돌아온다
        byte[] latin = { (byte)'{', (byte)'a', 0xE9, 0x85, 0xA0, 0xFF, (byte)'}' };
        string text = TextCodec.Decode(latin, out TextKind kind);
        T.Eq(TextKind.Latin1, kind, "no BOM is Latin-1");
        T.Eq(7, text.Length, "Latin-1 keeps one char per byte");
        T.Eq(latin, TextCodec.Encode(text, kind), "Latin-1 round-trips byte for byte");

        byte[] utf8 = { 0xEF, 0xBB, 0xBF, (byte)'{', 0xED, 0x95, 0x9C, (byte)'}' };
        text = TextCodec.Decode(utf8, out kind);
        T.Eq(TextKind.Utf8Bom, kind, "UTF-8 BOM is detected");
        T.Eq("{한}", text, "UTF-8 text is decoded without the BOM");
        T.Eq(utf8, TextCodec.Encode(text, kind), "UTF-8 BOM round-trips");

        byte[] utf16 = Encoding.Unicode.GetPreamble();
        utf16 = Concat(utf16, Encoding.Unicode.GetBytes("{a 1}"));
        text = TextCodec.Decode(utf16, out kind);
        T.Eq(TextKind.Utf16LeBom, kind, "UTF-16 LE BOM is detected");
        T.Eq("{a 1}", text, "UTF-16 text is decoded without the BOM");
        T.Eq(utf16, TextCodec.Encode(text, kind), "UTF-16 round-trips");

        T.Eq(new byte[0], TextCodec.Encode(TextCodec.Decode(new byte[0], out kind), kind), "empty file round-trips");
    }

    static byte[] Concat(byte[] a, byte[] b)
    {
        var r = new byte[a.Length + b.Length];
        a.CopyTo(r, 0); b.CopyTo(r, a.Length);
        return r;
    }
}
