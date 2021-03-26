﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Mesen.GUI.Interop
{
	class TestApi
	{
		private const string DllPath = "MesenSCore.dll";

		[DllImport(DllPath)] public static extern Int32 RunRecordedTest([MarshalAs(UnmanagedType.LPUTF8Str)]string filename, [MarshalAs(UnmanagedType.I1)]bool inBackground);
		[DllImport(DllPath)] public static extern void RomTestRecord([MarshalAs(UnmanagedType.LPUTF8Str)]string filename, [MarshalAs(UnmanagedType.I1)]bool reset);
		[DllImport(DllPath)] public static extern void RomTestStop();
		[DllImport(DllPath)] [return: MarshalAs(UnmanagedType.I1)] public static extern bool RomTestRecording();
	}
}