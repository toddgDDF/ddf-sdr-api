﻿using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using TransCelerate.SDR.Core.DTO;
using TransCelerate.SDR.Core.DTO.Study;
using TransCelerate.SDR.Core.Utilities.Common;

namespace TransCelerate.SDR.RuleEngine
{
    public class StudyObjectivesValidator : AbstractValidator<StudyObjectiveDTO>
    {
        /// <summary>
        /// Validator for studyObjectives
        /// </summary>
        public StudyObjectivesValidator()
        {           
            RuleFor(x => x.level)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage(Constants.ValidationErrorMessage.PropertyMissingError)
                .NotEmpty().WithMessage(Constants.ValidationErrorMessage.PropertyEmptyError);            
        }
    }
}
