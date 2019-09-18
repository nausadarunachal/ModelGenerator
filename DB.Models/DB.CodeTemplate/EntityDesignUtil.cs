namespace DB.CodeTemplate
{
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure.Pluralization;

    public static class EntityDesignUtil
    {
        static EntityDesignUtil()
        {
            PluralizationService =
                new EnglishPluralizationService();
            KnownPlural =
                new Dictionary<string, string>();
            KnownSingular =
                new Dictionary<string, string>();
            SingularToPlural =
                new Dictionary<string, string>();
            PluralToSingular =
                new Dictionary<string, string>();
            var mappings = TemplateConstants
                .PluralizationMappings
                .Split(',');
            for (var i = 0; i < mappings.Length; i += 2)
            {
                var map0 = mappings[i].Trim();
                var map1 = mappings[i + 1].Trim();
                KnownPlural.Add(map1, map1);
                KnownSingular.Add(map0, map0);
                SingularToPlural.Add(map0, map1);
                PluralToSingular.Add(map1, map0);
            }
        }

        private static readonly Dictionary<string, string> KnownPlural;

        private static readonly Dictionary<string, string> KnownSingular;

        private static readonly IPluralizationService PluralizationService;

        private static readonly Dictionary<string, string> PluralToSingular;

        private static readonly Dictionary<string, string> SingularToPlural;

        public static string Pluralize(string word)
        {
            // if known plural, return unchanged
            if (KnownPlural.ContainsKey(word))
            {
                return word;
            }
            // if in singular-to-plural, return plural
            return SingularToPlural
                .TryGetValue(word, out var plural)
                ? plural
                : PluralizationService.Pluralize(word);
        }

        public static string Singularize(string word)
        {
            // if known singular, return unchanged
            if (KnownSingular.ContainsKey(word))
            {
                return word;
            }
            // if in plural-to-singular, return plural
            return PluralToSingular.TryGetValue(word, out var singular)
                ? singular
                : PluralizationService.Singularize(word);
        }
    }
}
