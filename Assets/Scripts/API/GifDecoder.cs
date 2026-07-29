using System;
using System.Collections.Generic;
using UnityEngine;

public class GifDecodeResult
{
    public Sprite[] Frames;
    public float[] Delays;
}

public static class GifDecoder
{
    public static GifDecodeResult Decode(byte[] data)
    {
        if (data == null || data.Length < 13)
            return null;

        try
        {
            return DecodeInternal(data);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"GIF decode failed: {e.Message}");
            return null;
        }
    }

    private static GifDecodeResult DecodeInternal(byte[] data)
    {
        int pos = 0;

        if (data[0] != 'G' || data[1] != 'I' || data[2] != 'F')
            return null;

        pos = 6;

        int width = ReadUInt16(data, ref pos);
        int height = ReadUInt16(data, ref pos);
        byte packed = data[pos++];
        pos++; // background color index
        pos++; // pixel aspect ratio

        bool hasGlobalColorTable = (packed & 0x80) != 0;
        int globalColorTableSize = 2 << (packed & 0x07);
        Color32[] globalColorTable = null;

        if (hasGlobalColorTable)
            globalColorTable = ReadColorTable(data, ref pos, globalColorTableSize);

        Color32[] canvas = new Color32[width * height];
        Color32[] previous = new Color32[width * height];

        List<Sprite> frames = new List<Sprite>();
        List<float> delays = new List<float>();

        int transparentIndex = -1;
        int frameDisposal = 0;
        float delaySeconds = 0.1f;

        int previousDisposal = 0;
        int previousLeft = 0;
        int previousTop = 0;
        int previousFrameWidth = 0;
        int previousFrameHeight = 0;

        while (pos < data.Length)
        {
            byte block = data[pos++];

            if (block == 0x3B)
                break;

            if (block == 0x21)
            {
                byte label = data[pos++];

                if (label == 0xF9)
                {
                    pos++; // block size
                    byte gcePacked = data[pos++];
                    int delayCs = ReadUInt16(data, ref pos);
                    transparentIndex = data[pos++];
                    pos++; // block terminator

                    frameDisposal = (gcePacked >> 2) & 0x07;
                    bool hasTransparency = (gcePacked & 0x01) != 0;
                    if (!hasTransparency)
                        transparentIndex = -1;

                    delaySeconds = Mathf.Max(delayCs / 100f, 0.02f);
                }
                else
                {
                    SkipDataSubBlocks(data, ref pos);
                }

                continue;
            }

            if (block != 0x2C)
                continue;

            int left = ReadUInt16(data, ref pos);
            int top = ReadUInt16(data, ref pos);
            int frameWidth = ReadUInt16(data, ref pos);
            int frameHeight = ReadUInt16(data, ref pos);
            byte imagePacked = data[pos++];

            bool hasLocalColorTable = (imagePacked & 0x80) != 0;
            bool interlace = (imagePacked & 0x40) != 0;
            int localColorTableSize = 2 << (imagePacked & 0x07);

            Color32[] colorTable = globalColorTable;
            if (hasLocalColorTable)
                colorTable = ReadColorTable(data, ref pos, localColorTableSize);

            if (colorTable == null)
                return null;

            if (previousDisposal == 2)
            {
                ClearRect(
                    canvas,
                    width,
                    height,
                    previousLeft,
                    previousTop,
                    previousFrameWidth,
                    previousFrameHeight);
            }
            else if (previousDisposal == 3)
            {
                Array.Copy(previous, canvas, canvas.Length);
            }

            if (frameDisposal == 3)
                Array.Copy(canvas, previous, canvas.Length);

            byte lzwMinCodeSize = data[pos++];
            byte[] compressed = ReadDataSubBlocks(data, ref pos);
            byte[] indices = LzwDecode(compressed, lzwMinCodeSize, frameWidth * frameHeight);

            ApplyFrame(
                canvas,
                indices,
                colorTable,
                width,
                height,
                left,
                top,
                frameWidth,
                frameHeight,
                transparentIndex,
                interlace);

            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.SetPixels32(FlipVertically(canvas, width, height));
            texture.Apply(false, false);
            texture.filterMode = FilterMode.Point;

            frames.Add(Sprite.Create(
                texture,
                new Rect(0, 0, width, height),
                new Vector2(0.5f, 0f),
                100f));
            delays.Add(delaySeconds);

            previousDisposal = frameDisposal;
            previousLeft = left;
            previousTop = top;
            previousFrameWidth = frameWidth;
            previousFrameHeight = frameHeight;

            frameDisposal = 0;
            transparentIndex = -1;
            delaySeconds = 0.1f;
        }

        if (frames.Count == 0)
            return null;

        return new GifDecodeResult
        {
            Frames = frames.ToArray(),
            Delays = delays.ToArray()
        };
    }

    private static void ClearRect(
        Color32[] canvas,
        int canvasWidth,
        int canvasHeight,
        int left,
        int top,
        int rectWidth,
        int rectHeight)
    {
        for (int y = 0; y < rectHeight; y++)
        {
            int dstY = top + y;
            if (dstY < 0 || dstY >= canvasHeight)
                continue;

            for (int x = 0; x < rectWidth; x++)
            {
                int dstX = left + x;
                if (dstX < 0 || dstX >= canvasWidth)
                    continue;

                canvas[dstY * canvasWidth + dstX] = new Color32(0, 0, 0, 0);
            }
        }
    }

    private static void ApplyFrame(
        Color32[] canvas,
        byte[] indices,
        Color32[] colorTable,
        int canvasWidth,
        int canvasHeight,
        int left,
        int top,
        int frameWidth,
        int frameHeight,
        int transparentIndex,
        bool interlace)
    {
        int[] rowOffsets = interlace
            ? BuildInterlaceRows(frameHeight)
            : null;

        for (int row = 0; row < frameHeight; row++)
        {
            int dstYOffset = interlace ? rowOffsets[row] : row;

            for (int x = 0; x < frameWidth; x++)
            {
                int srcIndex = row * frameWidth + x;
                if (srcIndex < 0 || srcIndex >= indices.Length)
                    continue;

                int colorIndex = indices[srcIndex];
                if (colorIndex == transparentIndex)
                    continue;

                int dstX = left + x;
                int dstY = top + dstYOffset;
                if (dstX < 0 || dstY < 0 || dstX >= canvasWidth || dstY >= canvasHeight)
                    continue;

                if (colorIndex < 0 || colorIndex >= colorTable.Length)
                    continue;

                canvas[dstY * canvasWidth + dstX] = colorTable[colorIndex];
            }
        }
    }

    private static int[] BuildInterlaceRows(int height)
    {
        List<int> rows = new List<int>(height);
        for (int y = 0; y < height; y += 8) rows.Add(y);
        for (int y = 4; y < height; y += 8) rows.Add(y);
        for (int y = 2; y < height; y += 4) rows.Add(y);
        for (int y = 1; y < height; y += 2) rows.Add(y);

        int[] map = new int[height];
        for (int i = 0; i < rows.Count && i < height; i++)
            map[i] = rows[i];
        return map;
    }

    private static Color32[] FlipVertically(Color32[] source, int width, int height)
    {
        Color32[] flipped = new Color32[source.Length];
        for (int y = 0; y < height; y++)
        {
            int srcRow = y * width;
            int dstRow = (height - 1 - y) * width;
            Array.Copy(source, srcRow, flipped, dstRow, width);
        }

        return flipped;
    }

    private static Color32[] ReadColorTable(byte[] data, ref int pos, int count)
    {
        Color32[] table = new Color32[count];
        for (int i = 0; i < count; i++)
        {
            byte r = data[pos++];
            byte g = data[pos++];
            byte b = data[pos++];
            table[i] = new Color32(r, g, b, 255);
        }

        return table;
    }

    private static void SkipDataSubBlocks(byte[] data, ref int pos)
    {
        while (pos < data.Length)
        {
            int size = data[pos++];
            if (size == 0)
                return;
            pos += size;
        }
    }

    private static byte[] ReadDataSubBlocks(byte[] data, ref int pos)
    {
        List<byte> bytes = new List<byte>();
        while (pos < data.Length)
        {
            int size = data[pos++];
            if (size == 0)
                break;

            for (int i = 0; i < size; i++)
                bytes.Add(data[pos++]);
        }

        return bytes.ToArray();
    }

    private static int ReadUInt16(byte[] data, ref int pos)
    {
        int value = data[pos] | (data[pos + 1] << 8);
        pos += 2;
        return value;
    }

    private static byte[] LzwDecode(byte[] compressed, int minCodeSize, int expectedLength)
    {
        List<byte> output = new List<byte>(expectedLength);

        int clearCode = 1 << minCodeSize;
        int endCode = clearCode + 1;
        int codeSize = minCodeSize + 1;
        int nextCode = endCode + 1;

        Dictionary<int, List<byte>> dictionary = new Dictionary<int, List<byte>>();

        void ResetDictionary()
        {
            dictionary.Clear();
            for (int i = 0; i < clearCode; i++)
                dictionary[i] = new List<byte> { (byte)i };

            codeSize = minCodeSize + 1;
            nextCode = endCode + 1;
        }

        ResetDictionary();

        int bitPos = 0;

        int ReadCode()
        {
            int code = 0;
            for (int i = 0; i < codeSize; i++)
            {
                int byteIndex = bitPos / 8;
                if (byteIndex >= compressed.Length)
                    return endCode;

                int bit = (compressed[byteIndex] >> (bitPos % 8)) & 1;
                code |= bit << i;
                bitPos++;
            }

            return code;
        }

        int previousCode = -1;

        while (output.Count < expectedLength)
        {
            int code = ReadCode();
            if (code == endCode)
                break;

            if (code == clearCode)
            {
                ResetDictionary();
                previousCode = -1;
                continue;
            }

            List<byte> entry;
            if (dictionary.TryGetValue(code, out entry))
            {
                output.AddRange(entry);
            }
            else if (code == nextCode && previousCode >= 0)
            {
                entry = new List<byte>(dictionary[previousCode]);
                entry.Add(dictionary[previousCode][0]);
                output.AddRange(entry);
                dictionary[nextCode] = entry;
                nextCode++;
                previousCode = code;

                if (nextCode == (1 << codeSize) && codeSize < 12)
                    codeSize++;

                continue;
            }
            else
            {
                break;
            }

            if (previousCode >= 0 && nextCode < 4096)
            {
                List<byte> newEntry = new List<byte>(dictionary[previousCode]);
                newEntry.Add(entry[0]);
                dictionary[nextCode] = newEntry;
                nextCode++;

                if (nextCode == (1 << codeSize) && codeSize < 12)
                    codeSize++;
            }

            previousCode = code;
        }

        if (output.Count > expectedLength)
            output.RemoveRange(expectedLength, output.Count - expectedLength);

        return output.ToArray();
    }
}
