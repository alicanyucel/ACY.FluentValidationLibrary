namespace ACY.Validation;

public class ValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public List<string> Errors { get; } = new();
}

public class RuleBuilder<T>
{
    private readonly Func<T, object?> _propertyFunc;
    private readonly List<Func<object?, string?>> _rules = new();

    public RuleBuilder(Func<T, object?> propertyFunc)
    {
        _propertyFunc = propertyFunc;
    }

    public RuleBuilder<T> NotEmpty(string message = "Value cannot be empty.")
    {
        _rules.Add(value => string.IsNullOrEmpty(value?.ToString()) ? message : null);
        return this;
    }

    public RuleBuilder<T> GreaterThan(int min, string message = "Value too small.")
    {
        _rules.Add(value =>
        {
            if (value is int intVal && intVal <= min)
                return message;
            return null;
        });
        return this;
    }

    public string? Validate(T instance)
    {
        var value = _propertyFunc(instance);
        foreach (var rule in _rules)
        {
            var error = rule(value);
            if (error != null) return error;
        }
        return null;
    }
}

public abstract class AbstractValidator<T>
{
    private readonly List<RuleBuilder<T>> _rules = new();

    protected RuleBuilder<T> RuleFor(Func<T, object?> propertyFunc)
    {
        var rb = new RuleBuilder<T>(propertyFunc);
        _rules.Add(rb);
        return rb;
    }

    public ValidationResult Validate(T instance)
    {
        var result = new ValidationResult();
        foreach (var rule in _rules)
        {
            var error = rule.Validate(instance);
            if (error != null) result.Errors.Add(error);
        }
        return result;
    }
}

