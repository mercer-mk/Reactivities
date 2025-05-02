using System;
 using Application.Activities.Commands;
 using Application.Activities.DTOs;
using Application.Validators;
using FluentValidation;
 
 namespace Application.Validators;
 
 public class EditActivityValidator : BaseActivityValidaor<EditActivity.Command, EditActivityDto>
 {
     public EditActivityValidator() : base(x => x.ActivityDto)
     {
         RuleFor(x => x.ActivityDto.Id)
             .NotEmpty().WithMessage("Id is required");
     }
 }