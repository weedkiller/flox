﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using EI.RP.CoreServices.IoC.Autofac;
using EI.RP.CoreServices.Profiling;
using EI.RP.CoreServices.System;
using EI.RP.UiFlows.Core.Facade.CurrentView;
using EI.RP.UiFlows.Core.Facade.FlowResultResolver;
using EI.RP.UiFlows.Core.Facade.InitialView;
using EI.RP.UiFlows.Core.Facade.SetContainedView;
using EI.RP.UiFlows.Core.Facade.TriggerEventOnView;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Core.Infrastructure.DataSources.Repositories;
using EI.RP.UiFlows.Core.Infrastructure.DataSources.Repositories.Strategies;
using Microsoft.AspNetCore.Mvc;

namespace EI.RP.UiFlows.Core.Infrastructure.IoC
{
	public class UiFlowsCoreModule<TFlowTypesEnum> : BaseModule where TFlowTypesEnum : struct
	{
		private readonly ContextStoreStrategy _contextStoreStrategy;
		private readonly Func<Type, object> _getUninitializedObjectFactory;

		public UiFlowsCoreModule(ContextStoreStrategy contextStoreStrategy, Func<Type,object> getUninitializedObjectFactory)
		{
			_contextStoreStrategy = contextStoreStrategy;
			_getUninitializedObjectFactory = getUninitializedObjectFactory;
		}

		protected override void Load(ContainerBuilder builder)
		{
			RegisterContextRepository(builder);
			RegisterUiFlows(builder);
			RegisterFlowRequestHandlers(builder);
		}

		private void RegisterFlowRequestHandlers(ContainerBuilder builder)
		{
			builder.RegisterGeneric(typeof( FlowInitialViewRequestHandler < >)).AsImplementedInterfaces().WithInterfaceProfiling();
			builder.RegisterGeneric(typeof(FlowCurrentViewRequestHandler<>)).AsImplementedInterfaces().WithInterfaceProfiling();
			builder.RegisterGeneric(typeof(FlowTriggerEventOnViewHandler<>)).AsImplementedInterfaces().WithInterfaceProfiling();
			builder.RegisterGeneric(typeof(FlowResultResolverRequestHandler<>)).AsImplementedInterfaces().WithInterfaceProfiling();
			builder.RegisterGeneric(typeof(SetContainedViewRequestHandler<>)).AsImplementedInterfaces().WithInterfaceProfiling();
		}
		private void RegisterUiFlows(ContainerBuilder builder)
		{
			builder.RegisterType<UiFlow>().AsSelf();
			RegisterAppFlowScreens(builder);
			RegisterAppFlowTypes(builder);
		}

