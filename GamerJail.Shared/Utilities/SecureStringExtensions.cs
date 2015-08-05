using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace GamerJail.Shared.Utilities
{
    public static class SecureStringExtensions
    {
        public static bool SecureStringEqual(this SecureString s1, SecureString s2)
        {
            if (s1 == null)
                throw new ArgumentNullException(nameof(s1));

            if (s2 == null)
                throw new ArgumentNullException(nameof(s2));

            if (s1.Length != s2.Length)
            {
                return false;
            }

            IntPtr bstr1 = IntPtr.Zero;
            IntPtr bstr2 = IntPtr.Zero;

            RuntimeHelpers.PrepareConstrainedRegions();

            try
            {
                bstr1 = Marshal.SecureStringToBSTR(s1);
                bstr2 = Marshal.SecureStringToBSTR(s2);

                unsafe
                {
                    for (char* ptr1 = (char*)bstr1.ToPointer(), ptr2 = (char*)bstr2.ToPointer();
                        *ptr1 != 0 && *ptr2 != 0;
                         ++ptr1, ++ptr2)
                    {
                        if (*ptr1 != *ptr2)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
            finally
            {
                if (bstr1 != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(bstr1);
                }

                if (bstr2 != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(bstr2);
                }
            }
        }

        public static string SecureStringToString(this SecureString s)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(s);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

        public static SecureString ToSecureString(this string s)
        {
            var secure = new SecureString();
            foreach (char c in s)
                secure.AppendChar(c);

            return secure;
        }
    }
}