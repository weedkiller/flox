﻿using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using EI.RP.NavigationPrototype.Flows.AppFlows.BlueFlow.FlowDefinitions;
using EI.RP.NavigationPrototype.Flows.AppFlows.BlueFlow.Steps.FillData;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;


namespace EI.RP.NavigationPrototype.Flows.AppFlows.BlueFlow.Steps
{
	public class StepCScreen : BlueFlowScreen
	{
		public static class StepEvent
		{
			public static readonly ScreenEvent Previous = new ScreenEvent(nameof(StepCScreen), "Previous");
			public static readonly ScreenEvent FlowTransitionCompleted = new ScreenEvent(nameof(StepCScreen), "FlowCompleted");
			public static readonly ScreenEvent Reset = new ScreenEvent(nameof(StepCScreen), "Reset");
		}
		public override ScreenName ScreenStep =>  BlueFlowScreenName.StepCScreen;
		public override string ViewPath { get; } = "StepC";

		protected override bool OnValidateTransitionAttempt(ScreenEvent transitionTrigger,
            IUiFlowContextData contextData, out string errorMessage)
        {
			errorMessage = null;
			return true;
		}

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			var prestart=contextData.GetStepData<FlowInitializer.StartScreenModel>(ScreenName.PreStart);
			

			return screenConfiguration.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(StepEvent.Reset, BlueFlowScreenName.Step0Screen)
				.OnEventNavigatesTo(StepEvent.FlowTransitionCompleted, BlueFlowScreenName.FlowCompletedScreen,()=>!prestart.MustReturnToCaller,"Not Called from another flow expecting result")
				.OnEventNavigatesTo(StepEvent.FlowTransitionCompleted, BlueFlowScreenName.EndAndReturnToCaller, () => prestart.MustReturnToCaller, "Called from another flow expecting result")
				.OnEventNavigatesTo(StepEvent.Previous, BlueFlowScreenName.FillDataStep_StepBScreen,
					() => !ByPassedStepAandB(), "Comes from B")
				.OnEventNavigatesTo(StepEvent.Previous, BlueFlowScreenName.Step0Screen, ByPassedStepAandB,
					"Comes from step0");

			bool ByPassedStepAandB()
			{
				var stepValue1 = contextData
					.GetStepData<InitialScreen.InitialScreenScreenModel>(BlueFlowScreenName.Step0Screen).StepValue1;
				return stepValue1 != null && stepValue1.StartsWith('a');
			}


		}

		protected override Task OnHandlingStepEvent(ScreenEvent triggeredEvent, IUiFlowContextData contextData)
		{
			if (triggeredEvent == StepEvent.Reset) contextData.Reset();
			return Task.CompletedTask;
		}
		
		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			return new StepCScreenScreenModel
			{
				FlowInputData = contextData.GetStepData<FlowInitializer.StartScreenModel>(ScreenName.PreStart).GreenFlowData,
				InitialValue = contextData
					.GetStepData<InitialScreen.InitialScreenScreenModel>(BlueFlowScreenName.Step0Screen).StepValue1,
				StepAValue = contextData
					.GetStepData<StepAScreen.StepAScreenScreenModel>(BlueFlowScreenName.FillDataStep_StepAScreen)
					?.StepAValue1,
				StepBValue = contextData
					.GetStepData<StepBScreen.StepBScreenScreenModel>(BlueFlowScreenName.FillDataStep_StepBScreen)
					?.StepBValue1
			};
		}
		protected override async Task<UiFlowScreenModel> OnRefreshStepDataAsync(IUiFlowContextData contextData,
			UiFlowScreenModel originalScreenModel, IDictionary<string, object> stepViewCustomizations = null)
		{
			var result = (StepCScreenScreenModel)originalScreenModel ;
			

			if (stepViewCustomizations != null)
			{
				
				result = (StepCScreenScreenModel)originalScreenModel.CloneDeep(); 
				result.SetFlowCustomizableValue(stepViewCustomizations,  x=>x.CustomizableViewDataSample);

			}
			result.FlowInputData = contextData.GetStepData<FlowInitializer.StartScreenModel>(ScreenName.PreStart).GreenFlowData;
			result.InitialValue = contextData
				.GetStepData<InitialScreen.InitialScreenScreenModel>(BlueFlowScreenName.Step0Screen).StepValue1;
			result.StepAValue = contextData
				.GetStepData<StepAScreen.StepAScreenScreenModel>(BlueFlowScreenName.FillDataStep_StepAScreen)
				?.StepAValue1;
			result.StepBValue = contextData
				.GetStepData<StepBScreen.StepBScreenScreenModel>(BlueFlowScreenName.FillDataStep_StepBScreen)
				?.StepBValue1;
			return result;
		}


		public class StepCScreenScreenModel : UiFlowScreenModel
		{
			public string InitialValue { get; set; }
			public string StepAValue { get; set; }
			public string StepBValue { get; set; }
			public dynamic FlowInputData { get; set; }

			public int CustomizableViewDataSample { get; set; }

			public override bool IsValidFor(ScreenName screenStep)
			{
				return screenStep == BlueFlowScreenName.StepCScreen;
			}

		}
	}
}