		private void RegisterAppFlowScreens(ContainerBuilder builder)
		{
			RegisterScreens();

			RegisterFlowInitializers();

			void RegisterFlowInitializers()
			{
				var initializers = new Dictionary<TFlowTypesEnum, Type>();
				foreach (var type in TypesFinder.Resolver
					.FindConcreteTypesOf<IUiFlowInitializationStep<TFlowTypesEnum>>(
						false))
				{
					var initStep =
						(IUiFlowInitializationStep<TFlowTypesEnum>)_getUninitializedObjectFactory(type);

					var flowType = initStep.InitializerOfFlowType;
					if (Convert.ToInt32(flowType) == 0)
						throw new Exception(
							$"Could not resolve flow type of {type.FullName}.{nameof(IUiFlowInitializationStep<TFlowTypesEnum>.InitializerOfFlowType)}.");
					initializers.Add(flowType, initStep.GetType());
				}

				foreach (var key in initializers.Keys)
					builder.RegisterType(initializers[key])
						.Keyed<IUiFlowInitializationStep>(key.ToString().ToLowerInvariant()).WithInterfaceProfiling();
			}

			void RegisterScreens()
			{
				var sb = new StringBuilder();
				var steps = new Dictionary<TFlowTypesEnum, HashSet<ScreenName>>();
				var screens = new Dictionary<TFlowTypesEnum, HashSet<Type>>();
				foreach (var type in TypesFinder.Resolver.FindConcreteTypesOf<IUiFlowScreen<TFlowTypesEnum>>(
					false))
				{
					var uiFlowScreen = (IUiFlowScreen<TFlowTypesEnum>)_getUninitializedObjectFactory(type);
					var screenStep = uiFlowScreen.ScreenStep;
					var flowType = uiFlowScreen.IncludedInFlowType;
					if (screenStep == null)
						sb.AppendLine($"Could not resolve {type.FullName}.{nameof(IUiFlowScreen.ScreenStep)}.");

					if (Convert.ToInt32(flowType) == 0)
						sb.AppendLine(
							$"Could not resolve {type.FullName}.{nameof(IUiFlowScreen<TFlowTypesEnum>.IncludedInFlowType)}.");

					if (!screens.ContainsKey(flowType)) screens.Add(flowType, new HashSet<Type>());

					screens[flowType].Add(type);

					if (!steps.ContainsKey(flowType)) steps.Add(flowType, new HashSet<ScreenName>());

					steps[flowType].Add(screenStep);
				}

				if (sb.Length > 0)
					throw new InvalidOperationException(
						$"The following properties must be resolvable dynamically.{Environment.NewLine}{sb}");

				foreach (var key in steps.Keys)
					builder.Register(c => steps[key])
						.Keyed<IEnumerable<ScreenName>>(key.ToString().ToLowerInvariant()).SingleInstance().WithInterfaceProfiling();

				foreach (var key in screens.Keys)
					builder.RegisterTypes(screens[key].ToArray())
						.Keyed<IUiFlowScreen>(key.ToString().ToLowerInvariant()).WithInterfaceProfiling();
			}
		}

		private void RegisterAppFlowTypes(ContainerBuilder builder)
		{
			var flowTypes = Enum.GetValues(typeof(TFlowTypesEnum)).Cast<TFlowTypesEnum>()
				.Where(x => (int) (x as object) > 0).ToArray();
			foreach (var eiFlowType in flowTypes)
				builder.Register(c =>
					{
						var result = c.Resolve<UiFlow>();
						result.FlowTypeId = eiFlowType.ToString();

						return result;
					})
					//TODO: as strict enum value
					.Keyed<IUiFlow>(eiFlowType.ToString().ToLowerInvariant()).InstancePerLifetimeScope().WithInterfaceProfiling();

			builder.Register(c=>new FlowNames(flowTypes.Select(x => x.ToString().ToLowerInvariant()))).AsSelf();
		}

		private void RegisterContextRepository(ContainerBuilder builder)
		{
			builder.RegisterAssemblyTypes(GetType().Assembly).Where(x =>
					!x.IsAssignableTo<IUiFlowContextRepository>() &&
					!x.IsAssignableTo<IInternalUiFlowContextRepository>())
				.AsImplementedInterfaces().WithInterfaceProfiling();

			builder.RegisterType<InMemoryUiFlowContextDataRepository>()
				.AsImplementedInterfaces()
				.SingleInstance().WithInterfaceProfiling();

			builder.RegisterType<InSessionUiFlowContextDataRepository>()
				.AsImplementedInterfaces()
				.InstancePerLifetimeScope().WithInterfaceProfiling();

			

			builder.Register(c =>
				{
					var uiFlowContextRepositories = c.Resolve<IEnumerable<IInternalUiFlowContextRepository>>();
					return new UiFlowContextDataRepositoryDecorator(
						uiFlowContextRepositories
							.Single(x => x.StoreStrategy == _contextStoreStrategy), c.Resolve<IProfiler>());
				})
				.InstancePerLifetimeScope()
				.AsImplementedInterfaces()
				.WithInterfaceProfiling();
		}
	}
}