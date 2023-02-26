//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core.Tools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class CountryTool
    {
        public const string NotFoundString = "None";
        private const string BASE_URL = "https://tokketcontent.blob.core.windows.net";

        private static IDictionary<string, string> _mappings = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase) {
            #region List of countries
            {"af", "Afghanistan"},
            {"al", "Albania"},
            {"dz", "Algeria"},
            {"ar", "Argentina"},
            {"am", "Armenia"},
            {"au", "Australia"},
            {"at", "Austria"},
            {"az", "Azerbaijan"},
            {"bh", "Bahrain"},
            {"bd", "Bangladesh"},
            {"by", "Belarus"},
            {"be", "Belgium"},
            {"bz", "Belize"},
            {"bt", "Bhutan"},
            {"bo", "Bolivia"},
            {"ba", "Bosnia and Herzegovina"},
            {"bw", "Botswana"},
            {"br", "Brazil"},
            {"bn", "Brunei"},
            {"bg", "Bulgaria"},
            {"kh", "Cambodia"},
            {"cm", "Cameroon"},
            {"ca", "Canada"},
            //{"029", "Caribbean"},
            {"cl", "Chile"},
            {"cn", "China"},
            {"co", "Colombia"},
            {"cd", "Congo (DRC)"},
            {"cr", "Costa Rica"},
            {"ci", "Côte d’Ivoire"},
            {"hr", "Croatia"},
            {"cu", "Cuba"},
            {"cz", "Czechia"},
            {"dk", "Denmark"},
            {"dj", "Djibouti"},
            {"do", "Dominican Republic"},
            {"ec", "Ecuador"},
            {"eg", "Egypt"},
            {"sv", "El Salvador"},
            {"er", "Eritrea"},
            {"ee", "Estonia"},
            {"et", "Ethiopia"},
            {"fo", "Faroe Islands"},
            {"fi", "Finland"},
            {"fr", "France"},
            {"ge", "Georgia"},
            {"de", "Germany"},
            {"gr", "Greece"},
            {"gl", "Greenland"},
            {"gt", "Guatemala"},
            {"ht", "Haiti"},
            {"hn", "Honduras"},
            //{"hk", "Hong Kong"},// SAR
            {"hu", "Hungary"},
            {"is", "Iceland"},
            {"in", "India"},
            {"id", "Indonesia"},
            {"ir", "Iran"},
            {"iq", "Iraq"},
            {"ie", "Ireland"},
            {"il", "Israel"},
            {"it", "Italy"},
            {"jm", "Jamaica"},
            {"jp", "Japan"},
            {"jo", "Jordan"},
            {"kz", "Kazakhstan"},
            {"ke", "Kenya"},
            {"kr", "Korea"},
            {"kw", "Kuwait"},
            {"kg", "Kyrgyzstan"},
            {"la", "Laos"},
            //{"419", "Latin America"},
            {"lv", "Latvia"},
            {"lb", "Lebanon"},
            {"ly", "Libya"},
            {"li", "Liechtenstein"},
            {"lt", "Lithuania"},
            {"lu", "Luxembourg"},
            //{"mo", "Macao"},// SAR
            {"mk", "Macedonia"},//, FYRO
            {"my", "Malaysia"},
            {"mv", "Maldives"},
            {"ml", "Mali"},
            {"mt", "Malta"},
            {"mx", "Mexico"},
            {"md", "Moldova"},
            {"mc", "Monaco"},
            {"mn", "Mongolia"},
            {"me", "Montenegro"},
            {"ma", "Morocco"},
            {"mm", "Myanmar"},
            {"np", "Nepal"},
            {"nl", "Netherlands"},
            {"nz", "New Zealand"},
            {"ni", "Nicaragua"},
            {"ng", "Nigeria"},
            {"no", "Norway"},
            {"om", "Oman"},
            {"pk", "Pakistan"},
            {"pa", "Panama"},
            {"py", "Paraguay"},
            {"pe", "Peru"},
            {"ph", "Philippines"},
            {"pl", "Poland"},
            {"pt", "Portugal"},
            {"pr", "Puerto Rico"},
            {"qa", "Qatar"},
            {"re", "Réunion"},
            {"ro", "Romania"},
            {"ru", "Russia"},
            {"rw", "Rwanda"},
            {"sa", "Saudi Arabia"},
            {"sn", "Senegal"},
            {"rs", "Serbia"},
            {"sg", "Singapore"},
            {"sk", "Slovakia"},
            {"si", "Slovenia"},
            {"so", "Somalia"},
            {"za", "South Africa"},
            {"es", "Spain"},
            {"lk", "Sri Lanka"},
            {"se", "Sweden"},
            {"ch", "Switzerland"},
            {"sy", "Syria"},
            {"tw", "Taiwan"},
            {"tj", "Tajikistan"},
            {"th", "Thailand"},
            {"tt", "Trinidad and Tobago"},
            {"tn", "Tunisia"},
            {"tr", "Turkey"},
            {"tm", "Turkmenistan"},
            {"ua", "Ukraine"},
            {"ae", "United Arab Emirates"},
            {"gb", "United Kingdom"},
            {"us", "United States"},
            {"uy", "Uruguay"},
            {"uz", "Uzbekistan"},
            {"ve", "Venezuela"},
            {"vn", "Vietnam"},
            //{"001", "World"},
            {"ye", "Yemen"},
            {"zw", "Zimbabwe"}
            #endregion
        };

        private static IDictionary<string, string> _stateMappings = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase) {
            #region List of states
            {"al", "Alabama"},
            {"ak", "Alaska"},
            {"az", "Arizona"},
            {"ar", "Arkansas"},
            {"ca", "California"},
            {"co", "Colorado"},
            {"ct", "Connecticut"},
            {"de", "Delaware"},
            {"fl", "Florida"},
            {"ga", "Georgia"},
            {"hi", "Hawaii"},
            {"id", "Idaho"},
            {"il", "Illinois"},
            {"in", "Indiana"},
            {"ia", "Iowa"},
            {"ks", "Kansas"},
            {"ky", "Kentucky"},
            {"la", "Louisiana"},
            {"me", "Maine"},
            {"md", "Maryland"},
            {"ma", "Massachusetts"},
            {"mi", "Michigan"},
            {"mn", "Minnesota"},
            {"ms", "Mississippi"},
            {"mo", "Missouri"},
            {"mt", "Montana"},
            {"ne", "Nebraska"},
            {"nv", "Nevada"},
            {"nh", "New Hampshire"},
            {"nj", "New Jersey"},
            {"nm", "New Mexico"},
            {"ny", "New York"},
            {"nc", "North Carolina"},
            {"nd", "North Dakota"},
            {"oh", "Ohio"},
            {"ok", "Oklahoma"},
            {"or", "Oregon"},
            {"pa", "Pennsylvania"},
            {"ri", "Rhode Island"},
            {"sc", "South Carolina"},
            {"sd", "South Dakota"},
            {"tn", "Tennessee"},
            {"tx", "Texas"},
            {"ut", "Utah"},
            {"vt", "Vermont"},
            {"va", "Virginia"},
            {"wa", "Washington"},
            {"wv", "West Virginia"},
            {"wi", "Wisconsin"},
            {"wy", "Wyoming"}
            #endregion
        };

        public static string GetCountryName(string isoCode)
        {
            if (isoCode == null)
            {
                throw new ArgumentNullException("isoCode");
            }

            string countryName;

            return _mappings.TryGetValue(isoCode, out countryName) ? countryName : NotFoundString;
        }

        #region Get Country Flag
        public static string GetCountryFlagJPG1x1(string isoCode)
        {
            if (isoCode == null)
                throw new ArgumentNullException("isoCode");
            return $"{BASE_URL}/flags1x1/{isoCode}.jpg";
        }

        public static string GetCountryFlagSVG1x1(string isoCode)
        {
            if (isoCode == null)
                throw new ArgumentNullException("isoCode");
            return $"{BASE_URL}/flags1x1/{isoCode}.svg";
        }

        public static string GetCountryFlagJPG4x3(string isoCode)
        {
            if (isoCode == null)
                throw new ArgumentNullException("isoCode");
            return $"{BASE_URL}/flags4x3/{isoCode}.jpg";
        }

        public static string GetCountryFlagSVG4x3(string isoCode)
        {
            if (isoCode == null)
                throw new ArgumentNullException("isoCode");
            return $"{BASE_URL}/flags4x3/{isoCode}.svg";
        }
        #endregion

        public static string GetCountryAbbreviation(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            var pairs = _stateMappings.Where(x => x.Value.Contains(name));
            if (pairs == null)
                return "None";
            else
                return pairs.FirstOrDefault().Key;
        }

        public static string GetStateName(string abbr)
        {
            if (string.IsNullOrEmpty(abbr))
                throw new ArgumentNullException("abbreviation");

            string stateName;
            return _mappings.TryGetValue(abbr, out stateName) ? stateName : NotFoundString;
        }

        #region Get State Flag
        public static string GetStateFlagJPG1x1(string abbr)
        {
            if (string.IsNullOrEmpty(abbr))
                throw new ArgumentNullException("abbreviation");
            return $"{BASE_URL}/stateflags1x1/{abbr}.jpg";
        }

        public static string GetStateFlagSVG1x1(string abbr)
        {
            if (string.IsNullOrEmpty(abbr))
                throw new ArgumentNullException("abbreviation");
            return $"{BASE_URL}/stateflags1x1/{abbr}.svg";
        }

        public static string GetStateFlagJPG4x3(string abbr)
        {
            if (string.IsNullOrEmpty(abbr))
                throw new ArgumentNullException("abbreviation");
            return $"{BASE_URL}/stateflags4x3/{abbr}.jpg";
        }

        public static string GetStateFlagSVG4x3(string abbr)
        {
            if (string.IsNullOrEmpty(abbr))
                throw new ArgumentNullException("abbreviation");
            return $"{BASE_URL}/stateflags4x3/{abbr}.svg";
        }
        #endregion

        public static string GetStateAbbreviation(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            var pairs = _stateMappings.Where(x => x.Value.Contains(name));
            if (pairs == null)
                return NotFoundString;
            else
                return pairs.FirstOrDefault().Key;
        }

        public static List<CountryModel> GetCountries()
        {
            List<CountryModel> countries = new List<CountryModel>();
            foreach (var map in _mappings)
            {
                countries.Add(new CountryModel() { Id = map.Key, Name = map.Value });
            }

            return countries;
        }

        public static List<StateModel> GetUnitedStates()
        {
            List<StateModel> states = new List<StateModel>();
            foreach (var map in _stateMappings)
            {
                states.Add(new StateModel() { Id = map.Key, Name = map.Value, Country = "United States" });
            }

            return states;
        }
    }

    public class CountryModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string State { get; set; }
    }

    public class StateModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
    }
}
