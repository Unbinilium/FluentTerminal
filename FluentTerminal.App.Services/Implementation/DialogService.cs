﻿using FluentTerminal.App.Services.Dialogs;
using FluentTerminal.Models;
using System;
using System.Threading.Tasks;

namespace FluentTerminal.App.Services.Implementation
{
    public class DialogService : IDialogService
    {
        private readonly Func<IMessageDialog> _messageDialogFactory;
        private readonly Func<ICreateKeyBindingDialog> _createKeyBindingDialogFactory;
        private readonly Func<IInputDialog> _inputDialogFactory;
        private readonly Func<ISshConnectionInfoDialog> _sshConnectionInfoDialogFactory;
        private readonly Func<ICustomCommandDialog> _customCommandDialogFactory;
        private readonly Func<IAboutDialog> _aboutDialogFactory;

        public DialogService(Func<IMessageDialog> messageDialogFactory, Func<ICreateKeyBindingDialog> createKeyBindingDialogFactory,
            Func<IInputDialog> inputDialogFactory, Func<ISshConnectionInfoDialog> sshConnectionInfoDialogFactory,
            Func<ICustomCommandDialog> customCommandDialogFactory,
            Func<IAboutDialog> aboutDialogFactory)
        {
            _messageDialogFactory = messageDialogFactory;
            _createKeyBindingDialogFactory = createKeyBindingDialogFactory;
            _inputDialogFactory = inputDialogFactory;
            _sshConnectionInfoDialogFactory = sshConnectionInfoDialogFactory;
            _customCommandDialogFactory = customCommandDialogFactory;
            _aboutDialogFactory = aboutDialogFactory;
        }

        public Task<KeyBinding> ShowCreateKeyBindingDialog()
        {
            var dialog = _createKeyBindingDialogFactory();

            return dialog.CreateKeyBinding();
        }

        public Task<DialogButton> ShowMessageDialogAsync(string title, string content, params DialogButton[] buttons)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentNullException(nameof(title));
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (buttons.Length == 0)
            {
                throw new ArgumentException("Must not be empty", nameof(buttons));
            }

            var dialog = _messageDialogFactory();
            dialog.Content = content;
            dialog.Title = title;

            foreach (var button in buttons)
            {
                dialog.AddButton(button);
            }

            return dialog.ShowAsync();
        }

        public Task<string> ShowInputDialogAsync(string title)
        {
            var dialog = _inputDialogFactory.Invoke();
            dialog.SetTitle(title);

            return dialog.GetInput();
        }

        public Task<SshProfile> ShowSshConnectionInfoDialogAsync(SshProfile input = null) =>
            _sshConnectionInfoDialogFactory().GetSshConnectionInfoAsync(input);

        public Task<ShellProfile> ShowCustomCommandDialogAsync(ShellProfile input = null) =>
            _customCommandDialogFactory().GetCustomCommandAsync(input);

        public Task ShowAboutDialogAsync()
        {
            var dialog = _aboutDialogFactory();

            return dialog.ShowAsync();
        }
    }
}