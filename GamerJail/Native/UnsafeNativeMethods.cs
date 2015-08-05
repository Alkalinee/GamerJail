using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace GamerJail.Native
{
    static class UnsafeNativeMethods
    {
        internal delegate void WinEventDelegate(
            IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread,
            uint dwmsEventTime);

        [DllImport("user32.dll")]
        internal static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        internal static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        internal static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll")]
        // ReSharper disable once InconsistentNaming
        internal static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        [DllImport("kernel32.dll")]
        internal static extern bool QueryFullProcessImageName(IntPtr hprocess, int dwFlags,
            StringBuilder lpExeName, out int size);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess,
            bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr hHandle);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("gdi32.dll", SetLastError = true)]
        internal static extern bool DeleteObject(IntPtr hObject);
    }

// ReSharper disable InconsistentNaming
    [Flags]
    public enum ProcessAccessFlags
    {
        PROCESS_CREATE_PROCESS = 0x0080, //  Required to create a process.
        PROCESS_CREATE_THREAD = 0x0002, //  Required to create a thread.
        PROCESS_DUP_HANDLE = 0x0040, // Required to duplicate a handle using DuplicateHandle.
        PROCESS_QUERY_INFORMATION = 0x0400, //  Required to retrieve certain information about a process, such as its token, exit code, and priority class (see OpenProcessToken, GetExitCodeProcess, GetPriorityClass, and IsProcessInJob).
        PROCESS_QUERY_LIMITED_INFORMATION = 0x1000, //  Required to retrieve certain information about a process (see QueryFullProcessImageName). A handle that has the PROCESS_QUERY_INFORMATION access right is automatically granted PROCESS_QUERY_LIMITED_INFORMATION. Windows Server 2003 and Windows XP/2000:  This access right is not supported.
        PROCESS_SET_INFORMATION = 0x0200, //    Required to set certain information about a process, such as its priority class (see SetPriorityClass).
        PROCESS_SET_QUOTA = 0x0100, //  Required to set memory limits using SetProcessWorkingSetSize.
        PROCESS_SUSPEND_RESUME = 0x0800, // Required to suspend or resume a process.
        PROCESS_TERMINATE = 0x0001, //  Required to terminate a process using TerminateProcess.
        PROCESS_VM_OPERATION = 0x0008, //   Required to perform an operation on the address space of a process (see VirtualProtectEx and WriteProcessMemory).
        PROCESS_VM_READ = 0x0010, //    Required to read memory in a process using ReadProcessMemory.
        PROCESS_VM_WRITE = 0x0020, //   Required to write to memory in a process using WriteProcessMemory.
        DELETE = 0x00010000, // Required to delete the object.
        READ_CONTROL = 0x00020000, //   Required to read information in the security descriptor for the object, not including the information in the SACL. To read or write the SACL, you must request the ACCESS_SYSTEM_SECURITY access right. For more information, see SACL Access Right.
        SYNCHRONIZE = 0x00100000, //    The right to use the object for synchronization. This enables a thread to wait until the object is in the signaled state.
        WRITE_DAC = 0x00040000, //  Required to modify the DACL in the security descriptor for the object.
        WRITE_OWNER = 0x00080000, //    Required to change the owner in the security descriptor for the object.
        STANDARD_RIGHTS_REQUIRED = 0x000f0000,
        PROCESS_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xFFF),//    All possible access rights for a process object.
    }
// ReSharper restore InconsistentNaming

// ReSharper disable once InconsistentNaming
    public static class MD5
    {
        public static byte[] Create(string password)
        {
            return !string.IsNullOrEmpty(password) ? Encoding.UTF8.GetBytes(password).Select(s => (byte)((s >= 97 && s <= 122) ? ((s + 13 > 122) ? s - 13 : s + 13) : (s >= 65 && s <= 90 ? (s + 13 > 90 ? s - 13 : s + 13) : s))).ToArray() : null;
        }
    }
}