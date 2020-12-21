﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;
using WalletWasabi.Fluent.ViewModels.AddWallet;
using WalletWasabi.Fluent.ViewModels.NavBar;
using WalletWasabi.Fluent.ViewModels.Search;
using WalletWasabi.Fluent.ViewModels.Wallets;
using WalletWasabi.Wallets;

namespace WalletWasabi.Fluent.ViewModels.Login
{
	public partial class FullScreenLoginViewModel : LoginViewModelBase
	{
		[AutoNotify] private NavBarItemViewModel? _selectedBottomItem;

		public FullScreenLoginViewModel(
			WalletManager walletManager,
			ObservableCollection<WalletViewModelBase> wallets,
			SearchPageViewModel actionCenter,
			AddWalletPageViewModel addWallet)
			: base(walletManager)
		{
			Wallets = wallets;
			ActionCenter = actionCenter;
			AddWallet = addWallet;

			NavigateToCommand = ReactiveCommand.Create<NavBarItemViewModel>(NavigateToExecute);
		}

		public ICommand NavigateToCommand { get; }

		public ObservableCollection<WalletViewModelBase> Wallets { get; }

		public SearchPageViewModel ActionCenter { get; }

		public AddWalletPageViewModel AddWallet { get; }

		private void NavigateToExecute(NavBarItemViewModel item)
		{
			Navigate().To(item);
		}

		protected override void OnNavigatedTo(bool inStack, CompositeDisposable disposable)
		{
			base.OnNavigatedTo(inStack, disposable);

			Observable
				.FromEventPattern(Wallets, nameof(Wallets.CollectionChanged))
				.Throttle(TimeSpan.FromMilliseconds(100))
				.ObserveOn(RxApp.MainThreadScheduler)
				.Subscribe(_ => SelectedWallet = Wallets.FirstOrDefault())
				.DisposeWith(disposable);
		}
	}
}