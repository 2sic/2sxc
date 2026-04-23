namespace ToSic.Sxc.Backend.SaveHelpers;

[PrivateApi]
public interface ISaveEntityValidator
{
    SaveEntityValidationResult Validate(SaveEntityValidationContext context);
}