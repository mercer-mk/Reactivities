using System;
 using Application.Activities.Commands;
using Application.Activities.DTOs;
using Application.Validators;
using FluentValidation;
 
namespace Application.Validators;
 
 public class CreateActivityValidator : BaseActivityValidaor<CreateActivity.Command,CreateActivityDto>
 {
     public CreateActivityValidator(): base(x=> x.ActivityDto){

     }
 }