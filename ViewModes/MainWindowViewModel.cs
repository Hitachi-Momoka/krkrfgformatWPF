using Prism.Mvvm;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Media;
using Li.Drawing;
using Li.Krkr.krkrfgformatWPF.Servises;
using Li.Krkr.krkrfgformatWPF.Models;

namespace Li.Krkr.krkrfgformatWPF.ViewModes
{
    public partial class MainWindowViewModel : BindableBase
    {
        #region Command
        public DelegateCommand ClearAllCommand { set; get; }
        public DelegateCommand ClearSelectedCommand { get; set; }
        //public DelegateCommand SideOnlyCommand { get; set; }
        public DelegateCommand HelpButtonCommand { set; get; }
        public DelegateCommand SelectRulePathCommand { get; set; }
        public DelegateCommand SelectSavePathcommand { get; set; }
        public DelegateCommand FormatSelectedCommand { get; set; }
        public DelegateCommand OpenSaveFloderCommand { get; set; }
        #endregion

        #region Mumber
        private bool canUpData;
        private bool messageBoxIsShow;
        private bool isClearing;
        
        private SelectedItemWithIndexModel _selectItemTmp;

        public SelectedItemWithIndexModel SelectItemTmp
        {
            get { return _selectItemTmp; }
            set
            {
                _selectItemTmp = value;
                if (string.IsNullOrEmpty(RulePath) && this.SelectItemTmp != null)
                {
                    RulePath = GetRulePath(SelectItemTmp.SelectedItem.ToString());
                    SavePath = CreatDefultSavePath(SelectItemTmp.SelectedItem.ToString());
                }
                UpDataAllSelectedItems(SelectItemTmp);
                base.RaisePropertyChanged();
            }
        }


        private ObservableCollection<SelectedItemWithIndexModel> _allSelectedItems;
        public ObservableCollection<SelectedItemWithIndexModel> AllSelectedItems
        {
            get => _allSelectedItems;
            set
            {
                _allSelectedItems = value;
                base.RaisePropertyChanged();
            }
        }
        private string _rulePath;

        public string RulePath
        {
            get { return _rulePath; }
            set
            {
                _rulePath = value;
                _ruleData = RuleDataServises.CreatFromFile(RulePath);
                base.RaisePropertyChanged();
            }
        }

        private string _savePath;

        public string SavePath
        {
            get { return _savePath; }
            set
            {
                _savePath = value;
                base.RaisePropertyChanged();
            }
        }

        private string _saveName;

        public string SaveName
        {
            get { return _saveName; }
            set
            {
                _saveName = value;
                base.RaisePropertyChanged();
            }
        }

        private bool _isSideOnly;
        public bool IsSideOnly
        {
            get { return _isSideOnly; }
            set
            {
                _isSideOnly = value;
                base.RaisePropertyChanged();
                this.CollectionChanged(null, null);//选择状态变化就引起一次合成刷新。
            }
        }
        private ImageSource _imageBoxSource;

        public ImageSource ImageBoxSource
        {
            get { return _imageBoxSource; }
            set
            {
                _imageBoxSource = value;
                base.RaisePropertyChanged();
            }
        }
        private RuleDataModel _ruleData;
        public RuleDataModel RuleData
        {
            get => this._ruleData;
            set
            {
                this._ruleData = value;
            }
        }
        #endregion

        public MainWindowViewModel()
        {
            this.Init();
        }
        private void Init()
        {
            canUpData = true;
            messageBoxIsShow = false;
            isClearing = false;

            _selectItemTmp = null;
            _rulePath = "";
            _isSideOnly = false;
            _savePath = "";
            _saveName = "";
            _ruleData = null;
            _imageBoxSource = null;

            this._allSelectedItems = new ObservableCollection<SelectedItemWithIndexModel>();
            this._allSelectedItems.CollectionChanged += this.CollectionChanged;
            
            this.SelectSavePathcommand = new DelegateCommand(this.EmptyMethod, () => false); //关闭按钮，不予以支持
            this.ClearSelectedCommand = new DelegateCommand(this.ClearSelected);
            this.ClearAllCommand = new DelegateCommand(this.ClearAll);
            this.FormatSelectedCommand = new DelegateCommand(this.FormatSelected);
            this.SelectRulePathCommand = new DelegateCommand(this.SelectRulePath);
            this.OpenSaveFloderCommand = new DelegateCommand(this.OpenSaveFloder);
        }

        
    }
}
