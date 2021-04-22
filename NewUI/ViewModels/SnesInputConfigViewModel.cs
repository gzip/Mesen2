﻿using Avalonia;
using Avalonia.Controls;
using Mesen.GUI.Config;
using Mesen.Localization;
using Mesen.Utilities;
using Mesen.Views;
using Mesen.Windows;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mesen.ViewModels
{
	public class SnesInputConfigViewModel : ViewModelBase
	{
		[Reactive] public SnesConfig Config { get; set; }
		
		[Reactive] public ControllerConfig Multitap1 { get; set; }
		[Reactive] public ControllerConfig Multitap2 { get; set; }
		[Reactive] public ControllerConfig Multitap3 { get; set; }
		[Reactive] public ControllerConfig Multitap4 { get; set; }

		[ObservableAsProperty] public bool HasMultitap { get; set; }
		[ObservableAsProperty] public string? Multitap1Label { get; set; }

		public ReactiveCommand<Button, Unit> SetupPlayer1 { get; }
		public ReactiveCommand<Button, Unit> SetupPlayer2 { get; }
		public ReactiveCommand<Button, Unit> SetupMultitap1 { get; }
		public ReactiveCommand<Button, Unit> SetupMultitap2 { get; }
		public ReactiveCommand<Button, Unit> SetupMultitap3 { get; }
		public ReactiveCommand<Button, Unit> SetupMultitap4 { get; }

		public Enum[] AvailableControllerTypesP12 => new Enum[] {
			ControllerType.None,
			ControllerType.SnesController,
			ControllerType.SnesMouse,
			ControllerType.SuperScope,
			ControllerType.Multitap,
		};

		public Enum[] AvailableControllerTypesP345 => new Enum[] {
			ControllerType.None,
			ControllerType.SnesController
		};

		//For designer preview
		public SnesInputConfigViewModel() : this(new SnesConfig()) { }

		public SnesInputConfigViewModel(SnesConfig config)
		{
			Config = config;
		
			this.Multitap1 = Config.Controllers[1];
			this.Multitap2 = Config.Controllers[2];
			this.Multitap3 = Config.Controllers[3];
			this.Multitap4 = Config.Controllers[4];

			this.WhenAnyValue(x => x.Config.Controllers[0].Type, x => x.Config.Controllers[1].Type)
				.Select(x => x.Item1 == ControllerType.Multitap || x.Item2 == ControllerType.Multitap)
				.ToPropertyEx(this, x => x.HasMultitap);

			this.WhenAnyValue(x => x.Config.Controllers[0].Type, x => x.Config.Controllers[1].Type)
				.Select(x => ResourceHelper.GetViewLabel(nameof(SnesInputConfigView), x.Item1 == ControllerType.Multitap ? "lblPlayer1" : "lblPlayer2"))
				.ToPropertyEx(this, x => x.Multitap1Label);

			this.WhenAnyValue(x => x.Config.Controllers[0].Type).Subscribe((t) => {
				if(t == ControllerType.Multitap) {
					this.Multitap1 = Config.Controllers[0];
					Config.Controllers[1].Type = ControllerType.SnesController;
				}
			});

			this.WhenAnyValue(x => x.Config.Controllers[1].Type).Subscribe((t) => {
				if(t == ControllerType.Multitap) {
					this.Multitap1 = Config.Controllers[1];
					Config.Controllers[0].Type = ControllerType.SnesController;
				}
			});

			IObservable<bool> button1Enabled = this.WhenAnyValue(x => x.Config.Controllers[0].Type, x => x.CanConfigure());
			this.SetupPlayer1 = ReactiveCommand.Create<Button>(btn => this.OpenSetup(btn, 0), button1Enabled);

			IObservable<bool> button2Enabled = this.WhenAnyValue(x => x.Config.Controllers[1].Type, x => x.CanConfigure());
			this.SetupPlayer2 = ReactiveCommand.Create<Button>(btn => this.OpenSetup(btn, 1), button2Enabled);

			this.SetupMultitap1 = ReactiveCommand.Create<Button>(btn => this.OpenMultitapSetup(btn, 0));
			this.SetupMultitap2 = ReactiveCommand.Create<Button>(btn => this.OpenMultitapSetup(btn, 1));
			this.SetupMultitap3 = ReactiveCommand.Create<Button>(btn => this.OpenMultitapSetup(btn, 2));
			this.SetupMultitap4 = ReactiveCommand.Create<Button>(btn => this.OpenMultitapSetup(btn, 3));
		}

		private async void OpenSetup(Button btn, int port)
		{
			PixelPoint startPosition = btn.PointToScreen(new Point(-7, btn.Height));
			ControllerConfigWindow wnd = new ControllerConfigWindow();
			wnd.WindowStartupLocation = WindowStartupLocation.Manual;
			wnd.Position = startPosition;

			ControllerConfig cfg = JsonHelper.Clone(this.Config.Controllers[port]);
			wnd.DataContext = new ControllerConfigViewModel(cfg);

			if(await wnd.ShowDialog<bool>(btn.Parent.VisualRoot as Window)) {
				this.Config.Controllers[port] = cfg;
			}
		}

		private void OpenMultitapSetup(Button btn, int port)
		{
			if(port > 0) {
				this.OpenSetup(btn, port + 1);
			} else {
				if(this.Config.Controllers[0].Type == ControllerType.Multitap) {
					this.OpenSetup(btn, 0);
				} else {
					this.OpenSetup(btn, 1);
				}
			}
		}
	}
}