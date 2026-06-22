// Polyfills required to compile C# 9-12 features when targeting .NET Framework 4.8.
// These types are defined in .NET 5+ BCL; re-declaring them internally satisfies the compiler.

namespace System.Runtime.CompilerServices
{
    // C# 9: required for 'init' accessors and positional record types
    internal static class IsExternalInit { }

    // C# 11: required for the 'required' property modifier
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct |
        AttributeTargets.Field | AttributeTargets.Property,
        AllowMultiple = false, Inherited = false)]
    internal sealed class RequiredMemberAttribute : Attribute { }

    // C# 11: companion attribute emitted by the compiler alongside RequiredMemberAttribute
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
    internal sealed class CompilerFeatureRequiredAttribute : Attribute
    {
        public CompilerFeatureRequiredAttribute(string featureName)
            => FeatureName = featureName;
        public string FeatureName { get; }
        public bool IsOptional { get; init; }
    }
}

namespace System.Diagnostics.CodeAnalysis
{
    // C# 11: marks a constructor as setting all required members
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    internal sealed class SetsRequiredMembersAttribute : Attribute { }
}
