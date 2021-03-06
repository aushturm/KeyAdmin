﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace KeyAdmin.Model
{
    public static class PasswordHandler
    {
        public static SecureString Entropy { set; private get; }

        public static string EncryptString(this SecureString input, SecureString entropy)
        {
            if (entropy == null && Entropy == null)
                return null;
            byte[] encryptedData;
            if (entropy == null)
            {
                encryptedData = ProtectedData.Protect(
                    Encoding.Unicode.GetBytes(ToInsecureString(input)),
                    Encoding.Unicode.GetBytes(ToInsecureString(Entropy)),
                    DataProtectionScope.CurrentUser);
            }
            else
            {
                encryptedData = ProtectedData.Protect(
                    Encoding.Unicode.GetBytes(ToInsecureString(input)),
                    Encoding.Unicode.GetBytes(ToInsecureString(entropy)),
                    DataProtectionScope.CurrentUser);
            }
            return Convert.ToBase64String(encryptedData);
        }

        public static SecureString DecryptString(this string encryptedData, SecureString entropy)
        {
            try
            {
                byte[] decryptedData;
                if (entropy == null)
                {
                    decryptedData = ProtectedData.Unprotect(
                    Convert.FromBase64String(encryptedData),
                    Encoding.Unicode.GetBytes(ToInsecureString(Entropy)),
                    DataProtectionScope.CurrentUser);
                }
                else
                {
                    decryptedData = ProtectedData.Unprotect(
                    Convert.FromBase64String(encryptedData),
                    Encoding.Unicode.GetBytes(ToInsecureString(entropy)),
                    DataProtectionScope.CurrentUser);
                }
                return ToSecureString(Encoding.Unicode.GetString(decryptedData));
            }
            catch
            {
                return null;
            }
        }

        public static SecureString ToSecureString(this string input)
        {
            SecureString secure = new SecureString();
            foreach (char c in input)
            {
                secure.AppendChar(c);
            }
            secure.MakeReadOnly();
            return secure;
        }

        public static string ToInsecureString(this SecureString input)
        {
            string returnValue = string.Empty;
            IntPtr ptr = Marshal.SecureStringToBSTR(input);
            try
            {
                returnValue = Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                Marshal.ZeroFreeBSTR(ptr);
            }
            return returnValue;
        }

        public static bool SecureStringEqual(this SecureString pw1, SecureString pw2)
        {
            if (pw1 == null)
            {
                throw new ArgumentNullException("s1");
            }
            if (pw2 == null)
            {
                throw new ArgumentNullException("s2");
            }

            if (pw1.Length != pw2.Length)
            {
                return false;
            }

            IntPtr ss_bstr1_ptr = IntPtr.Zero;
            IntPtr ss_bstr2_ptr = IntPtr.Zero;

            try
            {
                ss_bstr1_ptr = Marshal.SecureStringToBSTR(pw1);
                ss_bstr2_ptr = Marshal.SecureStringToBSTR(pw2);

                string str1 = Marshal.PtrToStringBSTR(ss_bstr1_ptr);
                string str2 = Marshal.PtrToStringBSTR(ss_bstr2_ptr);

                return str1.Equals(str2);
            }
            finally
            {
                if (ss_bstr1_ptr != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(ss_bstr1_ptr);
                }

                if (ss_bstr2_ptr != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(ss_bstr2_ptr);
                }
            }
        }
    }
}
