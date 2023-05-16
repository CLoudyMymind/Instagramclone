using System.ComponentModel.DataAnnotations;

namespace Instagram.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class AllowedExtensionsAttribute : ValidationAttribute
{
    private readonly string[] _extensions;

    public AllowedExtensionsAttribute(string[] extensions)
    {
        _extensions = extensions;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is IFormFile file)
        {
            var fileExtension = Path.GetExtension(file.FileName.ToLower());

            if (_extensions.All(e => e != fileExtension))
            {
                return new ValidationResult($"Недопустимые расширения файлов");
            }
            
        }
        return ValidationResult.Success;
    }
}