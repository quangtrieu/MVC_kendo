//******************************************************************************
//Description: Contains Actions and functions for SecurityMethod
//Remarks: SecurityMethod
//Author : HiepNV
//Copyright(C) 2015 Seta International VietNam. All right reserved.
//******************************************************************************

using System;
using System.Security.Cryptography;
using System.Text;

namespace BCS.Framework.SecurityServices.Utils
{
    public static class SecurityMethod
    {
        public static string RandomString(int length)
        {
            const string str = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int strlen = str.Length;
            var rnd = new Random();
            string retVal = String.Empty;

            for (int i = 0; i < length; i++)
                retVal += str[rnd.Next(strlen)];

            return retVal;
        }

        public static string MD5Encrypt(string plainText)
        {
            var encoder = new UTF8Encoding();
            var hasher = new MD5CryptoServiceProvider();

            byte[] data = encoder.GetBytes(plainText);
            byte[] output = hasher.ComputeHash(data);

            return BitConverter.ToString(output).Replace("-", "").ToLower();
        }

        public static string RandomPassword()
        {
            string retVal = String.Empty;
            var rd = new Random(DateTime.Now.Millisecond);
            for (int i = 1; i < 10; i++)
            {
                retVal += rd.Next(0, 9);
            }
            return retVal;
        }
       
        public static string Base64Encode(string input)
        {
            byte[] toEncodeAsBytes = Encoding.ASCII.GetBytes(input);

            return Convert.ToBase64String(toEncodeAsBytes);
        }
        
        public static string Base64Decode(string input)
        {
            byte[] encodedDataAsBytes = Convert.FromBase64String(input);
            return Encoding.ASCII.GetString(encodedDataAsBytes);
        }
        
        public static string StripPunctuation(string html, bool retainSpace)
        {

            //Create Regular Expression objects
            const string punctuationMatch = "[~!#\\$%\\^&*\\(\\)-+=\\{\\[\\}\\]\\|;:\\x22'<,>\\.\\?\\\\\\t\\r\\v\\f\\n]";
            var afterRegEx = new System.Text.RegularExpressions.Regex(punctuationMatch + "\\s");
            var beforeRegEx = new System.Text.RegularExpressions.Regex("\\s" + punctuationMatch);

            //Define return string
            string retHTML = html;
            //Make sure any punctuation at the end of the String is removed

            //Set up Replacement String
            string repString = null;
            repString = retainSpace ? " " : "";

            while (beforeRegEx.IsMatch(retHTML))
            {
                //Strip punctuation from beginning of word
                retHTML = beforeRegEx.Replace(retHTML, repString);
            }

            while (afterRegEx.IsMatch(retHTML))
            {
                //Strip punctuation from end of word
                retHTML = afterRegEx.Replace(retHTML, repString);
            }

            // Return modified string
            return retHTML;
        }

        //public class DesEncrypter
        //{
        //    Cipher ecipher;
        //    Cipher dcipher;

        //    // 8-byte Salt
        //    private byte[] salt =
        //    {
        //        (byte) 0xA9, (byte) 0x9B, (byte) 0xC8, (byte) 0x32,
        //        (byte) 0x56, (byte) 0x35, (byte) 0xE3, (byte) 0x03
        //    };

        //    // Iteration count
        //    int iterationCount = 19;


        //    public DesEncrypter(String passPhrase)
        //    {
        //        try
        //        {
        //            // Create the key
        //            KeySpec keySpec = new PBEKeySpec(passPhrase.toCharArray(),
        //            salt, iterationCount);
        //            SecretKey key =
        //            SecretKeyFactory.getInstance("PBEWithMD5AndDES").generateSecret(keySpec);
        //            ecipher = Cipher.getInstance(key.getAlgorithm());
        //            dcipher = Cipher.getInstance(key.getAlgorithm());

        //            // Prepare the parameter to the ciphers
        //            AlgorithmParameterSpec paramSpec = new
        //            PBEParameterSpec(salt, iterationCount);

        //            // Create the ciphers
        //            ecipher.init(Cipher.ENCRYPT_MODE, key, paramSpec);
        //            dcipher.init(Cipher.DECRYPT_MODE, key, paramSpec);
        //        }
        //        catch (Exception e)
        //        {
        //            e.printStackTrace();
        //        }
        //    }

        //    public String encrypt(String str)
        //    {
        //        try
        //        {
        //            // Encode the string into bytes using utf-8
        //            byte[] utf8 = str.getBytes("UTF8");

        //            // Encrypt
        //            byte[] enc = ecipher.doFinal(utf8);

        //            // Encode bytes to base64 to get a string
        //            return new sun.misc.BASE64Encoder().encode(enc);
        //        }
        //        catch (Exception e)
        //        {
        //            e.printStackTrace();
        //        }
        //        return null;
        //    }

        //    public String encryptHex(String str)
        //    {
        //        try
        //        {
        //            // Encode the string into bytes using utf-8
        //            byte[] utf8 = str.getBytes("UTF8");

        //            // Encrypt
        //            byte[] enc = ecipher.doFinal(utf8);

        //            return toHex(enc);
        //        }
        //        catch (Exception e)
        //        {
        //            e.printStackTrace();
        //        }
        //        return null;
        //    }

        //    public static String toHex(byte[] b)
        //    {
        //        if (b == null)
        //        {
        //            return null;
        //        }
        //        String hits = "0123456789ABCDEF";
        //        StringBuffer sb = new StringBuffer();

        //        for (int i = 0; i < b.length; i++)
        //        {
        //            int j = ((int)b) & 0xFF;

        //            char first = hits.charAt(j / 16);
        //            char second = hits.charAt(j % 16);

        //            sb.append(first);
        //            sb.append(second);
        //        }

        //        return sb.toString();
        //    }

        //    public String decrypt(String str)
        //    {
        //        try
        //        {
        //            // Decode base64 to get bytes
        //            byte[] dec = new
        //            sun.misc.BASE64Decoder().decodeBuffer(str);

        //            // Decrypt
        //            byte[] utf8 = dcipher.doFinal(dec);

        //            // Decode using utf-8
        //            return new String(utf8, "UTF8");
        //        }
        //        catch (Exception e)
        //        {
        //            e.printStackTrace();
        //        }
        //        return null;
        //    }
        //}
    }
}
