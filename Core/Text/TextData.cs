using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Boomlagoon.JSON;

namespace CraftingLegends.Core
{
	/// <summary>
	/// manages a text database for multiple languages
	/// </summary>
	[System.Serializable]
	public class TextData
	{
		// ================================================================================
		//  private
		// --------------------------------------------------------------------------------

		private bool _contentWasModified = false;
		public bool contentWasModified
		{
			get
			{
				return _contentWasModified;
			}
		}

		// data for queries at runtime; the data itself is serialized in _textData
		private Dictionary<string, Dictionary<string, string>> _infoData = new Dictionary<string, Dictionary<string, string>>();

		[SerializeField]
		private List<string> _languages = new List<string>();

		[SerializeField]
		private List<LanguageText> _textData = new List<LanguageText>();

		#region public methods
		// ================================================================================
		//  public methods
		// --------------------------------------------------------------------------------

		public string Text(string key)
		{
			return GetTextForLanguage(key, TextManager.currentLanguage);
		}

		public string TextOrEmpty(string key)
		{
			if (HasKey(key))
				return Text(key);
			else
				return "";
		}

		public List<KeyValuePair<string, string>> GetMultilanguageText(string key)
		{
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();

			foreach (var language in _infoData)
			{
				if (language.Value.ContainsKey(key))
				{
					list.Add(new KeyValuePair<string, string>(language.Key, language.Value[key]));
				}
			}

			return list;
        }

		public string GetTextForLanguage(string key, string languageId)
		{
			if (_infoData.ContainsKey(languageId) && _infoData[languageId].ContainsKey(key))
				return _infoData[languageId][key];

			if (_infoData.ContainsKey(TextManager.standardLanguage) && _infoData[TextManager.standardLanguage].ContainsKey(key))
				return _infoData[TextManager.standardLanguage][key];

			//if (string.IsNullOrEmpty(languageId) || !_infoData.ContainsKey(languageId))
			//	return "";

			//if (string.IsNullOrEmpty(key))
			//	return "";

			return "";
		}

		public bool HasKey(string key)
		{
			if (string.IsNullOrEmpty(key))
				return false;

			if (!_infoData.ContainsKey(TextManager.standardLanguage))
				return false;

			return _infoData[TextManager.standardLanguage].ContainsKey(key);
		}

		public List<string> Search(string searchString)
		{
			List<string> result = new List<string>();

			Dictionary<string, string> standardData = _infoData[TextManager.standardLanguage];
			foreach (var item in standardData)
			{
				// filter out keys which are reserved for the framework
				if (item.Key[0] == '#')
					continue;

				if (item.Key.ToLower().Contains(searchString.ToLower()))
					result.Add(item.Key);
			}

			return result;
		}

		#endregion

		#region editor methods
		// ================================================================================
		//  editor methods
		// --------------------------------------------------------------------------------

		public void SetText(string key, string content, string languageId = "")
		{
			if (languageId == "")
				languageId = TextManager.currentLanguage;

			if (string.IsNullOrEmpty(languageId) || !TextManager.HasLanguage(languageId))
				return;

			if (string.IsNullOrEmpty(key))
				return;

			if (!_infoData.ContainsKey(languageId))
				AddLanguage(languageId);

			_infoData[languageId][key] = content;
			int languageIndex = _languages.IndexOf(languageId);
			_textData[languageIndex].Add(key, content);

			// set field in standard language so it can be found by HasKey
			if (languageId != TextManager.standardLanguage)
			{
				if (!HasLanguage(TextManager.standardLanguage))
                {
					AddLanguage(TextManager.standardLanguage);
				}

				if (!_infoData[TextManager.standardLanguage].ContainsKey(key))
				{
					_infoData[TextManager.standardLanguage][key] = "";
					languageIndex = _languages.IndexOf(TextManager.standardLanguage);
					_textData[languageIndex].Add(key, "");
				}
			}

			MarkAsChanged();
		}

		public void SetMultilanguageText(string key, List<KeyValuePair<string, string>> text)
		{
			foreach (var item in text)
			{
				if (!_infoData.ContainsKey(item.Key))
				{
					AddLanguage(item.Key);
				}

				int languageIndex = _languages.IndexOf(item.Key);
				_textData[languageIndex].Add(key, item.Value);
				_infoData[item.Key][key] = item.Value;
			}

			MarkAsChanged();
		}

		public void DeleteAllExceptThese(List<string> keys)
		{
			for (int i = 0; i < _languages.Count; i++)
			{
				List<string> deleteList = new List<string>();

				foreach (var item in _textData[i])
				{
					if (!keys.Contains(item.Key))
						deleteList.Add(item.Key);
				}

				foreach (var key in deleteList)
				{
					_textData[i].Remove(key);
					MarkAsChanged();
				}
			}

			BuildIndex();
		}

		public void BuildIndex()
		{
			_infoData.Clear();
			for (int i = 0; i < _languages.Count; i++)
			{
				Dictionary<string, string> languageEntries = new Dictionary<string, string>();

				foreach (var item in _textData[i])
				{
					languageEntries.Add(item.Key, item.Value);
				}

				_infoData[_languages[i]] = languageEntries;
			}
		}

		//public void RemoveKey(string key)
		//{
		//	if (string.IsNullOrEmpty(key))
		//		return;

		//	for (int i = 0; i < TextManager.GetLanguages().Count; i++)
		//	{
		//		var languageData = _data[TextManager.GetLanguages()[i]];
		//		if (languageData.ContainsKey(key))
		//		{
		//			languageData.Remove(key);
		//		}
		//	}

		//	MarkAsChanged();
		//}

		#endregion

		#region loading and saving
		// ================================================================================
		//  loading and saving
		// --------------------------------------------------------------------------------

		public void Clear()
		{
			_languages.Clear();
			_textData.Clear();
			_infoData.Clear();
		}

		//public void ImportJSON(JSONObject importData)
		//{
		//	Clear();

		//	if (importData != null)
		//	{
		//		foreach (var language in importData)
		//		{
		//			AddLanguage(language.Key);

		//			foreach (var item in language.Value.Obj)
		//			{
		//				_infoData[language.Key][item.Key] = item.Value.Str;
		//			}
		//		}
		//	}

		//	_contentWasModified = false;
		//}

		//public JSONObject ExportJSON()
		//{
		//	JSONObject data = new JSONObject();

		//	foreach (var language in _infoData)
		//	{
		//		JSONObject languageData = new JSONObject();

		//		foreach (var item in language.Value)
		//		{
		//			languageData.Add(item.Key, new JSONValue(item.Value.Trim()));
		//		}

		//		data.Add(language.Key, languageData);
		//	}

		//	return data;
		//}

		private bool HasLanguage(string languageId)
		{
			return _languages.Contains(languageId);
		}

		private void AddLanguage(string languageId)
		{
			if (!string.IsNullOrEmpty(languageId) && !_languages.Contains(languageId))
			{
				_infoData[languageId] = new Dictionary<string, string>();
				_languages.Add(languageId);
				_textData.Add(new LanguageText());
			}

			// MarkAsChanged will be called by caller of this method
		}

		private void MarkAsChanged()
		{
			_contentWasModified = true;
		}

		#endregion
	}
}
