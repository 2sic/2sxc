using System.Reflection;

namespace ToSic.Sxc.Code.Internal.HotBuild;

internal class AssemblyAnalyzer
{
    public static List<string> TypeInformation(Assembly assembly)
    {
        if (assembly == null) return ["no assembly"];

        var result = new List<string>();

        foreach (var type in assembly.GetTypes().Where(t => t.FullName != null && !SkipTypeRoots.Any(prefix => t.FullName.StartsWith(prefix))))
        {
            // Get the name and all base classes
            // Log all base classes
            var typePath = type.FullName;
            var baseType = type.BaseType;
            while (baseType != null)
            {
                typePath = $"{typePath} > {baseType.Name}";
                baseType = baseType.BaseType;
            }
            result.Add($"Type: {typePath}");


            // Log all methods / properties
            var x = type.GetMethods();
            var y = type.GetMethods(BindingFlags.DeclaredOnly);
            var z = type.GetMethods(BindingFlags.Instance);
            var q = type.GetMethods(BindingFlags.Public);
            var a = type.GetMethods(OwnPropsOnly);


            result.AddRange(type.GetMethods(OwnPropsOnly).Where(m => !SkipMethods.Any(skip => m.Name.StartsWith(skip)))
                .Select(m => $"----- 🏃 Method: {m.Name}({GetParameterList(m.GetParameters())})"));

            result.AddRange(type.GetProperties(OwnPropsOnly)
                .Select(p => $"----- 🌈 Property: {p.Name} ({p.PropertyType})"));

            result.AddRange(type.GetFields(OwnPropsOnly)
                .Select(f => $"----- ⏹️ Field: {f.Name} ({f.FieldType})"));

            result.AddRange(type.GetEvents(OwnPropsOnly)
                .Select(evt => $"----- 🎉 Event: {evt.Name}"));

            result.AddRange(type.GetNestedTypes(OwnPropsOnly)
                .Select(nt => $"----- 🎯 Nested Type: {nt.FullName}"));

            result.AddRange(type.GetConstructors(OwnPropsOnly)
                .Select(c => $"----- 🚧 Constructor: {c.Name}({GetParameterList(c.GetParameters())})"));

            result.AddRange(type.GetInterfaces().Where(i => !(i.FullName ?? "").StartsWith("ToSic"))
                .Select(i => $"----- 🔌 Interface: {i.FullName}"));

            result.AddRange(type.GetCustomAttributes()
                .Select(a => $"----- 🟫 Attribute: [{a.GetType().FullName}]"));
        }
        return result;
    }

    private static string GetParameterList(IEnumerable<ParameterInfo> parameters) 
        => string.Join(", ", parameters.Select(parameter => $"{parameter.ParameterType} {parameter.Name}"));

    private const BindingFlags OwnPropsOnly = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;

    private static readonly string[] SkipTypeRoots = []; // ["Microsoft", "System"];
    private static readonly string[] SkipMethods = ["get_", "set_"];
}