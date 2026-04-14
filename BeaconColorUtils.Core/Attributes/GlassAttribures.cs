namespace BeaconColorUtils.Core.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class GlassIdAttribute(string id) : Attribute
{
    public string Id { get; } = id;
}

[AttributeUsage(AttributeTargets.Field)]
public class GlassPaneIdAttribute(string id) : Attribute
{
    public string Id { get; } = id;
}