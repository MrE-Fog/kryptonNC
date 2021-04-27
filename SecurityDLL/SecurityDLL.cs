﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;


namespace Security{
    [ComVisible(true)]
    public class HSM
    {
        public HSM()
        {

        }
    }

    [ComVisible(true)]
    public class helper
    {
        public helper()
        {

        }
        
        [ComVisible(true)]
        public static Tuple<byte[], byte[]> AESEncrypt(byte[] text, byte[] key)
        {
            byte[] encrypted;
            var handle = GCHandle.Alloc(text, GCHandleType.Pinned);
            var hand = GCHandle.Alloc(key, GCHandleType.Pinned);
            // Create an Aes object
            // with the specified key and IV.
            byte[] iv;
            using (Aes myAes = Aes.Create())
            {
                iv = myAes.IV;
            }
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(text);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
                aesAlg.Clear();
            }
            var r = new Tuple<byte[], byte[]>(encrypted, iv);
            Array.Clear(text, 0, text.Length);
            Array.Clear(key, 0, key.Length);
            handle.Free();
            hand.Free();
            text = null;
            key = null;
            return r;

        }

        [ComVisible(true)]
        public static String AESDecrypt(Byte[] key, Byte[] thing, Byte[] IV)
        {
            var handle = GCHandle.Alloc(key, GCHandleType.Pinned);
            // Create an Aes object
            // with the specified key and IV.
            string plaintext;
            GCHandle hand;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(thing))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                            hand = GCHandle.Alloc(plaintext, GCHandleType.Pinned);

                        }
                    }
                }
                aesAlg.Clear();
            }

            Array.Clear(key, 0, key.Length);
            handle.Free();
            key = null;

            try
            {
                return plaintext;
            }
            finally
            {
                unsafe
                {
                    fixed (char* p = plaintext)
                    {
                        for (int i = 0; p[i] != '\0'; i++)
                        {
                            char* x = p + i;
                            *x = '0';
                        }
                    }
                }
                hand.Free();
                plaintext = null;
            }

        }
    }
}