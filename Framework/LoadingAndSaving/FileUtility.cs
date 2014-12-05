using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#if !UNITY_WEBPLAYER && !UNITY_METRO

using System.IO;

#endif

namespace CraftingLegends.Framework
{
	public static class FileUtility
	{
		public static bool canLoadAndSaveFiles
		{
			get
			{
				if (Application.isWebPlayer)
					return false;

				return true;
			}
		}

#if !UNITY_WEBPLAYER && !UNITY_WINRT


		public static bool FileExists(string fileName)
		{
			return File.Exists(fileName);
        }

		public static void DeleteFile(string fileName)
		{
			File.Delete(fileName);
		}

		public static string ReadTextFile(string fileName)
		{
			return File.ReadAllText(fileName);
        }

		public static void WriteTextFile(string fileName, string content)
		{
			File.WriteAllText(fileName, content);
        }

#endif

#if UNITY_WEBPLAYER

		public static bool FileExists(string fileName)
		{
			return false;  // always return false; will not be called
		}

		public static void DeleteFile(string fileName)
		{
			// do nothing; will not be called
		}

		public static string ReadTextFile(string fileName)
		{
			return ""; // do nothing; will not be called
		}

		public static void WriteTextFile(string fileName, string content)
		{
			// do nothing; will not be called
		}

#endif

#if UNITY_METRO

		public static bool FileExists(string fileName)
		{
			return UnityEngine.Windows.File.Exists(fileName);
		}

		public static void DeleteFile(string fileName)
		{
			UnityEngine.Windows.File.Delete(fileName);
		}

		public static string ReadTextFile(string fileName)
		{
			var bytes = UnityEngine.Windows.File.ReadAllBytes(fileName);
			return System.Text.Encoding.UTF8.GetString(bytes, 0, bytes.Length);
		}

		public static void WriteTextFile(string fileName, string content)
		{
			var bytes = System.Text.Encoding.UTF8.GetBytes(content);
			UnityEngine.Windows.File.WriteAllBytes(fileName, bytes);
		}

#endif

#if UNITY_WP8

		public static bool FileExists(string fileName)
		{
			return UnityEngine.Windows.File.Exists(fileName);
		}

		public static void DeleteFile(string fileName)
		{
			UnityEngine.Windows.File.Delete(fileName);
		}

		public static string ReadTextFile(string fileName)
		{
			using (StreamReader reader = new StreamReader(fileName, System.Text.Encoding.UTF8))
			{
				return reader.ReadToEnd();
			}
		}

		public static void WriteTextFile(string fileName, string content)
		{
			using (StreamWriter writer = new StreamWriter(fileName, false, System.Text.Encoding.UTF8))
			{
				writer.Write(content);
			}
		}

#endif

	}
}