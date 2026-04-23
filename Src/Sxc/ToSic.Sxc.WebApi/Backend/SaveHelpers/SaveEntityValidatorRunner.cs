namespace ToSic.Sxc.Backend.SaveHelpers;

[PrivateApi]
public class SaveEntityValidatorRunner : ServiceBase
{
    private readonly ISaveEntityValidator[] _validators;

    public SaveEntityValidatorRunner(IEnumerable<ISaveEntityValidator> validators)
        : this(validators.ToArray())
    { }

    private SaveEntityValidatorRunner(ISaveEntityValidator[] validators)
        : base("Sav.ValRun", connect: validators.Cast<object>().ToArray())
        => _validators = validators;

    internal void ValidateOrThrow(SaveEntityValidationContext context)
    {
        var l = Log.Fn($"index:{context.Index}, type:{context.Entity.Type.NameId}");

        foreach (var validator in _validators)
        {
            var result = validator.Validate(context);
            if (!result.IsValid)
                throw result.Exception!;
        }

        l.Done();
    }
}