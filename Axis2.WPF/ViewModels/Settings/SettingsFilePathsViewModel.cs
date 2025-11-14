using Axis2.WPF.Mvvm;
using System.IO;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Text.Json.Serialization;
using System.Windows.Interop;
using System.Windows;
using System;
using Axis2.WPF.ViewModels;

namespace Axis2.WPF.ViewModels.Settings
{
    public class SettingsFilePathsViewModel : BindableBase
    {
        private string _artIdx;
        public string ArtIdx { get => _artIdx; set => SetProperty(ref _artIdx, value); }

        private string _artMul;
        public string ArtMul { get => _artMul; set => SetProperty(ref _artMul, value); }

        private string _animIdx;
        public string AnimIdx { get => _animIdx; set => SetProperty(ref _animIdx, value); }

        private string _animMul;
        public string AnimMul { get => _animMul; set => SetProperty(ref _animMul, value); }

        private string _anim2Idx;
        public string Anim2Idx { get => _anim2Idx; set => SetProperty(ref _anim2Idx, value); }

        private string _anim2Mul;
        public string Anim2Mul { get => _anim2Mul; set => SetProperty(ref _anim2Mul, value); }

        private string _anim3Idx;
        public string Anim3Idx { get => _anim3Idx; set => SetProperty(ref _anim3Idx, value); }

        private string _anim3Mul;
        public string Anim3Mul { get => _anim3Mul; set => SetProperty(ref _anim3Mul, value); }

        private string _anim4Idx;
        public string Anim4Idx { get => _anim4Idx; set => SetProperty(ref _anim4Idx, value); }

        private string _anim4Mul;
        public string Anim4Mul { get => _anim4Mul; set => SetProperty(ref _anim4Mul, value); }

        private string _anim5Idx;
        public string Anim5Idx { get => _anim5Idx; set => SetProperty(ref _anim5Idx, value); }

        private string _anim5Mul;
        public string Anim5Mul { get => _anim5Mul; set => SetProperty(ref _anim5Mul, value); }

        private string _anim6Idx;
        public string Anim6Idx { get => _anim6Idx; set => SetProperty(ref _anim6Idx, value); }

        private string _anim6Mul;
        public string Anim6Mul { get => _anim6Mul; set => SetProperty(ref _anim6Mul, value); }

        private string _huesMul;
        public string HuesMul { get => _huesMul; set => SetProperty(ref _huesMul, value); }

        private string _lightColorsTxt;
        public string LightColorsTxt { get => _lightColorsTxt; set => SetProperty(ref _lightColorsTxt, value); }

        private string _drawConfigTxt;
        public string DrawConfigTxt { get => _drawConfigTxt; set => SetProperty(ref _drawConfigTxt, value); }

        private string _scriptsPath;
        public string ScriptsPath { get => _scriptsPath; set => SetProperty(ref _scriptsPath, value); }

        private bool _samePathAsClient;
        private string _defaultClientPath;
        private string _defaultMulPath;

        public bool SamePathAsClient
        {
            get => _samePathAsClient;
            set
            {
                if (SetProperty(ref _samePathAsClient, value) && value)
                {
                    UpdatePathsFromClientPath();
                }
            }
        }

        public string DefaultClientPath
        {
            get => _defaultClientPath;
            set
            {
                if (SetProperty(ref _defaultClientPath, value) && SamePathAsClient)
                {
                    UpdatePathsFromClientPath();
                }
            }
        }

        public string DefaultMulPath
        {
            get => _defaultMulPath;
            set => SetProperty(ref _defaultMulPath, value);
        }

        [JsonIgnore]
        public ICommand BrowseClientPathCommand { get; }
        [JsonIgnore]
        public ICommand BrowseMulPathCommand { get; }
        [JsonIgnore]
        public ICommand BrowseScriptsPathCommand { get; }
        [JsonIgnore]
        public ICommand ResetPathsSettingsCommand { get; }

        public SettingsFilePathsViewModel()
        {
            // Initialize properties with default values
            SamePathAsClient = false;
            DefaultClientPath = "";
            DefaultMulPath = "";
            ScriptsPath = ""; // Should be set by the user

            UpdateMulPaths(DefaultMulPath);

            BrowseClientPathCommand = new RelayCommand(BrowseClientPath);
            BrowseMulPathCommand = new RelayCommand(BrowseMulPath);
            BrowseScriptsPathCommand = new RelayCommand(BrowseScriptsPath);
            ResetPathsSettingsCommand = new RelayCommand(ResetPathsSettings);
        }

        private void UpdatePathsFromClientPath()
        {
            if (!string.IsNullOrEmpty(DefaultClientPath) && File.Exists(DefaultClientPath))
            {
                DefaultMulPath = Path.GetDirectoryName(DefaultClientPath) + "\\";
                UpdateMulPaths(DefaultMulPath);
            }
        }

        private void UpdateMulPaths(string mulPath)
        {
            ArtIdx = Path.Combine(mulPath, "artidx.mul");
            ArtMul = Path.Combine(mulPath, "art.mul");
            HuesMul = Path.Combine(mulPath, "hues.mul");
            AnimIdx = Path.Combine(mulPath, "anim.idx");
            AnimMul = Path.Combine(mulPath, "anim.mul");
            Anim2Idx = Path.Combine(mulPath, "anim2.idx");
            Anim2Mul = Path.Combine(mulPath, "anim2.mul");
            Anim3Idx = Path.Combine(mulPath, "anim3.idx");
            Anim3Mul = Path.Combine(mulPath, "anim3.mul");
            Anim4Idx = Path.Combine(mulPath, "anim4.idx");
            Anim4Mul = Path.Combine(mulPath, "anim4.mul");
            Anim5Idx = Path.Combine(mulPath, "anim5.idx");
            Anim5Mul = Path.Combine(mulPath, "anim5.mul");
            Anim6Idx = Path.Combine(mulPath, "anim6.idx");
            Anim6Mul = Path.Combine(mulPath, "anim6.mul");

            // OrionData files
            // string orionDataPath = Path.Combine(mulPath, "OrionData"); // Removed duplicate declaration
            LightColorsTxt = Path.Combine(Path.Combine(mulPath, "OrionData"), "light_colors.txt");
            DrawConfigTxt = Path.Combine(Path.Combine(mulPath, "OrionData"), "draw_config.txt");

            // OrionData files
            string orionDataPath = Path.Combine(mulPath, "OrionData");
            // Ensure OrionData directory exists if needed, or handle its absence
            // For now, just combine paths
            LightColorsTxt = Path.Combine(orionDataPath, "light_colors.txt");
            DrawConfigTxt = Path.Combine(orionDataPath, "draw_config.txt");
        }

        private void BrowseClientPath()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                DefaultClientPath = openFileDialog.FileName;
            }
        }

        private void BrowseMulPath()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog(new Wpf32Window(System.Windows.Application.Current.MainWindow)) == DialogResult.OK)
            {
                DefaultMulPath = folderBrowserDialog.SelectedPath + "\\";
                UpdateMulPaths(DefaultMulPath);
            }
        }

        private void BrowseScriptsPath()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog(new Wpf32Window(System.Windows.Application.Current.MainWindow)) == DialogResult.OK)
            {
                ScriptsPath = folderBrowserDialog.SelectedPath;
            }
        }

        public void ResetPathsSettings()
        {
            SamePathAsClient = false;
            DefaultClientPath = "";
            DefaultMulPath = "";
            ScriptsPath = "";
            UpdateMulPaths(DefaultMulPath);
        }
    }
}