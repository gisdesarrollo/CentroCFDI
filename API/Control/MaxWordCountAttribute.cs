using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Control
{
    public class MaxWordCountAttribute : ValidationAttribute
    {
        private readonly int _maxWords;

        public MaxWordCountAttribute(int maxWords)
        {
            _maxWords = maxWords;
            ErrorMessage = $"Se permiten solo {_maxWords} palabras.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var wordCount = value.ToString().Length;
                if (wordCount > _maxWords)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }
}
