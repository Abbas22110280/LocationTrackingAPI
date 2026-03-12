using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
namespace LocationTrackingAPI.Models
{
    public class CommonBLL
    {
        public static string Encrypt(string plaintext)
        {
            string password = "rachnasagarpvtltdNDLD4583";
            string s1 = "NDLD4583securityrachnasagar";
            int iterations = 3124;
            string s2 = "0A1B2c3D4e5F6g7H8I9";
            int num = 256;
            byte[] bytes1 = Encoding.ASCII.GetBytes(s2);
            byte[] bytes2 = Encoding.ASCII.GetBytes(s1);
            byte[] bytes3 = Encoding.Unicode.GetBytes(plaintext);
            byte[] bytes4 = new Rfc2898DeriveBytes(password, bytes2, iterations).GetBytes(checked((int)Math.Round(unchecked((double)num / 8.0))));
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            rijndaelManaged.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = rijndaelManaged.CreateEncryptor(bytes4, bytes1);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(bytes3, 0, bytes3.Length);
            cryptoStream.FlushFinalBlock();
            byte[] array = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(array);
        }

        public static string Decrypt(string cipherText)
        {
            string password = "rachnasagarpvtltdNDLD4583";
            string s1 = "NDLD4583securityrachnasagar";
            int iterations = 3124;
            string s2 = "0A1B2c3D4e5F6g7H8I9";
            int num = 256;
            byte[] bytes1 = Encoding.ASCII.GetBytes(s2);
            byte[] bytes2 = Encoding.ASCII.GetBytes(s1);
            byte[] buffer = Convert.FromBase64String(cipherText);
            byte[] bytes3 = new Rfc2898DeriveBytes(password, bytes2, iterations).GetBytes(checked((int)Math.Round(unchecked((double)num / 8.0))));
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            rijndaelManaged.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = rijndaelManaged.CreateDecryptor(bytes3, bytes1);
            MemoryStream memoryStream = new MemoryStream(buffer);
            CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] numArray = new byte[checked(buffer.Length + 1)];
            int count = cryptoStream.Read(numArray, 0, numArray.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.Unicode.GetString(numArray, 0, count);
        }

        public static string EncodeString(string value)
        {
            string s = "rachnasagardilipNDLD4583";
            MACTripleDES macTripleDes = new MACTripleDES();
            MD5CryptoServiceProvider cryptoServiceProvider = new MD5CryptoServiceProvider();
            macTripleDes.Key = cryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(s));
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value)) + "-" + Convert.ToBase64String(macTripleDes.ComputeHash(Encoding.UTF8.GetBytes(value)));
        }

        public static string DecodeString(string value)
        {
            string s1 = "rachnasagardilipNDLD4583";
            MACTripleDES macTripleDes = new MACTripleDES();
            MD5CryptoServiceProvider cryptoServiceProvider = new MD5CryptoServiceProvider();
            macTripleDes.Key = cryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(s1));
            string s2;
            try
            {
                string[] strArray = value.Split('-');
                s2 = Encoding.UTF8.GetString(Convert.FromBase64String(strArray[0]));
                if ((object)Encoding.UTF8.GetString(Convert.FromBase64String(strArray[1].Replace(" ", "+"))) == (object)Encoding.UTF8.GetString(macTripleDes.ComputeHash(Encoding.UTF8.GetBytes(s2))))
                    throw new ArgumentException("Hash value does not match");
            }
            catch (Exception ex)
            {
                string str = ex.Message;
                throw new ArgumentException("Invalid Argument");
            }
            return s2;
        }

        public static DataTable ToDataTable<T>(IList<T> data)// T is any generic type
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));

            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }

        public static string NumberToWords(Int64 number)
        {
            if (number == 0)
                return "Zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " Million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " Thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
                var tensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }

        public static string GetUniqueKey()
        {
            int num1 = 8;
            char[] chArray = new char[62];
            char[] charArray = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data1 = new byte[1];
            RNGCryptoServiceProvider cryptoServiceProvider = new RNGCryptoServiceProvider();
            cryptoServiceProvider.GetNonZeroBytes(data1);
            int capacity = num1;
            byte[] data2 = new byte[checked(capacity - 1 + 1)];
            cryptoServiceProvider.GetNonZeroBytes(data2);
            StringBuilder stringBuilder = new StringBuilder(capacity);
            byte[] numArray = data2;
            int index = 0;
            while (index < numArray.Length)
            {
                byte num2 = numArray[index];
                stringBuilder.Append(charArray[(int)num2 % checked(charArray.Length - 1)]);
                checked { ++index; }
            }
            return stringBuilder.ToString();
        }


    }
}