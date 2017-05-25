﻿using System;
using Autofac;
using FluentValidation;

namespace ArchitectNow.Web.Services
{
	class AutofacValidatorFactory : ValidatorFactoryBase
	{
		private readonly IComponentContext _componentContext;

		public AutofacValidatorFactory(IComponentContext componentContext)
		{
			_componentContext = componentContext;
		}
		public override IValidator CreateInstance(Type validatorType)
		{
			return (IValidator)_componentContext.ResolveOptional(validatorType);
		}
	}
}