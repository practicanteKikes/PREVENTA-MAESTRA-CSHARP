using MainProject.Services.CustomHelper.Models.FnRes;
using Microsoft.Extensions.Localization;
using System.Text.Json;
using System.Xml;

namespace MainProject.Services.CustomHelper
{
    public interface ICustomHelper
    {
        Dictionary<string, object> MergeArrays(Dictionary<string, object> array1, Dictionary<string, object> array2);

        JsonSerializerOptions getJsonSerializeOptions();

        ObjFnResCustomConverToInt32 CustomConvertToInt32(string? lsPropertyName, string? lsPropertyValue);

        (bool status, string message, List<DateTime> data) CustomConvertToDateTime(string? lsPropertyName, string? lsPropertyValue, int? li_line_number = null);

        double howManyHoursHavePassedSinceByDateTime(DateTime oldDateTime);
        int LineNumber([System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0);

        string? getMergedPropertyString(string ls_property_name, Dictionary<string, object> ldictionary_mergedResult);

        int? getMergedPropertyInt(string ls_property_name, Dictionary<string, object> ldictionary_mergedResult);

        long? getMergedPropertyLong(string ls_property_name, Dictionary<string, object> ldictionary_mergedResult);
        ulong? getMergedPropertyUlong(string ls_property_name, Dictionary<string, object> ldictionary_mergedResult);
        void consoleLog(object message);
        void errorLog(object message);

        void requestInLog(object message);
        void requestOutLog(object message);

        string? Trans(IStringLocalizer _localizer, string ls_property);
        string Substring(string ls_input, int li_start, int maxLength);
        JsonDocument CreateJsonObjReadonly(string ls_json = "");
        string RemoveLeadingZeros(string ls_nit);

        DateTime? GetLastCompiledProject(string ls_main_project_name);

        string ConvertJsonToXml(string ls_json);

        string XmlObjectToString<T>(T obj, bool enableIndentation = false);

        void CustomNumberDecimalSeparator(string ls_number_decimal_separator = ".");

        string CustomStringToDoubleWithLastTwoDecimal(string ls_numbers);

        string EncryptString(string ls_plain_ext, string ls_key);
        string DecryptString(string ls_cipher_text, string ls_key);
        string RemoveRootElement(string ls_xmlContent);
        string AddParameterToRequestQuery(string url, string param, string value);

        HttpContext GetCurrentHttpContext();
    }
}
