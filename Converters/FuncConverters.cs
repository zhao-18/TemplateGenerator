using Avalonia.Data.Converters;
using Microsoft.TemplateEngine.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateGenerator.Converters
{
    public static class FuncConverters
    {
        public static FuncValueConverter<ITemplateInfo?, string> LanguageGetter { get; } = new FuncValueConverter<ITemplateInfo?, string>(
            template =>
            {
                string? language = string.Empty;
                template?.TagsCollection.TryGetValue("language", out language);
                return language ?? string.Empty;
            }
        );

        public static FuncValueConverter<ITemplateInfo?, string> ShortNameGetter { get; } = new FuncValueConverter<ITemplateInfo?, string>(
            template => template?.ShortNameList.Count > 0 ? template.ShortNameList[0] : string.Empty
        );

        public static FuncValueConverter<ITemplateInfo?, bool> LanguageNullToVisibility { get; } = new FuncValueConverter<ITemplateInfo?, bool>(
            template =>
            {
                string? language = default;
                template?.TagsCollection.TryGetValue("language", out language);
                return language is not null;
            }
        );

        public static string? PathConverter(string? AvaloniaPath)
        {
            if (AvaloniaPath?.StartsWith("file:///") ?? false)
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    return AvaloniaPath.Substring(8);
                }
                return AvaloniaPath.Substring(7);
            }
            return AvaloniaPath;
        }
    }
}
