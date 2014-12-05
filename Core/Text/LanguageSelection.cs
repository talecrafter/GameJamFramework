using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CraftingLegends.Core
{
    /// <summary>
    /// helper class which stores a list of language names and an index of the currently selected language
    /// </summary>
    public class LanguageSelection
    {
        public List<string> languages;
        public List<string> languageNames;

        private int _index = 0;
        public int index
        {
            get
            {
                return _index;
            }
            set
            {
                if (_index != value)
                {
                    TextManager.SetLanguage(languages[value]);
                }

                _index = value;
            }
        }

        // ================================================================================
        //  constructor and deconstructor
        // --------------------------------------------------------------------------------

        public LanguageSelection()
        {
            languages = TextManager.GetLanguages();

            languageNames = new List<string>();
            for (int i = 0; i < languages.Count; i++)
            {
                if (languages[i] == TextManager.standardLanguage)
                    languageNames.Add(languages[i] + " (Standard)");
                else
                    languageNames.Add(languages[i]);
            }
            UpdateIndex();

            TextManager.languageChanged += TextManager_languageChanged;
        }

        ~LanguageSelection()
        {
            TextManager.languageChanged -= TextManager_languageChanged;
        }

        // ================================================================================
        //  private methods
        // --------------------------------------------------------------------------------

        private void TextManager_languageChanged()
        {
            UpdateIndex();
        }

        private void UpdateIndex()
        {
            _index = TextManager.currentLanguageIndex;
        }
    }

}