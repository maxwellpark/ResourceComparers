using System.Diagnostics.CodeAnalysis;

// Request fields
var requestFields = new Dictionary<string, object>
{
    { "foo", "f" }, { "bar", "b" }, { "baz", "b" }
};

// Original state of resource  
var backupFields = new Dictionary<string, object>
{
    { "id", 1 }, { "foo", "g" }, { "bar", "c" }, { "baz", "a" }, { "quux", "q" }, { "corge", null }
};

// Response fields
var responseFields = new Dictionary<string, object>
{
    { "id", 1 }, { "foo", "f" }, { "bar", "b" }, { "baz", "b" }, { "quux", "q" }, { "corge", null }
};

var requestResult = new ResourceResult(requestFields);
var responseResult = new ResourceResult(responseFields);
var comparer = new ResourceResultComparer(new string[] { "id" });

//// Remove nulls (for /create)
//requestResult.Fields = requestResult.Fields
//    .Where(f => !(f.Value == null && !responseFields.ContainsKey(f.Key)))
//    .ToDictionary(x => x.Key, x => x.Value);

var fieldsToCompare = new Dictionary<string, object>();

foreach (var field in backupFields)
{
    var fieldToAdd = !requestFields.ContainsKey(field.Key)
        ? field
        : new KeyValuePair<string, object>(field.Key, requestFields[field.Key]);

    fieldsToCompare[fieldToAdd.Key] = fieldToAdd.Value;
}

var fieldsSelect = new Dictionary<string, object>(backupFields);
backupFields.ToList().ForEach(field =>
{
    if (requestFields.ContainsKey(field.Key))
        fieldsSelect[field.Key] = field.Value;
});

var equals = comparer.Equals(new ResourceResult(fieldsToCompare), responseResult);
Console.WriteLine(equals);

Console.ReadKey();

class ResourceResult
{
    public ResourceResult(IDictionary<string, object> fields)
    {
        Fields = fields;
    }

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