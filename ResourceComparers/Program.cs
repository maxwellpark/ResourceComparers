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

var reqiestResult = new GetResourceResult
{
    Fields = requestFields
};

var responseResult = new GetResourceResult
{
    Fields = responseFields
};
var comparer = new GetResourceResultComparer(new string[] { "id" });

reqiestResult.Fields = reqiestResult.Fields
    .Where(f => !(f.Value == null && !responseFields.ContainsKey(f.Key)))
    .ToDictionary(x => x.Key, x => x.Value);

var equal = comparer.Equals(reqiestResult, responseResult);
Console.WriteLine(equal);

Console.ReadKey();

class GetResourceResult
{
    public IDictionary<string, object> Fields { get; set; }
}

class GetResourceResultComparer : IEqualityComparer<GetResourceResult>
{
    public GetResourceResultComparer(IEnumerable<string> fieldsToIgnore)
    {
        FieldsToIgnore = fieldsToIgnore;
    }

    public IEnumerable<string> FieldsToIgnore { get; }

    public bool Equals(GetResourceResult? x, GetResourceResult? y)
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

    public int GetHashCode([DisallowNull] GetResourceResult obj)
    {
        throw new NotImplementedException();
    }
}