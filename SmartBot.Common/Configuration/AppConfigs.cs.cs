using Microsoft.Extensions.Configuration;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;

namespace SmartBot.Common.Configuration
{
    public class AppConfigs
    {
        private static IConfiguration _configuration;
        public static void LoadAll(IConfiguration configuration)
        {
            _configuration = configuration;
            ApiUrlBase = GetConfigValue("Appconfig:ApiUrlBase", "");
            ApiAir = GetConfigValue("Appconfig:ApiAir", "");
            ItemPerPage = GetConfigValue("Appconfig:ItemPerPage", 50);
            SqlConnection = GetConfigValue("ConnectionStrings:SmartBotSqlConn", "No connection");
            GoogleClientId = GetConfigValue("Google:ClientId", "No value");
            GoogleClientSecret = GetConfigValue("Google:ClientSecret", "No value");
            FacebookClientId = GetConfigValue("Facebook:ClientId", "No value");
            FacebookClientSecret = GetConfigValue("Facebook:ClientSecret", "No value");
            SecretKey = GetConfigValue("Appconfig:SecretKey", "");
            AppVersion = GetConfigValue("Appconfig:AppVersion", "");
            Directorio = GetConfigValue("Appconfig:Directorio", "wwwroot\\upload\\");
            FullPath = GetConfigValue("Appconfig:FullPath", "wwwroot\\upload\\");
            RootPath = GetConfigValue("Appconfig:RootPath", "");
            NewsImagePath = GetConfigValue("Appconfig:NewsImagePath", "wwwroot\\shared\\UserFiles");
        }
        public static string SecretKey { get; set; }
        public static string FullPath { get; set; }
        public static string RootPath { get; set; }
        public static string NewsImagePath { get; set; }

        public static string CurrentUserCK = "CurrentUser";
        public static string CurrentUserAdmin = "CurrentUserAdmin";
        public static string FormatCurrency(string currencyCode, decimal amount)
        {
            CultureInfo culture = (from c in CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                                   let r = new RegionInfo(c.LCID)
                                   where r != null
                                   && r.ISOCurrencySymbol.ToUpper() == currencyCode.ToUpper()
                                   select c).FirstOrDefault();
            if (culture == null)
            {
                // fall back to current culture if none is found
                // you could throw an exception here if that's not supposed to happen
                culture = CultureInfo.CurrentCulture;

            }
            culture = (CultureInfo)culture.Clone();
            culture.NumberFormat.CurrencySymbol = currencyCode;
            culture.NumberFormat.CurrencyPositivePattern = culture.NumberFormat.CurrencyPositivePattern == 0 ? 2 : 3;
            var cnp = culture.NumberFormat.CurrencyNegativePattern;
            switch (cnp)
            {
                case 0: cnp = 14; break;
                case 1: cnp = 9; break;
                case 2: cnp = 12; break;
                case 3: cnp = 11; break;
                case 4: cnp = 15; break;
                case 5: cnp = 8; break;
                case 6: cnp = 13; break;
                case 7: cnp = 10; break;
            }
            culture.NumberFormat.CurrencyNegativePattern = cnp;

            return amount.ToString("C" + ((amount % 1) == 0 ? "0" : "2"), culture);
        }
        public static string FormatMoney(string unit, decimal amount)
        {
            CultureInfo culture = (from c in CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                                   let r = new RegionInfo(c.LCID)
                                   where r != null
                                   && r.ISOCurrencySymbol.ToUpper() == unit.ToUpper()
                                   select c).FirstOrDefault();
            if (culture == null)
            {
                // fall back to current culture if none is found
                // you could throw an exception here if that's not supposed to happen
                culture = CultureInfo.CurrentCulture;

            }
            culture = (CultureInfo)culture.Clone();
            culture.NumberFormat.CurrencySymbol = "";
            culture.NumberFormat.CurrencyPositivePattern = culture.NumberFormat.CurrencyPositivePattern == 0 ? 2 : 3;
            var cnp = culture.NumberFormat.CurrencyNegativePattern;
            switch (cnp)
            {
                case 0: cnp = 14; break;
                case 1: cnp = 9; break;
                case 2: cnp = 12; break;
                case 3: cnp = 11; break;
                case 4: cnp = 15; break;
                case 5: cnp = 8; break;
                case 6: cnp = 13; break;
                case 7: cnp = 10; break;
            }
            culture.NumberFormat.CurrencyNegativePattern = cnp;
            return amount.ToString("C" + ((amount % 1) == 0 ? "0" : "2"), culture)+$" {unit}";
        }
        public static string ApiUrlBase { get; set; }
        public static string ApiAir { get; set; }
        public static string AppVersion { get; set; }
        public static string Hotline = "0866.560.388";
        public static string Email = "info@sgogroup.com.vn";
        public static string Address = "Tầng 2, HPC Landmark 105, Tố Hữu, La Khê, Hà Đông, Hà Nội";
        public static string TitlePage = "SGO Land - SGO Group - Kết nối bất động sản";
        public static string descriptionPage = "SGO Land thuộc SGO Group là công ty chuyên kinh doanh, môi giới bất động sản từ các Chủ đầu tư lớn trên thị trường bất động sản Việt Nam.";
        public static string Directorio { get; set; }
        public static string GoogleClientId { get; set; }
        public static string GoogleClientSecret { get; set; }
        public static string FacebookClientId { get; set; }
        public static string FacebookClientSecret { get; set; }
        public static int ItemPerPage { get; set; }
        public static string SqlConnection { get; set; }
        /// <summary>
        /// plhd
        /// </summary>
        /// 

        public static int acvivemenu = 0;
        /// <summary>
        /// Lấy ra giá trị config trong file .config
        /// </summary>
        private static T GetConfigValue<T>(string configKey, T defaultValue)
        {
            var value = defaultValue;
            var converter = TypeDescriptor.GetConverter(typeof(T));
            try
            {
                if (converter != null)
                {
                    var setting = _configuration.GetSection(configKey).Value;
                    if (!string.IsNullOrEmpty(setting))
                    {
                        value = (T)converter.ConvertFromString(setting);
                    }
                }
            }
            catch
            {
                value = defaultValue;
            }
            return value;
        }

        public static string toUrl(string? OldString)
        {
            if (OldString == null)
                return "";
            else
            {
                string strNewString = "";
                OldString = Regex.Replace(OldString, @"[^\p{L}\p{M}\p{N}]+", "-");
                if (OldString.Length > 160)
                {
                    OldString = OldString.Substring(0, 160);
                }

                Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
                strNewString = OldString.ToLower().Normalize(NormalizationForm.FormD);
                strNewString = regex.Replace(strNewString, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D').Replace('\uD83D', '-');

                return strNewString;
            }

        }
        public static string replaceDate(DateTime inputDate)
        {
            DateTime startDate = DateTime.Now;
            TimeSpan t = startDate - inputDate;

            if (t.Days <= 0)
            {
                if (t.Hours < 1)
                {
                    if (t.Minutes <= 2)
                    {
                        return "Vừa xong";
                    }
                    else
                    {
                        return t.Minutes + " phút trước.";
                    }

                }
                else
                {
                    return t.Hours + " giờ trước.";
                }
            }
            else
            {
                return t.Days + " ngày trước.";
            }
        }
        public static string Activecss(int tab, int tabactive)
        {
            if (tab == tabactive)
            {
                return "active";
            }
            else
            {
                return "";
            }
        }
    }
}
