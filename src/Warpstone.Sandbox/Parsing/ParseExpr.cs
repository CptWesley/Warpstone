using System.Globalization;
using Warpstone;

namespace ClausewitzLsp.Core.Parsing;

/// <summary>
/// Base class for all other parse expressions.
/// </summary>
public abstract record ParseExpr
{
    /// <summary>
    /// An empty error set.
    /// </summary>
    protected static readonly IEnumerable<IParseError> EmptyErrors = Array.Empty<IParseError>();

    private bool computedErrors;
    private IEnumerable<IParseError> errors = null!;

    /// <summary>
    /// Gets the errors.
    /// </summary>
    public IEnumerable<IParseError> Errors
    {
        get
        {
            if (!computedErrors)
            {
                errors = GetErrors();
                computedErrors = true;
            }

            return errors;
        }
    }

    /// <summary>
    /// Gets the errors.
    /// </summary>
    /// <returns>The found errors.</returns>
    public virtual IEnumerable<IParseError> GetErrors()
        => EmptyErrors;
}

/// <summary>
/// Represents a parsed integer.
/// </summary>
/// <param name="Value">The value.</param>
public record ParseIntExpr(int Value) : ParseExpr
{
    /// <inheritdoc/>
    public override string ToString()
        => Value.ToString(CultureInfo.InvariantCulture);
}

/// <summary>
/// Represents a parsed floating point number.
/// </summary>
/// <param name="Value">The value.</param>
public record ParseFloatExpr(float Value) : ParseExpr
{
    /// <inheritdoc/>
    public override string ToString()
        => Value.ToString(CultureInfo.InvariantCulture);
}

/// <summary>
/// Represents a parsed boolean.
/// </summary>
/// <param name="Value">The value.</param>
public record ParseBoolExpr(bool Value) : ParseExpr
{
    /// <inheritdoc/>
    public override string ToString()
        => Value ? "yes" : "no";
}

/// <summary>
/// Represents a parsed string.
/// </summary>
/// <param name="Value">The value.</param>
public record ParseStringExpr(string Value) : ParseExpr
{
    /// <inheritdoc/>
    public override string ToString()
        => $"\"{Value}\"";
}

/// <summary>
/// Represents a parsed identifier.
/// </summary>
/// <param name="Name">The name.</param>
public record ParseIdentifierExpr(string Name) : ParseExpr
{
    /// <inheritdoc/>
    public override string ToString()
        => Name;
}

/// <summary>
/// Represents a parsed date.
/// </summary>
/// <param name="Year">The year of the date.</param>
/// <param name="Month">The month of the date.</param>
/// <param name="Day">The day of the date.</param>
public record ParseDateExpr(int Year, int Month, int Day) : ParseExpr
{
    /// <inheritdoc/>
    public override string ToString()
        => $"{Year}.{Month}.{Day}";
}

/// <summary>
/// Represents a parsed field.
/// </summary>
/// <param name="Key">The key of the field.</param>
/// <param name="Value">The value of the field.</param>
public record ParseFieldExpr(ParseOption<ParseExpr> Key, ParseOption<ParseExpr> Value) : ParseExpr
{
    /// <inheritdoc/>
    public override string ToString()
        => $"{Key} = {Value}";

    /// <inheritdoc/>
    public override IEnumerable<IParseError> GetErrors()
    {
        if (Key is ParseNone<ParseExpr> keyFail)
        {
            yield return keyFail.Error;
        }

        if (Value is ParseNone<ParseExpr> valueFail)
        {
            yield return valueFail.Error;
        }
        else if (Value is ParseSome<ParseExpr> valueSuccess)
        {
            foreach (IParseError error in valueSuccess.Result.Errors)
            {
                yield return error;
            }
        }
    }
}

/// <summary>
/// Represents a parsed field.
/// </summary>
/// <param name="Elements">The elements of the object.</param>
public record ParseObjectExpr(IList<ParseOption<ParseExpr>> Elements) : ParseExpr
{
    /// <inheritdoc/>
    public override string ToString()
        => $"{{\n{string.Join("\n", Elements.Select(x => x.ToString()))}\n}}";

    /// <inheritdoc/>
    public override IEnumerable<IParseError> GetErrors()
    {
        foreach (ParseOption<ParseExpr> option in Elements)
        {
            if (option is ParseNone<ParseExpr> optionFail)
            {
                yield return optionFail.Error;
            }
            else if (option is ParseSome<ParseExpr> optionSuccess)
            {
                foreach (IParseError error in optionSuccess.Result.Errors)
                {
                    yield return error;
                }
            }
        }
    }
}

/// <summary>
/// Represents a parsed file.
/// </summary>
/// <param name="FileName">The file name.</param>
/// <param name="Body">The file body.</param>
public record ParseFileExpr(string FileName, ParseOption<ParseObjectExpr> Body) : ParseExpr
{
    /// <inheritdoc/>
    public override string ToString()
        => $"# {FileName}\n{Body}";

    /// <inheritdoc/>
    public override IEnumerable<IParseError> GetErrors()
    {
        if (Body is ParseNone<ParseObjectExpr> optionFail)
        {
            yield return optionFail.Error;
        }
        else if (Body is ParseSome<ParseObjectExpr> optionSuccess)
        {
            foreach (IParseError error in optionSuccess.Result.Errors)
            {
                yield return error;
            }
        }
    }
}
