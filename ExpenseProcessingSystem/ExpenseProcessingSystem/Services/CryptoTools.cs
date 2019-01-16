namespace ExpenseProcessingSystem.Services
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public class CryptoTools
    {
        private static int STRETCH_COUNT = 1000;
        private static readonly string FirstEncrKey = "SmPsMM17";
        private static readonly string ScndEncrKey = "これはセキュリティのためのきーです. This is a key for security.";
        private static readonly byte[] salt = new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 };

        /// <summary>
        /// パスワードハッシュ取得(SHA512)
        /// </summary>
        /// <param name="siteCd">現場コード</param>
        /// <param name="staffNo">スタフNO</param>
        /// <param name="passwd">パスワード</param>
        /// <returns>暗号化パスワード</returns>
        public static string getHashPasswd(string siteCd, string staffNo, string passwd)
        {
            string salt = GetSha512(siteCd + staffNo);
            //Debug.WriteLine("salt:\"" + salt + "\"");
            string hash = "";

            for (int i = 0; i < STRETCH_COUNT; i++)
            {
                //Debug.WriteLine("Input Str Idx(" + i + ") :\"" + hash + salt + passwd + "\"");
                hash = GetSha512(hash + salt + passwd);
                //Debug.WriteLine("Output Hash Idx(" + i + ") :\"" + hash + "\"");
            }

            return hash;
        }

        /// <summary>
        /// Saltを取得する
        /// </summary>
        /// <param name="target"></param>
        /// <returns>Salt</returns>
        private static string GetSha512(string target)
        {
            SHA512 crypto = null;
            byte[] hashValue = { };
            try
            {
                // テキストをUTF-8エンコードでバイト配列化
                byte[] byteValue = Encoding.UTF8.GetBytes(target);

                // SHA512のハッシュ値を取得する
                crypto = SHA512Managed.Create();
                hashValue = crypto.ComputeHash(byteValue);
            }
            finally
            {
                crypto.Clear();
            }

            // バイト配列をUTF8エンコードで文字列化
            StringBuilder hashedText = new StringBuilder();
            for (int i = 0; i < hashValue.Length; i++)
            {
                hashedText.AppendFormat("{0:X2}", hashValue[i]);
            }

            return hashedText.ToString();
        }

        /// <summary>
        /// 第2暗号化を行う
        /// </summary>
        /// <param name="encryptString">文字列</param>
        /// <returns>暗号</returns>
        public static string SecondaryEncryption(string encryptString)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(FirstEncrKey, salt);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
            }

            return encryptString;
        }

        /// <summary>
        /// 第2復号化を行う
        /// </summary>
        /// <param name="cipherText">暗号</param>
        /// <returns>文字列</returns>
        public static string SecondaryDecryption(string cipherText)
        {
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(FirstEncrKey, salt);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        /// <summary>
        /// 第3暗号化を行う
        /// </summary>
        /// <param name="encryptString">文字列</param>
        /// <returns>暗号</returns>
        public static string ThirdEncryption(string encryptString)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(ScndEncrKey, salt);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptString;
        }

        /// <summary>
        /// 第3復号化を行う
        /// </summary>
        /// <param name="cipherText">暗号</param>
        /// <returns>文字列</returns>
        public static string ThirdDecryption(string cipherText)
        {
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(ScndEncrKey, salt);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
    }
}
