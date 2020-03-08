using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RsaAndShaInUse
{
    class Program
    {
        static void Main(string[] args)
        {   
            //get user input & convert to bytes
            Console.WriteLine(" * type something to encrypt");
            string input = Console.ReadLine();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            //encryption with RSA
            byte[] rsaEncryptedBytes = RsaEncrypt(inputBytes);
            string rsaEncryptedString = Encoding.UTF8.GetString(rsaEncryptedBytes);
            Console.WriteLine("\n * Encrypted text:\n" + rsaEncryptedString);

            //decryption with RSA
            byte[] rsaDecryptedBytes = RsaDecrypt(rsaEncryptedBytes);
            string rsaDecryptedString = Encoding.UTF8.GetString(rsaDecryptedBytes);
            Console.WriteLine("\n * Decrypted text:\n" + rsaDecryptedString);

            //encryption with SHA256
            byte[] shaEncryptedBytes = ShaEncrypt(inputBytes);
            Console.WriteLine("\n * SHA256 Encryption:\n" + ShaStringBuilder(shaEncryptedBytes));
            
            //encryption with SHA256 & THEN RSA
            byte[] shaRsaEncrytedBytes = RsaEncrypt(shaEncryptedBytes);
            Console.WriteLine("\n *  Encryption with sha256 and THEN RSA:\n" + ShaStringBuilder(shaRsaEncrytedBytes));

            //decryption with SHA256 & THEN RSA
            byte[] shaRsaDecryptedBytes = RsaDecrypt(shaRsaEncrytedBytes);
            Console.WriteLine("\n * Decryption with sha256 and THEN RSA:\n" + ShaStringBuilder(shaRsaDecryptedBytes));

            //program has ended
            Console.ReadLine();
        }

        //Converts SHA256 byte[] to string
        static string ShaStringBuilder(byte[] bytesToRead)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var i in bytesToRead)
            {
                stringBuilder.Append(i.ToString("x2"));
            }
            return stringBuilder.ToString();
        }

        static byte[] ShaEncrypt(byte[] ct)
        {
            SHA256Managed hashstring = new SHA256Managed();
            return hashstring.ComputeHash(ct);
        }

        static byte[] RsaEncrypt(byte[] textBytes)
        {
            //get PublicKey pem File
            PemReader KeyTextReader = new PemReader(File.OpenText(@"C:\Users\x\Documents\PublicKey.pem"));
            RsaKeyParameters publicKey =  KeyTextReader.ReadObject() as RsaKeyParameters;

            //encrypt byte array
            IAsymmetricBlockCipher encryptCipher = new OaepEncoding(new RsaEngine());
            encryptCipher.Init(true, publicKey);
            return encryptCipher.ProcessBlock(textBytes, 0, textBytes.Length);
        }

        static byte[] RsaDecrypt(byte[] ct)
        {
            //get private key pem file
            AsymmetricCipherKeyPair keyPair;
            StreamReader reader = File.OpenText(@"C:\Users\x\Documents\PrivateKey.pem");
            keyPair = (AsymmetricCipherKeyPair)new PemReader(reader).ReadObject();
            RsaKeyParameters privateKey = keyPair.Private as RsaKeyParameters;

            //decrypt byte array
            IAsymmetricBlockCipher decryptCipher = new OaepEncoding(new RsaEngine());
            decryptCipher.Init(false, privateKey);
            return decryptCipher.ProcessBlock(ct, 0, ct.Length);
        }
    }
}