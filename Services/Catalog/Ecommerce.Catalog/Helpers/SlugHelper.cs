using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Ecommerce.Catalog.Helpers
{
    
    
    
    
    public static class SlugHelper
    {
        
        
        
        private static readonly Dictionary<char, string> TurkishCharMap = new()
        {
            { 'ş', "s" }, { 'Ş', "s" },
            { 'ğ', "g" }, { 'Ğ', "g" },
            { 'ü', "u" }, { 'Ü', "u" },
            { 'ö', "o" }, { 'Ö', "o" },
            { 'ı', "i" }, { 'İ', "i" },
            { 'ç', "c" }, { 'Ç', "c" }
        };

        
        
        
        
        
        public static string GenerateSlug(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            
            var sb = new StringBuilder();
            foreach (var c in text)
            {
                if (TurkishCharMap.TryGetValue(c, out var replacement))
                    sb.Append(replacement);
                else
                    sb.Append(c);
            }

            
            var normalized = sb.ToString().Normalize(NormalizationForm.FormD);
            sb.Clear();
            foreach (var c in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(c);
                if (category != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            
            var slug = sb.ToString().ToLowerInvariant();

            
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", "-");
            slug = Regex.Replace(slug, @"-+", "-");

            
            slug = slug.Trim('-');

            return slug;
        }

        
        
        
        
        
        
        public static string GenerateUniqueSlug(string baseSlug, IEnumerable<string> existingSlugs)
        {
            if (string.IsNullOrEmpty(baseSlug))
                return Guid.NewGuid().ToString("N")[..8];

            var slugSet = new HashSet<string>(existingSlugs ?? Enumerable.Empty<string>());
            
            if (!slugSet.Contains(baseSlug))
                return baseSlug;

            int counter = 2;
            string uniqueSlug;
            do
            {
                uniqueSlug = $"{baseSlug}-{counter}";
                counter++;
            } while (slugSet.Contains(uniqueSlug));

            return uniqueSlug;
        }
    }
}
