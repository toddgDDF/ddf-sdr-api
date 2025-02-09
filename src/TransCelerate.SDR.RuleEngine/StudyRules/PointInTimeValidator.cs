﻿using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using TransCelerate.SDR.Core.DTO;
using TransCelerate.SDR.Core.DTO.Study;
using TransCelerate.SDR.Core.Utilities.Helpers;
using TransCelerate.SDR.Core.Utilities.Common;

namespace TransCelerate.SDR.RuleEngine
{
    public class PointInTimeValidator : AbstractValidator<PointInTimeDTO>
    {
        /// <summary>
        /// Validator for PointInTime
        /// </summary>
        public PointInTimeValidator()
        {           
            RuleFor(x => x.type)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage(Constants.ValidationErrorMessage.PropertyMissingError)
                .NotEmpty().WithMessage(Constants.ValidationErrorMessage.PropertyEmptyError);
            RuleFor(x => x.subjectStatusGrouping)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage(Constants.ValidationErrorMessage.PropertyMissingError)
                .NotEmpty().WithMessage(Constants.ValidationErrorMessage.PropertyEmptyError);
            RuleFor(x => x.startDate)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage(Constants.ValidationErrorMessage.PropertyMissingError)
                .NotEmpty().WithMessage(Constants.ValidationErrorMessage.PropertyEmptyError)
                .Must(x=>DateValidationHelper.IsValid(x)).WithMessage(Constants.ValidationErrorMessage.ValidDateError);
            RuleFor(x => x.endDate)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage(Constants.ValidationErrorMessage.PropertyMissingError)
                .NotEmpty().WithMessage(Constants.ValidationErrorMessage.PropertyEmptyError)
                .Must(x => DateValidationHelper.IsValid(x)).WithMessage(Constants.ValidationErrorMessage.ValidDateError);
        }
    }
}
