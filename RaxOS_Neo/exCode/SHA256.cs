using System;

namespace RaxOS_Neo.Crypt;
public static class Sha256
{
    private static readonly uint[] K = new uint[]
    {
        0x428a2f98,0x71374491,0xb5c0fbcf,0xe9b5dba5,
        0x3956c25b,0x59f111f1,0x923f82a4,0xab1c5ed5,
        0xd807aa98,0x12835b01,0x243185be,0x550c7dc3,
        0x72be5d74,0x80deb1fe,0x9bdc06a7,0xc19bf174,
        0xe49b69c1,0xefbe4786,0x0fc19dc6,0x240ca1cc,
        0x2de92c6f,0x4a7484aa,0x5cb0a9dc,0x76f988da,
        0x983e5152,0xa831c66d,0xb00327c8,0xbf597fc7,
        0xc6e00bf3,0xd5a79147,0x06ca6351,0x14292967,
        0x27b70a85,0x2e1b2138,0x4d2c6dfc,0x53380d13,
        0x650a7354,0x766a0abb,0x81c2c92e,0x92722c85,
        0xa2bfe8a1,0xa81a664b,0xc24b8b70,0xc76c51a3,
        0xd192e819,0xd6990624,0xf40e3585,0x106aa070,
        0x19a4c116,0x1e376c08,0x2748774c,0x34b0bcb5,
        0x391c0cb3,0x4ed8aa4a,0x5b9cca4f,0x682e6ff3,
        0x748f82ee,0x78a5636f,0x84c87814,0x8cc70208,
        0x90befffa,0xa4506ceb,0xbef9a3f7,0xc67178f2
    };
 
    public static byte[] Compute(byte[] data)
    {
        uint h0 = 0x6a09e667;
        uint h1 = 0xbb67ae85;
        uint h2 = 0x3c6ef372;
        uint h3 = 0xa54ff53a;
        uint h4 = 0x510e527f;
        uint h5 = 0x9b05688c;
        uint h6 = 0x1f83d9ab;
        uint h7 = 0x5be0cd19;

        ulong bitLen = (ulong)data.Length * 8;
        int padding = 64 - ((data.Length + 9) % 64);
        if (padding == 64) padding = 0;

        byte[] padded = new byte[data.Length + 9 + padding];
        Array.Copy(data, padded, data.Length);
        padded[data.Length] = 0x80;

        for (int i = 0; i < 8; i++)
            padded[padded.Length - 1 - i] = (byte)(bitLen >> (8 * i));

        uint[] w = new uint[64];

        for (int chunk = 0; chunk < padded.Length; chunk += 64)
        {
            for (int i = 0; i < 16; i++)
                w[i] = (uint)(
                    padded[chunk + i * 4] << 24 |
                    padded[chunk + i * 4 + 1] << 16 |
                    padded[chunk + i * 4 + 2] << 8 |
                    padded[chunk + i * 4 + 3]);

            for (int i = 16; i < 64; i++)
            {
                uint s0 = RotR(w[i - 15], 7) ^ RotR(w[i - 15], 18) ^ (w[i - 15] >> 3);
                uint s1 = RotR(w[i - 2], 17) ^ RotR(w[i - 2], 19) ^ (w[i - 2] >> 10);
                w[i] = w[i - 16] + s0 + w[i - 7] + s1;
            }

            uint a = h0, b = h1, c = h2, d = h3;
            uint e = h4, f = h5, g = h6, h = h7;

            for (int i = 0; i < 64; i++)
            {
                uint S1 = RotR(e, 6) ^ RotR(e, 11) ^ RotR(e, 25);
                uint ch = (e & f) ^ (~e & g);
                uint temp1 = h + S1 + ch + K[i] + w[i];
                uint S0 = RotR(a, 2) ^ RotR(a, 13) ^ RotR(a, 22);
                uint maj = (a & b) ^ (a & c) ^ (b & c);
                uint temp2 = S0 + maj;

                h = g; g = f; f = e;
                e = d + temp1;
                d = c; c = b; b = a;
                a = temp1 + temp2;
            }

            h0 += a; h1 += b; h2 += c; h3 += d;
            h4 += e; h5 += f; h6 += g; h7 += h;
        }

        byte[] hash = new byte[32];
        Write(hash, 0, h0); Write(hash, 4, h1);
        Write(hash, 8, h2); Write(hash, 12, h3);
        Write(hash, 16, h4); Write(hash, 20, h5);
        Write(hash, 24, h6); Write(hash, 28, h7);

        return hash;
    }

    static uint RotR(uint x, int n) => (x >> n) | (x << (32 - n));
    static void Write(byte[] b, int i, uint v)
    {
        b[i] = (byte)(v >> 24);
        b[i + 1] = (byte)(v >> 16);
        b[i + 2] = (byte)(v >> 8);
        b[i + 3] = (byte)v;
    }
}
