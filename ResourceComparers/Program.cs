using System.Diagnostics.CodeAnalysis;

// Request fields
var requestFields = new Dictionary<string, object>
{
    { "foo", "f" }, { "bar", "b" }, { "baz", "b" }
};

// Response fields
var responseFields = new Dictionary<string, object>
{
    { "id", 1 }, { "foo", "f" }, { "bar", "b" }, { "baz", "b" }, { "quux", null }, { "corge", null }
};

var requestResult = new ResourceResult
{
    Fields = requestFields
};

var responseResult = new ResourceResult
{
    Fields = responseFields
};
var comparer = new ResourceResultComparer(new string[] { "id" });

// Remove nulls (for /create)
requestResult.Fields = requestResult.Fields
    .Where(f => !(f.Value == null && !responseFields.ContainsKey(f.Key)))
    .ToDictionary(x => x.Key, x => x.Value);

var createReqResEquals = comparer.Equals(requestResult, responseResult);
Console.WriteLine(createReqResEquals);

Console.ReadKey();

class ResourceResult
{
    public IDictionary<string, object> Fields { get; set; }
}

class ResourceResultComparer : IEqualityComparer<ResourceResult>
{
    public ResourceResultComparer(IEnumerable<string> fieldsToIgnore)
    {
        FieldsToIgnore = fieldsToIgnore;
    }

    public IEnumerable<string> FieldsToIgnore { get; }

    public bool Equals(ResourceResult? x, ResourceResult? y)
    {
        var xUniqueKeys = x.Fields.Keys.Except(y.Fields.Keys);

        if (xUniqueKeys.Any(k => !FieldsToIgnore.Contains(k)))
            return false;

        var yUniqueKeys = y.Fields.Keys.Except(x.Fields.Keys);

        if (yUniqueKeys.Any(k => !FieldsToIgnore.Contains(k)))
            return false;

        foreach (var field in x.Fields)
        {
            if (FieldsToIgnore.Contains(field.Key))
                continue;

            if (!y.Fields.ContainsKey(field.Key) || field.Value != y.Fields[field.Key])
                return false;
        }
        return true;
    }

    public int GetHashCode([DisallowNull] ResourceResult obj)
    {
        throw new NotImplementedException();
    }
}