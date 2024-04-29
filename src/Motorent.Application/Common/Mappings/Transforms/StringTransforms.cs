using System.Linq.Expressions;

namespace Motorent.Application.Common.Mappings.Transforms;

internal static class StringTransforms
{
    public static readonly Expression<Func<string, string>> Trim = str =>
        string.IsNullOrEmpty(str) ? str : str.Trim();
}